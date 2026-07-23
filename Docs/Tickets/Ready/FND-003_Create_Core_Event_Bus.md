# FND-003: Create Core Event Bus

**Status:** Ready  
**Owner:** Unassigned  
**Created:** 2026-07-23  
**Updated:** 2026-07-23  
**Roadmap Phase:** Phase 1 — Foundation  
**Epic:** [Foundation Runtime Services](../../../Planning/Epics/Foundation_Runtime_Services.md)  
**Related ADRs:** [`../../ADR/0001_Use_Unity_5_6.md`](../../ADR/0001_Use_Unity_5_6.md), [`../../ADR/0004_Ticket_Driven_AI_Implementation.md`](../../ADR/0004_Ticket_Driven_AI_Implementation.md)

## Summary

Create the smallest generic, strongly typed event-bus foundation needed by later Project Minerva runtime systems.

The implementation provides reusable base contracts and deterministic dispatch behavior only. It must not define narrative, phone, application, simulation, save, notification, presentation, or platform-specific event classes.

## Background

FND-002 established explicit runtime service composition and deterministic lifecycle management. Later services need a shared mechanism for announcing completed facts without direct system coupling, global singleton access, reflection-based discovery, or scene dependencies.

The Event Architecture requires immutable event messages, explicit disposable subscriptions, deterministic ordering, queued nested publication, and subscriber-failure isolation. This ticket implements only the generic foundation that later domain-specific event types and services build upon.

## Goal

Provide a Unity 5.6-compatible plain-C# event system that:

- exposes small generic publishing and subscription contracts;
- uses strongly typed event messages;
- preserves deterministic subscriber invocation order;
- makes subscriptions explicit and disposable;
- safely handles subscription changes during dispatch;
- queues nested publication and drains it predictably;
- isolates subscriber exceptions and reports useful diagnostics;
- supports cleanup and repeated construction without leaked static mutable state;
- contains no domain-specific event definitions.

## Required Context

Read only:

- `Docs/Project/README.md`
- `Docs/Project/CODING_STANDARDS.md`
- `Docs/Project/AI_ENGINEERING_GUIDE.md`
- `Docs/Architecture/SYSTEM_OVERVIEW.md`
- `Docs/Architecture/RUNTIME_LAYERS.md`
- `Docs/Architecture/DEPENDENCY_RULES.md`
- `Docs/Architecture/INITIALIZATION_AND_LIFECYCLE.md`
- `Docs/Architecture/EVENT_ARCHITECTURE.md`
- `Docs/Architecture/EDITOR_RUNTIME_BOUNDARIES.md`
- `Docs/Context/Foundation.md`
- `Docs/Handbook/Definition_of_Done.md`
- `Docs/Handbook/Ticket_Workflow.md`

## Optional Context

Read only when required by a specific implementation question:

- `Docs/Architecture/STATE_OWNERSHIP.md`
- `Docs/Handbook/Git_Workflow.md`
- `Docs/Handbook/Branch_Strategy.md`
- `Docs/Tickets/Complete/FND-002_Create_Runtime_Bootstrap.md`
- `Planning/Epics/Foundation_Runtime_Services.md`

## Code Areas to Inspect

- `Assets/Minerva/Runtime/Core/`
- `Assets/Minerva/Tests/Runtime/`
- this ticket file for lifecycle movement and implementation reporting

Do not perform repository-wide discovery beyond Required Context and these areas.

## Requirements

- Create a marker contract or equivalent generic constraint for event messages without imposing domain behavior.
- Create a small public event-bus contract supporting strongly typed subscription and publication.
- Return an explicit disposable subscription handle or equivalent token from subscription calls.
- Preserve subscription registration order for handlers of the same exact event type.
- Reject null handlers and null event messages clearly where applicable.
- Make subscription disposal idempotent.
- A handler unsubscribed before its turn during dispatch must not run later in that dispatch.
- A handler subscribed during dispatch must not receive the current event.
- Queue nested publications and drain them FIFO after the current event finishes.
- Avoid recursive nested dispatch and unbounded call-stack growth.
- Catch subscriber exceptions individually, continue remaining eligible subscribers, and return useful failure diagnostics.
- Preserve event type and subscriber identity sufficient for focused diagnostics without creating a logging framework.
- Publishing with no subscribers must succeed with a clear empty result.
- Cleanup or bus disposal must be idempotent and remove subscriptions.
- Publication and new subscriptions after final disposal must be rejected clearly.
- Avoid static mutable registries, global accessors, service locators, reflection discovery, and scene searches.
- Keep public contracts small, documented, and reusable by later domain event classes.
- Use the `Minerva` namespace root and Unity 5.6-compatible language/runtime features.
- Add focused tests for dispatch, mutation, nested publication, failure isolation, and disposal.

