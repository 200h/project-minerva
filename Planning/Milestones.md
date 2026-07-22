# Milestones

**Last Updated:** 2026-07-22  
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

### M1.1 Unity Repository Skeleton

**Status:** Complete

- FND-001 executed through PR #5.
- Approved `Assets/Minerva/` structure created.
- Unity-focused `.gitignore` and root project guidance added.
- Unity 5.6.7f1 batch recognition completed successfully.
- FND-001 accepted and categorized under `Docs/Tickets/Complete/`.

### M1.2 Runtime Bootstrap

**Status:** Active

- Prepare and approve FND-002.
- Implement deterministic explicit service registration and lifecycle.
- Verify failure handling and reverse shutdown behavior.
- Verify repeated runtime construction without leaked static state.
- Keep event, time, save, narrative, phone, editor-tool, and platform behavior out of scope.

**Current Deliverable:** `Docs/Tickets/Ready/FND-002_Create_Runtime_Bootstrap.md`

**Exit Criteria:**

- FND-002 is approved and merged into `main` as a Ready ticket.
- The runtime bootstrap implementation is reviewed and merged.
- The ticket is moved through `Active` and `Review`, then closed under `Complete` after acceptance.
- Unity 5.6 compilation and focused lifecycle validation pass.
- No prohibited global discovery, service locator, singleton, or future-system behavior is introduced.

## Later Phase Milestones

Detailed deliverables and exit criteria for later Phase 1 work and Phases 1.5 through 6 remain authoritative in [`../Docs/Project/DEVELOPMENT_ROADMAP.md`](../Docs/Project/DEVELOPMENT_ROADMAP.md).

## Milestone Rule

A milestone is complete only when its deliverables are merged, reviewed, accepted, correctly categorized in the ticket workflow, and usable as prerequisites for the next milestone.