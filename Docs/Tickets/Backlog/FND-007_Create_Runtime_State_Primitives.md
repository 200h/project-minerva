# FND-007: Create Runtime State Primitives

**Status:** Backlog  
**Owner:** Unassigned  
**Created:** 2026-07-23  
**Updated:** 2026-07-23  
**Roadmap Phase:** Phase 1 — Foundation  
**Epic:** [Foundation Runtime Services](../../../Planning/Epics/Foundation_Runtime_Services.md)  

## Summary

Define provisional generic primitives for controlled runtime state ownership and observable state changes.

## Goal

Provide constrained typed state containers or equivalent contracts that preserve ownership, validate mutation, and produce immutable change records suitable for later event publication.

## Provisional Boundaries

- State must have an explicit owner; this must not become an unrestricted global property bag.
- Mutation must pass through a narrow controlled API.
- Change records must distinguish previous and current values without exposing mutable internal collections.
- Integration with the event bus must preserve state ownership and avoid event-driven request/response behavior.
- Stable identity requirements must be resolved before readiness.

## Out of Scope

Story variables, player profile schemas, phone/app state, save serialization, reflection-driven state discovery, dynamic scripting, editor tooling, and domain-specific validation.

## Dependencies and Prerequisites

- FND-004 is Complete.

## Acceptance Direction

- [ ] Ownership and mutation authority are explicit.
- [ ] The design is strongly typed and avoids a global string/object property bag.
- [ ] Change records are immutable by contract.
- [ ] Event integration announces completed changes only.
- [ ] Repeated isolated construction does not leak static state.

## Readiness Notes

This is a provisional backlog ticket. The Technical Director must resolve the typed-container versus stable-key model and refine validation, file authority, tests, and acceptance criteria before promotion to `Ready`.
