# Persistence Context

Use for save/load, migration, serialization, stable identifiers, autosave, or restoration tasks.

## Required Core

- `Docs/Architecture/STATE_OWNERSHIP.md`
- `Docs/Architecture/DATA_AND_SERIALIZATION.md`
- `Docs/Architecture/INITIALIZATION_AND_LIFECYCLE.md`
- `Docs/Systems/Save_Load_System.md`
- `Docs/Project/CODING_STANDARDS.md`

## Add by State Owner

Read only the system specifications whose state is captured or restored by the ticket, such as:

- `Docs/Systems/World_State_System.md`
- `Docs/Systems/Time_And_Clock_System.md`
- `Docs/Systems/Narrative_Runtime_System.md`
- `Docs/Systems/Notification_System.md`
- `Docs/Systems/App_Runtime_System.md`

## Required Evidence

Persistence tickets should include round-trip fixtures, version checks, failure-path validation, and migration evidence when applicable.

## Exclusions

Do not serialize transient views, services, subscriptions, delegates, scene graphs, editor objects, or Unity instance IDs.