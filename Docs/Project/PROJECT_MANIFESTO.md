# Project Minerva Manifesto

**Version:** 1.0  
**Status:** Draft  
**Owner:** Creative Director and Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`README.md`](README.md), [`PROJECT_VISION.md`](PROJECT_VISION.md)

## We Are Building a Living Phone, Not a Collection of Screens

Project Minerva is a story-driven, real-time narrative game experienced entirely through a fictional smartphone operating system.

The phone is not a wrapper around the game. It is not a menu, a framing device, or a temporary interface between traditional scenes. The phone is the player’s world.

Every app is a different surface over one shared reality. Messages reveal conversations. Contacts reveal identities and relationships. Photos reveal evidence and memory. Maps reveal movement and location. Calendar reveals commitments and missed opportunities. Files, notes, calls, voicemail, weather, and future apps reveal other dimensions of the same hidden simulation.

The experience succeeds when the operating system feels inhabited, coherent, and consequential even when the player is not actively touching it.

## The Story Is Authored as Events in Time

Project Minerva’s narrative is written as a chronological sequence of structured events.

We do not author abstract emotional labels and ask the engine to invent the story. Context is communicated through what characters say, do, send, ignore, delete, schedule, reveal, and withhold.

A character’s fear matters because it is expressed through behavior. A relationship matters because it changes what is said, when it is said, which choices appear, and what later events become possible.

The Story Timeline is therefore the primary authoring environment. It must allow the game to be written linearly by timestamp while supporting filters across all known objects and supporting branches caused by immediate choices or persistent earlier state.

Branches may diverge and rejoin. State does not disappear when branches merge. Earlier decisions remain part of the world and continue to shape later presentation and choices.

## Author Once

No meaningful content should require duplicate entry.

When an authored event causes Alice to send a photograph, that event should be available to every relevant surface and tool: the conversation, notification system, photo library, character history, save data, Story Timeline, validation system, and Game Director.

Information has one authoritative home. Other systems consume, transform, index, or present it without creating competing copies.

Repetition is a signal that the tooling or data model is incomplete.

## Tooling Is Part of the Product

Project Minerva will be built by first creating the environment required to build it well.

The Story Timeline, branch visualization, variable exploration, validation, asset tracking, phone preview, Game Director, replay tools, and state inspection are not secondary conveniences. They are core production software.

We deliberately spend more engineering time early so that later development can focus on story, implementation through supported event types, playtesting, and replacing placeholder assets.

The goal is not to make the first event easy. The goal is to make the thousandth event no harder to author, validate, understand, and maintain than the tenth.

## The Story Timeline Creates; the Game Director Explains

Project Minerva separates two responsibilities that must not be confused.

The **Story Timeline** answers:

> What happens?

It is the primary authoring tool for chronological events, branches, conditions, effects, object references, and persistent state changes.

The **Game Director** answers:

> What is happening, and why?

It is the runtime inspection and debugging environment. It shows current state, pending events, failed conditions, state provenance, app state, notification causes, branch history, and the reasons the simulation produced its present result.

The Story Timeline changes authored content. The Game Director observes and explains runtime behavior. Their shared data models should support both without collapsing their responsibilities into one tool.

## Simulation Is Authoritative

The hidden simulation owns truth.

Phone apps do not own isolated versions of the story. User interface components do not decide narrative outcomes. A badge does not independently decide that messages are unread. A contact screen does not independently decide the player’s relationship with a character.

The simulation and world state establish what is true. Applications present and interact with that truth through clearly defined services, events, and data contracts.

This separation allows multiple surfaces to remain consistent and gives the Game Director a reliable source from which to explain behavior.

## Runtime Simplicity Is a Feature

Project Minerva’s shipped runtime should execute validated content predictably.

Where practical, complexity belongs in authoring, validation, import, indexing, and debugging tools rather than in fragile runtime interpretation. The runtime should not compensate silently for invalid content or ambiguous references.

Errors should be found before play. Missing assets, impossible dependencies, broken branch references, duplicate identifiers, invalid timestamps, and unsupported combinations should be reported clearly by tools.

