# ADR-0003: Separate Story Timeline and Game Director

**Status:** Accepted  
**Date:** 2026-07-21  
**Owners:** Product Owner, Technical Director  
**Related Documents:** [`0002_Story_Timeline_As_Primary_Authoring_Tool.md`](0002_Story_Timeline_As_Primary_Authoring_Tool.md), [`../Project/GLOSSARY.md`](../Project/GLOSSARY.md)

## Context

Project Minerva requires both a narrative-authoring environment and a runtime inspection suite. Combining them would blur authority between authored definitions and observed mutable state.

## Decision

The Story Timeline and Game Director will remain separate tools with explicit boundaries.

The Story Timeline authors and edits narrative definitions. The Game Director observes, explains, replays, and debugs runtime simulation state.

## Rationale

Separation keeps authoring authoritative, runtime inspection trustworthy, and each interface focused on a distinct user question: what should happen versus what is happening and why.

## Alternatives Considered

### One combined director tool

Rejected because editing and observation would become entangled, increasing the risk that debug controls silently alter authored truth.

### Runtime logs only

Rejected because logs cannot provide the interactive state, queue, branch, save, and phone-surface inspection required by production.

## Consequences

### Positive

- clear authority boundaries;
- focused UX for authors and debuggers;
- safer runtime inspection;
- easier testing and modular development.

### Negative

- some shared infrastructure is required;
- cross-navigation between tools must be designed;
- two interfaces must remain consistent.

### Risks

- duplicated views or filters;
- Game Director accidentally gaining authoring authority;
- Story Timeline accumulating runtime-only controls.

## Compatibility and Migration

Shared identifiers, selection contracts, and inspection interfaces may be reused, but authored definitions and runtime state remain separate data domains.

## Validation

Tool specifications and reviews must map every operation to one authority. Runtime changes made for debugging must be explicit, temporary, and never silently rewrite authored data.

## Follow-Up Work

- define shared inspector and selection frameworks;
- specify Game Director runtime contracts;
- define safe replay and debug-control behavior.

## Supersedes / Superseded By

None.
