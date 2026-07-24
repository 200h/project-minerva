# TOOL-001: Create Unity 5.6 Execution Reliability Workflow

**Status:** Complete
**Owner:** Codex
**Created:** 2026-07-24
**Updated:** 2026-07-24
**Roadmap Phase:** Phase 1 — Foundation
**Epic:** None
**Related ADRs:** [`../../ADR/0001_Use_Unity_5_6.md`](../../ADR/0001_Use_Unity_5_6.md), [`../../ADR/0004_Ticket_Driven_AI_Implementation.md`](../../ADR/0004_Ticket_Driven_AI_Implementation.md)

## Summary

Create a deterministic local Unity 5.6.7f1 preflight and EditMode validation workflow for macOS. Document the split between cloud implementation and local Unity verification, produce review-ready evidence, and protect repository integrity without changing the existing ticket lifecycle, acceptance roles, or merge policy.

## Background

FND-007 demonstrated that a cloud implementation environment may edit Unity code without having Unity or a C# compiler available. Local validation on macOS 12 succeeded, but manual command construction, result discovery, warning interpretation, and cleanup caused significant delay.

The approved response is an operational reliability layer: declare environment capabilities early, route Unity execution to a local verification operator, automate the known-good Unity 5.6 command sequence, require XML results, preserve evidence, and restore only narrowly allowlisted generated artifacts.

## Goal

Provide one documented local command that validates the exact checked-out branch and commit with Unity 5.6.7f1, reports authoritative test totals, classifies known warnings without hiding unknown errors, proves repository integrity, and emits a Markdown evidence report.

## Required Context

Read only:

- [`../../Project/AI_ENGINEERING_GUIDE.md`](../../Project/AI_ENGINEERING_GUIDE.md)
- [`../../Handbook/Ticket_Workflow.md`](../../Handbook/Ticket_Workflow.md)
- [`../../Handbook/Testing_Strategy.md`](../../Handbook/Testing_Strategy.md)
- [`../../Templates/Ticket_Template.md`](../../Templates/Ticket_Template.md)
- [`../../ADR/0001_Use_Unity_5_6.md`](../../ADR/0001_Use_Unity_5_6.md)

## Optional Context

Read only when required by a specific implementation question:

- [`../../Project/CODING_STANDARDS.md`](../../Project/CODING_STANDARDS.md)
- [`../../../README.md`](../../../README.md)
- completed FND-007 validation evidence as a concrete example of the workflow failure being addressed.

## Code Areas to Inspect

- `.gitignore`
- existing shell scripts under `Tools/`, if present.

Do not inspect or modify runtime or test implementation files.

## Requirements

### Documentation

- Create `Docs/Runbooks/UNITY_5_6_VALIDATION.md`.
- Create `Docs/Process/UNITY_EXECUTION_MODEL.md`.
- Link the runbook and execution model from the appropriate existing workflow, testing, AI-engineering, and ticket-template documentation.
- Require Unity-related handoffs to declare implementation environment, Unity execution environment, Unity version, executable, and validation responsibility.
- Define documentation/planning, static code, Unity runtime code, editor/assets, and platform/device work classifications.
- Define the Local Unity Verification Operator as a validation-only role that cannot accept work, move tickets to `Complete`, or merge.

### Preflight

- Create `Tools/Verification/unity56-preflight.sh`.
- Default to `/Applications/Unity/Unity.app/Contents/MacOS/Unity`.
- Support an explicit environment override without using broad or ambiguous system variable names.
- Remain read-only with respect to the repository.
- Check macOS, Unity executable and version, Git repository, clean working tree, branch and SHA, project lock or active Unity use, disk space, Unity Test Runner presence, Python 3, and Python XML parsing.
- Run a Unity batch launch against a disposable scratch project outside the repository.
- Return deterministic nonzero exit codes and actionable messages.

### EditMode Verification

- Create `Tools/Verification/verify-unity56-editmode.sh`.
- Invoke the preflight.
- Require a clean repository and record branch and SHA.
- Snapshot tracked state and committed `.meta` index state.
- Use a unique external temporary evidence directory.
- Run import/compilation separately from tests.
- Run the complete EditMode suite without `-quit`.
- Require and parse the XML result; XML is authoritative over process exit code alone.
- Print result, total, passed, failed, skipped, inconclusive, and assertion counts.
- Treat the suite as passing only when XML reports `Passed`, `failed = 0`, `inconclusive = 0`, and `passed = total`.
- Detect compiler failures and unknown errors or exceptions.
- Recognize the observed UnityShaderCompiler socket warning, callback-unregistration warning, and `ms_Instance` shutdown assertion only as known nonblocking warnings after an authoritative passing XML result.
- Preserve logs, XML, summaries, and diagnostic extracts on success and failure.
- Remove only explicitly classified generated artifacts that did not exist before the run.
- Never invoke broad `git clean` operations.
- Refuse to remove unexpected untracked paths.
- Verify tracked state, committed `.meta` state, working-tree cleanliness, and `git diff --check`.
- Emit a Markdown evidence report containing repository, branch, SHA, Unity version, import result, test totals, warning classification, and integrity results.

