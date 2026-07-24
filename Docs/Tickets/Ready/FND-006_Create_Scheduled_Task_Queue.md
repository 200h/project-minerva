# FND-006: Create Scheduled Task Queue

**Status:** Ready  
**Owner:** Unassigned  
**Created:** 2026-07-23  
**Updated:** 2026-07-23  
**Roadmap Phase:** Phase 1 — Foundation  
**Epic:** [Foundation Runtime Services](../../../Planning/Epics/Foundation_Runtime_Services.md)  
**Related ADRs:** [Use Unity 5.6](../../ADR/0001_Use_Unity_5_6.md), [Ticket-Driven AI Implementation](../../ADR/0004_Ticket_Driven_AI_Implementation.md)

## Summary

Create the generic, deterministic, single-threaded scheduled-task queue that later runtime systems can use to execute callbacks when the accepted FND-005 runtime clock reaches an authored elapsed timestamp.

FND-006 defines scheduling, cancellation, bounded due-work draining, callback-failure diagnostics, and lifecycle ownership only. It does not implement narrative events, recurring schedules, automatic ticking, persistence, event-bus publication, platform time, or domain policy.

## Background

FND-005 accepted exact nonnegative `RuntimeInstant` and `RuntimeDuration` millisecond values, the narrow read-only `IRuntimeClock` contract, and a deterministic `ManualRuntimeClock`. The clock advances only through explicit control and participates in the accepted runtime lifecycle through `IRuntimeService`.

Later systems need a deterministic way to register work for an elapsed runtime timestamp and explicitly process work that has become due. The queue must preserve total order, remain safe when callbacks schedule or cancel other work, isolate callback failures, prevent one drain call from executing unbounded work, and shut down through the existing lifecycle contracts.

The queue is not an update loop. A future authorized owner will decide when the clock advances and when draining occurs. FND-006 consumes `IRuntimeClock` synchronously and exposes explicit scheduling and processing capabilities without changing the accepted clock or composition contracts.

## Goal

Provide Unity 5.6-compatible plain-C# scheduling primitives that:

- accept past, current, and future elapsed runtime timestamps without normalization;
- order scheduled callbacks first by due timestamp and then by instance-local insertion sequence;
- return explicit cancellation handles with deterministic observable state;
- define scheduling and cancellation before, during, and after a drain;
- capture one drain boundary so newly scheduled work never joins the active drain;
- bound callback attempts per drain and report when eligible work remains;
- isolate callback exceptions and return actionable diagnostics while continuing unrelated eligible work;
- reject reentrant draining without corrupting the active drain;
- participate in explicit runtime lifecycle ownership through `IRuntimeService`;
- remain free of automatic advancement, domain meaning, persistence, global access, and Unity scene behavior.

## Required Context

Read only:

- Docs/Project/README.md
- Docs/Project/ARCHITECTURAL_PRINCIPLES.md
- Docs/Project/CODING_STANDARDS.md
- Docs/Project/AI_ENGINEERING_GUIDE.md
- Docs/Architecture/SYSTEM_OVERVIEW.md
- Docs/Architecture/RUNTIME_LAYERS.md
- Docs/Architecture/DEPENDENCY_RULES.md
- Docs/Architecture/INITIALIZATION_AND_LIFECYCLE.md
- Docs/Architecture/EDITOR_RUNTIME_BOUNDARIES.md
- Docs/Context/Foundation.md
- Docs/Context/Runtime.md
- Docs/Systems/Time_And_Clock_System.md
- Docs/Handbook/Definition_of_Done.md
- Docs/Handbook/Ticket_Workflow.md
- Docs/Tickets/Complete/FND-004_Create_Runtime_Composition_Root.md
- Docs/Tickets/Complete/FND-005_Create_Core_Clock_Abstractions.md

## Optional Context

Read only when required by a specific implementation question:

