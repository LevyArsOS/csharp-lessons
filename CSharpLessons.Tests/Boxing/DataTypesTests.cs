using CSharpLessons.Boxing.Data;
using Xunit;

namespace CSharpLessons.Tests.Boxing;

// One behaviour test per example type of the Boxing lesson: equality and ordering must hold
// whether or not the type avoids boxing. Timings belong to the benchmark, not here.
public class DataTypesTests
{
    [Fact]
    public void Coordinate_equality_needs_the_custom_comparer()
    {
        var a = new Coordinate { X = 1, Y = 2 };
        var b = new Coordinate { X = 1, Y = 2 };
        var comparer = new CoordinateEqualityComparer();

        Assert.True(comparer.Equals(a, b));
        Assert.Equal(comparer.GetHashCode(a), comparer.GetHashCode(b));
        Assert.False(comparer.Equals(a, new Coordinate { X = 2, Y = 1 }));
    }

    [Fact]
    public void Counter_compares_by_value_and_rejects_other_types()
    {
        IComparable small = new Counter { Value = 1 };

        Assert.True(small.CompareTo(new Counter { Value = 2 }) < 0);
        Assert.Equal(0, small.CompareTo(new Counter { Value = 1 }));
        Assert.Throws<ArgumentException>(() => small.CompareTo("not a counter"));
    }

    [Fact]
    public void DataWithSpecializedOverride_equals_through_both_overloads()
    {
        var a = new DataWithSpecializedOverride { Id = 7, Velocity = 1.5f };
        var b = new DataWithSpecializedOverride { Id = 7, Velocity = 1.5f };

        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.Equal("[Id:7, Velocity:1.5]", a.ToString());
        Assert.False(a.Equals(new DataWithSpecializedOverride { Id = 7, Velocity = 2f }));
    }

    [Fact]
    public void DataWithoutSpecializedOverride_uses_default_value_equality()
    {
        var a = new DataWithoutSpecializedOverride { Id = 7, Velocity = 1.5f };
        var b = new DataWithoutSpecializedOverride { Id = 7, Velocity = 1.5f };

        Assert.True(a.Equals(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Point_equality_and_hash_follow_coordinates()
    {
        var a = new Point { X = 3, Y = 4 };
        var b = new Point { X = 3, Y = 4 };

        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
        Assert.False(a.Equals(new Point { X = 4, Y = 3 }));
    }

    [Fact]
    public void Product_equals_by_id_and_sorts_by_price()
    {
        var cheap = new Product { Id = 1, Price = 5m };
        var pricey = new Product { Id = 2, Price = 50m };
        var sameIdOtherPrice = new Product { Id = 1, Price = 99m };

        Assert.True(cheap.Equals(sameIdOtherPrice));
        Assert.False(cheap.Equals(pricey));
        Assert.True(cheap.CompareTo(pricey) < 0);

        var sorted = new List<Product> { pricey, cheap };
        sorted.Sort();
        Assert.Equal(new[] { 1, 2 }, sorted.Select(p => p.Id));
    }

    [Fact]
    public void Temperature_orders_through_generic_and_non_generic_interfaces()
    {
        var cold = new Temperature { Celsius = -5 };
        var warm = new Temperature { Celsius = 25 };

        Assert.True(cold.CompareTo(warm) < 0);
        Assert.True(((IComparable)warm).CompareTo(cold) > 0);
        Assert.Throws<ArgumentException>(() => ((IComparable)warm).CompareTo(42));
    }
}
