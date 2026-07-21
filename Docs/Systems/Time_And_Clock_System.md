# Time and Clock System

**Status:** Draft  
**Owner:** Technical Director

## Purpose

Own authoritative simulation time, delayed work, schedules, pause policy, and deterministic catch-up after suspension.

## Responsibilities

- expose current simulation time;
- advance time through explicit commands;
- schedule, cancel, and execute delayed items;
- preserve deterministic ordering for equal timestamps;
- distinguish simulation pause from presentation pause;
- calculate approved catch-up after backgrounding;
- capture and restore time and schedule state.

## Does Not Own

Narrative meaning, notification presentation, wall-clock platform access, or app-specific state.

## Inputs

Platform time readings through an adapter, advancement commands, schedule requests, cancellation requests, and restore payloads.

## Outputs

Time-advanced events, due-item facts, schedule diagnostics, and versioned persistence state.

## Dependencies

Core identifiers, Event Bus, and a narrow platform clock interface.

## Invariants

- simulation time never moves backward except through an explicit development restore;
- equal-time work uses stable ordering;
- due work executes once unless its contract explicitly allows repetition;
- pause and catch-up behavior is explicit and testable.

## Persistence

Persist current simulation time, schedule entries, stable ordering data, and lifecycle timestamps required for catch-up.

## Failure Behavior

Invalid timestamps, duplicate schedule identifiers, or incompatible restored entries fail with actionable diagnostics rather than being silently skipped.