- Docs/Tickets/Complete/FND-002_Create_Runtime_Bootstrap.md
- Docs/Tickets/Complete/FND-003_Create_Core_Event_Bus.md
- Assets/Minerva/Runtime/Core/RuntimeBootstrap.cs
- Assets/Minerva/Runtime/Core/RuntimeCompositionRoot.cs
- Assets/Minerva/Runtime/Core/RuntimeCompositionResult.cs
- Assets/Minerva/Runtime/Core/ComposedRuntime.cs
- Assets/Minerva/Tests/Runtime/Editor/RuntimeCompositionRootTests.cs
- Assets/Minerva/Runtime/Core/InMemoryEventBus.cs
- Assets/Minerva/Tests/Runtime/Editor/InMemoryEventBusTests.cs
- Docs/Handbook/Git_Workflow.md
- Docs/Handbook/Branch_Strategy.md
- Planning/Epics/Foundation_Runtime_Services.md

Do not load unrelated state, save, narrative, phone, notification, application, UI, presentation, editor-tool, content, platform, or planning documents.

## Code Areas to Inspect

Inspect only:

- Assets/Minerva/Runtime/Core/RuntimeInstant.cs
- Assets/Minerva/Runtime/Core/RuntimeDuration.cs
- Assets/Minerva/Runtime/Core/IRuntimeClock.cs
- Assets/Minerva/Runtime/Core/IRuntimeClockControl.cs
- Assets/Minerva/Runtime/Core/ManualRuntimeClock.cs
- Assets/Minerva/Runtime/Core/IRuntimeService.cs
- Assets/Minerva/Runtime/Core/ServiceInitializationResult.cs
- Assets/Minerva/Runtime/Core/RuntimeCompositionRoot.cs
- Assets/Minerva/Tests/Runtime/Editor/ManualRuntimeClockTests.cs
- Assets/Minerva/Tests/Runtime/Editor/RuntimeCompositionRootTests.cs
- this ticket file for lifecycle movement and reporting

Use narrow exact-symbol searches only when required to locate a direct accepted dependency. Do not perform repository-wide discovery.

## Approved Queue Ownership and Lifecycle

- The concrete scheduled-task queue is a sealed plain-C# instance with no static mutable state.
- The constructor requires a non-null `IRuntimeClock` and retains only that narrow read capability.
- The queue implements `IRuntimeService` so callers can compose the clock before the queue through `RuntimeCompositionRoot.Compose(clock, queue)`.
- Lifecycle registration order is explicit: a queue that depends on a lifecycle-owned clock must be registered after that clock so reverse shutdown stops the queue before the clock.
- The queue is operational immediately after construction for focused deterministic tests. `Initialize` is idempotently successful before shutdown and must not alter scheduled work, handle state, insertion order, or drain state.
- `Shutdown` is idempotent and terminal. It rejects future scheduling and draining, cancels every still-pending task, releases callback references held by pending entries, and does not invoke callbacks.
- If `Shutdown` is called by an executing callback, the current callback is allowed to return, every other pending task becomes cancelled, and the active drain stops without invoking another callback.
- `Initialize` after shutdown returns an actionable failed `ServiceInitializationResult`, does not throw, and does not revive the queue.
- Separate queue instances, including queues using separate clock instances with equal values, share no task state, insertion counter, lifecycle state, or diagnostics.
- The queue must not implement `IDisposable` when that would create a competing lifecycle path after ownership transfer. `IRuntimeService.Shutdown` is authoritative.

## Approved Public Scheduling Contracts

The implementation must expose small, XML-documented contracts equivalent to:

- a scheduling capability that accepts a `RuntimeInstant` and a non-null parameterless callback;
- a processing capability that explicitly drains due callbacks with a positive caller-supplied callback-attempt limit;
- a task handle that exposes immutable due time, observable task state, and an idempotent cancellation operation;
- a task-state enumeration with exactly the semantic states `Pending`, `Executing`, `Completed`, and `Cancelled`;
- an immutable drain result containing callback-attempt count, successful-completion count, cancellation-skip count, callback failures, and whether eligible work remains because the limit was reached;
- immutable callback-failure diagnostics sufficient to identify due time, insertion order, callback target/type and method where available, exception type, and exception message.

Exact type names may vary only when a clearer equivalent preserves these boundaries. The anticipated shape is:

- `IScheduledTaskScheduler` with `IScheduledTaskHandle Schedule(RuntimeInstant dueTime, Action callback)`;
- `IScheduledTaskProcessor` with `ScheduledTaskDrainResult DrainDue(int maxCallbacks)`;
- `IScheduledTaskHandle` with `RuntimeInstant DueTime`, `ScheduledTaskState State`, and `bool Cancel()`;
- sealed `ScheduledTaskQueue` implementing the scheduling, processing, and lifecycle contracts.

