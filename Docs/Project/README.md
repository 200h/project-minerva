# Project Minerva — Project Constitution

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Repository:** `project-minerva`  
**Namespace Root:** `Minerva`

## Purpose

This directory contains the highest-level governing documents for Project Minerva. Together, these documents define what the project is, why it is being built, which principles guide its design, and how contributors—human or AI—are expected to work within the repository.

The Project Constitution is authoritative. When an implementation, ticket, design proposal, or workflow conflicts with an approved constitutional document, the implementation or proposal is presumed incorrect until the Constitution is intentionally amended.

The Constitution is not intended to capture every technical detail. System designs, tool specifications, architecture diagrams, implementation notes, and operational procedures belong elsewhere in the repository. This directory establishes the stable context those materials must follow.

## Project Identity

Project Minerva is a story-driven, real-time narrative game built in Unity 5.6 for Windows and iOS.

The player experiences the game entirely through a fictional smartphone operating system. The phone is not a menu layered over a separate game world. The phone is the game world. Messages, notifications, calls, photos, contacts, maps, calendars, files, notes, and future applications are different surfaces through which the player encounters a shared hidden simulation.

The project is intentionally tooling-first. Early development emphasizes the frameworks, authoring systems, validation, inspection tools, and production workflows required to make later story creation efficient, reliable, and largely free of new gameplay code.

## Constitutional Documents

### Identity

- [`PROJECT_MANIFESTO.md`](PROJECT_MANIFESTO.md) — the durable beliefs and non-negotiable values that define the project.
- [`PROJECT_VISION.md`](PROJECT_VISION.md) — the concrete product, technical, authoring, and production goals derived from the Manifesto.

### Principles

The following documents will be added in subsequent constitution waves:

- `ARCHITECTURAL_PRINCIPLES.md`
- `ENGINEERING_PHILOSOPHY.md`
- `GLOSSARY.md`
- `AI_ENGINEERING_GUIDE.md`

### Execution

The following documents will define the approved development order and implementation standards:

- `DEVELOPMENT_ROADMAP.md`
- `CODING_STANDARDS.md`
- `THIRD_PARTY_ASSETS.md`

## Authority and Change Control

Constitutional documents use three statuses:

- **Draft** — actively under review and not yet binding.
- **Approved** — authoritative for design and implementation decisions.
- **Deprecated** — retained for historical reference but superseded by an identified replacement.

Material constitutional changes should be intentional, reviewed, and accompanied by an Architecture Decision Record when they affect established technical or production direction. Minor wording corrections and clarifications do not require an ADR unless they change meaning.

The Constitution is versioned as a set. Additive clarifications may increment the minor version. Changes that alter the project’s fundamental identity, architecture, or production philosophy should increment the major version.

## Repository Knowledge Hierarchy

Project Minerva uses a clear hierarchy of authority:

1. **Project Constitution** — defines identity, principles, and governing standards.
2. **Architecture Decision Records** — preserve major decisions and their rationale.
3. **Architecture and System Documentation** — define approved designs and boundaries.
4. **Tickets** — authorize specific, scoped work.
5. **Implementation** — realizes approved designs and tickets.
6. **Tests and validation output** — demonstrate expected behavior and expose defects.

A lower level must not silently contradict a higher level.

## Contributor Expectations

Before proposing or implementing significant work, contributors should understand the relevant constitutional principles and read only the additional architecture, system, tool, and ticket context required for the task.

Contributors must not infer permission to redesign unrelated systems, expand ticket scope, introduce dependencies, move repository structures, or modify architectural boundaries. Codex and other AI implementation agents work from explicit tickets that specify required reading, authorized files, prohibited actions, acceptance criteria, and reporting requirements.

Project documentation is a first-class project asset. Significant systems and tools must have their purpose, responsibilities, boundaries, dependencies, and intended extension points documented before production implementation begins.

## Core Terminology

Until the formal Glossary is approved, the following terms are used consistently:

- **Story Timeline** — the primary editor tool for authoring the game as a chronological, filterable sequence of typed narrative events, including branches, conditions, effects, and persistent state.
- **Game Director** — the runtime inspection and debugging suite used to understand what is happening, why it is happening, and which state or conditions produced the current result.
- **Timeline Event** — a structured authored occurrence placed at a defined story timestamp.
- **Story Variable** — a persistent value used by conditions, effects, branching, and later narrative presentation.
- **World State** — the authoritative mutable state of the running simulation.
- **Phone App** — a presentation and interaction surface over shared data and simulation state; it does not own isolated story logic.
- **Vertical Slice** — the smallest complete proof of the authoring-to-runtime pipeline, not merely a polished sample of gameplay.

## Guiding Production Test

The project’s central production test is:

> By the end of the vertical slice, a new supported story event must be authorable, validated, executed, saved, loaded, and debugged without writing new C# code.

This criterion does not mean every future feature requires no engineering. It means routine content production must flow through established tools and data structures rather than one-off scripts.

## Related Repository Areas

As the repository develops, the Constitution will be supported by:

- `Docs/Architecture/` — engine-wide design and layer boundaries.
- `Docs/Systems/` — individual runtime system documentation.
- `Docs/Tools/` — editor and developer-tool specifications.
- `Docs/Handbook/` — development workflow and contributor procedures.
- `Docs/ADR/` — architecture decision records.
- `Docs/Tickets/` — scoped work packages for Codex and other implementers.
- `Docs/Templates/` — standardized document and ticket templates.
- `Planning/` — active backlog, milestones, risks, limitations, and technical debt.

## Reading Order

A new contributor should read this file first, followed by:

1. [`PROJECT_MANIFESTO.md`](PROJECT_MANIFESTO.md)
2. [`PROJECT_VISION.md`](PROJECT_VISION.md)
3. `ARCHITECTURAL_PRINCIPLES.md` once available
4. `ENGINEERING_PHILOSOPHY.md` once available
5. Only the architecture, system, tool, and ticket documents required for the assigned work

This reading order provides shared purpose without unnecessarily expanding a contributor’s context window.