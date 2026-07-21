# Runtime Layers

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21

## Purpose

Define stable responsibility layers so systems can evolve without circular ownership or presentation-driven simulation logic.

## Layers

### 1. Core Contracts

Stable identifiers, result types, clocks, commands, event contracts, validation interfaces, and minimal shared primitives. Core contracts contain no phone-app logic and no editor dependencies.

### 2. Authored Data

Validated immutable definitions, schemas, references, conditions, effects, and compiled narrative content. Authored data describes intended behavior but does not mutate itself.

### 3. Simulation Services

World state, time, narrative execution, scheduling, notifications, app runtime state, and other authoritative gameplay systems. This layer owns mutable story truth.

### 4. Application Services

Narrow workflows that coordinate multiple simulation services, such as loading a save, advancing time through a scheduled boundary, or opening an app in response to a notification. Application services do not replace system ownership.

### 5. Presentation

Phone screens, view models, navigation, animations, audio, and input translation. Presentation observes state and submits commands. It may own transient visual state only.

### 6. Platform Adapters

Unity lifecycle integration, filesystem access, platform notifications, device-safe areas, audio output, and other environment-specific details. Platform adapters implement interfaces defined inward.

### 7. Editor Tooling

Story Timeline, Game Director, validation windows, previews, importers, and inspectors. Editor tooling may use runtime contracts and test harnesses but cannot ship as player authority.

## Rules

- Dependencies point inward toward more stable contracts.
- Lower-level layers never import presentation or editor code.
- Authored definitions are not runtime state containers.
- Presentation cannot mutate authoritative state directly.
- Platform-specific behavior is hidden behind narrow interfaces.
- Cross-layer convenience access is not a valid reason to bypass boundaries.

## Shared Utilities

A utility belongs in the narrowest owning domain. A global `Utils` dumping ground is prohibited. Reuse alone does not justify moving a type into Core.

## Layer Violations

A ticket that requires a prohibited dependency must stop and request an architecture decision. Temporary violations must not be introduced as scaffolding.