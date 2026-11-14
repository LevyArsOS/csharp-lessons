# Data Locality Benchmark

## Environment
- BenchmarkDotNet `v0.15.8`, Linux Ubuntu 24.04.4 LTS
- CPU: AMD Ryzen 7 5800X 3.79GHz (1 CPU, 8 physical / 16 logical cores)
- Runtime: `.NET 9.0.22` (`.NET SDK 9.0.320`, X64 RyuJIT x86-64-v3)
- Workload: `50,000` particles (`7` floats = `28 B` payload) updated over `60` frames (`dt = 0.016f`)

## Results

| Method | Mean | StdDev | Ratio | Speedup |
|---|---:|---:|---:|---:|
| `UpdateClassList` (`List<ParticleClass>`, scattered heap) | `12.811 ms` | `0.157 ms` | `1.00` | `1.00x` |
| `UpdateStructArray` (`ParticleStruct[]`, contiguous) | `3.787 ms` | `0.093 ms` | `0.30` | `3.38x` faster |

## Analysis
1. **Per-object heap tax & pointer chasing**: Each `ParticleClass` carries a 16-byte .NET object header + MethodTable pointer plus an 8-byte reference in `List<ParticleClass>`, and scattered heap references defeat CPU hardware prefetching (`12.811 ms`).
2. **Contiguous cache lines**: `ParticleStruct[]` stores 28-byte structs inline back-to-back with zero per-particle heap overhead, cutting 60-frame update time by `70.4%` (`3.38x` faster, `3.787 ms`).
