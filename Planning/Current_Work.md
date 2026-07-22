# Current Work

**Last Updated:** 2026-07-22  
**Owner:** Technical Director

## Active Milestone

**M1.2 — Runtime Bootstrap**

## Active Branch

`docs/fnd-001-closeout-and-fnd-002`

## Current Scope

- Close out FND-001 after accepted implementation through PR #5.
- Correct ticket lifecycle categorization and status metadata.
- Strengthen workflow ownership for `Ready`, `Active`, `Review`, and `Complete` transitions.
- Prepare and review FND-002 for the deterministic runtime bootstrap.
- Keep FND-002 implementation blocked until its Ready ticket is approved and merged.

## Explicitly Not Active

- FND-002 runtime implementation
- Event Bus implementation
- Time and Clock implementation
- Save/Load implementation
- World State implementation
- Narrative Runtime implementation
- Phone applications or UI navigation
- Story Timeline implementation
- Game Director implementation
- Third-party dependencies
- Assembly-definition design

## Completed Ticket

- `Docs/Tickets/Complete/FND-001_Create_Unity_Project_Skeleton.md`

## Current Ready Ticket

- `Docs/Tickets/Ready/FND-002_Create_Runtime_Bootstrap.md`

## Next Approved Planning Step

After this documentation PR is reviewed and merged, assign `FND-002` to Codex on a dedicated implementation branch. Codex must move the ticket to `Active` when work begins and to `Review` when the implementation pull request opens. The Technical Director performs the post-merge move to `Complete`.