---
sidebar_position: 1
title: PipeForge Use Cases
---

PipeForge pipelines are lightweight, flexible, and built to support a wide range of structured workflows - far beyond traditional request processing. Here are some of the most common and powerful ways to use them.

## 🎮 Game Loop and Simulation Ticks

Structure turn-based or tick-based game logic using ordered steps for input handling, state updates, AI decisions, and rule enforcement. Each tick runs as a pipeline, keeping logic deterministic and cleanly separated. Great for simulations, turn-based games, and event-driven systems.

## 🧩 Middleware-Style Request Processing

Build modular request pipelines similar to ASP.NET middleware - without needing a full web host. Use PipeForge steps to handle routing, authorization, validation, transformation, and response formatting in console apps, background services, or minimal APIs.

## 🛠️ DevOps and Automation Pipelines

Automate builds, deployments, and environment setup using step-driven workflows. Each step can perform checks, run scripts, transform files, or trigger rollbacks. You get clear control flow, built-in error handling, and testable, reusable logic.

## 🔐 Security and Auditing Pipelines

Apply policies, redact data, and emit audit logs with structured steps. Keep security logic isolated from business concerns, and use filters to enable or disable behaviors based on environment or role. Steps can be reused across pipelines to ensure consistent enforcement.

## 📊 ETL and Data Processing Pipelines

Break ETL flows into clean steps for validation, transformation, and persistence. Pipelines are easy to test and extend with instrumentation, branching, and conditional logic - perfect for data-driven systems that need clarity and control.

## 🤖 LLM and AI Workflows

Orchestrate complex AI interactions with steps for prompt prep, model execution, fallback logic, and response parsing. PipeForge's lazy resolution and short-circuiting make it ideal for chaining models, retry logic, and structured output handling.

## 🗂️ Business Logic and Domain Orchestration

Replace brittle `if`/`else` trees with pipelines that express business behavior clearly and predictably. Run rules, apply policies, trigger side effects, or moderate content in well-defined steps. Ideal for decision trees, policy enforcement, and modular domain workflows.

## 📬 Command and Event Handling

Handle application commands or domain events with pipelines that validate input, apply rules, mutate state, or trigger additional actions. Works well with CQRS or event-driven systems. Use filters to target handlers based on event type or source.

---

Whatever you're building, PipeForge helps you organize logic into pipelines that are clean, testable, and built to adapt.
