# Contributing

1. Open or pick an issue; every change belongs to one.
2. Write the specification in `docs/specs/<TASK>-<slug>/spec.md` and get it approved before coding.
3. Branch from `develop` as `feature/<TASK>/<slug>` (`bugfix/` for fixes; `hotfix/` from `main`).
4. Commits: [Conventional Commits](https://www.conventionalcommits.org/), English, signed,
   at most 200 changed lines each.
5. Run `scripts/check` (build and tests, .NET 9 SDK) before pushing; CI runs it again.
   Benchmarks run on demand: `dotnet run --project CSharpLessons -c Release`.
6. Open the pull request against `develop` using the template.

## Adding a lesson
A lesson is a folder under `CSharpLessons/` with `Data/` (example types), `Benchmark.cs`,
`Benchmark.md` (results) and `Tutorial/en.md` plus `Tutorial/pt-br.md`. Add one unit test per
example type under `CSharpLessons.Tests/<Lesson>/` and a line in the README topics list.
