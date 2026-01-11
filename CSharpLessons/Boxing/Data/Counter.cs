namespace CSharpLessons.Boxing.Data
{
    public struct Counter : IComparable
    {
        public int Value { get; set; }

        public int CompareTo(object obj) // Object parameter causes boxing
        {
            if (obj is Counter other)
                return Value.CompareTo(other.Value);
            throw new ArgumentException();
        }
    }
}
