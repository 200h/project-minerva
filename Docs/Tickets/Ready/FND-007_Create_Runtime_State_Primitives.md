# FND-007: Create Runtime State Primitives

**Status:** Ready  
**Owner:** Unassigned  
**Created:** 2026-07-23  
**Updated:** 2026-07-23  
**Roadmap Phase:** Phase 1 — Foundation  
**Epic:** [Foundation Runtime Services](../../../Planning/Epics/Foundation_Runtime_Services.md)  
**Related ADRs:** [Use Unity 5.6](../../ADR/0001_Use_Unity_5_6.md), [Ticket-Driven AI Implementation](../../ADR/0004_Ticket_Driven_AI_Implementation.md)

## Summary

Create the generic, deterministic, single-threaded runtime-state primitives that later state-owning systems can use to expose typed reads, retain explicit mutation authority, validate proposed values, and announce completed changes.

FND-007 defines isolated typed state cells, stable owner-qualified identity, controlled mutation results, immutable completed-change records, and optional event publication only. It does not create a global state store, story variables, a state registry, persistence, snapshot capture, restore behavior, provenance policy, or domain schemas.

## Background

Project Minerva requires every mutable fact to have one authoritative owner. Consumers may read state or observe completed changes, but they must not gain unrestricted mutation access or maintain competing truth.

FND-003 accepted exact-type event publication and subscription through `IEventPublisher` and `IEventSubscriber`. FND-004 accepted explicit runtime composition and narrow event capabilities without service lookup. Those contracts permit an owner to construct a state primitive with an optional borrowed publisher while retaining a separate mutation capability.

The provisional FND-007 ticket left one central question unresolved: whether the foundation should introduce typed containers or a stable-key store. This readiness pass chooses typed state cells with explicit stable owner-qualified identity. A state cell is created deliberately, returns separate read and mutation capabilities, and contains one value of one compile-time type. There is no global registry, string/object property bag, runtime type lookup, or reflection discovery.

FND-008 will later define save-snapshot participation. FND-007 provides stable identity and typed values that owner-defined snapshot contributors may reference, but it does not enumerate state automatically or decide what is persistent.

## Goal

Provide Unity 5.6-compatible plain-C# state primitives that:

- identify each state cell with an immutable owner-qualified stable identity;
- expose separate strongly typed read and mutation capabilities;
- keep mutation authority explicit through capability passing;
- validate proposed values before mutation;
- distinguish applied, unchanged, and rejected mutation outcomes;
- apply equality and no-change behavior deterministically;
- produce one immutable completed-change record after every applied mutation;
- optionally publish that completed-change record through the accepted event contract;
- preserve an accepted mutation even when event publication reports or throws a failure;
- remain isolated across repeated construction with no global registry or static mutable state;
- define a clean boundary for later owner-defined snapshot contracts without implementing persistence.

## Required Context

Read only:

- Docs/Project/README.md
- Docs/Project/ARCHITECTURAL_PRINCIPLES.md
- Docs/Project/CODING_STANDARDS.md
- Docs/Project/AI_ENGINEERING_GUIDE.md
- Docs/Architecture/SYSTEM_OVERVIEW.md
- Docs/Architecture/RUNTIME_LAYERS.md
- Docs/Architecture/DEPENDENCY_RULES.md
- Docs/Architecture/STATE_OWNERSHIP.md
- Docs/Architecture/EVENT_ARCHITECTURE.md
- Docs/Architecture/DATA_AND_SERIALIZATION.md
- Docs/Architecture/EDITOR_RUNTIME_BOUNDARIES.md
- Docs/Context/Foundation.md
- Docs/Context/Runtime.md
- Docs/Systems/World_State_System.md
- Docs/Handbook/Definition_of_Done.md
- Docs/Handbook/Ticket_Workflow.md
- Docs/Tickets/Complete/FND-003_Create_Core_Event_Bus.md
- Docs/Tickets/Complete/FND-004_Create_Runtime_Composition_Root.md

## Optional Context

Read only when required by a specific implementation question:

- Docs/Tickets/Complete/FND-002_Create_Runtime_Bootstrap.md
- Docs/Tickets/Backlog/FND-008_Define_Save_Snapshot_Contracts.md
- Assets/Minerva/Tests/Runtime/Editor/InMemoryEventBusTests.cs
- Assets/Minerva/Tests/Runtime/Editor/RuntimeCompositionRootTests.cs
- Docs/Handbook/Git_Workflow.md
- Docs/Handbook/Branch_Strategy.md
- Planning/Epics/Foundation_Runtime_Services.md

Do not load unrelated time, scheduling, save implementation, narrative, phone, notification, application, UI, presentation, editor-tool, content, platform, or planning documents.

## Code Areas to Inspect

Inspect only:

- Assets/Minerva/Runtime/Core/IEvent.cs
- Assets/Minerva/Runtime/Core/IEventPublisher.cs
- Assets/Minerva/Runtime/Core/IEventSubscriber.cs
- Assets/Minerva/Runtime/Core/EventPublicationResult.cs
- Assets/Minerva/Runtime/Core/EventSubscriberFailure.cs
- Assets/Minerva/Runtime/Core/InMemoryEventBus.cs
- Assets/Minerva/Runtime/Core/ComposedRuntime.cs
- Assets/Minerva/Runtime/Core/RuntimeCompositionResult.cs
- Assets/Minerva/Runtime/Core/RuntimeCompositionRoot.cs
- Assets/Minerva/Tests/Runtime/Editor/InMemoryEventBusTests.cs
- Assets/Minerva/Tests/Runtime/Editor/RuntimeCompositionRootTests.cs
- this ticket file for lifecycle movement and reporting

Use narrow exact-symbol searches only when required to locate a direct accepted dependency. Do not perform repository-wide discovery.

## Approved State Model

### Typed Cells, Not a Key/Value Store

- The foundation unit is one sealed generic state cell containing one value of compile-time type `T`.
- Creation returns separate narrow capabilities:
  - a read capability exposing identity and current value;
  - a mutation capability accepting a proposed `T` and returning an immutable mutation result.
- The concrete mutable cell must not be exposed as a public general-purpose store.
- There is no global registry, dictionary of `string` to `object`, arbitrary lookup API, reflection-driven type discovery, or untyped mutation path.
- Capability passing is the authority boundary. The creating owner retains the mutation capability and gives consumers only the read capability when they must not mutate.
- The API must not claim security against malicious code that deliberately retains or redistributes a mutation capability. It establishes explicit architectural authority, not an access-control sandbox.

### Stable Identity and Ownership

- Each state cell has one immutable `RuntimeStateIdentity` or clearly equivalent value contract.
- Identity contains:
  - a nonempty stable owner identifier;
  - a nonempty stable state identifier local to that owner.
- Owner and state identifiers use ordinal, case-sensitive equality and deterministic hashing.
- Identifiers reject null, empty, or whitespace-only text.
- Identifiers are independent of display names, file positions, Unity instance IDs, object references, and runtime construction order.
- Identity values are immutable after construction and safe to include in completed-change records.
- Renaming or migration policy for identifiers is outside this ticket.

### Duplicate Identity Behavior

- FND-007 deliberately introduces no registry and no process-wide uniqueness check.
- Two independently constructed cells may use equal identities and remain completely isolated.
- Equal identity does not cause shared values, shared mutation, shared validation, or shared publication state.
- A later aggregate owner, state collection, or snapshot coordinator that accepts multiple identities must reject duplicates atomically within its own ownership boundary.
- FND-008 must not infer that equal identity makes independent cells the same object without an explicit owner contribution contract.

## Approved Public Contracts

The implementation must expose small XML-documented contracts equivalent to:

- `RuntimeStateIdentity`, an immutable owner-qualified stable identity;
- `IRuntimeState<T>`, a read capability exposing `Identity` and `Value`;
- `IRuntimeStateMutator<T>`, a mutation capability exposing `TrySet(T proposedValue)`;
- `IRuntimeStateValidator<T>`, a validator receiving identity, current value, and proposed value and returning an immutable validation result;
- `RuntimeStateValidationResult`, representing accepted or rejected validation with an actionable rejection reason;
- `RuntimeStateMutationStatus` with exactly the semantic outcomes `Changed`, `Unchanged`, and `Rejected`;
- `RuntimeStateChange<T>`, an immutable completed-change record implementing `IEvent`;
- `RuntimeStateMutationResult<T>`, an immutable mutation outcome;
- one creation/factory boundary that returns the paired read and mutation capabilities without exposing mutable internal collections or a registry.

