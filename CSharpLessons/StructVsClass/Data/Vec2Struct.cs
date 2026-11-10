namespace CSharpLessons.StructVsClass.Data
{
    public readonly struct Vec2Struct : IEquatable<Vec2Struct>
    {
        public float X { get; }
        public float Y { get; }

        public Vec2Struct(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vec2Struct Add(in Vec2Struct other) => new Vec2Struct(X + other.X, Y + other.Y);

        public Vec2Struct Scale(float factor) => new Vec2Struct(X * factor, Y * factor);

        public float LengthSquared() => (X * X) + (Y * Y);

        public static Vec2Struct operator +(in Vec2Struct a, in Vec2Struct b) => a.Add(in b);

        public static Vec2Struct operator *(in Vec2Struct a, float factor) => a.Scale(factor);

        public bool Equals(Vec2Struct other) => X.Equals(other.X) && Y.Equals(other.Y);

        public override bool Equals(object? obj) => obj is Vec2Struct other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static bool operator ==(in Vec2Struct left, in Vec2Struct right) => left.Equals(right);

        public static bool operator !=(in Vec2Struct left, in Vec2Struct right) => !left.Equals(right);
    }
}
