# Project Minerva — AI Engineering Guide

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`README.md`](README.md), [`ARCHITECTURAL_PRINCIPLES.md`](ARCHITECTURAL_PRINCIPLES.md), [`ENGINEERING_PHILOSOPHY.md`](ENGINEERING_PHILOSOPHY.md), [`GLOSSARY.md`](GLOSSARY.md), `Docs/Handbook/Ticket_Workflow.md`, `Docs/Templates/Ticket_Template.md`

## Purpose

This document defines how Codex and other AI implementation agents must work within Project Minerva.

AI-assisted development is a controlled implementation workflow, not a substitute for architecture, product decisions, or technical direction. AI agents execute approved tickets using curated context and explicit authority.

The objective is to keep each task focused, reproducible, reviewable, and small enough to avoid unnecessary context expansion or architectural drift.

## Role Boundaries

### Product Owner and Creative Director

Defines product priorities, player experience, narrative direction, and approval of major scope decisions.

### Technical Director

Owns architecture, governing documentation, system design, roadmap structure, ticket definition, implementation guardrails, and code review.

### AI Implementation Engineer

Implements the exact work authorized by an approved ticket, reports results, and stops when scope is complete or blocked.

The AI Implementation Engineer does not independently redefine architecture, expand product scope, or create future work without authorization.

## Governing Rule

> Implement the ticket, not the surrounding opportunity.

Related improvements, cleanup ideas, and future extensions may be reported. They must not be implemented unless the ticket explicitly authorizes them.

## Required Ticket Inputs

An implementation ticket should provide enough information to perform the work without repository-wide discovery.

Each ticket must define, as applicable:

- ticket identifier and title;
- summary and background;
- goal;
- required context;
- optional context;
- code areas to inspect;
- requirements;
- out-of-scope work;
- dependencies and prerequisites;
- authorized files or directories;
- prohibited files or systems;
- permitted actions;
- prohibited actions;
- expected files;
- documentation updates;
- architectural notes;
- acceptance criteria;
- definition of done;
- required validation;
- implementation-report format.

A ticket missing material implementation context should be returned for refinement rather than silently broadened through guesswork.

## Context Loading Rules

### 1. Read the Ticket First

The ticket is the primary task authority. Read it before searching the repository or opening project documentation.

### 2. Read Required Context Only

Read every document and code area listed under Required Context. Do not substitute similarly named files or assume broader context is necessary.

### 3. Use Optional Context Conditionally

Optional Context may be read only when a specific implementation question requires it. The implementation report should identify optional material that was used when it influenced the result.

### 4. Do Not Load the Entire Repository

Do not recursively inspect all project files, all documentation, all tickets, or the complete history by default.

Repository-wide searches may be used only when the ticket explicitly permits them or when a narrowly scoped symbol or reference search is necessary to locate a required dependency.

### 5. Stop Context Expansion When the Task Is Understood

Once the ticket, required documentation, and relevant code establish the contracts needed for implementation, stop gathering context and begin the authorized work.

More context is not automatically better. Excess context increases distraction, cost, and the risk of unrelated changes.

### 6. Report Missing Context

If required information is absent, contradictory, inaccessible, or too broad to resolve safely, stop and report:

- what is missing;
- why it blocks or materially risks the task;
- the smallest additional context or decision needed.

Do not compensate by inventing architecture.

## Context Indexes

Project Minerva may maintain curated Markdown context indexes under `Docs/Context/`.

A context index identifies a maintained reading set for a domain such as Foundation, Tooling, Phone, Story, Save, or Time. It is not a blanket instruction to read every linked file.

Tickets remain authoritative and should name:

- the required context index sections;
- any additional exact documents;
- the exact code areas to inspect;
- any exclusions.

Project Minerva does not rely on custom `.context` files as an architectural requirement.

## Scope and Authority Rules

### Authorized Files

Create or modify only the paths explicitly authorized by the ticket.

