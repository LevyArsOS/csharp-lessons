using System;
using CSharpLessons.SoaVsAos.Data;
using Xunit;

namespace CSharpLessons.Tests.SoaVsAos
{
    public class SoaVsAosTests
    {
        [Fact]
        public void IntegrateAllThreeLayouts_ProduceEquivalentPositionsAndChecksums()
        {
            const int count = 100_000;
            const int frames = 10;
            const float dt = 0.016f;

            ParticleAos[] aos = ParticleIntegrator.CreateAos(count);
            ParticleSoa soaScalar = ParticleIntegrator.CreateSoa(count);
            ParticleSoa soaSimd = ParticleIntegrator.CreateSoa(count);

            double sumAos = ParticleIntegrator.IntegrateAos(aos, dt, frames);
            double sumScalar = ParticleIntegrator.IntegrateSoaScalar(soaScalar, dt, frames);
            double sumSimd = ParticleIntegrator.IntegrateSoaSimd(soaSimd, dt, frames);

            Assert.Equal(sumAos, sumScalar, precision: 2);
            Assert.Equal(sumAos, sumSimd, precision: 2);

            for (int i = 0; i < 64; i++)
            {
                Assert.Equal(aos[i].X, soaSimd.PosX[i], precision: 4);
                Assert.Equal(aos[i].Y, soaSimd.PosY[i], precision: 4);
                Assert.Equal(aos[i].Life, soaSimd.Life[i], precision: 4);
            }
        }

        [Fact]
        public void IntegrateSoaSimd_HandlesNonMultipleOfVectorWidthTail()
        {
            const int count = 13;
            const float dt = 0.016f;

            ParticleAos[] aos = ParticleIntegrator.CreateAos(count);
            ParticleSoa soaSimd = ParticleIntegrator.CreateSoa(count);

            double sumAos = ParticleIntegrator.IntegrateAos(aos, dt, 5);
            double sumSimd = ParticleIntegrator.IntegrateSoaSimd(soaSimd, dt, 5);

            Assert.Equal(sumAos, sumSimd, precision: 4);
        }

        [Fact]
        public void EmptyAndInvalidArguments_BehaveSafely()
        {
            Assert.Equal(0.0, ParticleIntegrator.IntegrateAos(Array.Empty<ParticleAos>(), 0.016f, 3));
            Assert.Equal(0.0, ParticleIntegrator.IntegrateSoaScalar(new ParticleSoa(0), 0.016f, 3));
            Assert.Equal(0.0, ParticleIntegrator.IntegrateSoaSimd(new ParticleSoa(0), 0.016f, 3));

            Assert.Throws<ArgumentOutOfRangeException>(() => ParticleIntegrator.CreateAos(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ParticleSoa(-1));
            Assert.Throws<ArgumentNullException>(() => ParticleIntegrator.IntegrateAos(null!, 0.016f, 1));
            Assert.Throws<ArgumentNullException>(() => ParticleIntegrator.IntegrateSoaScalar(null!, 0.016f, 1));
            Assert.Throws<ArgumentNullException>(() => ParticleIntegrator.IntegrateSoaSimd(null!, 0.016f, 1));
        }

        [Fact]
        public void IntegrateAosAndSoa_AllocateZeroBytesOnHotPath()
        {
            const int count = 1_024;
            const int frames = 5;
            const float dt = 0.016f;

            ParticleAos[] aos = ParticleIntegrator.CreateAos(count);
            ParticleSoa soaScalar = ParticleIntegrator.CreateSoa(count);
            ParticleSoa soaSimd = ParticleIntegrator.CreateSoa(count);

            // Warm up JIT before measuring thread allocations
            _ = ParticleIntegrator.IntegrateAos(aos, dt, 1);
            _ = ParticleIntegrator.IntegrateSoaScalar(soaScalar, dt, 1);
            _ = ParticleIntegrator.IntegrateSoaSimd(soaSimd, dt, 1);

            long aosBefore = GC.GetAllocatedBytesForCurrentThread();
            double aosSum = ParticleIntegrator.IntegrateAos(aos, dt, frames);
            long aosAfter = GC.GetAllocatedBytesForCurrentThread();

            long scalarBefore = GC.GetAllocatedBytesForCurrentThread();
            double scalarSum = ParticleIntegrator.IntegrateSoaScalar(soaScalar, dt, frames);
            long scalarAfter = GC.GetAllocatedBytesForCurrentThread();

            long simdBefore = GC.GetAllocatedBytesForCurrentThread();
            double simdSum = ParticleIntegrator.IntegrateSoaSimd(soaSimd, dt, frames);
            long simdAfter = GC.GetAllocatedBytesForCurrentThread();

            Assert.Equal(0, aosAfter - aosBefore);
            Assert.Equal(0, scalarAfter - scalarBefore);
            Assert.Equal(0, simdAfter - simdBefore);
            Assert.Equal(aosSum, scalarSum, precision: 2);
            Assert.Equal(aosSum, simdSum, precision: 2);
        }
    }
}
