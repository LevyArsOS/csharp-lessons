using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSharpLessons.SoaVsAos.Data;

namespace CSharpLessons.SoaVsAos
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.Throughput, launchCount: 1, warmupCount: 2, iterationCount: 3)]
    public class Benchmark
    {
        private const int ParticleCount = 100_000;
        private const int Frames = 60;
        private const float Dt = 0.016f;

        private ParticleAos[] _aos = null!;
        private ParticleSoa _soaScalar = null!;
        private ParticleSoa _soaSimd = null!;

        [GlobalSetup]
        public void Setup()
        {
            _aos = ParticleIntegrator.CreateAos(ParticleCount);
            _soaScalar = ParticleIntegrator.CreateSoa(ParticleCount);
            _soaSimd = ParticleIntegrator.CreateSoa(ParticleCount);
        }

        [Benchmark(Baseline = true)]
        public double IntegrateAos()
        {
            return ParticleIntegrator.IntegrateAos(_aos, Dt, Frames);
        }

        [Benchmark]
        public double IntegrateSoaScalar()
        {
            return ParticleIntegrator.IntegrateSoaScalar(_soaScalar, Dt, Frames);
        }

        [Benchmark]
        public double IntegrateSoaSimd()
        {
            return ParticleIntegrator.IntegrateSoaSimd(_soaSimd, Dt, Frames);
        }
    }
}