Exact type names may vary only when a clearer equivalent preserves these boundaries.

The API must not expose:

- an untyped `object` value API;
- arbitrary lookup by identifier;
- enumeration of every state cell;
- external replacement of identity, validator, comparer, or publisher;
- direct setter access;
- collection mutation;
- reset, restore, or bypass-validation mutation;
- subscriptions owned by the state cell.

## Approved Construction Semantics

- Creation requires a non-null identity contract.
- Creation accepts:
  - an initial value of type `T`;
  - an optional `IEqualityComparer<T>`;
  - an optional `IRuntimeStateValidator<T>`;
  - an optional `IEventPublisher`.
- A null comparer selects `EqualityComparer<T>.Default`.
- A null validator means every proposed value is valid.
- A null event publisher disables completed-change publication for that cell.
- The comparer, validator, and publisher choices are fixed for the cell lifetime.
- The initial value is validated when a validator is supplied.
- Rejected initial state throws `ArgumentException` with the validator's actionable reason and creates no usable capabilities.
- Exceptions thrown by the validator during initial validation propagate and create no usable capabilities.
- Structural argument failures occur before any usable cell or capability is returned.
- Creation performs no publication. Initial state is established state, not a runtime change.
- The state cell does not clone values. Values supplied to it must be treated as immutable by contract; mutable collections and mutable object graphs are outside this ticket.

## Approved Validation and Mutation Semantics

### Validation

- `TrySet` first rejects use of a structurally invalid mutation capability according to normal argument/lifetime rules; otherwise it evaluates equality before invoking the validator.
- When the proposed value equals the current value under the selected comparer:
  - return `Unchanged`;
  - do not invoke the validator;
  - do not replace the stored value;
  - do not create a change record;
  - do not publish an event.
- When values differ and a validator exists, call it exactly once with identity, current value, and proposed value.
- A validator rejection:
  - returns `Rejected`;
  - retains the current value;
  - includes the actionable rejection reason;
  - creates no change record;
  - publishes no event.
- A validator rejection result requires a nonempty reason.
- If a validator throws, the exception propagates and state remains unchanged.
- Validation must not receive or mutate internal state-cell implementation objects.

### Null and Default Values

- `default(T)` and `null` for reference or nullable types are valid state values unless the supplied validator rejects them.
- The foundation must not add implicit null prohibition, default-value substitution, normalization, coercion, or type conversion.
- Equality for null and default values follows the selected comparer.
- Change records may contain null previous or current values when valid for `T`.

### Applied Mutation

- After validation succeeds, replace the current value exactly once.
- Construct one immutable `RuntimeStateChange<T>` containing:
  - the stable identity;
  - the previous value;
  - the current value.
- Return `Changed` with that change record.
- The change record represents a completed fact. Subscribers cannot veto or roll back the mutation.
- Each successful mutation produces a distinct change-record instance.
- No sequence number, simulation timestamp, cause, correlation identifier, or provenance field is added by this generic ticket. Later owner systems may wrap or supplement the record when their domain requires it.

## Approved Event Publication Semantics

- Event integration is optional per cell and fixed at construction.
- When no publisher is supplied, a changed mutation succeeds with no publication attempt.
- When a publisher is supplied, publish the completed `RuntimeStateChange<T>` only after the new value is authoritative.
- `Unchanged` and `Rejected` outcomes never publish.
- Use the accepted exact generic event type; do not publish through an untyped or base-event path.
- Include the returned `EventPublicationResult` in the mutation result when publication returns normally.
- A nested publication result may be incomplete until the accepted event bus drains its outer queue. FND-007 must not block, poll, republish, or reinterpret that result.
- Subscriber failures reported by `EventPublicationResult` do not roll back state.
- If `IEventPublisher.Publish` throws:
  - retain the already-applied new value;
  - do not retry or roll back;
  - return `Changed`;
  - include an immutable publication-failure diagnostic containing exception type and message;
  - do not retain the exception object.
- Mutation results distinguish:
  - publication not configured;
  - publication returned an `EventPublicationResult`;
  - publication threw and produced a failure diagnostic.
