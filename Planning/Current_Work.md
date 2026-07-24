# Current Work

**Last Updated:** 2026-07-23  
**Owner:** Technical Director

## Active Milestone

**M1.3 — Foundation Runtime Services**

## Active Epic

[Foundation Runtime Services](Epics/Foundation_Runtime_Services.md)

## Active Branch

`tickets/fnd-007-runtime-state-primitives`

## Current Scope

- Complete the Technical Director readiness pass for FND-007 against the accepted FND-003 event and FND-004 composition contracts and tests.
- Promote FND-007 from `Backlog` to `Ready` as the sole approved implementation ticket.
- Define isolated generic typed state cells with explicit owner-qualified stable identity.
- Define separate read and mutation capabilities, deterministic equality/no-change behavior, validation results, null/default handling, and immutable completed-change records.
- Define optional completed-change event publication without rollback, event ownership, subscriptions, or composition changes.
- Preserve FND-008 snapshot, restore, serialization, persistence, and owner-contribution design for its own readiness pass.
- Keep the Foundation Runtime Services epic register synchronized with authoritative ticket metadata and directories.
- Prepare a documentation-only pull request against `main`; do not implement FND-007 in this branch.

## Explicitly Not Active

- FND-007 runtime implementation before this readiness pull request is merged.
- FND-008 implementation.
- A global runtime-state registry, stable-key property bag, arbitrary lookup, or reflection-driven discovery.
- Story variables, world-state schemas, branch history, relationship state, phone/app state, or domain-specific validators.
- Snapshot capture, restore, persistence, serialization, schema migration, save file I/O, or platform storage.
- Provenance, correlation, cause, simulation timestamps, history, undo, redo, or event sourcing.
- Automatic runtime composition or service lookup.
- Narrative Runtime implementation.
- Phone applications or UI navigation.
- Story Timeline implementation.
- Game Director implementation.
- Third-party dependencies.
- Assembly-definition design.

## Completed Tickets

- `Docs/Tickets/Complete/FND-001_Create_Unity_Project_Skeleton.md`
- `Docs/Tickets/Complete/FND-002_Create_Runtime_Bootstrap.md`
- `Docs/Tickets/Complete/FND-003_Create_Core_Event_Bus.md`
- `Docs/Tickets/Complete/FND-004_Create_Runtime_Composition_Root.md`
- `Docs/Tickets/Complete/FND-005_Create_Core_Clock_Abstractions.md`
- `Docs/Tickets/Complete/FND-006_Create_Scheduled_Task_Queue.md`

## Current Ready Ticket

- `Docs/Tickets/Ready/FND-007_Create_Runtime_State_Primitives.md`

FND-007 is the sole `Ready` ticket and the next current execution ticket after this documentation pull request merges.

## Sequenced Backlog

- FND-008 — Save Snapshot Contracts

## Next Approved Execution Step

After this readiness pull request is reviewed and merged, assign FND-007 to Codex on a dedicated implementation branch created from updated `main`. Codex moves the ticket to `Active` when work begins and to `Review` when its implementation pull request opens. Independent Implementation Review Agent validation and Technical Director acceptance are required before the ticket moves to `Complete`; the Repository Owner retains merge authority.
