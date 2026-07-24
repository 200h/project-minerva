#!/usr/bin/env bash

# Project Minerva Unity 5.6 local-verification preflight.
# This script must not modify the repository.

set -u

EXIT_USAGE=10
EXIT_PLATFORM=20
EXIT_UNITY=21
EXIT_REPOSITORY=22
EXIT_DIRTY=23
EXIT_PROJECT_IN_USE=24
EXIT_DISK=25
EXIT_TEST_RUNNER=26
EXIT_PYTHON=27
EXIT_UNITY_LAUNCH=28
EXIT_REPOSITORY_CHANGED=29

DEFAULT_UNITY_PATH="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
MINIMUM_DISK_KB_DEFAULT=2097152

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DEFAULT_PROJECT_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"
PROJECT_ROOT="${MINERVA_PROJECT_ROOT:-${DEFAULT_PROJECT_ROOT}}"
UNITY_EXECUTABLE="${MINERVA_UNITY56_PATH:-${DEFAULT_UNITY_PATH}}"
MINIMUM_DISK_KB="${MINERVA_UNITY56_MIN_DISK_KB:-${MINIMUM_DISK_KB_DEFAULT}}"

fail()
{
    code="$1"
    shift
    printf 'FAIL: %s\n' "$*" >&2
    exit "${code}"
}

pass()
{
    printf '%-24s PASS  %s\n' "$1" "$2"
}

usage()
{
    cat <<'EOF'
Usage: unity56-preflight.sh

Environment overrides:
  MINERVA_PROJECT_ROOT           Repository root (default: script-relative root)
  MINERVA_UNITY56_PATH           Unity executable
  MINERVA_UNITY56_MIN_DISK_KB    Required free disk space in KiB
  MINERVA_UNITY56_VERSION        Explicit version for controlled test harnesses
  MINERVA_ALLOW_NON_MACOS_TESTS  Set to 1 only for mocked shell-level tests
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

printf 'Unity 5.6 Preflight\n'
printf '%s\n' '-------------------'

OS_NAME="$(uname -s 2>/dev/null || true)"
if [ "${OS_NAME}" != "Darwin" ] && [ "${MINERVA_ALLOW_NON_MACOS_TESTS:-0}" != "1" ]; then
    fail "${EXIT_PLATFORM}" "macOS is required; detected '${OS_NAME:-unknown}'."
fi
OS_NOTE="${OS_NAME}"
if [ "${MINERVA_ALLOW_NON_MACOS_TESTS:-0}" = "1" ]; then
    OS_NOTE="${OS_NOTE} (test override allowed)"
fi
pass "Operating system" "${OS_NOTE}"

if [ ! -x "${UNITY_EXECUTABLE}" ]; then
    fail "${EXIT_UNITY}" "Unity executable is missing or not executable: ${UNITY_EXECUTABLE}"
fi
pass "Unity executable" "${UNITY_EXECUTABLE}"

if ! GIT_ROOT="$(git -C "${PROJECT_ROOT}" rev-parse --show-toplevel 2>/dev/null)"; then
    fail "${EXIT_REPOSITORY}" "Not a Git worktree: ${PROJECT_ROOT}"
fi
PROJECT_ROOT="$(cd "${GIT_ROOT}" && pwd)"
pass "Repository" "${PROJECT_ROOT}"

BEFORE_STATUS="$(git -C "${PROJECT_ROOT}" status --porcelain=v1 --untracked-files=all)"
if [ -n "${BEFORE_STATUS}" ]; then
    printf '%s\n' "${BEFORE_STATUS}" >&2
    fail "${EXIT_DIRTY}" "working tree must be clean before Unity validation."
fi
pass "Working tree" "clean"

BRANCH="$(git -C "${PROJECT_ROOT}" symbolic-ref --quiet --short HEAD 2>/dev/null || printf 'DETACHED')"
SHA="$(git -C "${PROJECT_ROOT}" rev-parse HEAD 2>/dev/null)" ||
    fail "${EXIT_REPOSITORY}" "Unable to resolve the current commit."
pass "Branch" "${BRANCH}"
pass "Commit" "${SHA}"

if [ -e "${PROJECT_ROOT}/Temp/UnityLockfile" ]; then
    fail "${EXIT_PROJECT_IN_USE}" "Unity project lock exists: ${PROJECT_ROOT}/Temp/UnityLockfile"
fi

if command -v lsof >/dev/null 2>&1; then
    if lsof +D "${PROJECT_ROOT}" 2>/dev/null | awk 'NR > 1 && /Unity/ { found = 1 } END { exit found ? 0 : 1 }'; then
        fail "${EXIT_PROJECT_IN_USE}" "A Unity process has files open inside this project."
    fi
fi
pass "Project lock" "absent"

case "${MINIMUM_DISK_KB}" in
    *[!0-9]*|"")
        fail "${EXIT_USAGE}" "MINERVA_UNITY56_MIN_DISK_KB must be a positive integer."
        ;;
esac

