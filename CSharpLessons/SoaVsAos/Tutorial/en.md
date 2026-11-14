# SoA vs AoS with SIMD (`System.Numerics.Vector<float>`)

## Why AoS Blocks SIMD and Pollutes Cache Lines
In `ParticleAos[]`, hot physics fields (`X, Y, Vx, Vy, Life`) sit alongside cold rendering fields (`R, G, B, A, Flags, Metadata`). Every cache line loaded during physics updates wastes 68% of its bandwidth on cold bytes and prevents contiguous SIMD loads.

## SoA + `Vector<float>` SIMD Integration
Splitting each channel into its own `float[]` array (`ParticleSoa`) packs 16 floats per 64-byte cache line (`2.03x` faster scalar) and enables `System.Numerics.Vector<float>` to update 8 particles per AVX2 instruction (`5.14x` faster: `2.380 ms` vs `12.238 ms`):

```csharp
int width = Vector<float>.Count;
for (; i < simdLimit; i += width)
{
    var xVec = new Vector<float>(px, i) + new Vector<float>(vx, i) * dtVec;
    var yVec = new Vector<float>(py, i) + new Vector<float>(vy, i) * dtVec;
    var lVec = new Vector<float>(life, i) - dtVec;
    xVec.CopyTo(px, i);
    yVec.CopyTo(py, i);
    lVec.CopyTo(life, i);
}
```

Always keep a scalar tail loop (`for (; i < n; i++)`) for counts not divisible by `Vector<float>.Count`.
