# Documentation Standards

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`../Project/README.md`](../Project/README.md), [`../Project/GLOSSARY.md`](../Project/GLOSSARY.md)

## Purpose

This document defines how Project Minerva documentation is structured, written, linked, reviewed, and maintained.

## Required Metadata

Long-lived documents should include:

- Version
- Status
- Owner
- Last Updated
- Related Documents

Templates may add fields appropriate to their document type.

## Status Values

- **Draft** — under active review and not yet authoritative.
- **Approved** — accepted and authoritative within its scope.
- **Deprecated** — retained for history but superseded by a named replacement.

## Writing Principles

Documentation should be:

- explicit about purpose and authority;
- written in stable project terminology;
- specific enough to guide implementation;
- concise enough to remain usable;
- honest about uncertainty and future work;
- free of claims that have not been verified.

## Structure

Use descriptive headings and short sections. State responsibilities, boundaries, dependencies, failure behavior, persistence concerns, and extension points when relevant.

## Links and Paths

Use repository-relative Markdown links. Link to authoritative documents rather than duplicating their contents. Paths in prose should use exact repository casing.

## Authority and Duplication

Each important rule or contract should have one authoritative home. Other documents should summarize and link to it. Duplicated rules create drift and should be removed during review.

## Diagrams

Diagrams must have a text explanation and should use formats that can be version controlled. A diagram is supporting evidence, not a substitute for defining contracts.

## Historical Material

Superseded work belongs in `Docs/Archive/` or remains in place with Deprecated status and a replacement link. Do not silently rewrite history when rationale matters.

## Documentation With Code Changes

A code or tooling change must update affected documentation in the same PR when it changes:

- public contracts;
- state ownership;
- data schemas;
- save behavior;
- authoring workflows;
- validation rules;
- supported platforms or dependencies.

## Review Questions

Reviewers should ask:

- Is this the correct authoritative location?
- Does it conflict with the Constitution or an ADR?
- Are current behavior and future intent clearly separated?
- Are terms used consistently?
- Will a contributor know what to do and what not to do?
