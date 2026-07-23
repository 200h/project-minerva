# FND-004: Create Runtime Composition Root

**Status:** Ready  
**Owner:** Unassigned  
**Created:** 2026-07-23  
**Updated:** 2026-07-23  
**Roadmap Phase:** Phase 1 — Foundation  
**Epic:** [Foundation Runtime Services](../../../Planning/Epics/Foundation_Runtime_Services.md)  
**Related ADRs:** [`../../ADR/0001_Use_Unity_5_6.md`](../../ADR/0001_Use_Unity_5_6.md), [`../../ADR/0004_Ticket_Driven_AI_Implementation.md`](../../ADR/0004_Ticket_Driven_AI_Implementation.md)

## Summary

Create the explicit plain-C# composition root that constructs, starts, exposes, and owns the accepted foundation runtime services.

The composition root must integrate the accepted FND-002 `RuntimeBootstrap` lifecycle and FND-003 `InMemoryEventBus` contracts without introducing service lookup, scene discovery, global state, or domain-specific behavior.

## Background

FND-002 established explicit service registration, deterministic initialization, first-failure handling, reverse shutdown, and idempotent disposal through `RuntimeBootstrap`. FND-003 established the instance-owned `IEventBus` / `InMemoryEventBus` event foundation with explicit subscriptions and idempotent disposal.

Those accepted mechanisms are currently independent. Later foundation tickets need one authoritative construction boundary that creates a coherent runtime instance, transfers ownership clearly, returns only narrow capabilities to consumers, and prevents partially initialized resources from escaping when startup fails.

This ticket composes accepted contracts. It must not redesign them, generalize them into a dependency-injection container, or implement any later service.

## Goal

Provide a Unity 5.6-compatible runtime composition boundary that:

- constructs one instance-owned core event bus per composed runtime;
- constructs and configures one `RuntimeBootstrap` per composed runtime;
- registers lifecycle-owned services explicitly in a documented deterministic order;
- initializes the runtime as one composition operation;
- returns either a usable composed runtime or a diagnostic startup failure, never a usable partial runtime;
- cleans up all composition-owned resources when startup fails;
- exposes event publishing and subscription through narrow accepted interfaces rather than concrete lookup;
- shuts down and disposes all owned resources exactly once;
- permits separate runtime instances to be composed, used, and disposed repeatedly without shared mutable state.

## Required Context

Read only:

- `Docs/Project/README.md`
- `Docs/Project/ARCHITECTURAL_PRINCIPLES.md`
- `Docs/Project/CODING_STANDARDS.md`
- `Docs/Project/AI_ENGINEERING_GUIDE.md`
- `Docs/Architecture/SYSTEM_OVERVIEW.md`
- `Docs/Architecture/RUNTIME_LAYERS.md`
- `Docs/Architecture/DEPENDENCY_RULES.md`
- `Docs/Architecture/INITIALIZATION_AND_LIFECYCLE.md`
- `Docs/Architecture/EVENT_ARCHITECTURE.md`
- `Docs/Architecture/EDITOR_RUNTIME_BOUNDARIES.md`
- `Docs/Context/Foundation.md`
- `Docs/Context/Runtime.md`
- `Docs/Systems/Event_Bus_System.md`
- `Docs/Handbook/Definition_of_Done.md`
- `Docs/Tickets/Complete/FND-002_Create_Runtime_Bootstrap.md`
- `Docs/Tickets/Complete/FND-003_Create_Core_Event_Bus.md`

## Optional Context

Read only when required by a specific implementation question:

- `Docs/Architecture/STATE_OWNERSHIP.md`
- `Docs/Handbook/Git_Workflow.md`
- `Docs/Handbook/Branch_Strategy.md`
- `Planning/Epics/Foundation_Runtime_Services.md`

Do not load unrelated system, tool, narrative, phone, save, or planning documents.

## Code Areas to Inspect

Inspect only:

