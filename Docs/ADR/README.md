# Project Minerva Architecture Decision Records

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`../Project/ARCHITECTURAL_PRINCIPLES.md`](../Project/ARCHITECTURAL_PRINCIPLES.md), [`../Templates/ADR_Template.md`](../Templates/ADR_Template.md)

## Purpose

Architecture Decision Records preserve significant technical and production decisions, their context, alternatives, and consequences.

## Status Values

- **Proposed** — drafted for review.
- **Accepted** — approved and authoritative.
- **Superseded** — replaced by a newer ADR.
- **Rejected** — considered but not adopted.
- **Deprecated** — no longer applicable, with rationale recorded.

## Naming

Use four-digit sequential identifiers:

`NNNN_Concise_Decision_Title.md`

Identifiers are never reused, even when an ADR is superseded.

## When an ADR Is Required

Create an ADR for decisions that materially affect:

- architectural layers or dependency direction;
- authoritative state ownership;
- persistence or save compatibility;
- engine, platform, or language constraints;
- third-party dependency strategy;
- primary authoring workflows;
- major tool/runtime boundaries;
- AI implementation governance.

## Process

1. Copy the ADR template.
2. Assign the next identifier.
3. Draft context, decision, alternatives, and consequences.
4. Review against the Constitution.
5. Mark Accepted only after approval.
6. Link affected architecture, systems, tools, and tickets.
7. Supersede rather than erase decisions when direction changes.

## Initial Record

The first ADRs formalize decisions already approved through Project Minerva Constitution v1.0.
