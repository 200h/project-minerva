# Unity Execution Model

**Version:** 1.0
**Status:** Draft
**Owner:** Technical Director
**Last Updated:** 2026-07-24
**Related Documents:** [`../Runbooks/UNITY_5_6_VALIDATION.md`](../Runbooks/UNITY_5_6_VALIDATION.md), [`../Project/AI_ENGINEERING_GUIDE.md`](../Project/AI_ENGINEERING_GUIDE.md), [`../Handbook/Ticket_Workflow.md`](../Handbook/Ticket_Workflow.md)

## Purpose

This document assigns implementation and Unity validation to environments that actually possess the required capabilities. It prevents an unavailable cloud toolchain from being mistaken for a code failure and prevents unverified Unity work from being accepted.

The existing ticket lifecycle remains:

```text
Backlog → Ready → Active → Review → Complete
```

The existing acceptance and merge roles do not change.

## Work Classification

| Work type | Default implementation | Required validation |
|---|---|---|
| Documentation and planning | Cloud or local | Markdown, links, repository checks |
| Static non-Unity code | Cloud or local | Available compiler/static checks defined by ticket |
| Unity runtime code or EditMode tests | Cloud or local | Local Unity 5.6.7f1 verification |
| Editor tooling, scenes, prefabs, or imported assets | Local | Local Unity plus ticket-specific visual/manual checks |
| Platform build or device behavior | Local or dedicated build machine | Target-platform build/device validation |

Ticket authority may choose a different environment, but it must name the capability and validation owner explicitly.

## Required Environment Declaration

Before Unity-related implementation begins, the handoff records:

```text
Implementation environment:
Unity execution environment:
Unity version:
Unity executable:
Validation responsibility:
```

Example:

```text
Implementation environment: Codex cloud
Unity execution environment: Repository Owner local Mac
Unity version: 5.6.7f1
Unity executable: /Applications/Unity/Unity.app/Contents/MacOS/Unity
Validation responsibility: Local Unity Verification Operator
```

The implementation agent also reports:

```text
Repository editing:
C# compiler:
Unity 5.6.7f1:
Unity tests:
```

Unavailable capability is a routing fact, not evidence that the implementation failed.

## Execution Phases

### Cloud or Local Implementation

The Implementation Engineer:

- implements the approved ticket;
- creates or updates tests;
- performs available static checks;
- reports unavailable validation accurately;
- opens the pull request;
- moves the ticket to `Review`;
- hands off the exact PR, branch, and SHA.

The engineer must not imply that Unity validation occurred when Unity was unavailable.

### Local Unity Verification

The Local Unity Verification Operator:

- checks out the exact review head;
- runs the preflight;
- runs the standard verifier;
- preserves and reports evidence;
- reports failures without redesigning the solution;
- may update ticket or PR validation evidence when explicitly authorized.

The operator may not:

- silently repair code;
- alter requirements;
- accept the implementation;
- move the ticket to `Complete`;
- merge the pull request.

When validation fails, the ticket remains in `Review` and returns to the Implementation Engineer with exact evidence.

### Independent Review and Acceptance

After Unity validation evidence exists:

1. the Implementation Review Agent reviews the current head, implementation, scope, and evidence;
2. the Technical Director evaluates the recommendation and accepts or rejects the work;
3. accepted work moves to `Complete`;
4. the Repository Owner retains manual merge authority.

## Required Review Handoff

A Unity implementation handoff includes:

- ticket path and current status;
- PR number and URL;
- branch;
- exact head SHA;
- implementation environment and capabilities;
- Unity execution environment;
- changed paths;
- expected Unity test scope;
- validation already performed;
- validation still pending;
- the standard command:

  ```bash
  ./Tools/Verification/verify-unity56-editmode.sh
  ```

## Evidence Integrity

An execution claim progresses through timestamped states:

```text
Planned → In Progress → Committed → Verified
```

`Verified` requires inspection of the resulting repository state or generated evidence. A process exit code without the required results XML is not verified Unity test success.

Workflow timestamps use `America/New_York`.

## Execution Paths

Project Minerva continues to support:

- Path A — GitHub-native mutation;
- Path B — local Codex workspace mutation and Git push;
- Path C — human local mutation and Git push.

Unity execution normally uses a local path because cloud repository access does not imply access to the Repository Owner's installed Unity editor.

## Operational Principle

> A blocked integration path is not automatically a blocked task when another authorized path has the required capability.

Routing must remain explicit. Agents must not claim unavailable execution, and they must not broaden authority merely because a different environment is used.
