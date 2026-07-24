# Foundation Runtime Services Epic

**Status:** Active  
**Owner:** Product Owner and Technical Director  
**Created:** 2026-07-23  
**Last Updated:** 2026-07-24  
**Roadmap Phase:** Phase 1 — Foundation  

## Objective

Establish the generic runtime infrastructure required by later simulation, narrative, phone, application, notification, and persistence systems without introducing domain-specific behavior prematurely.

## Architectural Boundary

This epic owns reusable foundation mechanisms only:

- Unity repository structure;
- deterministic runtime lifecycle and composition;
- generic event publication and subscription;
- runtime clock abstractions;
- deterministic scheduled work;
- controlled runtime state primitives;
- versioned save-snapshot contracts.

It does not own narrative rules, phone applications, UI navigation, authored content, platform persistence, notification policy, or final domain event schemas.

## Progress

**Progress:** 7 Complete · 1 Ready · 0 Backlog  
**Current Execution Ticket:** FND-008  
**Last Updated:** 2026-07-24  

## Included Tickets

Ticket directories and ticket metadata are authoritative. This table is a synchronized planning summary; stale rows are planning defects.

| Ticket | Title | Status | Created | Last Updated | Depends On |
|---|---|---|---|---|---|
| [FND-001](../../Docs/Tickets/Complete/FND-001_Create_Unity_Project_Skeleton.md) | Create Unity Project Skeleton | Complete | 2026-07-21 | 2026-07-23 | — |
| [FND-002](../../Docs/Tickets/Complete/FND-002_Create_Runtime_Bootstrap.md) | Create Runtime Bootstrap | Complete | 2026-07-21 | 2026-07-23 | FND-001 |
| [FND-003](../../Docs/Tickets/Complete/FND-003_Create_Core_Event_Bus.md) | Create Core Event Bus | Complete | 2026-07-23 | 2026-07-23 | FND-002 |
| [FND-004](../../Docs/Tickets/Complete/FND-004_Create_Runtime_Composition_Root.md) | Create Runtime Composition Root | Complete | 2026-07-23 | 2026-07-23 | FND-003 |
| [FND-005](../../Docs/Tickets/Complete/FND-005_Create_Core_Clock_Abstractions.md) | Create Core Clock Abstractions | Complete | 2026-07-23 | 2026-07-23 | FND-004 |
| [FND-006](../../Docs/Tickets/Complete/FND-006_Create_Scheduled_Task_Queue.md) | Create Scheduled Task Queue | Complete | 2026-07-23 | 2026-07-23 | FND-005 |
| [FND-007](../../Docs/Tickets/Complete/FND-007_Create_Runtime_State_Primitives.md) | Create Runtime State Primitives | Complete | 2026-07-23 | 2026-07-23 | FND-004 |
| [FND-008](../../Docs/Tickets/Ready/FND-008_Define_Save_Snapshot_Contracts.md) | Define Save Snapshot Contracts | Ready | 2026-07-23 | 2026-07-24 | FND-007 |

## Dependency Graph

```text
FND-001 Unity Repository Structure
        ↓
FND-002 Runtime Bootstrap
        ↓
FND-003 Core Event Bus
        ↓
FND-004 Runtime Composition Root
        ├── FND-005 Core Clock Abstractions
        │       └── FND-006 Scheduled Task Queue
        └── FND-007 Runtime State Primitives
                └── FND-008 Save Snapshot Contracts
```

## Ticket Detail Horizon

- FND-008 is the sole ticket approved for immediate implementation.
- No later foundation ticket is authorized by this epic.
- Being listed in this epic does not authorize implementation beyond a ticket's current workflow state.

## Completion Criteria

This epic is complete when:

- all included tickets are accepted and merged;
- foundation services are composed explicitly without scene discovery or global service locators;
- deterministic event, timing, scheduling, state, and snapshot contracts are validated in Unity 5.6;
- later systems can depend on these contracts without modifying their foundational semantics;
- deferred domain behavior remains outside the foundation layer.

## Deferred Capabilities

- domain-specific event catalogs;
- narrative variables and story-state rules;
- phone clock presentation and calendar semantics;
- notification delivery policy;
- save file I/O, save envelopes, slots, autosave policy, encryption, compression, and platform paths;
- concrete serialization formats and schema migrations;
- asynchronous or multithreaded dispatch;
- editor tooling and visualization;
- final application and phone composition.

## Resolved Design Decisions

- FND-007 uses isolated generic typed state cells with explicit owner-qualified stable identity and separate read/mutation capabilities.
- FND-007 does not introduce a global stable-key registry or untyped property bag.
- Equal identities on separately constructed primitive cells are allowed and remain isolated; aggregate owners must reject duplicates within their own boundary.
- Completed-change event publication is optional and fixed at state-cell construction.
- State mutation is authoritative before publication; event failures do not roll back the completed change.
- Snapshot participation remains owner-defined; FND-008 does not enumerate runtime-state cells automatically.
- FND-008 uses immutable owner-qualified contribution identities and explicit positive per-contribution schema versions.
- FND-008 uses explicit contributor registration order as capture, preparation, and apply order.
- FND-008 requires complete structural validation and non-mutating preparation before any restore apply begins.
- FND-008 uses rollback-capable prepared operations and reverse-order rollback after apply failure.
- FND-008 defines an in-memory, format-agnostic coordination boundary and does not choose storage, serialization, migration, or domain payloads.
- The snapshot coordinator is constructed explicitly, owns no contributor lifecycle, and is not added automatically to the accepted runtime composition API.

## Design Questions to Revisit

- Which later epic introduces the Save/Load System that owns save envelopes, storage adapters, integrity checks, and user-facing policy.
- Which concrete serialization format and migration strategy are accepted after Unity 5.6 compatibility testing.
- How concrete domain owners define snapshot payload schemas and supported prior versions.
- Whether later domain events require envelopes for timestamps, correlation, or provenance.

These questions must not broaden earlier tickets unless a concrete dependency requires it.
