using System;
using System.Collections.Generic;

namespace CSharpLessons.ObjectPooling.Data
{
    public sealed class ObjectPool<T> : IObjectPool<T> where T : class
    {
        private readonly Stack<T> _available;
        private readonly Action<T> _onReset;

        public int Capacity { get; }
        public int ActiveCount { get; private set; }
        public int AvailableCount => _available.Count;

        public ObjectPool(int capacity, Func<T> factory, Action<T> onReset)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");
            }

            ArgumentNullException.ThrowIfNull(factory);
            ArgumentNullException.ThrowIfNull(onReset);

            Capacity = capacity;
            _onReset = onReset;
            _available = new Stack<T>(capacity);

            for (int i = 0; i < capacity; i++)
            {
                T instance = factory();
                _onReset(instance);
                _available.Push(instance);
            }
        }

        public T Rent()
        {
            if (_available.Count == 0)
            {
                throw new InvalidOperationException("Object pool exhausted.");
            }

            ActiveCount++;
            return _available.Pop();
        }

        public void Return(T item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (_available.Count >= Capacity)
            {
                throw new InvalidOperationException("Cannot return item to a full pool.");
            }

            _onReset(item);
            _available.Push(item);
            ActiveCount--;
        }
    }
}
