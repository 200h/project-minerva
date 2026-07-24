# FND-005: Create Core Clock Abstractions

**Status:** Review
**Owner:** Codex
**Created:** 2026-07-23  
**Updated:** 2026-07-23  
**Roadmap Phase:** Phase 1 — Foundation  
**Epic:** [Foundation Runtime Services](../../../Planning/Epics/Foundation_Runtime_Services.md)  
**Related ADRs:** [Use Unity 5.6](../../ADR/0001_Use_Unity_5_6.md), [Ticket-Driven AI Implementation](../../ADR/0004_Ticket_Driven_AI_Implementation.md)

## Summary

Create the generic, domain-neutral clock contracts and deterministic manually advanced clock that later scheduling and simulation systems can build upon.

FND-005 defines elapsed runtime time only. It does not implement scheduling, wall-clock/calendar semantics, frame ownership, platform time, catch-up policy, or narrative behavior.

## Background

FND-004 accepted one explicit composition root that owns lifecycle services through the FND-002 RuntimeBootstrap contract. Services are registered explicitly in deterministic order, initialized once, and shut down in reverse order. The successful composed runtime currently exposes only narrow event capabilities.

The next foundation capability is an authoritative monotonic elapsed-time representation. FND-006 will use this clock to schedule deterministic future work. That scheduler requires exact, comparable, overflow-safe time values and a test clock whose advancement behavior is unambiguous.

The clock must fit the existing lifecycle and composition contracts without changing RuntimeCompositionRoot, ComposedRuntime, RuntimeBootstrap, or the event bus. First-class production clock construction and exposure may be added later through a separately reviewed composition ticket.

## Goal

Provide Unity 5.6-compatible plain-C# clock primitives that:

- represent elapsed runtime timestamps and durations exactly in integer milliseconds;
- never permit observable time to move backward;
- expose read-only clock state separately from mutation controls;
- define deterministic pause, resume, zero-advance, negative-input, and overflow behavior;
- provide a manually advanced clock suitable for tests and future deterministic simulation control;
- participate in existing runtime lifecycle ownership through IRuntimeService;
- remain independent of scheduling, domains, Unity frame time, platform clocks, and wall-clock concepts.

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
- Docs/Tickets/Complete/FND-002_Create_Runtime_Bootstrap.md
- Docs/Tickets/Complete/FND-004_Create_Runtime_Composition_Root.md

## Optional Context

Read only when required by a specific implementation question:

- Docs/Tickets/Complete/FND-003_Create_Core_Event_Bus.md
- Assets/Minerva/Runtime/Core/IEventPublisher.cs
- Assets/Minerva/Runtime/Core/IEventSubscriber.cs
- Assets/Minerva/Runtime/Core/InMemoryEventBus.cs
- Assets/Minerva/Tests/Runtime/Editor/InMemoryEventBusTests.cs
- Docs/Handbook/Git_Workflow.md
- Docs/Handbook/Branch_Strategy.md
- Planning/Epics/Foundation_Runtime_Services.md

Do not load unrelated system, tool, narrative, phone, save, UI, content, or planning documents.

## Code Areas to Inspect

Inspect only:

- Assets/Minerva/Runtime/Core/IRuntimeService.cs
- Assets/Minerva/Runtime/Core/ServiceInitializationResult.cs
- Assets/Minerva/Runtime/Core/RuntimeBootstrap.cs
- Assets/Minerva/Runtime/Core/RuntimeLifecycleState.cs
- Assets/Minerva/Runtime/Core/RuntimeCompositionRoot.cs
- Assets/Minerva/Runtime/Core/RuntimeCompositionResult.cs
- Assets/Minerva/Runtime/Core/ComposedRuntime.cs
- Assets/Minerva/Tests/Runtime/Editor/RuntimeBootstrapTests.cs
- Assets/Minerva/Tests/Runtime/Editor/RuntimeCompositionRootTests.cs
- this ticket file for lifecycle movement and reporting

Use narrow exact-symbol searches only when required to locate a direct accepted dependency. Do not perform repository-wide discovery.

