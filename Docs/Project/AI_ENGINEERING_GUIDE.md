# Project Minerva — AI Engineering Guide

**Version:** 1.2
**Status:** Draft
**Owner:** Technical Director
**Last Updated:** 2026-07-24
**Related Documents:** [`README.md`](README.md), [`ARCHITECTURAL_PRINCIPLES.md`](ARCHITECTURAL_PRINCIPLES.md), [`ENGINEERING_PHILOSOPHY.md`](ENGINEERING_PHILOSOPHY.md), [`GLOSSARY.md`](GLOSSARY.md), [`../Handbook/Ticket_Workflow.md`](../Handbook/Ticket_Workflow.md), [`../Templates/Ticket_Template.md`](../Templates/Ticket_Template.md), [`../Process/UNITY_EXECUTION_MODEL.md`](../Process/UNITY_EXECUTION_MODEL.md), [`../Runbooks/UNITY_5_6_VALIDATION.md`](../Runbooks/UNITY_5_6_VALIDATION.md)

## Purpose

This document defines how Codex and other AI agents work within Project Minerva.

AI-assisted development is a controlled implementation and review workflow, not a substitute for architecture, product decisions, technical direction, or repository ownership. Agents execute approved work using curated context and explicit authority.

The objective is to keep each task focused, reproducible, reviewable, and small enough to avoid unnecessary context expansion or architectural drift.

## Role Boundaries

### Product Owner and Creative Director

Defines product priorities, player experience, narrative direction, and approval of major scope decisions.

### Repository Owner

Retains manual pull-request merge authority unless that authority is explicitly delegated through a future documented decision.

### Technical Director

Owns architecture, governing documentation, system design, roadmap structure, ticket definition, implementation guardrails, agent delegation, final acceptance decisions, and readiness-for-merge decisions.

The Technical Director may delegate implementation validation. Delegated checks must be attributed to the agent that performed them; the Technical Director must not represent delegated work as personally executed.

### AI Implementation Engineer

Implements the exact work authorized by an approved ticket, reports results, and stops when scope is complete or blocked.

The AI Implementation Engineer does not independently redefine architecture, expand product scope, accept its own work, move its own ticket to `Complete`, or merge the pull request.

### Implementation Review Agent

Independently reviews the current pull-request head against the approved ticket and required context. The Implementation Review Agent:

- inspects the changed files and implementation delta;
- verifies authorized paths and prohibited actions;
- evaluates acceptance criteria and validation evidence;
- identifies blocking findings, risks, and deviations;
- recommends `Accept`, `Changes Required`, or `Blocked`.

The Implementation Review Agent does not redesign requirements, implement fixes, make the final acceptance decision, move the ticket to `Complete`, or merge the pull request.

### Repository Maintenance Operator

Performs a narrowly authorized repository mutation when the normal execution path is unavailable or inappropriate. This is an operational role, not an implementation-review role.

A maintenance operator may move or update an accepted ticket, correct metadata, or perform another explicitly bounded repository action. The operator must not reinterpret the acceptance decision or expand the authorized change.

### Local Unity Verification Operator

The Local Unity Verification Operator validates an exact pull-request head with the locally installed Unity editor. The operator runs the approved preflight and verification scripts, preserves evidence, and reports failures without silently changing implementation.

The operator does not redesign the solution, accept the implementation, move a ticket to `Complete`, or merge a pull request. See the [Unity Execution Model](../Process/UNITY_EXECUTION_MODEL.md).

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
- implementation-report format;
- review and acceptance records;
- explicit lifecycle movement instructions.

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

All generated C# and editor code must be compatible with Unity 5.6 and the project's configured scripting runtime.

Do not assume availability of:

- modern Unity packages or package-manager workflows;
- APIs introduced after Unity 5.6;
- modern UI Toolkit APIs;
- newer serialization behavior;
- newer C# syntax or runtime libraries;
- current assembly-definition behavior without verification;
- modern platform lifecycle helpers.

When uncertain, verify against project-local compatibility documentation or report the uncertainty. Do not silently use a newer API because it is common in contemporary examples.

### Unity Execution Capability

Repository editing, C# compilation, and Unity execution are separate capabilities. Before beginning a Unity-related ticket, the implementation handoff must declare:

```text
Implementation environment:
Unity execution environment:
Unity version:
Unity executable:
Validation responsibility:
```

