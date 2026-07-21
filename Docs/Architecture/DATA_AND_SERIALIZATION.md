# Data and Serialization

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21

## Goals

Project Minerva data must be inspectable, versionable, deterministic, compatible with Unity 5.6, and safe to migrate.

## Data Domains

- **Source definitions** — authoritative authored content.
- **Compiled definitions** — validated runtime-ready representation derived from source definitions.
- **Runtime state** — mutable authoritative playthrough state.
- **Presentation state** — transient view concerns, persisted only when explicitly valuable.
- **Save envelope** — version metadata and per-system state payloads.

## Stable Identifiers

Characters, apps, threads, events, branches, variables, assets, and other cross-referenced entities use stable identifiers independent of display names, file positions, and Unity instance IDs. Identifiers are never silently regenerated after use in content or saves.

## Schema Rules

- Every persistent format has an explicit schema version.
- Required and optional fields are documented.
- Unknown or invalid identifiers produce actionable validation errors.
- Runtime code consumes validated data rather than repeatedly interpreting raw author input.
- Derived data is reproducible and should not become a competing source of truth.

## Serialization Boundaries

Each state-owning system provides a versioned capture and restore contract. The Save/Load System coordinates these contracts and owns the envelope, ordering, integrity checks, storage, and migration sequence.

Do not serialize scene graphs, delegates, service references, Unity instance IDs, editor objects, or transient subscriptions.

## Migration

A format change that affects existing content or saves must provide one of:

- backward-compatible reading;
- a deterministic migration;
- an explicit incompatibility decision approved through an ADR.

Migrations must be testable using fixed fixtures and must not depend on current wall-clock time or mutable external services.

## Validation Pipeline

Source content progresses through parse/import, structural validation, reference validation, semantic validation, and compilation. Errors include location, entity identifier, rule, and remediation guidance where possible.

## Security and Integrity

Save and content readers validate sizes, versions, required fields, and references before mutation. Partial restoration must not leave the running simulation in an ambiguous state.

## Format Decisions

This baseline does not choose JSON, binary, ScriptableObject, or another concrete format. That decision requires Unity 5.6 compatibility testing and a focused ADR or system design.