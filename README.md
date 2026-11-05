# C# Lessons

## Motivation

This project is motivated by learning through teaching, following the **[Feynman technique](https://en.wikipedia.org/wiki/Learning_by_teaching#Plastic_platypus_learning)**. The goal is to cover gaps identified during research, as most videos and pages provide a non-practical view of topics, which makes it difficult to apply in real projects or recognize problem occurrences. Another motivation is to provide content with explanations in **Brazilian Portuguese**, which lacks quality content in this area.

## Project Structure

This is a C# repository to store C# lessons with practical examples and benchmarks. Each folder within the project represents a different topic with practical examples and benchmarks. This approach helps understand concepts through real-world scenarios and performance comparisons.

## Building and testing

```bash
scripts/check                                   # build (warnings as errors) + unit tests; needs the .NET 9 SDK
dotnet run --project CSharpLessons -c Release   # run the benchmarks (minutes)
```

Changes land through pull requests to `develop`; see [CONTRIBUTING.md](CONTRIBUTING.md) and
the shared vocabulary in [GLOSSARY.md](GLOSSARY.md).

## Topics

- Boxing [English](CSharpLessons/Boxing/Tutorial/en.md) [Português](CSharpLessons/Boxing/Tutorial/pt-br.md)
  - [Benchmark](CSharpLessons/Boxing/Benchmark.md)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

**LevyArsOS**

---

*This project is a work in progress. More topics will be added over time.*
