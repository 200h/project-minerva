# Project Minerva Architecture

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21

## Purpose

This directory defines Project Minerva's approved technical structure. These documents translate the Project Constitution and accepted ADRs into concrete boundaries for runtime systems, editor tools, data, persistence, initialization, and dependencies.

Architecture documents are authoritative below the Constitution and ADRs and above system specifications, tickets, and implementation.

## Documents

- [`SYSTEM_OVERVIEW.md`](SYSTEM_OVERVIEW.md) — complete architectural model and major responsibilities.
- [`REPOSITORY_STRUCTURE.md`](REPOSITORY_STRUCTURE.md) — intended Unity and documentation layout.
- [`RUNTIME_LAYERS.md`](RUNTIME_LAYERS.md) — architectural layers and permitted responsibilities.
- [`DEPENDENCY_RULES.md`](DEPENDENCY_RULES.md) — allowed dependency direction and prohibited coupling.
- [`STATE_OWNERSHIP.md`](STATE_OWNERSHIP.md) — ownership of authored data, mutable world state, UI state, and derived views.
- [`EVENT_ARCHITECTURE.md`](EVENT_ARCHITECTURE.md) — event contracts, routing, ordering, and diagnostics.
- [`DATA_AND_SERIALIZATION.md`](DATA_AND_SERIALIZATION.md) — identifiers, schemas, validation, persistence, and migration.
- [`EDITOR_RUNTIME_BOUNDARIES.md`](EDITOR_RUNTIME_BOUNDARIES.md) — separation of editor-only and player-build code.
- [`INITIALIZATION_AND_LIFECYCLE.md`](INITIALIZATION_AND_LIFECYCLE.md) — startup, runtime phases, pause/resume, save/load, and shutdown.

## Reading Order

Read `SYSTEM_OVERVIEW.md` first, followed by the specific boundary documents needed for a design or ticket. System and tool specifications should cite the exact architecture documents they depend on.

## Change Control

Material changes to state ownership, dependency direction, persistence contracts, engine boundaries, or primary tool authority require an ADR before this baseline is changed.