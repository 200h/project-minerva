# Event Architecture

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21

## Purpose

Define how systems announce completed facts without creating hidden ownership or untraceable behavior.

## Event Types

- **Domain events** describe completed authoritative changes, such as a story variable changing or a notification being delivered.
- **Lifecycle events** describe startup, load, pause, resume, and shutdown transitions.
- **Presentation events** request or describe cosmetic behavior and must not determine story outcomes.
- **Diagnostic events** expose validation or runtime observations for tooling and tests.

## Rules

- Events are immutable after publication.
- Names use completed-fact language.
- Publishers own the meaning and schema of their events.
- Subscribers cannot rely on undocumented ordering between unrelated event types.
- Request/response behavior uses direct interfaces or commands, not paired events.
- Events must not carry mutable internal collections or concrete view references.
- Exceptions in one subscriber must not silently prevent unrelated subscribers from receiving a fact.

## Ordering

Events produced by one command are published in a documented deterministic order. Nested publication should be queued and drained predictably rather than recursively mutating the simulation without bounds.

## Subscription Lifecycle

Subscriptions must be explicit and disposable. Scene or screen teardown must remove transient subscribers. Long-lived services register during composition and unregister during shutdown.

## Diagnostics

The bus should support development-time inspection of event type, publisher, simulation timestamp, correlation identifier, and subscriber failures. Diagnostics must not change event behavior.

## Persistence

The event bus is not an event store. Save data captures owned system state and necessary execution records. Events may provide provenance but are not assumed to reconstruct all state unless a future ADR adopts event sourcing.

## Validation

Tests should verify deterministic ordering, safe unsubscribe behavior, exception handling, and absence of editor-only types in runtime contracts.