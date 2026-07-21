# Dependency Rules

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21

## Governing Direction

Dependencies flow from volatile outer layers toward stable inner contracts. A component may depend only on contracts required for its responsibility.

## Allowed Relationships

- Presentation may depend on application and read-only simulation contracts.
- Application services may depend on multiple simulation-system interfaces.
- Simulation systems may depend on Core contracts and validated authored-data contracts.
- Platform adapters may implement inward-defined interfaces.
- Editor tools may depend on runtime contracts, authored-data contracts, and editor-only shared tooling.
- Tests may depend on the exact subjects and fixtures they validate.

## Prohibited Relationships

- Runtime code referencing `UnityEditor`.
- Core contracts referencing phone apps, scenes, prefabs, or tools.
- Simulation systems depending on concrete views or navigation screens.
- One system mutating another system's internal collections.
- Phone apps owning separate copies of narrative truth.
- Game Director becoming a required runtime service.
- Story Timeline depending on live runtime state to define authoritative content.
- Vendor-specific APIs spreading beyond approved adapters.
- Circular dependencies between systems.

## Interaction Patterns

Use, in priority order:

1. direct dependency on a narrow interface when synchronous collaboration is required;
2. command submission when requesting an owned state change;
3. typed events when announcing a completed fact to unknown subscribers;
4. immutable query/read models for presentation and inspection.

Do not use the event bus to disguise a required synchronous dependency or to create hidden request/response flows.

## Composition Root

Concrete services are assembled in one controlled startup area. Services must not discover one another through scene searches, global mutable registries, or implicit singleton creation.

## Public Contracts

Public interfaces must be small, documented, and stable enough for their consumers. Internal implementation types remain private where possible.

## Enforcement

Architecture reviews, narrow symbol searches, compile boundaries, and tests should detect violations. Any deliberate exception requires an accepted ADR and a documented removal or permanence decision.