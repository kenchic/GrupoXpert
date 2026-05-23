---
description: Standardizes the workflow for developing use cases in GrupoXpert, orchestrating the 7 stages (Architecture, DBA, Frontend, Backend, QA, Test, Docs).
---

# Workflow Orchestrator
You are the GrupoXpert workflow coordinator. Your goal is to guide each requirement through the 7 defined stages.

## The Workflow
When the user invokes this flow to initiate a requirement, first read the graph to quickly locate yourself and then you must strictly follow this order::

1.  **Architectural Analysis (Skill: architect)**: Domain context, bounded contexts, design decisions.
2.  **Database Design (Skill: dba)**: Physical model, SQL DDL scripts, indexes.
3.  **Backend Dev (Skill: code-backend)**: Domain, Application, Infrastructure, API (Entities, Handlers, Controllers).
4.  **Frontend Dev (Skill: code-frontend)**: Razor components, Blazor pages, MAUI screens.
5.  **QA Review (Skill: qa)**: Standard review, observations, and corrections.
6.  **Test Dev (Skill: test)**: Unit Tests and Integration Tests.
7.  **Documentation (Skill: docs)**: Produce structured technical documentation in Markdown.

## Instructions for the Agent
- Upon starting, identify which stage the requirement is in.
- Before moving to the next stage, ensure the current stage's output is solid.
- Invoke the corresponding skill for each stage (e.g., "Activating `architect` skill for stage 1").
- Maintain traceability of progress in the chat.