The API must not expose the mutable internal collection, arbitrary task lookup, rescheduling, due-time mutation, priority, recurrence, or callback replacement.

## Approved Scheduling Semantics

### Timestamp Acceptance

- `Schedule` accepts any valid `RuntimeInstant`: earlier than, equal to, or later than the clock's current time.
- Scheduling never rewrites, clamps, or normalizes the supplied due time.
- A past- or current-due task is eligible on the next qualifying `DrainDue` call; scheduling does not invoke it immediately.
- A future task becomes eligible only when a later drain captures a clock value at or after its due time.
- A null callback throws `ArgumentNullException` before queue state or insertion order changes.
- Scheduling after shutdown throws `ObjectDisposedException` before queue state changes.

### Total Ordering

- Each successfully scheduled task receives an instance-local strictly increasing insertion sequence.
- Ordering is ascending due timestamp, then ascending insertion sequence.
- Equal-timestamp work executes in exact scheduling order, regardless of the underlying collection implementation.
- Cancellation does not renumber or reuse insertion sequences.
- A new queue instance starts with a fresh sequence domain.
- If another successful insertion sequence cannot be represented, scheduling throws `OverflowException` atomically without adding the task, retaining its callback, or changing existing order.
- Insertion sequence is exposed only through diagnostics when needed; it is not a public task identifier or cross-session stable ID.

## Approved Cancellation Semantics

- A newly returned handle is `Pending`.
- Calling `Cancel` on a pending task atomically changes it to `Cancelled`, releases the queued callback reference as soon as practical, and returns `true`.
- Repeated cancellation of a cancelled task returns `false` and changes nothing.
- Cancellation of an executing task, including self-cancellation from its callback, returns `false`, does not interrupt the callback, and leaves it `Executing` until the callback returns or throws.
- Cancellation of a completed task returns `false` and changes nothing.
- A pending task cancelled before its turn in an active drain must not execute in that drain.
- A callback may cancel another pending task scheduled later in the active drain; that task is skipped without consuming callback-attempt budget.
- Cancellation after queue shutdown returns `false` because shutdown has already changed every pending task to `Cancelled`. Handle reads remain available after queue shutdown.
- A callback that returns normally or throws becomes `Completed`; callback failure is not a cancellation state.
- Completed and cancelled handles remain readable and must not retain callback targets unnecessarily.

## Approved Drain Boundary and Mutation Semantics

### Captured Boundary

At the start of each successful non-reentrant `DrainDue` call, the queue captures:

1. the clock's current `RuntimeInstant` as the due-time cutoff; and
2. the highest insertion sequence already assigned as the eligibility ceiling.

Only tasks that were successfully scheduled before that boundary, remain pending, have due time at or before the captured cutoff, and have insertion sequence at or below the captured ceiling may execute in that drain.

Consequences:

- Clock advancement during a callback does not expand the active drain's cutoff.
- Work scheduled during a callback never executes in the active drain, even when it is past-due, current-due, or has the same due time as the executing callback.
- Newly scheduled due work is eligible on a later drain.
- This snapshot rule, together with the caller-supplied limit, prevents self-scheduling same-tick loops from becoming unbounded inside one drain.

### Processing Order and Mutation

- `DrainDue` invokes eligible pending callbacks one at a time in total queue order.
- Immediately before invocation, the task changes from `Pending` to `Executing`.
- After the callback returns or throws, the task changes to `Completed` and releases its callback reference.
- Tasks cancelled before their turn are removed or skipped deterministically and do not consume callback-attempt budget.
- Scheduling during a callback is allowed and follows the captured-boundary rule.
- Cancelling pending work during a callback is allowed and follows the cancellation rules.
- Pausing or advancing the clock from a callback does not change the captured cutoff for the active drain.
- No callback is invoked more than once.

## Approved Bounded-Drain Contract

