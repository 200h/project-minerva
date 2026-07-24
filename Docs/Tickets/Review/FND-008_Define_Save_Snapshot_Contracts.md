# FND-008: Define Save Snapshot Contracts

**Status:** Review
**Owner:** Codex
**Created:** 2026-07-23  
**Updated:** 2026-07-24  
**Roadmap Phase:** Phase 1 — Foundation  
**Epic:** [Foundation Runtime Services](../../../Planning/Epics/Foundation_Runtime_Services.md)  
**Related ADRs:** [Use Unity 5.6](../../ADR/0001_Use_Unity_5_6.md), [Ticket-Driven AI Implementation](../../ADR/0004_Ticket_Driven_AI_Implementation.md)

## Summary

Define the generic, format-agnostic runtime snapshot contracts and deterministic in-memory coordination required for later save/load work.

FND-008 establishes stable contribution identity, explicit per-contribution schema versions, immutable detached snapshot data, deterministic capture and restore order, complete pre-mutation validation, transactional restore operations, rollback diagnostics, and explicit failure results. It does not choose a serialization format, read or write files, implement save slots or autosave policy, migrate schemas, or define narrative, phone, app, or other domain snapshot payloads.

## Background

Project Minerva requires every mutable fact to remain owned by one authoritative runtime system. The Save/Load System may coordinate capture and reconstruction, but it must not become the business owner of captured values.

FND-007 accepted isolated typed runtime-state primitives with stable owner-qualified identity, explicit mutation authority, validation, and immutable completed-change records. It deliberately deferred snapshot participation to owner-defined contracts. FND-008 now defines that boundary without enumerating runtime state automatically or adding a global state registry.

The architecture requires versioned per-system state, deterministic ordering, actionable compatibility failures, and protection against partial restoration. A restore therefore cannot mutate live state while structural or participant validation is incomplete. Contributors prepare rollback-capable restore operations first; only after every contribution is accepted may the coordinator apply them. If application fails, attempted operations are rolled back in reverse order and all failures are reported.

## Goal

Provide Unity 5.6-compatible plain-C# snapshot foundations that:

- identify every contribution through an immutable owner-qualified stable identity;
- expose an explicit positive schema version for every contribution;
- carry owner-defined immutable snapshot data without selecting a disk or wire format;
- require contributors to capture detached data rather than service, delegate, scene, or Unity-object references;
- register contributors explicitly and reject duplicate contribution identities atomically;
- capture contributions in registration order;
- validate a complete snapshot before any authoritative state mutation;
- reject missing, unknown, duplicate, malformed, or incompatible contributions with actionable diagnostics;
- prepare restore operations in registration order without mutating authoritative state;
- apply prepared operations in registration order;
- roll back attempted operations in reverse order after an apply failure;
- preserve both the primary restore failure and any rollback or release failures;
- isolate expected participant failures and thrown exceptions into immutable diagnostics;
- remain independent of storage, serialization, migration, domain schemas, and global service lookup;
- provide a clean contract for a later Save/Load System to serialize and store validated snapshots.

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
- Docs/Architecture/DATA_AND_SERIALIZATION.md
- Docs/Architecture/INITIALIZATION_AND_LIFECYCLE.md
- Docs/Architecture/EDITOR_RUNTIME_BOUNDARIES.md
- Docs/Context/Foundation.md
- Docs/Context/Runtime.md
- Docs/Context/Persistence.md
- Docs/Systems/Save_Load_System.md
- Docs/Handbook/Definition_of_Done.md
- Docs/Handbook/Ticket_Workflow.md
- Docs/Process/UNITY_EXECUTION_MODEL.md
- Docs/Runbooks/UNITY_5_6_VALIDATION.md
- Docs/Tickets/Complete/FND-007_Create_Runtime_State_Primitives.md

## Optional Context

Read only when required by a specific implementation question:

- Docs/Tickets/Complete/FND-002_Create_Runtime_Bootstrap.md
- Docs/Tickets/Complete/FND-004_Create_Runtime_Composition_Root.md
- Assets/Minerva/Tests/Runtime/Editor/RuntimeStatePrimitivesTests.cs
- Assets/Minerva/Tests/Runtime/Editor/RuntimeCompositionRootTests.cs
- Planning/Epics/Foundation_Runtime_Services.md
- Docs/Handbook/Git_Workflow.md
- Docs/Handbook/Branch_Strategy.md

