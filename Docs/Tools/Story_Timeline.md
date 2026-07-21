# Story Timeline

**Status:** Draft  
**Owner:** Technical Director

## Purpose

Provide the primary authoring environment for chronological narrative definitions, conditions, effects, choices, branches, merge points, schedules, and references.

## Authority

Story Timeline edits authoritative source definitions. It does not own live world state and does not use Game Director state as authored truth.

## Core Capabilities

- linear timestamped event authoring;
- typed event creation and editing;
- filtering by character, app, thread, branch, variable, asset, and event type;
- immediate player choices and prior-choice gating;
- persistent state effects and conditions;
- branch merge/rejoin representation;
- stable identifier and reference management;
- validation and compilation;
- preview and cross-navigation to related definitions.

## Inputs

Author actions, source definitions, schema/registry contracts, asset references, and validation results.

## Outputs

Updated source definitions, validation reports, compiled runtime definitions, and selection/navigation intents for supporting tools.

## Boundaries

- no player-build dependency;
- no silent mutation of live saves or runtime state;
- no app-specific duplicate authoring authority;
- no direct reliance on scene-object identity;
- no compilation when blocking validation errors remain.

## UX Principles

Chronology is the primary mental model. Branch views support, rather than replace, the timeline. Filters must preserve context and make hidden dependencies visible. Destructive changes require clear impact information.

## Integration

Uses Shared Editor Framework for selection/filtering and Validation Framework for diagnostics. May launch Game Director at a matching identifier or timestamp, but Game Director does not write back authored truth.

## Validation

The vertical slice must prove authoring, validation, compilation, runtime execution, save/load, and debugging for supported event types without new C#.