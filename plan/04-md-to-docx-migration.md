# 04 — Migrate documentation from Markdown to .docx

## Goal

Produce a `.docx` copy of every Markdown file under `Documentation/Team` and
`Documentation/Individual/Soufian Manai, 1114385`, with embedded images preserved at their
original positions. The original `.md` files stay; the `.docx` copies live side by side and
are committed to git. A committed, reusable PowerShell script regenerates them on demand.

## Context

- Source request: `requests/2026-06-documents-migration.md`. It asks to copy each Markdown
  file to `.docx` (keep the `.md` too, side by side), limited to `Documentation/Team` and
  `Documentation/Individual/Soufian Manai, 1114385/*`, and to keep embedded images
  (standard `[Text](img.png)` / `![alt](img.png)` relative pathing) at the same location.
- Governing decisions are in `plan/00-decisions.md` under
  `2026-06-15 — Migrate documentation from Markdown to .docx`. In short: convert with
  **Pandoc**, run per file from the file's own directory, keep a reusable script in the repo,
  commit the `.docx`, keep the `.md`, scope to the two named roots only.
- **Tooling state (verified):** Pandoc is **not** installed; `winget` (v1.28) is available;
  `unzip` is available (used for validation). The repo has no `.gitattributes` yet and does
  not git-ignore `.docx`.

### In-scope files (23 `.md`)

Under `Documentation/Team` (21):

- `2A - SCRUM/Definition of done/Definition of Done.md`
- `2A - SCRUM/Product backlog/Product Backlog - User Stories.md`
- `2A - SCRUM/Sprint backlogs/Sprint 1 Backlog.md` … `Sprint 6 Backlog.md` (6)
- `2A - SCRUM/Sprint reviews/Sprint 1 Review.md` … `Sprint 6 Review.md` (6)
- `2B - PO NEEDS AND WISHES/Implementing needs and wishes of the PO/Implementation Summary.md`
- `2B - PO NEEDS AND WISHES/Implementing needs and wishes of the PO/PO Interview.md`
- `2B - PO NEEDS AND WISHES/product backlog/Product Backlog - User Stories.md`
- `3 - APPLICATION/ERD _ Entity Relationship Diagram/ERD - Description.md`
- `3 - APPLICATION/The use of technical concepts - LAB concepts/OODP Concepts.md`
- `4 - TESTING/UAT Story - Reservering plaatsen.md`
- `DISCLAIMER.md`

Under `Documentation/Individual/Soufian Manai, 1114385` (2):

- `Individuele Bijdrage.md`
- `Time Registration.md`

### Images that must end up embedded (verified relative refs)

- `Documentation/Team/3 - APPLICATION/ERD _ Entity Relationship Diagram/ERD - Description.md`
  → `![img.png](ERD-Debug-Diner.png)` (same directory).
- `Documentation/Individual/Soufian Manai, 1114385/Individuele Bijdrage.md`
  → `![Sprint 3 Backlog](Sprint-3-Backlog.png)`, `![User Stories Part 1/2](UserStories-pt1.png)`,
  `![User Stories Part 2/2](UserStories-pt2.png)` (same directory).

All other in-scope `.md` files contain no image references, so their `.docx` will have no
embedded media — that is expected.

## Implementation plan

### 1. Install Pandoc

Install once on the build machine:

```
winget install --id JohnMacFarlane.Pandoc --accept-source-agreements --accept-package-agreements
```

Then confirm it is reachable (a new shell may be needed so PATH refreshes):

```
pandoc --version
```

> If `pandoc` is not on PATH in the current session, either open a fresh shell or call it by
> its full install path for the run. Installation requires user/network access; if it cannot
> be installed in this environment, stop and report that — the conversion cannot proceed
> without it.

### 2. Add `Documentation/Convert-MarkdownToDocx.ps1`

Create the script at the `Documentation/` root. It walks the two in-scope roots, and for
each `*.md` produces a sibling `*.docx` by running Pandoc from the file's own directory so
relative image paths resolve and the output lands side by side.

