# Project Minerva — Third-Party Assets

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [PROJECT_VISION.md](PROJECT_VISION.md), [ARCHITECTURAL_PRINCIPLES.md](ARCHITECTURAL_PRINCIPLES.md), [ENGINEERING_PHILOSOPHY.md](ENGINEERING_PHILOSOPHY.md), [CODING_STANDARDS.md](CODING_STANDARDS.md)

## Purpose

This document defines how Project Minerva evaluates, adopts, wraps, updates, and replaces third-party Unity assets and software libraries.

The project follows a simple rule:

> Buy or adopt proven infrastructure where it reduces risk and development time; build the systems that define Project Minerva’s identity.

Third-party assets are implementation details, not architectural owners. No external package may become the sole source of truth for story state, phone behavior, branching, simulation, or content-authoring workflow.

## Adoption Principles

A third-party asset may be considered when it:

- Solves a mature, generic engineering problem.
- Has a clear licensing model suitable for commercial use.
- Has a version demonstrably compatible with Unity 5.6 and the project’s scripting runtime.
- Supports Windows and iOS where it is used at runtime.
- Can be isolated behind a Minerva-owned abstraction when it provides infrastructure.
- Does not force story logic, save contracts, or authoring data into a proprietary format that is difficult to inspect or migrate.
- Reduces total maintenance cost rather than only reducing initial implementation time.
- Can be removed or replaced without rewriting unrelated systems.

An asset should be rejected when it:

- Requires a newer Unity version.
- Introduces an incompatible runtime or language requirement.
- Depends on unsupported platform features.
- Owns or obscures critical project state.
- Uses reflection, dynamic code generation, or unsupported serialization in a way that is unsafe for iOS ahead-of-time compilation.
- Creates editor or runtime coupling across unrelated domains.
- Prevents deterministic validation or debugging.
- Has unclear licensing, redistribution, or source-access terms.
- Is abandoned without an acceptable source-code fallback.

## Approval States

Third-party assets use the following statuses:

- **Candidate** — potentially useful but not yet technically or legally approved.
- **Evaluation** — being tested in an isolated branch or prototype.
- **Approved** — permitted for use within a defined scope and version.
- **Restricted** — approved only for a specific editor or runtime purpose.
- **Rejected** — evaluated and intentionally not adopted.
- **Deprecated** — previously approved but scheduled for removal or replacement.

Approval applies to a specific asset version and scope. A newer version is not automatically approved.

## Required Evaluation Record

Before adoption, create or update an evaluation record containing:

- Asset name and vendor
- Exact version
- License and seat requirements
- Unity version compatibility evidence
- Windows and iOS compatibility where relevant
- Source-code availability
- Runtime and editor dependencies
- Build-size and performance impact
- Serialization or save-data implications
- AOT and stripping considerations
- Known limitations
- Proposed Minerva abstraction boundary
- Replacement or removal strategy
- Evaluation owner and date
- Approval decision

A significant adoption decision should be captured in an ADR.

## Candidate Infrastructure Assets

The following assets have been identified as candidates. None should be integrated until the exact legacy-compatible version, license, platform behavior, and source availability have been verified in the Project Minerva repository.

### Easy Save 3

**Proposed Role:** Low-level persistence provider beneath `Minerva.Save`.

**Potential Value:**

- Reduces effort required for file IO, serialization support, encryption, compression, and storage backends.
- Provides a mature implementation for generic persistence concerns.

**Required Boundary:**

Project code must call a Minerva-owned save abstraction. Story systems, apps, and simulation code must not invoke Easy Save APIs directly.

```text
Simulation and Apps
        ↓
Minerva.Save contracts
        ↓
Approved persistence adapter
        ↓
Easy Save or replacement provider
```

**Evaluation Concerns:**

- Exact Unity 5.6-compatible version availability
- iOS AOT behavior
- Supported custom data structures
- Save migration control
- Vendor-specific metadata in save files
- Behavior when the provider is removed or replaced

**Status:** Candidate

### DOTween / DOTween Pro

**Proposed Role:** UI and presentation tweening beneath an approved Minerva animation boundary.

**Potential Value:**

- Reduces custom coroutine and interpolation code.
- Supports reusable transitions for screens, notifications, typing indicators, panels, and other phone presentation.

**Required Boundary:**

Simulation and story code must not depend on tween completion as the source of gameplay truth. Presentation controllers may use DOTween through localized adapters or conventions.