AVAILABLE_DISK_KB="$(df -Pk "${PROJECT_ROOT}" | awk 'NR == 2 { print $4 }')"
if [ -z "${AVAILABLE_DISK_KB}" ] || [ "${AVAILABLE_DISK_KB}" -lt "${MINIMUM_DISK_KB}" ]; then
    fail "${EXIT_DISK}" "Insufficient free space: ${AVAILABLE_DISK_KB:-unknown} KiB available; ${MINIMUM_DISK_KB} KiB required."
fi
pass "Disk space" "${AVAILABLE_DISK_KB} KiB available"

UNITY_CONTENTS="$(cd "$(dirname "${UNITY_EXECUTABLE}")/.." && pwd)"
TEST_RUNNER_EDITOR="${UNITY_CONTENTS}/UnityExtensions/Unity/TestRunner/Editor/UnityEditor.TestRunner.dll"
TEST_RUNNER_ENGINE="${UNITY_CONTENTS}/UnityExtensions/Unity/TestRunner/UnityEngine.TestRunner.dll"
if [ ! -f "${TEST_RUNNER_EDITOR}" ] || [ ! -f "${TEST_RUNNER_ENGINE}" ]; then
    fail "${EXIT_TEST_RUNNER}" "Unity Test Runner extension was not found below ${UNITY_CONTENTS}."
fi
pass "Test Runner" "found"

if ! command -v python3 >/dev/null 2>&1; then
    fail "${EXIT_PYTHON}" "python3 is required for XML parsing and safe path classification."
fi
if ! python3 -c 'import xml.etree.ElementTree' >/dev/null 2>&1; then
    fail "${EXIT_PYTHON}" "python3 cannot import xml.etree.ElementTree."
fi
pass "Python XML support" "$(command -v python3)"

UNITY_VERSION="${MINERVA_UNITY56_VERSION:-}"
if [ -z "${UNITY_VERSION}" ] && [ -x /usr/libexec/PlistBuddy ]; then
    INFO_PLIST="${UNITY_CONTENTS}/Info.plist"
    if [ -f "${INFO_PLIST}" ]; then
        UNITY_VERSION="$(/usr/libexec/PlistBuddy -c 'Print :CFBundleShortVersionString' "${INFO_PLIST}" 2>/dev/null || true)"
    fi
fi

LAUNCH_LOG="$(mktemp "${TMPDIR:-/private/tmp}/minerva-unity56-preflight.XXXXXX")" ||
    fail "${EXIT_UNITY_LAUNCH}" "Unable to create a temporary launch log."
SCRATCH_PROJECT="$(mktemp -d "${TMPDIR:-/private/tmp}/minerva-unity56-project.XXXXXX")" ||
    fail "${EXIT_UNITY_LAUNCH}" "Unable to create a temporary scratch project."

"${UNITY_EXECUTABLE}" \
    -batchmode \
    -quit \
    -createProject "${SCRATCH_PROJECT}" \
    -logFile "${LAUNCH_LOG}" >/dev/null 2>&1
UNITY_LAUNCH_EXIT=$?
rm -rf "${SCRATCH_PROJECT}"

if [ -z "${UNITY_VERSION}" ]; then
    UNITY_VERSION="$(sed -n 's/.*Unity version: *//p' "${LAUNCH_LOG}" | head -n 1)"
fi

if [ "${UNITY_LAUNCH_EXIT}" -ne 0 ]; then
    printf 'Unity launch log: %s\n' "${LAUNCH_LOG}" >&2
    fail "${EXIT_UNITY_LAUNCH}" "Unity launch-only batch check exited ${UNITY_LAUNCH_EXIT}."
fi

case "${UNITY_VERSION}" in
    5.6.7f1*)
        ;;
    *)
        printf 'Unity launch log: %s\n' "${LAUNCH_LOG}" >&2
        fail "${EXIT_UNITY}" "Unity 5.6.7f1 is required; detected '${UNITY_VERSION:-unknown}'."
        ;;
esac
pass "Unity version" "${UNITY_VERSION}"

AFTER_STATUS="$(git -C "${PROJECT_ROOT}" status --porcelain=v1 --untracked-files=all)"
if [ "${AFTER_STATUS}" != "${BEFORE_STATUS}" ]; then
    printf 'Before:\n%s\nAfter:\n%s\n' "${BEFORE_STATUS}" "${AFTER_STATUS}" >&2
    fail "${EXIT_REPOSITORY_CHANGED}" "preflight changed repository state."
fi
pass "Preflight integrity" "repository unchanged"

printf '\nREADY\n'
printf 'MINERVA_PROJECT_ROOT=%s\n' "${PROJECT_ROOT}"
printf 'MINERVA_UNITY56_PATH=%s\n' "${UNITY_EXECUTABLE}"
printf 'MINERVA_UNITY56_VERSION=%s\n' "${UNITY_VERSION}"
printf 'MINERVA_BRANCH=%s\n' "${BRANCH}"
printf 'MINERVA_COMMIT=%s\n' "${SHA}"
