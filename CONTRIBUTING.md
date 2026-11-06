# Contributing

Thanks for helping improve C# Lessons. Fixes, new lessons, translations and better benchmarks are
all welcome.

## Contents

- [Ways to contribute](#ways-to-contribute)
- [Development setup](#development-setup)
- [Workflow](#workflow)
- [Commit messages](#commit-messages)
- [Pull requests](#pull-requests)
- [Adding a lesson](#adding-a-lesson)
- [Style](#style)
- [Reporting bugs and vulnerabilities](#reporting-bugs-and-vulnerabilities)

## Ways to contribute

- Report a mistake in a tutorial, an example or a benchmark result.
- Improve a translation between English and Brazilian Portuguese.
- Propose or write a new lesson.

## Development setup

Prerequisite: .NET SDK 9.0.100 or a later 9.0 feature band (pinned in [global.json](global.json)).

```bash
git clone https://github.com/LevyArsOS/csharp-lessons.git
cd csharp-lessons
git switch develop
scripts/check                                   # build (warnings as errors) + unit tests
dotnet run --project CSharpLessons -c Release   # benchmarks, on demand (takes minutes)
```

VS Code users get recommended extensions and launch/test tasks from `.vscode/`.

## Workflow

The project follows gitflow: `main` holds releases, `develop` integrates the next one.

1. Open or pick an issue; every change belongs to one.
2. Agree on the design in the issue before coding (maintainers keep the detailed specification
   outside this repository).
3. Branch from `develop` as `feature/<TASK>/<slug>` (`bugfix/` for fixes; `hotfix/` from `main`),
   where `<TASK>` is the issue number and `<slug>` is lowercase kebab-case,
   e.g. `feature/12/span-lesson`.
4. Run `scripts/check` before pushing; CI runs it again.

## Commit messages

- [Conventional Commits](https://www.conventionalcommits.org/) in English:
  `<type>(<scope>)?: <description>`, e.g. `fix: correct the boxing count in the Queue example`.
- Types: `feat`, `fix`, `docs`, `test`, `refactor`, `perf`, `build`, `ci`, `chore`, `style`, `revert`.
- Signed commits only.
- At most 200 changed lines per commit; split larger work into logical steps.

## Pull requests

- Target `develop` and fill in the [template](.github/PULL_REQUEST_TEMPLATE.md).
- Reference the issue (`Closes #N`).
- CI must pass: branch and commit hygiene, secret scan, build and tests.
- Performance claims include the benchmark command and the before/after numbers.

## Adding a lesson

A lesson is a folder under `CSharpLessons/`:

| Path | Content |
|---|---|
| `Data/` | Example types used by the tutorial and the benchmarks |
| `Benchmark.cs` | BenchmarkDotNet benchmarks |
| `Benchmark.md` | Recorded results and analysis (machine, runtime, numbers) |
| `Tutorial/en.md` | Tutorial in English |
| `Tutorial/pt-br.md` | Tutorial in Brazilian Portuguese |

Then:

1. Add one unit test per example type under `CSharpLessons.Tests/<Lesson>/`.
2. Add a row to the Topics table in the [README](README.md#topics).
3. Add new terms to [GLOSSARY.md](GLOSSARY.md).

## Style

- Formatting follows [.editorconfig](.editorconfig); the build treats warnings as errors.
- Use the terms from [GLOSSARY.md](GLOSSARY.md) in code, tutorials and discussions.
- Both tutorials of a lesson cover the same content; keep them in sync.

## Reporting bugs and vulnerabilities

- Bugs and questions: open an issue with steps to reproduce and the SDK version
  (`dotnet --version`).
- Vulnerabilities: report them privately as described in [SECURITY.md](SECURITY.md).

By contributing, you agree that your contributions are licensed under the [MIT License](LICENSE).
