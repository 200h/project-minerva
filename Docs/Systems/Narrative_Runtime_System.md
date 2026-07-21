# Narrative Runtime System

**Status:** Draft  
**Owner:** Technical Director

## Purpose

Evaluate validated narrative definitions against authoritative state and execute supported timeline events deterministically.

## Responsibilities

- load compiled narrative definitions;
- evaluate conditions and prior-choice gates;
- determine eligible events;
- apply supported effects through owning systems;
- record choices, execution, branch progress, and merge progress;
- coordinate timestamped and delayed narrative work;
- publish completed narrative facts;
- capture and restore execution state.

## Does Not Own

Source authoring, world-variable storage, simulation time, phone rendering, or save-file storage.

## Inputs

Compiled definitions, time/due-item facts, player-choice commands, app interaction commands, world-state queries, and restore payloads.

## Outputs

Commands to owning systems, execution results, choice/branch events, validation/runtime errors, and versioned state.

## Dependencies

World State, Time and Clock, Event Bus, and narrow app/notification command contracts. Dependencies are explicit and cannot be discovered globally.

## Invariants

- one authoritative execution record per event occurrence;
- conditions are side-effect free;
- effects apply atomically or report failure without ambiguous partial progress;
- branch merges preserve prior consequences while rejoining chronology;
- unsupported event types fail clearly.

## Persistence

Persist completed and pending event identifiers, choices, branch/merge state, correlation data, and any execution cursor required to resume deterministically.

## Diagnostics

Expose eligibility decisions, failed conditions, applied effects, source event identifiers, timestamps, and causation chains for Game Director.