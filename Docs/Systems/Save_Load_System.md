# Save/Load System

**Status:** Draft  
**Owner:** Technical Director

## Purpose

Coordinate versioned, atomic persistence and restoration of authoritative runtime state.

## Responsibilities

- define and validate the save envelope;
- request versioned state from participating systems;
- write saves through a platform storage adapter;
- verify integrity before replacement;
- migrate supported prior versions;
- restore systems in approved dependency order;
- coordinate autosave policy and failure reporting.

## Does Not Own

Business meaning of captured fields, authored definitions, simulation rules, or arbitrary scene serialization.

## Inputs

Save commands, load commands, per-system capture/restore contracts, compiled definition version information, and platform storage results.

## Outputs

Save/load lifecycle events, success/failure results, migration diagnostics, and restored-session coordination.

## Invariants

- a failed write does not destroy the last valid save;
- a failed load does not leave partially restored active state;
- every payload identifies schema and owning system version;
- unknown required systems or incompatible versions fail clearly;
- transient view objects and service references are never serialized.

## Autosave

Autosave triggers are explicit, debounced where necessary, and never run during an unsafe partial state transition. Autosave failure is visible but must not silently corrupt the session.

## Extension Points

Additional slots, cloud adapters, compression, or integrity mechanisms may be added behind existing contracts after compatibility review.