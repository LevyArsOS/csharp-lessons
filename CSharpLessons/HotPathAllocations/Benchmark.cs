using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpLessons.HotPathAllocations.Data;

namespace CSharpLessons.HotPathAllocations
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 2, iterationCount: 3)]
    public class Benchmark
    {
        private const int EntityCount = 1_000;
        private const int Frames = 60;
        private const float MinEnergy = 18f;

        private EntityWorld _world = null!;
        private List<int> _scratchIds = null!;

        [GlobalSetup]
        public void Setup()
        {
            _world = new EntityWorld(EntityCount);
            _scratchIds = new List<int>(EntityCount);
        }

        [Benchmark(Baseline = true)]
        public float NaiveLinqFrameLoop()
        {
            return HotPathQueries.RunNaiveLinqFrameLoop(_world, Frames, MinEnergy);
        }

        [Benchmark]
        public float ZeroAllocFrameLoop()
        {
            return HotPathQueries.RunZeroAllocFrameLoop(_world, Frames, MinEnergy, _scratchIds);
        }
    }
}
