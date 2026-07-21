# Notification System

**Status:** Draft  
**Owner:** Technical Director

## Purpose

Own notification records, routing, delivery state, badge state, and player-facing notification commands across phone applications.

## Responsibilities

- accept validated notification requests;
- resolve target app and route;
- schedule immediate or delayed delivery through Time and Clock;
- track queued, delivered, opened, dismissed, and expired states;
- update badge/read state;
- publish completed delivery and interaction events;
- capture and restore notification state.

## Does Not Own

Narrative eligibility, global simulation time, screen navigation, message-thread state, or platform push services beyond an adapter.

## Inputs

Notification definitions or runtime requests, schedule facts, open/dismiss commands, app registry queries, and restore payloads.

## Outputs

Notification read models, delivery events, app-routing intents, diagnostics, and versioned save state.

## Invariants

- every notification has a stable identifier and valid target;
- delivery occurs at most once unless repetition is explicit;
- opening a notification does not directly mutate unrelated story state;
- badges are derived from owned unread/delivery state;
- missing routes fail visibly during validation or delivery.

## Persistence

Persist delivery status, timestamps, target identifiers, payload references, and read/dismiss state. Transient animation state is excluded.

## Extension Points

Platform notifications, grouped notifications, and richer actions may be added behind routing and delivery contracts.