If an authorized file requires a necessary change to an unauthorized interface or dependency, stop and report the conflict. Do not silently expand the modification boundary.

### Expected Files

Expected Files describe the anticipated implementation shape. They do not grant authority outside Authorized Files.

If a better file split is necessary within the authorized boundary, follow ticket guidance and explain the change in the implementation report.

### Prohibited Areas

Do not inspect or modify prohibited files, systems, generated data, vendor assets, or configuration unless the ticket is amended.

### No Opportunistic Refactoring

Do not rename, reformat, reorganize, optimize, or refactor unrelated code.

A small local refactor may be performed only when required to complete the ticket safely and when it remains within the authorized files. Report it explicitly.

### No Anticipatory Features

Do not implement hooks, settings, abstractions, event types, managers, editor controls, or data fields solely because they might be useful in a future ticket.

Implement the smallest complete solution that satisfies the current approved design.

## Architectural Guardrails

Unless a ticket explicitly authorizes an architectural change, an AI agent must not:

- create or remove architectural layers;
- redefine state ownership;
- introduce new global managers or singletons;
- change namespace roots;
- move repository directories;
- alter save contracts or serialization formats;
- replace stable identifiers;
- introduce new third-party packages;
- change Unity or platform targets;
- couple runtime code to UnityEditor APIs;
- make editor code ship in player builds;
- duplicate authoritative story data across systems;
- move narrative ownership away from the Story Timeline;
- turn the Game Director into an authoring source;
- bypass project abstractions to call third-party assets broadly;
- redesign public interfaces outside ticket scope.

When an apparent requirement conflicts with the Project Constitution, stop and report the conflict.

## Unity 5.6 Guardrails

All generated C# and editor code must be compatible with Unity 5.6 and the project’s configured scripting runtime.

Do not assume availability of:

- modern Unity packages or package-manager workflows;
- APIs introduced after Unity 5.6;
- modern UI Toolkit APIs;
- newer serialization behavior;
- newer C# syntax or runtime libraries;
- current assembly-definition behavior without verification;
- modern platform lifecycle helpers.

When uncertain, verify against project-local compatibility documentation or report the uncertainty. Do not silently use a newer API because it is common in contemporary examples.

## Dependency Guardrails

Do not add, upgrade, remove, or replace external dependencies unless the ticket explicitly authorizes it.

When working near a third-party asset:

- use existing Minerva abstractions;
- avoid spreading vendor-specific types;
- do not edit vendor source unless authorized;
- do not copy vendor code into project-owned directories;
- preserve license and attribution files;
- report compatibility concerns.

## Implementation Behavior

### Prefer Existing Contracts

Use approved interfaces, data models, naming, registration patterns, and error-handling conventions found in Required Context.

Do not create parallel abstractions because an existing contract is inconvenient.

### Keep Changes Focused

Minimize the diff while producing a complete result. Avoid formatting churn, import reordering, line-ending changes, and unrelated comments.

### Preserve Existing Behavior

Unless the ticket explicitly changes behavior, maintain compatibility with existing callers, data, tests, and editor workflows.

### Make Failures Explainable

Add validation, diagnostics, or error handling required by the ticket. Avoid silent fallback that conceals invalid story or simulation state.

### Avoid Hidden Global State

Do not add static mutable state, service locators, scene searches, or implicit singleton creation unless specifically required by the approved architecture.

### Respect Generated and Authored Content

Do not hand-edit generated outputs unless the ticket identifies them as authoritative. Do not overwrite authored narrative data as part of a code task.

## Repository Search Rules

Allowed narrow searches include:

- locating an exact class, interface, method, identifier, or event name named by the ticket;
- finding direct callers or implementers of an authorized contract;
- locating tests or documentation explicitly referenced by required code;
- confirming that an expected file or symbol does not already exist.

Broad exploratory searches require explicit ticket authority when they could expand task scope.

Search results do not grant modification permission.

## Handling Ambiguity