A simple runtime is easier to save, replay, test, profile, and ship on both Windows and iOS.

## Architecture Is a Player-Facing Feature

Players may never see the architecture, but they will feel its consequences.

Clean boundaries enable believable timing. Shared state keeps apps coherent. Deterministic event execution makes branches trustworthy. Good save architecture protects long-form progress. Strong tooling makes richer content possible. Debuggability reduces the chance that invisible state errors break a player’s story.

Architecture is not ceremony surrounding the game. It is part of how the game earns the player’s trust.

## We Build for Change Without Building Everything in Advance

Project Minerva must support a multi-year development cycle without trying to predict every future feature.

We build stable foundations, explicit extension points, and reusable frameworks. We do not build speculative systems merely because they might be useful someday.

The vertical slice must prove the pipeline. It should demonstrate that supported content can move from authoring through validation, runtime execution, persistence, and debugging without new one-off code.

Future systems are added deliberately when story and production needs justify them.

## Data Drives Content; Code Defines Capabilities

Routine narrative content belongs in data authored through tools.

Code defines reusable capabilities: event types, conditions, effects, applications, presentation components, services, validators, and debugging support. Story content selects and configures those capabilities.

When a new story beat requires only a new instance of a supported capability, no new C# should be written. When a genuinely new capability is required, it should be designed as a reusable addition rather than a story-specific exception.

## Every Important State Must Be Inspectable

Invisible state without inspection becomes unmanageable.

Story variables, branch history, scheduled events, notification state, app state, character state, save data, and event conditions must be observable through development tools appropriate to their layer.

When something does not happen, the tools should make the reason discoverable. When something does happen, the tools should show which authored event and prior state caused it.

Debugging is not an afterthought. It is a design requirement.

## AI Assists Implementation; It Does Not Own Direction

Codex and other AI agents are implementation engineers working within approved architecture and explicit tickets.

They do not independently redesign systems, broaden scope, introduce packages, move files, rename namespaces, or refactor unrelated code. Each ticket defines the context to read, files authorized for modification, prohibited actions, acceptance criteria, and expected report.

Context is curated. More context is not automatically better. Tickets should provide enough information to complete the task without forcing an agent to load the entire repository.

Architectural thinking happens before implementation is delegated.

## Unity 5.6 Compatibility Is Non-Negotiable

Project Minerva intentionally targets Unity 5.6.

All runtime and editor code must respect the APIs, language features, serialization behavior, platform constraints, and tooling limitations available to that environment unless an approved decision explicitly changes the engine target.

Modern examples and packages must not be adopted merely because they are convenient in current Unity versions.

## Performance Serves Responsiveness and Believability

The project prioritizes responsive interaction, reliable background simulation, efficient UI, and practical mobile performance over visual spectacle.

The game is primarily text, interface, animation, audio, and state. Performance work should protect the immediacy of the phone experience and the stability of long-running sessions.

Optimization should be evidence-based, but architecture should avoid obvious waste such as unnecessary frame polling, uncontrolled allocations, duplicate state, and heavyweight dependencies.

## Documentation Preserves Intent

Important decisions must not exist only in conversations, memory, or code.

The repository records project principles, approved architecture, system responsibilities, tool behavior, major decisions, implementation tickets, and review outcomes.

Documentation should explain why a design exists, not merely restate what the code does.

A future contributor should be able to understand the project’s intent without reconstructing it from commit history or chat transcripts.

## Our Standard

We will know the project’s foundation is working when creating content increasingly feels like writing and directing a story rather than assembling software by hand.

We will know the vertical slice is successful when a supported story event can be authored, validated, executed, saved, loaded, replayed, and debugged without new C# code.

We will know the architecture is healthy when new apps and event types can be added through explicit contracts without forcing unrelated systems to change.

We will know the tools are successful when they help us understand the whole story while allowing us to focus on exactly the character, app, branch, variable, asset, or moment that matters.

Project Minerva exists to make a living narrative phone possible—and to make building that narrative sustainable.