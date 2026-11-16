using BenchmarkDotNet.Attributes;
using CSharpLessons.DelegateCaching.Data;

namespace CSharpLessons.DelegateCaching;

[MemoryDiagnoser]
public class Benchmark
{
    private const int EntityCount = 1_024;
    private const int EventCount = 10_000;
    private const int GlobalModifier = 5;

    private EntityState[] _entities = null!;
    private CombatEvent[] _events = null!;

    [GlobalSetup]
    public void Setup()
    {
        _entities = new EntityState[EntityCount];
        _events = new CombatEvent[EventCount];

        for (int i = 0; i < EventCount; i++)
        {
            _events[i] = new CombatEvent(
                EntityIndex: (i * 37) % EntityCount,
                RawDamage: (i % 45) + 10,
                ArmorPenetration: i % 12,
                BonusMultiplier: (i % 3) + 1);
        }
    }

    [IterationSetup]
    public void ResetEntities()
    {
        for (int i = 0; i < _entities.Length; i++)
        {
            _entities[i] = new EntityState(10_000, 15);
        }
    }

    [Benchmark(Baseline = true)]
    public int DispatchCapturingLambda()
    {
        EventDispatcher.DispatchCapturingLambda(_entities, _events, GlobalModifier);
        return _entities[0].Health;
    }

    [Benchmark]
    public int DispatchCachedStaticDelegate()
    {
        EventDispatcher.DispatchCachedStaticDelegate(_entities, _events, GlobalModifier);
        return _entities[0].Health;
    }

    [Benchmark]
    public int DispatchInterfaceListener()
    {
        var listener = new CombatEventListener(GlobalModifier);
        EventDispatcher.DispatchInterfaceListener(_entities, _events, ref listener);
        return _entities[0].Health;
    }

    [Benchmark]
    public int DispatchFunctionPointer()
    {
        EventDispatcher.DispatchFunctionPointer(_entities, _events, GlobalModifier);
        return _entities[0].Health;
    }
}
