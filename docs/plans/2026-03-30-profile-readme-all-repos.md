# Profile README All Repos Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Update the GitHub profile README in `RanaRedoan/RanaRedoan` so all public repositories are represented in a clean, categorized section.

**Architecture:** Keep the current profile README structure and Stata package showcase intact, then append a new categorized section for the missing repositories. Use GitHub API-based file update flow because the profile README repo is not the current local workspace.

**Tech Stack:** Markdown, GitHub CLI, GitHub Contents API

---

### Task 1: Inspect Current Profile README

**Files:**
- Read remote: `RanaRedoan/RanaRedoan/README.md`

**Step 1: Decode the current README**

Fetch and decode the existing profile README content.

**Step 2: Confirm missing repositories**

Compare the current README against the list of public repositories and identify the missing ones.

### Task 2: Draft the New Section

**Files:**
- Modify remote: `RanaRedoan/RanaRedoan/README.md`

**Step 1: Create a new section**

Add a section such as `Featured Software & Data Tools`.

**Step 2: Add all missing public repositories**

Include:

- `ScheduleCodeRunner`
- `surveycto-random-dice-plugin`
- `surveycto-radar-navigator`
- `exporttables`
- `texttranslation`

**Step 3: Group by category**

Use a readable Markdown table with category and short descriptions.

### Task 3: Verify and Publish

**Files:**
- Verify remote: `RanaRedoan/RanaRedoan/README.md`

**Step 1: Review the updated Markdown**

Check the final README body before publishing.

**Step 2: Push the update**

Update the README in the profile repository.

**Step 3: Verify the remote content**

Fetch the README again and confirm the new section and all missing public repos are present.
