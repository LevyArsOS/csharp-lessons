# SparseSet Benchmark

## Environment
- Runtime: .NET 9.0.22 (X64 RyuJIT x86-64-v3), Linux Ubuntu 24.04.4 LTS, AMD Ryzen 7 5800X
- Command: `dotnet run --project CSharpLessons -c Release -- --filter "*SparseSet*" --job short`
- Workload: 50,000 `VelocityComponent` entries across 20 iteration frames + 50,000 lookups, and 50,000 adds + 25,000 removals

## Results

| Method | Mean | Ratio | Allocated |
|---|---:|---:|---:|
| `DictionaryIterateAndLookup` | 1,637.0 us | 1.00 | 0 B |
| `SparseSetIterateAndLookup` | **1,037.6 us** | **0.63** (1.58x faster) | 0 B |
| `DictionaryAddAndRemoveChurn` | 1,291.1 us | 1.00 | 1,675,894 B |
| `SparseSetAddAndRemoveChurn` | **229.4 us** | **0.18** (**5.63x** faster) | **0 B** |

## Key Takeaways
- Iterating `SparseSet<T>.AsDenseSpan()` and indexing `_sparse[entityId]` directly is **1.58x faster** than `Dictionary<int, VelocityComponent>` iteration + hashed lookup.
- During spawn/despawn churn (50,000 adds + 25,000 removals), `SparseSet<T>` with `Clear()` + swap-and-pop runs **5.63x faster** (`229.4 us` vs `1,291.1 us`) and allocates **0 B** compared to `1,675,894 B` (and Gen0/1/2 collections) in `Dictionary`.
