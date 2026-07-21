# Initialization and Lifecycle

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21

## Purpose

Define predictable startup, operation, pause, save/load, resume, and shutdown behavior.

## Startup Phases

1. Validate environment and project configuration.
2. Create platform adapters and core services.
3. Load and validate compiled authored definitions.
4. Construct simulation systems through the composition root.
5. Register long-lived subscriptions.
6. Create a new session or restore a save atomically.
7. Start simulation time and scheduled processing.
8. Initialize phone applications and presentation.
9. Publish a completed runtime-ready lifecycle event.

A later phase must not begin when a required earlier phase fails.

## New Game

A new game creates initial state from versioned defaults and authored starting definitions. Defaults are deterministic and do not depend on editor scene state.

## Save Load

Loading suspends simulation processing, validates the save envelope, migrates data when required, restores systems in dependency order, rebuilds derived views, resumes subscriptions, and only then exposes the restored session to presentation.

Failed load must leave the existing session intact or return to a known safe state.

## Pause and Resume

Pause reasons are explicit. Presentation pause, application backgrounding, debug pause, and simulation pause are not assumed to be identical. Time progression and delayed work follow documented platform policy.

## Application Backgrounding

On iOS or desktop suspension, platform adapters notify lifecycle coordination. The system records the necessary timestamp and state for deterministic catch-up according to the Time and Clock specification.

## Shutdown

Shutdown stops new commands, flushes approved persistence work, disposes transient and long-lived subscriptions, releases platform resources, and reports failures. Systems must not rely on arbitrary Unity destruction order.

## Reinitialization

Development reload and tests must be able to construct and dispose a runtime session without leaked static mutable state or duplicate subscriptions.

## Diagnostics

Lifecycle transitions include phase, correlation identifier, simulation timestamp, duration, and failure details for Game Director and tests.