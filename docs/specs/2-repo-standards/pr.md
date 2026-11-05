# feat: repo standards

<!-- First heading = PR title (Conventional Commits). Everything below = PR body. Used by bin/task-pr. -->

## Summary
Spec: `docs/specs/2-repo-standards/spec.md`

Adopts the repository standards so every lesson lands through a reviewed, tested pull request: gitflow branches, a `scripts/check` entry point, a CI workflow, an xunit test project for the Boxing lesson, standard project files, and the removal of a per-user IDE file.

## Changes
- Remove `CSharpLessons.sln.DotSettings.user` and `.idea/`, ignore `*.user`, `*.suo`, `.vs/`, `.idea/`; translate comments in `.editorconfig` and `.vscode/tasks.json` to English.
- Add CI (`check`, `hygiene`, `secrets` jobs, actions pinned by SHA, .NET 9 via `global.json`), dependabot (actions + nuget), PR template, `CONTRIBUTING.md`, `SECURITY.md`, `GLOSSARY.md`.
- Add `scripts/check` (restore, build with `-warnaserror`, test) and `CSharpLessons.Tests` with one behaviour test per `Boxing/Data` type; register it in the solution.
- README: build/test commands and contribution pointers.
- Fix two nullability mismatches in `Counter` and `Temperature` surfaced by `-warnaserror`; pin a full SDK version in `global.json`.

## Evidence
- Before: `git ls-files '*.user'` → `CSharpLessons.sln.DotSettings.user`; `grep -cP '[^\x00-\x7F]' .editorconfig .vscode/tasks.json` → 10; no test project, no CI.
- After: `git ls-files '*.user'` → empty; the grep → 0; `bin/pin-actions --check` → clean; `ci.yml` parses; content scan of the tree → clean.
- `scripts/check` (SDK 9.0.320): first run failed the build with two nullability errors in the existing examples (`Counter.CompareTo(object)`, `Temperature.CompareTo(object)`) because of `-warnaserror`; fixed to `object?`. Second run: `Build succeeded`, `Passed! - Failed: 0, Passed: 7, Skipped: 0, Total: 7`.
- CI on this PR: `check`, `hygiene` and `secrets` green (first run failed in `setup-dotnet`: `global.json` had `9.0.0`, not a full SDK version; pinned `9.0.100` with `rollForward: latestFeature`).

## Tests
| Level | Result |
|---|---|
| Unit | 7 xunit tests in `CSharpLessons.Tests/Boxing/DataTypesTests.cs`: 7 passed locally |
| Integration | `scripts/check` end to end: green locally, repeated by the CI `check` job |
| Chaos / fault injection | N/A: no I/O in the lessons |

Coverage: not measured yet (follow-up: coverlet + threshold in `scripts/check`).

## Metrics
N/A: no performance claim.

## Merge danger
Door: two-way (revert the merge commit restores everything; no data, API or external state involved).
Blast radius: repository tooling and CI only; lesson code and tutorials untouched.

## Checklist
- [x] Spec acceptance criteria met
- [x] Tests added/updated, all passing
- [x] Docs and CHANGELOG updated where relevant
- [x] No secrets or sensitive data

Closes #2
