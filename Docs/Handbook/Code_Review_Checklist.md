# Code Review Checklist

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`Definition_of_Done.md`](Definition_of_Done.md), [`Testing_Strategy.md`](Testing_Strategy.md), [`../Project/CODING_STANDARDS.md`](../Project/CODING_STANDARDS.md)

## Scope

- Does the change match the approved ticket?
- Are unrelated changes absent?
- Were only authorized paths modified?
- Are future ideas reported rather than implemented?

## Architecture

- Is state owned by the correct system?
- Does dependency direction follow approved boundaries?
- Are runtime and editor concerns separated?
- Are Story Timeline and Game Director responsibilities preserved?
- Is authoritative data duplicated?

## Unity 5.6 Compatibility

- Are APIs and C# syntax supported?
- Is player code free of `UnityEditor` dependencies?
- Are serialization and platform assumptions valid for Windows and iOS?
- Has uncertain compatibility been documented?

## Correctness

- Are normal, edge, and failure paths addressed?
- Are stable identifiers preserved?
- Are save/load implications handled?
- Are events, conditions, and effects deterministic where required?
- Are failures visible and actionable rather than silent?

## Maintainability

- Are names clear and consistent with the Glossary?
- Are classes and methods focused?
- Are abstractions justified by current requirements?
- Is vendor-specific code isolated?
- Is logging useful and appropriately scoped?

## Validation

- Were required tests or checks run?
- Are results reported accurately?
- Do tests verify behavior rather than implementation trivia?
- Are manual checks documented when automation is unavailable?

## Documentation

- Are affected contracts and workflows documented?
- Do comments explain intent rather than restate code?
- Are links and metadata current?
- Does documentation describe actual behavior?

## Review Outcome

Approve only when the change is safe, scoped, understandable, and meets the Definition of Done. Otherwise request concrete changes tied to the ticket or governing documentation.
