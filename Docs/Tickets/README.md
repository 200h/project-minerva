# Project Minerva Tickets

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`../Handbook/Ticket_Workflow.md`](../Handbook/Ticket_Workflow.md), [`../Templates/Ticket_Template.md`](../Templates/Ticket_Template.md), [`../Project/AI_ENGINEERING_GUIDE.md`](../Project/AI_ENGINEERING_GUIDE.md)

## Purpose

This directory contains repository-authoritative work tickets for human and AI implementation.

## Workflow Directories

- `Backlog/` — candidate work not yet implementation-ready.
- `Ready/` — approved, specified, and unblocked work.
- `Active/` — work currently being implemented.
- `Review/` — completed implementation awaiting acceptance and merge.
- `Complete/` — merged and accepted work retained as project history.

A ticket exists in exactly one workflow directory at a time.

## Naming

Use `<PREFIX>-<NNN>_<concise-slug>.md`, for example:

`ARCH-001_define-runtime-layer-boundaries.md`

## Authority

A ticket authorizes only the work it explicitly permits. Search results, adjacent code, optional context, and expected-file lists do not expand modification authority.

## Creating Tickets

Copy [`../Templates/Ticket_Template.md`](../Templates/Ticket_Template.md), assign the next domain identifier, and begin in Backlog. Move to Ready only after the readiness requirements in the Ticket Workflow are satisfied.

## Codex Prompt

```text
Implement <TICKET-ID> from <exact ticket path>.

Follow the ticket’s Required Context, authority boundaries, acceptance criteria,
and implementation-report format. Do not read or modify unrelated areas.
Stop and report if a defined stop condition occurs.
```

## Completion History

Completed tickets are retained so future contributors can understand what was authorized, implemented, validated, and deferred.
