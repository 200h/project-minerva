# FND-001: Create Unity Project Skeleton

**Status:** Ready  
**Owner:** Unassigned  
**Created:** 2026-07-21  
**Updated:** 2026-07-21  
**Roadmap Phase:** Phase 1 — Foundation  
**Related ADRs:** [`../../ADR/0001_Use_Unity_5_6.md`](../../ADR/0001_Use_Unity_5_6.md), [`../../ADR/0004_Ticket_Driven_AI_Implementation.md`](../../ADR/0004_Ticket_Driven_AI_Implementation.md)

## Summary

Create the approved Unity 5.6 repository skeleton for Project Minerva without implementing gameplay, simulation, editor-tool, or platform behavior.

This ticket establishes the physical folder boundaries required by the approved architecture so later foundation tickets can add code without inventing repository structure.

## Background

Project Minerva Constitution v1.0, the Phase 0 workflow foundation, and the M0.3 Architecture Baseline are merged into `main`.

The repository currently contains documentation and planning artifacts but does not yet contain the Unity project structure described in `Docs/Architecture/REPOSITORY_STRUCTURE.md`.

The first implementation step must create only the approved skeleton and repository-support files. It must not add speculative systems, managers, packages, or abstractions.

## Goal

Produce a clean Unity 5.6-compatible project skeleton that:

- reflects the approved runtime, editor, test, content, and resource boundaries;
- preserves empty directories in Git using minimal placeholder files;
- establishes the `Minerva` namespace convention through documentation only;
- includes an appropriate Unity `.gitignore`;
- includes minimal root documentation for opening the project;
- contains no gameplay or framework implementation.

## Required Context

Read only:

- `Docs/Project/README.md`
- `Docs/Project/CODING_STANDARDS.md`
- `Docs/Project/AI_ENGINEERING_GUIDE.md`
- `Docs/Architecture/REPOSITORY_STRUCTURE.md`
- `Docs/Architecture/RUNTIME_LAYERS.md`
- `Docs/Architecture/DEPENDENCY_RULES.md`
- `Docs/Architecture/EDITOR_RUNTIME_BOUNDARIES.md`
- `Docs/Context/Foundation.md`
- `Docs/Handbook/Definition_of_Done.md`

## Optional Context

Read only when required by a specific implementation question:

- `Docs/Architecture/SYSTEM_OVERVIEW.md`
- `Docs/Handbook/Git_Workflow.md`
- `Docs/Handbook/Branch_Strategy.md`

## Code Areas to Inspect

- Repository root
- Existing `Assets/`, `ProjectSettings/`, or `Packages/` paths if present
- Existing `.gitignore`
- Existing root `README.md`

Do not perform repository-wide architecture discovery beyond these areas and Required Context.

## Requirements

- Create the approved directory structure under `Assets/Minerva/`:
  - `Runtime/Core/`
  - `Runtime/Application/`
  - `Runtime/Simulation/`
  - `Runtime/Phone/`
  - `Runtime/Presentation/`
  - `Runtime/Platform/`
  - `Editor/`
  - `Tests/Runtime/`
  - `Tests/Editor/`
  - `Content/Source/`
  - `Content/Compiled/`
  - `Resources/`
- Preserve intentionally empty directories with minimal `.gitkeep` files or another approved non-code placeholder.
- Add or update the repository `.gitignore` for Unity-generated artifacts while preserving tracked source, settings, documentation, and assets.
- Add or update a root `README.md` containing:
  - project name;
  - engine target: Unity 5.6;
  - supported targets: Windows and iOS;
  - concise instructions for opening the repository as a Unity project;
  - a link to `Docs/Project/README.md`;
  - a note that project code uses the `Minerva` namespace root.
- Create Unity-required top-level project folders only when necessary for a valid Unity project skeleton.
- Keep all added files compatible with a public GitHub repository.
- Use plain UTF-8 text files with final newlines.

## Out of Scope