Use the following order:

1. Apply explicit ticket language.
2. Apply Required Context.
3. Apply the Project Constitution.
4. Apply established local patterns in authorized code.
5. Choose the smallest reversible implementation detail.
6. If the decision affects architecture, persistence, public contracts, or scope, stop and report.

Do not ask broad design questions when a reasonable local implementation choice is available within established constraints. Do not make major assumptions merely to avoid reporting a blocker.

## Handling Discovered Problems

When unrelated defects, debt, or improvement opportunities are found:

- do not fix them unless necessary and authorized;
- record the file and concise observation;
- explain whether they block the ticket;
- recommend a separate ticket when appropriate.

An implementation report may identify follow-up work, but it must not create commitments or change the roadmap.

## Validation Requirements

Run only the checks required by the ticket and the smallest relevant additional checks available within the environment.

Validation may include:

- compilation;
- focused unit or editor tests;
- play-mode checks;
- serialization round trips;
- validation fixtures;
- static searches;
- documented manual verification.

Do not claim checks were run when they were not. Distinguish between passed validation, unavailable validation, and recommended follow-up.

If a check fails because of pre-existing unrelated issues, report the exact failure and evidence without fixing unrelated scope.

## Implementation Report

Every completed or blocked ticket must return a concise implementation report with:

### Ticket

Identifier and title.

### Status

Completed, partially completed, or blocked.

### Changed Files

Exact files created, modified, moved, or deleted.

### Work Completed

Summary mapped to ticket requirements.

### Validation

Commands, tests, checks, or manual verification performed and their results.

### Deviations

Any approved or necessary difference from the expected implementation. State `None` when applicable.

### Blockers or Risks

Unresolved issues, unavailable dependencies, compatibility concerns, or assumptions. State `None` when applicable.

### Optional Context Used

List optional documents or code areas read beyond Required Context. State `None` when applicable.

### Follow-Up Suggestions

Concise observations suitable for future tickets. Do not implement them.

## Stop Conditions

Stop implementation and report before proceeding when:

- the ticket conflicts with the Project Constitution;
- required context is unavailable or contradictory;
- an unauthorized file must change;
- a public contract requires redesign;
- save or authored-data compatibility may break;
- a new dependency appears necessary;
- Unity 5.6 compatibility cannot be established;
- the requested task is already implemented differently than the ticket assumes;
- validation reveals corruption or a broader architectural defect;
- the ticket’s acceptance criteria cannot be met within its stated scope.

Stopping under these conditions is correct engineering behavior, not failure.

## Prohibited AI Behaviors

An AI implementation agent must not:

- claim to have read files it did not access;
- claim to have run tests it did not run;
- fabricate APIs, files, repository state, or tool results;
- conceal incomplete work behind confident language;
- silently weaken acceptance criteria;
- rewrite the ticket during implementation;
- make broad changes to appear productive;
- add placeholders presented as finished behavior;
- leave unexplained dead code or commented-out alternatives;
- create documentation that contradicts implementation;
- continue after a defined stop condition without authorization.

## Ticket Prompt Pattern

A Codex work request should be concise because the ticket contains the details:

```text
Implement <TICKET-ID> from <exact ticket path>.

Follow the ticket’s Required Context, authority boundaries, acceptance criteria,
and implementation-report format. Do not read or modify unrelated areas.
Stop and report if a defined stop condition occurs.
```

Do not paste the entire project history into the prompt when the repository ticket and curated context are available.

## Completion Standard

The AI implementation workflow is successful when:

- the task remains within approved scope;
- the context window contains only relevant authoritative material;
- the diff is focused and reviewable;
- architectural ownership remains intact;
- required validation is honestly reported;
- the repository contains enough updated knowledge for the next contributor;
- unresolved issues are surfaced rather than hidden.

The purpose of these guardrails is not to limit useful engineering judgment. It is to ensure that judgment operates within explicit project decisions and produces durable, trustworthy work.