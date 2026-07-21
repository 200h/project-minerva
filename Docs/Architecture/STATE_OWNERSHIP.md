# State Ownership

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21

## Principle

Every mutable fact has one authoritative owner. Other systems receive read models, queries, or events; they do not maintain competing truth.

## State Categories

### Authored Definitions

Owned by source content and Story Timeline authoring. Immutable at runtime except through explicit development-only reload workflows. Definitions use stable identifiers and are validated before execution.

### World State

Owned by the World State System. Includes story variables, facts, branch history, progress markers, and provenance needed to explain changes.

### Simulation Time and Schedules

Owned by the Time and Clock System. Other systems request scheduling or time advancement and receive completed facts.

### Narrative Execution State

Owned by the Narrative Runtime System. Includes event eligibility, execution records, choice outcomes, merge progress, and pending narrative operations.

### App Domain State

Owned by the responsible runtime system, not by screens. Examples include conversations, contacts, messages, notification delivery, and app permissions.

### Navigation and Presentation State

Owned by UI Navigation and views. Includes screen stack, focus, scroll position, open panels, animation progress, and other transient display concerns. It is not story truth.

### Save State

The Save/Load System owns serialization and reconstruction, but it does not become the business owner of captured values. Each system provides and restores its versioned state through explicit contracts.

## Mutation Rules

- State changes occur only through owner-approved commands or methods.
- Collections are not exposed for external mutation.
- Published events describe completed changes and are not permission to mutate the source.
- Derived views may be cached but must be reproducible from authoritative state.
- Debug overrides must be explicit, labeled, reversible, and must not silently rewrite authored content.

## Provenance

Important narrative state changes should retain enough context to answer what changed, when, and which command or event caused it. Game Director consumes this information without owning it.

## Conflicts

When two proposed systems appear to own the same fact, architecture must resolve ownership before implementation begins.