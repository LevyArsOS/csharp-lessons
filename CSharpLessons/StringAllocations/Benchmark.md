# String Allocations Benchmark Results

## Environment
- **BenchmarkDotNet:** `v0.15.8`, Ubuntu 24.04.4 LTS (Noble Numbat)
- **CPU:** AMD Ryzen 7 5800X 3.79GHz (1 CPU, 16 logical / 8 physical cores)
- **Runtime:** `.NET 9.0.22` (`9.0.2226.52218`), X64 RyuJIT `x86-64-v3`, SDK `9.0.320`
- **Workload:** `2,000` combat events formatted per frame (`#TargetId -Damage (RemainingHp/MaxHp)`).

## Results

| Method | Mean | StdDev | Ratio | Gen0 | Allocated | Speedup |
|---|---:|---:|---:|---:|---:|---:|
| `FormatNaiveConcatenation` | `204.518 us` | `3.462 us` | `1.000` | `32.7148` | `549,880 B` | `1.00x` |
| `FormatReusableStringBuilder` | `137.365 us` | `0.757 us` | `0.672` | `-` | `0 B` | `1.49x` |
| `FormatSpanTryFormat` | `56.945 us` | `0.762 us` | `0.278` | `-` | `0 B` | `3.59x` |
| `LookupPreallocatedDamageStrings` | `1.654 us` | `0.020 us` | `0.008` | `-` | `0 B` | `123.65x` |

## Analysis
1. **Per-Frame String Concatenation (`204.518 us`, `549,880 B`):** Formatting `2,000` combat events with `+` / string concatenation allocates four temporary `int.ToString()` strings plus the final concatenated `string` per event (`~550 KB` per frame, or `~33 MB/s` at 60 FPS).
2. **Reusable `StringBuilder` (`137.365 us`, `0 B`, `1.49x` faster):** Calling `builder.Clear()` and appending integers directly into a pre-sized `StringBuilder` eliminates heap allocations, though chunk management and virtual/bounds checks remain.
3. **`Span<char>` + `ISpanFormattable.TryFormat` (`56.945 us`, `0 B`, `3.59x` faster):** Formatting integers directly into a caller-supplied `Span<char>` buffer writes UTF-16 characters sequentially in L1 cache with zero heap allocations.
4. **Pre-Allocated String Table (`1.654 us`, `0 B`, `123.65x` faster):** When a legacy UI widget requires an actual `string` reference for bounded numbers (`0..999`), indexing a pre-allocated lookup array avoids both formatting and allocation.