Do not load unrelated time, scheduling, narrative, phone, notification, application, UI, presentation, editor-tool, content, or platform documents.

## Code Areas to Inspect

Inspect only:

- Assets/Minerva/Runtime/Core/IRuntimeService.cs
- Assets/Minerva/Runtime/Core/ServiceInitializationResult.cs
- Assets/Minerva/Runtime/Core/RuntimeStateIdentity.cs
- Assets/Minerva/Runtime/Core/IRuntimeState.cs
- Assets/Minerva/Runtime/Core/IRuntimeStateMutator.cs
- Assets/Minerva/Runtime/Core/RuntimeState.cs
- Assets/Minerva/Runtime/Core/RuntimeStateCapabilities.cs
- Assets/Minerva/Runtime/Core/RuntimeStateMutationResult.cs
- Assets/Minerva/Tests/Runtime/Editor/RuntimeStatePrimitivesTests.cs
- Assets/Minerva/Runtime/Core/RuntimeCompositionRoot.cs
- Assets/Minerva/Runtime/Core/RuntimeCompositionResult.cs
- Assets/Minerva/Runtime/Core/ComposedRuntime.cs
- Assets/Minerva/Tests/Runtime/Editor/RuntimeCompositionRootTests.cs
- this ticket file for lifecycle movement and reporting

Use narrow exact-symbol searches only when required to locate a direct accepted dependency. Do not perform repository-wide discovery.

## Execution Environment

**Implementation Environment:** Codex cloud or local workspace  
**Unity Execution Environment:** Repository Owner local Mac  
**Unity Version:** 5.6.7f1  
**Unity Executable:** `/Applications/Unity/Unity.app/Contents/MacOS/Unity`  
**Validation Responsibility:** Local Unity Verification Operator using the accepted verification tooling

Implementation capability declaration:

```text
Repository editing: Implementation agent must report before editing
C# compiler: Implementation agent must report before editing
Unity 5.6.7f1: Expected unavailable in cloud; report actual capability
Unity tests: Local verification is mandatory even when other checks are available
```

The implementation handoff must identify the exact PR, branch, and head SHA and must explicitly state whether Unity validation remains pending.

## Approved Snapshot Model

### Stable Contribution Identity

- Each contributor has one immutable `RuntimeSnapshotContributionIdentity` or clearly equivalent value contract.
- Identity contains:
  - a nonempty stable owner identifier;
  - a nonempty stable contribution identifier local to that owner.
- Identifiers use ordinal, case-sensitive equality and deterministic hashing.
- Null, empty, or whitespace-only identifiers are rejected.
- Identity is independent of display names, registration order, file position, Unity instance IDs, object references, and runtime construction order.
- The identity must remain stable after a save has been produced.
- Identifier renaming and migration policy are outside this ticket.

### Explicit Schema Version

- Every contribution has one positive signed 32-bit integer schema version.
- Zero and negative versions are rejected.
- A contributor exposes its current capture schema version.
- Restore compatibility is decided by the owning contributor during preparation.
- Equality with the current version must be supported.
- Support for older versions is allowed only when the contributor explicitly validates and prepares them.
- Automatic migration, version-range conventions, and version conversion are outside this ticket.

### Snapshot Data Boundary

- Define a marker contract such as `IRuntimeSnapshotData` for owner-defined immutable detached data.
- The foundation does not serialize, clone, reflect over, or mutate payload data.
- A contributor must return data detached from live mutable state.
- Payloads must not contain:
  - delegates;
  - service references;
  - subscriptions;
  - scene objects;
  - Unity instance IDs;
  - editor objects;
  - mutable collections exposed for external mutation.
- A `RuntimeSnapshotContribution` or equivalent immutable record contains only:
  - contribution identity;
  - schema version;
  - non-null snapshot data.
