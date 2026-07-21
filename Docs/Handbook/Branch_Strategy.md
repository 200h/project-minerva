# Branch Strategy

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`Git_Workflow.md`](Git_Workflow.md), [`Ticket_Workflow.md`](Ticket_Workflow.md)

## Purpose

This document defines branch naming, ownership, lifespan, and merge expectations.

## Default Model

Project Minerva uses short-lived branches created from `main` and merged back through pull requests.

## Branch Categories

- `docs/<description>` — documentation and governance.
- `arch/<description>` — architecture specifications and ADR-driven structural work.
- `feature/<description>` — runtime or editor features.
- `tool/<description>` — developer and authoring tools.
- `fix/<description>` — defect corrections.
- `chore/<description>` — repository maintenance with no product behavior change.
- `experiment/<description>` — explicitly time-boxed research that is not production-approved.

AI-generated implementation branches may use `agent/<description>` when created through an automated publishing workflow.

## Naming Rules

Names must be lowercase, concise, hyphen-separated, and describe the work rather than the contributor.

Good examples:

- `docs/phase-0-workflow-foundation`
- `arch/save-system-boundaries`
- `tool/story-timeline-filtering`
- `fix/delayed-message-resume`

## Lifecycle

A branch should:

1. start from current `main`;
2. serve one approved scope;
3. remain open only while that scope is active;
4. receive review fixes related to that scope;
5. be deleted after merge.

Do not reuse merged branches. Do not stack unrelated work onto an open branch merely to avoid opening another PR.

## Long-Lived Branches

Long-lived development branches are discouraged. They require explicit Technical Director approval, a documented purpose, synchronization rules, and an exit plan.

## Experiments

Experiment branches must state:

- the question being tested;
- time or effort limit;
- success and failure criteria;
- whether results may be merged;
- cleanup expectations.

Experimental code must not become production architecture by accident.

## Protection Expectations

`main` should require pull-request review once repository settings and team size make enforcement practical. Required checks should be added incrementally as the project gains build and test automation.
