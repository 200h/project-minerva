# Foundation Runtime Services Epic

**Status:** Active  
**Owner:** Product Owner and Technical Director  
**Created:** 2026-07-23  
**Last Updated:** 2026-07-23  
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

**Progress:** 2 Complete · 1 Ready · 5 Backlog  
**Current Execution Ticket:** FND-003  
**Last Updated:** 2026-07-23  

## Included Tickets

Ticket directories and ticket metadata are authoritative. This table is a synchronized planning summary; stale rows are planning defects.

| Ticket | Title | Status | Created | Last Updated | Depends On |
|---|---|---|---|---|---|
| [FND-001](../../Docs/Tickets/Complete/FND-001_Create_Unity_Project_Skeleton.md) | Create Unity Project Skeleton | Complete | 2026-07-21 | 2026-07-21 | — |
| [FND-002](../../Docs/Tickets/Complete/FND-002_Create_Runtime_Bootstrap.md) | Create Runtime Bootstrap | Complete | 2026-07-21 | 2026-07-21 | FND-001 |
| [FND-003](../../Docs/Tickets/Ready/FND-003_Create_Core_Event_Bus.md) | Create Core Event Bus | Ready | 2026-07-23 | 2026-07-23 | FND-002 |
| [FND-004](../../Docs/Tickets/Backlog/FND-004_Create_Runtime_Composition_Root.md) | Create Runtime Composition Root | Backlog | 2026-07-23 | 2026-07-23 | FND-003 |
| [FND-005](../../Docs/Tickets/Backlog/FND-005_Create_Core_Clock_Abstractions.md) | Create Core Clock Abstractions | Backlog | 2026-07-23 | 2026-07-23 | FND-004 |
| [FND-006](../../Docs/Tickets/Backlog/FND-006_Create_Scheduled_Task_Queue.md) | Create Scheduled Task Queue | Backlog | 2026-07-23 | 2026-07-23 | FND-005 |
| [FND-007](../../Docs/Tickets/Backlog/FND-007_Create_Runtime_State_Primitives.md) | Create Runtime State Primitives | Backlog | 2026-07-23 | 2026-07-23 | FND-004 |
| [FND-008](../../Docs/Tickets/Backlog/FND-008_Define_Save_Snapshot_Contracts.md) | Define Save Snapshot Contracts | Backlog | 2026-07-23 | 2026-07-23 | FND-007 |

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

- FND-003 through FND-006 are detailed enough for readiness review when their dependencies complete.
- FND-007 and FND-008 are provisional backlog tickets and require design refinement before promotion to `Ready`.
- Being listed in this epic does not authorize implementation.

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
- save file I/O, autosave policy, encryption, compression, and platform paths;
- asynchronous or multithreaded dispatch;
- editor tooling and visualization;
- final application and phone composition.

## Design Questions to Revisit

- Whether runtime state primitives should use typed containers, stable keys, or another constrained ownership model.
- Whether snapshot participation is coordinated directly by the composition root or by a later dedicated save service.
- Whether later domain events require envelopes for timestamps, correlation, or provenance.

These questions must not broaden earlier tickets unless a concrete dependency requires it.
