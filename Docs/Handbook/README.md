# Project Minerva — Development Handbook

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`../Project/README.md`](../Project/README.md), [`../Project/AI_ENGINEERING_GUIDE.md`](../Project/AI_ENGINEERING_GUIDE.md)

## Purpose

The Development Handbook converts the Project Constitution into repeatable day-to-day practices. It governs how work is proposed, prepared, implemented, reviewed, validated, and completed.

The Handbook does not override the Constitution. When a handbook procedure conflicts with an approved constitutional document, the Constitution takes precedence.

## Contents

- [`Git_Workflow.md`](Git_Workflow.md) — repository operations from branch creation through merge and cleanup.
- [`Branch_Strategy.md`](Branch_Strategy.md) — branch types, naming, ownership, and lifecycle.
- [`Definition_of_Done.md`](Definition_of_Done.md) — completion requirements for documentation, code, tools, and content systems.
- [`Documentation_Standards.md`](Documentation_Standards.md) — structure, metadata, authority, links, and maintenance expectations.
- [`Ticket_Workflow.md`](Ticket_Workflow.md) — movement from Backlog through Complete.
- [`Code_Review_Checklist.md`](Code_Review_Checklist.md) — review criteria for correctness, scope, compatibility, and maintainability.
- [`Testing_Strategy.md`](Testing_Strategy.md) — layered validation appropriate to Unity 5.6 and Project Minerva.

## Operating Principle

> Work should be small enough to understand, explicit enough to review, and complete enough to trust.

## Authority

Tickets authorize work. Branches isolate work. Pull requests expose work for review. Validation demonstrates behavior. Documentation preserves intent.

No step alone is sufficient. A technically correct implementation that exceeds ticket scope or leaves required documentation stale is not complete.

## Intended Audience

This handbook applies to:

- the Product Owner and Creative Director;
- the Technical Director;
- human contributors;
- Codex and other AI implementation agents;
- reviewers and maintainers.

## Change Control

Handbook updates should be made through pull requests. Material changes to architectural authority, state ownership, platform support, or production philosophy belong in the Constitution or an ADR rather than only in this directory.