- The aggregate `RuntimeSnapshot` or equivalent exposes an immutable registration-ordered contribution view.
- The aggregate is an in-memory handoff contract, not a persistence envelope or chosen serialized representation.

### Contributor Contract

Define a narrow contributor contract equivalent to:

- immutable `Identity`;
- positive `CurrentSchemaVersion`;
- `Capture()` returning an immutable success/failure result;
- `PrepareRestore(RuntimeSnapshotContribution contribution)` returning an immutable success/failure result and, on success, one rollback-capable restore operation.

Contributor requirements:

- capture and preparation are deterministic for the same authoritative state and input;
- capture does not mutate authoritative state;
- preparation validates identity, schema compatibility, payload type, payload content, and owner-specific invariants;
- preparation does not mutate authoritative state;
- contributor-returned failures require nonempty actionable reasons;
- thrown contributor exceptions are converted to immutable diagnostics by the coordinator;
- contributor identity and current schema version are fixed for the contributor lifetime;
- the foundation does not discover contributors automatically.

### Prepared Restore Operation

Define a narrow operation contract equivalent to:

- `Apply()`, returning an immutable success/failure result;
- `Rollback()`, returning an immutable success/failure result;
- `Release()`, returning an immutable success/failure result or a clearly equivalent non-throwing diagnostic boundary.

Required semantics:

- preparation captures everything required to apply and, if needed, restore the participant's pre-restore state;
- `Apply` may mutate only the owning contributor's authoritative state;
- successful apply is idempotently observable and must not apply twice;
- rollback is valid after an apply attempt, including an apply that reports failure after partial owner-local mutation;
- rollback restores the participant's pre-restore authoritative state as far as its contract permits;
- repeated rollback after a completed rollback is an idempotent no-op success;
- release is idempotent and relinquishes operation-held data or references;
- apply after release, rollback after release, or apply after completed rollback is rejected with an actionable result;
- operations do not publish events, schedule work, access storage, or coordinate other contributors;
- exceptions thrown by operation methods are caught by the coordinator and converted to immutable diagnostics.

Exact operation-state representation is an implementation detail, but tests must prove the externally observable semantics.

## Approved Coordinator Semantics

### Construction and Registration

- Provide one sealed plain-C# `RuntimeSnapshotCoordinator` or clearly equivalent type.
- Construction receives a complete ordered contributor sequence.
- Null contributor sequences, null entries, and duplicate contribution identities are rejected before a usable coordinator is returned.
- Registration order is the sole deterministic dependency order.
- The coordinator does not sort contributors by identity, type name, or runtime hash.
- Contributors cannot be added, removed, or reordered after construction.
- Separate coordinator instances share no mutable state.
- The coordinator is not a singleton, service locator, global registry, or reflection-discovered catalog.

### Capture

- Capture invokes each contributor exactly once in registration order.
- A successful capture returns one immutable aggregate snapshot with contributions in that same order.
- A contributor may return only its own declared identity and current schema version; mismatches fail capture.
- Null results, null data, mismatched identity, mismatched version, actionable failure results, and thrown exceptions fail capture with contributor identity and reason.
- Capture stops at the first failure.
- A failed capture returns no usable partial snapshot.
- Previously captured immutable contribution data may remain referenced only by the failure result when required for diagnostics; no failed aggregate is exposed as success.
- Capture never mutates contributor state.

### Structural Restore Validation

Before calling any contributor's preparation method, restore must validate the complete aggregate:

- snapshot is non-null;
- contribution collection is non-null;
- no contribution or payload is null;
- no contribution identity is duplicated;
- every registered contributor has exactly one matching contribution;
- no unknown contribution is present;
- versions are positive.

Structural validation failure invokes no contributor preparation and performs no authoritative mutation.

Snapshot contribution order does not grant authority. Matching uses stable identity, while preparation and apply order always follow coordinator registration order.

### Preparation

- Preparation runs in registration order.
- Each contributor receives exactly its matching contribution.
- Every preparation must succeed and return one non-null restore operation before any apply begins.
- Preparation failure stops further preparation.
- Already prepared operations are released in reverse preparation order.
- No rollback is invoked because no authoritative mutation has begun.
- The result preserves the primary preparation failure and any release failures.

