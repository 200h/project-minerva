# App Runtime System

**Status:** Draft  
**Owner:** Technical Director

## Purpose

Own the phone app registry, app availability, lifecycle, permissions, background policy, routes, and app-level runtime metadata.

## Responsibilities

- register apps by stable identifier;
- validate app metadata and routes;
- track installed, available, locked, hidden, foreground, background, and suspended states;
- enforce declared permissions and launch requirements;
- expose app descriptors and route resolution;
- coordinate app lifecycle events;
- capture and restore persistent app runtime state.

## Does Not Own

Global navigation stack, narrative truth, app-specific domain records, notification delivery, or authored story definitions.

## Inputs

Validated app definitions, launch/background/close commands, permission updates from owning workflows, lifecycle notifications, and restore payloads.

## Outputs

App registry queries, route descriptors, lifecycle events, availability changes, diagnostics, and versioned state.

## Invariants

- app identifiers and route identifiers are unique;
- unavailable apps cannot be launched through normal player commands;
- lifecycle transitions follow a documented state machine;
- app screens do not register themselves as authoritative apps;
- runtime state is independent of concrete scene objects.

## Persistence

Persist availability, install/lock state, permissions, and other approved long-lived metadata. Foreground screen animation and ephemeral view state are excluded unless separately justified.

## Extension Points

New app types register through metadata and factories/adapters rather than changes to a central switch statement where practical within Unity 5.6 constraints.