- Gameplay code
- Runtime services or managers
- Event Bus implementation
- World State implementation
- Time and Clock implementation
- Narrative Runtime implementation
- Save/Load implementation
- Notification implementation
- App Runtime implementation
- UI Navigation implementation
- Story Timeline implementation
- Game Director implementation
- Validation Framework implementation
- Shared Editor Framework implementation
- Scenes, prefabs, art, audio, or narrative content
- Third-party assets or packages
- CI/CD workflows
- Build automation
- iOS or Windows platform integration
- Assembly definitions unless a separately approved ticket and Unity 5.6 compatibility decision authorize them

## Dependencies and Prerequisites

- Project Minerva Constitution v1.0 merged
- Phase 0 workflow foundation merged
- M0.3 Architecture Baseline merged
- Unity 5.6 remains the approved engine target

## Authorized Files or Directories

- `.gitignore`
- `README.md`
- `Assets/Minerva/**`
- `ProjectSettings/**` only when required to establish a valid Unity 5.6 project skeleton
- `Packages/**` only when already required by the checked-in Unity project; do not introduce package-manager dependencies
- `Docs/Tickets/Ready/FND-001_Create_Unity_Project_Skeleton.md` only for the implementation report and status movement performed by the approved workflow

## Prohibited Files or Systems

- `Docs/Project/**`
- `Docs/Architecture/**`
- `Docs/Systems/**`
- `Docs/Tools/**`
- `Docs/ADR/**`
- `Planning/**`
- Existing approved tickets other than `FND-001`
- Vendor or third-party directories
- Any generated Unity cache directory such as `Library/`, `Temp/`, `Obj/`, `Build/`, or `Logs/`

## Permitted Actions

- Create approved folders and placeholder files.
- Add or update `.gitignore` within ticket scope.
- Add or update the root `README.md` within ticket scope.
- Add minimal Unity project metadata required for Unity 5.6 recognition when such files can be created accurately.
- Perform narrow searches to confirm whether authorized files or folders already exist.
- Record implementation results in the ticket report.

## Prohibited Actions

- Do not create C# classes, interfaces, enums, structs, ScriptableObjects, MonoBehaviours, editor windows, tests, scenes, or prefabs.
- Do not introduce a service locator, singleton, dependency-injection container, composition root, or manager.
- Do not add third-party packages or modify dependency versions.
- Do not invent assembly definitions.
- Do not rename or reorganize approved documentation.
- Do not modify architecture or system specifications.
- Do not add placeholder code presented as future architecture.
- Do not inspect or modify unrelated repository areas.

## Expected Files

At minimum:

- `.gitignore`
- `README.md`
- placeholder files retaining each approved `Assets/Minerva/` directory

Unity project metadata may be added only when accurate and necessary for a valid Unity 5.6 skeleton.

## Documentation Updates

- Complete the Implementation Report in this ticket.
- Do not update architecture, system, tool, ADR, or planning documents during implementation.

## Architect's Notes

This ticket establishes physical boundaries only. Folder existence does not authorize implementation inside every folder.

`Resources/` is reserved and should remain empty except for a placeholder. Future tickets must justify runtime resource loading before adding content there.

Do not add assembly-definition files based on modern Unity conventions. Unity 5.6 compatibility and the project’s intended compile-boundary mechanism require a focused decision after the basic project can be opened and inspected.

When the repository already contains Unity project metadata, preserve it unless a required ticket outcome cannot be met. Report contradictions rather than replacing unknown settings.

## Acceptance Criteria

- [ ] Every approved `Assets/Minerva/` directory exists and is retained by Git.
- [ ] No unapproved directory is added under `Assets/Minerva/`.
- [ ] No C# source, scene, prefab, ScriptableObject, or gameplay asset is added.
- [ ] `.gitignore` excludes standard Unity-generated caches, temporary files, logs, and build outputs.
- [ ] `.gitignore` does not exclude approved source, documentation, project settings, or content paths.
- [ ] Root `README.md` identifies Unity 5.6, Windows, iOS, the `Minerva` namespace root, and links to the Project Constitution.
- [ ] Runtime paths contain no `UnityEditor` references.
- [ ] No third-party package or dependency is added.
- [ ] No assembly-definition file is added.
- [ ] The final diff contains only authorized files and directories.
- [ ] The implementation report accurately records all changes and validation.

## Required Validation

