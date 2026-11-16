# DelegateCaching Benchmark

## Environment
- Runtime: .NET 9.0.22 (X64 RyuJIT x86-64-v3), Linux Ubuntu 24.04.4 LTS, AMD Ryzen 7 5800X
- Command: `dotnet run --project CSharpLessons -c Release -- --filter "*DelegateCaching*" --job short`
- Workload: 10,000 `CombatEvent` dispatches across 1,024 `EntityState` entries

## Results

| Method | Mean | Median | Speedup (Mean) | Allocated | Alloc Ratio |
|---|---:|---:|---:|---:|---:|
| `DispatchCapturingLambda` | 1,013.5 us | 542.8 us | 1.00x | 1,040,032 B | 1.00 |
| `DispatchCachedStaticDelegate` | 134.6 us | 136.4 us | 7.53x | 0 B | 0.00 |
| `DispatchInterfaceListener` | 114.9 us | 109.6 us | 8.82x | 0 B | 0.00 |
| `DispatchFunctionPointer` | 269.1 us | 262.5 us | 3.77x | 0 B | 0.00 |

## Key Takeaways
- Capturing `entities`, `ev`, and `globalModifier` inside an inline lambda allocates a compiler-generated `<>c__DisplayClass` and an `Action<int>` delegate on every event (`1,040,032 B` for 10,000 events).
- Passing state as explicit parameters to a cached `static` delegate eliminates all heap allocations (`0 B`, `7.53x` faster mean).
- Constrained generic struct dispatch (`where TListener : struct, IGameplayEventListener`) achieves the fastest execution (`114.9 us`, `8.82x` faster, `0 B`) because RyuJIT devirtualizes and inlines the struct callback directly into the loop.
- C# 9 `delegate* managed<...>` function pointers also run with `0 B` allocated (`269.1 us`, `3.77x` faster) without requiring a delegate object.
