# C# Lessons

Practical C# lessons with runnable examples, tutorials in English and Brazilian Portuguese, and
benchmarks that measure the cost of each approach.

[![CI](https://github.com/LevyArsOS/csharp-lessons/actions/workflows/ci.yml/badge.svg?branch=develop)](https://github.com/LevyArsOS/csharp-lessons/actions/workflows/ci.yml)
[![Release](https://img.shields.io/github/v/release/LevyArsOS/csharp-lessons?sort=semver)](https://github.com/LevyArsOS/csharp-lessons/releases)
[![License: MIT](https://img.shields.io/github/license/LevyArsOS/csharp-lessons)](LICENSE)
[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![BenchmarkDotNet](https://img.shields.io/badge/benchmarks-BenchmarkDotNet-blue)](https://benchmarkdotnet.org)
[![Languages: EN | PT-BR](https://img.shields.io/badge/tutorials-EN%20%7C%20PT--BR-green)](#topics)

## Contents

- [Motivation](#motivation)
- [Topics](#topics)
- [Getting started](#getting-started)
- [Project structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

## Motivation

This project is motivated by [learning by teaching](https://en.wikipedia.org/wiki/Learning_by_teaching),
in the spirit of the **Feynman technique**: explaining a topic in simple terms to find the gaps in
your own understanding. The goal is to cover gaps identified during research, as most videos and
pages provide a non-practical view of topics, which makes it difficult to apply in real projects or
recognize problem occurrences. Another motivation is to provide content with explanations in
**Brazilian Portuguese**, which lacks quality content in this area.

## Topics

| Lesson | Tutorial | Benchmark |
|---|---|---|
| Boxing | [English](CSharpLessons/Boxing/Tutorial/en.md) · [Português](CSharpLessons/Boxing/Tutorial/pt-br.md) | [Results](CSharpLessons/Boxing/Benchmark.md) |
| Struct vs Class | [English](CSharpLessons/StructVsClass/Tutorial/en.md) · [Português](CSharpLessons/StructVsClass/Tutorial/pt-br.md) | [Results](CSharpLessons/StructVsClass/Benchmark.md) |


## Getting started

Prerequisite: .NET SDK 9.0.100 or a later 9.0 feature band (pinned in [global.json](global.json)).

```bash
git clone https://github.com/LevyArsOS/csharp-lessons.git
cd csharp-lessons
scripts/check                                   # build (warnings as errors) + unit tests
dotnet run --project CSharpLessons -c Release   # run the benchmarks (Release only; takes minutes)
```

## Project structure

Each topic is a lesson: a folder under `CSharpLessons/` with practical examples, a tutorial and
benchmarks that compare the approaches.

```text
CSharpLessons/<Lesson>/
  Data/              example types used by the tutorial and the benchmarks
  Benchmark.cs       BenchmarkDotNet benchmarks
  Benchmark.md       recorded benchmark results and analysis
  Tutorial/en.md     tutorial in English
  Tutorial/pt-br.md  tutorial in Brazilian Portuguese
CSharpLessons.Tests/<Lesson>/  unit tests for the example types
scripts/check                  build and test entry point (also run by CI)
```

## Contributing

Changes land through pull requests to `develop`; see [CONTRIBUTING.md](CONTRIBUTING.md) (including
how to add a lesson) and the shared vocabulary in [GLOSSARY.md](GLOSSARY.md). Report
vulnerabilities privately as described in [SECURITY.md](SECURITY.md). Release notes are in
[CHANGELOG.md](CHANGELOG.md).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

**[LevyArsOS](https://github.com/LevyArsOS)**

---

*This project is a work in progress. More topics will be added over time.*