- `DrainDue` requires `maxCallbacks` greater than zero. Zero or a negative value throws `ArgumentOutOfRangeException` before queue or task state changes.
- The limit counts callback attempts, including callbacks that throw. Cancelled tasks skipped without invocation do not consume the limit.
- The result's attempted count equals successful completions plus callback-failure count.
- The result reports the number of cancelled entries skipped or removed while resolving the active eligible set.
- When the callback-attempt limit is reached and at least one additional task from the captured eligible set remains pending, the result sets `LimitReached` to `true` and `HasRemainingDueWork` to `true`.
- When the limit is reached exactly as the captured eligible set is exhausted, both flags are `false`.
- `HasRemainingDueWork` and `LimitReached` describe only eligible work from the captured boundary. Work scheduled during the drain is deliberately excluded even if already due.
- A later drain may process remaining due work with a new cutoff and eligibility ceiling.
- Draining when no work is eligible returns a completed empty result and changes no task state.

## Approved Callback-Failure Diagnostics

- Each callback is invoked inside an individual exception boundary.
- An exception from one callback is captured as an immutable failure diagnostic; it does not escape `DrainDue` and does not prevent later eligible callbacks from running within the remaining limit.
- The failed task becomes `Completed` after the callback exits through the exception.
- Failure diagnostics record the scheduled due time, insertion sequence, callback target type when available, callback method name when available, exception type, and exception message.
- Diagnostics must not retain the exception object, callback delegate, callback target, or mutable task entry after result construction.
- The drain result preserves failure order in callback-attempt order.
- This ticket does not log, publish an event, retry, reschedule, aggregate-throw, or assign domain meaning to callback failures.

## Approved Reentrant Drain Behavior

- A direct or indirect call to `DrainDue` while the same queue is already draining throws `InvalidOperationException` immediately without starting a nested drain or changing queued task state.
- If a scheduled callback makes that reentrant call, the outer drain captures the resulting exception as that callback's failure under the normal isolation rule, marks the callback task `Completed`, and continues later eligible work.
- The queue must restore its non-draining state through `finally`-equivalent cleanup even when internal processing encounters an unexpected exception.
- Reentrancy is instance-local. Draining a different queue instance from a callback is permitted.

## Clock and Composition Boundary

- The queue depends only on the accepted read-only `IRuntimeClock` contract.
- It must not require or cast to `IRuntimeClockControl`.
- It must not advance, pause, resume, replace, or dispose the clock.
- It reads `CurrentTime` once when establishing each drain boundary and does not poll the clock between callbacks.
- FND-006 must prove compatibility by explicitly constructing a clock and queue, then supplying them to `RuntimeCompositionRoot.Compose(clock, queue)` in dependency order.
- Runtime disposal shuts down the queue before the clock through the accepted reverse-order lifecycle.
- FND-006 does not modify `RuntimeCompositionRoot`, `ComposedRuntime`, `RuntimeCompositionResult`, `RuntimeBootstrap`, or clock contracts.
- FND-006 does not make the composition root automatically construct or expose a queue.
- Callers retain only explicitly authorized narrow scheduler and processor capabilities before transferring lifecycle ownership.
- No service locator, static accessor, scene discovery, reflection discovery, or arbitrary runtime-service lookup may be added.

## Requirements

- Add small public scheduling, processing, handle, state, result, and failure-diagnostic contracts under `Minerva.Core`.
- Add one sealed deterministic `ScheduledTaskQueue` that consumes `IRuntimeClock` and implements `IRuntimeService`.
- Implement the approved timestamp acceptance, total ordering, cancellation, captured-boundary, bounded-drain, failure-isolation, reentrancy, lifecycle, shutdown, and composition semantics exactly.
- Use an internal data structure that preserves deterministic ordering without relying on APIs introduced after Unity 5.6.
- Reject invalid constructor and method arguments before observable queue state changes.
- Release callback references when work becomes completed, cancelled, or discarded by shutdown.
- Keep all public contracts XML documented.
- Use only Unity 5.6-compatible C# syntax and runtime libraries.
- Add one focused Unity 5.6-compatible EditMode test fixture covering the complete approved behavior.
- Preserve all accepted FND-002 through FND-005 production files and tests unchanged.

## Out of Scope

