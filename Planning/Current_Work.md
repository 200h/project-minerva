# Current Work

**Last Updated:** 2026-07-23  
**Owner:** Technical Director

## Active Milestone

**M1.3 — Foundation Runtime Services**

## Active Epic

[Foundation Runtime Services](Epics/Foundation_Runtime_Services.md)

## Active Branch

`tickets/fnd-006-scheduled-task-queue`

## Current Scope

- Complete the Technical Director readiness pass for FND-006 against the accepted FND-005 clock contracts and tests.
- Promote FND-006 from `Backlog` to `Ready` as the sole approved implementation ticket.
- Define queue ownership, lifecycle, scheduling, cancellation, total ordering, captured drain boundaries, bounded processing, callback diagnostics, reentrancy, shutdown, and composition semantics.
- Preserve FND-005 clock contracts unchanged and keep clock advancement outside the queue.
- Keep automatic processing, recurrence, persistence, event publication, and domain behavior outside FND-006.
- Keep the Foundation Runtime Services epic register synchronized with authoritative ticket metadata and directories.
- Prepare a documentation-only pull request against `main`; do not implement FND-006 in this branch.

## Explicitly Not Active

- FND-006 runtime implementation before the readiness pull request is merged.
- FND-007 and FND-008 implementation.
- Automatic Unity, frame, platform, wall-clock, background, or catch-up time.
- Runtime update-loop or automatic scheduler ownership.
- Recurring tasks, retries, persistence, or restore behavior.
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
- `Docs/Tickets/Complete/FND-005_Create_Core_Clock_Abstractions.md`

## Current Ready Ticket

- `Docs/Tickets/Ready/FND-006_Create_Scheduled_Task_Queue.md`

FND-006 is the sole `Ready` ticket and the next current execution ticket after this documentation pull request merges.

## Sequenced Backlog

- FND-007 — Runtime State Primitives
- FND-008 — Save Snapshot Contracts

## Next Approved Execution Step

After this readiness pull request is reviewed and merged, assign FND-006 to Codex on a dedicated implementation branch created from updated `main`. Codex moves the ticket to `Active` when work begins and to `Review` when its implementation pull request opens. Independent Implementation Review Agent validation and Technical Director acceptance are required before the ticket moves to `Complete`; the Repository Owner retains merge authority.