### Apply, Rollback, and Release

- Apply runs in registration order.
- Each operation is attempted at most once.
- If every apply succeeds:
  - operations are released in reverse order;
  - restore succeeds only when release produces no blocking failure;
  - release failure after successful apply is reported clearly without pretending authoritative state was rolled back.
- If an apply reports failure or throws:
  - stop applying later operations;
  - roll back every attempted operation in reverse attempt order, including the failing operation;
  - release every prepared operation in reverse preparation order;
  - restore fails;
  - preserve the primary apply failure, ordered rollback failures, and ordered release failures.
- Contributors that were prepared but never applied are released without rollback.
- A failed restore must never be reported as success merely because rollback succeeded.
- A rollback failure is blocking and must identify the contribution and reason.
- The coordinator cannot guarantee recovery from a contributor that violates its rollback contract; it must preserve exact diagnostics so the later Save/Load System can treat the session as unsafe.

### Reentrancy and Single-Thread Assumption

- The foundation is explicitly single-threaded.
- Capture and restore are mutually exclusive coordinator operations.
- Calling capture or restore on the same coordinator while either operation is active throws `InvalidOperationException` before invoking another contributor.
- Always restore the coordinator's idle state through finally-equivalent cleanup.
- Operating a different coordinator instance from a callback is permitted.
- Thread synchronization, background execution, async, tasks, and locking are outside this ticket.

### Lifecycle and Composition

- The coordinator owns orchestration only; it does not own contributor lifetime.
- Do not make the coordinator an `IRuntimeService` in this ticket.
- Do not initialize or shut down contributors.
- Do not dispose contributor-owned data beyond invoking prepared operation release.
- Prove that contributors and the coordinator can be constructed explicitly alongside accepted runtime composition without modifying `RuntimeCompositionRoot`, `ComposedRuntime`, or accepted lifecycle contracts.
- Do not automatically expose the coordinator through `ComposedRuntime`.
- A later Save/Load System owns when capture and restore are requested and how the resulting snapshot is serialized or stored.

## Failure and Diagnostic Contracts

- Expected failures are immutable result values, not control-flow exceptions.
- Programmer-contract violations at public boundaries may throw `ArgumentNullException`, `ArgumentException`, `ArgumentOutOfRangeException`, `InvalidOperationException`, or `ObjectDisposedException` where explicitly applicable.
- Contributor and restore-operation exceptions must not escape coordinator capture or restore.
- Diagnostics identify:
  - operation phase;
  - contribution identity when known;
  - exception type when an exception occurred;
  - exception message;
  - actionable failure reason.
- Diagnostics must not retain exception objects, delegates, contributor instances, restore-operation instances, or mutable payload references.
- Diagnostic order follows observation order.
- Do not log, publish events, retry, reschedule, or aggregate-throw failures.

## Requirements

- Implement the approved identity, version, data, contribution, contributor, restore-operation, result, diagnostic, aggregate snapshot, and coordinator contracts.
- Keep public contracts narrow, immutable, XML documented, and Unity 5.6-compatible.
- Use explicit construction and constructor injection only.
- Use defensive copies or read-only views so callers cannot mutate aggregate contribution or diagnostic collections.
- Define deterministic equality and hashing for contribution identities only; payload equality is owner-defined and outside scope.
- Validate every public argument before mutation or contributor invocation.
- Preserve the primary failure plus ordered cleanup diagnostics.
- Add stable Unity `.meta` files for every new runtime and test asset.
- Add one focused EditMode test fixture covering all acceptance behavior.
- Leave accepted FND-001 through FND-007 production and test files unchanged.

## Out of Scope

