# Project Minerva Vision

**Version:** 1.0  
**Status:** Draft  
**Owner:** Creative Director and Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`README.md`](README.md), [`PROJECT_MANIFESTO.md`](PROJECT_MANIFESTO.md)

## Product Vision

Project Minerva is a story-driven, real-time text adventure experienced entirely through a fictional smartphone operating system.

The player never steps into a conventional rendered world. Instead, the world is inferred through the phone: conversations, notifications, calls, photographs, calendars, maps, files, notes, contacts, and other applications. The operating system should feel familiar enough to understand immediately, but distinct enough to have its own identity and to support gameplay-specific behavior without copying a commercial mobile platform.

The project targets Windows and iOS and is built in Unity 5.6. It prioritizes responsive interface behavior, believable timing, narrative coherence, maintainable systems, and efficient content production over high-end graphics.

## Player Experience

The intended experience is intimate, immediate, and persistent.

The player should feel that:

- the phone belongs to a real person inside a larger world;
- characters continue to act whether or not the player responds;
- timing, silence, delay, interruption, and notification pressure carry narrative meaning;
- choices have both immediate and long-lived consequences;
- earlier decisions can change later choices, wording, timing, relationships, and available paths;
- information discovered in one app may alter how content in another app is understood;
- the operating system remains internally consistent as the story evolves.

Realism comes from behavior rather than visual imitation. Typing indicators, edited or deleted messages, missed calls, delayed replies, read states, unknown contacts, attachments, calendar conflicts, and background notifications should support the story rather than exist as decorative simulation.

## Core Design Pillars

### 1. The Phone Is the World

All player-facing story interaction occurs through the fictional phone. Applications are not isolated minigames; they are coordinated views into one shared narrative and simulation state.

### 2. Time Is Narrative

The story unfolds on an authored timeline. Timestamps, delays, deadlines, schedules, missed opportunities, and asynchronous responses are first-class narrative tools.

### 3. State Persists Across Branches

Immediate choices may create short branches. Earlier choices may determine which later branch is presented. Branches may reconnect to a shared timeline, but their state changes remain active and continue to influence future conditions and presentation.

### 4. Context Is Expressed Through Events

The author communicates character emotion, intent, and knowledge through dialogue, actions, app interactions, timing, and consequences. The authoring model does not depend on abstract prose such as “Alice is scared” as executable story input. Instead, Alice may say she is scared, hesitate, delete a message, call someone, share a photograph, or change her behavior.

### 5. The Operating System Feels Alive

The phone should maintain coherent state across applications, notifications, navigation, and time. It should feel active even when the player is focused elsewhere.

### 6. Tools Enable the Story

The project’s primary production goal is to make story authoring, implementation, validation, and debugging efficient enough that later development is dominated by narrative work and asset creation rather than repetitive engineering.

## Authoring Vision

The central authoring workflow is chronological and narrative-first.

The author should be able to open the Story Timeline and write everything that happens in the game in timestamp order. Each entry is a structured Timeline Event with fields appropriate to its type, such as:

- timestamp;
- actor and target objects;
- application or phone surface;
- content and referenced assets;
- entry conditions;
- state effects;
- branch membership;
- branch or merge relationships;
- dependencies;
- validation status.

The Timeline must remain readable as a linear sequence while allowing the author to filter by any known object or relevant dimension, including:

- character;
- app;
- conversation;
- event type;
- branch;
- variable or flag;
- asset;
- location;
- story arc, chapter, day, or time range.

The author should enter information once. Supporting systems should index, validate, disseminate, and present that information without requiring duplicate content creation in separate app-specific tools.

Domain tools may provide focused views and specialized editing where necessary, but they must operate on shared authoritative data rather than own competing copies.

## Story Timeline Vision

The Story Timeline answers: **What happens?**

It is the primary content-authoring environment and should ultimately support:

- chronological event creation and reordering;
- typed event editors;
- immediate-choice branches;
- branches selected by prior persistent state;
- branch merge points;
- persistent variables and relationship effects;
- multi-object filtering;
- global search;
- asset references and missing-asset tracking;
- dependency visualization;
- validation before runtime;
- previews of relevant phone surfaces;
- traceability from authored event to runtime result.

The Timeline is not expected to infer a complete story from unstructured narrative prose. It should make structured authoring feel natural and linear while preserving the precision required by the runtime.

## Game Director Vision

The Game Director answers: **What is happening, and why?**

It is a runtime inspection and debugging suite, separate from primary story authoring. It should ultimately allow developers to:

- inspect world, story, character, relationship, app, notification, and save state;
- view pending, completed, cancelled, expired, and blocked events;
- identify failed conditions and their current values;
- trace a runtime result back to the Timeline Event and prior state that caused it;
- jump to or replay supported timestamps and scenarios safely;
- inspect phone screens and application state;
- view branch history and state provenance;
- understand why content did or did not appear;
- compare expected and actual state during debugging.

The Game Director should prefer read-only inspection and controlled simulation operations. It must not become a second, conflicting story-authoring environment.

## Runtime Vision

