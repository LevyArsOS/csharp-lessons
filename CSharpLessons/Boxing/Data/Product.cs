namespace CSharpLessons.Boxing.Data
{
    public struct Product : IEquatable<Product>, IComparable<Product>
    {
        public int Id { get; set; }
        public decimal Price { get; set; }

        // IEquatable<T> - for equality comparisons without boxing
        public bool Equals(Product other) => Id == other.Id;
        public override bool Equals(object? obj) => obj is Product other && Equals(other);
        public override int GetHashCode() => Id.GetHashCode();

        // IComparable<T> - for sorting without boxing
        public int CompareTo(Product other) => Price.CompareTo(other.Price);
    }
}