### Optional Worktree Helpers

- Create safe worktree creation and removal helpers when they remain narrowly scoped.
- Create detached disposable worktrees from an explicitly resolved ref.
- Refuse removal of the primary worktree, unregistered paths, or dirty worktrees.
- Do not use forced removal.

### Validation Harness

- Add a shell-level mock harness that exercises at least:
  - successful preflight and passing verification;
  - missing XML failure;
  - unexpected generated-path rejection;
  - clean-repository preservation.
- Use `shellcheck` when available.
- Always run `bash -n`.
- Keep test artifacts outside the repository or clean them deterministically.

## Out of Scope

- Runtime or test implementation changes.
- Unity scenes, prefabs, assets, packages, or assembly definitions.
- CI/CD, remote build hosts, device testing, or automated PR mutation.
- Changing ticket lifecycle states, acceptance authority, or repository-owner merge authority.
- Installing Unity, Python, shellcheck, Rosetta, packages, or third-party tools.
- Silently repairing implementation failures.
- Broad cleanup or deletion commands.

## Dependencies and Prerequisites

- Unity 5.6.7f1 is installed locally at the default path or supplied override.
- The repository is a Git worktree.
- Python 3 is available for XML parsing and safe path classification.
- Existing ticket and review roles remain authoritative.

## Authorized Files or Directories

- `Docs/Runbooks/**`
- `Docs/Process/**`
- `Tools/Verification/**`
- `Docs/Project/AI_ENGINEERING_GUIDE.md`
- `Docs/Handbook/Ticket_Workflow.md`
- `Docs/Handbook/Testing_Strategy.md`
- `Docs/Templates/Ticket_Template.md`
- `.gitignore` only if a narrowly required evidence-path rule is missing.
- This ticket file for lifecycle movement and reporting.

## Prohibited Files or Systems

- `Assets/**`
- `ProjectSettings/**`
- `Packages/**`
- `Planning/**`
- completed ticket contents
- architecture, system, narrative, and tool-design documents not explicitly authorized.

## Permitted Actions

- Move this ticket from `Ready` to `Active` when implementation begins.
- Move this ticket from `Active` to `Review` when the implementation pull request opens.
- Create and validate the approved documentation and shell tooling.
- Complete the Implementation Report before requesting review.

## Prohibited Actions

- Do not move this ticket to `Complete`; independent review and Technical Director acceptance are required.
- Do not merge the implementation pull request.
- Do not change runtime or test implementation files.
- Do not install software or modify machine configuration.
- Do not delete unexpected repository content.

## Expected Files

- `Docs/Runbooks/UNITY_5_6_VALIDATION.md`
- `Docs/Process/UNITY_EXECUTION_MODEL.md`
- `Tools/Verification/unity56-preflight.sh`
- `Tools/Verification/verify-unity56-editmode.sh`
- `Tools/Verification/create-unity-worktree.sh`
- `Tools/Verification/remove-unity-worktree.sh`
- `Tools/Verification/tests/test-verification-scripts.sh`

## Documentation Updates

- Add exact cross-links rather than duplicating the full runbook.
- Preserve the five existing ticket states and existing role ownership.
- Use `America/New_York` for date and execution-state metadata.
- Set `Status` to `Active`, record the owner, and update the date when implementation begins.
- Set `Status` to `Review` and update the date when the implementation pull request opens.

## Architect's Notes

- The scripts are operational safety tools, not an alternative architecture or test framework.
- A successful Unity process exit is insufficient without a valid passing XML result.
- Known warnings are contextual observations, not globally ignored patterns. They remain nonblocking only when the XML proves a passing run.
- Repository restoration must fail closed: unexpected paths remain untouched and are reported for human review.
- macOS 12 ships an older Bash; avoid features requiring modern Bash when a portable equivalent is practical.

## Acceptance Criteria

- [x] The runbook and execution model clearly separate cloud implementation from local Unity verification.
- [x] Unity-related handoffs require an explicit environment declaration.
- [x] Preflight is repository-read-only and reports all required capability checks.
- [x] One command performs import, compilation, full EditMode tests, XML parsing, integrity checks, cleanup, and evidence generation.
- [x] Missing or invalid XML causes a clear nonzero failure.
- [x] Exact test and assertion totals are printed automatically.
- [x] Known warnings are reported but never used to hide unknown errors.
- [x] Unexpected untracked content is rejected and preserved.
- [x] No broad Git cleanup command is used.
- [x] Worktree helpers are safe and non-forcing, or their omission is justified.
- [x] Shell validation and targeted mock tests pass.
- [x] No runtime or test implementation file changes.
- [x] The ticket directory and `Status` metadata match throughout implementation.
- [x] The implementation pull request contains this ticket under `Docs/Tickets/Review/` while independent review is pending.
- [x] After acceptance, the implementation pull request contains this ticket under `Docs/Tickets/Complete/` before merge.

