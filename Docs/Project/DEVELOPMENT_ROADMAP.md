# Project Minerva — Development Roadmap

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [PROJECT_VISION.md](PROJECT_VISION.md), [ARCHITECTURAL_PRINCIPLES.md](ARCHITECTURAL_PRINCIPLES.md), [ENGINEERING_PHILOSOPHY.md](ENGINEERING_PHILOSOPHY.md), [AI_ENGINEERING_GUIDE.md](AI_ENGINEERING_GUIDE.md)

## Purpose

This roadmap defines the approved development order for Project Minerva. It converts the project vision into staged technical and production milestones, with explicit goals, deliverables, exit criteria, and exclusions for each phase.

The roadmap is intentionally tooling-first. Project Minerva is expected to spend a significant portion of early development building the systems required to author, validate, execute, inspect, and maintain narrative content efficiently. The vertical slice is therefore a proof of the production pipeline, not only a demonstration of player-facing presentation.

## Roadmap Principles

The following rules apply across all phases:

1. Every phase must leave the repository in a coherent, reviewable state.
2. A phase is complete only when its exit criteria are satisfied.
3. Later-phase features must not be pulled forward without a documented need.
4. Repetitive content work is a signal to improve tooling or data structures.
5. Runtime systems must remain compatible with Unity 5.6 and the approved target platforms.
6. New systems and tools require documented purpose and boundaries before implementation.
7. Codex implementation work must originate from approved tickets with curated context and explicit authority.
8. The project should optimize for the hundredth and thousandth content entry, not only the first.

## Phase 0 — Preproduction and Project Constitution

### Goal

Establish the project identity, engineering rules, documentation model, workflow, repository structure, and decision-making framework before production code is introduced.

### Deliverables

- Project Constitution under `Docs/Project/`
- Repository and namespace identity
- Documentation hierarchy
- Ticket and context-loading strategy
- Git and review workflow
- Initial architecture decision records
- Top-level repository structure
- Initial technical backlog and milestones
- Third-party asset review policy

### Exit Criteria

- The Project Constitution is approved and merged.
- Repository naming and namespace conventions are fixed.
- The documentation hierarchy and ticket lifecycle are documented.
- Codex role boundaries and context guardrails are approved.
- The next phase can be decomposed into implementation-ready tickets.

### Out of Scope

- Runtime gameplay systems
- Player-facing phone applications
- Story content production
- Final UI design
- Production art and audio

## Phase 1 — Foundation

### Goal

Build the minimal shared runtime foundation required by all future systems and tools.

### Major Deliverables

- Bootstrap and initialization flow
- Core event framework
- Time framework and deterministic game clock
- Save abstraction and versioned save contract
- Data registry and identifier conventions
- Configuration loading
- Logging and diagnostics foundation
- Serialization conventions
- Resource and content loading boundaries
- Platform abstraction where required for Windows and iOS

### Required Characteristics

- Small, focused interfaces
- Explicit initialization order
- No app-specific or story-specific behavior
- Runtime state serializable through approved contracts
- Debug visibility designed in from the beginning
- Unity 5.6 compatibility verified

### Exit Criteria

- Core services initialize deterministically.
- Services can be tested independently where practical.
- Time, events, data, logging, and save boundaries have approved documentation.
- No future application or narrative system needs to bypass the foundation.
- Foundation tickets meet the project Definition of Done.

### Out of Scope

- Story Timeline UI
- Game Director UI
- Full Phone OS
- Messages application
- Narrative content
- Visual polish

## Phase 1.5 — Shared Frameworks

### Goal

Build reusable frameworks that support both runtime systems and editor tooling without prematurely implementing complete applications or tools.

### Major Deliverables

- Screen and navigation framework
- Phone application registration framework
- State machine conventions
- Command framework with undo and redo support for editor tooling
- Selection and inspection framework
- Generic timeline data and view foundations
- Validation result framework
- Search and filtering primitives
- Editor integration conventions
- Shared data-binding or presentation-update pattern appropriate to Unity 5.6

### Exit Criteria

- Frameworks have at least one minimal example or test consumer.
- Runtime and editor concerns remain explicitly separated.
- Frameworks do not encode story-specific assumptions unnecessarily.
- The Narrative Suite can be implemented without creating parallel foundational infrastructure.

### Out of Scope

- Complete Story Timeline authoring workflow
- Complete Phone OS presentation
- Game Director
- Production-ready app layouts

## Phase 2 — Narrative Suite

### Goal

Create the primary authoring environment used to build the game as a chronological, filterable, branching narrative.

### Major Deliverables

#### Story Timeline

- Linear timestamp-based event authoring
- Typed timeline events
- Actor, target, app, conversation, location, asset, and variable references
- Immediate player-choice branching
- Branching based on prior persistent state
- Merge points that preserve divergent state
- Event conditions and effects
- Filtering by all known authored object types
- Search and navigation across timeline content

#### Supporting Tools

- Branch Viewer
- Variable Explorer
- Asset Browser
- Validation window
- Missing-reference reporting
- Dependency and state-usage inspection
- Conversation preview where useful
- Content import and export foundations

### Exit Criteria

- A representative branching sequence can be authored without editing raw serialized data.
- Branches can merge while preserving persistent state changes.
- Variables can be traced to writers and readers.
- Missing references and invalid structures are reported before runtime.
- Authored events can be consumed by a minimal test runner.
- Routine authoring does not require new C# code.

