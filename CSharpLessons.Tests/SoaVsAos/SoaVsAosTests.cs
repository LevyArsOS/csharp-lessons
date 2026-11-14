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
    }
}
