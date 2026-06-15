# 03 — Translate Team documentation to Dutch

## Goal

Bring every prose document under `Documentation/Team` to consistent, natural Dutch, so the
whole `Team` documentation set reads in one language. `Documentation/Individual` is left
untouched. The change is translation plus punctuation cleanup only, never a content rewrite.

## Context

- Source request: `requests/2026-06-documentation.md`.
- The `Team` docs are currently a mix of Dutch and English. Some files are already largely
  Dutch (PO Interview, UAT Story, sprint reviews, Implementation Summary, ERD description);
  others are heavily or fully English (both `Product Backlog - User Stories.md` files,
  `OODP Concepts.md`, `README_video.txt`) or contain scattered English fragments (sprint
  backlogs, Definition of Done).
- `Documentation/Individual/*` is **out of scope** by explicit request. It is already done
  and may be read only as a writing-style reference (Dutch prose mixed with English
  code/feature names; e.g. `Documentation/Individual/Lars Werner, 1127685/Individuele
  Bijdrage.md`).
- The five governing decisions for this changeset are recorded in `plan/00-decisions.md`
  under `2026-06-15 — Translate Team documentation to Dutch`. Read them first. In short:

  1. **Keep established Scrum/Agile terms in English** — Definition of Done, Sprint, Sprint
     Review, Sprint Planning, Daily Stand-up, Retrospective, Product Backlog, User Story,
     Scrum Master.
  2. **Keep names/identifiers in English** — view/repository/class/story/task names and
     anything that maps to a code symbol, file name, PR/branch name, or established product
     title (e.g. `ReservationsView`, "Information View", "User login", the `### <Story>`
     headings, the "Opgeleverde taken" entries). Translate only the descriptive prose around
     them.
  3. **Keep verbatim factual content** — commit messages and author names in the sprint
     tables, code, file paths, GitHub URLs, enum values, and literal in-app strings (backtick
     UI text like `"All fields are required."`, status names like `Pending`, and the
     `✅ AVAILABLE` / `❌ TAKEN` labels). The app's UI is English; do not translate what it
     actually prints.
  4. **Strip em-dashes everywhere** — replace every `—` (and any en-dash `–` used as a
     separator or in a number range) with a plain hyphen `-`, or restructure the sentence.
     This is the one mechanical edit applied even to otherwise-verbatim content (commit
     messages, code comments). Visible meaningful arrows (`→`, `⇒`, `↔`) are **not** dashes
     and are kept.
  5. **Clean ASCII punctuation** — straight quotes/apostrophes (`'`, `"`), no non-breaking
     spaces (U+00A0), zero-width characters (U+200B/FEFF), or stray BOM. Leave existing
     authored-Dutch text (including pre-existing typos) intact apart from translation and this
     cleanup. Do not fix content typos or restructure documents.

### How to apply the rules to a mixed line

The rules combine to "Dutch prose, English names, no em-dashes." Worked examples:

- Heading `## Stories — Detail` → `## Stories - Detail` (keep "Stories"; strip dash).
- Story row value `Log in with my email and password` → `Inloggen met mijn e-mailadres en
  wachtwoord` (descriptive prose → Dutch). The story-name key `User login` stays English.
- Commit cell `Merge pull request #1 — souf/sql-scripts` → `Merge pull request #1 -
  souf/sql-scripts` (verbatim text kept; only the em-dash becomes a hyphen).
- Code comment `// BaseEntity.cs — lines 13–19` → `// BaseEntity.cs - lines 13-19` (code kept
  verbatim; em-dash and en-dash become hyphens).
- The "Focus" / "Sprintdoel" lines and "Opgeleverde taken" bullets are essentially lists of
  feature/component names that mirror the Tasks Tracker, so they stay in English; apply only
  the em-dash/character cleanup to them. Translate the connective/descriptive sentences
  around them (e.g. the Sprint 2 `> Note:` blockquote, the `(sprint total — see commit log
  below)` parenthetical → `(sprinttotaal, zie de commit-log hieronder)`).

When in doubt: if a fragment is a label, sentence, or description a reader parses as language,
translate it; if it is a token a developer would grep for, keep it.

## Implementation plan

Edit only the files listed below. Process them one at a time; for each, translate English
prose to Dutch per the rules, then sweep the whole file for em-dashes and non-clean
characters. Markdown structure (tables, headings, links, code fences) must stay intact.

### Group 2A — SCRUM

