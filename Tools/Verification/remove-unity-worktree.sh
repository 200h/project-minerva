#!/usr/bin/env bash

# Safely remove a clean, registered, non-primary verification worktree.

set -u

EXIT_USAGE=10
EXIT_REPOSITORY=20
EXIT_TARGET=21
EXIT_UNREGISTERED=22
EXIT_PRIMARY=23
EXIT_DIRTY=24
EXIT_REMOVE=25

usage()
{
    cat <<'EOF'
Usage: remove-unity-worktree.sh <worktree-path>

The target must be a registered, clean, non-primary worktree. Forced removal is
intentionally unsupported.
EOF
}

if [ "$#" -ne 1 ]; then
    usage >&2
    exit "${EXIT_USAGE}"
fi

TARGET_INPUT="$1"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="${MINERVA_PROJECT_ROOT:-$(cd "${SCRIPT_DIR}/../.." && pwd)}"

if ! GIT_ROOT="$(git -C "${PROJECT_ROOT}" rev-parse --show-toplevel 2>/dev/null)"; then
    printf 'FAIL: not a Git worktree: %s\n' "${PROJECT_ROOT}" >&2
    exit "${EXIT_REPOSITORY}"
fi
PROJECT_ROOT="$(cd "${GIT_ROOT}" && pwd -P)"

if [ ! -d "${TARGET_INPUT}" ]; then
    printf 'FAIL: worktree path does not exist: %s\n' "${TARGET_INPUT}" >&2
    exit "${EXIT_TARGET}"
fi
TARGET="$(cd "${TARGET_INPUT}" && pwd -P)"

PRIMARY_WORKTREE_RAW="$(
    git -C "${PROJECT_ROOT}" worktree list --porcelain |
        awk '/^worktree / { print substr($0, 10); exit }'
)"
PRIMARY_WORKTREE="$(cd "${PRIMARY_WORKTREE_RAW}" && pwd -P)"
if [ "${TARGET}" = "${PRIMARY_WORKTREE}" ]; then
    printf 'FAIL: refusing to remove the primary worktree: %s\n' "${TARGET}" >&2
    exit "${EXIT_PRIMARY}"
fi

REGISTERED=0
while IFS= read -r line; do
    case "${line}" in
        "worktree "*)
            REGISTERED_PATH="${line#worktree }"
            if [ -d "${REGISTERED_PATH}" ]; then
                REGISTERED_PATH="$(cd "${REGISTERED_PATH}" && pwd -P)"
                if [ "${REGISTERED_PATH}" = "${TARGET}" ]; then
                    REGISTERED=1
                    break
                fi
            fi
            ;;
    esac
done <<EOF
$(git -C "${PROJECT_ROOT}" worktree list --porcelain)
EOF

if [ "${REGISTERED}" -ne 1 ]; then
    printf 'FAIL: target is not a registered worktree: %s\n' "${TARGET}" >&2
    exit "${EXIT_UNREGISTERED}"
fi

if [ -n "$(git -C "${TARGET}" status --porcelain=v1 --untracked-files=all)" ]; then
    git -C "${TARGET}" status --short >&2
    printf 'FAIL: refusing to remove a dirty worktree.\n' >&2
    exit "${EXIT_DIRTY}"
fi

if ! git -C "${PROJECT_ROOT}" worktree remove "${TARGET}"; then
    printf 'FAIL: git refused to remove the worktree; no force was used.\n' >&2
    exit "${EXIT_REMOVE}"
fi

printf 'REMOVED_WORKTREE=%s\n' "${TARGET}"
