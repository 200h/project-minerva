# Project Minerva — Coding Standards

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [ARCHITECTURAL_PRINCIPLES.md](ARCHITECTURAL_PRINCIPLES.md), [ENGINEERING_PHILOSOPHY.md](ENGINEERING_PHILOSOPHY.md), [AI_ENGINEERING_GUIDE.md](AI_ENGINEERING_GUIDE.md), [GLOSSARY.md](GLOSSARY.md)

## Purpose

This document defines the baseline coding standards for Project Minerva. Its purpose is to keep the codebase readable, predictable, reviewable, and compatible with Unity 5.6 across Windows and iOS.

Consistency is more valuable than individual preference. These standards may be refined through deliberate review, but contributors must not silently introduce competing conventions.

## Compatibility Baseline

All runtime and editor code must be compatible with the project’s approved Unity 5.6 environment.

Contributors must:

- Avoid Unity APIs introduced after Unity 5.6.
- Avoid C# language features unsupported by the project’s configured scripting runtime.
- Confirm compatibility before introducing framework or library syntax not already used by the repository.
- Keep editor-only code inside an `Editor` folder or an editor-only compilation boundary.
- Avoid assumptions that depend on later Unity package systems, UI Toolkit, addressable assets, the newer Input System, or other post-5.6 features.
- Treat iOS ahead-of-time compilation constraints as a design consideration.

When compatibility is uncertain, stop and document the uncertainty rather than approximating.

## Namespace Standard

The root namespace is:

```csharp
Minerva
```

Namespaces should follow stable product domains rather than temporary folder names.

Expected top-level domains include:

```text
Minerva.Core
Minerva.Data
Minerva.Events
Minerva.Runtime
Minerva.Time
Minerva.Story
Minerva.Characters
Minerva.Relationships
Minerva.Phone
Minerva.UI
Minerva.Save
Minerva.Tools
Minerva.Editor
```

Subnamespaces should be introduced only when they clarify ownership or prevent a domain from becoming crowded.

Examples:

```csharp
Minerva.Core.Bootstrap
Minerva.Story.Timeline
Minerva.Story.Branching
Minerva.Phone.Notifications
Minerva.Phone.Apps.Messages
Minerva.Editor.StoryTimeline
```

`Minerva.Tools` is reserved for development tools that may execute with the runtime in development builds. `Minerva.Editor` is reserved for Unity Editor-only code that must not ship.

## File and Type Organization

- One primary public type per file.
- The filename must match the primary type name.
- Small private helper types may share a file only when they are inseparable from the primary type.
- Public interfaces, enums, and data contracts should generally have their own files.
- Partial classes should not be used to hide excessive responsibility.
- Generated files must be clearly identified and must not be edited manually.

## Naming Conventions

### Types

Use `PascalCase` for:

- Classes
- Structs
- Interfaces
- Enums
- Delegates
- Public properties
- Methods
- Events

Interfaces use an `I` prefix:

```csharp
IEventBus
ITimeSource
ISaveProvider
```

Abstract base classes do not require a `Base` suffix unless it improves clarity.

### Fields and Variables

Use `camelCase` for:

- Parameters
- Local variables
- Private fields

Private instance fields use an underscore prefix:

```csharp
private IEventBus _eventBus;
private bool _isInitialized;
```

Serialized private fields follow the same convention:

```csharp
[SerializeField]
private GameObject _rootView;
```

Constants use `PascalCase`:

```csharp
private const int MaximumRetryCount = 3;
```

Static readonly values use `PascalCase` unless a future approved formatter enforces another standard.

### Boolean Names

Boolean names should read as true-or-false statements:

```csharp
_isInitialized
_hasUnreadMessages
_canAdvanceTime
_shouldPersist
```

Avoid vague names such as `flag`, `state`, or `enabled` without domain context.

### Events

Event types should describe facts that occurred, usually in past tense:

```csharp
TimelineEventScheduled
StoryVariableChanged
NotificationDelivered
AppOpened
```

Commands or requests should describe intent:

```csharp
OpenAppCommand
AdvanceTimeRequest
SaveGameRequest
```

Do not name a runtime event simply `Event`.

