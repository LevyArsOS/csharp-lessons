# Benchmark Results - Struct vs Class

This document presents benchmark results comparing reference types (`class Vec2Class`) and value types (`readonly struct Vec2Struct`) for 2D vectors in C#. Tests were executed using BenchmarkDotNet on an AMD Ryzen 7 5800X with .NET 9.0.

## Test Environment

- **Hardware**: AMD Ryzen 7 5800X 3.79GHz, 1 CPU, 16 logical and 8 physical cores
- **Runtime**: .NET 9.0.22, X64 RyuJIT x86-64-v3
- **GC**: Concurrent Workstation
- **Tool**: BenchmarkDotNet v0.15.8

---

## 1. Vector Summation

Iterative vector summation calculating the cumulative sum across arrays of 100, 1,000, and 100,000 vectors.

### Comparative Table - Summation

| Method | Count | Mean Time | Allocated | Ratio vs Baseline | Alloc Ratio |
| ------ | ----- | --------- | --------- | ----------------- | ----------- |
| **SumClass** (Baseline) | 100 | 967.41 ns | 2,424 B | 1.00x | 1.00 |
| **SumStruct** | 100 | 334.34 ns | 0 B | 0.35x | 0.00 |
| **SumClass** (Baseline) | 1,000 | 10,339.29 ns | 24,024 B | 1.00x | 1.00 |
| **SumStruct** | 1,000 | 3,309.52 ns | 0 B | 0.32x | 0.00 |
| **SumClass** (Baseline) | 100,000 | 1,116,317.71 ns | 2,400,024 B | 1.00x | 1.00 |
| **SumStruct** | 100,000 | 342,009.69 ns | 0 B | 0.31x | 0.00 |

### Analysis - Summation

- **Allocation**: `SumClass` allocates ~24 bytes per element for intermediate objects (2.4 MB for 100k vectors), triggering dozens of GC Gen0 sweeps. `SumStruct` allocates **0 B** on the managed heap.
- **Throughput**: `SumStruct` is approximately **3.2x faster** than `SumClass` due to avoiding heap allocation and GC pressure.

---

## 2. In-Place Entity Transform Integration

Position update loop simulating a game frame step (`position += velocity * dt`) across 100, 1,000, and 100,000 entities.

### Comparative Table - Integration

| Method | Count | Mean Time | Allocated | Ratio vs Baseline | Alloc Ratio |
| ------ | ----- | --------- | --------- | ----------------- | ----------- |
| **IntegrateClass** | 100 | 2,677.17 ns | 4,800 B | 2.77x | 1.98 |
| **IntegrateStruct** | 100 | 96.15 ns | 0 B | 0.10x | 0.00 |
| **IntegrateClass** | 1,000 | 21,497.26 ns | 48,000 B | 2.09x | 2.00 |
| **IntegrateStruct** | 1,000 | 963.74 ns | 0 B | 0.09x | 0.00 |
| **IntegrateClass** | 100,000 | 5,165,243.31 ns | 4,800,000 B | 4.65x | 2.00 |
| **IntegrateStruct** | 100,000 | 94,371.10 ns | 0 B | 0.08x | 0.00 |

### Analysis - Integration

- **Throughput**: `IntegrateStruct` executes in **94 microseconds** for 100,000 entities, compared to **5,165 microseconds** (5.16 ms) for `IntegrateClass` — a **54x performance improvement**.
- **Allocation**: `IntegrateClass` allocates 4.8 MB every frame step, while `IntegrateStruct` runs completely in-place with **0 bytes allocated**.
- **Cache Locality**: Struct arrays pack elements contiguously in memory (`[X, Y, X, Y...]`), whereas class arrays store 64-bit pointers pointing to scattered heap objects, suffering massive CPU cache misses.
