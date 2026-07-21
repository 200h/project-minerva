# Phone UI Context

Use for phone applications, navigation, notifications, app lifecycle, routes, and player-facing UI integration.

## Required Core

- `Docs/Project/PROJECT_VISION.md`
- `Docs/Architecture/RUNTIME_LAYERS.md`
- `Docs/Architecture/DEPENDENCY_RULES.md`
- `Docs/Architecture/STATE_OWNERSHIP.md`
- `Docs/Systems/App_Runtime_System.md`
- `Docs/Systems/UI_Navigation_System.md`

## Add by Task

- `Docs/Systems/Notification_System.md` for badges, delivery, or deep links.
- `Docs/Architecture/INITIALIZATION_AND_LIFECYCLE.md` for backgrounding and resume.
- relevant app-domain specification when it exists.

## Boundaries

Phone screens render read models and submit explicit commands. They do not own narrative truth, duplicate app-domain state, or mutate simulation collections directly.

## Persistence

Treat screen stack, focus, animations, and scroll state as transient unless a ticket explicitly justifies restoration.