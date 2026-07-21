# Project Minerva — Engineering Philosophy

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`README.md`](README.md), [`PROJECT_MANIFESTO.md`](PROJECT_MANIFESTO.md), [`PROJECT_VISION.md`](PROJECT_VISION.md), [`ARCHITECTURAL_PRINCIPLES.md`](ARCHITECTURAL_PRINCIPLES.md), [`AI_ENGINEERING_GUIDE.md`](AI_ENGINEERING_GUIDE.md), [`GLOSSARY.md`](GLOSSARY.md)

## Purpose

This document defines how Project Minerva approaches software engineering. It complements the Architectural Principles by describing the expected habits, tradeoffs, and standards of judgment used while designing, implementing, reviewing, debugging, and maintaining the project.

The goal is not to maximize abstraction or ceremony. The goal is to create a codebase and toolchain that remain understandable, testable, and productive throughout a long-term narrative game project.

## 1. Build for Production, Not for Demonstration

A prototype may prove that an idea is possible. Project Minerva must prove that the idea can be produced repeatedly.

Systems should be evaluated by how they support the full content pipeline, not only whether they produce a successful first example. The first conversation, notification, branch, or save file is useful only if it helps establish a reliable pattern for every future instance.

## 2. Tooling Is Product Work

Developer tools are not secondary utilities added after gameplay. They are part of the production system.

The Story Timeline, validators, branch views, state inspectors, asset browsers, previews, and Game Director must receive the same architectural care as runtime features because they directly determine the cost and reliability of building the game.

A tool is valuable when it reduces repeated work, prevents errors, improves understanding, or shortens the path from authored content to verified runtime behavior.

## 3. Design Before Implementation

Significant systems require a written statement of purpose, responsibilities, boundaries, inputs, outputs, dependencies, and extension points before production implementation begins.

Documentation should be proportionate to the decision. Small helpers do not require standalone design documents. Systems that own state, establish interfaces, affect saves, define editor workflows, or influence multiple modules do.

Implementation tickets should execute an approved design rather than invent one implicitly.

## 4. Prefer the Smallest Complete Design

Build the smallest architecture that fully supports the known requirements and credible near-term extension points.

Avoid both extremes:

- one-off code that cannot survive the vertical slice;
- speculative frameworks designed for hypothetical games or features with no approved need.

A design should be simple enough to understand and strong enough to extend.

## 5. Readability Is a Performance Feature

Clear code reduces implementation errors, review time, debugging cost, and context required by future contributors and AI agents.

Prefer explicit names, focused methods, clear ownership, predictable control flow, and unsurprising data structures. Cleverness is not a substitute for clarity.

Code should communicate why a responsibility exists, not merely how it executes.

## 6. Keep Classes Focused

A class should have a coherent reason to change.

Large managers, broad utility classes, and multi-purpose controllers are warning signs. Split responsibilities when a class begins to own unrelated state, policy, presentation, persistence, and orchestration.

Small classes are not an end in themselves. Fragmentation that forces readers to jump across many trivial wrappers is also undesirable. Favor cohesive units with meaningful boundaries.

## 7. Composition Before Inheritance

Use services, policies, strategies, handlers, and data composition to assemble behavior.

Inheritance should express a stable substitutable relationship. It should not be used to share incidental code or force all event types, applications, or screens into a single deep hierarchy.

Prefer interfaces that describe capability over base classes that dictate internal implementation.

## 8. Make State Ownership Obvious

Every mutable value should have a clear owner.

Other systems may query, observe, or request changes through defined contracts, but they should not mutate internal state casually. Shared mutable state without ownership is a major source of inconsistent saves, hidden dependencies, and debugging difficulty.

When ownership is unclear, stop and resolve the model before adding more behavior.

## 9. Debuggability Is Part of Completion

A feature is not complete merely because it works under ideal conditions.

Important systems should expose enough information to answer:

- What is the current state?
- What changed it?
- Which conditions were evaluated?
- Why did an event execute or fail?
- What work is pending?
- What data was loaded or saved?

Logging, state inspection, validation output, and deterministic reproduction should be designed alongside the feature.

## 10. Validate Early and Specifically

Prefer precise validation errors near the authoring source over generic runtime failures.

A useful error identifies the affected object, the violated rule, the expected value or relationship, and a likely correction path. Warnings should be actionable and should not overwhelm authors with noise.

Validation should distinguish between blocking errors, review-worthy warnings, and informational diagnostics.

## 11. Fail Clearly

Silent failure is unacceptable for story-critical behavior.

When a system cannot satisfy its contract, it should report the failure at the appropriate boundary with enough context to diagnose it. Recovery may be appropriate, but recovery must not conceal corrupted or contradictory state.

Runtime resilience and developer visibility should be balanced deliberately.

## 12. Refactor With Evidence

Refactor when code is blocking new work, producing repeated defects, violating ownership, duplicating meaningful logic, or making a system difficult to understand or test.

Do not refactor unrelated areas during a focused ticket merely because an alternative style is preferred. Broad cleanup expands risk and makes review less reliable.

