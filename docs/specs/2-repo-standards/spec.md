# 2: repo standards

Status: Draft
Type: feature
Created: 2026-11-05

<!-- Status flow: Draft -> Approved (set only after the user approves) -> Done (all criteria checked, before the PR). Code commits are rejected until Approved. -->

## Context
The repository stores C# lessons (one folder per topic with examples, a tutorial in English and Brazilian Portuguese, and a BenchmarkDotNet benchmark). It has three commits on `master`, no tests, no CI and a tracked per-user Rider settings file. Nothing checks that a lesson compiles before it is published.

## Goal
Every lesson lands through a pull request that CI has built and tested, with the standard project files in place.

## Scope
### In scope
- Gitflow branches (`main`, `develop`), standard project files (`CONTRIBUTING.md`, `SECURITY.md`, `GLOSSARY.md`, PR template, dependabot, CI workflow).
- `scripts/check`: restore, build (warnings as errors) and test.
- A `CSharpLessons.Tests` xunit project with tests for the Boxing lesson data helpers.
- Remove `CSharpLessons.sln.DotSettings.user` and ignore `*.user`; translate config comments to English.
### Out of scope
- Rewriting the existing commit history (signing, conventional messages); the branch rules apply from now on.
- Renaming the remote default branch on GitHub (done by the owner in repository settings).
- New lessons or changes to tutorial content.

## Requirements
### Functional
- FR1: `scripts/check` exits non-zero when the build or any test fails, zero otherwise.
- FR2: CI runs `scripts/check`, commit hygiene and secret scanning on pull requests to `main` and `develop`.
- FR3: The test project references the lesson project and covers each `Boxing/Data` type with at least one behaviour test.
### Non-functional
- NFR1: `scripts/check` completes in under two minutes on the CI runner (benchmarks are excluded from the check; they run on demand).
- NFR2: No user-specific or machine-specific file is tracked.

## Acceptance criteria
- [ ] AC1: Given a clean clone with the .NET 9 SDK, when `scripts/check` runs, then the solution builds with zero warnings and all tests pass.
- [ ] AC2: Given a pull request, when CI runs, then the `check`, `hygiene` and `secrets` jobs succeed.
- [ ] AC3: Given the tracked files, when listed, then no `*.user` file is present and `.gitignore` contains `*.user`.
- [ ] AC4: Given `CSharpLessons.Tests`, when run, then one test per `Boxing/Data` type passes.
- [ ] AC5: Given `.editorconfig` and `.vscode/tasks.json`, when read, then every comment is in English.

## Verified context
Base commit: a28f114 (`master`, renamed locally to `main`).

| Fact | Command | Result |
|---|---|---|
| No test project | `ls *.sln; grep -c Tests CSharpLessons.sln` | `CSharpLessons.sln`; 0 |
| User file tracked | `git ls-files '*.user'` | `CSharpLessons.sln.DotSettings.user` |
| Portuguese comments | `grep -nP '[^\x00-\x7F]' .editorconfig .vscode/tasks.json \| wc -l` | 10 |
| SDK pinned | `cat global.json` | 9.0.0, rollForward latestFeature |

## Design
- `scripts/check` (bash): `dotnet restore`, `dotnet build -warnaserror --no-restore`, `dotnet test --no-build`. Benchmarks are not executed (minutes long); the benchmark project still compiles.
- `CSharpLessons.Tests` (xunit, net9.0) next to `CSharpLessons`, added to the solution. Tests call the public helpers in `Boxing/Data` and assert behaviour, not timings.
- CI from the workspace project template: `check` installs the .NET 9 SDK (`actions/setup-dotnet`, pinned by SHA) then runs `scripts/check`.
- Alternatives rejected: running benchmarks in CI (too slow, noisy numbers on shared runners); a Makefile instead of `scripts/check` (the workspace convention is `scripts/check`).

## Implementation plan
| # | Step | Files | Proof | Depends on | Parallel with |
|---|---|---|---|---|---|
| 1 | Remove the user file, ignore `*.user`, translate config comments | `.gitignore`, `*.user`, `.editorconfig`, `.vscode/tasks.json` | AC3, AC5 (`git ls-files`, grep) | — | 2, 3 |
| 2 | Standard project files | `CONTRIBUTING.md`, `SECURITY.md`, `GLOSSARY.md`, `.github/*` | files present, CI yaml valid | — | 1, 3 |
| 3 | `scripts/check` and the test project with one test per data type | `scripts/check`, `CSharpLessons.Tests/*`, `CSharpLessons.sln` | AC1, AC4 (`scripts/check`) | — | 1, 2 |
| 4 | CI job installs .NET and runs the check; README gains the check command | `.github/workflows/ci.yml`, `README.md` | AC2 (PR checks green) | 2, 3 | — |

## Test plan
| Level | What is covered | Location |
|---|---|---|
| Unit | Boxing data helpers: equality, boxing of value types, tuple/ValueTuple behaviour | `CSharpLessons.Tests/Boxing/` |
| Integration | `scripts/check` end to end (build + test) | CI `check` job |
| Chaos / fault injection | N/A: no I/O or external dependency in the lessons | — |
| End-to-end / contract | N/A | — |

Coverage target: every public helper in `Boxing/Data` called by at least one test (coverage tooling for .NET is a follow-up).

## Metrics
N/A: no performance claim in this task. Benchmarks keep their own numbers in `Benchmark.md`.

## Decisions
| ID | Decision | Status | Default / rationale |
|---|---|---|---|
| D1 | Keep `.idea/` tracked | open | default: keep (shared Rider settings), remove only if the owner prefers |
| D2 | Benchmarks excluded from `scripts/check` | closed | minutes per run, numbers not comparable on CI |
| D3 | Remote default branch renamed to `main` | open | owner action on GitHub; until then `main` tracks `origin/master` |

## Gaps
| Gap | Workaround used | Impact |
|---|---|---|
| No .NET SDK on the authoring machine | tests and `scripts/check` validated in CI only | AC1/AC4 proven by the CI run, not locally |

## Risks and rollback
- Risk: `-warnaserror` surfaces existing warnings in lesson code. Rollback: fix the warnings in the same PR or drop the flag with a note.
- Rollback of the whole task: revert the merge commit; no data or external state involved.
