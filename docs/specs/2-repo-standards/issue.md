# Adopt repository standards: gitflow, checks, CI and project files

Labels: feature

## Summary
The repository has a single `master` branch, no automated checks, no CI and a user-specific IDE file under version control. This task adds the standard project files, a `scripts/check` entry point that builds and runs the tests, a CI workflow, and the gitflow branch layout, so every later lesson lands through a reviewed pull request.

## Current behavior
- Only `master`; commits land on it directly.
- `CSharpLessons.sln.DotSettings.user` (per-user Rider settings) is tracked.
- No test project: nothing verifies that the examples compile or that the benchmarks still run.
- No CI, no pull request template, no contributing notes.
- `.editorconfig` and `.vscode/tasks.json` carry Portuguese comments while the code and README are in English.

## Expected behavior
- `main` and `develop` exist; `main` only moves through pull requests.
- `scripts/check` builds the solution and runs the test project; CI runs it on every pull request.
- A test project covers the lesson helpers (the boxing examples) so each lesson ships with proof.
- The user-specific IDE file is removed and ignored; comments in config files are in English.
- CONTRIBUTING, SECURITY, PR template, dependabot and GLOSSARY are in place.

## Acceptance criteria
- [ ] Given a fresh clone, when `scripts/check` runs with the .NET 9 SDK, then the solution builds and all tests pass.
- [ ] Given a pull request, when CI runs, then the check, hygiene and secrets jobs run and pass.
- [ ] Given the repository tree, when it is scanned, then no `*.user` file is tracked and `.gitignore` excludes it.
- [ ] Given the Boxing lesson, when its tests run, then at least one test per example data type passes.

## Additional context
Tutorials in Brazilian Portuguese (`Tutorial/pt-br.md`) are a feature of this project and stay as they are.
