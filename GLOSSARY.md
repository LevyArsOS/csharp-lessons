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
| Value type | A data type stored inline where declared without heap metadata overhead (e.g. `struct`) | A reference type |
| Reference type | A data type stored on the managed heap with an object header and method table pointer (e.g. `class`) | A value type |
| Inline allocation | Storing struct fields directly in stack memory or consecutively inside an array without separate heap allocations | Heap allocation with pointers |
| Object pool | A pre-allocated collection of reusable class instances acquired via `Rent()` and returned via `Return()` with deterministic state reset | Per-spawn `new` heap allocation |
| NonAlloc pattern | Filling a caller-owned pre-allocated buffer (`Clear()` + reuse) instead of allocating a new collection per frame | Returning a fresh `ToList()` per frame |
| Data locality | Arranging hot fields sequentially in memory so sequential iterations hit contiguous 64-byte CPU cache lines | Scattered heap object references |
| Cache line | The fixed 64-byte block of memory transferred between RAM and CPU caches (L1/L2/L3) on every read | A single variable or field size |
| Hardware prefetcher | CPU logic that detects sequential or constant-stride memory reads and fetches upcoming cache lines ahead of time | Software `Prefetch` instructions or GC |
| Pointer chasing | Following scattered heap references across non-contiguous memory addresses, defeating hardware prefetching | Sequential array indexing |
| SIMD (SoA) | Single Instruction, Multiple Data vector operations (`Vector<float>`) over parallel Structure-of-Arrays channels | Scalar Array-of-Structures loop |
| Span formatting (`TryFormat`) | Writing formatted text directly into a caller-supplied `Span<char>` buffer via `ISpanFormattable.TryFormat` with zero heap allocations | Allocating new `string` instances via `+` or `ToString()` |

