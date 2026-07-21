# Backlog

**Last Updated:** 2026-07-21  
**Owner:** Product Owner and Technical Director

## Purpose

This file tracks high-level candidate work before it is sufficiently defined to become a repository ticket.

## Current Backlog

### Phase 0 — Preproduction

- Define repository architecture and top-level Unity project structure.
- Document runtime layer boundaries and dependency direction.
- Document foundational systems: bootstrap, event bus, time, save, data registry, logging, configuration, and resource loading.
- Define curated context indexes under `Docs/Context/`.
- Prepare the first implementation-ready Codex tickets.
- Decide and record exact Unity scripting runtime constraints.
- Establish third-party asset evaluation records.

### Phase 1 — Foundation

- Runtime bootstrap.
- Event framework.
- Time and scheduling framework.
- Save abstraction and versioning.
- Data registry and stable identifier contracts.
- Logging and diagnostics.
- Configuration and serialization foundations.

### Phase 1.5 — Framework

- Screen and app framework.
- State machine framework.
- Timeline framework.
- Selection and inspector framework.
- Command and undo/redo framework.

## Backlog Rules

- Backlog entries are not authorized work.
- Promote an item by creating a complete ticket in `Docs/Tickets/Backlog/`.
- Remove or archive entries when superseded, rejected, or represented by tickets.
