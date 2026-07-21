# Validation Framework

**Status:** Draft  
**Owner:** Technical Director

## Purpose

Provide one shared model for structural, reference, semantic, compatibility, and runtime-precondition validation across authored content, imports, saves, and editor tools.

## Responsibilities

- define validation rule and result contracts;
- classify errors, warnings, and informational findings;
- attach findings to stable entities, fields, files, and locations;
- support deterministic batch and focused validation;
- aggregate results without losing source ownership;
- expose machine-readable and human-readable reports;
- block compilation or execution when defined fatal rules fail.

## Rule Categories

- schema and required-field rules;
- identifier uniqueness and reference rules;
- condition/effect compatibility;
- branch reachability and merge consistency;
- time and schedule consistency;
- save/version compatibility;
- Unity 5.6 and editor/runtime boundary checks.

## Boundaries

The framework reports violations; owning systems and tools decide when validation runs and how approved fixes are applied. Validation must not silently repair authoritative content.

## Result Requirements

Each finding includes rule identifier, severity, message, owning entity, location when available, related identifiers, and remediation guidance when reliable.

## Extension

New systems register narrow rule providers. Rule identifiers are stable so tickets, tests, suppressions, and documentation can refer to them.