# Runtime Settings and Installer Metadata Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add structured runtime settings for Python, R, Stata, and Batch/CMD, then improve the installer metadata and information page and rebuild the shareable setup executable.

**Architecture:** Extend the existing `AppSettings` model and settings UI so runtime paths are configured centrally, then thread those settings through task validation, command resolution, and manual/scheduled execution. Update the Inno Setup script to include publisher/support metadata and an informational license page, and keep the existing installer build pipeline.

**Tech Stack:** C#, WPF, .NET 8, Inno Setup, existing console-style test harness

---

### Task 1: Expand settings persistence model

**Files:**
- Modify: `src/ScheduleCodeRunner.Core/Models/AppSettings.cs`
- Test: `tests/ScheduleCodeRunner.Tests/Program.cs`

**Step 1: Write the failing test**
- Extend the settings roundtrip test to persist and reload Python, R, Stata, and Batch/CMD paths.

**Step 2: Run test to verify it fails**
Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: FAIL because `AppSettings` only contains `StataExecutablePath`.

**Step 3: Write minimal implementation**
- Add `PythonExecutablePath`, `RExecutablePath`, and `BatchShellPath` to `AppSettings`.
- Keep defaults as empty strings.

**Step 4: Run test to verify it passes**
Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: PASS for settings persistence.

### Task 2: Add runtime-aware command resolution and validation

**Files:**
- Modify: `src/ScheduleCodeRunner.Core/Execution/ScriptCommandResolver.cs`
- Modify: `src/ScheduleCodeRunner.App/Services/AppController.cs`
- Modify: `src/ScheduleCodeRunner.Worker/Program.cs`
- Test: `tests/ScheduleCodeRunner.Tests/Program.cs`

**Step 1: Write the failing tests**
- Add tests for Python resolution using configured path.
- Add tests for R resolution using configured path.
- Add tests for Batch/CMD resolution using configured shell or default `cmd.exe`.
- Add tests that missing Python/R/Stata configuration is rejected with clear messages when required.

**Step 2: Run test to verify they fail**
Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: FAIL due to missing settings-aware resolution behavior.

**Step 3: Write minimal implementation**
- Update `ScriptCommandResolver` to accept all runtime paths.
- Update `AppController` task save/run validation to check the required runtime by extension.
- Update worker startup to load the expanded settings model and pass it into the resolver.

**Step 4: Run test to verify it passes**
Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: PASS for runtime resolution and validation.

### Task 3: Redesign the settings window

**Files:**
- Modify: `src/ScheduleCodeRunner.App/SettingsWindow.xaml`
- Modify: `src/ScheduleCodeRunner.App/SettingsWindow.xaml.cs`
- Test: `tests/ScheduleCodeRunner.Tests/Program.cs`

**Step 1: Write the failing tests**
- Add settings validation coverage for valid/invalid runtime paths through app-level save behavior if not already covered.

**Step 2: Run test to verify it fails**
Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: FAIL because only Stata is represented in the settings UI/model behavior.

**Step 3: Write minimal implementation**
- Replace the one-field Stata settings form with rows for Python, R, Stata, and Batch/CMD.
- Add browse buttons for each executable path.
- Add a read-only note for `.exe` tasks.
- Keep the layout simple and readable.

**Step 4: Run test to verify it passes**
Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: PASS.

### Task 4: Improve installer metadata and information page

**Files:**
- Modify: `installer/ScheduleCodeRunner.iss`
- Create: `installer/LICENSE_INFO.txt`
- Test: `tests/ScheduleCodeRunner.Tests/Program.cs`

**Step 1: Write the failing tests**
- Add tests that the installer script contains the updated publisher name and references an informational license page.

**Step 2: Run test to verify it fails**
Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: FAIL because the installer still uses old publisher metadata and has no info page.

**Step 3: Write minimal implementation**
- Update publisher metadata in the Inno script.
- Add optional support URLs and app comments where supported.
- Add `LicenseFile` pointing to a readable information/support text file.
- Preserve desktop shortcut and launch behavior.

**Step 4: Run test to verify it passes**
Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: PASS.

### Task 5: Rebuild and verify the installer artifact

**Files:**
- Modify: `installer/Build-Installer.ps1` only if required for successful packaging
- Output: `artifacts/installer/ScheduleCodeRunner-Setup.exe`

**Step 1: Run full test suite**
Run: `dotnet run --project tests/ScheduleCodeRunner.Tests/ScheduleCodeRunner.Tests.csproj`
Expected: PASS.

**Step 2: Run solution build**
Run: `dotnet build ScheduleCodeRunner.sln`
Expected: PASS.

**Step 3: Build installer**
Run: `powershell -ExecutionPolicy Bypass -File .\installer\Build-Installer.ps1`
Expected: installer build succeeds and writes `artifacts/installer/ScheduleCodeRunner-Setup.exe`.

**Step 4: Verify artifact exists**
Run: `Get-ChildItem .\artifacts\installer\ScheduleCodeRunner-Setup.exe | Select-Object FullName,Length,LastWriteTime`
Expected: updated setup executable exists.