## Event Contract Boundaries

The foundation may define:

- an event marker interface or minimal base type;
- publisher/subscriber or combined event-bus interfaces;
- disposable subscription handles;
- publication result and subscriber-failure diagnostic types;
- a deterministic in-memory event-bus implementation.

The foundation must not require timestamps, correlation IDs, publisher IDs, simulation metadata, or other speculative fields on every event. Later layers may define envelopes when concrete requirements are approved.

Dispatch is exact-type only. Base-type, interface-wide, wildcard, or hierarchy traversal is outside this ticket.

## Out of Scope

- domain-specific narrative, phone, app, screen, message, contact, notification, time, save, presentation, or platform events;
- clock, scheduler, delayed-event, persistence, replay, or event-sourcing behavior;
- command, request/response, query, mediator, or RPC frameworks;
- async, multithreaded, coroutine, task-based, or network dispatch;
- priority or wildcard subscriptions;
- inheritance-polymorphic dispatch;
- attribute or reflection-based registration;
- dependency-injection framework behavior;
- static/global event-bus access;
- scenes, prefabs, MonoBehaviours, ScriptableObjects, or editor windows;
- logging, analytics, telemetry, third-party packages, or assembly definitions.

## Dependencies and Prerequisites

- FND-001 is Complete.
- FND-002 is Complete and merged.
- PR #8 workflow documentation is merged.
- The Unity 5.6 project skeleton opens successfully.
- No unresolved ADR is required for this narrow generic implementation.

## Authorized Files or Directories

- `Assets/Minerva/Runtime/Core/**`
- `Assets/Minerva/Tests/Runtime/**`
- this ticket file for status movement and implementation reporting

## Prohibited Files or Systems

- `Assets/Minerva/Runtime/Application/**`
- `Assets/Minerva/Runtime/Simulation/**`
- `Assets/Minerva/Runtime/Phone/**`
- `Assets/Minerva/Runtime/Presentation/**`
- `Assets/Minerva/Runtime/Platform/**`
- `Assets/Minerva/Editor/**`
- `Assets/Minerva/Tests/Editor/**`
- `Assets/Minerva/Content/**`
- `Assets/Minerva/Resources/**`
- `ProjectSettings/**`
- `Packages/**`
- `Docs/Project/**`
- `Docs/Architecture/**`
- `Docs/Systems/**`
- `Docs/Tools/**`
- `Docs/ADR/**`
- `Planning/**`
- third-party or vendor directories

## Permitted Actions

- Add generic event contracts, diagnostics, subscription handling, and an in-memory implementation under the authorized Core path.
- Add focused runtime EditMode tests.
- Reuse FND-002 lifecycle contracts when needed without redesigning them.
- Remove obsolete `.gitkeep` files only from authorized directories receiving implementation files.
- Move this ticket from `Ready` to `Active` when implementation begins.
- Move it from `Active` to `Review` when the implementation pull request opens.
- Complete the Implementation Report and required validation.

## Prohibited Actions

- Do not implement domain-specific event classes or speculative metadata.
- Do not implement inheritance-polymorphic, wildcard, priority, asynchronous, or reflection-based dispatch.
- Do not expose a static or globally discoverable event bus.
- Do not create a mediator, command bus, query bus, service locator, or dependency-injection container.
- Do not use scene search or hidden singleton creation.
- Do not add third-party packages, scenes, prefabs, ScriptableObjects, editor assets, or assembly definitions.
- Do not change architecture, planning, system, ADR, tool, or governance documents.
- Do not move the ticket to `Complete`; independent review and Technical Director acceptance are required.
- Do not merge the implementation pull request.

## Expected Files

Exact names may vary within the authorized boundary:

