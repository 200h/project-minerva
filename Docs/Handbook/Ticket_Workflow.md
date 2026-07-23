# Ticket Workflow

**Version:** 1.2  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-23  
**Related Documents:** [`../Project/AI_ENGINEERING_GUIDE.md`](../Project/AI_ENGINEERING_GUIDE.md), [`../Templates/Ticket_Template.md`](../Templates/Ticket_Template.md)

## Purpose

Tickets are the authoritative work packages for implementation. They translate approved direction into bounded, reviewable tasks.

## States

`Backlog → Ready → Active → Review → Complete`

### Backlog

Candidate work that is not approved for implementation. It may be incomplete, unprioritized, or dependent on unresolved decisions.

### Ready

Approved, sufficiently specified, and unblocked. Required context, authority boundaries, acceptance criteria, and validation expectations must be present.

### Active

Implementation has begun. One owner is responsible. Scope changes require ticket revision or a new ticket.

### Review

Implementation is complete enough for independent review and includes the required implementation report and validation evidence. The implementation pull request is open, the Implementation Review Agent review is pending or underway, and the ticket remains unaccepted.

### Complete

The Implementation Review Agent has completed validation, the Technical Director has accepted the work, documentation is current, and follow-up items are captured separately. A Complete ticket is approved for merge; GitHub remains authoritative for whether the pull request has actually merged.

## Workflow Roles

### Technical Director

The Technical Director owns architecture, ticket definition, acceptance criteria, delegation, final acceptance decisions, and readiness-for-merge decisions. The Technical Director may delegate implementation validation but does not represent delegated checks as personally executed.

### Implementation Engineer

The Implementation Engineer executes the approved ticket, reports evidence, and moves the ticket through implementation-owned states. The implementation engineer does not accept its own work or move its own ticket to `Complete`.

### Implementation Review Agent

The Implementation Review Agent independently compares the pull request against the ticket, validates authorized scope and acceptance criteria, reviews test and validation evidence, identifies blocking findings, and recommends acceptance or changes.

The Implementation Review Agent does not redesign the ticket, implement fixes, make the final acceptance decision, or merge the pull request.

### Repository Owner

The repository owner retains manual pull-request merge authority unless that authority is explicitly delegated in a future documented decision.

## Ticket Identification

Use a domain prefix and sequential number, for example:

- `DOC-001`
- `ARCH-001`
- `FND-001`
- `FRAME-001`
- `TOOL-001`
- `PHONE-001`
- `SAVE-001`
- `NARR-001`
- `BUG-001`

File names should begin with the identifier and use a concise slug.

## Readiness Checklist

A ticket may enter Ready only when it defines:

- goal and background;
- required and optional context;
- requirements and exclusions;
- dependencies;
- authorized and prohibited paths;
- expected deliverables;
- acceptance criteria;
- validation requirements;
- implementation-report format;
- explicit lifecycle movement instructions.

## Directory Authority

The containing directory is the authoritative workflow state. The ticket's `Status` field must match that directory exactly:

- `Docs/Tickets/Backlog/` → `Backlog`
- `Docs/Tickets/Ready/` → `Ready`
- `Docs/Tickets/Active/` → `Active`
- `Docs/Tickets/Review/` → `Review`
- `Docs/Tickets/Complete/` → `Complete`

A mismatch between directory and status is a workflow defect and must be corrected before further ticket execution.

## Movement Rules

Move the Markdown file between state directories. Do not duplicate it. Retain the same identifier and filename, update the `Status`, `Owner`, and `Updated` metadata, and preserve its history in Git.

Workflow dates and execution-state timestamps use `America/New_York`. Date-only metadata uses `YYYY-MM-DD`. Execution-state records use `YYYY-MM-DD HH:mm z`, including the applicable `EST` or `EDT` abbreviation.

### Backlog to Ready

The Technical Director moves the ticket after readiness review and approval.

### Ready to Active

The implementation owner moves the ticket when implementation begins, records ownership, and updates `Status` to `Active`.

### Active to Review

The implementation owner moves the ticket when the implementation pull request is opened, updates `Status` to `Review`, and completes the Implementation Report with available validation evidence.

### Review Validation

The Technical Director dispatches an Implementation Review Agent. The agent reviews the current pull-request head and returns findings plus an acceptance recommendation.

When the pull-request head changes after review, the Technical Director determines whether the prior review remains valid. Material implementation changes require a full or delta review. Administrative changes that do not affect the reviewed implementation may be accepted without repeating the full review when their delta is verified.

### Review to Complete

After the Implementation Review Agent recommends acceptance, the Technical Director evaluates the findings and makes the final acceptance decision. When accepted, the Technical Director moves the ticket to `Complete` before merge and records:

- pull-request reference and reviewed head when available;
- Implementation Review Agent recommendation;
- final validation decision;
- accepted deviations;
- acceptance date;
- follow-up ticket identifiers.

Implementation owners must not move their own tickets to `Complete`. The Implementation Review Agent recommends; the Technical Director accepts; the repository owner merges.

## Pull Request Requirements

A ticket-driven implementation pull request must include the ticket in `Review/` while independent review is pending.

After successful review and Technical Director acceptance, the same implementation pull request must include the ticket in `Complete/` before merge. A separate post-merge closeout pull request is not required.

GitHub pull-request state is authoritative for whether accepted work has merged. Ticket directories do not duplicate GitHub's merge-state tracking.

## Scope Changes

Small clarifications may be edited into an Active ticket with an explicit note. Material scope expansion should return the ticket to Ready or create a separate ticket.

## Blocked Work

A blocked ticket remains Active only while the blocker is being resolved immediately. Otherwise return it to Ready or Backlog with the blocker documented.

## AI Execution Prompt

Use the concise prompt pattern defined in the AI Engineering Guide and point Codex to the exact ticket path. The ticket, not the chat prompt, carries the detailed authority.

The prompt must remind Codex to:

- move `Ready` to `Active` when work begins;
- move `Active` to `Review` when the pull request opens;
- never move its own ticket to `Complete`;
- stop rather than alter unauthorized planning or governance files.

## Execution State Integrity

Repository work must distinguish intent from verified execution. Use these states when reporting repository mutations:

- `Planned` — the action and authority are defined, but no mutation has begun;
- `In Progress` — an authorized actor is actively performing the mutation;
- `Committed` — a commit containing the intended mutation exists;
- `Verified` — the resulting branch, commit, diff, or pull-request state has been inspected and matches the intended action.

Each state record must include:

- local timestamp in `America/New_York`;
- actor;
- state;
- concise evidence or notes.

Do not report a repository change as committed or complete until the corresponding commit or repository state can be verified.

## Completion

A ticket is Complete after independent validation and Technical Director acceptance. Before merge, confirm:

- the ticket exists only in `Docs/Tickets/Complete/`;
- the `Status` metadata is `Complete`;
- the acceptance record identifies the review recommendation and Technical Director decision;
- the PR reference and accepted validation evidence are recorded;
- accepted deviations and follow-up tickets are recorded when applicable.

Completed acceptance criteria and Definition of Done checkboxes should be checked only when supported by accepted evidence.
