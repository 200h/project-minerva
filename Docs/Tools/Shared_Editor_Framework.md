# Shared Editor Framework

**Status:** Draft  
**Owner:** Technical Director

## Purpose

Provide reusable editor-only infrastructure for selection, filtering, cross-navigation, diagnostics, persistence of tool layout, and stable entity references.

## Responsibilities

- shared stable-identifier selection model;
- filter definitions and query composition;
- cross-tool navigation intents;
- common list, tree, timeline, inspector, and diagnostic patterns;
- editor-window lifecycle and state restoration;
- safe undo/redo integration for authored changes;
- shared test fixtures and editor diagnostics.

## Does Not Own

Narrative definitions, runtime state, system business rules, or final product UI components.

## Design Rules

- common infrastructure must remain domain-neutral;
- Story Timeline and Game Director keep separate authority;
- shared selection refers to stable identifiers, not concrete window or scene instances;
- editor state corruption must not corrupt authored data;
- framework APIs remain compatible with Unity 5.6 editor capabilities;
- abstractions are introduced only after a demonstrated shared need.

## Integration

Tools register entity descriptors, filters, inspectors, and navigation handlers through narrow contracts. Unsupported entity types remain visible as unknown rather than being dropped silently.

## Persistence

May persist editor layout, active filters, selected identifiers, and recent navigation. This data is development-only and never enters player saves.

## Validation

Editor tests should cover selection propagation, filter determinism, undo safety, window reload, stale identifiers, and separation from runtime builds.