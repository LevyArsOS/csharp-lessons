namespace CSharpLessons.Boxing.Data
{
    public struct DataWithSpecializedOverride : IEquatable<DataWithSpecializedOverride>
    {
        public int Id;
        public float Velocity;

        // IEquatable<T> implementation - does not cause boxing
        public bool Equals(DataWithSpecializedOverride other) =>
            Id == other.Id && Math.Abs(Velocity - other.Velocity) < float.Epsilon;

        // Override necessary for compatibility
        public override bool Equals(object? obj) =>
            obj is DataWithSpecializedOverride other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Id, Velocity);

        public override string ToString() => $"[Id:{Id}, Velocity:{Velocity}]";
    }
}
