# Runtime Settings and Installer Metadata Design

## Summary

Expand `Schedule Code Runner` from a Stata-only runtime setting into a structured runtime configuration screen that covers the supported script types: Python, R, Stata, Batch/CMD, and direct executables. At the same time, improve the Windows installer so it shows real publisher/support information and a readable information/license page during setup.

The goal is to make the app easier to share with non-technical users. A user who installs the app should see who built it, where to get support, and how to configure the required runtimes for the script types they want to run.

## Runtime Settings

### Supported runtime rows
- Python executable
- R executable
- Stata executable
- Batch/CMD shell
- Executable tasks note: runs directly, no runtime required

### Behavior
- The settings window shows one field and one browse button for each configurable runtime.
- Empty values mean:
  - Python: not configured
  - R: not configured
  - Stata: not configured
  - Batch/CMD: default to `cmd.exe`
- `.exe` tasks do not need a runtime path.
- Settings are persisted in the existing app settings store.

### Validation
- Save-time settings validation checks that any non-empty configured path exists.
- Save-time task validation checks that the required runtime exists for the selected script type.
- Manual `Run now` uses the same runtime validation as scheduled execution.
- Error messages should clearly state which runtime must be configured.

### Runtime resolution
- `.py` uses configured Python executable
- `.r` uses configured R executable
- `.do` uses configured Stata executable
- `.bat` / `.cmd` uses configured shell or `cmd.exe`
- `.exe` runs directly

## Installer Experience

### Metadata
Use the following developer metadata in the installer:
- Publisher: `Md. Redoan Hossain Bhuiyan`
- Email: `redoanhossain630@gmail.com`
- GitHub: `github.com/ranaredoan`
- LinkedIn: `linkedin.com/mdredoanhossainbhuiyan`

### Installer content
- Keep the normal welcome/setup flow.
- Add an information/license page users can read and continue past.
- Include support/contact information in that page.
- Keep Start Menu entry, desktop shortcut, and launch-after-install behavior.

### Scope constraints
- Do not implement auto-detection of runtimes in this iteration.
- Do not add custom runtime types.
- Do not add real legal enforcement or required agreement text; this is an informational page only.

## Data Model Changes
- Extend `AppSettings` to store paths for Python, R, Stata, and Batch/CMD.
- Keep backward compatibility by allowing missing new fields to default to empty strings.

## UI Changes
- Redesign `SettingsWindow` into a compact runtime configuration form.
- Each runtime row should explain what file type it supports.
- The EXE row should be informational only.

## Testing Strategy
- Add persistence tests for the expanded settings model.
- Add runtime resolution tests for Python, R, Stata, and Batch/CMD behavior.
- Add validation tests for missing configured runtimes.
- Add installer script tests for publisher/support metadata and info/license page content.
- Rebuild the installer and verify the setup executable is produced.