- automatic draining, frame/update ownership, polling loops, MonoBehaviours, coroutines, threads, tasks, or async behavior;
- clock advancement, pause policy, time scaling, rewind, reset, restore, platform clocks, wall-clock time, calendars, UTC, local time, or time zones;
- recurring tasks, intervals, cron-like rules, alarms, timers, retries, backoff, priorities, rescheduling, or callback replacement;
- domain-specific events, commands, messages, notifications, narrative actions, phone apps, UI behavior, or content;
- event-bus publication, subscriptions, logging, telemetry, analytics, or a generalized diagnostics service;
- persistence, save snapshots, stable cross-session task identifiers, serialization, restore, or offline catch-up;
- arbitrary lookup, enumeration, removal by ID, mutable queue exposure, or editing due timestamps;
- multithreaded safety or concurrent scheduling;
- modifying accepted clock, lifecycle, composition, or event-bus contracts;
- service locators, dependency-injection containers, singletons, mutable static state, scene search, hierarchy lookup, or reflection discovery;
- scenes, prefabs, ScriptableObjects, Resources assets, editor assets, packages, assembly definitions, or third-party dependencies.

## Dependencies and Prerequisites

- FND-001 through FND-005 are Complete and merged.
- The accepted FND-005 clock contracts and tests are present on `main`.
- The accepted FND-002 lifecycle and FND-004 composition contracts and tests are present on `main`.
- M0.3 Architecture Baseline is merged.
- No unresolved ADR is required for this bounded, in-memory, single-threaded queue.

## Authorized Files or Directories

