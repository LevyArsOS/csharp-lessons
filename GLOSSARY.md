# Glossary

Shared vocabulary of this project. Lessons, code names, commits and discussions use these terms
exactly; a concept that needs a sentence to explain gets an entry here instead.

| Term | Meaning | Not to be confused with |
|---|---|---|
| Lesson | One topic folder under `CSharpLessons/` with examples, a benchmark and a tutorial in English and Brazilian Portuguese | A single tutorial file |
| Tutorial | The explanatory text of a lesson (`Tutorial/en.md`, `Tutorial/pt-br.md`) | The benchmark report (`Benchmark.md`) |
| Benchmark | A BenchmarkDotNet class measuring the lesson's scenarios; run on demand, never in `scripts/check` | A unit test |
| Boxing | Wrapping a value type in a heap object when it is treated as `object` or a non-generic interface | Casting between value types |
| Specialized override | A struct implementing `IEquatable<T>` / `IComparable<T>` so equality and ordering avoid boxing | Overriding `Equals(object)` alone |