- The state primitive does not own or dispose the supplied publisher and creates no event subscriptions.
- It does not publish validation failures, unchanged attempts, construction, reads, or lifecycle events.

## Approved Mutation Result and Change Contracts

- `RuntimeStateMutationResult<T>` is immutable after construction.
- For `Changed`:
  - `Change` is non-null;
  - rejection reason is null;
  - publication result or publication-failure diagnostic reflects the optional publication attempt.
- For `Unchanged`:
  - `Change` is null;
  - rejection reason is null;
  - no publication result or publication-failure diagnostic exists.
- For `Rejected`:
  - `Change` is null;
  - rejection reason is nonempty;
  - no publication result or publication-failure diagnostic exists.
- Invalid combinations must not be publicly constructible.
- `RuntimeStateChange<T>` is immutable by API. It retains the supplied values without cloning, so callers must use values that are themselves treated as immutable.
- Results and changes expose no mutable internal collections, delegates, validator references, comparer references, or state-cell implementation references.

## Lifecycle, Composition, and Threading

- These are value and capability primitives, not independently lifecycle-owned runtime services.
- FND-007 adds no `IRuntimeService`, `IDisposable`, initialization, shutdown, or global owner service.
- State remains readable and mutable as long as callers retain its capabilities. Lifecycle restrictions belong to the future authoritative service that owns those capabilities.
- The optional publisher is borrowed and never disposed by the state primitive.
- Composition compatibility is proven without modifying accepted composition contracts:
  - compose a runtime through `RuntimeCompositionRoot.Compose()`;
  - construct a state cell with `ComposedRuntime.EventPublisher`;
  - observe the completed change through `ComposedRuntime.EventSubscriber`;
  - dispose the composed runtime through its accepted lifetime.
- No state capability is automatically exposed through `ComposedRuntime`.
- No post-composition service registration, service lookup, composition-root modification, or global state access is introduced.
- The contracts are single-threaded and not thread-safe.
- Concurrent reads or mutations are unsupported and need not be synchronized.
- Reentrant mutation from a completed-change subscriber is permitted and follows the accepted event bus's queued nested-publication behavior. Each `TrySet` completes its own validation and value replacement synchronously before publication.
- Separate state cells share no mutable state, even when they have equal identities, equal values, equal validators, or the same borrowed publisher.

## FND-008 Snapshot Boundary

- FND-007 does not implement snapshot capture, restore, serialization, schema versions, migration, contribution enumeration, or save ownership.
- Stable state identity is available for later owner-defined snapshot contracts, but it is not itself a serialized schema.
- FND-008 must preserve the rule that authoritative owner services decide which state participates in snapshots.
- FND-008 must not discover cells through reflection, global registries, or static enumeration.
- Restore must not be implemented through a hidden raw setter added by FND-007.
- If later restore needs a controlled mutation path distinct from normal gameplay mutation, FND-008 or an owner-specific ticket must define and validate that authority explicitly.
- FND-007 adds no serialization attributes and makes no file-format commitment.

## Requirements

- Add the approved immutable owner-qualified identity contract under `Minerva.Core`.
- Add separate generic read and mutation capability interfaces.
- Add the generic validator and immutable validation-result contracts.
- Add the exact semantic mutation-status outcomes.
- Add immutable generic completed-change and mutation-result contracts.
- Add an immutable publication-failure diagnostic when publication throws.
- Add one sealed internal or otherwise non-leaking state-cell implementation and a small public creation boundary that returns the paired capabilities.
- Implement approved equality, validation, null/default, mutation, publication, reentrancy, identity, and isolation behavior exactly.
- Reject invalid structural arguments before returning usable capabilities.
- Keep all public contracts XML documented.
- Use only Unity 5.6-compatible C# syntax and runtime libraries.
- Add one focused Unity 5.6-compatible EditMode fixture covering the complete approved behavior.
- Preserve all accepted FND-002 through FND-006 production files and tests unchanged.

## Out of Scope

