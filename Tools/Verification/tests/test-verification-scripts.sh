#!/usr/bin/env bash

# Shell-level safety tests for the Unity 5.6 verification workflow.

set -u

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
VERIFICATION_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
TEST_ROOT="$(mktemp -d "${TMPDIR:-/private/tmp}/minerva-verification-tests.XXXXXX")" || exit 1
REPOSITORY="${TEST_ROOT}/repository"
FAKE_UNITY_APP="${TEST_ROOT}/Unity.app"
FAKE_UNITY="${FAKE_UNITY_APP}/Contents/MacOS/Unity"
EVIDENCE_ROOT="${TEST_ROOT}/evidence"
FAILURES=0

cleanup()
{
    rm -rf "${TEST_ROOT}"
}
trap cleanup EXIT INT TERM

record_failure()
{
    printf 'FAIL: %s\n' "$*" >&2
    FAILURES=$((FAILURES + 1))
}

expect_exit()
{
    expected="$1"
    shift
    "$@"
    actual=$?
    if [ "${actual}" -ne "${expected}" ]; then
        record_failure "expected exit ${expected}, received ${actual}: $*"
    fi
}

mkdir -p \
    "${REPOSITORY}/Assets/Minerva/Runtime/Core" \
    "${FAKE_UNITY_APP}/Contents/MacOS" \
    "${FAKE_UNITY_APP}/Contents/UnityExtensions/Unity/TestRunner/Editor" \
    "${FAKE_UNITY_APP}/Contents/UnityExtensions/Unity/TestRunner"

printf 'tracked\n' > "${REPOSITORY}/Assets/Minerva/Runtime/Core/Tracked.txt"
printf 'runner\n' > "${FAKE_UNITY_APP}/Contents/UnityExtensions/Unity/TestRunner/Editor/UnityEditor.TestRunner.dll"
printf 'runner\n' > "${FAKE_UNITY_APP}/Contents/UnityExtensions/Unity/TestRunner/UnityEngine.TestRunner.dll"

cat > "${FAKE_UNITY}" <<'EOF'
#!/usr/bin/env bash

set -u

LOG_FILE=""
RESULT_FILE=""
PROJECT_PATH=""
RUN_TESTS=0

while [ "$#" -gt 0 ]; do
    case "$1" in
        -logFile)
            LOG_FILE="$2"
            shift 2
            ;;
        -testResults)
            RESULT_FILE="$2"
            shift 2
            ;;
        -projectPath)
            PROJECT_PATH="$2"
            shift 2
            ;;
        -runTests)
            RUN_TESTS=1
            shift
            ;;
        *)
            shift
            ;;
    esac
done

if [ -n "${LOG_FILE}" ]; then
    mkdir -p "$(dirname "${LOG_FILE}")"
    printf 'Unity version: 5.6.7f1\n' > "${LOG_FILE}"
fi

if [ -n "${PROJECT_PATH}" ] && [ "${RUN_TESTS}" -eq 0 ]; then
    printf 'folder meta\n' > "${PROJECT_PATH}/Assets/Minerva.meta"
    mkdir -p "${PROJECT_PATH}/ProjectSettings"
    printf 'generated\n' > "${PROJECT_PATH}/ProjectSettings/ProjectVersion.txt"
fi

if [ "${RUN_TESTS}" -eq 1 ]; then
    {
        printf 'Failed to get socket connection from UnityShaderCompiler shader compiler!\n'
        printf 'Callback unregistration failed. Callback not registered.\n'
        printf "Assertion failed on expression: 'ms_Instance != NULL'\n"
    } >> "${LOG_FILE}"

    if [ "${MINERVA_MOCK_MODE:-pass}" != "missing_xml" ]; then
        mkdir -p "$(dirname "${RESULT_FILE}")"
        cat > "${RESULT_FILE}" <<'XML'
<test-run result="Passed" total="3" testcasecount="3" passed="3" failed="0" skipped="0" inconclusive="0" asserts="7" />
XML
    fi
fi

if [ -n "${PROJECT_PATH}" ] && [ "${MINERVA_MOCK_MODE:-pass}" = "unexpected" ]; then
    printf 'preserve me\n' > "${PROJECT_PATH}/unexpected-generated.txt"
fi

if [ -n "${PROJECT_PATH}" ] && [ "${MINERVA_MOCK_MODE:-pass}" = "unknown_project_settings" ]; then
    mkdir -p "${PROJECT_PATH}/ProjectSettings"
    printf 'preserve me\n' > "${PROJECT_PATH}/ProjectSettings/do-not-delete.txt"
fi

if [ "${RUN_TESTS}" -eq 1 ] && [ "${MINERVA_MOCK_MODE:-pass}" = "unknown_error" ]; then
    printf 'Error: synthetic native failure\n' >> "${LOG_FILE}"
fi

exit 0
EOF
chmod +x "${FAKE_UNITY}"

git -C "${REPOSITORY}" init -q
git -C "${REPOSITORY}" config user.name "Project Minerva Test"
git -C "${REPOSITORY}" config user.email "test@example.invalid"
git -C "${REPOSITORY}" add Assets
git -C "${REPOSITORY}" commit -qm "test fixture"