The implementation agent reports whether repository editing, a C# compiler, Unity 5.6.7f1, and Unity tests are available in its environment. When Unity is unavailable, record Unity validation as pending and route it to the Local Unity Verification Operator. Do not describe unavailable execution as a compilation or test failure.

Local validation follows the [Unity 5.6 Validation Runbook](../Runbooks/UNITY_5_6_VALIDATION.md). A Unity exit code alone is not test evidence; the required results XML is authoritative.

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

## Independent Review and Acceptance

The implementation workflow uses separate implementation, review, acceptance, and merge responsibilities:

1. The AI Implementation Engineer completes the ticket and opens the pull request with the ticket in `Review`.
2. The Technical Director dispatches an Implementation Review Agent against the current pull-request head.
3. The Implementation Review Agent returns findings and an acceptance recommendation.
4. The Technical Director evaluates that recommendation and makes the final acceptance decision.
5. When accepted, the Technical Director moves the ticket to `Complete` in the same implementation pull request.
6. The repository owner performs the manual merge.

If commits are added after review, the Technical Director reviews the delta. Material implementation changes require a full or delta Implementation Review Agent pass. Administrative changes may rely on the existing review when their delta is independently verified and cannot affect the reviewed implementation.

## Repository Execution Paths

An unavailable integration path does not automatically block an authorized task when another approved path exists.

### Path A — GitHub-Native Execution

`Agent → GitHub API → Commit → Pull Request`

Use when the acting agent has verified GitHub write capability for the required mutation.

### Path B — Local Workspace Execution

`Agent → Local Filesystem → Local Git Commit → Push`

Use when GitHub-native mutation is unavailable but an authorized local agent, such as Codex, can access the checked-out repository.

### Path C — Human Execution

`Repository Owner → Local Filesystem → Local Git Commit → Push`

Use when agent execution is unavailable, unsafe, or unnecessary.

Before selecting a path, verify the actor has the required read, write, commit, and push capabilities. Do not dispatch another agent merely to create the appearance of progress when it has the same unavailable capability.

The chosen path does not change ticket scope, acceptance authority, or merge authority.

## Execution State Integrity

Agents must distinguish intended actions from verified repository mutations.

Use these states:

- `Planned` — authority and intended mutation are defined; execution has not begun.
- `In Progress` — the authorized actor is performing the mutation.
- `Committed` — a commit containing the intended mutation exists.
- `Verified` — the resulting commit, branch, diff, or pull-request state has been inspected and matches the intended action.

Each state record must include:

- state;
- timestamp in `America/New_York` using `YYYY-MM-DD HH:mm z`;
- actor;
- concise evidence or notes.

Date-only ticket metadata uses `YYYY-MM-DD` in `America/New_York`.

Do not say that a file was changed, a commit was created, a ticket moved, or a pull request updated until that state is supported by repository evidence. When a mutation fails or has not occurred, say so directly.

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
- the ticket's acceptance criteria cannot be met within its stated scope.

Stopping under these conditions is correct engineering behavior, not failure.

An unavailable GitHub mutation path is not itself a stop condition when an authorized Path B or Path C execution route is available. Report the failed capability accurately and route the task through the approved fallback.

## Prohibited AI Behaviors

An AI agent must not:

- claim to have read files it did not access;
- claim to have run tests it did not run;
- claim repository mutations occurred without commit or state evidence;
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

Follow the ticket's Required Context, authority boundaries, acceptance criteria,
and implementation-report format. Do not read or modify unrelated areas.
Stop and report if a defined stop condition occurs.
```

For a narrowly authorized maintenance fallback, use:

```text
Perform the approved repository maintenance task against <exact branch and paths>.
Do not implement code, reinterpret acceptance, expand scope, or merge the PR.
Show the diff and validation evidence before commit or push when requested.
```

Do not paste the entire project history into the prompt when the repository ticket and curated context are available.

## Completion Standard

The AI engineering workflow is successful when:

- the task remains within approved scope;
- the context window contains only relevant authoritative material;
- the diff is focused and reviewable;
- architectural ownership remains intact;
- implementation, review, acceptance, and merge roles remain distinct;
- required validation is honestly reported;
- repository execution states are supported by evidence;
- an unavailable integration is routed through an authorized fallback when possible;
- the repository contains enough updated knowledge for the next contributor;
- unresolved issues are surfaced rather than hidden.

The purpose of these guardrails is not to limit useful engineering judgment. It is to ensure that judgment operates within explicit project decisions and produces durable, trustworthy work.