- List every created and modified path.
- Confirm no files were changed outside Authorized Files or Directories.
- Search the final diff for `.cs`, `.unity`, `.prefab`, `.asset`, and `.asmdef` files; none may have been added.
- Search runtime paths for `UnityEditor`; no matches may exist.
- Confirm each required directory is retained in Git.
- Review `.gitignore` against the created structure to ensure required files are not ignored.
- Confirm all new text files end with a newline.
- When Unity 5.6 is available, open the project and report whether it is recognized without import-blocking errors. When unavailable, state that this validation was not run.

## Definition of Done

- [ ] Acceptance criteria satisfied.
- [ ] Required validation reported accurately.
- [ ] Documentation updated.
- [ ] No unauthorized changes.
- [ ] Implementation report completed.

## Implementation Report

### Status

Completed.

### Changed Files

- `.gitignore` — added Unity-generated artifact exclusions.
- `README.md` — renamed the existing root readme to the required casing and added project-opening guidance.
- `Assets/Minerva/Content/Compiled/.gitkeep` — retained the compiled-content directory.
- `Assets/Minerva/Content/Source/.gitkeep` — retained the source-content directory.
- `Assets/Minerva/Editor/.gitkeep` — retained the editor directory.
- `Assets/Minerva/Resources/.gitkeep` — retained the reserved resources directory.
- `Assets/Minerva/Runtime/Application/.gitkeep` — retained the application runtime directory.
- `Assets/Minerva/Runtime/Core/.gitkeep` — retained the core runtime directory.
- `Assets/Minerva/Runtime/Phone/.gitkeep` — retained the phone runtime directory.
- `Assets/Minerva/Runtime/Platform/.gitkeep` — retained the platform runtime directory.
- `Assets/Minerva/Runtime/Presentation/.gitkeep` — retained the presentation runtime directory.
- `Assets/Minerva/Runtime/Simulation/.gitkeep` — retained the simulation runtime directory.
- `Assets/Minerva/Tests/Editor/.gitkeep` — retained the editor-test directory.
- `Assets/Minerva/Tests/Runtime/.gitkeep` — retained the runtime-test directory.
- `Docs/Tickets/Ready/FND-001_Create_Unity_Project_Skeleton.md` — recorded the implementation report.

### Work Completed

- Read the ticket and all documents listed directly under Required Context.
- Inspected only the repository root and the ticket-authorized Unity project paths.
- Created exactly the twelve approved `Assets/Minerva/` leaf directories and retained each with a newline-only `.gitkeep` placeholder.
- Added a Unity-focused `.gitignore` without excluding project source, documentation, settings, content, or placeholders.
- Added the project name, Unity 5.6 engine target, Windows and iOS targets, opening instructions, `Minerva` namespace note, and Project Constitution link to the root `README.md`.
- Added no gameplay code, packages, scenes, prefabs, assembly definitions, assets, or speculative architecture.

### Validation

- Listed every changed path under Changed Files and confirmed each is authorized by FND-001.
- Compared the created leaf-directory list with the twelve required paths; the lists matched exactly.
- Listed every file under `Assets/Minerva/`; only the twelve `.gitkeep` placeholders exist.
- Searched `Assets/Minerva/` for `.cs`, `.unity`, `.prefab`, `.asset`, and `.asmdef` files; no matches were found.
- Searched `Assets/Minerva/Runtime/` for `UnityEditor`; no matches were found.
- Ran `git check-ignore` for every placeholder, `README.md`, and `Docs/Project/README.md`; none are ignored.
- Reviewed `.gitignore`; it excludes Unity caches, temporary files, logs, generated IDE files, diagnostics, and build outputs without excluding approved paths.
- Checked every new or modified text file for a final newline; all passed.
- Ran `git diff --check`; it passed.
- Inspected the installed Unity application version; only Unity 5.3.0f4 is available. Unity 5.6 project recognition and import-blocking-error validation were therefore not run.

### Deviations

None.

### Blockers or Risks

- Unity 5.6 project recognition remains unverified because Unity 5.6 is not available in the environment.

### Optional Context Used

- `Docs/Handbook/Git_Workflow.md` — used for the requested pull-request workflow.
- `Docs/Handbook/Branch_Strategy.md` — used for the requested branch workflow.

### Follow-Up Suggestions

- Open the repository with Unity 5.6 and confirm recognition without import-blocking errors.
