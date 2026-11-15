namespace CSharpLessons.StringAllocations.Data;

/// <summary>
/// Represents a single floating combat text and HUD status event produced during a frame.
/// </summary>
public readonly record struct CombatEvent(
    int TargetId,
    int Damage,
    int RemainingHp,
    int MaxHp);