COMMON_ENV="
MINERVA_PROJECT_ROOT=${REPOSITORY}
MINERVA_UNITY56_PATH=${FAKE_UNITY}
MINERVA_UNITY56_VERSION=5.6.7f1
MINERVA_UNITY56_EVIDENCE_ROOT=${EVIDENCE_ROOT}
MINERVA_ALLOW_NON_MACOS_TESTS=1
"

env ${COMMON_ENV} "${VERIFICATION_DIR}/unity56-preflight.sh" > "${TEST_ROOT}/preflight.out" 2>&1
if [ "$?" -ne 0 ]; then
    record_failure "passing preflight failed"
fi
if [ -n "$(git -C "${REPOSITORY}" status --porcelain=v1 --untracked-files=all)" ]; then
    record_failure "preflight mutated the clean repository"
fi

env ${COMMON_ENV} MINERVA_MOCK_MODE=pass \
    "${VERIFICATION_DIR}/verify-unity56-editmode.sh" > "${TEST_ROOT}/pass.out" 2>&1
if [ "$?" -ne 0 ]; then
    record_failure "passing verification failed"
fi
if [ -n "$(git -C "${REPOSITORY}" status --porcelain=v1 --untracked-files=all)" ]; then
    record_failure "passing verification did not restore the repository"
fi
if ! grep -Rqs 'VALIDATION PASSED' "${EVIDENCE_ROOT}"; then
    record_failure "passing verification did not emit a passing evidence report"
fi
if ! grep -Rqs -- '- Assertions: 7' "${EVIDENCE_ROOT}"; then
    record_failure "passing evidence did not contain assertion totals"
fi

expect_exit 41 env ${COMMON_ENV} MINERVA_MOCK_MODE=missing_xml \
    "${VERIFICATION_DIR}/verify-unity56-editmode.sh"
if [ -n "$(git -C "${REPOSITORY}" status --porcelain=v1 --untracked-files=all)" ]; then
    record_failure "missing-XML verification did not restore the repository"
fi
if ! grep -Rqs 'Unity did not create the required EditMode results XML' "${EVIDENCE_ROOT}"; then
    record_failure "missing-XML evidence did not preserve the failure reason"
fi

expect_exit 60 env ${COMMON_ENV} MINERVA_MOCK_MODE=unexpected \
    "${VERIFICATION_DIR}/verify-unity56-editmode.sh"
if [ ! -f "${REPOSITORY}/unexpected-generated.txt" ]; then
    record_failure "unexpected generated content was deleted"
fi
rm -f "${REPOSITORY}/unexpected-generated.txt"
rm -f "${REPOSITORY}/Assets/Minerva.meta"
rm -rf "${REPOSITORY}/ProjectSettings"

expect_exit 60 env ${COMMON_ENV} MINERVA_MOCK_MODE=unknown_project_settings \
    "${VERIFICATION_DIR}/verify-unity56-editmode.sh"
if [ ! -f "${REPOSITORY}/ProjectSettings/do-not-delete.txt" ]; then
    record_failure "unknown ProjectSettings content was deleted"
fi
rm -f "${REPOSITORY}/Assets/Minerva.meta"
rm -rf "${REPOSITORY}/ProjectSettings"

expect_exit 50 env ${COMMON_ENV} MINERVA_MOCK_MODE=unknown_error \
    "${VERIFICATION_DIR}/verify-unity56-editmode.sh"
if [ -n "$(git -C "${REPOSITORY}" status --porcelain=v1 --untracked-files=all)" ]; then
    record_failure "unknown-error verification did not restore allowlisted generated content"
fi
if ! grep -Rqs 'Error: synthetic native failure' "${EVIDENCE_ROOT}"; then
    record_failure "unknown-error evidence did not preserve the blocking log line"
fi

WORKTREE_OUTPUT="$(
    MINERVA_PROJECT_ROOT="${REPOSITORY}" \
        MINERVA_UNITY56_WORKTREE_ROOT="${TEST_ROOT}/worktrees" \
        "${VERIFICATION_DIR}/create-unity-worktree.sh" HEAD
)"
WORKTREE_PATH="$(printf '%s\n' "${WORKTREE_OUTPUT}" | sed -n 's/^WORKTREE_PATH=//p')"
if [ -z "${WORKTREE_PATH}" ] || [ ! -d "${WORKTREE_PATH}" ]; then
    record_failure "worktree helper did not create a worktree"
else
    MINERVA_PROJECT_ROOT="${REPOSITORY}" \
        "${VERIFICATION_DIR}/remove-unity-worktree.sh" "${WORKTREE_PATH}" \
        > "${TEST_ROOT}/remove-worktree.out" 2>&1
    if [ "$?" -ne 0 ] || [ -e "${WORKTREE_PATH}" ]; then
        cat "${TEST_ROOT}/remove-worktree.out" >&2
        record_failure "worktree helper did not remove the clean worktree"
    fi
fi

if [ -n "$(git -C "${REPOSITORY}" status --porcelain=v1 --untracked-files=all)" ]; then
    record_failure "test fixture repository is not clean after all scenarios"
fi

if [ "${FAILURES}" -ne 0 ]; then
    printf '\n%d verification-script test(s) failed.\n' "${FAILURES}" >&2
    exit 1
fi

printf 'All Unity verification shell tests passed.\n'
