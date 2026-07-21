# Definition of Done

**Version:** 1.0  
**Status:** Draft  
**Owner:** Technical Director  
**Last Updated:** 2026-07-21  
**Related Documents:** [`Ticket_Workflow.md`](Ticket_Workflow.md), [`Code_Review_Checklist.md`](Code_Review_Checklist.md), [`Testing_Strategy.md`](Testing_Strategy.md)

## Purpose

This document defines the minimum conditions required before work is considered complete.

## Universal Requirements

Work is done only when:

- the approved scope is fully implemented;
- acceptance criteria are satisfied;
- required validation has been performed and reported honestly;
- no unauthorized files or systems were changed;
- documentation is accurate and current;
- known limitations and risks are recorded;
- the pull request is reviewable and free of unrelated changes;
- the associated ticket has an implementation report;
- no unresolved blocker is being concealed.

## Documentation Work

Documentation is done when:

- purpose, authority, audience, and related documents are clear;
- terminology matches the Glossary;
- links resolve correctly;
- status and version metadata are present;
- the document does not contradict approved architecture or implementation;
- future work is distinguished from current commitments.

## Runtime Code

Runtime code is done when:

- it is compatible with Unity 5.6 and supported platforms;
- ownership and dependency boundaries are respected;
- behavior is deterministic where required;
- failure cases are handled or surfaced clearly;
- save implications are addressed;
- focused tests or manual checks cover expected behavior;
- no UnityEditor dependency enters player code.

## Editor and Tooling Work

Tooling is done when:

- the authoring workflow is usable for the stated task;
- validation explains invalid data;
- undo/redo expectations are addressed where editing occurs;
- data is stored in authoritative project formats;
- runtime behavior can be traced to authored inputs;
- tool state does not silently become game state.

## Data and Narrative Work

Content-system work is done when:

- stable identifiers are used;
- conditions and effects are validated;
- branches and merge behavior preserve required state;
- missing references and invalid ordering are detected;
- save/load and replay implications are understood.

## Not Done

Work is not done merely because:

- code compiles;
- a happy path works once;
- a placeholder exists;
- a PR has been opened;
- the implementation appears visually correct;
- future cleanup is assumed;
- documentation says behavior exists when it has not been verified.

## Completion Decision

The Technical Director determines whether architectural and engineering work satisfies this standard. The Product Owner approves product and narrative outcomes when applicable.
