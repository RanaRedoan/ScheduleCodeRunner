# README User Guide Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Replace the placeholder repository README with a detailed end-user guide for installing and using Schedule Code Runner.

**Architecture:** This is a documentation-only change focused on the repository landing page. The README should mirror actual application behavior and storage paths from the codebase so users can install and operate the app without reading source files.

**Tech Stack:** Markdown, GitHub Releases, repository docs

---

### Task 1: Gather Product Facts

**Files:**
- Read: `src/ScheduleCodeRunner.App/Services/AppController.cs`
- Read: `src/ScheduleCodeRunner.Core/Execution/ScriptCommandResolver.cs`
- Read: `src/ScheduleCodeRunner.Core/Execution/RuntimeConfigurationRules.cs`
- Read: `src/ScheduleCodeRunner.Core/Execution/TaskExecutionService.cs`
- Read: `installer/ScheduleCodeRunner.iss`

**Step 1: Confirm supported file types and required runtimes**

Record the user-visible behavior for `.py`, `.r`, `.do`, `.bat`, `.cmd`, and `.exe`.

**Step 2: Confirm storage and log paths**

Record the actual `%LocalAppData%` data folder and log folder behavior.

**Step 3: Confirm installer naming**

Record the release asset name `ScheduleCodeRunner-Setup.exe`.

### Task 2: Rewrite README for End Users

**Files:**
- Modify: `README.md`

**Step 1: Replace the placeholder heading**

Remove the minimal placeholder README content.

**Step 2: Add user guide sections**

Add sections for summary, download, install, supported task types, first-time setup, usage, troubleshooting, limitations, uninstall, and updates.

**Step 3: Keep the tone practical**

Write for non-developers and avoid source-build instructions in the main README.

### Task 3: Verify and Publish

**Files:**
- Verify: `README.md`

**Step 1: Review the edited README**

Run: `Get-Content README.md`
Expected: the file contains a full user-facing guide rather than a single title.

**Step 2: Review the git diff**

Run: `git diff -- README.md`
Expected: the diff shows the placeholder content replaced by a detailed README.

**Step 3: Commit and push**

Commit the README update and push it so GitHub renders the new landing-page documentation.
