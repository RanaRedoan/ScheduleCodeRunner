<p align="center">
  <img src="src/ScheduleCodeRunner.App/Assets/app-logo.png" alt="Schedule Code Runner logo" width="140">
</p>

<h1 align="center">Schedule Code Runner</h1>

<p align="center">
  A professional Windows desktop app for scheduling scripts and executables with a clean interface, task history, and Windows Task Scheduler integration.
</p>

<p align="center">
  <a href="https://github.com/RanaRedoan/ScheduleCodeRunner/releases/latest">
    <img src="https://img.shields.io/github/v/release/RanaRedoan/ScheduleCodeRunner?label=Latest%20Release&style=for-the-badge" alt="Latest release">
  </a>
  <img src="https://img.shields.io/badge/Platform-Windows-0A66C2?style=for-the-badge" alt="Windows">
  <img src="https://img.shields.io/badge/Installer-EXE-1F6F43?style=for-the-badge" alt="Installer EXE">
</p>

<p align="center">
  <a href="https://github.com/RanaRedoan/ScheduleCodeRunner/releases/latest"><strong>Download Installer</strong></a>
  &nbsp;|&nbsp;
  <a href="https://github.com/RanaRedoan/ScheduleCodeRunner/releases/tag/v1.0.0"><strong>View Release</strong></a>
</p>

---

## Overview

Schedule Code Runner helps Windows users schedule scripts and executables without manually creating scheduled tasks. From one desktop application, you can define when a job should run, choose whether it should be visible or hidden, pass arguments, and review execution history later.

It is designed for practical use cases such as:

- automating recurring scripts
- scheduling research or analytics jobs
- launching executables at a defined time
- tracking whether a scheduled run completed or failed

## At a Glance

| Area | Details |
| --- | --- |
| Platform | Windows |
| Installer | `ScheduleCodeRunner-Setup.exe` |
| Scheduling engine | Windows Task Scheduler |
| Supported file types | `.py`, `.r`, `.do`, `.bat`, `.cmd`, `.exe` |
| Run modes | Hidden or Visible |
| Data storage | `%LocalAppData%\ScheduleCodeRunner\data` |
| History and logs | Stored locally for later review |

## Download and Install

Get the installer from the GitHub Releases page:

- [Download the latest release](https://github.com/RanaRedoan/ScheduleCodeRunner/releases/latest)
- Installer name: `ScheduleCodeRunner-Setup.exe`

### Installation Steps

1. Open the [latest release](https://github.com/RanaRedoan/ScheduleCodeRunner/releases/latest).
2. Download `ScheduleCodeRunner-Setup.exe`.
3. Run the installer.
4. Complete the setup wizard.
5. Launch **Schedule Code Runner** from the Start menu or desktop shortcut.

If Windows displays a security prompt, verify that the installer came from this repository before continuing.

## Before You Create Your First Task

This is the most important first-time setup step.

Before adding a new script-based task, open **Settings** and configure the runtime path you need:

- Python executable path for `.py`
- R executable path for `.r`
- Stata executable path for `.do`
- Optional batch shell path for `.bat` and `.cmd`

Important:

- **Settings is where you configure runtimes**
- **The actual script or app file you want to run is selected when creating a new task**

So the normal flow is:

1. Configure the required runtime in **Settings**
2. Create a new task
3. Browse to the source file, script, batch file, or executable you want to run

## Quick Start

### 1. Configure Settings

Open **Settings** and add any required runtime executable paths before saving script-based tasks.

### 2. Create a New Task

From the main window:

1. Create a new task
2. Browse to the file you want to run
3. Confirm or edit the task name
4. Add command-line arguments if needed
5. Choose the run date and time
6. Choose `Hidden` or `Visible`
7. Save the task

### 3. Let the Schedule Run

After saving, the app creates or updates the related Windows scheduled task for you. The job can still run even if the main application is closed.

### 4. Review Results

Use the app to review:

- current task status
- recent run history
- short output and error excerpts

## Supported Workloads

| File Type | Example Use | Requirement |
| --- | --- | --- |
| `.py` | Python automation scripts | Python path configured in Settings |
| `.r` | R analysis scripts | R path configured in Settings |
| `.do` | Stata jobs | Stata path configured in Settings |
| `.bat` / `.cmd` | Batch and command scripts | Uses `cmd.exe` or configured shell |
| `.exe` | Desktop or console applications | No extra runtime path required |

## How It Works

Schedule Code Runner uses Windows Task Scheduler behind the scenes.

The workflow is straightforward:

1. You configure a task in the desktop app.
2. The app stores task data locally.
3. Windows Task Scheduler launches the bundled worker at the scheduled time.
4. The worker runs your selected script or executable.
5. The app records status, output summary, and error summary for later review.

This approach gives you a user-friendly interface while still relying on the native Windows scheduling system.

## Why Use Schedule Code Runner

- Cleaner than manually configuring Task Scheduler commands
- Better visibility into run history and task state
- Works well for script-driven workflows
- Keeps user data and logs in a predictable local location
- Lets you manage multiple scheduled jobs from one place

## Troubleshooting

<details>
<summary><strong>A task will not save</strong></summary>

Check the following:

- the selected file path exists
- the scheduled time is in the future
- the required runtime path is configured in Settings
- the configured runtime file still exists on disk

</details>

<details>
<summary><strong>A task shows invalid path</strong></summary>

This usually means the original script or executable was moved, renamed, or deleted after the task was created.

</details>

<details>
<summary><strong>A task runs but fails</strong></summary>

Review:

- task arguments
- runtime path settings
- run history inside the app
- local logs under `%LocalAppData%\ScheduleCodeRunner\data\logs`

</details>

<details>
<summary><strong>Nothing runs at the scheduled time</strong></summary>

Check:

- Windows Task Scheduler is available on the machine
- the scheduled task was created successfully
- the selected time was in the future when saved
- the target file and required runtime still exist

</details>

## Data, History, and Logs

Schedule Code Runner stores its data locally under:

- data root: `%LocalAppData%\ScheduleCodeRunner\data`
- settings and task store: `%LocalAppData%\ScheduleCodeRunner\data\store.json`
- logs: `%LocalAppData%\ScheduleCodeRunner\data\logs`

This means your scheduled task metadata and run history are kept outside the source repository and outside the installer package.

## Known Limitations

- Windows only
- depends on Windows Task Scheduler
- Python, R, and Stata tasks require proper runtime configuration before saving
- tasks can become invalid if the target file is moved or deleted later

## Update and Uninstall

### Update

1. Download the newest installer from the [Releases page](https://github.com/RanaRedoan/ScheduleCodeRunner/releases/latest).
2. Run the installer again.
3. Your local task data should remain under `%LocalAppData%\ScheduleCodeRunner\data`.

### Uninstall

You can uninstall Schedule Code Runner from standard Windows installed apps or from the uninstall entry created by the installer.

## Repository Note

This repository contains the source code, but end users only need the installer from the Releases page.
