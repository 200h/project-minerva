#!/usr/bin/env bash

# Project Minerva Unity 5.6 import, compilation, and EditMode verification.

set -u

EXIT_USAGE=10
EXIT_PREFLIGHT=20
EXIT_IMPORT_PROCESS=30
EXIT_COMPILATION=31
EXIT_TEST_PROCESS=40
EXIT_XML_MISSING=41
EXIT_XML_INVALID=42
EXIT_TEST_FAILURE=43
EXIT_UNKNOWN_LOG_ERROR=50
EXIT_INTEGRITY=60
EXIT_CLEANUP=61
EXIT_REPORT=70

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="${MINERVA_PROJECT_ROOT:-$(cd "${SCRIPT_DIR}/../.." && pwd)}"
UNITY_EXECUTABLE="${MINERVA_UNITY56_PATH:-/Applications/Unity/Unity.app/Contents/MacOS/Unity}"
EVIDENCE_PARENT="${MINERVA_UNITY56_EVIDENCE_ROOT:-/private/tmp/project-minerva-unity}"

usage()
{
    cat <<'EOF'
Usage: verify-unity56-editmode.sh

The script validates the current clean Git worktree. Evidence is written outside
the repository. Configure paths with MINERVA_PROJECT_ROOT,
MINERVA_UNITY56_PATH, and MINERVA_UNITY56_EVIDENCE_ROOT.
EOF
}

case "${1:-}" in
    "")
        ;;
    -h|--help)
        usage
        exit 0
        ;;
    *)
        usage >&2
        exit "${EXIT_USAGE}"
        ;;
esac

mkdir -p "${EVIDENCE_PARENT}" || {
    printf 'FAIL: cannot create evidence root: %s\n' "${EVIDENCE_PARENT}" >&2
    exit "${EXIT_REPORT}"
}

EVIDENCE_DIR="$(mktemp -d "${EVIDENCE_PARENT}/verification.XXXXXX")" || {
    printf 'FAIL: cannot create unique evidence directory.\n' >&2
    exit "${EXIT_REPORT}"
}

PREFLIGHT_LOG="${EVIDENCE_DIR}/preflight.log"
IMPORT_LOG="${EVIDENCE_DIR}/import.log"
TEST_LOG="${EVIDENCE_DIR}/editmode.log"
RESULT_XML="${EVIDENCE_DIR}/editmode-results.xml"
SUMMARY="${EVIDENCE_DIR}/verification-summary.md"
XML_VALUES="${EVIDENCE_DIR}/xml-values.tsv"
UNKNOWN_ERRORS="${EVIDENCE_DIR}/unknown-errors.txt"
GENERATED_PATHS="${EVIDENCE_DIR}/generated-paths.txt"
INTEGRITY_FINDINGS="${EVIDENCE_DIR}/integrity-findings.txt"

FINAL_EXIT=0
FAILURE_REASON=""
IMPORT_EXIT="not-run"
TEST_EXIT="not-run"
IMPORT_RESULT="Not Run"
TEST_RESULT="Not Run"
TOTAL_TESTS="0"
PASSED_TESTS="0"
FAILED_TESTS="0"
SKIPPED_TESTS="0"
INCONCLUSIVE_TESTS="0"
ASSERTIONS="0"
KNOWN_SHADER_WARNINGS="0"
KNOWN_CALLBACK_WARNINGS="0"
KNOWN_INSTANCE_WARNINGS="0"
UNKNOWN_ERROR_COUNT="0"
REPOSITORY_RESTORED="No"
DIFF_CHECK_RESULT="Not Run"

set_failure()
{
    code="$1"
    shift
    if [ "${FINAL_EXIT}" -eq 0 ]; then
        FINAL_EXIT="${code}"
        FAILURE_REASON="$*"
    fi
}

write_summary()
{
    cat > "${SUMMARY}" <<EOF
# Unity Validation Summary

**Repository:** ${PROJECT_ROOT}
**Branch:** ${BRANCH:-unknown}
**Commit:** ${SHA:-unknown}
**Unity:** ${UNITY_VERSION:-unknown}
**Evidence Directory:** ${EVIDENCE_DIR}

## Import and Compilation

- Result: ${IMPORT_RESULT}
- Exit code: ${IMPORT_EXIT}

## EditMode Tests

- Result: ${TEST_RESULT}
- Unity exit code: ${TEST_EXIT}
- Total: ${TOTAL_TESTS}
- Passed: ${PASSED_TESTS}
- Failed: ${FAILED_TESTS}
- Skipped: ${SKIPPED_TESTS}
- Inconclusive: ${INCONCLUSIVE_TESTS}
- Assertions: ${ASSERTIONS}

## Warning Classification

- UnityShaderCompiler socket warnings: ${KNOWN_SHADER_WARNINGS}
- Callback-unregistration warnings: ${KNOWN_CALLBACK_WARNINGS}
- \`ms_Instance\` shutdown assertions: ${KNOWN_INSTANCE_WARNINGS}
- Unknown blocking errors or exceptions: ${UNKNOWN_ERROR_COUNT}

Known warnings are nonblocking only when the authoritative XML result passed.
Unknown errors and exceptions are never hidden.

## Repository Integrity

- Working tree restored: ${REPOSITORY_RESTORED}
- \`git diff --check\`: ${DIFF_CHECK_RESULT}
- Generated-path record: ${GENERATED_PATHS}
- Integrity findings: ${INTEGRITY_FINDINGS}

## Final Decision

- Exit code: ${FINAL_EXIT}
- Result: $([ "${FINAL_EXIT}" -eq 0 ] && printf 'VALIDATION PASSED' || printf 'VALIDATION FAILED')
- Reason: ${FAILURE_REASON:-All required checks passed.}

## Evidence

- Preflight: ${PREFLIGHT_LOG}
- Import log: ${IMPORT_LOG}
- Test log: ${TEST_LOG}
- Test results: ${RESULT_XML}
- Unknown-error extract: ${UNKNOWN_ERRORS}
EOF
}

printf 'Evidence directory: %s\n' "${EVIDENCE_DIR}"

MINERVA_PROJECT_ROOT="${PROJECT_ROOT}" \
    MINERVA_UNITY56_PATH="${UNITY_EXECUTABLE}" \
    "${SCRIPT_DIR}/unity56-preflight.sh" > "${PREFLIGHT_LOG}" 2>&1
PREFLIGHT_EXIT=$?
if [ "${PREFLIGHT_EXIT}" -ne 0 ]; then
    FINAL_EXIT="${EXIT_PREFLIGHT}"
    FAILURE_REASON="Preflight failed (preflight exit ${PREFLIGHT_EXIT})."
    BRANCH="unknown"
    SHA="unknown"
    UNITY_VERSION="unknown"
    write_summary
    cat "${PREFLIGHT_LOG}" >&2
    printf 'Evidence preserved: %s\n' "${EVIDENCE_DIR}" >&2
    exit "${FINAL_EXIT}"
fi

BRANCH="$(git -C "${PROJECT_ROOT}" symbolic-ref --quiet --short HEAD 2>/dev/null || printf 'DETACHED')"
SHA="$(git -C "${PROJECT_ROOT}" rev-parse HEAD)"
UNITY_VERSION="$(sed -n 's/^MINERVA_UNITY56_VERSION=//p' "${PREFLIGHT_LOG}" | tail -n 1)"

git -C "${PROJECT_ROOT}" ls-files -s > "${EVIDENCE_DIR}/tracked-index-before.txt"
git -C "${PROJECT_ROOT}" ls-files -s -- '*.meta' > "${EVIDENCE_DIR}/meta-index-before.txt"
git -C "${PROJECT_ROOT}" status --porcelain=v1 --untracked-files=all > "${EVIDENCE_DIR}/status-before.txt"

"${UNITY_EXECUTABLE}" \
    -batchmode \
    -quit \
    -projectPath "${PROJECT_ROOT}" \
    -logFile "${IMPORT_LOG}" >/dev/null 2>&1
IMPORT_EXIT=$?

if [ "${IMPORT_EXIT}" -ne 0 ]; then
    IMPORT_RESULT="Failed"
    set_failure "${EXIT_IMPORT_PROCESS}" "Unity import process exited ${IMPORT_EXIT}."
elif grep -Eiq 'error CS[0-9]+|compilation failed|scripts have compiler errors' "${IMPORT_LOG}"; then
    IMPORT_RESULT="Failed"
    grep -Ein 'error CS[0-9]+|compilation failed|scripts have compiler errors' "${IMPORT_LOG}" \
        > "${EVIDENCE_DIR}/compiler-errors.txt" || true
    set_failure "${EXIT_COMPILATION}" "C# compilation errors were detected."
else
    IMPORT_RESULT="Passed"
fi

if [ "${FINAL_EXIT}" -eq 0 ]; then
    "${UNITY_EXECUTABLE}" \
        -batchmode \
        -projectPath "${PROJECT_ROOT}" \
        -runTests \
        -testPlatform editmode \
        -testResults "${RESULT_XML}" \
        -logFile "${TEST_LOG}" >/dev/null 2>&1
    TEST_EXIT=$?

    if [ ! -f "${RESULT_XML}" ]; then
        TEST_RESULT="Failed"
        set_failure "${EXIT_XML_MISSING}" "Unity did not create the required EditMode results XML."
    else
        if ! python3 - "${RESULT_XML}" "${XML_VALUES}" <<'PY'
import sys
import xml.etree.ElementTree as ET

source, output = sys.argv[1], sys.argv[2]
root = ET.parse(source).getroot()

def value(*names):
    for name in names:
        if name in root.attrib:
            return root.attrib[name]
    return ""

result = value("result")
fields = [
    result,
    value("total", "testcasecount"),
    value("passed", "passedcount"),
    value("failed", "failedcount"),
    value("skipped", "skippedcount"),
    value("inconclusive", "inconclusivecount"),
    value("asserts"),
]

if not result or any(item == "" for item in fields[1:]):
    raise ValueError("required NUnit result attributes are missing")

for item in fields[1:]:
    int(item)

with open(output, "w") as handle:
    handle.write("\t".join(fields) + "\n")
PY
        then
            TEST_RESULT="Failed"
            set_failure "${EXIT_XML_INVALID}" "EditMode results XML is missing required attributes or is invalid."
        else
            IFS="$(printf '\t')" read -r TEST_RESULT TOTAL_TESTS PASSED_TESTS FAILED_TESTS \
                SKIPPED_TESTS INCONCLUSIVE_TESTS ASSERTIONS < "${XML_VALUES}"

            if [ "${TEST_RESULT}" != "Passed" ] ||
                [ "${FAILED_TESTS}" -ne 0 ] ||
                [ "${INCONCLUSIVE_TESTS}" -ne 0 ] ||
                [ "${PASSED_TESTS}" -ne "${TOTAL_TESTS}" ]; then
                set_failure "${EXIT_TEST_FAILURE}" "Authoritative EditMode XML did not report a complete passing suite."
            elif [ "${TEST_EXIT}" -ne 0 ]; then
                set_failure "${EXIT_TEST_PROCESS}" "Unity exited ${TEST_EXIT} despite a passing XML result; manual review is required."
            fi
        fi
    fi
fi

if [ -f "${IMPORT_LOG}" ] || [ -f "${TEST_LOG}" ]; then
    KNOWN_SHADER_WARNINGS="$(
        grep -Ehc 'Failed to get socket connection from UnityShaderCompiler shader compiler' \
            "${IMPORT_LOG}" "${TEST_LOG}" 2>/dev/null | awk '{ total += $1 } END { print total + 0 }'
    )"
    KNOWN_CALLBACK_WARNINGS="$(
        grep -Ehc 'Callback unregistration failed\. Callback not registered\.' \
            "${IMPORT_LOG}" "${TEST_LOG}" 2>/dev/null | awk '{ total += $1 } END { print total + 0 }'
    )"
    KNOWN_INSTANCE_WARNINGS="$(
        grep -Ehc "Assertion failed on expression: 'ms_Instance != NULL'" \
            "${IMPORT_LOG}" "${TEST_LOG}" 2>/dev/null | awk '{ total += $1 } END { print total + 0 }'
    )"

    python3 - "${IMPORT_LOG}" "${TEST_LOG}" "${UNKNOWN_ERRORS}" <<'PY'
import re
import sys

known = (
    "Failed to get socket connection from UnityShaderCompiler shader compiler",
    "Callback unregistration failed. Callback not registered.",
    "Assertion failed on expression: 'ms_Instance != NULL'",
)
patterns = (
    re.compile(r"error CS\d+", re.I),
    re.compile(r"compilation failed", re.I),
    re.compile(r"scripts have compiler errors", re.I),
    re.compile(r"\bUnhandled Exception\b", re.I),
    re.compile(r"^\s*[A-Za-z0-9_.]+Exception\s*:", re.I),
    re.compile(r"^\s*Assertion failed on expression:", re.I),
)

findings = []
for path in sys.argv[1:3]:
    try:
        with open(path, "r", errors="replace") as handle:
            for number, line in enumerate(handle, 1):
                text = line.rstrip("\n")
                if any(item in text for item in known):
                    continue
                if any(pattern.search(text) for pattern in patterns):
                    findings.append("{}:{}:{}".format(path, number, text))
    except OSError:
        continue

with open(sys.argv[3], "w") as handle:
    for finding in findings:
        handle.write(finding + "\n")
PY
    UNKNOWN_ERROR_COUNT="$(wc -l < "${UNKNOWN_ERRORS}" | tr -d ' ')"
    if [ "${UNKNOWN_ERROR_COUNT}" -gt 0 ]; then
        set_failure "${EXIT_UNKNOWN_LOG_ERROR}" "Unknown blocking errors, exceptions, or assertions were detected."
    fi
else
    : > "${UNKNOWN_ERRORS}"
fi

python3 - "${PROJECT_ROOT}" "${GENERATED_PATHS}" "${INTEGRITY_FINDINGS}" <<'PY'
import os
import shutil
import subprocess
import sys

root, generated_output, findings_output = sys.argv[1:4]
raw = subprocess.check_output(
    ["git", "-C", root, "status", "--porcelain=v1", "--untracked-files=all", "-z"]
)
entries = raw.decode("utf-8", "surrogateescape").split("\0")
untracked = []
tracked_changes = []
unexpected = []
allowed = []

for entry in entries:
    if not entry:
        continue
    status = entry[:2]
    path = entry[3:]
    if status != "??":
        tracked_changes.append(entry)
        continue
    untracked.append(path)
    absolute = os.path.join(root, path)
    if path == "ProjectSettings/" or path.startswith("ProjectSettings/"):
        allowed.append(path)
    elif path == "Assets/Minerva.meta" and os.path.isdir(os.path.join(root, "Assets/Minerva")):
        allowed.append(path)
    elif path.startswith("Assets/Minerva/") and path.endswith(".meta"):
        paired = absolute[:-5]
        if os.path.isdir(paired):
            allowed.append(path)
        else:
            unexpected.append(path)
    else:
        unexpected.append(path)

with open(generated_output, "w") as handle:
    for path in untracked:
        classification = "ALLOWLISTED" if path in allowed else "UNEXPECTED"
        handle.write("{}\t{}\n".format(classification, path))

findings = []
if tracked_changes:
    findings.append("Tracked changes detected:")
    findings.extend(tracked_changes)
if unexpected:
    findings.append("Unexpected untracked paths preserved:")
    findings.extend(unexpected)

if not findings:
    for path in sorted(allowed, key=lambda item: len(item), reverse=True):
        absolute = os.path.join(root, path.rstrip("/"))
        if os.path.isdir(absolute):
            shutil.rmtree(absolute)
        elif os.path.lexists(absolute):
            os.unlink(absolute)

with open(findings_output, "w") as handle:
    for finding in findings:
        handle.write(finding + "\n")

sys.exit(1 if findings else 0)
PY
CLEANUP_EXIT=$?

if [ "${CLEANUP_EXIT}" -ne 0 ]; then
    set_failure "${EXIT_INTEGRITY}" "Repository changes include tracked or unexpected untracked paths; they were preserved."
fi

git -C "${PROJECT_ROOT}" ls-files -s > "${EVIDENCE_DIR}/tracked-index-after.txt"
git -C "${PROJECT_ROOT}" ls-files -s -- '*.meta' > "${EVIDENCE_DIR}/meta-index-after.txt"
git -C "${PROJECT_ROOT}" status --porcelain=v1 --untracked-files=all > "${EVIDENCE_DIR}/status-after.txt"

if ! cmp -s "${EVIDENCE_DIR}/tracked-index-before.txt" "${EVIDENCE_DIR}/tracked-index-after.txt"; then
    printf 'Tracked index state changed.\n' >> "${INTEGRITY_FINDINGS}"
    set_failure "${EXIT_INTEGRITY}" "Tracked index state changed during validation."
fi

if ! cmp -s "${EVIDENCE_DIR}/meta-index-before.txt" "${EVIDENCE_DIR}/meta-index-after.txt"; then
    printf 'Committed .meta index state changed.\n' >> "${INTEGRITY_FINDINGS}"
    set_failure "${EXIT_INTEGRITY}" "Committed .meta state changed during validation."
fi

if [ -s "${EVIDENCE_DIR}/status-after.txt" ]; then
    printf 'Working tree was not restored:\n' >> "${INTEGRITY_FINDINGS}"
    cat "${EVIDENCE_DIR}/status-after.txt" >> "${INTEGRITY_FINDINGS}"
    set_failure "${EXIT_CLEANUP}" "Working tree was not restored to clean state."
else
    REPOSITORY_RESTORED="Yes"
fi

if git -C "${PROJECT_ROOT}" diff --check > "${EVIDENCE_DIR}/diff-check.txt" 2>&1; then
    DIFF_CHECK_RESULT="Passed"
else
    DIFF_CHECK_RESULT="Failed"
    set_failure "${EXIT_INTEGRITY}" "git diff --check failed."
fi

write_summary || {
    printf 'FAIL: unable to write evidence report.\n' >&2
    exit "${EXIT_REPORT}"
}

printf '\nUnity validation\n'
printf '%s\n' '----------------'
printf 'Import/compilation: %s (exit %s)\n' "${IMPORT_RESULT}" "${IMPORT_EXIT}"
printf 'EditMode result:    %s (exit %s)\n' "${TEST_RESULT}" "${TEST_EXIT}"
printf 'Tests:              %s/%s passed\n' "${PASSED_TESTS}" "${TOTAL_TESTS}"
printf 'Assertions:         %s\n' "${ASSERTIONS}"
printf 'Failed:             %s\n' "${FAILED_TESTS}"
printf 'Skipped:            %s\n' "${SKIPPED_TESTS}"
printf 'Inconclusive:       %s\n' "${INCONCLUSIVE_TESTS}"
printf 'Repository restored:%s\n' " ${REPOSITORY_RESTORED}"
printf 'Evidence report:    %s\n' "${SUMMARY}"

if [ "${FINAL_EXIT}" -ne 0 ]; then
    printf '\nVALIDATION FAILED (%s): %s\n' "${FINAL_EXIT}" "${FAILURE_REASON}" >&2
    printf 'Evidence preserved: %s\n' "${EVIDENCE_DIR}" >&2
    exit "${FINAL_EXIT}"
fi

printf '\nVALIDATION PASSED\n'
exit 0
