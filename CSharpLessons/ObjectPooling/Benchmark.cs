using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpLessons.ObjectPooling.Data;

namespace CSharpLessons.ObjectPooling
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 2, iterationCount: 3)]
    public class Benchmark
    {
        [Params(5_000, 20_000)]
        public int TotalSpawns;

        private const int BatchSize = 500;
        private const int LifetimeFrames = 4;
        private const float Dt = 0.016f;

        private ObjectPool<Projectile> _pool = null!;
        private List<Projectile> _activeScratch = null!;

        [GlobalSetup]
        public void Setup()
        {
            _pool = new ObjectPool<Projectile>(BatchSize, static () => new Projectile(), static p => p.Reset());
            _activeScratch = new List<Projectile>(BatchSize);
        }

        [Benchmark(Baseline = true)]
        public float NaiveSpawnAndDespawn()
        {
            int batches = TotalSpawns / BatchSize;
            return ProjectileSimulation.RunNaive(BatchSize, batches, LifetimeFrames, Dt);
        }

        [Benchmark]
        public float PooledRentAndReturn()
        {
            int batches = TotalSpawns / BatchSize;
            return ProjectileSimulation.RunPooled(BatchSize, batches, LifetimeFrames, Dt, _pool, _activeScratch);
        }
    }
}