1. **`Documentation/Team/2A - SCRUM/Definition of done/Definition of Done.md`**
   - Title: strip em-dash → `# Definition of Done - Debug Diner` ("Definition of Done" and
     "Debug Diner" kept).
   - Translate the intro sentence ("A user story is considered Done when ALL of the following
     criteria are met:") to Dutch, keeping the terms *user story* and *Done*.
   - Table header `Criterium | Check` → `Criterium | Controle`. Body rows are already Dutch.
   - `## Scrum ceremonies` → `## Scrum-ceremonies`; table header `Ceremony` → `Ceremonie`.
     Ceremony names (Sprint Planning, Daily Stand-up, Sprint Review, Retrospective) stay
     English. Day/time cells are already Dutch.

2. **`Documentation/Team/2A - SCRUM/Product backlog/Product Backlog - User Stories.md`**
   - Heading `# User Stories` kept. Nav link `[← Home](Home)` kept (arrow, not a dash).
   - Overview table header `Story | As a | I want | Project | Risk | So that` →
     `Story | Als | Wil ik | Project | Risico | Zodat`.
   - Story-name column values (`User login`, `Admin add new employee`, …) stay English.
   - Role values (`User/ Employee/ Admin`, `Admin`, `Employee/ Admin`) stay English (they are
     `Role` enum names and match the detail tables).
   - "I want" and "So that" values: translate to Dutch sentences.
   - "Risk" values `High` / `Medium` / `Low` → `Hoog` / `Gemiddeld` / `Laag` (also in the
     detail tables' `Risico` rows).
   - `## Stories — Detail` → `## Stories - Detail`. Each `### <Story>` heading stays English.
     Detail tables already use Dutch labels `Als / Wil ik / Zodat / Risico`; translate their
     values the same way as the overview table.

3. **`Documentation/Team/2A - SCRUM/Sprint backlogs/Sprint 1 Backlog.md`** through
   **`Sprint 6 Backlog.md`** (six files)
   - Nav arrows and links kept. Labels `Periode / Scrum Master / Commits / Focus` kept.
   - `Focus`, `## Sprintdoel`, and `## Opgeleverde taken` content: feature/component name
     lists → keep English text; apply only em-dash/character cleanup.
   - Sprint 2 & 5 `Commits` cell parenthetical `45 (sprint total — see commit log below)` →
     `45 (sprinttotaal, zie de commit-log hieronder)`.
   - Sprint 2 `> Note:` blockquote (English prose about Floris Landman) → translate to Dutch.
     Keep the name "Floris Landman" and "Scrum Master".
   - `## Commits` table: headers `Datum | SHA | Auteur | Bericht` kept. Dates, SHAs, author
     names, and commit-message text are verbatim; the only edit is replacing em-dashes inside
     messages with hyphens.
   - Footer line is already Dutch; keep, clean characters if needed.

4. **`Documentation/Team/2A - SCRUM/Sprint reviews/Sprint 1 Review.md`** through
   **`Sprint 6 Review.md`** (six files)
   - `# Sprint N Review`, `**Datum:**`, `**Scrum Master:**`, `## Opgeleverd`,
     `## Commit & PR overzicht` and the Dutch summary paragraphs are kept (clean characters /
     em-dashes only). Embedded English tech terms inside the summaries (e.g. "logging",
     "culture enforcement", "foreign key fixes") are treated as technical names and kept.
   - Each review file then **embeds the full sprint backlog** for that sprint. Apply the exact
     same treatment as the matching standalone backlog in step 3, so the two copies stay
     identical.

### Group 2B — PO NEEDS AND WISHES

5. **`Documentation/Team/2B - PO NEEDS AND WISHES/Implementing needs and wishes of the PO/Implementation Summary.md`**
   - Already fully Dutch. Edits are character cleanup only: opening-times en-dashes
     `09:00–11:00`, `11:00–13:00`, `17:00–21:00` → hyphens; trim the trailing spaces on the
     `11:00–13:00` line. Code identifiers and PR/branch references kept.

6. **`Documentation/Team/2B - PO NEEDS AND WISHES/Implementing needs and wishes of the PO/PO Interview.md`**
   - Mostly Dutch. Title `# PO Interview — Debug Diner` → `# PO Interview - Debug Diner`.
   - Arrangement list `2 course dinner / 3 course dinner / 4 course dinner / wine arrangement`
     → `2-gangendiner / 3-gangendiner / 4-gangendiner / wijnarrangement` (matches the Dutch
     summary already at the top of the file and the Implementation Summary).
   - Strip remaining em-dashes. Keep `⇒` and `[← Home]` arrows. Do **not** fix the existing
     Dutch typos (e.g. "Samenvating", "talels", "arragement") or restructure the duplicated
     sections / trailing cut-off `Q:` — content is left as authored.

7. **`Documentation/Team/2B - PO NEEDS AND WISHES/Presentations and video/Video/README_video.txt`**
   - Full English → Dutch. Suggested: `Demonstratievideo hier toevoegen (maximaal 3 minuten,
     een echt videobestand, geen link). Zie de taak Screen Recording opnemen/editen in de
     Tasks Tracker.` Keep "Screen Recording opnemen/editen" and "Tasks Tracker" as names.

8. **`Documentation/Team/2B - PO NEEDS AND WISHES/product backlog/Product Backlog - User Stories.md`**
   - This file is currently **identical** to the 2A copy (step 2). Apply the exact same
     translation so both copies remain identical.

### Group 3 — APPLICATION

9. **`Documentation/Team/3 - APPLICATION/The use of technical concepts - LAB concepts/OODP Concepts.md`**
   (the largest translation)
   - Title → Dutch, e.g. `# Objectgeoriënteerde ontwerppatronen en -concepten - Debug Diner`.
   - `## Checklist` kept. Checklist table headers `Concept | Used | Location` →
     `Concept | Gebruikt | Locatie`. The OO concept names in the Concept column and the
     `## N. <Concept>` section headings (Encapsulation, Inheritance, Interfaces, Generics,
     Enumerations, Properties, Dependency Injection, Polymorphism, Composition, Async/Await)
     stay English as established technical terminology. `Location` values: translate
     descriptive words ("All layers" → "Alle lagen") but keep code names.
   - In each section, translate the bold labels `**Where:**` → `**Waar:**`, `**Why:**` →
     `**Waarom:**`, `**Why not …:**` → `**Waarom geen …:**`, and translate the following
     paragraph prose to Dutch.
   - Keep verbatim: all code fences and their contents (apply only em-dash/en-dash → hyphen in
     code comments), file paths, type/member names, and the `GitHub:` URLs.

10. **`Documentation/Team/3 - APPLICATION/ERD _ Entity Relationship Diagram/ERD - Description.md`**
    - Title `# Entity Relationship Diagram — Debug Diner` → `# Entity Relationship Diagram -
      Debug Diner` (term kept; dash stripped).
    - First line "See `ERD - Debug Diner.png` for the visual diagram." → Dutch, keeping the
      file name. The following Dutch line and the `![img.png](…)` image stay.
    - `## Entities` → `## Entiteiten`; table header `Entity | Table | Description` →
      `Entiteit | Tabel | Omschrijving`. Entity-name and db-table-name column values kept; the
      Description column is already Dutch.
    - `## Relationships` → `## Relaties`. Relationship bullets are already Dutch; keep the bold
      English entity names and the `↔` arrow.

### Group 4 — TESTING

11. **`Documentation/Team/4 - TESTING/UAT Story - Reservering plaatsen.md`**
    - Title `# User Acceptance Test Story — Debug Diner` → `# User Acceptance Test Story -
      Debug Diner` (term kept; dash stripped).
    - Body is mostly Dutch. Strip all `—` (e.g. `### Scenario 1 — Succesvol registreren` →
      `### Scenario 1 - Succesvol registreren`). Translate any remaining English connective
      prose to Dutch.
    - Keep verbatim: UI/menu labels and view names (`Register`, `Login`, `Make Reservation`,
      `My Reservations`, `Make Reservation`), backtick app strings (`"All fields are
      required."`, `"Invalid email or password."`, `"Reservation created. Ends at: 19:00"`,
      `"This table is already reserved for this time slot."`), status names (`Pending`),
      the `✅ AVAILABLE` / `❌ TAKEN` labels, file names, and the `dotnet run` command.
    - Preserve intentional Markdown hard-break trailing double-spaces (they are functional, not
      stray whitespace).

## Out of scope

- Everything under `Documentation/Individual/` (explicit request; read-only style reference).
- Build/project artifacts: `Documentation/obj/`, `Documentation/bin/`,
  `Documentation/Documentation.csproj`.
- Binary/asset files: `.gif`, `.xlsx`, `.png`, `.pdf`.
- `Documentation/Team/2A - SCRUM/Product backlog/LINK TO PRODUCT BACKLOG.txt` — contains only
  URLs, nothing translatable; leave as-is.
- The staged `requests/2026-06-documentation.md` and the modified
  `Documentation/Individual/Soufian Manai, 1114385/Individuele Bijdrage.md` shown in
  `git status` are unrelated to this work; do not touch them.
- No fixing of pre-existing content typos, no restructuring, no content additions/removals.

## Validation

No automated test covers documentation. Validate as follows from the repo root:

1. **No em-dashes or em-dash-style en-dashes remain in Team docs** (arrows are allowed):
   ```
   rg -n --pcre2 "[\x{2014}\x{2013}]" "Documentation/Team"
   ```
   Expect **zero** matches.

2. **No smart quotes / invisible characters remain**:
   ```
   rg -n --pcre2 "[\x{2018}\x{2019}\x{201C}\x{201D}\x{00A0}\x{200B}\x{FEFF}]" "Documentation/Team"
   ```
   Expect **zero** matches.

3. **No stray English prose left.** Read each edited file and confirm sentences/labels are
   Dutch while names/identifiers/verbatim strings are unchanged. Pay attention to the two
   `Product Backlog - User Stories.md` files (must be identical) and the sprint-review files
   (embedded backlog must match its standalone counterpart).

4. **Verbatim content is intact.** `git diff` and confirm that within commit-message cells,
   code fences, file paths, URLs, and backtick UI strings the only change is em-dash/en-dash →
   hyphen; no wording was altered.

5. **Markdown still renders.** Spot-check that tables, headings, links, and code fences are
   structurally unchanged (same column counts, fences balanced). Optionally
   `dotnet build Documentation/Documentation.csproj` to confirm nothing about the docs project
   broke (it will not validate language, only that the project still builds).

6. **Encoding.** Ensure edited files are saved as UTF-8 without a BOM and without introducing
   CRLF/whitespace churn beyond the intended edits (`git diff --stat` should show only the
   files listed above).
