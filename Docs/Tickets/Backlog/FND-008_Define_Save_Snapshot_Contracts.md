# FND-008: Define Save Snapshot Contracts

**Status:** Backlog  
**Owner:** Unassigned  
**Created:** 2026-07-23  
**Updated:** 2026-07-23  
**Roadmap Phase:** Phase 1 — Foundation  
**Epic:** [Foundation Runtime Services](../../../Planning/Epics/Foundation_Runtime_Services.md)  

## Summary

Define provisional generic contracts for versioned runtime snapshots without implementing persistence or platform storage.

## Goal

Establish how state-owning services contribute validated snapshot data and restore it in deterministic order.

## Provisional Boundaries

- Snapshot data must be versioned explicitly.
- Each service remains responsible for its own authoritative state schema.
- Capture and restore ordering must be deterministic and failure-aware.
- Validation must reject incompatible, missing, or malformed contributions clearly.
- Snapshot contracts must not assume a file format or storage location.

## Out of Scope

File I/O, autosave policy, slots, compression, encryption, cloud synchronization, platform paths, migration UI, narrative schemas, phone/app schemas, event sourcing, and replay.

## Dependencies and Prerequisites

- FND-007 is Complete.

## Acceptance Direction

- [ ] Version and compatibility information are explicit.
- [ ] Snapshot ownership remains with contributing services.
- [ ] Capture and restore ordering are deterministic.
- [ ] Failure results identify the contribution and reason.
- [ ] Contracts are independent of serialization format and disk storage.

## Readiness Notes

This is a provisional backlog ticket. The Technical Director must refine contribution identity, version compatibility, restore transaction semantics, authorized paths, tests, and acceptance criteria before promotion to `Ready`.
