# Schedule Code Runner Stata and Installer Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Fix `.do` scheduling by introducing a configurable Stata executable path, improve local date/time UX, and package the app as an installable Windows release with GitHub publishing support.

**Architecture:** Add a small settings layer in the core library, surface it in the WPF shell, and thread the configured Stata path into both manual and scheduled worker execution. Add a release publish path plus an `Inno Setup` installer script, while keeping runtime data in local app data so installed builds behave the same as the debug build.

**Tech Stack:** .NET 8, WPF, System.Text.Json, Windows Task Scheduler CLI, GitHub CLI, Inno Setup

---

### Task 1: Settings and Stata Path Support

**Files:**
- Modify: `src/ScheduleCodeRunner.Core/`
- Modify: `src/ScheduleCodeRunner.Worker/`
- Modify: `tests/ScheduleCodeRunner.Tests/Program.cs`

**Step 1: Write the failing test**

Add tests for loading default settings, saving a Stata executable path, and resolving `.do` execution with the configured path.

**Step 2: Run test to verify it fails**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: FAIL because settings storage and configurable `.do` execution do not exist yet.

**Step 3: Write minimal implementation**

Add settings persistence, load the configured Stata path in the worker and app controller, and reject `.do` runs when no Stata executable is configured.

**Step 4: Run test to verify it passes**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: PASS for the new settings and `.do` execution tests.

**Step 5: Commit**

Commit after the Stata settings slice passes.

### Task 2: Date and Time UX

**Files:**
- Modify: `src/ScheduleCodeRunner.App/`
- Modify: `tests/ScheduleCodeRunner.Tests/Program.cs`

**Step 1: Write the failing test**

Add tests for future-time validation and human-readable local time formatting.

**Step 2: Run test to verify it fails**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: FAIL because the app does not yet validate past times or expose clearer formatting.

**Step 3: Write minimal implementation**

Update the task editor and dashboard formatting to show local time clearly and block past schedules.

**Step 4: Run test to verify it passes**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: PASS for time validation and formatting behavior.

**Step 5: Commit**

Commit after the date/time slice passes.

### Task 3: Settings UI and Runtime Validation

**Files:**
- Modify: `src/ScheduleCodeRunner.App/`
- Modify: `src/ScheduleCodeRunner.Core/`

**Step 1: Write the failing test**

Add tests that `.do` tasks fail fast with a helpful message when Stata is not configured.

**Step 2: Run test to verify it fails**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: FAIL because the validation message and settings UI are missing.

**Step 3: Write minimal implementation**

Add a settings dialog or panel for Stata path configuration and surface validation messages in the save/run workflow.

**Step 4: Run test to verify it passes**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: PASS with clear `.do` guidance.

**Step 5: Commit**

Commit after the settings UI slice passes.

### Task 4: Installer Packaging

**Files:**
- Create: `installer/`
- Modify: `src/ScheduleCodeRunner.App/ScheduleCodeRunner.App.csproj`
- Modify: `tests/ScheduleCodeRunner.Tests/Program.cs`

**Step 1: Write the failing test**

Add a test that verifies release packaging files exist where expected after publish setup is added.

**Step 2: Run test to verify it fails**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: FAIL because installer assets and release publish configuration are missing.

**Step 3: Write minimal implementation**

Add release publish configuration, create an `Inno Setup` script, and wire the expected output paths.

**Step 4: Run test to verify it passes**

Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: PASS and successful release build.

**Step 5: Commit**

Commit after packaging passes.

### Task 5: GitHub Publishing Scaffold

**Files:**
- Create: `.gitignore`
- Modify: workspace repo metadata as needed

**Step 1: Write the failing test**

Use environment verification instead of a code test: confirm the workspace is not yet a git repo and auth is invalid.

**Step 2: Run verification to confirm it fails**

Run: `gh auth status`
Expected: FAIL with invalid token.

**Step 3: Write minimal implementation**

Re-auth GitHub CLI, initialize git, create the public repo, and push the code.

**Step 4: Run verification to confirm it passes**

Run: `gh auth status`
Expected: PASS with a valid logged-in account.

**Step 5: Commit**

Commit and push once the repo setup is complete.
