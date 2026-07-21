# Editor and Runtime Boundaries

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21

## Principle

Player builds must operate without editor assemblies, windows, asset-database access, or development-only mutation authority.

## Runtime Responsibilities

Runtime code owns simulation contracts, validated data consumption, commands, events, persistence, phone behavior, and player-facing presentation. It must compile without `UnityEditor` references.

## Editor Responsibilities

Editor code owns authoring windows, inspectors, importers, validation UI, previews, compilation commands, development fixtures, and Game Director interfaces. It may invoke runtime contracts through controlled adapters and test harnesses.

## Shared Contracts

Shared contracts must live in runtime-safe locations and contain no editor-only types. Editor tools may reference these contracts; runtime code may not reference concrete editor implementations.

## Story Timeline Boundary

Story Timeline edits source definitions and produces validated or compiled outputs. It does not mutate a live player's authoritative state as part of normal authoring.

## Game Director Boundary

Game Director observes runtime state through inspection interfaces. Development overrides are explicit commands, clearly labeled, excluded from production authority, and unable to silently modify source definitions.

## Asset Access

Editor tools may use Unity asset APIs to locate and modify source assets. Runtime code receives references or compiled data through approved loading mechanisms and does not call the AssetDatabase.

## Build Safety

Editor files reside under editor-only folders or equivalent Unity 5.6-safe boundaries. Build validation should detect runtime references to editor namespaces and editor-only assets.

## Test Boundaries

Editor tests may exercise import and tooling behavior. Runtime tests must validate player-safe contracts without requiring editor windows.

## Exceptions

Any proposed shared implementation that risks editor code entering player builds requires architecture review before implementation.