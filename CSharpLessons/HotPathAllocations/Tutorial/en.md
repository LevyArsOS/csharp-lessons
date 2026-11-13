# Hot-Path Allocations (Zero-Allocation Update Loops)

## Why Per-Frame LINQ Breaks 60 FPS Budgets
Calling `.Where(...).Select(...).ToList()` inside Unity `Update()` or Godot `_Process()` allocates on every single frame:
- LINQ iterator objects (`WhereArrayIterator`, `SelectEnumerableIterator`)
- Closure display classes if local variables are captured
- A fresh `List<int>` and internal array resizing every frame

Across `1,000` entities over `60` frames, that generates `149,280 B` of short-lived garbage per second.

## The `NonAlloc` Buffer-Reuse Pattern
Mirroring Unity's `Physics.RaycastNonAlloc` and `List<T>.Clear()` reuse, pass a pre-allocated `List<int>` scratch buffer and iterate with indexed `for` loops:

```csharp
public static void QueryActiveNonAlloc(Entity[] entities, float minEnergy, List<int> results)
{
    results.Clear();
    for (int i = 0; i < entities.Length; i++)
    {
        Entity e = entities[i];
        if (e.Active && e.Energy >= minEnergy)
            results.Add(e.Id);
    }
}
```

## Key Takeaways
- Keep `List<T>` scratch buffers as private fields on systems/controllers and call `Clear()` (which resets `_size = 0` without freeing the internal array).
- Avoid LINQ in `Update()` / `FixedUpdate()` / `_Process()`; reserve LINQ for cold paths (loading, tooling, editor scripts).
- Cache component and array references during `Awake()` / `_Ready()` instead of querying per frame.