## Required Validation

- Run `bash -n` on every new shell script.
- Run `shellcheck` when available and report when unavailable.
- Run the mock harness.
- Confirm preflight does not mutate a clean mock repository.
- Confirm the passing mock run emits a complete evidence report and restores the repository.
- Confirm missing XML returns its documented code and preserves evidence.
- Confirm unexpected generated paths are not deleted.
- Search for broad cleanup commands such as `git clean -fd`.
- Run `git diff --check`.
- Confirm no changed path is under `Assets/`, `ProjectSettings/`, `Packages/`, or `Planning/`.
- Confirm this ticket exists in exactly one workflow directory and its `Status` matches.

## Definition of Done

- [x] Acceptance criteria satisfied.
- [x] Required validation reported accurately.
- [x] Documentation updated.
- [x] No unauthorized changes.
- [x] Implementation report completed.

## Implementation Report

### Status

Implementation complete. Independent review pending.

### Changed Files

- `Docs/Handbook/Testing_Strategy.md`
- `Docs/Handbook/Ticket_Workflow.md`
- `Docs/Process/UNITY_EXECUTION_MODEL.md`
- `Docs/Project/AI_ENGINEERING_GUIDE.md`
- `Docs/Runbooks/UNITY_5_6_VALIDATION.md`
- `Docs/Templates/Ticket_Template.md`
- `Docs/Tickets/Review/TOOL-001_Create_Unity_5_6_Execution_Reliability_Workflow.md`
- `Tools/Verification/README.md`
- `Tools/Verification/create-unity-worktree.sh`
- `Tools/Verification/remove-unity-worktree.sh`
- `Tools/Verification/tests/test-verification-scripts.sh`
- `Tools/Verification/unity56-preflight.sh`
- `Tools/Verification/verify-unity56-editmode.sh`

### Work Completed

- Documented cloud/local execution classification, required environment declarations, validation handoffs, and the Local Unity Verification Operator role.
- Added a read-only macOS and Unity 5.6.7f1 preflight with actionable exit codes.
- Added one-command import, compilation, EditMode testing, XML parsing, warning classification, fail-closed cleanup, integrity verification, and Markdown evidence generation.
- Added detached worktree creation and clean non-forcing removal helpers.
- Added a mock Unity harness covering passing validation, missing XML, unexpected generated paths, repository restoration, and worktree helper behavior.

### Validation

- `bash -n Tools/Verification/*.sh Tools/Verification/tests/*.sh`: passed.
- `shellcheck`: unavailable in the local environment; no installation was attempted.
- Mock harness: passed all scenarios.
- Missing XML scenario: returned exit `41`, preserved evidence, and restored the mock repository.
- Unexpected-path scenario: returned exit `60`, preserved the unexpected file, and refused cleanup until the harness removed its own fixture.
- Unknown `ProjectSettings` scenario: returned exit `60` and preserved the unrecognized file.
- Unknown-error scenario: returned exit `50`, preserved the generic Unity/native error evidence, and restored allowlisted generated content.
- Worktree helper scenario: created a detached worktree and removed it only while clean, without force.
- Real Unity validation:
  - branch: `tool/unity56-execution-reliability`
  - validated implementation SHA after IRA-requested corrections: `551fa6362b49e44ba06e7065ba86716da53c9fcb`
  - Unity: `5.6.7f1`
  - import/compilation: passed, exit `0`
  - EditMode result: `Passed`, Unity exit `0`
  - tests: `95/95`
  - assertions: `412`
  - failed: `0`
  - skipped: `0`
  - inconclusive: `0`
  - UnityShaderCompiler warnings: `2`
  - callback-unregistration warnings: `1`
  - `ms_Instance` shutdown assertions: `1`
  - unknown blocking errors or exceptions: `0`
  - repository restored: yes
  - `git diff --check`: passed
  - evidence: `/private/tmp/project-minerva-unity/verification.0iEIhZ/verification-summary.md`
- Broad cleanup audit: no `git clean` invocation or forced worktree removal exists in shell scripts.
- Scope audit: no path under `Assets/`, `ProjectSettings/`, `Packages/`, or `Planning/` changed.

### Deviations

