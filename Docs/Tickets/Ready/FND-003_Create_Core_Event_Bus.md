# FND-003: Create Core Event Bus

**Status:** Ready  
**Owner:** Unassigned  
**Created:** 2026-07-23  
**Updated:** 2026-07-23  
**Roadmap Phase:** Phase 1 — Foundation  
**Related ADRs:** [`../../ADR/0001_Use_Unity_5_6.md`](../../ADR/0001_Use_Unity_5_6.md), [`../../ADR/0004_Ticket_Driven_AI_Implementation.md`](../../ADR/0004_Ticket_Driven_AI_Implementation.md)

## Summary

Create the smallest generic, strongly typed event-bus foundation needed by later Project Minerva runtime systems.

The implementation must provide reusable base contracts and deterministic dispatch behavior only. It must not define narrative, phone, application, simulation, save, notification, presentation, or platform-specific event classes.

## Background

FND-002 established explicit runtime service composition and deterministic lifecycle management. Later services need a shared mechanism for announcing completed facts without direct system coupling, global singleton access, reflection-based discovery, or scene dependencies.

The project Event Architecture requires immutable event messages, explicit disposable subscriptions, deterministic ordering, queued nested publication, and subscriber-failure isolation. This ticket implements only the generic foundation that later domain-specific event types and services will build upon.

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

## Code Areas to Inspect

- `Assets/Minerva/Runtime/Core/`
- `Assets/Minerva/Tests/Runtime/`
- this ticket file for lifecycle movement and implementation reporting

Do not perform repository-wide discovery beyond Required Context and these areas.

## Requirements

- Create a marker contract or equivalent generic constraint for event messages without imposing domain behavior.
- Create a small public event-bus contract that supports strongly typed subscription and publication.
- Return an explicit disposable subscription handle or equivalent token from subscription calls.
- Preserve subscription registration order when invoking handlers for the same event type.
- Reject null event handlers and null event messages clearly where the language/runtime permits null.
- Make disposing a subscription idempotent.
- Ensure a handler unsubscribed before its turn during an active dispatch is not invoked later in that same dispatch.
- Ensure a handler subscribed during an active dispatch does not receive the event currently being dispatched.
- Queue nested publications and drain them in first-in, first-out order after the current event finishes dispatching.
- Avoid recursive nested dispatch that can produce unbounded call-stack growth.
- Catch subscriber exceptions individually, continue delivering the event to remaining eligible subscribers, and expose failure diagnostics after publication.
- Preserve event type and subscriber identity information sufficient for focused diagnostics without creating a logging framework.
- Define clear behavior for publishing an event with no subscribers.
- Define clear behavior for clearing or disposing the event bus.
- Make bus disposal or cleanup idempotent.
- Reject publication and new subscriptions after final disposal with clear exceptions or result behavior.
- Avoid static mutable registries, global accessors, service locators, reflection-based event discovery, and scene searches.
- Keep public contracts small, documented, and suitable for later domain-specific event classes.
- Use the `Minerva` namespace root.
- Remain compatible with Unity 5.6 and its supported C# language/runtime constraints.
- Add focused tests for all required dispatch, mutation, nested-publication, failure, and disposal behavior.

## Event Contract Boundaries

The generic foundation may define abstractions such as:

- an event marker interface or minimal base type;
- an event publisher interface;
- an event subscriber interface or combined event-bus interface;
- a generic subscription handle;
- publication result and subscriber-failure diagnostic types;
- an in-memory event-bus implementation.

Exact names and file splits are implementation details within the authorized boundary.

The foundation must not require every event to inherit data, timestamps, correlation IDs, publisher IDs, or simulation metadata. Later layers may define envelopes or richer event base classes when their requirements are approved. This ticket should not pre-design those systems.

## Out of Scope

