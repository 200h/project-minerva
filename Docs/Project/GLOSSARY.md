# Project Minerva — Glossary

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`README.md`](README.md), [`PROJECT_MANIFESTO.md`](PROJECT_MANIFESTO.md), [`PROJECT_VISION.md`](PROJECT_VISION.md), [`ARCHITECTURAL_PRINCIPLES.md`](ARCHITECTURAL_PRINCIPLES.md), [`ENGINEERING_PHILOSOPHY.md`](ENGINEERING_PHILOSOPHY.md), [`AI_ENGINEERING_GUIDE.md`](AI_ENGINEERING_GUIDE.md)

## Purpose

This glossary establishes the preferred language for Project Minerva documentation, tickets, code, tools, and reviews.

Terms should be used consistently. When a new concept is introduced, contributors should first determine whether an existing term already describes it. Material changes to established terminology should be reviewed because naming influences architecture, authoring workflows, and implementation context.

## Project and Product Terms

### Project Minerva

The internal codename for the complete game project, including runtime software, editor tooling, documentation, production workflows, narrative content, and assets.

### Minerva

The root C# namespace for project-owned code.

### Minerva Framework

The reusable technical foundation created for Project Minerva, including core services, data contracts, phone-platform systems, story runtime, and development tools. The term does not imply that the framework is currently a separate commercial product.

### Player Build

A Windows or iOS build intended to run the game without Unity Editor-only authoring code.

### Vertical Slice

The smallest complete proof of Project Minerva’s authoring-to-runtime pipeline. It must demonstrate that a supported story event can be authored, validated, executed, saved, loaded, and debugged without new event-specific C# code.

## Narrative Authoring Terms

### Story Timeline

The primary Unity Editor tool for authoring the game as a chronological, filterable sequence of typed Timeline Events. It supports conditions, effects, branches, merge points, persistent state, object references, and asset references.

### Timeline Event

A single structured authored occurrence placed at a defined Story Timestamp. It describes what happens, which objects participate, which conditions apply, which effects occur, and which assets or applications are involved.

### Event Type

The schema and execution contract that define a category of Timeline Event, such as sending a message, receiving a call, adding a photograph, or changing a Story Variable.

### Story Timestamp

The authored date and time associated with a Timeline Event in the narrative chronology. It is distinct from real-world wall-clock time and may be evaluated relative to branch or runtime conditions.

### Main Timeline

The primary authored chronological flow from which branches diverge and to which they may later return.

### Story Branch

An authored divergence in narrative flow selected or activated by a condition, player choice, prior state, or defined rule.

### Immediate Choice Branch

A Story Branch created directly by a current player choice, such as selecting one response from several available options.

### State-Gated Branch

A Story Branch selected because of persistent state established earlier, even when the current event does not offer an explicit choice.

### Branch Path

The ordered Timeline Events executed within a particular Story Branch.

### Merge Point

An authored location where multiple Branch Paths reconnect to shared narrative flow. Rejoining the authored flow does not erase persistent state created within a branch.

### Condition

A rule evaluated against current state to determine whether a Timeline Event, branch, choice, effect, or presentation variant is eligible.

### Effect

A defined state change produced by a Timeline Event, command, or player action.

### Presentation Variant

Alternative authored content shown for the same broad narrative position when persistent state requires different wording, choices, assets, or app behavior.

### Story Variable

A named persistent value used by conditions, effects, branching, validation, and presentation. Supported types will be defined by the Story Variable system.

### Flag

A Boolean Story Variable. Use the broader term Story Variable when the value is not strictly Boolean.

### State Provenance

Information identifying what created or changed a particular state value, such as a Timeline Event, player action, load operation, migration, or developer command.

### Story Content

Authored narrative data, including Timeline Events, dialogue text, choices, conditions, effects, references, and structured metadata.

## Runtime and Simulation Terms

### Story Runtime

The runtime system that consumes validated authored narrative data, evaluates conditions, schedules or executes Timeline Events, applies effects, and coordinates resulting simulation behavior.

### Simulation

The collection of runtime systems and mutable state that represent the hidden world behind the phone interface.

### World State

The authoritative mutable state of the current playthrough, including Story Variables and domain-specific runtime state.

### Runtime State

Mutable data representing what is currently true during a playthrough. Runtime State is separate from design-time Definitions.

### Definition

Stable design-time data describing what an object, event type, application, asset, or rule is. A Definition is not a playthrough-specific mutable record.

### State Transition

A controlled change from one valid Runtime State to another.

### Runtime Event

A typed notification published by a runtime system after or during a meaningful occurrence. A Runtime Event is not the same as a Timeline Event, although executing a Timeline Event may publish one or more Runtime Events.

### Scheduled Event

Runtime work registered to become eligible at a future Simulation Time or Story Timestamp, subject to its defined conditions.

### Event Queue

An ordered runtime collection of pending, scheduled, delayed, or otherwise awaiting events or commands.

### Command

An explicit request for a system to perform an action or state change. A command has a clear target and owner and may succeed or fail.

### Query

A read-only request for information from a system or state owner.

### Service

A focused component that provides a coherent capability through a defined contract.

### Manager

A component responsible for lifecycle, registration, orchestration, or ownership within a coherent domain. The term must not be used as a generic suffix for unrelated behavior.

### Registry

A controlled mapping of stable identifiers to Definitions, handlers, applications, event types, assets, or other registered objects.

### Stable Identifier

A persistent unique value used to reference an object independently of its display name, collection order, scene hierarchy, or memory location.