- `Assets/Minerva/Runtime/Core/IRuntimeService.cs`
- `Assets/Minerva/Runtime/Core/RuntimeBootstrap.cs`
- `Assets/Minerva/Runtime/Core/RuntimeInitializationResult.cs`
- `Assets/Minerva/Runtime/Core/RuntimeLifecycleState.cs`
- `Assets/Minerva/Runtime/Core/RuntimeShutdownFailure.cs`
- `Assets/Minerva/Runtime/Core/RuntimeShutdownResult.cs`
- `Assets/Minerva/Runtime/Core/ServiceInitializationResult.cs`
- `Assets/Minerva/Runtime/Core/IEvent.cs`
- `Assets/Minerva/Runtime/Core/IEventBus.cs`
- `Assets/Minerva/Runtime/Core/IEventPublisher.cs`
- `Assets/Minerva/Runtime/Core/IEventSubscriber.cs`
- `Assets/Minerva/Runtime/Core/InMemoryEventBus.cs`
- `Assets/Minerva/Runtime/Core/EventPublicationResult.cs`
- `Assets/Minerva/Runtime/Core/EventSubscriberFailure.cs`
- `Assets/Minerva/Tests/Runtime/Editor/RuntimeBootstrapTests.cs`
- `Assets/Minerva/Tests/Runtime/Editor/InMemoryEventBusTests.cs`
- this ticket file for lifecycle movement and reporting

Do not perform repository-wide discovery beyond Required Context and these code areas.

## Accepted Contract Constraints

The implementation must preserve these accepted behaviors:

- `RuntimeBootstrap` owns explicit `IRuntimeService` registration, one initialization attempt, first-failure reporting, reverse shutdown of successfully initialized services, idempotent shutdown, and disposal.
- `InMemoryEventBus` is a plain-C# instance with explicit subscriptions and idempotent disposal.
- `IEventPublisher` and `IEventSubscriber` are the narrow consumer-facing event capabilities.
- Neither accepted subsystem exposes or requires a global registry, scene object, `MonoBehaviour`, reflection discovery, or editor API.
- FND-004 may add composition-specific types and the smallest lifecycle adapter required to own an accepted disposable dependency. It must not change the public semantics of FND-002 or FND-003.

## Ownership and Lifecycle Semantics

- Each composition attempt creates fresh runtime-owned objects. No mutable runtime object may be shared between separate composed runtimes.
- The composition root is the only production location in this ticket that knows the concrete `RuntimeBootstrap` and `InMemoryEventBus` types.
- The composed runtime owns its bootstrap, event bus, all composition-created lifecycle adapters, and every service explicitly transferred to it by the composition operation.
- Ownership transfer must be explicit in public XML documentation. After transfer, callers must not dispose transferred services independently.
- Consumers may obtain event publication and subscription capabilities through `IEventPublisher` and `IEventSubscriber`. They must not obtain arbitrary services by type, string, key, or generic lookup.
- The event bus must participate in runtime-owned shutdown through one explicit lifecycle path. It must not be disposed independently through competing production paths.
- Successful startup returns one usable composed-runtime handle. That handle is the lifetime boundary and must implement idempotent disposal.
- Disposing the composed runtime delegates lifecycle cleanup to the accepted bootstrap behavior and leaves event capabilities unusable.
- A startup failure must return diagnostic information that preserves the accepted `RuntimeInitializationResult` failure details.
- A failed composition must synchronously clean up every resource whose ownership was created or transferred during that attempt. It must not return a usable runtime handle or event capability.
- Cleanup after startup failure must preserve the bootstrap's reverse-order shutdown semantics and must not hide cleanup failures. Startup and cleanup outcomes must remain separately inspectable when both fail.
- Repeated calls to dispose a successful composed runtime must be safe and must not repeat service shutdown or event-bus disposal.

## Requirements

