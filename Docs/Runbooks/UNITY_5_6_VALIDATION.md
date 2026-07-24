# Unity 5.6 Validation Runbook

**Version:** 1.0
**Status:** Draft
**Owner:** Technical Director
**Last Updated:** 2026-07-24
**Related Documents:** [`../Process/UNITY_EXECUTION_MODEL.md`](../Process/UNITY_EXECUTION_MODEL.md), [`../Handbook/Testing_Strategy.md`](../Handbook/Testing_Strategy.md), [`../../Tools/Verification/README.md`](../../Tools/Verification/README.md)

## Purpose

This runbook defines the supported local validation procedure for Project Minerva's Unity 5.6.7f1 code and EditMode tests. It replaces improvised command construction and manual log interpretation with a deterministic, evidence-producing workflow.

This runbook does not change ticket states, independent review, Technical Director acceptance, or Repository Owner merge authority.

## Supported Local Environment

- macOS
- Unity 5.6.7f1
- default executable:

  ```text
  /Applications/Unity/Unity.app/Contents/MacOS/Unity
  ```

- Bash compatible with the macOS 12 system shell
- Git
- Python 3 with `xml.etree.ElementTree`
- Unity Test Runner extension included with Unity 5.6.7f1

Override the Unity executable when necessary:

```bash
MINERVA_UNITY56_PATH="/alternate/Unity.app/Contents/MacOS/Unity" \
  ./Tools/Verification/verify-unity56-editmode.sh
```

## Standard Workflow

### 1. Declare Execution Responsibilities

Every Unity-related implementation handoff records:

```text
Implementation environment:
Unity execution environment:
Unity version:
Unity executable:
Validation responsibility:
```

Cloud implementation and local Unity validation are separate capabilities. An implementation agent without Unity must report validation as pending rather than failed or passed.

### 2. Use the Exact Review Head

Check out the pull request head or create a detached disposable worktree:

```bash
./Tools/Verification/create-unity-worktree.sh <pull-request-ref-or-sha>
```

Record the branch and SHA before validation. The evidence report must identify the exact commit that Unity validated.

### 3. Run Preflight

```bash
./Tools/Verification/unity56-preflight.sh
```

Preflight is repository-read-only. It checks:

- macOS;
- the Unity executable and 5.6.7f1 version;
- Git worktree identity;
- clean working tree;
- current branch and commit;
- project lock or Unity use of the project;
- free disk space;
- Test Runner extension;
- Python XML support;
- Unity batch launch against a disposable scratch project outside the repository;
- unchanged repository state after preflight.

Do not continue after a failed preflight. Apply the printed correction and rerun.

### 4. Run Complete Validation

```bash
./Tools/Verification/verify-unity56-editmode.sh
```

The verifier:

1. invokes preflight;
2. snapshots tracked and committed `.meta` index state;
3. creates a unique evidence directory under `/private/tmp/project-minerva-unity/`;
4. imports and compiles the exact project;
5. runs the complete EditMode suite without `-quit`;
6. requires and parses NUnit XML;
7. classifies known and unknown log findings;
8. removes only explicitly allowlisted generated artifacts;
9. verifies tracked and metadata integrity;
10. requires a clean restored worktree;
11. runs `git diff --check`;
12. emits `verification-summary.md`.

## Authoritative Success Criteria

Unity process exit code alone is not authoritative.

The EditMode suite passes only when the results XML reports:

```text
result = Passed
failed = 0
inconclusive = 0
passed = total
```

Skipped tests are reported and evaluated against the active ticket. Missing, invalid, or incomplete XML is a blocking failure.

## Known Unity 5.6 Warnings

The following were observed after an otherwise passing Unity 5.6.7f1 EditMode run:

- `Failed to get socket connection from UnityShaderCompiler shader compiler`
- `Callback unregistration failed. Callback not registered.`
- `Assertion failed on expression: 'ms_Instance != NULL'`

They are nonblocking only when all of the following are true:

- a results XML exists;
- the XML result is `Passed`;
- failures are zero;
- inconclusive tests are zero;
- no unknown blocking error or exception is present.

The verifier counts and reports these messages. It does not suppress log content. Any unknown exception, compiler error, or assertion remains blocking until reviewed.

## Repository Restoration

Validation begins from a clean worktree.

The verifier may remove only generated paths that:

- did not exist before the run;
- are reported by Git as untracked;
- match an approved classification:
  - one of the explicitly listed Unity 5.6.7f1 default `ProjectSettings` files encoded in the verifier;
  - `Assets/Minerva.meta` when `Assets/Minerva/` exists;
  - an untracked `Assets/Minerva/**/*.meta` whose paired path is an existing directory.

Unexpected untracked content is preserved and causes validation to fail. Tracked changes are preserved and cause validation to fail.

The `ProjectSettings` rule is intentionally file-specific. A new or unknown file under that directory is preserved and blocks cleanup until a reviewed tooling change classifies it.

Never use:

```bash
git clean -fd
```

or an equivalent broad cleanup operation.

Logs, XML, diagnostics, and the evidence summary remain outside the repository on success and failure.

## Evidence Required for Review

Attach or transcribe the generated summary containing:

- repository path;
- branch;
- exact SHA;
- Unity version;
- import/compilation result and exit code;
- test result and Unity exit code;
- total, passed, failed, skipped, and inconclusive tests;
- assertion count;
- known-warning counts;
- unknown-error count;
- tracked and `.meta` integrity;
- `git diff --check`;
- restored working-tree result;
- evidence-directory path.

The Local Unity Verification Operator records evidence but does not accept the ticket, move it to `Complete`, or merge the pull request.

## Failure Routing

### Preflight Failure

Correct the local capability or repository-state issue. Do not change implementation code.

### Compilation Failure

Return the ticket to the Implementation Engineer with the exact compiler errors, source paths, and lines.

### Test Failure

Return the ticket with the XML, failing test names, assertion messages, and log path.

### Missing XML

Treat the run as failed even if Unity exits `0`. Preserve the log and rerun only after diagnosing the Test Runner invocation or environment.

### Unknown Error or Exception

Treat it as blocking. Do not add it to the known-warning list within an implementation ticket. Changing warning classification requires a reviewed tooling/process change.

### Integrity or Cleanup Failure

Do not delete unexpected paths. Inspect the evidence report and `integrity-findings.txt`, restore the worktree deliberately, and rerun from clean state.

## Optional Disposable Worktree

Create:

```bash
./Tools/Verification/create-unity-worktree.sh <ref>
```

The helper resolves the ref to a commit and creates a detached worktree. Run verification from that path.

Remove only after the worktree is clean:

```bash
./Tools/Verification/remove-unity-worktree.sh <path>
```

The removal helper refuses the primary worktree, unregistered paths, dirty worktrees, and forced removal.

## Exit-Code Families

| Range | Meaning |
|---|---|
| `10` | Invalid usage |
| `20–29` | Preflight or environment failure |
| `30–39` | Import or compilation failure |
| `40–49` | Test process or XML failure |
| `50–59` | Unknown blocking log finding |
| `60–69` | Repository integrity or cleanup failure |
| `70–79` | Evidence-report failure |

The exact failure and correction are printed and preserved in the evidence directory.
