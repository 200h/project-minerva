# Project Minerva Systems

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21

## Purpose

System specifications define one runtime owner's responsibilities, contracts, dependencies, state, persistence, diagnostics, and extension points. They translate architecture into implementable boundaries without prescribing ticket-level code.

## Initial Systems

- [`World_State_System.md`](World_State_System.md)
- [`Event_Bus_System.md`](Event_Bus_System.md)
- [`Time_And_Clock_System.md`](Time_And_Clock_System.md)
- [`Narrative_Runtime_System.md`](Narrative_Runtime_System.md)
- [`Save_Load_System.md`](Save_Load_System.md)
- [`Notification_System.md`](Notification_System.md)
- [`App_Runtime_System.md`](App_Runtime_System.md)
- [`UI_Navigation_System.md`](UI_Navigation_System.md)

## Specification Rule

A system specification grants no implementation authority by itself. Implementation requires a Ready ticket with exact context, files, acceptance criteria, and validation.

## Cross-System Rule

When a workflow spans multiple systems, each system retains ownership of its state. Coordination uses narrow commands, queries, application services, and typed events.