- Add one narrow, explicit composition-root entry point under `Minerva.Core`.
- Add the smallest composed-runtime lifetime type required to expose approved capabilities and own disposal.
- Add an explicit composition result or equivalent contract that distinguishes successful startup from failed startup without returning a usable partial runtime.
- Create and own exactly one `InMemoryEventBus` for each successful composed runtime.
- Integrate event-bus disposal into `RuntimeBootstrap` lifecycle ownership with a small explicit adapter if required.
- Register all lifecycle services through `RuntimeBootstrap.Register` in documented deterministic order.
- Call `RuntimeBootstrap.Initialize` exactly once per composition attempt.
- Preserve accepted initialization failure type and reason data.
- Ensure startup failure triggers immediate lifecycle cleanup before the composition operation returns.
- Surface shutdown cleanup failures without replacing or erasing the original startup failure.
- Expose no mutable service collection and no post-start registration API.
- Expose no concrete event-bus implementation to ordinary consumers when the accepted interfaces are sufficient.
- Allow the minimum explicit construction seam needed for focused failure and ownership tests. The seam must not become a public general-purpose dependency-injection container or service locator.
- Keep public contracts small, XML documented, and compatible with Unity 5.6 / supported C# syntax.
- Add focused Unity 5.6-compatible EditMode tests for composition, capability exposure, deterministic lifecycle, failure cleanup, disposal, and isolation.
- Preserve all accepted FND-002 and FND-003 tests.

## Startup-Failure Behavior

The following behavior is mandatory:

1. Construct composition-owned dependencies.
2. Register lifecycle services explicitly in the composition-defined order.
3. Initialize through `RuntimeBootstrap` once.
4. On success, return a usable composed runtime and no failure.
5. On a reported initialization failure, null result, or thrown initialization exception captured by `RuntimeBootstrap`, dispose the bootstrap before returning.
6. Return the original `RuntimeInitializationResult` failure details.
7. Also return any shutdown failures produced while cleaning up successfully initialized services.
8. Do not return a composed runtime, publisher, subscriber, concrete event bus, or other usable owned dependency after failure.
9. Do not retry initialization implicitly.
10. Do not publish runtime-ready or failure events; lifecycle event types belong to a later ticket.

## Out of Scope

- modifying the accepted dispatch, subscription, lifecycle, initialization, shutdown, or disposal semantics from FND-002 or FND-003;
- domain-specific, lifecycle-specific, diagnostic, narrative, phone, application, notification, state, save, time, scheduling, UI, presentation, or platform event classes;
- clock or scheduled-task implementation;
- runtime state primitives;
- snapshot or persistence contracts;
- configuration loading;
- file or disk access;
- logging framework;
- event envelopes, timestamps, correlation identifiers, or provenance metadata;
- command bus, request/response bus, middleware, event history, or event sourcing;
- general-purpose dependency injection;
- service locator, keyed lookup, generic `Get<T>` access, or mutable service registry;
- reflection-based construction or discovery;
- scene search, hierarchy lookup, `MonoBehaviour` composition, hidden singleton creation, or Unity lifecycle authority;
- asynchronous or multithreaded startup/shutdown;
- scenes, prefabs, ScriptableObjects, Resources assets, packages, or assembly definitions;
- Story Timeline, Game Director, editor windows, or editor tooling;
- platform-specific Windows or iOS behavior.

## Dependencies and Prerequisites

- FND-001 is Complete and merged.
- FND-002 is Complete and merged.
- FND-003 is Complete and merged.
- The accepted FND-002 and FND-003 contracts and tests are present on `main`.
- M0.3 Architecture Baseline is merged.
- No unresolved ADR is required for this narrow composition implementation.

## Authorized Files or Directories

- `Assets/Minerva/Runtime/Core/**`
- `Assets/Minerva/Tests/Runtime/Editor/**`
- this ticket file for lifecycle movement and reporting

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

Existing accepted Core files may be changed only when a narrowly required integration defect makes composition impossible. If such a change would alter an accepted public contract or behavior, stop and report rather than editing it.

## Permitted Actions

- Add composition-root, composed-runtime, composition-result, and narrowly required lifecycle-adapter types under the authorized Core path.
- Add stable Unity `.meta` files for every new Unity asset.
- Add focused EditMode tests under the authorized runtime test path.
- Use existing FND-002 and FND-003 public contracts directly.
- Move this ticket from `Ready` to `Active` when implementation begins.
- Move this ticket from `Active` to `Review` when the implementation pull request opens.
- Complete the Implementation Report and execution-state entries.
- Run narrow searches and Unity validation required by this ticket.

## Prohibited Actions