### Out of Scope

- Complete shipped Phone OS
- Full production story
- All planned phone apps
- Final visual design
- Full Game Director debugging suite

## Phase 3 — Vertical Slice Runtime

### Goal

Prove the complete authoring-to-runtime pipeline with the smallest coherent player-facing experience.

### Major Deliverables

- Phone OS shell
- Lock screen
- Home screen
- Status bar essentials
- Notification system
- Messages application
- Minimal Contacts support
- Story runtime capable of executing supported Timeline Events
- Branch and persistent-variable evaluation
- Save and load integration
- Minimal audio and transition support where necessary
- Representative vertical-slice content authored through the Narrative Suite

### Vertical Slice Production Test

By the end of this phase, a supported story event must be:

1. Authored in the Story Timeline.
2. Validated without entering Play Mode.
3. Executed by the runtime.
4. Presented through the appropriate phone surface.
5. Persisted to a save.
6. Restored from a save.
7. Diagnosed through available debugging information.
8. Added without writing new C# code.

### Exit Criteria

- The complete pipeline succeeds for the representative event set.
- Immediate and historical branching both function correctly.
- State survives branch merges and save/load cycles.
- Apps act as views and interaction surfaces over shared state.
- Performance is acceptable on representative Windows and iOS hardware.
- The architecture can support future content without redesigning the foundation.

### Out of Scope

- Full content volume
- Every planned phone app
- Final accessibility implementation
- Final performance optimization
- Release-ready polish

## Phase 4 — Simulation Suite

### Goal

Build the runtime inspection, replay, and diagnostic environment required to explain simulation behavior and accelerate content debugging.

### Major Deliverables

#### Game Director

- Current simulation summary
- State and condition inspection
- Event provenance
- Explanations for blocked, skipped, or triggered events
- Navigation to related authored events and variables

#### Supporting Tools

- Timeline Replay
- Event Queue viewer
- Notification Queue viewer
- Phone Preview and screen-state inspection
- Character Monitor
- Conversation Inspector
- Save Inspector
- Variable history and change provenance
- Controlled time advancement and scenario reconstruction

### Exit Criteria

- A developer can determine why a representative event did or did not occur.
- Runtime state can be inspected without manually searching serialized files.
- Screen and app state can be viewed for a known simulation snapshot.
- Replay or reconstruction does not corrupt the live save or authoring data.
- Debug tooling is excluded from release builds unless explicitly approved.

### Out of Scope

- Story authoring ownership
- Production content expansion
- Player-facing debug features

## Phase 5 — Content Production

### Goal

Shift the project’s primary effort from engine and tool construction to story creation, implementation, asset completion, and iterative playtesting.

### Major Activities

- Author chapters, scenes, choices, and branches in the Story Timeline
- Add and refine characters, contacts, conversations, media, files, locations, and calendar content
- Implement missing media and presentation assets referenced by story nodes
- Use validation and Game Director tools to resolve content defects
- Add new event types only when existing abstractions cannot represent required behavior cleanly
- Conduct narrative QA, continuity review, and branch coverage testing

### Exit Criteria

- The complete planned narrative is implemented.
- All required branches and merge paths are validated.
- Missing content assets are resolved or intentionally deferred.
- Save compatibility is maintained across production updates.
- Content-authoring throughput is stable and predictable.

### Out of Scope

- Unplanned framework rewrites
- Major architectural changes without ADR approval
- Feature expansion that does not support the approved game vision

## Phase 6 — Polish, Certification, and Release Preparation

### Goal

Prepare Project Minerva for release quality on Windows and iOS.

### Major Deliverables

- UI and interaction polish
- Accessibility improvements
- Performance and memory optimization
- Device and resolution coverage
- Save migration validation
- Content completion and editorial QA
- Audio balancing and presentation refinement
- Build automation and release packaging
- Platform compliance and certification preparation
- Crash, logging, and diagnostics strategy for release

### Exit Criteria

- Release candidates meet approved quality targets.
- Windows and iOS builds pass platform-specific validation.
- Critical branch paths and save scenarios are covered.
- No unresolved release-blocking defects remain.
- Documentation reflects the shipped architecture and workflow.

## Cross-Phase Reviews

Formal reviews should occur at major boundaries and may be recorded under `Docs/Reviews/`.

Recommended reviews include:

- Constitution approval review
- Foundation architecture review
- Narrative Suite workflow review
- Vertical-slice postmortem
- Simulation tooling review
- Production throughput review
- Performance and technical-debt audit

## Change Management

This roadmap may evolve, but changes must be deliberate.

A proposed change should identify:

- The phase affected
- The reason for the change
- The impact on dependencies and exit criteria
- Whether an ADR is required
- Which tickets or milestones must be updated

Work should not silently bypass phase boundaries because a later feature appears convenient to implement early.

## Immediate Next Milestone

After approval of the Project Constitution, the project should create the supporting documentation and workflow infrastructure required to complete Phase 0, including:

- `Docs/Handbook/`
- `Docs/Architecture/`
- `Docs/Tickets/`
- `Docs/Templates/`
- `Docs/ADR/`
- `Planning/`

Only then should Codex receive implementation tickets for the Foundation phase.