### Simulation Time

The authoritative clock used by runtime simulation systems. It may be paused, accelerated, advanced, saved, loaded, or replayed.

### Wall-Clock Time

Actual device or system time outside the game simulation.

### Elapsed Time

A duration measured between two moments. It is not itself a calendar date or Story Timestamp.

## Phone Platform Terms

### Phone OS

The fictional smartphone operating-system platform that manages screens, applications, navigation, notifications, device state, and shared presentation services.

### Phone App

A modular presentation and interaction surface registered with the Phone OS. A Phone App consumes shared data and Runtime State and does not own isolated story logic.

### App Definition

Design-time metadata describing a Phone App, such as its stable identifier, display name, icon, capabilities, and registration information.

### App State

Playthrough-specific state associated with a Phone App, such as navigation position, unread counts, selected conversation, or installed status, where appropriate.

### Screen

A defined phone-interface view managed through the screen and navigation framework. A screen is not necessarily a Unity scene.

### Screen State

The data required to represent a screen at a particular runtime snapshot. It should be derivable from authoritative state and explicit presentation state.

### Navigation Stack

The ordered history or hierarchy used by the Phone OS to determine the active screen and back-navigation behavior.

### Notification

A structured phone-facing presentation of an occurrence. A Notification is created from runtime behavior or authored intent and does not itself own the underlying event truth.

### Notification Queue

The runtime collection of notifications waiting to be presented, updated, expired, or otherwise processed.

### Badge

A visual count or status indicator derived from authoritative app or content state, such as unread conversations.

## Tooling Terms

### Narrative Suite

The collection of editor tools used to author, search, validate, inspect, and prepare Story Content. The Story Timeline is its primary authoring surface.

### Simulation Suite

The collection of runtime and editor-supported development tools used to inspect, replay, explain, and debug simulation behavior.

### Game Director

The primary runtime inspection and debugging tool within the Simulation Suite. It explains current state, failed conditions, queued work, state provenance, and resulting phone behavior.

### Branch Viewer

A tool that visualizes Story Branches, Branch Paths, conditions, and Merge Points without replacing the Story Timeline as the authoritative authoring source.

### Variable Explorer

A tool for locating Story Variables and inspecting their definitions, defaults, readers, writers, current values, and provenance.

### Asset Browser

A tool for locating story-referenced media and other assets, inspecting usage, and identifying missing or invalid references.

### Validation Panel

A tool surface that presents errors, warnings, and informational diagnostics produced by content and system validators.

### Timeline Replay

A debugging capability that reconstructs or advances simulation behavior across time for inspection. Replay must make its snapshot and branch assumptions explicit.

### Phone Preview

A development view that renders or inspects phone screens and applications against a selected state or snapshot without requiring normal player navigation.

### Screen Explorer

A grid or browser view of registered phone screens, allowing rapid preview and inspection against an identified state snapshot.

### State Snapshot

A read-only or isolated representation of relevant Runtime State at a defined point used for preview, comparison, replay, or debugging.

### Context Pack

A curated Markdown reading index for a project domain. It identifies relevant documentation but does not replace the exact context instructions in a ticket.

## Persistence Terms

### Save Model

The project-owned structure and contracts defining what data is persisted and how saved state is organized.

### Save Data

A serializable representation of playthrough state written through the Minerva save abstraction.

### Save Slot

A user- or system-addressable location containing Save Data and associated metadata.

### Save Version

A version identifier describing the schema expected by a saved data set.

### Migration

A controlled transformation that converts older persisted or authored data into a newer supported schema.

### Save Abstraction

The Minerva-owned interface and services through which persistence is performed, regardless of the underlying serializer or third-party asset.

## Process and Governance Terms

### Project Constitution

The highest-level governing document set in `Docs/Project/`. It defines project identity, principles, and execution standards.

### Architecture Decision Record (ADR)

A short document preserving a significant decision, its context, alternatives, rationale, and consequences.

### Ticket

The authoritative scoped work package provided to an implementation engineer or Codex session.

### Required Context

The exact documents and code areas that must be read before implementing a ticket.

### Optional Context

Additional identified material that may be read only when the ticket’s implementation requires it.

### Authorized Files

The files or directory boundaries a ticket permits the implementer to create or modify.

### Out of Scope

Work explicitly excluded from a ticket, even if related or potentially useful.

### Acceptance Criteria

Observable conditions that must be satisfied for a ticket’s requested result to be considered correct.

### Definition of Done

The broader completion standard that may include implementation, validation, tests, documentation, review, examples, and reporting.

### Implementation Report

The concise report returned by an implementer describing changed files, completed behavior, validation performed, deviations, blockers, and follow-up considerations.

### Architectural Drift

Unreviewed movement away from approved ownership, dependency, data, tooling, or workflow decisions.

### Technical Debt

A known engineering compromise that increases future cost or risk and is explicitly recorded for management.

## Usage Rules

- Use **Timeline Event** for authored chronological occurrences.
- Use **Runtime Event** for typed runtime notifications between systems.
- Use **Story Variable** for persistent branching values; use **Flag** only for Boolean Story Variables.
- Use **Definition** for design-time data and **Runtime State** for mutable playthrough data.
- Use **Story Timeline** for authoring and **Game Director** for runtime inspection.
- Use **Phone App** for registered application modules and **Screen** for individual views.
- Use **Simulation Time**, **Story Timestamp**, and **Wall-Clock Time** distinctly.

When a ticket or design requires a term not defined here, define it in the relevant document and propose a glossary update if it will be used across domains.