- narrative, story-variable, choice, or timeline event classes
- phone, app, screen, message-thread, contact, or notification event classes
- time, clock, scheduler, or delayed-event behavior
- save/load, persistence, event storage, replay, or event sourcing
- command, request/response, query, mediator, or RPC frameworks
- asynchronous, multithreaded, task-based, coroutine, or network dispatch
- priority-based subscribers
- wildcard subscriptions
- inheritance-polymorphic dispatch to base event types or interfaces
- attribute-based subscriptions
- reflection-based registration or discovery
- dependency-injection framework behavior
- global singleton or static event-bus access
- Unity scene objects, MonoBehaviours, ScriptableObjects, prefabs, or scenes
- editor windows or event-inspection tooling
- production logging framework
- analytics or telemetry integration
- domain event catalog or naming taxonomy beyond the existing architecture rules
- third-party event or messaging packages
- assembly definitions

## Dependencies and Prerequisites

- FND-001 is Complete.
- FND-002 is Complete and merged.
- PR #8 workflow documentation is merged.
- The Unity 5.6 project skeleton opens successfully.
- M0.3 Architecture Baseline is merged.
- No unresolved ADR is required for this narrow generic event implementation.

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

- Add generic event contracts, diagnostics, subscription handling, and an in-memory event-bus implementation under the authorized Core path.
- Add focused runtime tests under the authorized test path.
- Reuse the FND-002 runtime lifecycle contract only when needed for clean integration; do not redesign it.
- Remove obsolete `.gitkeep` placeholders only from directories that receive authorized files.
- Move this ticket from `Ready` to `Active` when implementation begins.
- Move this ticket from `Active` to `Review` when the implementation pull request opens.
- Complete the Implementation Report.
- Run narrow searches and Unity validation required by this ticket.

## Prohibited Actions

- Do not implement domain-specific event classes.
- Do not add event metadata fields solely for anticipated future systems.
- Do not implement inheritance-polymorphic or wildcard dispatch.
- Do not use reflection to discover event types or subscribers.
- Do not expose a static or globally discoverable event bus.
- Do not create a general mediator, command bus, query bus, or dependency-injection container.
- Do not use `GameObject.Find`, `FindObjectOfType`, scene-name lookup, or hidden singleton creation.
- Do not introduce third-party packages.
- Do not create scenes, prefabs, ScriptableObjects, or editor assets.
- Do not add assembly definitions.
- Do not change architecture, system, tool, ADR, planning, or project-governance documents.
- Do not move the ticket to `Complete`; the Technical Director owns acceptance after independent review.
- Do not merge the pull request.

## Expected Files

Exact names may vary when required by a clearer Unity 5.6-compatible design, but the implementation should remain within this shape:

- minimal event marker or generic constraint contract
- publisher/subscriber or combined event-bus contract
- disposable subscription implementation
- publication result and subscriber-failure diagnostic representation
- deterministic in-memory event-bus implementation
- focused EditMode runtime tests

All production files must remain under `Assets/Minerva/Runtime/Core/`. Tests should remain under the existing Unity 5.6-compatible runtime EditMode test location.

## Documentation Updates

- Move this ticket from `Ready` to `Active` when implementation begins; set `Status` to `Active`, record the owner, and update the date.
- Move this ticket from `Active` to `Review` when the implementation PR opens; set `Status` to `Review` and update the date.
- Complete the Implementation Report before requesting independent review.
- Do not move the ticket to `Complete`; after IRA validation, the Technical Director records acceptance and moves it before merge.
- Do not update planning or governance documents unless separately authorized.

## Architect's Notes

This ticket establishes generic machinery, not the project’s final domain event model. Prefer interfaces and small sealed plain-C# implementation types over speculative inheritance hierarchies.

Dispatch is exact-type only for this foundation. A subscriber to one event type receives only that exact event type. Base-type, interface-wide, wildcard, or hierarchy traversal can be evaluated later if a concrete use case requires it.

During publication, operate on stable dispatch semantics. Subscription mutations must have defined behavior rather than depending on collection-enumerator accidents.

