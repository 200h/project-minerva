# Project Minerva Planning

**Version:** 1.1  
**Status:** Draft  
**Owner:** Product Owner and Technical Director  
**Last Updated:** 2026-07-23  
**Related Documents:** [`../Docs/Project/DEVELOPMENT_ROADMAP.md`](../Docs/Project/DEVELOPMENT_ROADMAP.md), [`../Docs/Handbook/Ticket_Workflow.md`](../Docs/Handbook/Ticket_Workflow.md)

## Purpose

`Planning/` contains active project-management information that changes more frequently than constitutional, architecture, or system documentation.

## Planning Hierarchy

Project planning uses four distinct levels:

- **Phase** — a broad stage of project maturity.
- **Epic** — a bounded capability delivered through multiple related tickets.
- **Milestone** — a demonstrable project checkpoint that may draw outcomes from one or more epics.
- **Ticket** — the executable unit of work.

The normal relationship is:

```text
Phase
  └── Epic
        └── Ticket
```

Milestones remain cross-cutting checkpoints and are not replaced by epics.

Being listed in an epic does not authorize implementation. Work becomes executable only through a ticket in `Docs/Tickets/Ready/`.

## Files

- [`Backlog.md`](Backlog.md) — high-level candidate work not yet expressed as ready tickets.
- [`Milestones.md`](Milestones.md) — phase and milestone outcomes.
- [`Current_Work.md`](Current_Work.md) — the small set of work currently approved or active.
- [`Epics/`](Epics/) — bounded multi-ticket capabilities, sequencing, dependency graphs, progress summaries, and synchronized ticket registers.
- [`Risks.md`](Risks.md) — threats requiring monitoring or mitigation.
- [`Technical_Debt.md`](Technical_Debt.md) — known engineering compromises and repayment plans.
- [`Known_Limitations.md`](Known_Limitations.md) — accepted current capability limits.

## Epic Rules

Each epic should define:

- objective and architectural boundary;
- included-ticket register;
- ticket status, created date, last-updated date, and dependencies;
- dependency graph and sequence;
- completion criteria;
- deferred capabilities;
- unresolved design questions that may affect later tickets.

Ticket directories and ticket metadata remain authoritative. Epic tables are synchronized summaries; stale rows are planning defects.

Only the next executable ticket should normally be in `Ready`. Later tickets remain in `Backlog` until their dependencies, boundaries, and acceptance criteria pass readiness review.

## Boundary With Tickets

Planning files describe priorities and project state. They do not authorize implementation. Work becomes executable only through a Ready ticket under `Docs/Tickets/`.

## Maintenance

Update planning records when priorities, milestones, epic membership, ticket states, risks, debt, or limitations materially change. Preserve concise entries and link to tickets, ADRs, or documents instead of duplicating implementation detail.