Refactoring should preserve behavior unless the ticket explicitly authorizes behavior changes.

## 13. Tests Protect Contracts

Tests should focus on rules that must remain stable: state transitions, conditions, scheduling, serialization, migration, branch behavior, identifier resolution, validation, and deterministic execution.

Not every Unity-facing visual behavior requires a unit test. Use the most appropriate verification method, including unit tests, integration tests, editor tests, validation fixtures, play-mode tests, and documented manual checks.

A test should communicate the contract it protects.

## 14. Determinism Is Preferred

Given the same authored content, initial state, player actions, and time progression, systems should produce the same result unless intentional randomness is part of the design.

Random behavior must use controlled sources and record enough information to reproduce failures when story or save state depends on it.

Determinism improves testing, replay, debugging, and confidence in branching behavior.

## 15. Data Migrations Are Normal Engineering Work

Data formats and save models will evolve. Changes must account for existing authored content, editor assets, and saved games according to the project’s current compatibility commitments.

Do not assume a field rename or structural change is harmless because the project is early. Establish stable identifiers, versions, and migration patterns before content volume makes changes expensive.

## 16. Performance Starts With Architecture

Good performance begins by avoiding pathological design:

- unnecessary per-frame polling;
- repeated global searches;
- avoidable allocations in hot paths;
- excessive object churn;
- redundant serialization;
- duplicated derived data;
- unbounded event subscriptions;
- needless UI rebuilds.

Measure before applying low-level optimization. Preserve clarity unless profiling demonstrates a meaningful need.

## 17. Mobile Constraints Are First-Class

The iOS target requires disciplined memory use, responsive UI, predictable lifecycle handling, careful asset loading, and efficient background simulation.

Desktop hardware must not hide architectural problems. Features should be assessed against the lower-powered target throughout development rather than postponed until final optimization.

## 18. Unity 5.6 Assumptions Must Be Verified

Contributors must not rely on contemporary Unity APIs, package behavior, C# language features, serialization features, editor APIs, or platform support without confirming Unity 5.6 compatibility.

When documentation or examples are based on modern Unity versions, adapt the design intentionally rather than copying them directly.

Compatibility limitations should be documented where they affect architecture or workflow.

## 19. Third-Party Code Is a Dependency, Not an Owner

External assets may solve generic problems efficiently. They should not dictate core narrative architecture unless explicitly approved.

Wrap important dependencies where practical, document why they exist, pin compatible versions, and identify replacement risk. Do not distribute vendor-specific calls throughout unrelated systems.

The project should remain intelligible even to a contributor who is unfamiliar with the asset.

## 20. Tickets Define Authority

Implementation work begins from a reviewed ticket that defines the goal, context, scope, authorized files, prohibited actions, dependencies, acceptance criteria, and expected report.

A ticket is not permission to improve adjacent systems. If implementation reveals an architectural conflict or missing prerequisite, the engineer should report it rather than silently broadening scope.

## 21. Context Is Curated

Human and AI contributors should read the minimum authoritative context required to perform a task correctly.

Tickets specify required and optional reading. Contributors should not load the entire repository or all documentation by default. Context limits are a design constraint and a measure of modularity.

Documentation should be structured so that a focused task can be understood without reconstructing the entire project history.

## 22. Reviews Protect the System

Code review is not limited to syntax and correctness. Review should assess:

- architectural alignment;
- ownership and dependency direction;
- scope discipline;
- Unity 5.6 compatibility;
- save and data implications;
- validation and debugging support;
- tests or verification evidence;
- documentation impact;
- future content-production cost.

Review comments should distinguish blocking defects from optional improvements.

## 23. Completion Includes Knowledge Transfer

A completed system should leave the repository easier to understand.

Where applicable, completion includes updated documentation, examples, tests, validation rules, inspector support, and a concise implementation report. The project must not depend on the memory of the person or AI session that wrote the code.

## 24. Technical Debt Is Recorded, Not Hidden

Shortcuts may sometimes be appropriate, particularly during experiments. When accepted, they must be explicit, bounded, and recorded with their consequence and intended resolution point.

Temporary code that is not identified becomes permanent by accident.

## 25. Protect Creative Throughput

Engineering exists to support the finished narrative experience.

When two technically sound approaches are available, prefer the one that makes story authoring, iteration, validation, preview, and debugging faster and safer without compromising the project’s architectural principles.

The ultimate engineering measure is not the sophistication of the framework. It is the team’s ability to produce and maintain a rich, coherent story through the tools.

## Decision Heuristics

When choosing between approaches, ask:

1. Which option has the clearest owner and dependency direction?
2. Which option keeps authored content in one authoritative place?
3. Which option is easier to validate and inspect?
4. Which option reduces repeated work for future content?
5. Which option is simplest while satisfying known requirements?
6. Which option can be verified under Unity 5.6 on Windows and iOS?
7. Which option creates the smallest focused context for future maintenance?

If an answer requires changing established architecture, document and review that decision before implementation.