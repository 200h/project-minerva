# System Overview

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21

## Architectural Goal

Project Minerva is a deterministic, data-driven narrative simulation presented through a fictional smartphone operating system. The architecture must allow authored story definitions to drive a shared world simulation while phone applications render and interact with that state without owning isolated narrative logic.

## Core Domains

### Authored Definitions

Immutable, validated content produced by the Story Timeline and supporting authoring tools. This includes event definitions, conditions, effects, references, branches, merge points, schedules, app content, and stable identifiers.

### Runtime Simulation

Mutable authoritative state for the current playthrough. It includes story variables, completed events, branch history, simulated time, scheduled work, conversations, notifications, app data, and player-visible consequences.

### Presentation and Interaction

Phone applications, navigation, views, animations, audio, and input. Presentation reads state and issues explicit commands. It does not redefine story truth.

### Persistence

Versioned snapshots and records required to reconstruct the simulation. Save data stores authoritative state, not arbitrary scene-object graphs or transient presentation state.

### Tooling and Inspection

Editor-only authoring, validation, compilation, preview, and runtime inspection. Story Timeline owns authored definitions. Game Director observes and explains runtime state.

## Major Runtime Systems

- **World State System** — authoritative values, facts, progress, and state-change provenance.
- **Event Bus System** — typed decoupled notification within explicit contracts.
- **Time and Clock System** — authoritative simulation time and scheduled execution.
- **Narrative Runtime System** — evaluates conditions and applies supported narrative events.
- **Save/Load System** — versioned persistence, migration, autosave coordination, and restoration.
- **Notification System** — routes notification requests and tracks delivery state.
- **App Runtime System** — app registry, lifecycle, permissions, background behavior, and routing.
- **UI Navigation System** — screen stack, transitions, focus, back behavior, and presentation state.

## Command and Event Model

Player and tool actions enter the runtime as explicit commands. A responsible system validates and applies each command, mutates its owned state, and publishes facts describing completed changes. Events do not grant arbitrary mutation access to subscribers.

## Determinism

Given the same validated definitions, save state, ordered commands, and time progression, the simulation should produce the same authoritative result. Presentation timing and cosmetic animation may vary but must not alter story outcomes.

## Cross-System Coordination

No general-purpose god manager owns all systems. A small composition root initializes services in an approved order and supplies explicit dependencies. Multi-system workflows are coordinated through commands, typed events, and narrow orchestration services rather than direct access to internal state.

## Failure Model

Invalid authored definitions should fail validation before runtime. Invalid commands, missing references, incompatible save versions, or impossible state transitions must produce actionable diagnostics. Silent fallback is prohibited when it could conceal narrative corruption.

## Extension Model

New story event types, apps, or systems extend registered contracts and validated schemas. Routine content additions must not require changes to core runtime code after the vertical slice supports the relevant event type.

## Non-Goals

This baseline does not define detailed UX, final file formats, concrete C# APIs, or implementation classes. Those belong in system/tool specifications and scoped tickets.