- a global world-state service, owner registry, key/value property bag, arbitrary lookup, enumeration, query language, or reflection discovery;
- story variables, facts, flags, branch history, relationship values, player profile data, phone/app state, UI state, or domain schemas;
- commands, request/response over events, domain-specific validation rules, provenance, cause, correlation, timestamps, sequence numbers, audit history, undo, redo, or replay;
- mutable collection support, deep cloning, copy-on-write, structural snapshots, or defensive copying of arbitrary `T`;
- snapshot capture, restore, persistence, save/load, serialization, schema versions, migration, file formats, disk I/O, slots, autosave, compression, encryption, or platform paths;
- event sourcing or treating the event bus as persistence;
- lifecycle owner services, automatic composition, service lookup, post-start service registration, or changes to accepted composition contracts;
- asynchronous or multithreaded mutation, locks, tasks, threads, jobs, or thread-safe collections;
- editor tooling, Story Timeline, Game Director, runtime inspection UI, scenes, prefabs, ScriptableObjects, Resources assets, packages, assembly definitions, or third-party dependencies.

## Dependencies and Prerequisites

- FND-001 through FND-006 are Complete and merged.
- The accepted FND-003 event contracts and tests are present on `main`.
- The accepted FND-004 composition contracts and tests are present on `main`.
- M0.3 Architecture Baseline is merged.
- No unresolved ADR is required for these bounded, generic, in-memory, single-threaded primitives.

## Authorized Files or Directories

