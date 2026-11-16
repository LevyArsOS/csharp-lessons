using CSharpLessons.DelegateCaching.Data;
using Xunit;

namespace CSharpLessons.Tests.DelegateCaching;

public sealed class DelegateCachingTests
{
    private static (EntityState[] Entities, CombatEvent[] Events) CreateFixture(int entityCount, int eventCount)
    {
        var entities = new EntityState[entityCount];
        for (int i = 0; i < entityCount; i++)
        {
            entities[i] = new EntityState(health: 500 + (i % 100), armor: 10 + (i % 7));
        }

        var events = new CombatEvent[eventCount];
        for (int i = 0; i < eventCount; i++)
        {
            events[i] = new CombatEvent(
                EntityIndex: (i * 17) % entityCount,
                RawDamage: (i % 30) + 5,
                ArmorPenetration: i % 15,
                BonusMultiplier: (i % 3) + 1);
        }

        return (entities, events);
    }

    [Fact]
    public void AllStrategiesProduceIdenticalEntityStates()
    {
        const int entityCount = 128;
        const int eventCount = 2_000;
        const int globalModifier = 4;

        var (naiveEntities, events) = CreateFixture(entityCount, eventCount);
        var staticEntities = (EntityState[])naiveEntities.Clone();
        var interfaceEntities = (EntityState[])naiveEntities.Clone();
        var fnPtrEntities = (EntityState[])naiveEntities.Clone();

        EventDispatcher.DispatchCapturingLambda(naiveEntities, events, globalModifier);
        EventDispatcher.DispatchCachedStaticDelegate(staticEntities, events, globalModifier);

        var listener = new CombatEventListener(globalModifier);
        EventDispatcher.DispatchInterfaceListener(interfaceEntities, events, ref listener);

        EventDispatcher.DispatchFunctionPointer(fnPtrEntities, events, globalModifier);

        for (int i = 0; i < entityCount; i++)
        {
            Assert.Equal(naiveEntities[i], staticEntities[i]);
            Assert.Equal(naiveEntities[i], interfaceEntities[i]);
            Assert.Equal(naiveEntities[i], fnPtrEntities[i]);
        }
    }

    [Fact]
    public void OptimizedDispatchStrategiesAllocateZeroBytes()
    {
        const int entityCount = 256;
        const int eventCount = 10_000;
        const int globalModifier = 5;

        var (entities, events) = CreateFixture(entityCount, eventCount);
        var listener = new CombatEventListener(globalModifier);

        // Warm up JIT and static delegate field initialization before measuring allocations.
        EventDispatcher.DispatchCachedStaticDelegate(entities, events.AsSpan(0, 1), globalModifier);
        EventDispatcher.DispatchInterfaceListener(entities, events.AsSpan(0, 1), ref listener);
        EventDispatcher.DispatchFunctionPointer(entities, events.AsSpan(0, 1), globalModifier);

        long beforeStatic = GC.GetAllocatedBytesForCurrentThread();
        EventDispatcher.DispatchCachedStaticDelegate(entities, events, globalModifier);
        long staticAllocated = GC.GetAllocatedBytesForCurrentThread() - beforeStatic;

        long beforeInterface = GC.GetAllocatedBytesForCurrentThread();
        EventDispatcher.DispatchInterfaceListener(entities, events, ref listener);
        long interfaceAllocated = GC.GetAllocatedBytesForCurrentThread() - beforeInterface;

        long beforeFnPtr = GC.GetAllocatedBytesForCurrentThread();
        EventDispatcher.DispatchFunctionPointer(entities, events, globalModifier);
        long fnPtrAllocated = GC.GetAllocatedBytesForCurrentThread() - beforeFnPtr;

        long beforeNaive = GC.GetAllocatedBytesForCurrentThread();
        EventDispatcher.DispatchCapturingLambda(entities, events, globalModifier);
        long naiveAllocated = GC.GetAllocatedBytesForCurrentThread() - beforeNaive;

        Assert.Equal(0L, staticAllocated);
        Assert.Equal(0L, interfaceAllocated);
        Assert.Equal(0L, fnPtrAllocated);
        Assert.True(naiveAllocated > 500_000L, $"Expected capturing lambdas to allocate > 500 KB, got {naiveAllocated} B.");
    }

    [Fact]
    public void EmptyEventSpanLeavesEntitiesUntouchedWithZeroAllocations()
    {
        var entities = new[] { new EntityState(100, 10), new EntityState(200, 20) };
        var emptyEvents = Array.Empty<CombatEvent>();
        var listener = new CombatEventListener(3);

        EventDispatcher.DispatchCapturingLambda(entities, emptyEvents, 3);
        EventDispatcher.DispatchCachedStaticDelegate(entities, emptyEvents, 3);
        EventDispatcher.DispatchInterfaceListener(entities, emptyEvents, ref listener);
        EventDispatcher.DispatchFunctionPointer(entities, emptyEvents, 3);

        Assert.Equal(100, entities[0].Health);
        Assert.Equal(0, entities[0].EventsProcessed);
        Assert.Equal(200, entities[1].Health);
        Assert.Equal(0, entities[1].EventsProcessed);
    }

    [Fact]
    public void LethalDamageClampsHealthAtZero()
    {
        var entities = new[] { new EntityState(15, 2) };
        var events = new[] { new CombatEvent(0, RawDamage: 500, ArmorPenetration: 10, BonusMultiplier: 2) };

        EventDispatcher.DispatchCachedStaticDelegate(entities, events, globalModifier: 10);

        Assert.Equal(0, entities[0].Health);
        Assert.Equal(1, entities[0].EventsProcessed);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void OutOfBoundsEntityIndexThrowsArgumentOutOfRangeException(int invalidIndex)
    {
        var entities = new[] { new EntityState(100, 5), new EntityState(100, 5) };
        var events = new[] { new CombatEvent(invalidIndex, 10, 0, 1) };
        var listener = new CombatEventListener(0);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            EventDispatcher.DispatchCapturingLambda(entities, events, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            EventDispatcher.DispatchCachedStaticDelegate(entities, events, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            EventDispatcher.DispatchInterfaceListener(entities, events, ref listener));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            EventDispatcher.DispatchFunctionPointer(entities, events, 0));
    }
}