Nested publication should be queued. For example, when handler A publishes event B while event A is being delivered, all eligible handlers for event A finish before event B delivery begins. Multiple nested publications retain FIFO order.

Subscriber exceptions are observations, not permission to abort unrelated subscribers. Failure diagnostics should remain local result data rather than introducing a global logging or telemetry system.

Events themselves should be treated as immutable by convention and API design, but this ticket must not create a complex immutability framework or clone event objects.

The event bus may implement the existing `IRuntimeService` lifecycle contract when doing so creates a clean explicit composition path. It must not require that integration if a smaller disposable plain-C# design fully satisfies the ticket.

## Acceptance Criteria

- [ ] Generic event contracts are reusable by later domain-specific event classes without modification.
- [ ] Subscriptions and publications are strongly typed.
- [ ] Subscription registration order determines handler invocation order for the same event type.
- [ ] Subscription handles are explicit, disposable, and idempotent.
- [ ] A handler unsubscribed before its turn during dispatch is skipped in that dispatch.
- [ ] A handler subscribed during dispatch does not receive the current event.
- [ ] Nested publications are queued and delivered FIFO after the current event completes.
- [ ] Nested publication does not recurse directly through the call stack.
- [ ] One subscriber exception does not prevent remaining eligible subscribers from receiving the event.
- [ ] Publication diagnostics identify the event type and each failed subscriber sufficiently for tests and debugging.
- [ ] Publishing with no subscribers succeeds with a clear empty result.
- [ ] Cleanup or disposal is idempotent and removes subscriptions.
- [ ] Publication and new subscription after final disposal are rejected clearly.
- [ ] Separate event-bus instances can be constructed and disposed repeatedly without leaked static state.
- [ ] Dispatch is exact event type only; no wildcard, hierarchy, or reflection-based dispatch is introduced.
- [ ] Runtime implementation contains no `UnityEditor` reference.
- [ ] No scene search, hidden singleton, global accessor, service locator, or reflection-based discovery is introduced.
- [ ] No domain-specific event, command, time, save, narrative, phone, app, notification, presentation, or platform behavior is added.
- [ ] Focused tests cover all required behavior.
- [ ] The final diff contains only authorized paths.
- [ ] The Implementation Report accurately records changes and validation.

## Required Validation

- List every created, modified, moved, and deleted path.
- Confirm no path outside Authorized Files or Directories changed.
- Run all focused runtime EditMode tests available in the environment.
- Open the project in Unity 5.6 batch mode and report compiler/import results when Unity 5.6 is available.
- Search runtime files for `UnityEditor`; no matches may exist.
- Search implementation for `GameObject.Find`, `FindObjectOfType`, static/global event-bus access, service-locator access, reflection-based discovery, wildcard dispatch, and mutable static subscriber collections; no prohibited implementation may exist.
- Confirm no domain-specific event classes were added.
- Confirm no `.unity`, `.prefab`, `.asset`, `.asmdef`, package, or vendor file was added.
- Confirm all new text files end with a newline.
- Run `git diff --check`.
- Confirm this ticket exists in exactly one workflow directory and its `Status` matches that directory.

## Definition of Done

- [ ] Acceptance criteria satisfied.
- [ ] Required validation reported accurately.
- [ ] Documentation updated.
- [ ] No unauthorized changes.
- [ ] Implementation Report completed.
- [ ] Independent Review record completed by the IRA.
- [ ] Technical Director Acceptance record completed before merge.

## Implementation Report

### Status

### Changed Files

### Work Completed

### Validation

### Deviations

### Blockers or Risks

### Optional Context Used

### Follow-Up Suggestions

## Independent Review

Completed by the Implementation Review Agent after the implementation pull request enters `Review`.

### Recommendation

### Findings

### Validation Evidence Reviewed

### Residual Risks

## Technical Director Acceptance

Completed after independent review and before merge.

### Decision

### PR Reference

### Accepted Validation Evidence

### Accepted Deviations

### Follow-Up Tickets
