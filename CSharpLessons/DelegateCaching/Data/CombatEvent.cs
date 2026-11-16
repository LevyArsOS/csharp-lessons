namespace CSharpLessons.DelegateCaching.Data;

/// <summary>
/// Represents a single gameplay combat event targeting an entity by index.
/// </summary>
public readonly record struct CombatEvent(
    int EntityIndex,
    int RawDamage,
    int ArmorPenetration,
    int BonusMultiplier);
