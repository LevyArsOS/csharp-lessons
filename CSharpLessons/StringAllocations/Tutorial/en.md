# String Allocations in Combat Text and HUD Formatting

## Problem
Action RPGs, roguelikes, and strategy games often display hundreds or thousands of floating damage numbers and HUD counters per frame (`"#42 -185 (815/1000)"`). Building these strings every frame via `+` concatenation or `$"..."` interpolation allocates multiple temporary `string` objects on the managed heap (`549,880 B` for `2,000` events), triggering frequent Gen0 garbage collections.

## Solution
1. **`Span<char>` + `TryFormat`:** Write formatted numbers directly into a reusable `char[]` or `stackalloc char[64]` buffer using `int.TryFormat(..., CultureInfo.InvariantCulture)` and pass `ReadOnlySpan<char>` to the text renderer (`0 B` allocated, `3.59x` faster).
2. **Reusable `StringBuilder`:** When a mutable character buffer is preferred, `Clear()` and reuse a single `StringBuilder` instance (`0 B` allocated, `1.49x` faster).
3. **Pre-Allocated Lookup Tables:** When an external UI API requires a `string` reference and values fall within a small range (`0..999`), cache the formatted strings at startup (`0 B` allocated, `123.65x` faster).

## Benchmark Summary
On .NET 9 (`2,000` combat events per frame), `Span<char>` `TryFormat` reduces formatting time from `204.518 us` (`549,880 B` allocated) to `56.945 us` (`0 B` allocated, `3.59x` faster).
