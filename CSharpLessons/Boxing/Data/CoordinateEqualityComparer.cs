namespace CSharpLessons.Boxing.Data
{
    // Custom comparer to avoid boxing
    public class CoordinateEqualityComparer : IEqualityComparer<Coordinate>
    {
        public bool Equals(Coordinate x, Coordinate y) => x.X == y.X && x.Y == y.Y; // No boxing
        public int GetHashCode(Coordinate obj) => HashCode.Combine(obj.X, obj.Y); // No boxing
    }
}