- Assets/Minerva/Runtime/Core/**
- Assets/Minerva/Tests/Runtime/Editor/**
- this ticket file for lifecycle movement and reporting

Existing accepted Core implementation and test files may be inspected as listed but must not be modified. If an accepted contract must change to satisfy this ticket, stop and report.

## Prohibited Files or Systems

- existing accepted files under Assets/Minerva/Runtime/Core/**, including runtime lifecycle, composition, event-bus, and clock contracts or implementations;
- existing accepted test files under Assets/Minerva/Tests/Runtime/Editor/**;
- Assets/Minerva/Runtime/Application/**
- Assets/Minerva/Runtime/Simulation/**
- Assets/Minerva/Runtime/Phone/**
- Assets/Minerva/Runtime/Presentation/**
- Assets/Minerva/Runtime/Platform/**
- Assets/Minerva/Editor/**
- Assets/Minerva/Tests/Editor/**
- Assets/Minerva/Content/**
- Assets/Minerva/Resources/**
- ProjectSettings/**
- Packages/**
- Docs/Project/**
- Docs/Architecture/**
- Docs/Systems/**
- Docs/Tools/**
- Docs/ADR/**
- Planning/**
- third-party or vendor directories

New files may be added only inside the authorized runtime and runtime-test directories. Existing accepted files are read-only for this ticket.

## Permitted Actions

- Add the approved generic scheduling contracts, queue implementation, stable Unity metadata, and one focused test fixture.
- Use accepted clock, lifecycle, and composition contracts without modifying them.
- Add internal helper types nested in or private to the queue where they do not broaden public contracts.
- Move this ticket from `Ready` to `Active` when implementation begins.
- Move this ticket from `Active` to `Review` when the implementation pull request opens.
- Complete the Implementation Report and implementation-owned execution-state entries.
- Run the narrow searches and validation required by this ticket.

## Prohibited Actions

- Do not modify any accepted production or test file.
- Do not implement automatic draining, recurrence, persistence, domain behavior, event publication, or speculative future hooks.
- Do not change clock, lifecycle, composition, or event-bus contracts.
- Do not expose mutable queue internals, rescheduling, priority, global access, or arbitrary lookup.
- Do not add a service locator, DI container, singleton, mutable static state, scene discovery, or reflection discovery.
- Do not introduce third-party packages.
- Do not create scenes, prefabs, ScriptableObjects, Resources assets, editor assets, or assembly definitions.
- Do not change architecture, system, tool, ADR, epic, planning, or project-governance documents.
- Do not move this ticket to `Complete`; independent review and Technical Director acceptance are required.
- Do not merge the implementation pull request.

## Expected Files

Exact names may vary only when a clearer equivalent preserves the approved contracts:

- Assets/Minerva/Runtime/Core/IScheduledTaskScheduler.cs
- Assets/Minerva/Runtime/Core/IScheduledTaskScheduler.cs.meta
- Assets/Minerva/Runtime/Core/IScheduledTaskProcessor.cs
- Assets/Minerva/Runtime/Core/IScheduledTaskProcessor.cs.meta
- Assets/Minerva/Runtime/Core/IScheduledTaskHandle.cs
- Assets/Minerva/Runtime/Core/IScheduledTaskHandle.cs.meta
- Assets/Minerva/Runtime/Core/ScheduledTaskState.cs
- Assets/Minerva/Runtime/Core/ScheduledTaskState.cs.meta
- Assets/Minerva/Runtime/Core/ScheduledTaskDrainResult.cs
- Assets/Minerva/Runtime/Core/ScheduledTaskDrainResult.cs.meta
- Assets/Minerva/Runtime/Core/ScheduledTaskCallbackFailure.cs
- Assets/Minerva/Runtime/Core/ScheduledTaskCallbackFailure.cs.meta
- Assets/Minerva/Runtime/Core/ScheduledTaskQueue.cs
- Assets/Minerva/Runtime/Core/ScheduledTaskQueue.cs.meta
- Assets/Minerva/Tests/Runtime/Editor/ScheduledTaskQueueTests.cs
- Assets/Minerva/Tests/Runtime/Editor/ScheduledTaskQueueTests.cs.meta

## Documentation Updates

- Set `Status` to `Active`, set `Owner` to the implementation agent, and update `Updated` using `America/New_York` when moving from `Ready` to `Active`.
- Set `Status` to `Review` and update `Updated` when moving from `Active` to `Review`.
- Complete the Implementation Report before requesting independent review.
- Record execution states using `YYYY-MM-DD HH:mm z` in `America/New_York`.
- Do not update the epic or current-work planning documents during implementation.
- Do not move the ticket to `Complete`; the Technical Director performs acceptance closeout before merge.

## Architect's Notes

The captured eligibility ceiling is deliberate. A simple loop that repeatedly checks the queue against the same clock value would allow callbacks to schedule more same-time work indefinitely. Work created during a drain belongs to the next explicit drain, which gives the caller an observable control boundary.

The callback-attempt limit is still required even with the captured boundary. It lets callers budget processing across frames or simulation steps without weakening deterministic ordering. Failure attempts consume budget because the callback was invoked and may have performed partial external work before throwing.

Cancellation is a task-state transition, not physical interruption. Once callback execution begins, cancellation cannot safely undo arbitrary callback effects. This is why self-cancellation and cancellation of executing work return `false`.

Past timestamps are accepted because restore, catch-up, imported authored data, and explicitly advanced clocks may legitimately produce overdue work later. This foundation does not decide why work is overdue; it executes it deterministically when asked.

Do not use the event bus to announce task completion or failure. The queue has a direct synchronous caller and returns a complete drain result. Domain systems may later translate results into their own approved facts.

## Acceptance Criteria

- [ ] The constructor rejects a null `IRuntimeClock` with `ArgumentNullException` and no partially usable queue.
- [ ] The queue exposes separate narrow scheduling and processing capabilities plus readable cancellation handles.
- [ ] Past, current, and future `RuntimeInstant` values are accepted without normalization or immediate invocation.
- [ ] Null callbacks and post-shutdown scheduling are rejected before queue state changes.
- [ ] Due callbacks execute in ascending timestamp order and exact insertion order for equal timestamps.
- [ ] Insertion ordering is instance-local, monotonic, never reused after cancellation, and fails atomically on sequence overflow.
- [ ] Handles expose `Pending`, `Executing`, `Completed`, and `Cancelled` semantics exactly.
- [ ] Pending cancellation returns `true` once, later cancellation returns `false`, and the callback does not execute.
- [ ] Cancellation during execution or after completion returns `false` and cannot interrupt or alter completed state.
- [ ] A callback can cancel later pending eligible work; cancelled work is skipped without consuming callback-attempt budget.
- [ ] Each drain captures one clock cutoff and one insertion eligibility ceiling.
- [ ] Clock advancement during a callback does not expand the active due-time cutoff.
- [ ] Work scheduled during a drain is deferred to a later drain even when already due or equal-time.
- [ ] `DrainDue` rejects nonpositive limits without state changes.
- [ ] The callback-attempt limit counts successful and failed callback invocations but not cancelled skips.
- [ ] Limit-reached and remaining-due flags are true only when captured eligible pending work remains after the budget is exhausted.
- [ ] Empty drains and exact-budget exhaustion return unambiguous results.
- [ ] Callback exceptions are isolated, ordered diagnostics are returned, failed tasks become `Completed`, and later eligible work continues within budget.
- [ ] Diagnostics identify due time, insertion sequence, callback identity where available, exception type, and message without retaining delegates, targets, or exception objects.
- [ ] Reentrant draining of the same queue throws `InvalidOperationException`; when attempted by a callback it is captured as that callback's failure and the outer drain remains valid.
- [ ] Draining a different queue instance from a callback remains permitted.
- [ ] `Initialize` is idempotently successful before shutdown and state-neutral.
- [ ] `Shutdown` is idempotent and terminal, cancels pending work, releases callback references, and invokes no callback.
- [ ] Shutdown during a callback lets that callback finish, cancels other pending work, and stops the active drain deterministically.
- [ ] After shutdown, scheduling and draining throw `ObjectDisposedException`; handle reads remain available and cancellation returns `false`.
- [ ] Initialize after shutdown returns an actionable failed `ServiceInitializationResult` without revival.
- [ ] The queue consumes only `IRuntimeClock`, never advances or disposes the clock, and reads `CurrentTime` once per drain boundary.
- [ ] Clock then queue compose through `RuntimeCompositionRoot` without modifying accepted contracts, and reverse shutdown stops the queue before the clock.
- [ ] Repeated construction produces isolated deterministic queues with no static mutable state.
- [ ] No automatic, asynchronous, domain-specific, persistence, event-bus, scene, reflection, global-access, or speculative behavior is introduced.
- [ ] Focused Unity 5.6-compatible tests cover all approved behavior and preserve accepted tests unchanged.
- [ ] The final diff contains only authorized paths.
- [ ] The Implementation Report records changes and validation accurately.

## Required Validation

- List every created, modified, moved, and deleted path.
- Confirm no path outside Authorized Files or Directories changed.
- Run the complete existing runtime EditMode suite plus the new focused tests.
- Report test, assertion, failure, skipped, and inconclusive counts.
- Validate the exact `Assets/Minerva` tree through Unity 5.6 batch import/compile when available.
- Confirm Unity does not rewrite committed new `.meta` files or GUIDs during validation.
- Confirm every accepted production and test file is byte-for-byte unchanged.
- Search new runtime files for `UnityEditor`, `UnityEngine.Time`, `DateTime`, `DateTimeOffset`, `TimeSpan`, `Stopwatch`, coroutines, threads, tasks/async, scene search, hierarchy lookup, service locators, reflection discovery, mutable static state, and automatic processing; no prohibited implementation may exist.
- Search for recurrence, retry, priority, persistence, event publication, domain events, global scheduler access, and accepted-contract modification; no prohibited implementation may exist.
- Confirm no `.unity`, `.prefab`, `.asset`, `.asmdef`, package, vendor, editor-tool, or third-party file was added.
- Confirm all new text files end with a newline.
- Run `git diff --check`.
- Confirm the ticket exists in exactly one workflow directory and its `Status` matches.
- Record any validation unavailable in the environment without claiming it passed.

## Stop Conditions

Stop and report before implementation when:

- an accepted FND-002 through FND-005 production or test file must change;
- `IRuntimeClock` cannot support the approved captured-boundary behavior without modification;
- Unity 5.6 compatibility cannot be established for the required deterministic collection or result contracts;
- callback failure isolation would require a logging, event, or diagnostics framework outside this ticket;
- lifecycle ownership cannot be expressed through the accepted `IRuntimeService` and composition seam;
- an unauthorized file or system must change;
- a third-party dependency appears necessary;
- validation reveals a broader clock, lifecycle, composition, or repository defect;
- the acceptance criteria cannot be met within the authorized scope;
- required context is unavailable or contradictory.

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
| Planned | 2026-07-23 20:43 EDT | Technical Director | FND-006 readiness and promotion authorized after merged FND-005. |
| In Progress | 2026-07-23 20:43 EDT | Technical Director | Readiness pass defined queue contracts, lifecycle, drain boundaries, and validation scope. |
| Committed |  |  |  |
| Verified |  |  |  |
