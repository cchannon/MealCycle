---
description: "Use when writing functional requirements, user stories, use cases, acceptance criteria, BRD sections, or product specifications. Covers document structure, story format, and the level of detail expected before implementation begins."
---

## Functional Requirements Standards

### Document Structure
Functional requirements documents follow this structure:
1. **Overview** — Problem statement, goals, success metrics
2. **Stakeholders** — Roles affected and their primary concerns
3. **User Journeys** — High-level end-to-end flows before detailed requirements
4. **Functional Requirements** — Numbered, atomic, testable statements
5. **Non-Functional Requirements** — Performance, security, accessibility, availability
6. **Assumptions & Constraints** — What we are taking as given; what is out of scope
7. **Open Questions** — Unresolved decisions blocking progress

### Requirement Statements
- Atomic: one requirement = one testable behaviour
- Use "The system shall..." or "Users can..." — avoid vague verbs like "support" or "handle"
- Bad: `"The system shall handle data"`
- Good: `"Authenticated staff can create a meal cycle record, and the record is persisted to Table Storage within 2 seconds."`

### User Stories
Format: **As a** [role], **I want to** [action], **so that** [benefit].

Acceptance criteria in **Given / When / Then** format:
```
Given I am an authenticated operations user
When I submit a valid meal cycle form
Then a new meal cycle record is created in the system
And the record is persisted to Table Storage
And I see a success message
```

### Personas / Roles
Define roles at the top of any requirements document before referencing them in stories:
- Each role has a name, primary goal, and key frustrations
- Roles map to authentication/authorisation levels in the system

### Level of Detail
- Functional requirements are detailed upfront — implementation decisions are deferred to sprint planning
- Every external integration (Entra ID, Azure Table Storage, Service Bus, Key Vault) must have its trigger, data flow, and failure modes documented
- Wireframes or user journey maps are recommended but not mandatory at this stage

### Out of Scope
- Do NOT include database schema, API endpoint design, or technology choices in requirements documents — those belong in the architecture phase
- Do NOT estimate complexity in requirements — that is a sprint planning activity
