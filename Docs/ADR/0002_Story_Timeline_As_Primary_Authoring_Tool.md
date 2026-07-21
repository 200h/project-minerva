# ADR-0002: Story Timeline as Primary Authoring Tool

**Status:** Accepted  
**Date:** 2026-07-21  
**Owners:** Product Owner, Technical Director  
**Related Documents:** [`../Project/PROJECT_VISION.md`](../Project/PROJECT_VISION.md), [`../Project/ARCHITECTURAL_PRINCIPLES.md`](../Project/ARCHITECTURAL_PRINCIPLES.md)

## Context

Project Minerva needs a scalable way to author chronological narrative events, branches, conditions, effects, references, and persistent state without constructing story logic separately inside each phone app.

## Decision

The Story Timeline will be the primary narrative-authoring environment and authoritative source for supported story events.

## Rationale

A single chronological authoring surface supports the project’s content-first workflow, object filtering, branch visibility, validation, and the goal that routine story production requires no new C# code.

## Alternatives Considered

### Author story content inside individual apps

Rejected because it duplicates logic, hides chronology, and makes cross-app narrative state difficult to validate.

### Use only a node graph

Rejected as the primary surface because the intended workflow is linear and timestamped. Graph views may support branch inspection but do not replace the timeline.

### Store story logic directly in code

Rejected because it prevents efficient narrative production and weakens validation and tooling.

## Consequences

### Positive

- one authoritative authoring workflow;
- consistent typed events and validation;
- filters by character, app, branch, variable, and other objects;
- direct path from authored data to runtime execution.

### Negative

- significant editor-tool investment is required early;
- event schemas must be designed carefully;
- timeline usability becomes production-critical.

### Risks

- overloading the timeline with runtime debugging responsibilities;
- schemas becoming too rigid or too generic;
- insufficient branch and merge visualization.

## Compatibility and Migration

Authored definitions must use stable identifiers and versionable schemas. Changes to event formats require migration or compatibility planning.

## Validation

The vertical slice must prove that a supported event can be authored, validated, executed, saved, loaded, and debugged without new C# code.

## Follow-Up Work

- define timeline data model;
- define typed event contract;
- specify validation and compilation workflow;
- design branch and filter views.

## Supersedes / Superseded By

None.
