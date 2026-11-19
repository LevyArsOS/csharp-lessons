using BenchmarkDotNet.Attributes;
using CSharpLessons.SparseSet.Data;

namespace CSharpLessons.SparseSet;

[MemoryDiagnoser]
public class Benchmark
{
    private const int EntityCount = 50_000;
    private const int IterationFrames = 20;
    private const float DeltaTime = 0.016f;

    private DictionaryComponentStore _dictStore = null!;
    private SparseSet<VelocityComponent> _sparseSet = null!;
    private int[] _lookupOrder = null!;
    private int[] _removeOrder = null!;

    [GlobalSetup]
    public void Setup()
    {
        _dictStore = new DictionaryComponentStore(EntityCount);
        _sparseSet = new SparseSet<VelocityComponent>(maxEntityId: EntityCount, capacity: EntityCount);
        _lookupOrder = new int[EntityCount];
        _removeOrder = new int[EntityCount / 2];

        for (int i = 0; i < EntityCount; i++)
        {
            var v = new VelocityComponent(
                (i % 64) + 1f,
                (i % 32) + 1f,
                (i % 16) + 1f,
                1.25f);
            _dictStore.Add(i, in v);
            _sparseSet.Add(i, in v);
            _lookupOrder[i] = (i * 48271) % EntityCount;
        }

        for (int i = 0; i < _removeOrder.Length; i++)
        {
            _removeOrder[i] = i * 2;
        }
    }

    [Benchmark(Baseline = true)]
    public double DictionaryIterateAndLookup()
    {
        double sum = 0.0;
        for (int f = 0; f < IterationFrames; f++)
        {
            sum += _dictStore.IntegrateAll(DeltaTime);
        }

        for (int i = 0; i < _lookupOrder.Length; i++)
        {
            if (_dictStore.TryGet(_lookupOrder[i], out VelocityComponent v))
            {
                sum += v.X;
            }
        }
        return sum;
    }

    [Benchmark]
    public double SparseSetIterateAndLookup()
    {
        double sum = 0.0;
        Span<VelocityComponent> dense = _sparseSet.AsDenseSpan();
        for (int f = 0; f < IterationFrames; f++)
        {
            for (int i = 0; i < dense.Length; i++)
            {
                ref readonly VelocityComponent v = ref dense[i];
                sum += (v.X + v.Y + v.Z) * v.Speed * DeltaTime;
            }
        }

        for (int i = 0; i < _lookupOrder.Length; i++)
        {
            if (_sparseSet.TryGet(_lookupOrder[i], out VelocityComponent v))
            {
                sum += v.X;
            }
        }
        return sum;
    }

    [Benchmark]
    public int DictionaryAddAndRemoveChurn()
    {
        var store = new DictionaryComponentStore(EntityCount);
        for (int i = 0; i < EntityCount; i++)
        {
            var v = new VelocityComponent(1f, 2f, 3f, 1f);
            store.Add(i, in v);
        }
        for (int i = 0; i < _removeOrder.Length; i++)
        {
            store.Remove(_removeOrder[i]);
        }
        return store.Count;
    }

    [Benchmark]
    public int SparseSetAddAndRemoveChurn()
    {
        _sparseSet.Clear();
        for (int i = 0; i < EntityCount; i++)
        {
            var v = new VelocityComponent(1f, 2f, 3f, 1f);
            _sparseSet.Add(i, in v);
        }
        for (int i = 0; i < _removeOrder.Length; i++)
        {
            _sparseSet.Remove(_removeOrder[i]);
        }
        return _sparseSet.Count;
    }
}