- file, stream, database, PlayerPrefs, cloud, or platform storage;
- JSON, XML, binary, ScriptableObject, Easy Save, or other serialization-format selection;
- save envelopes, slots, filenames, platform paths, integrity hashes, encryption, compression, or cloud sync;
- autosave policy, debounce, checkpoints, user-facing save/load flows, or retry policy;
- schema migration, payload conversion, migration registries, or migration UI;
- concrete narrative, world-state, relationship, dialogue, phone, app, notification, time, schedule, or UI snapshot data;
- automatic enumeration or discovery of `RuntimeState` cells;
- a global snapshot registry, arbitrary lookup API, service locator, DI container, singleton, scene search, hierarchy lookup, or reflection discovery;
- event publication, event sourcing, replay, provenance history, undo, or redo;
- automatic runtime composition or changes to accepted composition APIs;
- Unity `MonoBehaviour`, coroutines, threads, tasks, async, locks, or background work;
- runtime/editor visualization or Game Director integration;
- scenes, prefabs, ScriptableObjects, Resources assets, packages, assembly definitions, or third-party dependencies.

## Dependencies and Prerequisites

- FND-001 through FND-007 are Complete.
- PR #19's Unity execution model, validation runbook, and verification scripts are merged into `main`.
- The implementation branch starts from current `main`.
- Local Unity 5.6.7f1 validation is available through:
  - `./Tools/Verification/unity56-preflight.sh`;
  - `./Tools/Verification/verify-unity56-editmode.sh`.

## Authorized Files or Directories

- New runtime files under `Assets/Minerva/Runtime/Core/**` required only for the approved snapshot contracts and coordinator.
- New metadata for those new runtime files.
- One new focused test fixture and its metadata under `Assets/Minerva/Tests/Runtime/Editor/**`.
- This ticket file for lifecycle movement and implementation reporting.

Existing accepted production and test files are read-only.

## Prohibited Files or Systems

- Existing files under `Assets/Minerva/Runtime/Core/**`.
- Existing files under `Assets/Minerva/Tests/Runtime/Editor/**`.
- `Tools/Verification/**`.
- `ProjectSettings/**`.
- `Packages/**`.
- `Assets/Plugins/**`.
- `Assets/ThirdParty/**`.
- scenes, prefabs, ScriptableObjects, Resources assets, and assembly definitions;
- planning, architecture, governance, handbook, context, runbook, and process documents unless separately authorized.

## Permitted Actions

- Create the approved new snapshot runtime contracts and coordinator.
- Create one focused test fixture and stable metadata.
- Move this ticket from `Ready` to `Active` when implementation begins.
- Move this ticket from `Active` to `Review` when the implementation pull request opens.
- Complete the Implementation Report and implementation-owned execution records.

## Prohibited Actions

- Do not modify accepted FND-001 through FND-007 runtime or test files.
- Do not modify the accepted Unity verification tooling.
- Do not implement persistence, serialization, migrations, domain payloads, or automatic composition.
- Do not introduce packages or assembly definitions.
- Do not move this ticket to `Complete`; independent review and Technical Director acceptance are required.
- Do not merge the implementation pull request.

## Expected Files

Exact splitting may vary when a clearer arrangement preserves the approved boundaries. Expected new assets include equivalents of:

- `RuntimeSnapshotContributionIdentity.cs`
- `IRuntimeSnapshotData.cs`
- `RuntimeSnapshotContribution.cs`
- `IRuntimeSnapshotContributor.cs`
- `IRuntimeSnapshotRestoreOperation.cs`
- `RuntimeSnapshotDiagnostic.cs`
- `RuntimeSnapshotOperationPhase.cs`
- `RuntimeSnapshotStepResult.cs`
- `RuntimeSnapshotCaptureResult.cs`
- `RuntimeSnapshotPreparationResult.cs`
- `RuntimeSnapshotRestoreResult.cs`
- `RuntimeSnapshot.cs`
- `RuntimeSnapshotCoordinator.cs`
- matching stable `.meta` files
- `RuntimeSnapshotContractsTests.cs`
- `RuntimeSnapshotContractsTests.cs.meta`

Do not create speculative adapters, serializers, migrations, storage abstractions, or domain payload types.

## Documentation Updates

- Set `Status` to `Active`, record the owner, and update the date when moving from `Ready` to `Active`.
- Set `Status` to `Review` and update the date when moving from `Active` to `Review`.
- Complete the Implementation Report before requesting review.
- Record the exact implementation environment capability declaration.
- Record the exact PR branch and head SHA handed to local validation.
- Record the generated Unity verification summary and evidence path.
- Do not update planning, architecture, process, runbook, or governance documents unless separately authorized.
- Use `America/New_York` for date and execution-state metadata.