## Class Design

A class should have one clear reason to change.

Contributors should prefer:

- Small cohesive classes
- Explicit dependencies
- Composition over inheritance
- Interfaces at stable boundaries
- Plain C# data models where Unity behavior is not required
- Deterministic initialization
- Read-only access where mutation is not required

Avoid:

- God managers
- Deep inheritance hierarchies
- Hidden service lookup
- Static mutable state
- Unnecessary singletons
- Direct dependencies from simulation to UI
- Story logic embedded in views or app controllers

Manager names are reserved for true subsystem coordinators. Do not add a `Manager` suffix merely because a class contains multiple methods.

## Dependency Injection and References

Dependencies should be visible through constructors, initialization methods, serialized references, or approved composition roots.

Plain C# services should prefer constructor injection when supported by their lifecycle.

`MonoBehaviour` dependencies may use serialized references or explicit initialization. Scene searches such as `FindObjectOfType`, tag lookups, or name-based searches must not be used as the default dependency mechanism.

Service locator patterns require explicit architectural approval.

## Unity Component Standards

`MonoBehaviour` should be used only when Unity lifecycle, scene attachment, coroutines, serialization, or component behavior is required.

Do not make a class a `MonoBehaviour` merely to gain global access.

Unity lifecycle methods should remain small:

- `Awake` establishes local invariants and references.
- `OnEnable` subscribes to events when appropriate.
- `Start` performs work that requires other initialized scene objects.
- `Update` performs only necessary frame-driven work.
- `OnDisable` unsubscribes from events.
- `OnDestroy` releases owned resources.

Avoid expensive searches, allocations, reflection, or data parsing inside `Update`.

## Initialization and Shutdown

Systems with meaningful lifecycle behavior should expose explicit state and initialization contracts.

Expected properties include:

- Initialization is idempotent where practical.
- Initialization order is documented.
- A system does not publish events before it is ready.
- Shutdown or disposal removes subscriptions and releases resources.
- Initialization failure is surfaced clearly rather than leaving a partially active system.

Do not rely on incidental Unity script execution order unless documented and approved.

## Data and Serialization

Project Minerva separates definitions from runtime state.

### Definitions

Definitions are authored, stable content such as character definitions, app definitions, event definitions, and media references. Definitions may use `ScriptableObject` where Unity-native authoring is beneficial.

### Runtime State

Runtime state is mutable, saveable state such as current story variables, message delivery status, notification history, relationships, and app state. Runtime state should generally use plain serializable C# data contracts.

### Rules

- Saved data must use stable identifiers rather than scene references.
- Serialized field renames require migration consideration.
- Save contracts must be versioned.
- Runtime state must not hold editor-only object references.
- Definitions must not be mutated as a substitute for runtime state.
- Polymorphic serialization must not be assumed without an approved implementation compatible with Unity 5.6.
- Date and time representations must use the approved Minerva time model rather than platform-local clock assumptions.

## Identifiers

Content and runtime objects that require persistence or cross-reference must use stable identifiers.

Identifiers must:

- Remain stable across renames and file moves.
- Be unique within their declared domain.
- Be validated before runtime.
- Avoid using display names as identity.
- Avoid relying on Unity instance IDs for persistence.

The specific identifier format will be defined in the Data Model architecture documentation.

## Events and Subscriptions

- Subscribe and unsubscribe symmetrically.
- Prefer typed event contracts.
- Do not publish UI-specific events from simulation systems unless they represent a domain fact.
- Event handlers should be small and should delegate substantial work.
- Publishing order must not be treated as implicit business logic unless documented.
- Avoid allocating new event payloads every frame.
- Event subscriptions must not keep destroyed Unity objects alive.

## Error Handling

Errors should be handled at the level that has enough context to make a meaningful decision.

Use exceptions for exceptional failures, not routine story conditions or branch evaluation.

Expected failures should use explicit results where appropriate, particularly for:

- Validation
- Content loading
- Save migration
- Identifier resolution
- Import operations
- Tool commands

Do not catch an exception solely to suppress it. A caught exception must be handled, translated into a meaningful result, or logged with sufficient context before rethrowing.

## Logging