- The Unity launch preflight uses a disposable scratch project outside the repository. Unity 5.6.7f1 aborts on macOS 12 when launched in batch mode without any project. The scratch project preserves the preflight's repository-read-only guarantee.
- `shellcheck` was unavailable; `bash -n`, the mock harness, targeted audits, and a real Unity run were used.

### Blockers or Risks

- The initial IRA review required changes for broad `ProjectSettings` cleanup classification and narrow unknown-error detection. Both corrections and regression scenarios are included in the current review head; delta review is pending.
- Known Unity 5.6 shutdown/environment warnings remain nonblocking only under the documented authoritative passing-XML conditions.

### Optional Context Used

- Completed FND-007 validation evidence informed the known warnings, expected `95/95` test and `412` assertion baseline, and generated-path allowlist.

### Follow-Up Suggestions

- Exercise the scripts periodically after macOS, Unity installation, or repository-layout changes.
- Consider a dedicated validation Mac only if Unity-ticket volume later justifies it.

## Implementation Review Agent Record

Completed by the independent reviewer while the ticket is in `Review`.

### Reviewer

Implementation Review Agent `/root/tool001_ira_review`

### Reviewed PR and Head

- PR #19
- Initial reviewed head: `f00a6b62cf8541f1044cde81221f63573aa20c82`
- Accepted delta head: `6af8f48d0c7b8b1e2b71821d125edbd8bca1e371`
- Real Unity-validated implementation commit: `551fa6362b49e44ba06e7065ba86716da53c9fcb`

### Scope and Acceptance Findings

- The PR contains only authorized tooling, process documentation, and ticket paths.
- The initial review identified two blocking fail-closed safety gaps.
- The accepted delta replaced broad `ProjectSettings` cleanup with an explicit Unity 5.6.7f1 file allowlist and expanded generic Unity/native error recognition.
- Regression scenarios prove unknown `ProjectSettings` content is preserved and generic unknown errors block validation.
- No runtime, test implementation, package, project-setting, or planning file changed.

### Validation Assessment

- `bash -n`: passed for all verification scripts.
- Expanded mock harness: passed.
- Missing XML: exit `41`.
- Unexpected root path: exit `60` and preserved.
- Unknown `ProjectSettings` file: exit `60` and preserved.
- Generic unknown error: exit `50` with evidence preserved.
- Real Unity 5.6.7f1 import and compilation: passed.
- EditMode tests: 95/95 passed with 412 assertions and zero failed, skipped, or inconclusive tests.
- Repository restoration and `git diff --check`: passed.

### Blocking Findings

None after the accepted delta.

### Recommendation

`Accept`

## Technical Director Acceptance

Completed after reviewing the Implementation Review Agent recommendation and before merge.

### Decision

`Accepted`

### PR Reference

PR #19

### Acceptance Date

2026-07-24

### Final Validation Decision

Accepted. The current PR head contains the reviewed and corrected tooling plus ticket-only evidence updates after the real Unity-validated implementation commit.

### Accepted Deviations

- `shellcheck` was unavailable and was not installed. Bash syntax checks, the expanded mock harness, targeted audits, independent review, and a real Unity 5.6.7f1 run provide accepted substitute evidence.

### Follow-Up Tickets

None.

## Execution State Log

Use `YYYY-MM-DD HH:mm z` in `America/New_York`.

| State | Timestamp | Actor | Evidence or Notes |
|---|---|---|---|
| Planned | 2026-07-24 00:09 EDT | Technical Director | Scope approved by Repository Owner and captured in TOOL-001. |
| In Progress | 2026-07-24 00:09 EDT | Codex | Ticket moved to Active; implementation started on `tool/unity56-execution-reliability`. |
| Committed | 2026-07-24 00:18 EDT | Codex | Implementation commits `2c8b871`, `14d790f`, and `78a9906` recorded on the tooling branch. |
| Verified | 2026-07-24 00:21 EDT | Codex | Mock safety harness passed; real Unity 5.6.7f1 validation passed 95/95 tests and 412 assertions with the repository restored clean. |
| In Progress | 2026-07-24 00:55 EDT | Codex | Addressed the IRA findings for fail-closed `ProjectSettings` cleanup and generic Unity/native error detection. |
| Committed | 2026-07-24 00:58 EDT | Codex | IRA-requested corrections committed as `551fa63`. |
| Verified | 2026-07-24 00:59 EDT | Codex | Expanded regression harness passed; real Unity revalidation at `551fa63` passed 95/95 tests and 412 assertions with the repository restored clean. |
| Verified | 2026-07-24 01:01 EDT | Implementation Review Agent | Delta head `6af8f48` reviewed; both prior P1 findings resolved; recommendation `Accept`. |
| Verified | 2026-07-24 01:01 EDT | Technical Director | IRA recommendation accepted; TOOL-001 approved for merge and moved to `Complete`. |