## Architect's Notes

This ticket creates a heterogeneous orchestration boundary because different authoritative owners necessarily use different payload types. The marker payload contract is not permission to create an untyped state store. Contributors retain responsibility for exact payload-type validation and business invariants, while the coordinator handles identity, ordering, completeness, failure isolation, and transactional sequencing.

Registration order is the explicit dependency order. Avoid numeric priorities, type-name sorting, or implicit discovery.

Preparation must be genuinely non-mutating. Any owner unable to stage a restore without mutation must capture sufficient rollback state inside its restore operation and still ensure that all validation capable of failing occurs before apply. The coordinator's transactional guarantee is only as strong as each contributor's rollback contract, so failure diagnostics must remain exact and complete.

FND-008 is the last ticket in the Foundation Runtime Services epic. It must leave storage, serialization, migration, and game-domain snapshot schemas to later epics.

## Acceptance Criteria

- [ ] Contribution identity is immutable, owner-qualified, ordinal, case-sensitive, and rejects invalid identifiers.
- [ ] Schema versions are explicit positive integers and contributor compatibility is owner-controlled.
- [ ] Snapshot data is owner-defined, immutable by contract, detached, and format-agnostic.
- [ ] Contribution and aggregate snapshot records expose no externally mutable collections.
- [ ] Construction rejects null contributors and duplicate identities atomically.
- [ ] Capture order exactly matches contributor registration order.
- [ ] Capture rejects null, mismatched, invalid, failed, or throwing contributor output with actionable diagnostics and no successful partial snapshot.
- [ ] Structural restore validation rejects null, duplicate, missing, unknown, or invalid contributions before any contributor preparation.
- [ ] Preparation follows registration order and completes for every contributor before apply begins.
- [ ] Preparation failure releases prior operations in reverse order without rollback or authoritative mutation.
- [ ] Apply follows registration order and attempts each operation at most once.
- [ ] Apply failure rolls back every attempted operation in reverse order, including the failing operation.
- [ ] Prepared but unattempted operations are released without rollback.
- [ ] Successful restore releases all operations in reverse order.
- [ ] Primary, rollback, and release failures remain distinct, immutable, ordered, and actionable.
- [ ] Contributor and operation exceptions do not escape coordinator capture or restore.
- [ ] Same-coordinator capture/restore reentrancy is rejected without starting nested work, and idle state is restored after every path.
- [ ] Separate coordinators share no mutable state.
- [ ] The coordinator does not own contributor lifecycle or modify accepted composition contracts.
- [ ] No file I/O, serialization format, migration, domain payload, event publication, automatic state enumeration, or global access is introduced.
- [ ] New public contracts have XML documentation and remain Unity 5.6-compatible.
- [ ] All new assets have committed stable metadata and unique GUIDs.
- [ ] The full existing EditMode suite plus the new focused tests passes through the accepted Unity verifier.
- [ ] The implementation changes only authorized paths.
- [ ] The ticket exists in exactly one workflow directory and its `Status` matches that directory.
- [ ] The implementation PR contains this ticket under `Docs/Tickets/Review/` while independent review is pending.
- [ ] After acceptance, the implementation PR contains this ticket under `Docs/Tickets/Complete/` before merge.

## Required Validation

### Local Unity Verification

Run from the exact implementation PR head:

```bash
./Tools/Verification/unity56-preflight.sh
./Tools/Verification/verify-unity56-editmode.sh
```

Record:

- exact branch and SHA;
- Unity version;
- import/compilation result and exit code;
- total, passed, failed, skipped, and inconclusive tests;
- assertion count;
- known-warning counts;
- unknown blocking-error count;
- tracked and `.meta` integrity;
- `git diff --check`;
- restored working-tree result;
- evidence-directory path.

Missing or invalid results XML is blocking even when Unity exits `0`.

### Focused Test Coverage

Cover at minimum:

