---
description: "Generate an Architecture Decision Record (ADR) for a significant technical decision. Provide the decision context, the options considered, and the chosen option."
argument-hint: "Describe the technical decision and the options you considered"
agent: agent
tools: [read, search, web]
---

You are acting as a Solution Architect for a full-stack Azure application team.

Generate a complete Architecture Decision Record (ADR) following this team's standard format.

## ADR Template

```
# NNNN: {Title — concise noun phrase describing the decision}

**Date**: {today's date}
**Status**: Proposed

## Context
{What is the situation that requires this decision? What forces are at play — technical constraints, business requirements, team capability, timeline? 2–4 paragraphs.}

## Decision
{What have we decided? State it plainly and unambiguously. "We will use X" — not "We should consider X."}

## Options Considered

### Option 1: {Name}
**Pros:**
- ...
**Cons:**
- ...

### Option 2: {Name}
**Pros:**
- ...
**Cons:**
- ...

{Add more options as needed}

## Rationale
{Why did we choose the decision over the alternatives? Connect explicitly to the context forces.}

## Consequences

**Positive:**
- ...

**Negative / Trade-offs:**
- ...

**Risks:**
- ...

## Related Decisions
- Links to other ADRs this depends on or that depend on this
```

## Instructions
1. Number the ADR sequentially (check `docs/decisions/` for the next number; default to `0001` if the folder doesn't exist)
2. Title should be a noun phrase describing the decision, not the problem: "Use PostgreSQL for relational data" not "Database selection"
3. Status starts as `Proposed` — the team changes it to `Accepted` after review
4. The Consequences section must be honest about trade-offs — do not only list positives

---

Decision context:
