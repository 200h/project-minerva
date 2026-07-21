# ADR-0004: Ticket-Driven AI Implementation

**Status:** Accepted  
**Date:** 2026-07-21  
**Owners:** Product Owner, Technical Director  
**Related Documents:** [`../Project/AI_ENGINEERING_GUIDE.md`](../Project/AI_ENGINEERING_GUIDE.md), [`../Handbook/Ticket_Workflow.md`](../Handbook/Ticket_Workflow.md)

## Context

Project Minerva will use Codex and other AI agents for implementation. Unbounded repository exploration and vague prompts create architectural drift, excessive context, unrelated changes, and unverifiable claims.

## Decision

AI implementation will be driven by repository-based Markdown tickets containing curated context, explicit file authority, requirements, exclusions, validation, stop conditions, and reporting requirements.

## Rationale

Tickets provide a durable contract between architecture and implementation. They keep AI work scoped, reviewable, reproducible, and aligned with the Constitution.

## Alternatives Considered

### Conversational prompts without repository tickets

Rejected because intent, constraints, and decisions are difficult to preserve and audit.

### Full-repository context for every task

Rejected because it increases distraction, cost, and unrelated modification risk.

### AI-directed architecture

Rejected because product and architecture authority remain with the Product Owner and Technical Director.

## Consequences

### Positive

- explicit implementation authority;
- controlled context windows;
- consistent acceptance and reporting;
- traceable work history;
- safer use of AI across a long-lived project.

### Negative

- ticket preparation requires discipline;
- poorly written tickets must be refined before work begins;
- some tasks will stop rather than guess.

### Risks

- tickets becoming too broad;
- optional context being treated as authority;
- acceptance criteria that cannot be validated;
- implementation reports becoming ceremonial rather than factual.

## Compatibility and Migration

This process applies to all future AI implementation. Existing work performed outside the process should be documented or ticketed before significant extension.

## Validation

Review tickets for readiness before execution. Review implementation diffs against authorized paths and compare reports with actual validation evidence.

## Follow-Up Work

- maintain the ticket template and workflow directories;
- create curated context indexes after architecture documents exist;
- issue the first Codex ticket only after Phase 0 workflow approval.

## Supersedes / Superseded By

None.
