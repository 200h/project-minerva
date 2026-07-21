# Project Minerva Tools

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21

## Purpose

Tool specifications define editor-only authoring, validation, preview, and runtime-inspection responsibilities. Tools consume approved runtime and data contracts but do not redefine system ownership.

## Initial Tools

- [`Story_Timeline.md`](Story_Timeline.md) — primary narrative authoring environment.
- [`Game_Director.md`](Game_Director.md) — runtime inspection, replay, and debugging suite.
- [`Validation_Framework.md`](Validation_Framework.md) — shared validation model and reporting.
- [`Shared_Editor_Framework.md`](Shared_Editor_Framework.md) — common selection, filtering, navigation, and editor infrastructure.

## Authority Rule

Story Timeline owns authored narrative definitions. Game Director observes runtime state. Shared infrastructure may connect the tools without merging their authority.

## Implementation Rule

Tool implementation begins only through Ready tickets after runtime contracts and Unity 5.6 editor compatibility have been verified.