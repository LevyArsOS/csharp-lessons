# SoA vs AoS com SIMD (`System.Numerics.Vector<float>`)

## Por que AoS Impede SIMD e Polui Linhas de Cache
Em `ParticleAos[]`, campos quentes de física (`X, Y, Vx, Vy, Life`) dividem espaço com dados frios de renderização (`R, G, B, A, Flags, Metadata`). Cada linha de cache carregada desperdiça 68% da banda com bytes frios e impede carregamentos SIMD contíguos.

## SoA + Integração SIMD com `Vector<float>`
Separar cada canal em seu próprio `float[]` (`ParticleSoa`) empacota 16 floats por linha de cache (`2.03x` mais rápido em escalar) e permite que `System.Numerics.Vector<float>` atualize 8 partículas por instrução AVX2 (`5.14x` mais rápido: `2.380 ms` vs `12.238 ms`):

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

Mantenha sempre um loop escalar residual (`for (; i < n; i++)`) para quantidades que não são múltiplas de `Vector<float>.Count`.
