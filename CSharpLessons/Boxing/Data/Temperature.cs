namespace CSharpLessons.Boxing.Data
{
    public struct Temperature : IComparable<Temperature>, IComparable
    {
        public double Celsius { get; set; }

        // IComparable<T> - NO boxing
        public int CompareTo(Temperature other) => Celsius.CompareTo(other.Celsius);

        // IComparable - WITH boxing (parameter is object)
        int IComparable.CompareTo(object obj)
        {
            if (obj is Temperature other)
                return CompareTo(other);
            throw new ArgumentException("Object is not a Temperature");
        }
    }
}
