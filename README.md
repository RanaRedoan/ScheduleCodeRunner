# Schedule Code Runner

Schedule Code Runner is a Windows desktop app for scheduling scripts and executables from a simple interface. It lets you create scheduled jobs, run them through Windows Task Scheduler, and review run history without manually writing task scheduler commands.

## Download

Download the latest installer from the GitHub Releases page:

- [Latest release](https://github.com/RanaRedoan/ScheduleCodeRunner/releases/latest)
- Installer file: `ScheduleCodeRunner-Setup.exe`

## What This App Does

Schedule Code Runner helps you:

- Schedule a script or executable to run at a future date and time
- Manage tasks from one desktop app
- Save launch arguments for each task
- Run tasks in hidden or visible mode
- View task status and run history
- Let Windows Task Scheduler run jobs even when the app is not open

## Supported Task Types

The app currently supports:

- Python scripts: `.py`
- R scripts: `.r`
- Stata scripts: `.do`
- Batch files: `.bat`
- Command files: `.cmd`
- Windows executables: `.exe`

## System Requirements

- Windows
- Permission to create Windows scheduled tasks on the machine
- The required runtime installed for the script type you want to run

Runtime notes:

- `.py` tasks require Python and a valid Python executable path in the app Settings
- `.r` tasks require R and a valid R executable path in the app Settings
- `.do` tasks require Stata and a valid Stata executable path in the app Settings
- `.bat` and `.cmd` tasks use `cmd.exe`
- `.exe` tasks do not require an extra runtime path

## Install

1. Open the [latest release](https://github.com/RanaRedoan/ScheduleCodeRunner/releases/latest).
2. Download `ScheduleCodeRunner-Setup.exe`.
3. Run the installer.
4. Complete the setup wizard.
5. Start **Schedule Code Runner** from the Start menu or desktop shortcut.

If Windows shows a security warning, review the file source and continue only if you trust the release from this repository.

## First-Time Setup

Before creating script-based tasks, open the app Settings and configure any runtime paths you need:

- Python executable path for `.py`
- R executable path for `.r`
- Stata executable path for `.do`
- Optional batch shell path for `.bat` and `.cmd`

You only need to configure the runtimes you actually plan to use.

## Create Your First Scheduled Task

1. Open **Schedule Code Runner**.
2. Create a new task from the main window.
3. Browse to the file you want to run.
4. Confirm or edit the task name.
5. Enter any command-line arguments if needed.
6. Choose the date and time.
7. Choose whether the task should run as `Hidden` or `Visible`.
8. Save the task.

After saving, the app creates or updates the related Windows scheduled task for you.

## How Scheduling Works

Schedule Code Runner uses Windows Task Scheduler in the background. The desktop app saves your task information locally, then creates a Windows scheduled task that launches the bundled worker process at the chosen time.

This means:

- Your scheduled jobs can still run even if the main app is closed
- Task execution is handled by Windows Task Scheduler
- The app can show history and last-run results based on its local data store

## Run History and Logs

Each task keeps a history of past runs, including status, exit code, and short output or error excerpts.

Log files are written locally so you can inspect full output when needed.

## Where Data Is Stored

Schedule Code Runner stores its data under your local application data folder:

- Data root: `%LocalAppData%\ScheduleCodeRunner\data`
- Task and settings store: `%LocalAppData%\ScheduleCodeRunner\data\store.json`
- Logs folder: `%LocalAppData%\ScheduleCodeRunner\data\logs`

This data is separate from the source code repository and separate from the installer download.

## Troubleshooting

### A script task will not save

Check the following:

- The selected file path exists
- The scheduled time is in the future
- The required runtime path is configured in Settings for that file type
- The configured runtime file still exists on disk

### A task shows invalid path

This usually means the original script or executable was moved, renamed, or deleted after the task was created.

### A task runs but fails

Check:

- The task arguments
- The runtime path in Settings
- The app history window
- The log files in `%LocalAppData%\ScheduleCodeRunner\data\logs`

### Nothing runs at the scheduled time

Check:

- Windows Task Scheduler is available on the machine
- The scheduled task was created successfully
- The selected time was in the future when saved
- The target file and required runtime still exist

## Known Limitations

- Windows only
- Depends on Windows Task Scheduler
- Python, R, and Stata tasks require correct runtime configuration before saving
- If a script or executable is moved after scheduling, the task can become invalid

## Updating

To update the app:

1. Download the latest installer from the [Releases page](https://github.com/RanaRedoan/ScheduleCodeRunner/releases/latest).
2. Run the installer again.
3. Your locally stored task data should remain under `%LocalAppData%\ScheduleCodeRunner\data`.

## Uninstall

You can uninstall Schedule Code Runner from Windows installed apps or from the standard uninstall entry created by the installer.

## Source Code

This repository also contains the source code, but end users only need the installer from the Releases page.
