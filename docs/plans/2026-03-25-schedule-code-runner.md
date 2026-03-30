# Schedule Code Runner Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Build a native WPF desktop app that manages Windows scheduled tasks for local scripts and executables.

**Architecture:** Use a three-project solution: a shared core library for task definitions and scheduling logic, a worker console app for job execution and log capture, and a WPF shell for dashboard management. Persist task data in a local JSON store and delegate timed execution to Windows Task Scheduler through `schtasks.exe`.

**Tech Stack:** .NET 8, WPF, System.Text.Json, Windows Task Scheduler CLI, PowerShell asset conversion

---

### Task 1: Solution Skeleton

**Files:**
- Create: `src/ScheduleCodeRunner.Core/`
- Create: `src/ScheduleCodeRunner.Worker/`
- Create: `src/ScheduleCodeRunner.App/`
- Create: `tests/ScheduleCodeRunner.Tests/`
- Create: `ScheduleCodeRunner.sln`

**Step 1: Write the failing test**

Create a smoke test in `tests/ScheduleCodeRunner.Tests` that references the core project and asserts a missing task store starts empty.

**Step 2: Run test to verify it fails**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests`
Expected: FAIL because the repository and model types do not exist yet.

**Step 3: Write minimal implementation**

Create the solution and core model skeleton needed for the first smoke test.

**Step 4: Run test to verify it passes**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests`
Expected: PASS for the new smoke test.

**Step 5: Commit**

Commit after the initial solution and smoke test pass.

### Task 2: Task Persistence and Validation

**Files:**
- Modify: `src/ScheduleCodeRunner.Core/`
- Modify: `tests/ScheduleCodeRunner.Tests/`

**Step 1: Write the failing test**

Add tests for saving tasks, loading tasks, preserving history, and marking missing paths invalid.

**Step 2: Run test to verify it fails**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests`
Expected: FAIL on missing persistence and validation behavior.

**Step 3: Write minimal implementation**

Add task models, a JSON repository, application storage paths, and validation logic.

**Step 4: Run test to verify it passes**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests`
Expected: PASS with persistence and validation behavior working.

**Step 5: Commit**

Commit after persistence and validation pass.

### Task 3: Scheduler and Runner

**Files:**
- Modify: `src/ScheduleCodeRunner.Core/`
- Modify: `src/ScheduleCodeRunner.Worker/`
- Modify: `tests/ScheduleCodeRunner.Tests/`

**Step 1: Write the failing test**

Add tests for file-type command resolution, scheduler command generation, and run result recording.

**Step 2: Run test to verify it fails**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests`
Expected: FAIL because the scheduler and worker behavior are missing.

**Step 3: Write minimal implementation**

Add extension-based execution resolution, worker argument handling, log capture, run result persistence, and `schtasks.exe` command generation.

**Step 4: Run test to verify it passes**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests`
Expected: PASS with command generation and run recording verified.

**Step 5: Commit**

Commit after scheduler and runner behavior pass.

### Task 4: WPF Dashboard and Task Editing

**Files:**
- Modify: `src/ScheduleCodeRunner.App/`
- Modify: `src/ScheduleCodeRunner.Core/`

**Step 1: Write the failing test**

Add view-model level tests for loading dashboard rows, creating tasks, editing tasks, deleting tasks, and exposing history summaries.

**Step 2: Run test to verify it fails**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests`
Expected: FAIL because the dashboard and editor behavior are missing.

**Step 3: Write minimal implementation**

Add the app shell, dashboard view, task editor dialog, history dialog, commands, and blue/red visual styling.

**Step 4: Run test to verify it passes**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests`
Expected: PASS for the view-model tests and successful WPF build.

**Step 5: Commit**

Commit after dashboard behavior and build pass.

### Task 5: Branding and Verification

**Files:**
- Create: `assets/App.ico`
- Modify: `src/ScheduleCodeRunner.App/`
- Modify: `tests/ScheduleCodeRunner.Tests/`

**Step 1: Write the failing test**

Add a test that verifies branded assets resolve from the configured app paths and that invalid file paths surface warning status.

**Step 2: Run test to verify it fails**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests`
Expected: FAIL because the icon path and final validation wiring are incomplete.

**Step 3: Write minimal implementation**

Convert the provided PNG into `.ico`, wire the icon into the app and project metadata, and finish dashboard warnings.

**Step 4: Run test to verify it passes**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests`
Expected: PASS and successful `dotnet build`.

**Step 5: Commit**

Commit after branding and verification pass.
