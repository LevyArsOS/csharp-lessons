using CSharpLessons.SparseSet.Data;
using Xunit;

namespace CSharpLessons.Tests.SparseSet;

public sealed class SparseSetTests
{
    [Fact]
    public void AddTryGetGetRefAndSwapAndPopMaintainExactInvariants()
    {
        var sparse = new SparseSet<VelocityComponent>(maxEntityId: 30, capacity: 10);
        var dict = new DictionaryComponentStore(10);

        int[] ids = [2, 7, 11, 19, 25];
        foreach (int id in ids)
        {
            var v = new VelocityComponent(id, id * 2f, 1.5f, 1.0f);
            sparse.Add(id, in v);
            dict.Add(id, in v);
        }

        // Overwrite existing component and mutate via GetRef.
        var updated = new VelocityComponent(100f, 200f, 300f, 2.0f);
        sparse.Add(11, in updated);
        dict.Add(11, in updated);

        ref VelocityComponent refVal = ref sparse.GetRef(7);
        refVal.Speed = 5.5f;
        dict.TryGet(7, out VelocityComponent d7);
        d7.Speed = 5.5f;
        dict.Add(7, in d7);

        // Remove first, middle, and last dense entries.
        Assert.True(sparse.Remove(2));
        Assert.True(dict.Remove(2));
        Assert.True(sparse.Remove(25));
        Assert.True(dict.Remove(25));
        Assert.True(sparse.Remove(11));
        Assert.True(dict.Remove(11));

        // Re-add previously removed ID.
        var reinserted = new VelocityComponent(9f, 18f, 27f, 3.0f);
        sparse.Add(2, in reinserted);
        dict.Add(2, in reinserted);

        Assert.Equal(dict.Count, sparse.Count);
        for (int id = 0; id <= 30; id++)
        {
            Assert.Equal(dict.Contains(id), sparse.Contains(id));
            if (dict.TryGet(id, out VelocityComponent expected))
            {
                Assert.True(sparse.TryGet(id, out VelocityComponent actual));
                Assert.Equal(expected, actual);
            }
        }
    }

    [Fact]
    public void LargeScaleLifecycleMatchesDictionaryAndDenseIterationAllocatesZeroBytes()
    {
        const int entityCount = 50_000;
        var sparse = new SparseSet<VelocityComponent>(maxEntityId: entityCount, capacity: entityCount);
        var dict = new DictionaryComponentStore(entityCount);

        for (int i = 0; i < entityCount; i++)
        {
            var v = new VelocityComponent((i % 64) + 1f, (i % 32) + 1f, (i % 16) + 1f, 1.5f);
            sparse.Add(i, in v);
            dict.Add(i, in v);
        }

        for (int i = 0; i < entityCount; i += 2)
        {
            Assert.True(sparse.Remove(i));
            Assert.True(dict.Remove(i));
        }

        Assert.Equal(25_000, sparse.Count);
        Assert.Equal(dict.Count, sparse.Count);

        // Warm up JIT before measuring allocations.
        double dictSum = dict.IntegrateAll(0.016f);

        static double SumDense(SparseSet<VelocityComponent> set)
        {
            double total = 0.0;
            Span<VelocityComponent> span = set.AsDenseSpan();
            for (int i = 0; i < span.Length; i++)
            {
                ref readonly VelocityComponent v = ref span[i];
                total += (v.X + v.Y + v.Z) * v.Speed * 0.016f;
            }
            return total;
        }

        _ = SumDense(sparse);
        long before = GC.GetAllocatedBytesForCurrentThread();
        double sparseSum = SumDense(sparse);
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(0L, allocated);
        Assert.Equal(dictSum, sparseSum, precision: 2);
    }

    [Fact]
    public void EdgeCasesAndFaultInjectionHandleInvalidIdsCleanly()
    {
        var sparse = new SparseSet<VelocityComponent>(maxEntityId: 4, capacity: 2);

        Assert.False(sparse.Contains(-1));
        Assert.False(sparse.Contains(999));
        Assert.False(sparse.TryGet(999, out _));
        Assert.False(sparse.Remove(-1));
        Assert.False(sparse.Remove(999));
        Assert.False(sparse.Remove(2));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            sparse.Add(10, new VelocityComponent(1f, 1f, 1f, 1f)));
        Assert.Throws<KeyNotFoundException>(() => sparse.GetRef(1));

        sparse.Add(0, new VelocityComponent(1f, 1f, 1f, 1f));
        sparse.Add(1, new VelocityComponent(2f, 2f, 2f, 2f));
        Assert.Throws<InvalidOperationException>(() =>
            sparse.Add(2, new VelocityComponent(3f, 3f, 3f, 3f)));
    }
}
