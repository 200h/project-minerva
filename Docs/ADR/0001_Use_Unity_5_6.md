# ADR-0001: Use Unity 5.6

**Status:** Accepted  
**Date:** 2026-07-21  
**Owners:** Product Owner, Technical Director  
**Related Documents:** [`../Project/PROJECT_VISION.md`](../Project/PROJECT_VISION.md), [`../Project/CODING_STANDARDS.md`](../Project/CODING_STANDARDS.md)

## Context

Project Minerva requires a stable engine target for a UI-heavy, simulation-driven narrative game shipping on Windows and iOS. The project owner explicitly selected Unity 5.6.

## Decision

Project Minerva will be designed and implemented for Unity 5.6 unless a future accepted ADR changes the engine target.

## Rationale

A fixed target prevents accidental use of modern Unity APIs, packages, serialization behavior, and editor workflows that are unavailable to the project. It also forces compatibility to be considered during architecture and ticket preparation rather than discovered late.

## Alternatives Considered

### Upgrade to a modern Unity release

Rejected because it contradicts the approved project constraint and would change compatibility, tooling, platform, and migration assumptions.

### Engine-agnostic design

Rejected as the sole approach because production implementation and editor tools must account for concrete Unity 5.6 behavior.

## Consequences

### Positive

- stable compatibility target;
- predictable implementation constraints;
- reduced architectural ambiguity.

### Negative

- limited modern APIs and language features;
- older editor tooling and platform integrations;
- additional verification burden for third-party assets.

### Risks

- accidental use of newer APIs;
- unsupported modern packages;
- outdated iOS integration requirements.

## Compatibility and Migration

All code, tools, serialization, dependencies, and tests must verify Unity 5.6 support. A future engine upgrade requires a separate ADR and migration plan.

## Validation

Coding standards, ticket review, compilation, and targeted platform checks enforce this decision.

## Follow-Up Work

- document exact scripting runtime and C# compatibility;
- establish Unity project settings during Phase 0/1;
- evaluate third-party assets individually.

## Supersedes / Superseded By

None.
