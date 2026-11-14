# Object Pooling Benchmark

## Environment
- BenchmarkDotNet `v0.15.8`, Linux Ubuntu 24.04.4 LTS
- CPU: AMD Ryzen 7 5800X 3.79GHz (1 CPU, 8 physical / 16 logical cores)
- Runtime: `.NET 9.0.22` (`.NET SDK 9.0.320`, X64 RyuJIT x86-64-v3)
- Job: `Throughput`, `LaunchCount=1`, `WarmupCount=2`, `IterationCount=3`

## Results

| Method | TotalSpawns | Mean | StdDev | Gen0 | Gen1 | Allocated | Alloc Ratio |
|---|---:|---:|---:|---:|---:|---:|---:|
| `NaiveSpawnAndDespawn` | `5000` | `121.1 us` | `3.02 us` | `11.9629` | `0.4883` | `204,056 B` | `1.00` |
| `PooledRentAndReturn` | `5000` | `116.9 us` | `1.12 us` | `-` | `-` | `0 B` | `0.00` |
| `NaiveSpawnAndDespawn` | `20000` | `488.2 us` | `9.91 us` | `47.8516` | `1.9531` | `804,056 B` | `1.00` |
| `PooledRentAndReturn` | `20000` | `508.1 us` | `24.52 us` | `-` | `-` | `0 B` | `0.00` |

## Analysis
1. **Zero steady-state heap allocations**: Pooling cuts managed allocations from `204,056 B` (at `5,000` spawns) and `804,056 B` (at `20,000` spawns) down to **`0 B`** (`Alloc Ratio = 0.00`), completely eliminating `Gen0` (`47.85` collections/1k ops) and `Gen1` (`1.95` collections/1k ops) GC pressure.
2. **Throughput vs. frame-time predictability**: In an isolated microbenchmark, .NET 9's bump-pointer Gen0 allocator keeps `new Projectile()` near `488 us` vs `508 us` for `Stack<T>` rent/return + reset. In a real 60 FPS game with a large live heap (textures, meshes, game state), eliminating `~785 KB` of per-cycle churn prevents unpredictable GC stop-the-world pauses.
