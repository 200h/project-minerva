# Project Minerva — Architectural Principles

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`README.md`](README.md), [`PROJECT_MANIFESTO.md`](PROJECT_MANIFESTO.md), [`PROJECT_VISION.md`](PROJECT_VISION.md), [`ENGINEERING_PHILOSOPHY.md`](ENGINEERING_PHILOSOPHY.md), [`GLOSSARY.md`](GLOSSARY.md), [`AI_ENGINEERING_GUIDE.md`](AI_ENGINEERING_GUIDE.md)

## Purpose

This document defines the architectural rules that govern Project Minerva. These principles apply to runtime systems, editor tools, data models, integrations, tests, and implementation tickets.

They are intended to protect the project from architectural drift as development expands across multiple years, systems, contributors, and AI-assisted implementation sessions.

When a local implementation choice conflicts with these principles, the local implementation is presumed incorrect until an intentional architectural decision says otherwise.

## 1. The Simulation Is Authoritative

The hidden simulation and its world state determine what is true. Phone applications, screens, badges, notifications, previews, and other interfaces display or interact with that truth; they do not independently redefine it.

A Messages screen may display that a message is unread, but unread state belongs to the shared conversation or world state. A Contacts screen may display that a person is blocked, but the block relationship must be represented in shared state so that calls, messages, notifications, and future story conditions can respond consistently.

Presentation code must not become an alternative source of gameplay truth.

## 2. The Phone Is a Platform, Not a Collection of Scenes

The fictional operating system must be designed as a reusable runtime platform with registered applications, screens, navigation rules, background behavior, notification routing, and shared services.

New applications should integrate through stable interfaces and data contracts. Adding an application must not require broad modifications to unrelated applications or core simulation systems.

The operating system coordinates applications. Applications do not coordinate the operating system.

## 3. Author Once, Consume Everywhere

Narrative information must have one authoritative authored representation.

A Timeline Event that sends a photograph may influence the Story Timeline, Messages, Photos, notifications, character history, save data, validation, and Game Director inspection. Those surfaces must consume or derive their information from the same authored event and resulting runtime state.

Contributors must not duplicate content manually across separate editors merely to satisfy different presentation surfaces.

## 4. The Story Timeline Owns Narrative Authoring

The Story Timeline is the primary source of authored chronological narrative behavior.

It records what happens, when it happens, which known objects participate, which conditions allow it, which state changes it produces, which assets it references, and which branch or merge structure contains it.

Supporting tools may provide specialized views or focused editing experiences, but they must not create competing narrative sources of truth.

## 5. The Game Director Observes and Explains

The Game Director is a runtime inspection and debugging environment. It explains current and historical state, event eligibility, failed conditions, queued work, state provenance, and screen behavior.

The Game Director must not become a second narrative-authoring environment. Any controlled state mutation offered for debugging must be explicit, isolated, reversible where practical, and distinguishable from authored content.

## 6. Definitions and Runtime State Are Separate

Immutable or design-time definitions must remain separate from mutable runtime state.

Definitions describe what an entity, event type, application, asset, or rule is. Runtime state describes what is currently true in a particular playthrough.

This separation is required for predictable saves, validation, replay, tooling, migration, comparison, and testability.

## 7. Persistent State Survives Branch Rejoins

Narrative branches may diverge and later rejoin a shared timeline, but persistent effects produced within those branches remain part of world state unless explicitly reversed.

A merge point combines authored flow; it does not erase player history.

Later conditions may therefore present different events, choices, text, assets, or app behavior even when multiple playthroughs have returned to the same main timeline segment.

## 8. Communication Uses Explicit Contracts

Systems should communicate through clear interfaces, typed events, commands, queries, and shared data contracts rather than hidden object references or scene-specific dependencies.

An event bus is appropriate for announcing that something occurred. It is not a replacement for every direct method call, nor should it conceal ownership of state.

Use direct dependencies when one system clearly owns and provides a service. Use events when multiple independent consumers may react to a completed occurrence.

## 9. Dependencies Point Inward

Higher-level presentation and feature modules may depend on stable lower-level abstractions. Core systems must not depend on individual applications, screens, story chapters, or content packages.

The intended direction is broadly:

```text
Presentation and Editor Views
        ↓
Application and Tool Logic
        ↓
Runtime Services and Simulation
        ↓
State, Data Contracts, and Infrastructure
```

Circular dependencies and upward knowledge are architectural defects.

## 10. Runtime Simplicity Is Preferred

Complex authoring, validation, reference resolution, and consistency checking should occur before or during content preparation whenever practical.

The shipped runtime should consume validated, structured data and execute deterministic responsibilities. It should not repeatedly rediscover relationships that tooling could have resolved earlier.

This does not prohibit runtime branching or simulation. It means avoid burdening the player build with avoidable authoring complexity.

## 11. Validation Is Part of the Architecture

Invalid content must be discoverable before it reaches a player.

Systems and data contracts must support validation for missing references, invalid timestamps, unreachable branches, conflicting identifiers, incompatible event fields, absent assets, unsupported app interactions, and other defined errors.

Validation rules are not optional editor polish. They are part of the feature design.

## 12. Every Important State Must Be Inspectable

A system that can change story-relevant state must expose enough information to explain its behavior.

