# FND-002: Create Runtime Bootstrap

**Status:** Ready  
**Owner:** Unassigned  
**Created:** 2026-07-22  
**Updated:** 2026-07-22  
**Roadmap Phase:** Phase 1 — Foundation  
**Related ADRs:** [`../../ADR/0001_Use_Unity_5_6.md`](../../ADR/0001_Use_Unity_5_6.md), [`../../ADR/0004_Ticket_Driven_AI_Implementation.md`](../../ADR/0004_Ticket_Driven_AI_Implementation.md)

## Summary

Create the smallest deterministic runtime bootstrap needed to construct, initialize, stop, and dispose future Project Minerva services.

The implementation must establish explicit lifecycle contracts and composition without implementing any gameplay, narrative, time, event, save, phone, editor-tool, or platform behavior.

## Background

FND-001 established the approved Unity repository structure. Phase 1 now requires a narrow bootstrap foundation so later services can be added without relying on scene searches, implicit singleton creation, global mutable registries, or undefined initialization order.

This ticket defines lifecycle and composition only. It must not become a general dependency-injection framework or introduce service implementations that belong to later tickets.

## Goal

Provide a Unity 5.6-compatible runtime bootstrap that:

- registers services explicitly;
- initializes them in deterministic registration order;
- stops later initialization after a failure;
- shuts down successfully initialized services in reverse order;
- supports repeated construction and disposal in tests;
- exposes lifecycle state and initialization results for diagnostics;
- contains no app-specific or story-specific behavior.

## Required Context

Read only:

- `Docs/Project/README.md`
- `Docs/Project/CODING_STANDARDS.md`
- `Docs/Project/AI_ENGINEERING_GUIDE.md`
- `Docs/Architecture/SYSTEM_OVERVIEW.md`
- `Docs/Architecture/RUNTIME_LAYERS.md`
- `Docs/Architecture/DEPENDENCY_RULES.md`
- `Docs/Architecture/INITIALIZATION_AND_LIFECYCLE.md`
- `Docs/Architecture/EDITOR_RUNTIME_BOUNDARIES.md`
- `Docs/Context/Foundation.md`
- `Docs/Handbook/Definition_of_Done.md`

## Optional Context

Read only when required by a specific implementation question:

- `Docs/Architecture/EVENT_ARCHITECTURE.md`
- `Docs/Architecture/STATE_OWNERSHIP.md`
- `Docs/Handbook/Git_Workflow.md`
- `Docs/Handbook/Branch_Strategy.md`

## Code Areas to Inspect

- `Assets/Minerva/Runtime/Core/`
- `Assets/Minerva/Tests/Runtime/`
- root Unity project metadata only when required to confirm Unity 5.6 compatibility
- this ticket file for lifecycle movement and the implementation report

Do not perform repository-wide discovery beyond Required Context and these areas.

## Requirements

- Create a narrow runtime lifecycle contract for services.
- Create a deterministic bootstrap/composition implementation.
- Register services through explicit code rather than scene search or reflection-based discovery.
- Preserve registration order for initialization.
- Reject duplicate registration of the same service instance.
- Prevent initialization from running more than once on the same runtime instance.
- Stop initialization immediately when a service reports failure or throws.
- Preserve enough information to identify the failed service and failure reason.
- Shut down only services that initialized successfully.
- Shut down initialized services in reverse order.
- Make shutdown idempotent.
- Allow the runtime to be created, initialized, shut down, and disposed repeatedly across separate test instances without leaked static mutable state.
- Keep public contracts small and documented.
- Use the `Minerva` namespace root.
- Remain compatible with Unity 5.6 and its supported C# language/runtime constraints.
- Add focused tests for deterministic order, failure handling, duplicate registration, reverse shutdown, idempotent shutdown, and repeated construction.

## Out of Scope

- Event Bus implementation
- Time and Clock implementation
- Save/Load implementation
- World State implementation
- Narrative Runtime implementation
- Notification implementation
- App Runtime implementation
- UI Navigation implementation
- configuration loading
- file or disk access
- serialization
- logging framework beyond minimal diagnostic result data
- scene creation
- prefabs
- MonoBehaviour-based global managers
- service locator APIs
- reflection-based service discovery
- third-party dependency-injection containers
- Story Timeline
- Game Director
- editor windows or editor tooling
- platform-specific Windows or iOS behavior
- assembly definitions

## Dependencies and Prerequisites