- Do not modify accepted FND-002 or FND-003 behavior to make composition easier.
- Do not add a service locator, dependency-injection container, global composition accessor, or mutable static state.
- Do not expose `RuntimeBootstrap.Register` through the successful runtime handle.
- Do not expose a general collection or lookup of composed services.
- Do not use `GameObject.Find`, `FindObjectOfType`, scene-name lookup, hierarchy lookup, or hidden singleton creation.
- Do not use reflection for construction, discovery, registration, or dispatch.
- Do not introduce third-party packages.
- Do not create scenes, prefabs, ScriptableObjects, Resources assets, editor assets, or assembly definitions.
- Do not add domain-specific event messages or future foundation services.
- Do not change architecture, system, tool, ADR, epic, planning, or project-governance documents.
- Do not move this ticket to `Complete`; independent review and Technical Director acceptance are required.
- Do not merge the implementation pull request.

## Expected Files

Exact names may vary when a clearer Unity 5.6-compatible design is justified, but the implementation should remain close to:

- one runtime composition-root/factory type;
- one composed-runtime lifetime/capability type;
- one composition result/failure representation;
- one narrow adapter that allows the bootstrap to own event-bus disposal, if required;
- stable `.meta` files for new Unity assets;
- one focused composition-root EditMode test fixture.

All production files must remain under `Assets/Minerva/Runtime/Core/`. All new tests must remain under `Assets/Minerva/Tests/Runtime/Editor/`.

## Documentation Updates

- Set `Status` to `Active`, set `Owner` to the implementation agent, and update `Updated` using `America/New_York` when moving from `Ready` to `Active`.
- Set `Status` to `Review` and update `Updated` when moving from `Active` to `Review`.
- Complete the Implementation Report before requesting independent review.
- Record execution states using `YYYY-MM-DD HH:mm z` in `America/New_York`.
- Do not update the epic or current-work planning documents; those are outside implementation authority.
- Do not move the ticket to `Complete`; the Technical Director performs acceptance closeout before merge.

## Architect's Notes

The composition root may know concrete types. The composed services and consumers must not know the composition root or discover peers globally.

Prefer one atomic composition operation over a public multi-step builder. A builder is permitted only if it cannot leak a partially configured runtime, cannot be reused after composition, and does not become a general registration API.

The accepted `RuntimeBootstrap` already provides ordering, failure capture, reverse shutdown, and disposal. Compose around that behavior rather than duplicating lifecycle logic.

The event bus is not currently an `IRuntimeService`. If an adapter is introduced, it should do no work during initialization beyond reporting success and should dispose the bus during shutdown. Ownership must remain singular and obvious.

A narrow testing seam may accept explicitly provided lifecycle-service test doubles or an internal factory. It must be justified in the Implementation Report and must not expose runtime service lookup to production consumers.

Do not anticipate FND-005 clock composition. FND-005 will extend the accepted composition boundary through a separate reviewed ticket.

## Acceptance Criteria

- [ ] One explicit composition root creates the approved foundation runtime instance.
- [ ] Each composition attempt creates a fresh `RuntimeBootstrap` and fresh `InMemoryEventBus` with no shared mutable runtime state.
- [ ] Successful composition initializes registered services once in documented deterministic order.
- [ ] Successful composition returns one usable runtime lifetime handle and no startup failure.
- [ ] The successful runtime exposes event publishing and subscription through `IEventPublisher` and `IEventSubscriber` or an equally narrow accepted interface boundary.
- [ ] Ordinary consumers cannot access arbitrary services through type, key, string, generic lookup, collection enumeration, or mutable registry.
- [ ] The successful runtime does not expose post-start service registration.
- [ ] Disposing the successful runtime shuts down initialized services in reverse order and disposes the event bus exactly once.
- [ ] Repeated disposal of the successful runtime is safe and does not repeat owned cleanup.
- [ ] Event publication and subscription reject new work after the composed runtime is disposed, consistent with accepted event-bus behavior.
- [ ] Reported, null-result, and thrown initialization failures produce no usable runtime handle.
- [ ] Startup failure preserves the original failed service type and failure reason from `RuntimeInitializationResult`.
- [ ] Startup failure immediately cleans up every successfully initialized or composition-owned resource before returning.
- [ ] Cleanup failures during failed startup remain inspectable without erasing the original initialization failure.
- [ ] Separate runtimes can be composed, used, and disposed repeatedly without shared subscriptions or leaked static state.
- [ ] Existing FND-002 and FND-003 tests continue to pass unchanged unless a stop condition is raised.
- [ ] Runtime implementation contains no `UnityEditor` reference.
- [ ] No scene search, hidden singleton, global registry, service locator, dependency-injection container, or reflection discovery is introduced.
- [ ] No domain-specific or later-foundation behavior is added.
- [ ] Focused tests cover success, capability exposure, ordering, ownership, failure cleanup, cleanup failure reporting, idempotent disposal, and runtime-instance isolation.
- [ ] Every new Unity asset has a stable `.meta` file.
- [ ] The final diff contains only authorized paths.
- [ ] The implementation report accurately records changes, design choices, optional context, and validation.
- [ ] The ticket directory and `Status` metadata match throughout implementation.
- [ ] The implementation pull request contains this ticket under `Docs/Tickets/Review/` while independent review is pending.
- [ ] After acceptance, the implementation pull request contains this ticket under `Docs/Tickets/Complete/` before merge.

