# Git Workflow

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`Branch_Strategy.md`](Branch_Strategy.md), [`Ticket_Workflow.md`](Ticket_Workflow.md), [`Code_Review_Checklist.md`](Code_Review_Checklist.md)

## Purpose

This document defines the standard Git workflow for Project Minerva.

## Standard Flow

1. Confirm the approved ticket or documentation scope.
2. Update local `main` from the remote.
3. Create a correctly named branch from current `main`.
4. Make only the authorized changes.
5. Review the diff and remove unrelated changes.
6. Run required validation.
7. Commit in focused units with clear messages.
8. Push the branch.
9. Open a draft pull request against `main`.
10. Address review feedback without expanding scope.
11. Mark ready only when the Definition of Done is satisfied.
12. Merge using the repository’s approved method.
13. Delete the merged branch unless it has an explicit continuing purpose.

## Main Branch

`main` represents reviewed, accepted repository state. Direct feature work on `main` is prohibited. Emergency direct changes require explicit Technical Director authorization and must be documented afterward.

## Commits

Commits should be coherent, reviewable, and limited to one logical purpose. Prefer messages such as:

- `docs: add ticket workflow`
- `arch: define event bus boundaries`
- `tool: add timeline event validator`
- `fix: preserve notification state on load`

Do not use vague messages such as `updates`, `changes`, or `misc`.

## Pull Requests

Pull requests should begin as drafts unless the change is already complete and independently validated. The body should include:

- summary;
- reason for the change;
- scope and exclusions;
- validation performed;
- risks or unresolved questions;
- review focus.

## Review Changes

Review feedback should be addressed on the same branch when it belongs to the approved scope. Unrelated improvements should become separate tickets.

## Merge and Cleanup

After merge:

- confirm the PR is merged into `main`;
- delete the remote feature branch unless retention is intentional;
- update the related ticket to `Complete`;
- record follow-up work separately;
- never reuse a merged branch for new work.

## Prohibited Practices

- force-pushing shared branches without authorization;
- committing generated or vendor files accidentally;
- mixing unrelated changes in one PR;
- silently resolving conflicts by discarding approved work;
- merging with failed required checks;
- bypassing review for convenience.