- valid and invalid contribution identities;
- deterministic identity equality and hashing;
- positive schema-version enforcement;
- immutable contribution and aggregate views;
- null contributor and duplicate-identity construction;
- capture ordering and exact single invocation;
- capture identity/version mismatch;
- capture returned failure, null data/result, and thrown exception;
- structural restore rejection for null, duplicate, missing, unknown, or invalid contributions;
- snapshot contribution order differing from registration order;
- preparation order and non-mutation before all preparation succeeds;
- preparation failure with reverse release;
- successful apply and reverse release;
- apply failure with reverse rollback of attempted operations;
- rollback of the failing partially applied operation;
- release of prepared but unattempted operations without rollback;
- callback exception conversion for capture, preparation, apply, rollback, and release;
- preservation and ordering of primary, rollback, and release diagnostics;
- same-coordinator reentrant capture and restore;
- another coordinator operating during a participant callback;
- idle-state restoration after every failure path;
- coordinator instance isolation;
- explicit coexistence with accepted runtime composition without composition changes.

### Repository and Scope Audits

- Run the complete existing EditMode suite, not only the new fixture.
- Confirm no accepted production or test file changed.
- Confirm every new Unity asset has a committed `.meta` file.
- Confirm new GUIDs are unique and stable after Unity import.
- Audit for prohibited storage, serialization, migration, domain, event, reflection, scene, concurrency, and global-access APIs.
- Confirm no package, scene, prefab, ScriptableObject, Resources asset, or assembly definition was added.
- Confirm every new text file ends with a newline.
- Run `git diff --check`.
- Confirm the ticket exists in exactly one workflow directory and `Status` matches.
- List every created, modified, moved, and deleted path.

## Definition of Done

- [ ] Acceptance criteria satisfied.
- [ ] Required local Unity validation completed on the exact PR head.
- [ ] Full EditMode suite passes with exact evidence.
- [ ] Focused snapshot tests cover success, failure, rollback, cleanup, reentrancy, and isolation.
- [ ] Repository and metadata integrity are verified.
- [ ] Documentation updated.
- [ ] No unauthorized changes.
- [ ] Implementation report completed.

## Implementation Report

### Status

Implementation is complete on `agent/fnd-008-save-snapshot-contracts`.
Draft PR #21 is open against `main`, and this ticket remains in `Review`
pending independent review and Technical Director acceptance.

### Changed Files

