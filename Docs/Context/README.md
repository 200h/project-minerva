# Project Minerva Context Indexes

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21

## Purpose

Context indexes provide maintained, domain-specific reading sets for tickets and reviewers. They reduce repository-wide exploration while preserving the exact authority chain needed for a task.

## Rules

- The ticket remains the primary authority.
- A ticket names the exact context index and sections required.
- Reading an index does not authorize modifying linked files.
- Required context is read completely; optional context is opened only for a specific question.
- Indexes link to approved documents and later may link to exact code areas.
- Do not interpret an index as permission to read the entire repository.

## Indexes

- [`Foundation.md`](Foundation.md)
- [`Runtime.md`](Runtime.md)
- [`Narrative.md`](Narrative.md)
- [`Tooling.md`](Tooling.md)
- [`Persistence.md`](Persistence.md)
- [`Phone_UI.md`](Phone_UI.md)

## Maintenance

Update an index when authoritative documents or implementation paths change. Remove obsolete links rather than allowing stale context to accumulate.