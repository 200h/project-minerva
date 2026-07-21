# World State System

**Status:** Draft  
**Owner:** Technical Director

## Purpose

Own authoritative story variables, facts, progress markers, branch history references, and change provenance for the active playthrough.

## Responsibilities

- register and validate variable definitions;
- expose typed read queries;
- apply validated state-change commands atomically;
- enforce type and domain constraints;
- publish completed change events;
- capture and restore versioned state;
- retain diagnostic provenance for important changes.

## Does Not Own

Authored narrative definitions, simulation time, event eligibility, UI state, save storage, or phone-screen rendering.

## Inputs

Validated variable definitions, mutation commands, and restore payloads.

## Outputs

Immutable query results, state-change domain events, validation failures, and versioned save state.

## Dependencies

Core identifiers, Event Bus contract, and validated authored-data contracts. It must not depend on presentation, editor tools, or concrete app views.

## Invariants

- one value per stable variable identifier;
- values match declared types and constraints;
- failed mutations do not partially apply;
- external consumers cannot mutate internal collections;
- restored state is validated before becoming active.

## Persistence

Persist authoritative values and required provenance/version metadata. Derived summaries are rebuilt after load.

## Diagnostics

Expose variable identifier, prior value, new value, cause/correlation identifier, and simulation timestamp where applicable.

## Extension Points

New value types require explicit serialization, comparison, validation, and migration support.