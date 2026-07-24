# Testing Strategy

**Version:** 1.1
**Status:** Draft
**Owner:** Technical Director
**Last Updated:** 2026-07-24
**Related Documents:** [`Definition_of_Done.md`](Definition_of_Done.md), [`../Project/CODING_STANDARDS.md`](../Project/CODING_STANDARDS.md), [`../Runbooks/UNITY_5_6_VALIDATION.md`](../Runbooks/UNITY_5_6_VALIDATION.md), [`../Process/UNITY_EXECUTION_MODEL.md`](../Process/UNITY_EXECUTION_MODEL.md)

## Purpose

Testing protects authored content, persistent state, runtime behavior, and production workflows. Project Minerva uses the smallest effective validation layer for each risk.

## Validation Layers

### Static Validation

Used for authored data, references, identifiers, conditions, effects, dependencies, timestamps, and unsupported configuration. Invalid content should fail before runtime whenever possible.

### Unit Tests

Used for deterministic logic such as condition evaluation, state changes, scheduling, identifier resolution, save migrations, and branch selection.

### Editor Tests

Used for importers, validators, asset creation, tool commands, undo behavior, and editor-only workflows.

### Integration Tests

Used for interactions among time, narrative, notification, save, app, and event systems.

### Play-Mode and Manual Checks

Used for Unity lifecycle behavior, UI interaction, platform-specific behavior, visual transitions, and workflows that cannot yet be automated reliably in Unity 5.6.

## Risk Priorities

Highest priority areas include:

- save corruption or incompatible migration;
- story branches selecting incorrectly;
- scheduled events firing twice or not at all;
- state changes that cannot be explained;
- authoring data accepted despite broken references;
- editor code entering player builds;
- platform lifecycle loss on iOS;
- non-deterministic replay or debugging.

## Test Design

Tests should be deterministic, focused, readable, and independent. Use fixtures that express meaningful story and simulation cases. Avoid tests coupled to incidental implementation details.

## Ticket Requirements

Every implementation ticket should identify required validation. When automation is not available, the ticket must specify reproducible manual steps and expected results.

## Reporting

Implementation reports must distinguish:

- checks run and passed;
- checks run and failed;
- checks unavailable in the environment;
- checks recommended but outside scope.

## Local Unity 5.6 Verification

Unity runtime code, EditMode tests, editor tooling, scenes, prefabs, and imported assets require validation in an environment with the approved Unity version. Cloud repository access does not establish that capability.

The standard local EditMode workflow is:

```bash
./Tools/Verification/verify-unity56-editmode.sh
```

Follow the [Unity 5.6 Validation Runbook](../Runbooks/UNITY_5_6_VALIDATION.md). Acceptance requires the ticket-defined validation evidence. For automated EditMode tests, the results XML—not the Unity process exit code alone—is authoritative.

## Regression Policy

A defect fix should include a regression test when the behavior can be tested reliably. When it cannot, document the reason and add the smallest repeatable manual verification.

## Evolution

Testing infrastructure will grow with the roadmap. Early phases prioritize pure logic and validation seams so later runtime and tool systems can be tested without requiring full scenes or complete phone UI.
