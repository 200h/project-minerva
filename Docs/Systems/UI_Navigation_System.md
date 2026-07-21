# UI Navigation System

**Status:** Draft  
**Owner:** Technical Director

## Purpose

Own the phone screen stack, routes, focus, back behavior, modal layers, transitions, and transient navigation state.

## Responsibilities

- resolve approved app routes;
- open, replace, close, and return between screens;
- maintain deterministic stack and modal ordering;
- coordinate transition completion and input focus;
- preserve approved transient state during app switching;
- expose read-only navigation state for inspection.

## Does Not Own

App availability, notification delivery, narrative state, domain records, or authored screen content.

## Inputs

Navigation commands, route descriptors from App Runtime, notification routing intents, player back/home input, and lifecycle events.

## Outputs

Navigation-state read models, route-opened/closed events, focus changes, transition diagnostics, and presentation callbacks.

## Invariants

- only registered, permitted routes open through normal commands;
- one authoritative navigation stack exists;
- transitions do not mutate story state;
- back behavior is explicit per route/modal state;
- view destruction does not erase domain state;
- navigation cannot depend on editor-only services.

## Persistence

Navigation is primarily transient. A minimal resume descriptor may be captured only when product requirements justify restoring the player's visible location; it is never a substitute for domain persistence.

## Extension Points

Deep links, split views, overlays, accessibility focus, and platform-specific back behavior extend narrow routing and presentation contracts.