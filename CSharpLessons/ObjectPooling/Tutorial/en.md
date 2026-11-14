# Object Pooling in C#

## Why `new Projectile()` Causes Frame Spikes
In 60 FPS games, spawning and destroying hundreds of projectiles, particles, or hit markers every second with `new Projectile()` fills Gen0 quickly. Even though individual Gen0 allocations are fast in .NET 9, sustained churn (`~804 KB` per 20,000 spawns) triggers Gen0 and Gen1 Garbage Collector sweeps that compete with the 16.6 ms frame budget.

## Pre-Allocated `IObjectPool<T>`
`ObjectPool<T>` pre-allocates `capacity` instances into a `Stack<T>` during scene load. `Rent()` pops an instance in $O(1)$ with zero heap allocations, and `Return(item)` resets every field deterministically before pushing it back:

```csharp
public T Rent()
{
    if (_available.Count == 0)
        throw new InvalidOperationException("Object pool exhausted.");
    ActiveCount++;
    return _available.Pop();
}

public void Return(T item)
{
    ArgumentNullException.ThrowIfNull(item);
    if (_available.Count >= Capacity)
        throw new InvalidOperationException("Cannot return item to a full pool.");
    _onReset(item);
    _available.Push(item);
    ActiveCount--;
}
```

## Key Takeaways
- Always reset every mutable field inside `onReset` so recycled projectiles never inherit stale positions or velocities.
- Keep `ActiveCount` and `AvailableCount` observable for pool sizing telemetry.
- Use swap-and-pop removal on the active `List<Projectile>` scratch buffer so despawning stays $O(1)$ without shifting elements.
