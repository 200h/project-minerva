# FND-005: Create Core Clock Abstractions

**Status:** Backlog  
**Owner:** Unassigned  
**Created:** 2026-07-23  
**Updated:** 2026-07-23  
**Roadmap Phase:** Phase 1 — Foundation  
**Epic:** [Foundation Runtime Services](../../../Planning/Epics/Foundation_Runtime_Services.md)  

## Summary

Define generic runtime-time contracts and deterministic test-clock behavior for later scheduling and simulation systems.

## Goal

- Provide an authoritative runtime clock interface.
- Represent monotonic elapsed runtime time without wall-clock assumptions.
- Define pause and resume semantics.
- Provide a manually advanced clock for deterministic tests.

## Requirements

- Use plain C# compatible with Unity 5.6.
- Prevent time from moving backward.
- Make pause/resume idempotent and observable.
- Define whether advancing while paused is rejected or ignored.
- Avoid direct dependence on `UnityEngine.Time` in the foundational contracts.
- Add focused tests for advancement, pause, resume, boundaries, and repeated construction.

## Out of Scope

- real-world dates, time zones, phone clock presentation, calendar behavior;
- scheduled work, delayed events, narrative timing, save persistence;
- coroutines, async tasks, frame ownership, platform clocks, or editor tooling.

## Dependencies and Prerequisites

- FND-004 is Complete.

## Authorized Files or Directories

- `Assets/Minerva/Runtime/Core/**`
- `Assets/Minerva/Tests/Runtime/**`
- this ticket file for lifecycle movement and reporting

## Acceptance Criteria

- [ ] Generic clock contracts are independent of domain behavior.
- [ ] Elapsed time is monotonic and deterministic.
- [ ] Pause and resume semantics are explicit and tested.
- [ ] A manual test clock supports exact controlled advancement.
- [ ] No scheduling, narrative, phone, calendar, or platform behavior is introduced.
- [ ] Only authorized paths change.

## Readiness Notes

Detailed scope is established, but final readiness requires review against the accepted FND-004 composition contracts.