The runtime is a deterministic consumer of validated, structured content.

Its responsibilities include:

- maintaining authoritative world and phone state;
- advancing and exposing game time;
- evaluating conditions;
- applying effects;
- scheduling and executing events;
- routing information to phone applications;
- managing notifications and asynchronous presentation;
- preserving state through save and load;
- exposing sufficient diagnostics for the Game Director.

Phone applications should remain modular presentation and interaction surfaces. They consume shared services and state; they do not embed independent copies of narrative progression logic.

## Technical Vision

Project Minerva will be built as a modular, data-driven, inspectable system compatible with Unity 5.6.

The technical direction prioritizes:

- explicit layer boundaries;
- small, focused responsibilities;
- composition over deep inheritance;
- definitions separated from mutable runtime state;
- event-driven communication where it reduces coupling;
- time-driven scheduling rather than unnecessary per-frame polling;
- deterministic and versionable save data;
- platform-neutral runtime logic where practical;
- editor-only tooling isolated from shipped code;
- measured performance appropriate for iOS and Windows;
- minimal reliance on third-party assets for project-defining gameplay systems.

Third-party assets may provide generic infrastructure such as serialization, text rendering, animation, editor productivity, or input abstraction, but they must be wrapped behind Minerva-owned interfaces where replacement risk or architectural coupling warrants it.

## Production Vision

Development is intentionally front-loaded toward architecture and tooling.

The approved phase structure is:

1. **Preproduction** — project constitution, repository standards, architecture, decisions, and ticket process.
2. **Foundation** — bootstrap, events, time, save abstraction, data registry, logging, configuration, serialization, and resource loading.
3. **Framework** — reusable app, screen, state-machine, timeline, selection, inspector, and undo/redo foundations.
4. **Narrative Suite** — Story Timeline, branch view, variable explorer, search, validation, and asset tracking.
5. **Vertical Slice Runtime** — minimal phone OS, lock screen, home screen, Messages, Notifications, Contacts, story runtime, and save/load.
6. **Simulation Suite** — Game Director, replay, phone preview, event queues, character monitoring, save inspection, and state provenance.
7. **Production** — full narrative authoring, implementation through supported capabilities, playtesting, and asset replacement.
8. **Polish** — performance, accessibility, UX, platform validation, content QA, automation, and release preparation.

The project should resist premature breadth. Additional phone apps and sophisticated simulation features should be added when the vertical slice or story requirements demonstrate their value.

## Vertical Slice Definition

The vertical slice is primarily a proof of the complete production pipeline.

It must demonstrate at least one meaningful sequence containing:

- chronological authored events;
- a player choice;
- a branch caused by that choice;
- persistent state that remains after branches rejoin;
- a later event or option changed by prior state;
- a Messages interaction;
- a notification;
- minimal Contact integration;
- save and load across meaningful state;
- validation of authored content;
- runtime inspection through the Game Director or its initial foundations.

The primary exit criterion is:

> A new instance of a supported story event can be authored, validated, executed, saved, loaded, and debugged without adding new C# code.

The vertical slice does not require every planned app, final art, full narrative scope, or release-level polish.

## Content Production End State

After the core tools and vertical slice are complete, the normal development loop should be:

1. Author or revise events in the Story Timeline.
2. Reference existing or placeholder content assets.
3. Validate the timeline and dependencies.
4. Run or preview the relevant scenario.
5. Inspect unexpected behavior in the Game Director.
6. Revise content or create a scoped engineering ticket only when a new reusable capability is genuinely required.
7. Replace placeholders with final visual, audio, document, or media assets as production advances.

Routine story expansion should not require manually wiring screens, duplicating app data, or adding one-off MonoBehaviours.

## Success Measures

Project Minerva will be considered technically and operationally successful when:

- the Story Timeline is the primary place where narrative content is authored;
- content can be filtered and understood across all relevant objects and branches;
- apps consistently reflect shared state;
- persistent choices reliably alter later content after branch merges;
- invalid content is caught before it creates opaque runtime failures;
- the Game Director can explain blocked or unexpected behavior;
- save/load preserves long-form narrative state safely;
- routine content production requires little or no engineering intervention;
- the Windows and iOS builds remain responsive and stable;
- new contributors can work from documentation and scoped tickets without reconstructing project intent.

## Non-Goals

Project Minerva is not currently intended to be:

- a recreation of iOS or Android;
- a general-purpose commercial game engine;
- an AI-generated freeform story simulator;
- a conventional dialogue-tree game with a phone skin;
- a collection of unrelated app minigames;
- a graphics-first showcase;
- a project whose core story logic is distributed across UI prefabs and app controllers.

Reusable frameworks are valuable, but they exist to support this game. Generalization must not displace the concrete needs of Project Minerva.

## Long-Term Outcome

The intended outcome is a maintainable narrative production platform tailored to one ambitious game.

Early development creates the architecture, tools, and vertical slice. Later development uses those tools to build the story, test branches, understand state, and add the media assets required by each narrative moment.

The finished game should feel like a coherent living phone to the player and a coherent, inspectable authoring system to the developer.