At minimum, significant state changes should be attributable to an event, command, player action, load operation, migration, or developer action. Debug tools should be able to show current values, relevant conditions, pending work, and provenance where the cost is reasonable.

Opaque systems are considered incomplete.

## 13. Time Is a Domain Model

Project Minerva uses story and simulation time as a first-class domain concept rather than scattering raw timers and frame checks throughout the codebase.

Scheduled events, delayed messages, appointments, calls, background behavior, expiry, and replay must use shared time abstractions.

Wall-clock time, simulation time, authored timestamps, elapsed durations, and presentation timestamps must be explicitly distinguished.

## 14. Save Data Is a Contract

Persistence must be accessed through the Minerva save abstraction, even when a third-party serializer is used beneath it.

Save data requires stable identifiers, version information, migration planning, and controlled ownership. Individual systems must not write arbitrary files or serialize undocumented object graphs directly.

A third-party save asset may provide storage mechanics. Project Minerva owns the save model.

## 15. Stable Identifiers Are Mandatory

Story events, characters, conversations, applications, variables, branches, assets, and other persistent or cross-referenced objects require stable identifiers that do not depend on display names, list positions, scene hierarchy, or memory references.

Renaming visible content must not silently break saves or references.

Identifier generation, validation, and migration must be deliberate.

## 16. Editor and Runtime Boundaries Are Explicit

Unity Editor-only code must live in editor-safe locations and namespaces and must not be included in player builds.

Reusable data contracts may be shared between editor and runtime code. Editor tools may prepare, validate, and preview runtime data, but runtime assemblies must not depend on UnityEditor APIs.

`Minerva.Editor` and `Minerva.Tools` are distinct domains: editor-only authoring tools versus runtime-accessible development utilities.

## 17. Composition Is Preferred Over Deep Inheritance

Behavior should be assembled from focused services, policies, handlers, and data rather than rigid inheritance trees.

Inheritance is acceptable where a genuine substitutable relationship exists and the base contract remains stable. It must not be used merely to reuse implementation or force unrelated event types into a fragile hierarchy.

## 18. Extension Should Favor Addition Over Modification

New event types, applications, validators, inspectors, and content should be addable through registration and well-defined extension points.

A feature is healthier when it can be introduced by adding a module rather than editing a growing central switch statement or manager class.

This principle must not be interpreted as a demand for premature abstraction. Extension points should be justified by known project direction.

## 19. Managers Coordinate; They Do Not Accumulate Everything

A manager may own lifecycle, registration, orchestration, or access to a coherent domain service. It must not become a miscellaneous container for unrelated behavior.

Before creating a manager, define:

- the state or lifecycle it owns;
- the services it coordinates;
- why that ownership must be globally reachable;
- how the system can be tested and inspected;
- whether a smaller service or registry would be clearer.

Singletons are not the default solution.

## 20. Unity 5.6 Compatibility Is Non-Negotiable

All production code, editor tooling, packages, APIs, language features, serialization assumptions, and build procedures must be compatible with the selected Unity 5.6 environment unless the project intentionally changes engine versions through an approved decision.

Contributors must verify compatibility rather than assuming modern Unity behavior.

## 21. Cross-Platform Behavior Is Designed, Not Patched Later

Windows and iOS are first-class targets.

Input, file paths, persistence, resolution, aspect ratio, touch interaction, text rendering, performance, application lifecycle, and platform restrictions must be considered at system boundaries.

Platform-specific behavior should be isolated behind interfaces or configuration rather than scattered conditional logic.

## 22. Third-Party Assets Remain Behind Project Abstractions

External assets may accelerate generic infrastructure such as serialization, text rendering, animation, inspector productivity, or input.

Core project code must depend on Minerva-owned abstractions where replacement risk or broad coupling exists. Story content and distinctive gameplay systems must not become structurally dependent on a vendor-specific workflow without an approved architectural decision.

Buy infrastructure where useful; retain ownership of gameplay identity.

## 23. Performance Work Must Preserve Clarity

The project prioritizes responsive UI and reliable mobile performance, but optimization must be based on identified constraints or measurements.

Frame-driven polling, avoidable allocations, repeated data discovery, excessive scene objects, and unnecessary serialization should be prevented through sound design. Premature micro-optimization that obscures correctness or maintainability is discouraged.

## 24. Architecture Must Support the Thousandth Event

Every major design should be evaluated against mature production scale.

The relevant question is not only whether the first message, branch, notification, or photograph can be implemented. The architecture must make the thousandth supported event routine to author, validate, execute, inspect, and maintain.

## 25. Changes Require Proportionate Evidence

Small, local implementation choices may be resolved within a ticket. Changes to ownership, layer boundaries, persistence contracts, public interfaces, core terminology, or production workflow require design review and may require an Architecture Decision Record.

Implementation must not silently establish architecture by precedent.

## Applying These Principles

Tickets must cite the principles most relevant to their scope when architectural guidance is necessary. Code review should evaluate both acceptance criteria and architectural alignment.

When principles appear to conflict, prefer the interpretation that best preserves:

1. authoritative and explainable state;
2. single-source authoring;
3. clear ownership and dependency direction;
4. efficient long-term content production;
5. Unity 5.6 and cross-platform reliability.

If the conflict materially changes the project’s direction, record the decision rather than burying it in implementation.