- Created `Assets/Minerva/Runtime/Core/IRuntimeSnapshotContributor.cs`.
- Created `Assets/Minerva/Runtime/Core/IRuntimeSnapshotContributor.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/IRuntimeSnapshotData.cs`.
- Created `Assets/Minerva/Runtime/Core/IRuntimeSnapshotData.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/IRuntimeSnapshotRestoreOperation.cs`.
- Created `Assets/Minerva/Runtime/Core/IRuntimeSnapshotRestoreOperation.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshot.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshot.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotCaptureResult.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotCaptureResult.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotContribution.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotContribution.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotContributionCaptureResult.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotContributionCaptureResult.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotContributionIdentity.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotContributionIdentity.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotCoordinator.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotCoordinator.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotDiagnostic.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotDiagnostic.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotOperationPhase.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotOperationPhase.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotPreparationResult.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotPreparationResult.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotRestoreOperation.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotRestoreOperation.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotRestoreResult.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotRestoreResult.cs.meta`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotStepResult.cs`.
- Created `Assets/Minerva/Runtime/Core/RuntimeSnapshotStepResult.cs.meta`.
- Created `Assets/Minerva/Tests/Runtime/Editor/RuntimeSnapshotContractsTests.cs`.
- Created `Assets/Minerva/Tests/Runtime/Editor/RuntimeSnapshotContractsTests.cs.meta`.
- Moved this ticket from `Docs/Tickets/Ready/` through
  `Docs/Tickets/Active/` to `Docs/Tickets/Review/`.

### Work Completed

- Added immutable owner-qualified contribution identities with ordinal,
  case-sensitive equality and deterministic hashing.
- Added format-agnostic detached payload, versioned contribution, aggregate
  snapshot, contributor, capture, preparation, restore-operation, result, and
  diagnostic contracts.
- Added a restore-operation base that enforces single apply, rollback after an
  apply attempt, completed-rollback idempotency, terminal release behavior,
  and rejection of invalid lifecycle calls.
- Added explicit immutable contributor registration with atomic rejection of
  null contributors, null identities, nonpositive current schema versions,
  and duplicate contribution identities.
- Added deterministic registration-ordered capture with exact identity,
  current-version, payload, result, and exception validation.
- Added complete structural restore validation before participant preparation,
  identity-based contribution matching, registration-ordered preparation and
  apply, reverse rollback of all attempted operations, and reverse release of
  every prepared operation.
- Added immutable ordered primary, rollback, and release diagnostics without
  retaining exceptions, delegates, participants, operations, or payloads.
- Added same-coordinator capture/restore reentrancy rejection with
  finally-equivalent idle restoration and independent coordinator isolation.
- Added one focused fixture containing 23 tests across identity, capture,
  structural validation, preparation, apply, rollback, release, failures,
  reentrancy, composition coexistence, and isolation.

### Validation

Implementation capability declaration:

```text
Repository editing: Available through the local Codex workspace
C# compiler: Available through Unity 5.6.7f1
Unity 5.6.7f1: Available locally
Unity tests: Executed by the Local Unity Verification Operator in this session
```

- PR: #21, branch `agent/fnd-008-save-snapshot-contracts`.
- Validated implementation head:
  `2af88077e106386fa5087bb257c8ed91adbecaad`.
- `./Tools/Verification/unity56-preflight.sh`: passed.
- `./Tools/Verification/verify-unity56-editmode.sh`: passed.
- Unity version: 5.6.7f1.
- Import and compilation: passed, exit code `0`.
- Complete EditMode suite: 118 total, 118 passed, 564 assertions, 0 failed,
  0 skipped, and 0 inconclusive.
- Focused fixture: 23 FND-008 tests.
- Known warnings: 2 UnityShaderCompiler socket warnings, 1 callback
  unregistration warning, and 1 `ms_Instance` shutdown assertion.
- Unknown blocking errors or exceptions: 0.
- Repository integrity: tracked and committed metadata remained unchanged,
  generated paths were safely cleaned, and the worktree was restored clean.
- `git diff --check`: passed.
- Evidence:
  `/private/tmp/project-minerva-unity/verification.1a0qlh/verification-summary.md`.
- Authorized-path audit: passed; the implementation changes only new Core
  runtime assets, one new focused runtime EditMode fixture, stable metadata,
  and this ticket.
- Accepted-file audit: passed; no accepted production or test file changed.
- Prohibited-API audit: passed; new runtime files contain no storage,
  serialization, migration, domain, event, Unity, reflection, scene,
  concurrency, or global-access implementation.
- Forbidden-asset audit: passed; no package, scene, prefab, ScriptableObject,
  Resources asset, assembly definition, vendor file, or dependency was added.
- Metadata audit: passed; all 16 new Unity assets have committed metadata and
  every GUID is unique under `Assets/`.
- Newline audit: passed for every new text file.

### Deviations

None.

### Blockers or Risks

None.

### Optional Context Used

None.

### Follow-Up Suggestions

None.

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
| Planned | 2026-07-24 | Technical Director | FND-007 is complete, PR #19 execution reliability is merged, and FND-008 readiness scope was approved for promotion. |
| In Progress | 2026-07-24 01:42 EDT | Codex | Began implementation on `agent/fnd-008-save-snapshot-contracts` from merged PR #20 (`4dd2082`). Repository editing, Unity 5.6.7f1, and Unity-backed C# compilation are available locally; mandatory validation is assigned to the Local Unity Verification Operator in this session. |
| Committed | 2026-07-24 01:49 EDT | Codex | Commit `2af88077e106386fa5087bb257c8ed91adbecaad` contains the FND-008 implementation, focused tests, stable metadata, and Active ticket state. |
| Verified | 2026-07-24 01:53 EDT | Codex | Draft PR #21 opened against `main`; Unity 5.6.7f1 import/compile and the complete 118-test, 564-assertion EditMode suite passed at implementation head `2af88077`; static scope, metadata, prohibited API, newline, and diff audits passed. |
