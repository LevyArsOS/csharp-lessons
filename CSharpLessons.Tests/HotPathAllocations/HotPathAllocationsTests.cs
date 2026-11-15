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

            // Warm up full frame count before measuring thread allocations so TieredCompilation settles.
            _ = HotPathQueries.RunZeroAllocFrameLoop(world, frames, minEnergy, scratch);

            long before = GC.GetAllocatedBytesForCurrentThread();
            float zeroAllocChecksum = HotPathQueries.RunZeroAllocFrameLoop(world, frames, minEnergy, scratch);
            long after = GC.GetAllocatedBytesForCurrentThread();

            Assert.Equal(naiveChecksum, zeroAllocChecksum, precision: 2);
            Assert.Equal(0, after - before);
        }

        [Fact]
        public void QueryActiveNonAlloc_AllocatesZeroBytes_WhileNaiveLinqAllocates()
        {
            var world = new EntityWorld(500);
            var scratch = new List<int>(500);

            // Warm up JIT before measuring thread allocations
            HotPathQueries.QueryActiveNonAlloc(world.Entities, 20f, scratch);
            _ = HotPathQueries.QueryActiveNaive(world.Entities, 20f);

            long nonAllocBefore = GC.GetAllocatedBytesForCurrentThread();
            HotPathQueries.QueryActiveNonAlloc(world.Entities, 20f, scratch);
            long nonAllocAfter = GC.GetAllocatedBytesForCurrentThread();

            long naiveBefore = GC.GetAllocatedBytesForCurrentThread();
            var naiveResult = HotPathQueries.QueryActiveNaive(world.Entities, 20f);
            long naiveAfter = GC.GetAllocatedBytesForCurrentThread();

            Assert.Equal(0, nonAllocAfter - nonAllocBefore);
            Assert.True(naiveAfter - naiveBefore > 0);
            Assert.Equal(naiveResult, scratch);
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
