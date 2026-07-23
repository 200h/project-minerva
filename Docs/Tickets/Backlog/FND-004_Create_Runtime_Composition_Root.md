# FND-004: Create Runtime Composition Root

**Status:** Backlog  
**Owner:** Unassigned  
**Created:** 2026-07-23  
**Updated:** 2026-07-23  
**Roadmap Phase:** Phase 1 — Foundation  
**Epic:** [Foundation Runtime Services](../../../Planning/Epics/Foundation_Runtime_Services.md)  
**Related ADRs:** [`../../ADR/0001_Use_Unity_5_6.md`](../../ADR/0001_Use_Unity_5_6.md), [`../../ADR/0004_Ticket_Driven_AI_Implementation.md`](../../ADR/0004_Ticket_Driven_AI_Implementation.md)

## Summary

Create the explicit plain-C# composition root that constructs and owns the approved foundation runtime services.

## Goal

- Construct the runtime bootstrap and core event bus explicitly.
- Register owned services in deterministic lifecycle order.
- Expose dependencies through constructors or narrow composition results.
- Centralize ownership and disposal without creating a service locator.

## Requirements

- Depend on the accepted FND-002 lifecycle contracts and FND-003 event-bus contracts.
- Keep concrete construction in one narrow composition boundary.
- Make startup failure and disposal ownership observable.
- Support repeated isolated construction in tests.
- Add focused Unity 5.6-compatible EditMode tests.

## Out of Scope

- time, scheduling, state, save, narrative, phone, app, notification, UI, platform, or editor behavior;
- reflection, scene discovery, hidden singleton creation, global registries, or a general dependency-injection framework;
- configuration loading or assembly definitions.

## Dependencies and Prerequisites

- FND-002 is Complete.
- FND-003 is Complete.

## Authorized Files or Directories

- `Assets/Minerva/Runtime/Core/**`
- `Assets/Minerva/Tests/Runtime/**`
- this ticket file for lifecycle movement and reporting

## Acceptance Criteria

- [ ] One explicit composition root constructs the approved foundation services.
- [ ] Service registration and disposal order are deterministic.
- [ ] No service locator, static mutable registry, scene search, or reflection discovery exists.
- [ ] Separate composed runtimes can be created and disposed repeatedly.
- [ ] Focused tests cover construction, failure propagation, ordering, and disposal.
- [ ] Only authorized paths change.

## Readiness Notes

Detailed scope is established, but this ticket remains `Backlog` until FND-003 is accepted and its final contracts are reviewed.