All significant systems should use the approved Minerva logging abstraction once available.

Log messages should include:

- The responsible system or domain
- The affected stable identifier where relevant
- The attempted operation
- The result or failure reason

Logging levels should distinguish routine diagnostics from warnings and failures.

Do not log sensitive user data, entire save payloads, or excessive per-frame output.

Temporary debugging logs must be removed or converted to controlled diagnostic logging before a ticket is complete.

## Documentation Comments

Public contracts and non-obvious architectural types should use XML documentation.

Documentation should explain intent, ownership, invariants, and side effects—not restate syntax.

Required documentation targets include:

- Public interfaces
- Public data contracts
- Extension points
- Serialization-sensitive fields or types
- Non-obvious event payloads
- Editor tool commands
- Methods with important preconditions or side effects

Private implementation details do not need exhaustive comments when the code is self-explanatory.

## Regions

Do not use regions to conceal oversized classes.

Regions may be used sparingly in Unity components to group standardized lifecycle, serialized fields, or editor-only sections when this improves navigation. A class that requires many regions should be reconsidered.

## Editor Code

Editor tooling is first-class production software.

Editor code must:

- Remain outside runtime assemblies.
- Support undo and redo for content mutations where practical.
- Mark changed assets or data as dirty through approved Unity Editor APIs.
- Avoid silently modifying unrelated assets.
- Validate inputs and present actionable errors.
- Preserve stable identifiers.
- Separate tool presentation from authoring commands and data mutation.
- Avoid destructive operations without confirmation or recoverability.

Editor windows should not become the sole location of business rules. Shared validation and data logic should live in testable non-UI classes.

## Performance

Performance decisions should be evidence-based.

Runtime priorities include:

- Avoiding unnecessary work every frame
- Reducing garbage collection pressure
- Controlling asset memory on iOS
- Using event- or time-driven updates where appropriate
- Pooling only where measurement justifies it
- Avoiding repeated parsing or reflection at runtime

Editor productivity and clarity may take priority over micro-optimization, but tools must remain responsive at anticipated production content scale.

## Testing and Validation

Every significant system should identify the most appropriate verification method:

- Unit tests for deterministic plain C# logic
- Integration tests for system boundaries
- Editor tests for import, validation, and authoring commands
- Runtime smoke tests for Unity lifecycle and presentation integration
- Content validation for references, branches, variables, and required assets

A successful compilation is not sufficient validation.

Bug fixes should include a regression test or durable validation rule where practical.

## Code Formatting

Until an automated formatter is approved:

- Use four spaces for indentation.
- Do not use tabs for indentation.
- Place opening braces on a new line for types, methods, properties, and control blocks.
- Use braces for conditional and loop bodies, including single-line bodies.
- Keep lines readable; split complex expressions rather than enforcing a rigid character limit.
- Separate logical sections with one blank line.
- Remove trailing whitespace.
- End text files with a newline.

## Source Control Standards

- Make focused commits with clear intent.
- Do not combine unrelated formatting, refactoring, and feature work.
- Do not commit generated caches, local settings, secrets, or build output.
- Do not modify files outside ticket authority.
- Do not rewrite shared branch history without explicit approval.
- Keep Unity `.meta` files paired with their assets.
- Treat accidental `.meta` regeneration as a review concern.

Commit messages should be concise and describe the completed change, for example:

```text
docs: define event model
feat: add typed event subscription
fix: preserve story variable during branch merge
```

The detailed Git workflow will be defined in `Docs/Handbook/`.

## Review Expectations

Code review should verify:

- Ticket scope and authorized files
- Architectural ownership
- Unity 5.6 compatibility
- Dependency direction
- Save and identifier stability
- Subscription cleanup
- Validation and failure behavior
- Tests or verification evidence
- Documentation changes
- Absence of unrelated refactoring

Readable code that clearly satisfies the approved design is preferred over a clever implementation that requires explanation.

## Deviations

A contributor may propose a deviation when these standards create a concrete technical problem. The proposal must explain:

- The rule being challenged
- The specific problem
- The proposed alternative
- Compatibility and maintenance consequences
- Whether the change should apply locally or update the standard

Do not silently deviate.
