# Current Work

**Last Updated:** 2026-07-23  
**Owner:** Technical Director

## Active Milestone

**M1.3 — Foundation Runtime Services**

## Active Epic

[Foundation Runtime Services](Epics/Foundation_Runtime_Services.md)

## Active Branch

`tickets/fnd-005-core-clock-abstractions`

## Current Scope

- Complete the Technical Director readiness pass for FND-005 against the accepted FND-004 runtime composition contracts and tests.
- Promote FND-005 from `Backlog` to `Ready` as the sole approved implementation ticket.
- Define exact integer-millisecond representation, monotonicity, pause/resume, boundary, lifecycle, manual-clock, and future composition semantics.
- Keep FND-006 scheduling explicitly outside FND-005.
- Keep the Foundation Runtime Services epic register synchronized with authoritative ticket metadata and directories.
- Prepare a documentation-only pull request against `main`; do not implement FND-005 in this branch.

## Explicitly Not Active

- FND-005 runtime implementation before the readiness pull request is merged.
- FND-006 scheduled-task implementation.
- FND-007 and FND-008 implementation.
- Automatic Unity, frame, platform, wall-clock, background, or catch-up time.
- Domain-specific event classes.
- Narrative Runtime implementation.
- Phone applications or UI navigation.
- Story Timeline implementation.
- Game Director implementation.
- Save file I/O or platform persistence.
- Third-party dependencies.
- Assembly-definition design.

## Completed Tickets

- `Docs/Tickets/Complete/FND-001_Create_Unity_Project_Skeleton.md`
- `Docs/Tickets/Complete/FND-002_Create_Runtime_Bootstrap.md`
- `Docs/Tickets/Complete/FND-003_Create_Core_Event_Bus.md`
- `Docs/Tickets/Complete/FND-004_Create_Runtime_Composition_Root.md`

## Current Ready Ticket

- `Docs/Tickets/Ready/FND-005_Create_Core_Clock_Abstractions.md`

FND-005 is the sole `Ready` ticket and the next current execution ticket after this documentation pull request merges.

## Sequenced Backlog

- FND-006 — Scheduled Task Queue
- FND-007 — Runtime State Primitives
- FND-008 — Save Snapshot Contracts

## Next Approved Execution Step

After this readiness pull request is reviewed and merged, assign FND-005 to Codex on a dedicated implementation branch created from updated `main`. Codex moves the ticket to `Active` when work begins and to `Review` when its implementation pull request opens. Independent Implementation Review Agent validation and Technical Director acceptance are required before the ticket moves to `Complete`; the Repository Owner retains merge authority.
