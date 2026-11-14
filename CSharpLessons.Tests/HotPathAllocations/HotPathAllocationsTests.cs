using System;
using System.Collections.Generic;
using CSharpLessons.HotPathAllocations.Data;
using Xunit;

namespace CSharpLessons.Tests.HotPathAllocations
{
    public class HotPathAllocationsTests
    {
        [Fact]
        public void QueryActiveNonAlloc_MatchesNaiveLinqOutput()
        {
            var world = new EntityWorld(1_000);
            var expected = HotPathQueries.QueryActiveNaive(world.Entities, 20f);
            var actual = new List<int>(1_000);

            HotPathQueries.QueryActiveNonAlloc(world.Entities, 20f, actual);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RunZeroAllocFrameLoop_MatchesNaiveOutput_WithZeroManagedAllocations()
        {
            var world = new EntityWorld(1_000);
            var scratch = new List<int>(1_000);
            const int frames = 60;
            const float minEnergy = 18f;

            float naiveChecksum = HotPathQueries.RunNaiveLinqFrameLoop(world, frames, minEnergy);

            // Warmup call before measuring thread allocations
            _ = HotPathQueries.RunZeroAllocFrameLoop(world, 1, minEnergy, scratch);

            long before = GC.GetAllocatedBytesForCurrentThread();
            float zeroAllocChecksum = HotPathQueries.RunZeroAllocFrameLoop(world, frames, minEnergy, scratch);
            long after = GC.GetAllocatedBytesForCurrentThread();

            Assert.Equal(naiveChecksum, zeroAllocChecksum, precision: 2);
            Assert.Equal(0, after - before);
        }

        [Fact]
        public void QueryActive_EmptyAndNoMatches_ReturnsEmpty()
        {
            var empty = Array.Empty<Entity>();
            var scratch = new List<int> { 99 };

            HotPathQueries.QueryActiveNonAlloc(empty, 0f, scratch);
            Assert.Empty(scratch);
            Assert.Empty(HotPathQueries.QueryActiveNaive(empty, 0f));

            var world = new EntityWorld(50);
            HotPathQueries.QueryActiveNonAlloc(world.Entities, 1_000f, scratch);
            Assert.Empty(scratch);
        }

        [Fact]
        public void HotPathQueries_NullArguments_ThrowArgumentNullException()
        {
            var world = new EntityWorld(10);
            var scratch = new List<int>();

            Assert.Throws<ArgumentNullException>(() => HotPathQueries.QueryActiveNaive(null!, 0f));
            Assert.Throws<ArgumentNullException>(() => HotPathQueries.QueryActiveNonAlloc(null!, 0f, scratch));
            Assert.Throws<ArgumentNullException>(() => HotPathQueries.QueryActiveNonAlloc(world.Entities, 0f, null!));
            Assert.Throws<ArgumentNullException>(() => HotPathQueries.RunNaiveLinqFrameLoop(null!, 1, 0f));
            Assert.Throws<ArgumentNullException>(() => HotPathQueries.RunZeroAllocFrameLoop(null!, 1, 0f, scratch));
            Assert.Throws<ArgumentNullException>(() => HotPathQueries.RunZeroAllocFrameLoop(world, 1, 0f, null!));
            Assert.Throws<ArgumentOutOfRangeException>(() => new EntityWorld(-1));
        }
    }
}
