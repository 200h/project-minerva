# Ticket Workflow

**Version:** 1.1  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
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

Implementation is complete enough for review and includes the required implementation report and validation evidence. The implementation pull request is open and the ticket remains unaccepted.

### Complete

The implementation change is merged, acceptance criteria and validation evidence are accepted, documentation is current, and follow-up items are captured separately.

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

### Backlog to Ready

The Technical Director moves the ticket after readiness review and approval.

### Ready to Active

The implementation owner moves the ticket when implementation begins, records ownership, and updates `Status` to `Active`.

### Active to Review

The implementation owner moves the ticket when the implementation pull request is opened, updates `Status` to `Review`, and completes the Implementation Report with available validation evidence.

### Review to Complete

The Technical Director moves the ticket only after the implementation pull request is merged and the work is accepted. The closeout records the PR reference, final validation decision, accepted deviations, completion date, and any follow-up ticket identifiers.

Implementation owners and AI agents must not move their own tickets to `Complete`.

## Pull Request Requirements

A ticket-driven implementation pull request must include the ticket in `Review/`. A pull request should not be approved while its ticket remains in `Ready/` or `Active/`.

A post-merge closeout change is required to move the ticket from `Review/` to `Complete/`. This closeout may be combined with the next ticket-preparation PR when scope remains documentation-only and reviewable.

## Scope Changes

Small clarifications may be edited into an Active ticket with an explicit note. Material scope expansion should return the ticket to Ready or create a separate ticket.

## Blocked Work

A blocked ticket remains Active only while the blocker is being resolved immediately. Otherwise return it to Ready or Backlog with the blocker documented.

## AI Execution Prompt

Use the concise prompt pattern defined in the AI Engineering Guide and point Codex to the exact ticket path. The ticket, not the chat prompt, carries the detailed authority.

The prompt must remind Codex to:

- move `Ready` to `Active` when work begins;
- move `Active` to `Review` when the pull request opens;
- never move the ticket to `Complete`;
- stop rather than alter unauthorized planning or governance files.

## Completion

After merge and acceptance, the Technical Director moves the ticket to Complete and records:

- merge or PR reference;
- final status and completion date;
- accepted validation evidence;
- accepted deviations;
- follow-up ticket identifiers.

Completed acceptance criteria and Definition of Done checkboxes should be checked only when supported by accepted evidence.
