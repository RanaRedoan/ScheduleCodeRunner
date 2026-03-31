# Profile README All Repos Design

**Audience:** Visitors to the GitHub profile `RanaRedoan`

**Goal:** Update the profile README so all public repositories are represented without weakening the existing Stata-focused branding.

**Problem:** The current profile README strongly showcases Stata packages, but several public repositories are not represented at all. Adding every repo into the existing Stata package section would make the profile crowded and blur the distinction between Stata packages and other tools.

**Approved direction:** Keep the current intro, skills, and Stata package sections intact. Add a separate professional section for the missing public repositories.

**Design decisions:**

1. Preserve the existing Stata-first identity.
2. Add a new section below the Stata content rather than mixing unrelated repositories into the Stata package table.
3. Group missing repositories by category for readability.
4. Use a compact Markdown table so all repos appear without making the profile feel cluttered.

**Missing public repositories to include:**

- `ScheduleCodeRunner`
- `surveycto-random-dice-plugin`
- `surveycto-radar-navigator`
- `exporttables`
- `texttranslation`

**Planned section structure:**

- Section title: `Featured Software & Data Tools`
- Columns:
  - Project
  - Category
  - Description
  - Updated

**Category plan:**

- Desktop App: `ScheduleCodeRunner`
- SurveyCTO Tools: `surveycto-random-dice-plugin`, `surveycto-radar-navigator`
- Data Utility: `exporttables`, `texttranslation`

**Content style:**

- Short one-line descriptions
- Direct GitHub links
- Updated month/year badges or plain month/year text
- Consistent tone with the rest of the profile README

**Verification plan:**

- Review the updated profile README content locally before publishing.
- Confirm all missing public repos are present exactly once.
- Push the README update to `RanaRedoan/RanaRedoan` and verify the remote file update.