```powershell
#Requires -Version 5
<#
.SYNOPSIS
  Generates a .docx copy of every in-scope Markdown doc, side by side with the .md.
.DESCRIPTION
  Scope is fixed to Documentation/Team and Documentation/Individual/Soufian Manai, 1114385
  per requests/2026-06-documents-migration.md. Requires Pandoc on PATH
  (winget install --id JohnMacFarlane.Pandoc). The original .md files are never modified or
  deleted; embedded images are resolved relative to each file's directory and embedded by
  Pandoc.
#>
$ErrorActionPreference = 'Stop'

if (-not (Get-Command pandoc -ErrorAction SilentlyContinue)) {
    throw "Pandoc not found on PATH. Install with: winget install --id JohnMacFarlane.Pandoc"
}

# Script lives in Documentation/, so resolve scope roots relative to $PSScriptRoot.
$roots = @(
    (Join-Path $PSScriptRoot 'Team'),
    (Join-Path $PSScriptRoot 'Individual\Soufian Manai, 1114385')
)

$count = 0
foreach ($root in $roots) {
    if (-not (Test-Path $root)) { throw "Scope root not found: $root" }
    Get-ChildItem -LiteralPath $root -Recurse -Filter *.md -File | ForEach-Object {
        $md = $_
        $docxName = [System.IO.Path]::GetFileNameWithoutExtension($md.Name) + '.docx'
        Push-Location -LiteralPath $md.DirectoryName
        try {
            # Run from the file's own directory so relative image paths resolve and the
            # .docx is written next to the .md. --from gfm covers GitHub tables / task lists.
            pandoc $md.Name --from gfm --resource-path . -o $docxName
            if ($LASTEXITCODE -ne 0) { throw "pandoc failed for $($md.FullName)" }
            Write-Host "converted: $($md.FullName) -> $docxName"
            $count++
        }
        finally {
            Pop-Location
        }
    }
}
Write-Host "Done. $count file(s) converted."
```

Notes for the implementer:

- The two scope roots are the only ones processed; do **not** generalize the glob to all of
  `Documentation/Individual` — other author folders are explicitly out of scope.
- `Documentation/bin` and `Documentation/obj` are at the `Documentation/` root, not under
  either scope root, so they are never visited. No `.md` exists there anyway.
- Default Pandoc styling is acceptable; do not add a reference/template docx or theming.

### 3. Run the script

From the repo root:

```
pwsh -File "Documentation/Convert-MarkdownToDocx.ps1"
```

Expect `Done. 23 file(s) converted.` and a `.docx` next to each of the 23 `.md` files.

### 4. Add `.gitattributes` and commit

- Create a repo-root `.gitattributes` (or append if one is later added) with:

  ```
  *.docx binary
  ```

- Stage and commit the 23 new `.docx` files, the new
  `Documentation/Convert-MarkdownToDocx.ps1`, and `.gitattributes`. Leave every `.md`
  unchanged. (Do not commit the unrelated already-staged `requests/...` file as part of this
  unless asked.)

## Out of scope

- Deleting or editing any `.md` file (request: keep them side by side).
- Any `Documentation/Individual/*` folder other than `Soufian Manai, 1114385` (e.g. Lars
  Werner, Quintin Blume, Randy Sheombar, Collaboration Contract).
- Non-Markdown assets in the in-scope trees: `LINK TO PRODUCT BACKLOG.txt`, `README_video.txt`,
  `Debug Diner - Test Results.xlsx`, `Debug Diner Tussentijdse Presentatie.gif`, and the
  source `.png` images themselves (they are inputs, embedded into the relevant `.docx`, not
  separately converted).
- Restyling/theming the `.docx` (no custom reference doc), changing Markdown content, or
  registering the `.docx`/script in `Documentation.csproj`.
- Application/source code, tests, and build configuration.

## Validation

From the repo root:

1. **Every in-scope `.md` has a sibling `.docx` and counts match (23).**

   ```
   pwsh -NoProfile -Command "
     $roots = @('Documentation/Team','Documentation/Individual/Soufian Manai, 1114385');
     $md = $roots | ForEach-Object { Get-ChildItem -LiteralPath $_ -Recurse -Filter *.md -File };
     $missing = $md | Where-Object { -not (Test-Path ([IO.Path]::ChangeExtension($_.FullName,'.docx'))) };
     'md: ' + $md.Count; 'missing docx: ' + $missing.Count; $missing.FullName
   "
   ```

   Expect `md: 23`, `missing docx: 0`.

2. **Images are actually embedded** (a `.docx` is a zip; embedded media lands in `word/media/`).
   The two docs with images must contain media; a text-only doc must not:

   ```
   unzip -l "Documentation/Team/3 - APPLICATION/ERD _ Entity Relationship Diagram/ERD - Description.docx" | grep -i "word/media"
   unzip -l "Documentation/Individual/Soufian Manai, 1114385/Individuele Bijdrage.docx" | grep -i "word/media"
   ```

   Expect 1 media entry for the ERD doc and 3 for `Individuele Bijdrage`. A spot-check on a
   text-only doc (e.g. `DISCLAIMER.docx`) should show **no** `word/media` entries.

3. **`.md` files untouched.** `git status` shows only added `.docx`, the new script, and
   `.gitattributes` — no modified `.md`.

4. **Manual spot-open (optional).** Open `ERD - Description.docx` and `Individuele
   Bijdrage.docx` in Word and confirm the diagram/screenshots appear at the same positions as
   in the Markdown, and that a table-heavy doc (e.g. a Sprint Backlog) renders its tables.
