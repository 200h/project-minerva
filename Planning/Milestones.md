# Milestones

**Last Updated:** 2026-07-23  
**Owner:** Product Owner and Technical Director

## Phase 0 — Preproduction

### M0.1 Constitution Approved

**Status:** Complete

- Project identity, principles, execution standards, and AI guardrails merged.

### M0.2 Workflow Foundation

**Status:** Complete

- Handbook approved.
- Templates approved.
- Ticket-state directories established.
- Initial ADRs accepted.
- Planning controls established.

### M0.3 Architecture Baseline

**Status:** Complete

- Repository and Unity folder architecture documented.
- Layer and dependency model approved.
- Core system boundaries documented.
- Context indexes created.

### M0.4 Implementation Readiness

**Status:** Complete

- First Codex ticket prepared and reviewed.
- Unity project skeleton scope, authority, exclusions, and validation defined.
- Unity project constraints recorded.
- Ticket-driven implementation workflow exercised successfully.

## Phase 1 — Foundation

Phase 1 work is organized through the [Foundation Runtime Services epic](Epics/Foundation_Runtime_Services.md). The epic defines the multi-ticket dependency sequence; milestones remain demonstrable checkpoints.

### M1.1 Unity Repository Skeleton

**Status:** Complete

- FND-001 executed through PR #5.
- Approved `Assets/Minerva/` structure created.
- Unity-focused `.gitignore` and root project guidance added.
- Unity 5.6.7f1 batch recognition completed successfully.
- FND-001 accepted and categorized under `Docs/Tickets/Complete/`.

### M1.2 Runtime Bootstrap

**Status:** Complete

- FND-002 prepared, implemented, independently reviewed, accepted, and merged through PR #7.
- Deterministic explicit service registration and lifecycle established.
- Failure handling and reverse shutdown behavior validated.
- Repeated runtime construction validated without leaked static state.

### M1.3 Foundation Runtime Services

**Status:** Active

**Epic:** [Foundation Runtime Services](Epics/Foundation_Runtime_Services.md)

- Implement generic core event publication and subscription.
- Establish explicit runtime composition ownership.
- Define deterministic runtime clock abstractions.
- Add a generic scheduled-task queue.
- Define constrained runtime state primitives.
- Define versioned save-snapshot contracts without persistence.

**Current Deliverable:** `Docs/Tickets/Ready/FND-003_Create_Core_Event_Bus.md`

**Exit Criteria:**

- FND-003 through FND-008 are accepted and merged.
- Generic event, composition, timing, scheduling, state, and snapshot contracts are validated in Unity 5.6.
- No narrative, phone, application, UI, platform, persistence, or editor behavior is introduced prematurely.
- Foundation services remain explicitly composed without scene discovery, global service locators, or reflection-based registration.

## Later Phase Milestones

Detailed deliverables and exit criteria for later Phase 1 work and Phases 1.5 through 6 remain authoritative in [`../Docs/Project/DEVELOPMENT_ROADMAP.md`](../Docs/Project/DEVELOPMENT_ROADMAP.md).

## Milestone Rule

A milestone is complete only when its deliverables are merged, reviewed, accepted, correctly categorized in the ticket workflow, and usable as prerequisites for the next milestone.
