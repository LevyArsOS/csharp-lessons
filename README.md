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

Lessons are organized into progressive learning paths—starting from allocation mechanics and GC pressure, moving to CPU cache layout and SIMD vectorization, and continuing to dispatch and concurrency.

### 1. Memory & Allocations

| Lesson | Tutorial | Benchmark | Focus |
|---|---|---|---|
| Boxing | [English](CSharpLessons/Boxing/Tutorial/en.md) · [Português](CSharpLessons/Boxing/Tutorial/pt-br.md) | [Results](CSharpLessons/Boxing/Benchmark.md) | Value-type boxing on heap & `IEquatable<T>` |
| Struct vs Class | [English](CSharpLessons/StructVsClass/Tutorial/en.md) · [Português](CSharpLessons/StructVsClass/Tutorial/pt-br.md) | [Results](CSharpLessons/StructVsClass/Benchmark.md) | Inline `readonly struct` vs 24 B heap objects |
| Object Pooling | [English](CSharpLessons/ObjectPooling/Tutorial/en.md) · [Português](CSharpLessons/ObjectPooling/Tutorial/pt-br.md) | [Results](CSharpLessons/ObjectPooling/Benchmark.md) | `ObjectPool<T>` reuse & zero steady-state GC |
| Hot-Path Allocations | [English](CSharpLessons/HotPathAllocations/Tutorial/en.md) · [Português](CSharpLessons/HotPathAllocations/Tutorial/pt-br.md) | [Results](CSharpLessons/HotPathAllocations/Benchmark.md) | `NonAlloc` scratch buffers vs per-frame LINQ |
| String Allocations | [English](CSharpLessons/StringAllocations/Tutorial/en.md) · [Português](CSharpLessons/StringAllocations/Tutorial/pt-br.md) | [Results](CSharpLessons/StringAllocations/Benchmark.md) | `Span<char>` `TryFormat` & lookup tables |

### 2. Data Layout & SIMD

| Lesson | Tutorial | Benchmark | Focus |
|---|---|---|---|
| Data Locality | [English](CSharpLessons/DataLocality/Tutorial/en.md) · [Português](CSharpLessons/DataLocality/Tutorial/pt-br.md) | [Results](CSharpLessons/DataLocality/Benchmark.md) | Contiguous `struct[]` vs scattered `List<class>` |
| SoA vs AoS (SIMD) | [English](CSharpLessons/SoaVsAos/Tutorial/en.md) · [Português](CSharpLessons/SoaVsAos/Tutorial/pt-br.md) | [Results](CSharpLessons/SoaVsAos/Benchmark.md) | Hot channel packing & AVX2 `Vector<float>` |
| Sparse Set | [English](CSharpLessons/SparseSet/Tutorial/en.md) · [Português](CSharpLessons/SparseSet/Tutorial/pt-br.md) | [Results](CSharpLessons/SparseSet/Benchmark.md) | Dense ECS `Span<T>` pools & $O(1)$ swap-and-pop |

### 3. Dispatch & Control Flow

| Lesson | Tutorial | Benchmark | Focus |
|---|---|---|---|
| Delegate Caching | [English](CSharpLessons/DelegateCaching/Tutorial/en.md) · [Português](CSharpLessons/DelegateCaching/Tutorial/pt-br.md) | [Results](CSharpLessons/DelegateCaching/Benchmark.md) | Cached `static` delegates, constrained structs, & `delegate*` |

### 4. Concurrency & Serialization

*Planned:* lock contention, `ValueTask` vs. `Task`, and low-allocation serialization pipelines.


## Getting started

Prerequisite: .NET SDK 9.0.100 or a later 9.0 feature band (pinned in [global.json](global.json)).

```bash
git clone https://github.com/LevyArsOS/csharp-lessons.git
cd csharp-lessons
scripts/check                                   # build (warnings as errors) + unit tests
dotnet run --project CSharpLessons -c Release   # run the benchmarks (Release only; takes minutes)
```

`Program.cs` forwards CLI arguments to BenchmarkDotNet's `BenchmarkSwitcher`, so you can filter
lessons, shorten jobs, or enable memory and disassembly diagnostics on demand:

```bash
dotnet run --project CSharpLessons -c Release -- --filter "*SoaVsAos*" --job short --disasm
dotnet run --project CSharpLessons -c Release -- --filter "*Boxing*" --job short --memory
```

`--disasm` enables BenchmarkDotNet's `DisassemblyDiagnoser` and exports the JIT-compiled machine
code under `BenchmarkDotNet.Artifacts/results/` (useful for inspecting SIMD vectorization and
bounds-check elimination).

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
