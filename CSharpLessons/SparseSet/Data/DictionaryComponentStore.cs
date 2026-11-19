namespace CSharpLessons.SparseSet.Data;

/// <summary>
/// Naive: stores entity components in a <see cref="Dictionary{TKey, TValue}"/>, incurring hashing,
/// bucket indirection, and scattered entry iteration.
/// </summary>
public sealed class DictionaryComponentStore
{
    private readonly Dictionary<int, VelocityComponent> _components;

    public DictionaryComponentStore(int capacity)
    {
        _components = new Dictionary<int, VelocityComponent>(capacity);
    }

    public int Count => _components.Count;

    public void Add(int entityId, in VelocityComponent component)
    {
        _components[entityId] = component;
    }

    public bool Contains(int entityId) => _components.ContainsKey(entityId);

    public bool TryGet(int entityId, out VelocityComponent component) =>
        _components.TryGetValue(entityId, out component);

    public bool Remove(int entityId) => _components.Remove(entityId);

    public void Clear() => _components.Clear();

    public double IntegrateAll(float dt)
    {
        double checksum = 0.0;
        foreach (var kvp in _components)
        {
            VelocityComponent v = kvp.Value;
            checksum += (v.X + v.Y + v.Z) * v.Speed * dt;
        }
        return checksum;
    }
}