## Required Validation

- List every created, modified, moved, and deleted path.
- Confirm no path outside Authorized Files or Directories changed.
- Run the complete existing runtime EditMode test suite, not only new composition tests.
- Report total tests, assertions, failures, skipped tests, and inconclusive tests when the runner provides them.
- Open/import the exact `Assets/Minerva` tree in Unity 5.6 batch mode and report compiler/import results when Unity 5.6 is available.
- Confirm Unity does not rewrite committed `.meta` files or GUIDs during validation.
- Search runtime files for `UnityEditor`; no matches may exist.
- Search implementation for `GameObject.Find`, `FindObjectOfType`, scene/hierarchy lookup, global registries, service-locator APIs, generic `Get<T>` service access, reflection-based construction/discovery, and mutable static runtime collections; no prohibited implementation may exist.
- Search new public APIs for arbitrary service enumeration, lookup, or post-start registration; none may exist.
- Confirm no domain-specific event class or later foundation service was added.
- Confirm no `.unity`, `.prefab`, `.asset`, `.asmdef`, package, or vendor file was added.
- Confirm all new text files end with a newline.
- Run `git diff --check`.
- Confirm the ticket exists in exactly one workflow directory and its `Status` matches that directory.
- Confirm the linked epic exists and identifies FND-004 as the current `Ready` ticket on `main`; do not edit planning files during implementation.

If Unity 5.6 or another required validation tool is unavailable, report that limitation accurately. Do not substitute a newer Unity version as proof of Unity 5.6 compatibility.

## Stop Conditions

Stop and report rather than guessing when:

- an accepted FND-002 or FND-003 public contract must change to satisfy this ticket;
- composition requires modifying a prohibited path;
- ownership cannot be represented without exposing a service locator, mutable registry, or partial runtime;
- startup cleanup cannot preserve both initialization and shutdown failure information within the authorized scope;
- Unity 5.6 compatibility requires an unapproved language feature, package, assembly definition, or project setting;
- required context conflicts or leaves ownership materially ambiguous;
- existing accepted tests fail before implementation changes or require semantic alteration;
- the ticket cannot be completed within authorized paths.

## Definition of Done

- [ ] Acceptance criteria satisfied.
- [ ] Required validation reported accurately.
- [ ] Documentation updated.
- [ ] No unauthorized changes.
- [ ] Implementation report completed.
- [ ] Ticket exists only in `Docs/Tickets/Review/` when implementation review begins.

## Implementation Report

### Status

### Changed Files

### Work Completed

### Composition and Ownership Design

### Startup-Failure and Cleanup Behavior

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

### Lifecycle and Ownership Assessment

### Startup-Failure Assessment

### Validation Assessment

### Blocking Findings

### Recommendation

`Accept`, `Changes Required`, or `Blocked`.

## Technical Director Acceptance

Completed after reviewing the Implementation Review Agent recommendation and before merge.

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
| Planned | 2026-07-23 17:30 EDT | Technical Director | Readiness pass completed against accepted FND-002 and FND-003 contracts; ticket promoted to Ready in documentation PR. |
| In Progress |  |  |  |
| Committed |  |  |  |
| Verified |  |  |  |
