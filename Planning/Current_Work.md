# Current Work

**Last Updated:** 2026-07-24  
**Owner:** Technical Director

## Active Milestone

**M1.3 — Foundation Runtime Services**

## Active Epic

[Foundation Runtime Services](Epics/Foundation_Runtime_Services.md)

## Active Branch

`tickets/fnd-008-save-snapshot-contracts`

## Current Scope

- Complete the Technical Director readiness pass for FND-008 against the accepted FND-007 runtime-state contracts and the merged Unity execution model.
- Promote FND-008 from `Backlog` to `Ready` as the sole approved implementation ticket.
- Define stable owner-qualified contribution identity and explicit positive per-contribution schema versions.
- Define owner-controlled, immutable, detached, format-agnostic snapshot data and contributor contracts.
- Define explicit contributor registration as deterministic capture, preparation, apply, rollback, and release order.
- Require complete structural validation and non-mutating preparation before authoritative restore mutation begins.
- Define rollback-capable prepared restore operations, reverse-order rollback, and ordered immutable failure diagnostics.
- Preserve save envelopes, serialization formats, migrations, file I/O, slots, autosave policy, platform storage, and domain payload schemas for later work.
- Require the merged Unity 5.6 preflight and one-command local verification workflow for implementation acceptance.
- Keep the Foundation Runtime Services epic register synchronized with authoritative ticket metadata and directories.
- Prepare a documentation-only pull request against `main`; do not implement FND-008 in this branch.

## Explicitly Not Active

- FND-008 runtime implementation before this readiness pull request is merged.
- File, stream, database, PlayerPrefs, cloud, or platform storage.
- JSON, XML, binary, ScriptableObject, Easy Save, or another serialization-format decision.
- Save envelopes, slots, filenames, integrity hashes, encryption, compression, cloud synchronization, or autosave policy.
- Schema migrations, migration registries, payload conversion, or migration UI.
- Concrete narrative, world-state, relationship, dialogue, phone, app, notification, time, schedule, UI, or presentation snapshot payloads.
- Automatic runtime-state enumeration, global snapshot registries, service lookup, scene discovery, or reflection discovery.
- Event sourcing, replay, history, undo, or redo.
- Changes to accepted runtime composition, event, time, scheduling, or runtime-state contracts.
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
- `Docs/Tickets/Complete/FND-007_Create_Runtime_State_Primitives.md`

## Current Ready Ticket

- `Docs/Tickets/Ready/FND-008_Define_Save_Snapshot_Contracts.md`

FND-008 is the sole `Ready` ticket and the next current execution ticket after this documentation pull request merges.

## Sequenced Backlog

- No additional Foundation Runtime Services ticket is currently sequenced. The epic closes after accepted FND-008 implementation and merge.

## Next Approved Execution Step

After this readiness pull request is reviewed and merged, assign FND-008 to the Implementation Engineer on a dedicated branch created from updated `main`. The Implementation Engineer moves the ticket to `Active` when work begins and to `Review` when its implementation pull request opens.

The handoff must declare implementation capability and route mandatory Unity 5.6.7f1 validation to the Local Unity Verification Operator using:

```bash
./Tools/Verification/unity56-preflight.sh
./Tools/Verification/verify-unity56-editmode.sh
```

Independent Implementation Review Agent validation and Technical Director acceptance are required before the ticket moves to `Complete`; the Repository Owner retains merge authority.
