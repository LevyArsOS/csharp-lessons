# SoA vs AoS + SIMD Benchmark

## Environment
- BenchmarkDotNet `v0.15.8`, Linux Ubuntu 24.04.4 LTS
- CPU: AMD Ryzen 7 5800X 3.79GHz (1 CPU, 8 physical / 16 logical cores, AVX2 `Vector<float>.Count == 8`)
- Runtime: `.NET 9.0.22` (`.NET SDK 9.0.320`, X64 RyuJIT x86-64-v3)
- Workload: `100,000` particles integrated across `60` frames (`dt = 0.016f`)

## Results

| Method | Mean | StdDev | Ratio | Speedup vs AoS |
|---|---:|---:|---:|---:|
| `IntegrateAos` (`ParticleAos[]`, `64 B` struct) | `12.238 ms` | `1.298 ms` | `1.00` | `1.00x` |
| `IntegrateSoaScalar` (`ParticleSoa`, scalar) | `6.020 ms` | `0.312 ms` | `0.50` | `2.03x` faster |
| `IntegrateSoaSimd` (`ParticleSoa`, `Vector<float>`) | `2.380 ms` | `0.187 ms` | `0.20` | `5.14x` faster |

## Analysis
1. **Cold-field cache pollution (`IntegrateAos`)**: Only 20 bytes of each 64-byte `ParticleAos` are read during integration; 44 bytes of cold color/metadata waste cache-line bandwidth (`12.238 ms`).
2. **Hot channel locality (`IntegrateSoaScalar`)**: Separating `PosX, PosY, VelX, VelY, Life` into contiguous `float[]` arrays packs 16 floats per 64-byte cache line, doubling throughput (`6.020 ms`, `2.03x` faster).
3. **SIMD vectorization (`IntegrateSoaSimd`)**: `System.Numerics.Vector<float>` processes 8 particles per AVX2 instruction (`2.380 ms`, **`5.14x` faster** than AoS).
