# Repository Structure

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21

## Purpose

Define the intended repository layout before implementation creates Unity folders or assembly boundaries.

## Top-Level Layout

```text
Assets/
Docs/
Planning/
ProjectSettings/
```

`Docs/` and `Planning/` are already authoritative. `Assets/` and `ProjectSettings/` will be created through approved implementation tickets.

## Unity-Owned Project Area

```text
Assets/
└── Minerva/
    ├── Runtime/
    │   ├── Core/
    │   ├── Data/
    │   ├── Events/
    │   ├── Narrative/
    │   ├── Persistence/
    │   ├── Phone/
    │   ├── Simulation/
    │   └── UI/
    ├── Editor/
    │   ├── Common/
    │   ├── StoryTimeline/
    │   ├── GameDirector/
    │   └── Validation/
    ├── Tests/
    │   ├── Editor/
    │   ├── Runtime/
    │   └── Fixtures/
    ├── Content/
    │   ├── Definitions/
    │   ├── Localization/
    │   └── TestContent/
    └── Resources/
```

## Rules

- All project-owned C# uses the `Minerva` namespace root.
- Runtime code must not reference `UnityEditor` or files under `Assets/Minerva/Editor/`.
- Editor tools may depend on runtime contracts but must not become runtime authorities.
- Third-party assets live outside `Assets/Minerva/` unless an approved wrapper is project-owned.
- Generated content must be clearly separated from authoritative source content.
- Tests mirror the runtime or editor domain they validate.
- `Resources/` is restricted to content that must use Unity's Resources loading model; it is not a general asset folder.

## Documentation Areas

```text
Docs/
├── Project/
├── Handbook/
├── Architecture/
├── Systems/
├── Tools/
├── Context/
├── ADR/
├── Tickets/
└── Templates/
```

## Assembly Boundaries

The exact assembly mechanism must be verified against Unity 5.6. Logical boundaries still apply even if modern assembly-definition assets are unavailable or unsuitable. Tickets must not assume asmdef support without verification.

## Naming

Folders use PascalCase in Unity-owned areas and established repository naming elsewhere. File names follow the relevant coding or documentation standards.

## Creation Rule

This document authorizes no filesystem changes by itself. The Unity skeleton must be created through a reviewed ticket with exact paths and compatibility validation.