- event marker or generic constraint contract;
- publisher/subscriber or combined bus contract;
- disposable subscription implementation;
- publication result and subscriber-failure diagnostics;
- deterministic in-memory event bus;
- focused EditMode tests.

## Architect's Notes

Use small interfaces and sealed plain-C# implementation types rather than speculative inheritance hierarchies.

Subscription mutations must have defined semantics rather than depending on collection-enumerator behavior. Nested publications must be queued: all eligible handlers for event A finish before queued event B begins, and multiple nested publications retain FIFO order.

Subscriber failures are diagnostics, not permission to abort unrelated subscribers. Events are immutable by convention and API design; do not add a cloning or generalized immutability framework.

The bus may implement `IRuntimeService` when that produces a clean explicit lifecycle, but a smaller disposable design is acceptable when it satisfies all requirements.

## Acceptance Criteria

- [ ] Generic event contracts are reusable by later domain-specific classes without modification.
- [ ] Subscriptions and publications are strongly typed and exact-type only.
- [ ] Registration order determines invocation order for the same event type.
- [ ] Subscription handles are explicit, disposable, and idempotent.
- [ ] Unsubscription and subscription during dispatch follow the required semantics.
- [ ] Nested publications are queued FIFO without direct recursive dispatch.
- [ ] Subscriber exceptions do not prevent remaining eligible delivery.
- [ ] Diagnostics identify the event type and failed subscriber sufficiently for testing and debugging.
- [ ] Publishing with no subscribers returns a clear empty success result.
- [ ] Cleanup/disposal is idempotent; post-disposal subscription and publication are rejected.
- [ ] Separate bus instances can be repeatedly constructed and disposed without static-state leakage.
- [ ] No `UnityEditor`, scene search, singleton, global accessor, service locator, reflection discovery, or mutable static subscriber registry is introduced.
- [ ] No domain-specific or future-system behavior is added.
- [ ] Focused tests cover all required behavior.
- [ ] The final diff contains only authorized paths.
- [ ] The Implementation Report records changes and validation accurately.

## Required Validation

- List every created, modified, moved, and deleted path.
- Confirm no path outside Authorized Files or Directories changed.
- Run focused runtime EditMode tests available in the environment.
- Run Unity 5.6 batch import/compile validation when available.
- Search runtime files for `UnityEditor`; no matches may exist.
- Search for scene search, static/global bus access, service locators, reflection discovery, wildcard dispatch, and mutable static subscriber collections; no prohibited implementation may exist.
- Confirm no domain-specific events, `.unity`, `.prefab`, `.asset`, `.asmdef`, package, or vendor files were added.
- Confirm all new text files end with a newline.
- Run `git diff --check`.
- Confirm the ticket exists in exactly one workflow directory and its `Status` matches.

## Definition of Done

- [ ] Acceptance criteria satisfied.
- [ ] Required validation reported accurately.
- [ ] Documentation updated.
- [ ] No unauthorized changes.
- [ ] Implementation Report completed.
- [ ] Independent Review record completed by the IRA.
- [ ] Technical Director Acceptance completed before merge.

## Implementation Report

### Status

### Changed Files

### Work Completed

### Validation

### Deviations

### Blockers or Risks

### Optional Context Used

### Follow-Up Suggestions

## Implementation Review Agent Record

Completed by the independent reviewer while the ticket is in `Review`.

### Reviewer

### Reviewed PR and Head

### Scope and Acceptance Findings

### Validation Assessment

### Blocking Findings

### Recommendation

`Accept`, `Changes Required`, or `Blocked`.

## Technical Director Acceptance

Completed after reviewing the IRA recommendation and before merge.

### Decision

`Accepted`, `Changes Required`, or `Blocked`.

### PR Reference

### Acceptance Date

Use `YYYY-MM-DD` in `America/New_York`.

### Final Validation Decision

### Accepted Deviations

### Follow-Up Tickets

## Execution State Log

Use `YYYY-MM-DD HH:mm z` in `America/New_York`.

| State | Timestamp | Actor | Evidence or Notes |
|---|---|---|---|
| Planned |  |  |  |
| In Progress |  |  |  |
| Committed |  |  |  |
| Verified |  |  |  |
