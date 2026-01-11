namespace CSharpLessons.Boxing.Data
{
    public struct Point : IEquatable<Point>
    {
        public int X { get; set; }
        public int Y { get; set; }

        // IEquatable<T> implementation - NO boxing
        public bool Equals(Point other) => X == other.X && Y == other.Y;

        // Override necessary for compatibility - may cause boxing
        public override bool Equals(object? obj) => obj is Point other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}