**Evaluation Concerns:**

- Exact Unity 5.6-compatible version
- iOS build behavior
- Allocation profile
- Lifecycle cleanup for destroyed views
- Pro-only feature need
- Ease of replacing animations without changing simulation behavior

**Status:** Candidate

### TextMesh Pro

**Proposed Role:** Text rendering for the text-heavy phone interface.

**Potential Value:**

- Improved typography and text rendering compared with legacy Unity UI text.
- Rich-text and layout capabilities useful for messages, notifications, files, and other content-heavy surfaces.

**Required Boundary:**

TextMesh Pro remains a rendering dependency. Narrative content and message models must store presentation-independent text and metadata.

**Evaluation Concerns:**

- Exact legacy package version compatible with Unity 5.6
- Font asset generation workflow
- Dynamic font atlas behavior
- Emoji, fallback glyph, and localization strategy
- Runtime memory on iOS
- Licensing of all font assets

**Status:** Candidate

### Odin Inspector

**Proposed Role:** Editor productivity and inspector enhancement.

**Potential Value:**

- Faster creation of readable inspectors for data-heavy definitions.
- Reduced custom inspector code for early tooling.

**Restrictions:**

- Runtime systems must not require Odin unless separately approved.
- Project data must remain accessible without Odin.
- Core authoring workflows should not become impossible to maintain if Odin is removed.
- Odin serialization must not become the default save or story-data format without a separate architectural decision.

**Evaluation Concerns:**

- Unity 5.6-compatible version
- Editor performance at production content scale
- Serialization coupling
- Team seat requirements
- Whether bespoke Story Timeline tooling makes portions of Odin redundant

**Status:** Candidate, editor-only preference

### Rewired

**Proposed Role:** Input abstraction for Windows keyboard, mouse, and optional gamepad support.

**Potential Value:**

- Mature input mapping for legacy Unity versions.
- Consistent control remapping and device handling.

**Restrictions:**

Project Minerva is primarily a phone-style UI experience. Rewired should be adopted only if native Unity 5.6 input handling is insufficient for the approved Windows and accessibility requirements.

**Evaluation Concerns:**

- Actual control requirements
- iOS relevance
- Legacy-compatible version
- UI event-system integration
- Runtime and build-size overhead
- Whether an internal lightweight input abstraction is sufficient

**Status:** Candidate, deferred until input requirements are documented

## Assets Requiring Caution

### Dialogue Frameworks

Examples include node-based dialogue, conversation, and visual scripting packages.

Project Minerva’s narrative is asynchronous, timestamped, app-aware, stateful, and authored primarily through the Story Timeline. Traditional dialogue packages often assume a synchronous speaker-response graph rendered through a dialogue window.

A dialogue package may be used only if it can act as a limited component without taking ownership of:

- Timeline chronology
- Persistent branch state
- Message delivery timing
- Read receipts
- Typing behavior
- App routing
- World State
- Save authority
- Story Timeline authoring

**Default Position:** Do not adopt as the core narrative framework.

### Visual Scripting, Behavior Trees, and Node Graphs

Graph assets may be useful for visualization or limited behavior authoring, but they must not replace the primary linear Story Timeline authoring model.

A graph framework may be considered for generated branch visualization or isolated simulation behaviors. It must not force the entire story into a graph-first workflow.

**Default Position:** Evaluate only against a specific documented need.

### Complete Save, Quest, Inventory, or RPG Frameworks

Large frameworks often bring assumptions that conflict with Project Minerva’s state model and authoring workflow.

**Default Position:** Prefer focused infrastructure providers behind Minerva abstractions; reject frameworks that own multiple unrelated gameplay domains.

### Smartphone UI Templates

Phone UI templates may provide temporary visual references or prototype elements, but they commonly encode navigation, visual identity, and behavior that Project Minerva must own.

**Default Position:** Do not adopt as the Phone OS architecture. Limited art or prototype use requires explicit review.

## Systems That Must Remain Minerva-Owned

The following systems define the project’s product identity and must be designed and owned by Project Minerva:

- Story Timeline
- Timeline Event model
- Branch and merge semantics
- Story Variable system
- Story Runtime
- World State
- Game Director
- Timeline Replay
- Validation and provenance systems
- Phone OS framework
- App registry and navigation model
- Notification semantics
- Messages and conversation simulation
- Character and relationship state
- Time and scheduled narrative-event semantics
- Save contracts and migration policy
- Content identifiers and registries
- Screen-state inspection
- Authoring-to-runtime pipeline

