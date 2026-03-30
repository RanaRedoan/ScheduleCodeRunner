# README User Guide Design

**Audience:** End users who want to install and use Schedule Code Runner on Windows.

**Goal:** Replace the placeholder GitHub README with a practical user-facing guide that helps people download the installer, understand supported task types, configure required runtimes, create their first scheduled task, and troubleshoot common issues.

**Recommended structure:**

1. Product summary
2. Download and install
3. What the app can run
4. First-time setup
5. Create your first scheduled task
6. How the app works
7. Where data and logs are stored
8. Troubleshooting
9. Known limitations
10. Uninstall and updates

**Content decisions:**

- Optimize for non-developers, not contributors.
- Put the release download link near the top.
- Explain that the app is Windows-only and uses Windows Task Scheduler.
- Document the actual supported task types: `.py`, `.r`, `.do`, `.bat`, `.cmd`, and `.exe`.
- Explain that Python, R, and Stata require configured runtime paths in Settings before those script types can be saved.
- Describe the real storage location used by the app: `%LocalAppData%\\ScheduleCodeRunner\\data\\store.json` and the `logs` folder beneath the same data root.
- Avoid build-from-source instructions in the main README.

**Verification plan:**

- Check the README text locally after editing.
- Review the git diff to confirm the placeholder content was replaced with the intended user guide.
- Push the README update to GitHub so the repository landing page shows the new content.
