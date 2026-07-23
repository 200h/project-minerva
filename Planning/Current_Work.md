# Current Work

**Last Updated:** 2026-07-23  
**Owner:** Technical Director

## Active Milestone

**M1.3 — Foundation Runtime Services**

## Active Epic

[Foundation Runtime Services](Epics/Foundation_Runtime_Services.md)

## Active Branch

`tickets/fnd-004-runtime-composition-root`

## Current Scope

- Complete the Technical Director readiness pass for FND-004 against the accepted FND-002 and FND-003 contracts and tests.
- Promote FND-004 from `Backlog` to `Ready` as the sole approved implementation ticket.
- Define exact composition ownership, lifecycle, startup-failure cleanup, authority boundaries, acceptance criteria, and validation requirements.
- Keep the Foundation Runtime Services epic register synchronized with authoritative ticket metadata and directories.
- Prepare a documentation-only pull request against `main`; do not implement FND-004 in this branch.

## Explicitly Not Active

- FND-004 runtime implementation before the readiness pull request is merged.
- FND-005 through FND-008 implementation.
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

## Current Ready Ticket

- `Docs/Tickets/Ready/FND-004_Create_Runtime_Composition_Root.md`

FND-004 is the sole `Ready` ticket and the next current execution ticket after this documentation pull request merges.

## Sequenced Backlog

- FND-005 — Core Clock Abstractions
- FND-006 — Scheduled Task Queue
- FND-007 — Runtime State Primitives
- FND-008 — Save Snapshot Contracts

## Next Approved Execution Step

After this readiness pull request is reviewed and merged, assign FND-004 to Codex on a dedicated implementation branch created from updated `main`. Codex moves the ticket to `Active` when work begins and to `Review` when its implementation pull request opens. Independent Implementation Review Agent validation and Technical Director acceptance are required before the ticket moves to `Complete`; the Repository Owner retains merge authority.