Third-party libraries may support low-level implementation, but they must not define these systems’ public contracts or canonical data.

## Wrapping and Isolation

Third-party runtime infrastructure should be placed behind Minerva-owned interfaces whenever replacement cost would otherwise spread across the codebase.

Examples:

```text
Minerva.Save.ISaveStorage
Minerva.UI.ITweenService
Minerva.Input.IInputService
Minerva.Audio.IAudioService
```

Adapters should live in a clearly identified integration area, such as:

```text
Assets/Project/ThirdParty/Adapters/
```

or another location defined by the approved Folder Structure document.

Do not create abstractions mechanically for every small utility. The purpose of an abstraction is to protect project-owned contracts and reduce replacement cost, not to hide all external APIs indiscriminately.

## Repository Placement

Third-party assets should be isolated from Minerva-owned code.

The final folder structure will be defined separately, but the intended separation is:

```text
Assets/
├── Project/        # Minerva-owned assets and code
└── ThirdParty/     # Vendor assets, where package constraints allow
```

When an asset requires a specific folder, document the exception. Do not move vendor files casually because Unity references, import scripts, or future upgrades may depend on their original paths.

Vendor source code should not be reformatted or refactored unless required to fix a documented compatibility defect. Any modification must be recorded so future upgrades can reproduce it.

## Version Pinning and Updates

- Pin the exact approved version.
- Record the source and acquisition date.
- Do not update an asset during unrelated work.
- Test updates in an isolated branch.
- Review release notes and migration requirements.
- Re-run Windows and iOS validation where applicable.
- Confirm save and serialized-content compatibility.
- Preserve legally required notices.

An update should be treated as a scoped dependency change with its own ticket.

## Licensing and Source Control

Before committing an asset:

- Confirm the license permits repository use by the authorized team.
- Confirm whether the repository may be public.
- Do not commit packages that cannot legally be redistributed in a public repository.
- Follow vendor instructions for source control and seat licensing.
- Keep purchase receipts and license records outside the public repository where appropriate.
- Document whether contributors must acquire their own licensed copy.

Project Minerva’s current repository visibility must be considered before any commercial Asset Store package is committed.

## Public Repository Guardrail

Because `200h/project-minerva` is currently public, no paid or redistribution-restricted asset may be committed until its license has been reviewed for public source exposure.

Where a package cannot be stored publicly, the project should document:

- Installation prerequisites
- Approved version
- Expected installation path
- Required local configuration
- How automated builds obtain the dependency, if applicable

This guardrail applies even when the repository owner has purchased the asset.

## Security and Supply Chain

Third-party code should be treated as external code execution.

Evaluation should include:

- Editor scripts that run on import
- Native plugins
- Network access
- Telemetry
- Binary-only dependencies
- Build hooks
- Post-processing scripts
- Obfuscated code
- Unexpected file-system access

Do not import unreviewed packages into the primary development branch.

## Removal Strategy

Every approved infrastructure asset should have a plausible removal path.

The removal plan should identify:

- The Minerva abstraction that contains the dependency
- Data that requires migration
- Vendor-specific serialized fields
- Replacement behavior
- Tests that prove equivalence

A third-party asset is safest when removing it is inconvenient but bounded—not existential.

## Initial Recommendation

Proceed with evaluation in this order:

1. Text rendering, because the player experience is text intensive.
2. UI tweening, because screen and notification transitions will be pervasive.
3. Save provider, after the Minerva save contracts and migration model are designed.
4. Editor inspector enhancements, after the first authored data models reveal actual productivity needs.
5. Input middleware, only after Windows and accessibility requirements justify it.

Do not purchase or integrate all candidates merely because they appear on this list.

## Decision Summary

Project Minerva will:

- Use third-party assets selectively for generic infrastructure.
- Preserve Minerva-owned contracts and canonical data.
- Verify exact Unity 5.6 compatibility before adoption.
- Isolate runtime dependencies behind appropriate boundaries.
- Avoid external ownership of narrative, simulation, phone, and tooling systems.
- Pin versions and review upgrades deliberately.
- Respect licensing constraints, especially in a public repository.
- Prefer replaceable dependencies over deeply embedded frameworks.
