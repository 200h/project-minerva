# Risks

**Last Updated:** 2026-07-21  
**Owner:** Technical Director

## Active Risks

### RISK-001 — Unity 5.6 Compatibility

**Likelihood:** High  
**Impact:** High

Modern examples, packages, and APIs may not work in Unity 5.6.

**Mitigation:** Verify APIs, pin dependencies, document runtime constraints, and require compatibility review in tickets and PRs.

### RISK-002 — Tooling Scope Expansion

**Likelihood:** Medium  
**Impact:** High

Story Timeline and Game Director may grow into unfocused all-purpose tools.

**Mitigation:** Maintain separate responsibilities, write tool specifications, and phase features against concrete production workflows.

### RISK-003 — Authoring Schema Rigidity

**Likelihood:** Medium  
**Impact:** High

Early event schemas may not support later narrative needs without disruptive migration.

**Mitigation:** Use stable identifiers, versioned data, representative vertical-slice content, and explicit extension points.

### RISK-004 — AI Scope Drift

**Likelihood:** Medium  
**Impact:** High

Codex may inspect or modify unrelated areas when tickets are broad or ambiguous.

**Mitigation:** Enforce curated context, authorized paths, stop conditions, diff review, and factual implementation reports.

### RISK-005 — iOS Lifecycle and Toolchain Constraints

**Likelihood:** Medium  
**Impact:** High

Old Unity behavior may conflict with current iOS build requirements or lifecycle expectations.

**Mitigation:** Investigate early, isolate platform services, and schedule device/build validation before production content depends on assumptions.

### RISK-006 — Save Compatibility

**Likelihood:** Medium  
**Impact:** High

Changing schemas or identifiers may invalidate long-running story state.

**Mitigation:** Design versioned saves and migrations before production content scales.
