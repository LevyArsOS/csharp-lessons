namespace CSharpLessons.SparseSet.Data;

/// <summary>
/// Optimized: ECS sparse set pairing a direct <c>entityId</c>-indexed sparse array with
/// packed contiguous dense entity and component arrays for O(1) lookup, O(1) swap-and-pop
/// removal, and zero-allocation <see cref="Span{T}"/> iteration.
/// </summary>
public sealed class SparseSet<T> where T : struct
{
    private readonly int[] _sparse;
    private readonly int[] _denseEntities;
    private readonly T[] _denseComponents;
    private int _count;

    public SparseSet(int maxEntityId, int capacity)
    {
        if (maxEntityId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxEntityId));
        }
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        _sparse = new int[maxEntityId + 1];
        Array.Fill(_sparse, -1);
        _denseEntities = new int[capacity];
        _denseComponents = new T[capacity];
    }

    public int Count => _count;
    public int Capacity => _denseComponents.Length;
    public int MaxEntityId => _sparse.Length - 1;

    public void Add(int entityId, in T component)
    {
        ValidateEntityId(entityId);

        int existingDenseIndex = _sparse[entityId];
        if (existingDenseIndex >= 0 && existingDenseIndex < _count && _denseEntities[existingDenseIndex] == entityId)
        {
            _denseComponents[existingDenseIndex] = component;
            return;
        }

        if (_count >= _denseComponents.Length)
        {
            throw new InvalidOperationException($"SparseSet capacity ({_denseComponents.Length}) exceeded.");
        }

        _sparse[entityId] = _count;
        _denseEntities[_count] = entityId;
        _denseComponents[_count] = component;
        _count++;
    }

    public bool Contains(int entityId)
    {
        if ((uint)entityId >= (uint)_sparse.Length)
        {
            return false;
        }
        int denseIndex = _sparse[entityId];
        return denseIndex >= 0 && denseIndex < _count && _denseEntities[denseIndex] == entityId;
    }

    public bool TryGet(int entityId, out T component)
    {
        if (!Contains(entityId))
        {
            component = default;
            return false;
        }
        component = _denseComponents[_sparse[entityId]];
        return true;
    }

    public ref T GetRef(int entityId)
    {
        if (!Contains(entityId))
        {
            throw new KeyNotFoundException($"Entity {entityId} is not present in SparseSet.");
        }
        return ref _denseComponents[_sparse[entityId]];
    }

    public bool Remove(int entityId)
    {
        if (!Contains(entityId))
        {
            return false;
        }

        int removedDenseIndex = _sparse[entityId];
        int lastDenseIndex = _count - 1;

        if (removedDenseIndex != lastDenseIndex)
        {
            int movedEntity = _denseEntities[lastDenseIndex];
            _denseEntities[removedDenseIndex] = movedEntity;
            _denseComponents[removedDenseIndex] = _denseComponents[lastDenseIndex];
            _sparse[movedEntity] = removedDenseIndex;
        }

        _sparse[entityId] = -1;
        _count--;
        return true;
    }

    public Span<T> AsDenseSpan() => _denseComponents.AsSpan(0, _count);

    public ReadOnlySpan<int> AsDenseEntitiesSpan() => _denseEntities.AsSpan(0, _count);

    public void Clear()
    {
        for (int i = 0; i < _count; i++)
        {
            _sparse[_denseEntities[i]] = -1;
        }
        _count = 0;
    }

    private void ValidateEntityId(int entityId)
    {
        if ((uint)entityId >= (uint)_sparse.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(entityId),
                entityId,
                $"Entity ID must be in [0, {_sparse.Length}).");
        }
    }
}
