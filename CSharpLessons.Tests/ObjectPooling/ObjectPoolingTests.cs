using System;
using System.Collections.Generic;
using CSharpLessons.ObjectPooling.Data;
using Xunit;

namespace CSharpLessons.Tests.ObjectPooling
{
    public class ObjectPoolingTests
    {
        private static ObjectPool<Projectile> CreatePool(int capacity = 8)
        {
            return new ObjectPool<Projectile>(capacity, () => new Projectile(), p => p.Reset());
        }

        [Fact]
        public void RentAndReturn_ReuseSameInstanceAndResetStateDeterministically()
        {
            var pool = CreatePool(capacity: 1);
            Assert.Equal(1, pool.Capacity);
            Assert.Equal(0, pool.ActiveCount);
            Assert.Equal(1, pool.AvailableCount);

            var first = pool.Rent();
            first.Activate(12f, 34f, 5f, -2f, 3);
            Assert.Equal(1, pool.ActiveCount);
            Assert.Equal(0, pool.AvailableCount);

            pool.Return(first);
            Assert.Equal(0, pool.ActiveCount);
            Assert.Equal(1, pool.AvailableCount);
            Assert.False(first.Active);
            Assert.Equal(0f, first.X);
            Assert.Equal(0f, first.Y);
            Assert.Equal(0f, first.Vx);
            Assert.Equal(0f, first.Vy);
            Assert.Equal(0, first.RemainingFrames);

            var second = pool.Rent();
            Assert.Same(first, second);
        }

        [Fact]
        public void Rent_WhenExhausted_ThrowsInvalidOperationException()
        {
            var pool = CreatePool(capacity: 1);
            _ = pool.Rent();

            Assert.Throws<InvalidOperationException>(() => pool.Rent());
        }

        [Fact]
        public void Return_NullOrFullPool_Throws()
        {
            var pool = CreatePool(capacity: 1);
            Assert.Throws<ArgumentNullException>(() => pool.Return(null!));

            var extra = new Projectile();
            Assert.Throws<InvalidOperationException>(() => pool.Return(extra));
        }

        [Fact]
        public void Constructor_InvalidArguments_Throw()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ObjectPool<Projectile>(0, () => new Projectile(), p => p.Reset()));
            Assert.Throws<ArgumentNullException>(() => new ObjectPool<Projectile>(2, null!, p => p.Reset()));
            Assert.Throws<ArgumentNullException>(() => new ObjectPool<Projectile>(2, () => new Projectile(), null!));
        }

        [Fact]
        public void RunPooled_ProducesSameChecksumAsNaive_WithZeroSteadyStateAllocations()
        {
            const int batchSize = 100;
            const int batches = 10;
            const int lifetime = 4;
            const float dt = 0.016f;

            var pool = CreatePool(capacity: batchSize);
            var scratch = new List<Projectile>(batchSize);

            float naiveChecksum = ProjectileSimulation.RunNaive(batchSize, batches, lifetime, dt);

            // Warmup call before measuring thread allocations
            _ = ProjectileSimulation.RunPooled(batchSize, 1, lifetime, dt, pool, scratch);

            long before = GC.GetAllocatedBytesForCurrentThread();
            float pooledChecksum = ProjectileSimulation.RunPooled(batchSize, batches, lifetime, dt, pool, scratch);
            long after = GC.GetAllocatedBytesForCurrentThread();

            Assert.Equal(naiveChecksum, pooledChecksum, precision: 2);
            Assert.Equal(0, after - before);
            Assert.Equal(0, pool.ActiveCount);
            Assert.Equal(batchSize, pool.AvailableCount);
        }
    }
}
