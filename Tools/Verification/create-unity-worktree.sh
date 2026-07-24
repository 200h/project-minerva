#!/usr/bin/env bash

# Create a detached disposable worktree for local Unity verification.

set -u

EXIT_USAGE=10
EXIT_REPOSITORY=20
EXIT_REF=21
EXIT_DESTINATION=22
EXIT_CREATE=23

usage()
{
    cat <<'EOF'
Usage: create-unity-worktree.sh <git-ref> [destination]

When destination is omitted, a unique directory is created under
/private/tmp/project-minerva-worktrees. The ref is resolved to a commit and the
worktree is detached so verification cannot move a branch unexpectedly.
EOF
}

if [ "$#" -lt 1 ] || [ "$#" -gt 2 ]; then
    usage >&2
    exit "${EXIT_USAGE}"
fi

REF="$1"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="${MINERVA_PROJECT_ROOT:-$(cd "${SCRIPT_DIR}/../.." && pwd)}"
WORKTREE_PARENT="${MINERVA_UNITY56_WORKTREE_ROOT:-/private/tmp/project-minerva-worktrees}"

if ! GIT_ROOT="$(git -C "${PROJECT_ROOT}" rev-parse --show-toplevel 2>/dev/null)"; then
    printf 'FAIL: not a Git worktree: %s\n' "${PROJECT_ROOT}" >&2
    exit "${EXIT_REPOSITORY}"
fi
PROJECT_ROOT="$(cd "${GIT_ROOT}" && pwd)"

if ! COMMIT="$(git -C "${PROJECT_ROOT}" rev-parse --verify "${REF}^{commit}" 2>/dev/null)"; then
    printf 'FAIL: ref does not resolve to a commit: %s\n' "${REF}" >&2
    exit "${EXIT_REF}"
fi

if [ "$#" -eq 2 ]; then
    DESTINATION="$2"
    if [ -e "${DESTINATION}" ]; then
        printf 'FAIL: destination already exists: %s\n' "${DESTINATION}" >&2
        exit "${EXIT_DESTINATION}"
    fi
    DESTINATION_PARENT="$(dirname "${DESTINATION}")"
    mkdir -p "${DESTINATION_PARENT}" || exit "${EXIT_DESTINATION}"
else
    mkdir -p "${WORKTREE_PARENT}" || exit "${EXIT_DESTINATION}"
    DESTINATION="$(mktemp -d "${WORKTREE_PARENT}/worktree.XXXXXX")" || exit "${EXIT_DESTINATION}"
    rmdir "${DESTINATION}" || exit "${EXIT_DESTINATION}"
fi

if ! git -C "${PROJECT_ROOT}" worktree add --detach "${DESTINATION}" "${COMMIT}"; then
    if [ -d "${DESTINATION}" ]; then
        rmdir "${DESTINATION}" 2>/dev/null || true
    fi
    printf 'FAIL: unable to create verification worktree.\n' >&2
    exit "${EXIT_CREATE}"
fi

printf 'WORKTREE_PATH=%s\n' "${DESTINATION}"
printf 'WORKTREE_COMMIT=%s\n' "${COMMIT}"
printf 'WORKTREE_REF=%s\n' "${REF}"
