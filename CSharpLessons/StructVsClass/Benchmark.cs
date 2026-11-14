using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpLessons.StructVsClass.Data;

namespace CSharpLessons.StructVsClass
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 2, iterationCount: 3)]
    public class Benchmark
    {
        [Params(100, 1_000, 100_000)]
        public int Count;

        private Vec2Class[] classPositions = null!;
        private Vec2Class[] classVelocities = null!;
        private Vec2Struct[] structPositions = null!;
        private Vec2Struct[] structVelocities = null!;

        [GlobalSetup]
        public void Setup()
        {
            (classPositions, classVelocities) = EntityTransformSimulator.CreateClassEntities(Count);
            (structPositions, structVelocities) = EntityTransformSimulator.CreateStructEntities(Count);
        }

        [Benchmark(Baseline = true)]
        public Vec2Class SumClass()
        {
            return EntityTransformSimulator.SumClass(classPositions);
        }

        [Benchmark]
        public Vec2Struct SumStruct()
        {
            return EntityTransformSimulator.SumStruct(structPositions);
        }

        [Benchmark]
        public void IntegrateClass()
        {
            EntityTransformSimulator.IntegrateClass(classPositions, classVelocities, 0.016f);
        }

        [Benchmark]
        public void IntegrateStruct()
        {
            EntityTransformSimulator.IntegrateStruct(structPositions, structVelocities, 0.016f);
        }
    }
}
