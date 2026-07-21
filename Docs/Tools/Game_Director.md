# Game Director

**Status:** Draft  
**Owner:** Technical Director

## Purpose

Provide runtime inspection, explanation, replay, and controlled debugging for the active simulation.

## Authority

Game Director observes authoritative runtime state through inspection contracts. It is not a source of authored narrative definitions.

## Core Capabilities

- inspect world variables and provenance;
- inspect current simulation time and scheduled work;
- inspect narrative eligibility, executed events, choices, branches, and merge state;
- inspect app, notification, navigation, and save/load state;
- filter by entity, system, timestamp, correlation identifier, and severity;
- explain why an event did or did not occur;
- replay approved histories or fixtures;
- issue explicit development-only commands when authorized;
- navigate to the related Story Timeline definition.

## Inputs

Read-only system inspection interfaces, diagnostic event streams, save snapshots, test fixtures, and explicit debug-command contracts.

## Outputs

Inspector views, filtered traces, explanation reports, replay results, and clearly labeled temporary debug commands.

## Boundaries

- cannot silently edit source definitions;
- cannot become required for normal runtime operation;
- cannot bypass owner-system validation;
- debug changes must be explicit, attributable, and reversible or reloadable;
- editor-only code must not ship in player builds.

## Reliability

Displayed state must identify source system and timestamp. Stale snapshots must be labeled. Missing data should be reported as unavailable rather than inferred.

## Integration

Uses Shared Editor Framework for selection and cross-navigation and consumes Validation Framework diagnostics. Shared views do not merge authority with Story Timeline.