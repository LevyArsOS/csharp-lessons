namespace CSharpLessons.DelegateCaching.Data;

/// <summary>
/// Zero-allocation listener contract for constrained generic dispatch.
/// </summary>
public interface IGameplayEventListener
{
    void OnCombatEvent(ref EntityState entity, in CombatEvent ev);
}

/// <summary>
/// Value-type listener storing an explicit global damage modifier without heap closures.
/// </summary>
public readonly struct CombatEventListener : IGameplayEventListener
{
    private readonly int _globalModifier;

    public CombatEventListener(int globalModifier)
    {
        _globalModifier = globalModifier;
    }

    public void OnCombatEvent(ref EntityState entity, in CombatEvent ev)
    {
        EntityState.ApplyCombatEvent(ref entity, in ev, _globalModifier);
    }
}