- Assets/Minerva/Runtime/Core/**
- Assets/Minerva/Tests/Runtime/Editor/**
- this ticket file for lifecycle movement and reporting

Existing accepted Core implementation and test files may be inspected as listed but must not be modified. If an accepted contract must change to satisfy this ticket, stop and report.

## Prohibited Files or Systems

- existing accepted files under Assets/Minerva/Runtime/Core/**
- existing accepted files under Assets/Minerva/Tests/Runtime/Editor/**
- Assets/Minerva/Editor/**
- Assets/Minerva/Runtime/Application/**
- Assets/Minerva/Runtime/Simulation/**
- Assets/Minerva/Runtime/Phone/**
- Assets/Minerva/Runtime/Presentation/**
- Assets/Minerva/Runtime/Platform/**
- Assets/Minerva/Content/**
- Assets/Minerva/Resources/**
- Assets/Minerva/Tests/Editor/**
- Docs/Architecture/**
- Docs/Systems/**
- Docs/Tools/**
- Docs/Project/**
- Docs/Handbook/**
- Docs/Templates/**
- Planning/**
- Packages/**
- ProjectSettings/**

The authorized Core and runtime-test directories permit only new FND-007 files and metadata. Existing accepted files in those directories are read-only.

## Permitted Actions

- Create the approved generic runtime-state contracts and implementation under `Assets/Minerva/Runtime/Core/`.
- Create stable Unity `.meta` files for each new runtime asset.
- Create one focused EditMode fixture under `Assets/Minerva/Tests/Runtime/Editor/`.
- Create stable Unity `.meta` files for the focused test asset.
- Move this ticket from `Ready` to `Active` when implementation begins.
- Move this ticket from `Active` to `Review` when the implementation pull request opens.
- Complete the Implementation Report before requesting review.
- Run focused and complete runtime EditMode validation.

## Prohibited Actions

- Do not modify any accepted production or test file.
- Do not implement FND-008 or any persistence, snapshot, restore, or serialization behavior.
- Do not introduce a global state store, registry, service locator, singleton, mutable static state, scene discovery, hierarchy lookup, or reflection discovery.
- Do not add domain-specific state types or event catalogs.
- Do not add lifecycle services or modify runtime composition.
- Do not move this ticket to `Complete`; independent review and Technical Director acceptance are required.
- Do not update the epic, planning, governance, architecture, system, template, or context documents during implementation.
- Do not create packages, assembly definitions, scenes, prefabs, ScriptableObjects, Resources assets, or third-party dependencies.
- Do not merge the implementation pull request.

## Expected Files

Expected new production files under `Assets/Minerva/Runtime/Core/`, with stable matching `.meta` files:

- `RuntimeStateIdentity.cs`
- `IRuntimeState.cs` or a clearer generic read-capability equivalent
- `IRuntimeStateMutator.cs` or a clearer generic mutation-capability equivalent
- `IRuntimeStateValidator.cs`
- `RuntimeStateValidationResult.cs`
- `RuntimeStateMutationStatus.cs`
- `RuntimeStateChange.cs`
- `RuntimeStateMutationResult.cs`
- `RuntimeStatePublicationFailure.cs`
- one small factory/binding file and one sealed non-leaking implementation file, if separate files are required

Expected new test files:

- `Assets/Minerva/Tests/Runtime/Editor/RuntimeStatePrimitivesTests.cs`
- `Assets/Minerva/Tests/Runtime/Editor/RuntimeStatePrimitivesTests.cs.meta`

Exact production filenames may vary only when the implementation report maps each approved responsibility to a clearer equivalent. Do not create unrelated helpers.

Expected ticket movement:

- `Docs/Tickets/Ready/FND-007_Create_Runtime_State_Primitives.md`
- then `Docs/Tickets/Active/FND-007_Create_Runtime_State_Primitives.md`
- then `Docs/Tickets/Review/FND-007_Create_Runtime_State_Primitives.md`

Only one copy may exist at a time.

## Documentation Updates

- Set `Status` to `Active`, record the implementation owner, and update the date when moving from `Ready` to `Active`.
- Set `Status` to `Review` and update the date when moving from `Active` to `Review`.
- Complete the Implementation Report before requesting review.
- Record every created, modified, moved, and deleted path.
- Record exact tests, assertions, failures, skipped, and inconclusive counts.
- Record optional context used; write `None` when none was required.
- Record unavailable validation honestly without claiming it passed.
- Use `America/New_York` for date and execution-state metadata.
- Do not update planning or governance documents during implementation.

## Architect's Notes

The central architectural decision is typed ownership rather than a general stable-key store. Stable identity exists so owner-defined systems and later snapshots can refer to a fact consistently, but identity does not create a global registry or grant lookup authority.

Mutation authority is explicit through a separate capability. The primitive cannot prevent a caller from redistributing that capability, so code review and composition remain responsible for giving it only to the authoritative owner.

The completed-change record is deliberately small. Timestamps, causes, correlation identifiers, provenance, history, and domain constraints belong to later owning systems. Adding them here would prematurely force all state domains into one policy.

Event publication is optional because these primitives own no runtime lifetime and the accepted composition root exposes event capabilities only after successful composition. A state owner may construct a cell with the composed runtime's publisher, or omit publication in isolated contexts. The choice cannot be changed later.

State mutation is authoritative before publication. Event failures are diagnostics, not rollback instructions. This preserves the event architecture's completed-fact rule.

## Acceptance Criteria

- [ ] FND-007 exists only under `Docs/Tickets/Review/` with `Status: Review` while independent review is pending.
- [ ] All added production code is plain C# under `Minerva.Core` and is compatible with Unity 5.6.
- [ ] State is represented by isolated generic typed cells, not a global or untyped key/value store.
- [ ] Stable identity is immutable, owner-qualified, ordinal, case-sensitive, and independent of runtime object identity.
- [ ] Read and mutation capabilities are separate and the mutable implementation is not exposed as a general store.
- [ ] Equal identities on separately constructed cells remain isolated with no hidden uniqueness registry.
- [ ] Null/default values are accepted unless rejected by an explicit validator.
- [ ] Equality is deterministic through a fixed comparer, with `EqualityComparer<T>.Default` as the default.
- [ ] Equal proposed values return `Unchanged`, skip validation, retain the original stored value, and publish nothing.
- [ ] Differing proposed values invoke the validator exactly once when configured.
- [ ] Rejected validation returns an actionable immutable result, preserves state, creates no change, and publishes nothing.
- [ ] Validator exceptions propagate without state mutation.
- [ ] Applied mutations update state exactly once and return one immutable completed-change record with identity, previous value, and current value.
- [ ] Event publication is optional, fixed at construction, and occurs only after an applied change.
- [ ] Subscriber failures never roll back state and remain available through the accepted publication result.
- [ ] Publisher exceptions do not roll back state and are represented without retaining exception objects.
- [ ] Reentrant mutation through a subscriber is deterministic under the accepted queued nested-publication behavior.
- [ ] The primitives own no publisher, subscription, lifecycle service, global registry, or composition-root modification.
- [ ] The FND-008 boundary is preserved: no snapshot, restore, persistence, serialization, enumeration, or raw restore setter is added.
- [ ] Separate cells and repeated construction share no mutable state.
- [ ] No accepted FND-002 through FND-006 production or test file is modified.
- [ ] Focused tests cover identity, construction, capability separation, equality, validation, null/default values, mutation outcomes, publication, publication failures, reentrancy, duplicates, composition compatibility, and instance isolation.
- [ ] The complete existing runtime EditMode suite plus the new focused fixture passes in Unity 5.6 when available.
- [ ] No editor-only reference, forbidden asset type, package, assembly definition, third-party dependency, service locator, global mutable state, scene discovery, or reflection discovery is introduced.
- [ ] All new Unity assets have committed stable `.meta` files.
- [ ] All new text files end with a newline and `git diff --check` passes.
- [ ] The implementation pull request contains this ticket under `Docs/Tickets/Review/` while independent review is pending.
- [ ] After acceptance, the implementation pull request contains this ticket under `Docs/Tickets/Complete/` before merge.

## Required Validation

- Run the complete existing runtime EditMode suite plus the new focused FND-007 fixture.
- Report total tests, assertions, failures, skipped tests, and inconclusive tests.
- Validate the exact updated `Assets/Minerva` tree in Unity 5.6 when available.
- Confirm Unity imports and compiles every new runtime and test file.
- Confirm Unity does not rewrite committed new `.meta` files and every new GUID is unique.
- Confirm no existing accepted production or test file changed from the implementation merge base.
- Test at minimum:
  - identity argument validation, equality, hashing, and case sensitivity;
  - equal identities on isolated cells;
  - read and mutation capability behavior;
  - default comparer and custom comparer behavior;
  - initial-value validation;
  - null/default acceptance and validator rejection;
  - unchanged mutation skipping validation and publication;
  - accepted mutation and immutable change contents;
  - rejected mutation and actionable reason;
  - validator exception atomicity;
  - optional publication disabled;
  - successful completed-change publication;
  - event subscriber failure without rollback;
  - publisher exception diagnostic without rollback;
  - nested/reentrant mutation ordering;
  - composition compatibility through accepted event capabilities;
  - repeated isolated construction and absence of shared mutable state.
- Audit new runtime files for:
  - `UnityEditor` or editor-only references;
  - untyped `object` state APIs;
  - global registries, service locators, singletons, and mutable static state;
  - reflection, scene, hierarchy, Resources, or automatic discovery;
  - thread, task, async, lock, or concurrent-collection APIs;
  - snapshot, serialization, persistence, restore, or file-I/O behavior;
  - domain-specific state or event types.
- Confirm no scene, prefab, ScriptableObject asset, Resources asset, package, assembly definition, vendor directory, or third-party dependency was added.
- Confirm every changed path is authorized.
- Confirm all new text files end with a newline.
- Run `git diff --check`.
- Confirm the ticket exists in exactly one workflow directory and its `Status` matches.
- Record any validation unavailable in the environment without claiming it passed.

## Stop Conditions

Stop and report before implementation when:

- an accepted FND-002 through FND-006 production or test file must change;
- the separate read/mutation capability boundary cannot be implemented without exposing the mutable cell publicly;
- the accepted exact-type event contracts cannot publish the generic completed-change record without modification;
- publisher-failure behavior cannot be represented without rollback or retaining exception objects;
- Unity 5.6 cannot support the required generic contracts, comparer behavior, or immutable results;
- lifecycle ownership appears necessary inside the primitive rather than in a later authoritative owner service;
- FND-008 snapshot or restore behavior must be implemented to satisfy this ticket;
- an unauthorized file or system must change;
- a third-party dependency appears necessary;
- validation reveals a broader event, composition, repository, or architecture defect;
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
| Planned | 2026-07-23 21:31 EDT | Technical Director | FND-007 readiness and promotion authorized after merged FND-006. |
| In Progress | 2026-07-23 21:31 EDT | Technical Director | Readiness pass resolved typed ownership, stable identity, mutation, event, composition, and snapshot boundaries. |
| Active |  |  |  |
| Committed |  |  |  |
| Verified |  |  |  |
| Reviewed |  |  |  |
| Accepted |  |  |  |