## Approved Time Model

### Units and Representation

- One runtime time unit is exactly one millisecond.
- Timestamps and durations use signed 64-bit integer storage for Unity 5.6 compatibility.
- Public timestamp and duration contracts accept only values from zero through Int64.MaxValue inclusive.
- Floating-point seconds, DateTime, DateTimeOffset, TimeSpan, UnityEngine.Time, frame counts, local time, UTC, and time-zone data are not authoritative representations in this ticket.
- RuntimeInstant is an immutable elapsed-time value measured from the owning runtime session's zero point.
- RuntimeDuration is an immutable nonnegative elapsed interval.
- Both value types expose their millisecond value, equality, inequality, ordering, and XML-documented semantics without implicit conversion to wall-clock types.
- RuntimeInstant.Zero and RuntimeDuration.Zero, or equally clear equivalents, are permitted.

### Monotonicity and Advancement

- A newly constructed ManualRuntimeClock begins at RuntimeInstant.Zero unless an explicit valid initial RuntimeInstant is provided.
- Advance accepts a RuntimeDuration and is the only FND-005 operation that changes CurrentTime.
- When operational and not paused, advancing by a positive duration atomically adds the duration to CurrentTime.
- Advancing by RuntimeDuration.Zero is an idempotent no-op.
- Negative advancement is invalid. A negative duration must be rejected with ArgumentOutOfRangeException before clock state can change.
- RuntimeInstant and RuntimeDuration constructors must reject negative millisecond values with ArgumentOutOfRangeException.
- If CurrentTime plus the requested duration would exceed Int64.MaxValue, Advance throws OverflowException and leaves CurrentTime unchanged.
- No public API may set time backward, subtract elapsed time, reset an existing clock, wrap overflow, clamp silently, or accept negative deltas.
- Separate clock instances must not share mutable state.

### Pause and Resume

- IsPaused is observable through the read-only clock contract.
- Pause is idempotent. Repeated calls leave the clock paused without changing CurrentTime.
- Resume is idempotent. Repeated calls leave the clock running without changing CurrentTime.
- Advancing while paused is an intentional no-op: it does not accumulate hidden elapsed time, throw, or change CurrentTime.
- Zero advancement while paused remains a no-op.
- Pause and resume do not publish events in FND-005.
- This pause state represents only the foundational clock's progression gate. It does not define presentation pause, application backgrounding, platform suspension, or future catch-up policy.

### Lifecycle and Shutdown

- ManualRuntimeClock is a plain-C# IRuntimeService so the accepted FND-004 composition operation can own it explicitly.
- Initialize returns a successful ServiceInitializationResult and must not change CurrentTime or IsPaused.
- Repeated Initialize calls before shutdown return success and do not change state.
- ManualRuntimeClock is operational immediately after construction for focused deterministic tests; Initialize does not create a second semantic start point.
- Shutdown is idempotent and terminal.
- Shutdown must not reset CurrentTime or IsPaused.
- After shutdown, CurrentTime and IsPaused remain readable for diagnostics.
- After shutdown, Pause, Resume, and Advance throw ObjectDisposedException and do not change state.
- ManualRuntimeClock must not expose IDisposable if doing so would create a competing disposal path after ownership transfer. The accepted IRuntimeService shutdown path is authoritative.
- Initialize after shutdown returns a failed ServiceInitializationResult with an actionable reason, does not throw, and does not revive or change the clock.

### Manual Test-Clock Behavior

- ManualRuntimeClock performs no automatic advancement.
- It reads no system, platform, Unity, frame, or editor clock.
- Every observable timestamp change results from an explicit successful Advance call.
- It supports exact initial values, exact controlled advancement, pause/resume testing, boundary testing near Int64.MaxValue, shutdown testing, and repeated isolated construction.
- It allocates no per-frame work and requires no MonoBehaviour, coroutine, scene, prefab, ScriptableObject, or editor object.

## Composition Boundary

FND-005 must prove compatibility with the accepted FND-004 composition seam without modifying that seam:

