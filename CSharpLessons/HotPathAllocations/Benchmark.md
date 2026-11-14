# Hot-Path Allocations Benchmark

## Environment
- BenchmarkDotNet `v0.15.8`, Linux Ubuntu 24.04.4 LTS
- CPU: AMD Ryzen 7 5800X 3.79GHz (1 CPU, 8 physical / 16 logical cores)
- Runtime: `.NET 9.0.22` (`.NET SDK 9.0.320`, X64 RyuJIT x86-64-v3)
- Workload: `1,000` entities queried and updated over `60` frames (`1` second at `60 FPS`)

## Results

| Method | Mean | StdDev | Ratio | Gen0 | Allocated | Alloc Ratio |
|---|---:|---:|---:|---:|---:|---:|
| `NaiveLinqFrameLoop` | `242.01 us` | `3.962 us` | `1.00` | `8.7891` | `149,280 B` | `1.00` |
| `ZeroAllocFrameLoop` | `96.94 us` | `2.350 us` | `0.40` | `-` | `0 B` | `0.00` |

## Analysis
1. **Eliminating per-frame LINQ churn**: Calling `.Where(...).Select(...).ToList()` every frame allocates iterator state machines, closures, and a fresh `List<int>` backing array on every tick—accumulating `149,280 B` (`~145.8 KB`) and `8.79` Gen0 collections per 1,000 60-frame runs.
2. **2.50x faster with `0 B` allocated**: Reusing a caller-owned `List<int>` scratch buffer (`QueryActiveNonAlloc`) and indexed `for` loops cuts 60-frame execution time from `242.01 us` down to `96.94 us` (`2.50x` faster) with `0 B` on the managed heap.
