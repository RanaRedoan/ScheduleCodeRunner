# Schedule Code Runner Design

## Goal
Build a native Windows desktop app that lets non-technical users schedule scripts or executables to run at an exact date and time, even when the app is closed and the screen is locked.

## Product Summary
`Schedule Code Runner` is a Windows-only scheduler manager aimed at researchers and analysts. The user selects a script or executable, chooses a time, optionally supplies arguments, chooses hidden or visible execution, and saves the task. The app keeps a dashboard of tasks with current status, history, and basic safeguards such as invalid path detection.

## Architecture
The product consists of three local components:

1. A `WPF` desktop app for task management and history display.
2. A shared core layer that stores task definitions, run history, validation rules, and execution behavior.
3. A small worker executable launched by Windows Task Scheduler to execute saved tasks and record results.

Windows Task Scheduler is the execution engine. The WPF app creates, updates, deletes, and manually triggers Windows scheduled tasks. The app itself does not need to remain open for jobs to run.

## Task Model
Each task stores:

- Stable task id
- User-facing name
- Source file path
- Arguments
- Scheduled run time
- Run visibility mode
- Internal Windows scheduled task name
- Current status
- Last result summary

Each run history record stores:

- Run id
- Task id
- Start time
- End time
- Duration
- Result status
- Exit code when available
- Short output excerpt
- Short error excerpt
- Full log file paths

## Supported File Types
V1 supports:

- `.py`
- `.R`
- `.do`
- `.bat`
- `.exe`

Execution is resolved by file extension. Python uses `python`, R uses `Rscript`, batch files use `cmd.exe /c`, executables run directly, and Stata `.do` files use a configurable launcher path.

## User Experience
The app opens to a single dashboard with a `New Task` action and a task list. Each row shows the script name, scheduled time, status, and quick actions for `Run now`, `Edit`, `Delete`, and `History`.

Task creation uses a short form:

1. Select file
2. Enter optional arguments
3. Choose visible or hidden execution
4. Pick date and time
5. Save

The UI should remain clean and minimal, with blue as the primary accent and red reserved for warnings and failures.

## Reliability and Safeguards
- Save-time validation ensures the selected path exists.
- Startup and refresh validation mark missing paths as `Invalid Path`.
- Scheduled and manual runs use the same worker path for consistent behavior.
- Failure details are stored in logs and summarized in history.
- If the required runtime is unavailable, the task is marked failed with a clear message.

## Branding
The existing `app logo.png` in the project root is the source asset. It should be converted into a Windows `.ico` file for application branding and reused inside the WPF UI.

## V1 Scope Boundary
Included:

- One-time schedules
- Hidden or visible execution
- Arguments
- Dashboard CRUD
- Manual run
- Basic history
- Invalid-path detection
- Logo integration

Deferred:

- Daily or weekly recurrence
- Advanced runtime discovery UI
- Notifications
- Parallel job orchestration
