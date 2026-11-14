using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpLessons.DataLocality.Data;

namespace CSharpLessons.DataLocality
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 2, iterationCount: 3)]
    public class Benchmark
    {
        private const int ParticleCount = 50_000;
        private const int Frames = 60;
        private const float Dt = 0.016f;

        private List<ParticleClass> _classList = null!;
        private ParticleStruct[] _structArray = null!;

        [GlobalSetup]
        public void Setup()
        {
            _classList = ParticleUpdateSimulator.CreateClassList(ParticleCount, scatterHeap: true);
            _structArray = ParticleUpdateSimulator.CreateStructArray(ParticleCount);
        }

        [Benchmark(Baseline = true)]
        public double UpdateClassList()
        {
            return ParticleUpdateSimulator.UpdateClassList(_classList, Dt, Frames);
        }

        [Benchmark]
        public double UpdateStructArray()
        {
            return ParticleUpdateSimulator.UpdateStructArray(_structArray, Dt, Frames);
        }
    }
}