- FND-001 is Complete.
- The Unity 5.6 project skeleton opens successfully.
- M0.3 Architecture Baseline is merged.
- No unresolved ADR is required for this narrow lifecycle implementation.

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

- Add lifecycle contracts and runtime bootstrap implementation under the authorized Core path.
- Add focused runtime tests under the authorized test path.
- Remove obsolete `.gitkeep` placeholders only from directories that receive authorized files.
- Move this ticket through `Ready`, `Active`, and `Review` as required by the workflow.
- Complete the Implementation Report.
- Run narrow searches and Unity validation required by this ticket.

## Prohibited Actions

- Do not implement any future foundation service.
- Do not create a general-purpose dependency-injection container.
- Do not expose a global service locator.
- Do not use `GameObject.Find`, `FindObjectOfType`, scene-name lookup, or hidden singleton creation for composition.
- Do not add static mutable service registries.
- Do not use reflection to discover services.
- Do not introduce third-party packages.
- Do not create scenes, prefabs, ScriptableObjects, or editor assets.
- Do not add assembly definitions.
- Do not change architecture, system, tool, ADR, planning, or project-governance documents.
- Do not move the ticket to `Complete`; post-merge closeout belongs to the Technical Director.

## Expected Files

Exact names may vary when required by a clearer Unity 5.6-compatible design, but the implementation should remain within this shape:

- lifecycle service contract
- initialization result or failure representation
- lifecycle state representation
- runtime bootstrap/coordinator
- explicit composition or builder helper
- focused runtime tests

All production files must remain under `Assets/Minerva/Runtime/Core/`. All tests must remain under `Assets/Minerva/Tests/Runtime/`.

## Documentation Updates

- Move this ticket from `Ready` to `Active` when implementation begins and set `Status` to `Active`.
- Move this ticket from `Active` to `Review` when the implementation PR is opened and set `Status` to `Review`.
- Complete the Implementation Report before requesting review.
- Do not move the ticket to `Complete`; the Technical Director performs that closeout after merge and acceptance.

## Architect's Notes

Prefer plain C# objects and explicit constructor dependencies. Unity component lifecycle methods must not become the authoritative runtime lifecycle for foundation services.

A composition root may know concrete service types. Individual services must not know the composition root or discover peers globally.

Minimal diagnostic data is allowed to identify lifecycle stage, service type, and failure reason. A broader logging framework belongs to a later ticket.

Tests should avoid depending on Unity scenes or editor APIs.

## Acceptance Criteria

- [ ] Runtime services can be registered explicitly in a deterministic order.
- [ ] Duplicate registration of the same instance is rejected clearly.
- [ ] Initialization occurs in registration order.
- [ ] Initialization stops at the first reported failure or exception.
- [ ] Failure information identifies the failing service and reason.
- [ ] Only successfully initialized services are shut down.
- [ ] Shutdown occurs in reverse initialization order.
- [ ] Repeated shutdown calls are safe and do not repeat service shutdown.
- [ ] Separate runtime instances can be created and disposed repeatedly without leaked static state.
- [ ] Runtime implementation contains no `UnityEditor` reference.
- [ ] No scene search, hidden singleton, global service locator, or reflection-based discovery is introduced.
- [ ] No app-specific, story-specific, event, time, save, phone, or platform behavior is added.
- [ ] Focused tests cover all required lifecycle behavior.
- [ ] The final diff contains only authorized paths.
- [ ] The implementation report accurately records changes and validation.

## Required Validation

- List every created, modified, moved, and deleted path.
- Confirm no path outside Authorized Files or Directories changed.
- Run all focused runtime tests available in the environment.
- Open the project in Unity 5.6 batch mode and report compiler/import results when Unity 5.6 is available.
- Search runtime files for `UnityEditor`; no matches may exist.
- Search implementation for `GameObject.Find`, `FindObjectOfType`, global service-locator access, reflection-based discovery, and mutable static service collections; no prohibited implementation may exist.
- Confirm no `.unity`, `.prefab`, `.asset`, `.asmdef`, package, or vendor file was added.
- Confirm all new text files end with a newline.
- Run `git diff --check`.

## Definition of Done

- [ ] Acceptance criteria satisfied.
- [ ] Required validation reported accurately.
- [ ] Documentation updated.
- [ ] No unauthorized changes.
- [ ] Implementation report completed.

## Implementation Report

### Status

### Changed Files

### Work Completed

### Validation

### Deviations

### Blockers or Risks

### Optional Context Used

### Follow-Up Suggestions
