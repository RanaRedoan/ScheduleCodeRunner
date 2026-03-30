# Schedule Code Runner Stata and Installer Design

## Goal
Extend the existing app so `.do` files run reliably through a configured Stata executable, the date/time UX is clearer for local device time, and the app can be distributed as a normal Windows installer.

## Findings
- Scheduled tasks are firing and invoking the worker.
- Current `.do` execution fails because the app launches `stata` directly and the machine does not expose Stata on `PATH`.
- Current task storage uses local timezone offsets correctly, but the UI does not present local time clearly enough.
- GitHub CLI is installed, but the saved login token is invalid and needs re-authentication.
- `Inno Setup` is not currently installed.

## Approved Design
### Stata execution
- Add a persistent app setting for the Stata executable path.
- Expose that setting in the UI with a browse flow and validation.
- Block `.do` task saves and manual runs with a clear message when Stata is not configured.
- Use the configured executable for both manual runs and scheduled worker runs.

### Date and time UX
- Keep scheduled values stored as local-aware `DateTimeOffset`.
- Improve labels and formatting so users see local time clearly.
- Validate that scheduled times are future times before saving.
- Show more readable timestamps in the dashboard and task editor.

### Installer and release
- Publish the app as a Windows release build.
- Install `Inno Setup` and add an installer script that installs the app, worker, and assets.
- Create a desktop shortcut and Start Menu entry.
- Keep user settings and history under local app data so updates do not wipe runtime data.

### GitHub
- Re-authenticate GitHub CLI with browser login.
- Initialize a local git repository in this workspace.
- Create a public GitHub repository and push the source once auth succeeds.

## Scope
Included:
- Stata path settings
- `.do` execution fix
- Local time UX improvements
- Installer generation
- GitHub repo creation

Deferred:
- Auto-detecting Stata from the registry
- Code signing
- Auto-update support
