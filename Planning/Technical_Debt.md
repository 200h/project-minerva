# Technical Debt

**Last Updated:** 2026-07-21  
**Owner:** Technical Director

## Purpose

This register tracks intentional engineering compromises that create future maintenance cost. It is not a general wish list or substitute for defect tickets.

## Current Debt

No implementation debt has been accepted because production code has not begun.

## Entry Format

Each future entry should include:

- identifier;
- date introduced;
- affected systems or files;
- reason the compromise was accepted;
- operational or production cost;
- risk of leaving it unresolved;
- proposed repayment milestone;
- related ticket or ADR.

## Rules

- Debt must be explicit, not inferred from poor quality.
- A workaround without a repayment plan is a defect or architectural risk, not accepted debt.
- Debt should be reviewed at milestone boundaries.
- Documentation debt counts when stale guidance can misdirect implementation.
