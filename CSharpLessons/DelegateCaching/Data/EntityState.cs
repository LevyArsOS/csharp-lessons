namespace CSharpLessons.DelegateCaching.Data;

/// <summary>
/// Mutable value-type state for an active gameplay entity.
/// </summary>
public struct EntityState
{
    public int Health;
    public int Armor;
    public int EventsProcessed;

    public EntityState(int health, int armor)
    {
        Health = health;
        Armor = armor;
        EventsProcessed = 0;
    }

    /// <summary>
    /// Applies a combat event using deterministic integer math and clamps health at zero.
    /// </summary>
    public static void ApplyCombatEvent(ref EntityState entity, in CombatEvent ev, int globalModifier)
    {
        int effectiveArmor = entity.Armor - ev.ArmorPenetration;
        if (effectiveArmor < 0)
        {
            effectiveArmor = 0;
        }

        int scaledDamage = (ev.RawDamage * ev.BonusMultiplier) + globalModifier - effectiveArmor;
        if (scaledDamage < 1)
        {
            scaledDamage = 1;
        }

        int nextHealth = entity.Health - scaledDamage;
        entity.Health = nextHealth > 0 ? nextHealth : 0;
        entity.EventsProcessed++;
    }
}
