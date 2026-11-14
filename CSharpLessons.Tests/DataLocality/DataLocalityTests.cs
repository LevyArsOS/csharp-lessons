using System;
using System.Collections.Generic;
using CSharpLessons.DataLocality.Data;
using Xunit;

namespace CSharpLessons.Tests.DataLocality
{
    public class DataLocalityTests
    {
        [Fact]
        public void UpdateStructArray_MatchesClassListFinalChecksum_Across50kParticles()
        {
            const int count = 50_000;
            const int frames = 10;
            const float dt = 0.016f;

            List<ParticleClass> classList = ParticleUpdateSimulator.CreateClassList(count, scatterHeap: true);
            ParticleStruct[] structArray = ParticleUpdateSimulator.CreateStructArray(count);

            double classSum = ParticleUpdateSimulator.UpdateClassList(classList, dt, frames);

            // Warmup before checking thread allocations
            var warmup = ParticleUpdateSimulator.CreateStructArray(16);
            _ = ParticleUpdateSimulator.UpdateStructArray(warmup, dt, 1);

            long before = GC.GetAllocatedBytesForCurrentThread();
            double structSum = ParticleUpdateSimulator.UpdateStructArray(structArray, dt, frames);
            long after = GC.GetAllocatedBytesForCurrentThread();

            Assert.Equal(classSum, structSum, precision: 2);
            Assert.Equal(0, after - before);
        }

        [Fact]
        public void EmptyCollection_ReturnsZero_AndInvalidArgsThrow()
        {
            var emptyClass = ParticleUpdateSimulator.CreateClassList(0);
            var emptyStruct = ParticleUpdateSimulator.CreateStructArray(0);

            Assert.Equal(0.0, ParticleUpdateSimulator.UpdateClassList(emptyClass, 0.016f, 5));
            Assert.Equal(0.0, ParticleUpdateSimulator.UpdateStructArray(emptyStruct, 0.016f, 5));

            Assert.Throws<ArgumentOutOfRangeException>(() => ParticleUpdateSimulator.CreateClassList(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ParticleUpdateSimulator.CreateStructArray(-1));
            Assert.Throws<ArgumentNullException>(() => ParticleUpdateSimulator.UpdateClassList(null!, 0.016f, 1));
            Assert.Throws<ArgumentNullException>(() => ParticleUpdateSimulator.UpdateStructArray(null!, 0.016f, 1));
        }
    }
}