- ManualRuntimeClock implements IRuntimeService and can be supplied explicitly to RuntimeCompositionRoot.Compose.
- A caller may retain only the narrow IRuntimeClock and, when authorized, IRuntimeClockControl capabilities while transferring lifecycle ownership to composition.
- Services that synchronously require time receive IRuntimeClock through explicit constructor or initialization dependencies; they do not discover it through a locator.
- Registration order remains explicit. A later service that depends on the clock must be registered after the clock.
- Runtime shutdown invokes clock shutdown through RuntimeBootstrap reverse-order lifecycle ownership.
- FND-005 does not make RuntimeCompositionRoot automatically create a clock.
- FND-005 does not add clock properties to ComposedRuntime, modify RuntimeCompositionResult, or expose arbitrary service lookup.
- A later ticket may add first-class production clock construction/exposure when a concrete consumer requires it.
- FND-006 may consume IRuntimeClock and IRuntimeClockControl, but scheduling behavior is not part of FND-005.

## Requirements

- Add an immutable RuntimeInstant value type under Minerva.Core using nonnegative Int64 milliseconds.
- Add an immutable RuntimeDuration value type under Minerva.Core using nonnegative Int64 milliseconds.
- Add a narrow IRuntimeClock read contract exposing CurrentTime and IsPaused.
- Add a separate narrow IRuntimeClockControl contract exposing Pause, Resume, and Advance.
- Add a sealed ManualRuntimeClock implementing IRuntimeClock, IRuntimeClockControl, and IRuntimeService.
- Implement the approved units, monotonicity, pause, advancement, overflow, lifecycle, and shutdown semantics exactly.
- Use explicit checked boundary handling that preserves clock state after a failed advance.
- Keep all public contracts XML documented.
- Use only Unity 5.6-compatible C# syntax and runtime libraries.
- Add focused Unity 5.6-compatible EditMode tests for values, advancement, pause/resume, boundaries, repeated initialization, post-shutdown initialization failure, composition compatibility, and isolation.
- Preserve all accepted FND-002 through FND-004 tests unchanged.

## Out of Scope

- FND-006 scheduled-task queue or any scheduling API;
- callbacks, alarms, timers, delayed commands, due-item processing, or equal-time ordering;
- events for time advancement, pause, resume, initialization, or shutdown;
- event-bus integration;
- automatic advancement from Unity Update, fixed update, coroutines, threads, async tasks, or platform callbacks;
- UnityEngine.Time, System.Diagnostics.Stopwatch, DateTime, DateTimeOffset, local time, UTC, time zones, calendars, weekdays, months, or daylight-saving policy;
- application backgrounding, suspension, offline progress, catch-up, or clock-source adapters;
- time scaling, rewind, reset, seek, restore, save/load, or snapshot contracts;
- narrative, phone, notification, app, UI, presentation, editor, or platform behavior;
- frame ownership, tick loops, polling services, or update managers;
- modifying RuntimeCompositionRoot, ComposedRuntime, RuntimeCompositionResult, RuntimeBootstrap, or accepted event contracts;
- service locator, dependency-injection container, static global clock, singleton, scene discovery, or reflection discovery;
- new scenes, prefabs, ScriptableObjects, Resources assets, packages, or assembly definitions;
- third-party dependencies.

## Dependencies and Prerequisites

- FND-001 through FND-004 are Complete and merged.
- The accepted FND-002 lifecycle and FND-004 composition contracts and tests are present on main.
- M0.3 Architecture Baseline is merged.
- No unresolved ADR is required for this bounded clock foundation.

## Authorized Files or Directories

