namespace CSharpLessons.DelegateCaching.Data;

/// <summary>
/// Delegate signature accepting explicit state by reference to avoid closure captures.
/// </summary>
public delegate void StaticEntityEventHandler(ref EntityState entity, in CombatEvent ev, int globalModifier);

/// <summary>
/// Compares capturing lambda closures against cached static delegates, constrained interface
/// dispatch, and C# 9 managed function pointers across gameplay event batches.
/// </summary>
public static class EventDispatcher
{
    private static readonly StaticEntityEventHandler CachedHandler =
        static (ref EntityState entity, in CombatEvent ev, int globalModifier) =>
            EntityState.ApplyCombatEvent(ref entity, in ev, globalModifier);

    /// <summary>
    /// Naive: captures <paramref name="entities"/>, <c>ev</c>, and <paramref name="globalModifier"/>
    /// in a compiler-generated display class and allocates a new <see cref="Action{T}"/> per event.
    /// </summary>
    public static void DispatchCapturingLambda(
        EntityState[] entities,
        ReadOnlySpan<CombatEvent> events,
        int globalModifier)
    {
        for (int i = 0; i < events.Length; i++)
        {
            CombatEvent ev = events[i];
            ValidateIndex(ev.EntityIndex, entities.Length);
            InvokeCallback(ev.EntityIndex, index =>
            {
                EntityState.ApplyCombatEvent(ref entities[index], in ev, globalModifier);
            });
        }
    }

    /// <summary>
    /// Optimized: uses a cached static delegate and passes all state via explicit parameters (0 B).
    /// </summary>
    public static void DispatchCachedStaticDelegate(
        Span<EntityState> entities,
        ReadOnlySpan<CombatEvent> events,
        int globalModifier)
    {
        StaticEntityEventHandler handler = CachedHandler;
        for (int i = 0; i < events.Length; i++)
        {
            ref readonly CombatEvent ev = ref events[i];
            ValidateIndex(ev.EntityIndex, entities.Length);
            handler(ref entities[ev.EntityIndex], in ev, globalModifier);
        }
    }

    /// <summary>
    /// Optimized: constrained generic interface dispatch on a value-type listener (0 B, JIT-inlined).
    /// </summary>
    public static void DispatchInterfaceListener<TListener>(
        Span<EntityState> entities,
        ReadOnlySpan<CombatEvent> events,
        ref TListener listener)
        where TListener : struct, IGameplayEventListener
    {
        for (int i = 0; i < events.Length; i++)
        {
            ref readonly CombatEvent ev = ref events[i];
            ValidateIndex(ev.EntityIndex, entities.Length);
            listener.OnCombatEvent(ref entities[ev.EntityIndex], in ev);
        }
    }

    /// <summary>
    /// Optimized: C# 9 managed function pointer (<c>calli</c>) dispatch (0 B).
    /// </summary>
    public static unsafe void DispatchFunctionPointer(
        Span<EntityState> entities,
        ReadOnlySpan<CombatEvent> events,
        int globalModifier)
    {
        delegate* managed<ref EntityState, in CombatEvent, int, void> handler = &EntityState.ApplyCombatEvent;
        for (int i = 0; i < events.Length; i++)
        {
            ref readonly CombatEvent ev = ref events[i];
            ValidateIndex(ev.EntityIndex, entities.Length);
            handler(ref entities[ev.EntityIndex], in ev, globalModifier);
        }
    }

    private static void InvokeCallback(int entityIndex, Action<int> callback)
    {
        callback(entityIndex);
    }

    private static void ValidateIndex(int entityIndex, int entityCount)
    {
        if ((uint)entityIndex >= (uint)entityCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(entityIndex),
                entityIndex,
                $"Entity index must be in [0, {entityCount}).");
        }
    }
}
