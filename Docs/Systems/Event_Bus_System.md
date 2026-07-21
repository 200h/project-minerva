# Event Bus System

**Status:** Draft  
**Owner:** Technical Director

## Purpose

Provide typed, deterministic publication of completed runtime facts without granting subscribers mutation authority.

## Responsibilities

- register typed subscribers;
- publish immutable events;
- queue nested publication predictably;
- isolate and report subscriber failures;
- dispose subscriptions safely;
- expose development diagnostics.

## Does Not Own

Commands, request/response workflows, authoritative domain state, persistence, or business rules.

## Inputs

Typed event instances and subscription registrations.

## Outputs

Ordered subscriber delivery, failure diagnostics, and optional inspection records.

## Invariants

- publication order is deterministic within one command;
- subscriptions can be removed idempotently;
- one subscriber failure does not silently cancel unrelated delivery;
- runtime contracts contain no editor-only types;
- event payloads do not expose mutable internals.

## Persistence

None. The bus is not an event store.

## Diagnostics

Event type, publisher, correlation identifier, simulation timestamp, delivery count, duration, and subscriber errors.

## Extension Points

Optional tracing and filtered inspection may be added without changing domain event semantics.