- Assets/Minerva/Runtime/Core/**
- Assets/Minerva/Tests/Runtime/Editor/**
- this ticket file for lifecycle movement and reporting

Existing accepted Core implementation files may be inspected but must not be modified. If an accepted contract must change to satisfy this ticket, stop and report.

## Prohibited Files or Systems

- existing accepted files under Assets/Minerva/Runtime/Core/**, including RuntimeBootstrap, RuntimeCompositionRoot, ComposedRuntime, RuntimeCompositionResult, and event-bus contracts or implementations;
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

- Add the approved clock value types, narrow interfaces, manual implementation, stable Unity meta files, and focused tests.
- Use accepted lifecycle and composition contracts without modifying them.
- Add one new focused test fixture rather than editing accepted test fixtures.
- Move this ticket from Ready to Active when implementation begins.
- Move this ticket from Active to Review when the implementation pull request opens.
- Complete the Implementation Report and implementation-owned execution-state entries.
- Run the narrow searches and validation required by this ticket.

## Prohibited Actions

- Do not implement FND-006 scheduling or add speculative scheduling hooks.
- Do not modify any accepted production or test file.
- Do not add automatic clock advancement or a production platform clock.
- Do not publish clock events.
- Do not add time scaling, rewind, reset, restore, persistence, or catch-up behavior.
- Do not change the accepted composition root or expose a clock through ComposedRuntime.
- Do not add a service locator, global clock, singleton, mutable static state, scene search, hierarchy lookup, or reflection discovery.
- Do not introduce third-party packages.
- Do not create scenes, prefabs, ScriptableObjects, Resources assets, editor assets, or assembly definitions.
- Do not change architecture, system, tool, ADR, epic, planning, or project-governance documents.
- Do not move this ticket to Complete; independent review and Technical Director acceptance are required.
- Do not merge the implementation pull request.

## Expected Files

Exact names may vary only when a clearer equivalent is justified within the same approved contracts:

- Assets/Minerva/Runtime/Core/RuntimeInstant.cs
- Assets/Minerva/Runtime/Core/RuntimeInstant.cs.meta
- Assets/Minerva/Runtime/Core/RuntimeDuration.cs
- Assets/Minerva/Runtime/Core/RuntimeDuration.cs.meta
- Assets/Minerva/Runtime/Core/IRuntimeClock.cs
- Assets/Minerva/Runtime/Core/IRuntimeClock.cs.meta
- Assets/Minerva/Runtime/Core/IRuntimeClockControl.cs
- Assets/Minerva/Runtime/Core/IRuntimeClockControl.cs.meta
- Assets/Minerva/Runtime/Core/ManualRuntimeClock.cs
- Assets/Minerva/Runtime/Core/ManualRuntimeClock.cs.meta
- Assets/Minerva/Tests/Runtime/Editor/ManualRuntimeClockTests.cs
- Assets/Minerva/Tests/Runtime/Editor/ManualRuntimeClockTests.cs.meta

## Documentation Updates

- Set Status to Active, set Owner to the implementation agent, and update Updated using America/New_York when moving from Ready to Active.
- Set Status to Review and update Updated when moving from Active to Review.
- Complete the Implementation Report before requesting independent review.
- Record execution states using YYYY-MM-DD HH:mm z in America/New_York.
- Do not update the epic or current-work planning documents during implementation.
- Do not move the ticket to Complete; the Technical Director performs acceptance closeout before merge.

## Architect's Notes

Integer milliseconds are the authoritative unit because they are exact, human-readable, adequate for a phone-centered narrative simulation, straightforward to persist later, and compatible with Unity 5.6. FND-006 must not reinterpret the unit.

RuntimeInstant and RuntimeDuration are distinct concepts even though both store nonnegative Int64 milliseconds. Keeping them distinct prevents accidental use of an interval where an absolute elapsed timestamp is required.

Paused advancement is deliberately ignored instead of rejected or accumulated. This makes pause a deterministic progression gate. Future background catch-up or offline progress must be modeled as an explicit policy and explicit advancement, not hidden inside this clock.

The manual clock is the first concrete implementation because deterministic control is required before any platform adapter or frame driver is approved. It is a valid foundational runtime service, not a test-only assembly artifact.

Do not add event notifications merely to make state observable. Synchronous reads through IRuntimeClock are sufficient for this ticket. Later systems may publish completed facts after advancing time if their approved responsibilities require it.

## Acceptance Criteria

- [x] RuntimeInstant is immutable, nonnegative, uses Int64 milliseconds, exposes Zero, and supports deterministic equality and ordering.
- [x] RuntimeDuration is immutable, nonnegative, uses Int64 milliseconds, exposes Zero, and supports deterministic equality and ordering.
- [x] Negative timestamp and duration construction throws ArgumentOutOfRangeException.
- [x] IRuntimeClock exposes only CurrentTime and IsPaused.
- [x] IRuntimeClockControl exposes only Pause, Resume, and Advance.
- [x] A new ManualRuntimeClock begins at zero unless given an explicit valid initial instant.
- [x] Positive advancement while running increases CurrentTime exactly by the requested milliseconds.
- [x] Zero advancement is an idempotent no-op.
- [x] Pause and Resume are idempotent and never change CurrentTime.
- [x] Advancement while paused is ignored without hidden accumulation or exception.
- [x] An overflowing advance throws OverflowException and leaves CurrentTime unchanged.
- [x] No API permits time to move backward, reset, wrap, or silently clamp.
- [x] Initialize is idempotently successful before shutdown and never changes CurrentTime or IsPaused.
- [x] Initialize after shutdown returns an actionable failed ServiceInitializationResult without reviving or changing the clock.
- [x] Shutdown is idempotent and terminal without resetting readable state.
- [x] After shutdown, CurrentTime and IsPaused remain readable while Pause, Resume, and Advance throw ObjectDisposedException.
- [x] ManualRuntimeClock implements IRuntimeService and composes successfully through RuntimeCompositionRoot without changing accepted composition files.
- [x] Composition-owned shutdown makes the clock reject later mutation.
- [x] Separate clock and composed-runtime instances remain isolated with no shared mutable state.
- [x] No scheduling, event publication, Unity time, platform time, domain behavior, or automatic advancement is introduced.
- [x] Existing FND-002 through FND-004 tests pass unchanged.
- [x] Runtime implementation contains no UnityEditor reference.
- [x] Every new Unity asset has a stable meta file.
- [x] Only authorized new files and ticket lifecycle changes are present.
- [x] The implementation report accurately records design, validation, optional context, and deviations.
- [x] The ticket directory and Status metadata match throughout implementation.
- [x] The implementation pull request contains this ticket under Docs/Tickets/Review/ while independent review is pending.
- [ ] After acceptance, the implementation pull request contains this ticket under Docs/Tickets/Complete/ before merge.

## Required Validation

- List every created, modified, moved, and deleted path.
- Confirm every implementation change is a new file within authorized runtime or runtime-test paths, apart from this ticket's lifecycle movement.
- Confirm no existing accepted runtime or test file changed.
- Run the complete existing runtime EditMode test suite, not only the new clock tests.
- Report total tests, assertions, failures, skipped tests, and inconclusive tests when the runner provides them.
- Open/import the exact updated Assets/Minerva tree in Unity 5.6 batch mode and report compiler/import results when Unity 5.6 is available.
- Confirm Unity does not rewrite committed meta files or GUIDs during validation.
- Search runtime implementation for UnityEditor, UnityEngine.Time, DateTime, DateTimeOffset, Stopwatch, Thread, Task, async, coroutine, MonoBehaviour, scene search, hierarchy lookup, reflection discovery, global clock access, singleton patterns, and mutable static runtime state; no prohibited implementation may exist.
- Search for scheduling, callback queues, alarms, timers, delayed work, time-scale, rewind, reset, restore, persistence, and catch-up APIs; none may be introduced.
- Confirm RuntimeCompositionRoot, ComposedRuntime, RuntimeCompositionResult, RuntimeBootstrap, accepted event files, and their accepted tests remain byte-unchanged from main.
- Confirm no .unity, .prefab, .asset, .asmdef, package, vendor, or third-party file was added.
- Confirm all new text files end with a newline.
- Run git diff --check.
- Confirm the ticket exists in exactly one workflow directory and its Status matches that directory.
- Confirm the linked epic exists and identifies FND-005 as the current Ready ticket on main; do not edit planning files during implementation.

If Unity 5.6 or another required validation tool is unavailable, report that limitation accurately. Do not substitute a newer Unity version as proof of Unity 5.6 compatibility.

## Stop Conditions

Stop and report rather than guessing when:

- any accepted FND-002 through FND-004 production or test file must change;
- the approved integer-millisecond model cannot satisfy an acceptance criterion;
- composition compatibility requires exposing service lookup, modifying ComposedRuntime, or adding a competing ownership path;
- lifecycle behavior cannot be implemented through IRuntimeService within authorized new files;
- overflow-safe monotonic behavior cannot be proven without an unapproved library or language feature;
- Unity 5.6 compatibility requires a package, assembly definition, project setting, or unsupported syntax;
- required context conflicts or leaves a public contract materially ambiguous;
- existing accepted tests fail before implementation changes;
- the ticket cannot be completed within authorized paths.

## Definition of Done

- [x] Acceptance criteria satisfied.
- [x] Required validation reported accurately.
- [x] Documentation updated.
- [x] No unauthorized changes.
- [x] Implementation report completed.
- [x] Ticket exists only in Docs/Tickets/Review/ when implementation review begins.

## Implementation Report

### Status

Implementation and validation are complete. Draft pull request #14 is open
against `main` for independent review. The ticket moved from Ready to Active at
2026-07-23 20:03 EDT and from Active to Review at 2026-07-23 20:11 EDT.

### Changed Files

- Created `Assets/Minerva/Runtime/Core/RuntimeInstant.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeInstant.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/RuntimeDuration.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeDuration.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/IRuntimeClock.cs`.
- Created `Assets/Minerva/Runtime/Core/IRuntimeClock.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/IRuntimeClockControl.cs`.
- Created `Assets/Minerva/Runtime/Core/IRuntimeClockControl.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/ManualRuntimeClock.cs`.
- Created `Assets/Minerva/Runtime/Core/ManualRuntimeClock.cs.meta`.
- Created `Assets/Minerva/Tests/Runtime/Editor/ManualRuntimeClockTests.cs`.
- Created `Assets/Minerva/Tests/Runtime/Editor/ManualRuntimeClockTests.cs.meta`.
- Moved this ticket from `Docs/Tickets/Ready/` through
  `Docs/Tickets/Active/` to `Docs/Tickets/Review/`.

### Work Completed

- Added distinct immutable elapsed timestamp and duration value types using
  exact nonnegative signed 64-bit integer milliseconds.
- Added separate read-only and mutation-control clock contracts.
- Added a sealed, instance-owned, manually advanced clock that participates in
  the accepted runtime-service lifecycle without automatic advancement or
  external clock dependencies.
- Added one focused EditMode fixture covering value semantics, exact and paused
  advancement, overflow atomicity, lifecycle behavior, composition ownership,
  and instance isolation.

### Time Representation and Boundary Behavior

- `RuntimeInstant` and `RuntimeDuration` expose exact millisecond values, zero
  constants, equality, inequality, ordering, comparison, and stable hash
  behavior.
- Both value constructors reject negative milliseconds with
  `ArgumentOutOfRangeException`.
- Running advancement performs an explicit pre-addition boundary check.
  Overflow throws `OverflowException` before assignment, leaving current time
  unchanged.
- Zero advancement is an idempotent no-op. No subtraction, reset, rewind,
  clamping, wrapping, or wall-clock conversion API is exposed.

### Pause, Lifecycle, and Composition Behavior

- Pause and resume are idempotent and do not change current time.
- Advancement while paused is ignored without validation side effects or hidden
  accumulation; resuming restores only future explicit advancement.
- Initialization is repeatably successful and state-neutral before shutdown.
  Shutdown is idempotent and terminal while current time and pause state remain
  readable.
- Initialization after shutdown returns an actionable failed result. Pause,
  resume, and advance throw `ObjectDisposedException` after shutdown.
- `ManualRuntimeClock` implements `IRuntimeClock`, `IRuntimeClockControl`, and
  `IRuntimeService`. Passing it explicitly to `RuntimeCompositionRoot.Compose`
  proves compatibility and transfers lifecycle ownership without modifying or
  expanding the accepted composition seam.

### Validation

- Pre-implementation Unity 5.6 runtime EditMode baseline: passed 38 of 38 tests
  with 118 assertions, 0 failures, 0 skipped, and 0 inconclusive tests.
- Unity 5.6.7f1 batch import and compilation of an isolated project containing
  the exact updated `Assets/Minerva` tree: passed without compiler or import
  errors.
- Complete Unity 5.6 runtime EditMode suite: passed 53 of 53 tests with 182
  assertions, 0 failures, 0 skipped, and 0 inconclusive tests. The new focused
  fixture contributed 15 passing tests and 64 assertions.
- Unity metadata stability: passed for all committed `.meta` files and GUIDs.
  Unity generated only uncommitted directory metadata in the isolated project
  and did not rewrite any copied committed metadata.
- Accepted-file integrity: passed; the accepted FND-002 through FND-004
  production and test files are byte-unchanged from `main`.
- Prohibited implementation search: passed; no Unity/editor/platform clock,
  automatic advancement, scheduling, event integration, scene discovery,
  reflection discovery, global access, singleton, asynchronous, persistence,
  reset, restore, or catch-up implementation was introduced.
- Forbidden asset and dependency check: passed; no scene, prefab,
  ScriptableObject asset, assembly definition, package, vendor, or third-party
  file was added.
- Newline check: passed for every created text file and this ticket.
- `git diff --check`: passed.
- Workflow location check: passed; the ticket exists only under
  `Docs/Tickets/Review/` with matching Status.
- Authorized-path check: passed; all changes are limited to new approved Core
  runtime files, one new approved runtime test fixture, their metadata, and
  this ticket lifecycle/report update.
- Linked epic check: passed against `main`; it exists and identifies FND-005 as
  the sole Ready execution ticket. No planning file was changed.

### Deviations

None.

### Blockers or Risks

- Unity 5.6 emitted its previously observed legacy callback-unregistration and
  player-communicator assertions while exiting after saving the successful test
  result. It also reported an unavailable Unity public-CDN configuration
  request. The process exited successfully and import, compilation, and all
  tests passed.

### Optional Context Used

None.

### Follow-Up Suggestions

None.
## Implementation Review Agent Record

Completed by the independent reviewer while the ticket is in Review.

### Reviewer

### Reviewed PR and Head

### Scope and Acceptance Findings

### Time and Boundary Assessment

### Lifecycle and Composition Assessment

### Validation Assessment

### Blocking Findings

### Recommendation

Accept, Changes Required, or Blocked.

## Technical Director Acceptance

Completed after reviewing the Implementation Review Agent recommendation and before merge.

### Decision

Accepted, Changes Required, or Blocked.

### PR Reference

### Acceptance Date

Use YYYY-MM-DD in America/New_York.

### Final Validation Decision

### Accepted Deviations

### Follow-Up Tickets

## Execution State Log

Use YYYY-MM-DD HH:mm z in America/New_York.

| State | Timestamp | Actor | Evidence or Notes |
|---|---|---|---|
| Planned | 2026-07-23 17:58 EDT | Technical Director | Readiness pass completed against accepted FND-004 composition contracts and tests; promotion change prepared. |
| In Progress | 2026-07-23 20:03 EDT | Codex | Began implementation on `agent/fnd-005-core-clock-abstractions` from merged PR #13 (`642d84c`); ticket moved from Ready to Active. |
| Committed | 2026-07-23 20:10 EDT | Codex | Commit `eb73442006a3fb4df043cc3b344df67cff85713c` contains the validated implementation and Active ticket report. |
| Verified | 2026-07-23 20:11 EDT | Codex | Draft PR #14 opened against `main`; base is merged PR #13 (`642d84c`) and implementation head is `eb73442`. Ticket moved from Active to Review. |
