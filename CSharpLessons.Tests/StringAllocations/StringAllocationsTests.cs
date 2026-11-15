using System;
using System.Text;
using CSharpLessons.StringAllocations.Data;
using Xunit;

namespace CSharpLessons.Tests.StringAllocations;

public sealed class StringAllocationsTests
{
    private static CombatEvent[] CreateSampleEvents(int count)
    {
        var events = new CombatEvent[count];
        for (int i = 0; i < count; i++)
        {
            int damage = (i * 17) % 950;
            int maxHp = 1000 + (i % 500);
            int remainingHp = Math.Max(0, maxHp - damage);
            events[i] = new CombatEvent(i + 1, damage, remainingHp, maxHp);
        }

        return events;
    }

    [Fact]
    public void TryFormatEventSpan_MatchesNaiveFormattedString()
    {
        var ev = new CombatEvent(TargetId: 42, Damage: 185, RemainingHp: 815, MaxHp: 1000);
        string expected = CombatTextFormatter.FormatEventNaive(ev);

        Span<char> buffer = stackalloc char[64];
        bool ok = CombatTextFormatter.TryFormatEventSpan(ev, buffer, out int written);

        Assert.True(ok);
        Assert.Equal("#42 -185 (815/1000)", expected);
        Assert.Equal(expected, new string(buffer.Slice(0, written)));
    }

    [Fact]
    public void FormatBatch_AllStrategiesProduceIdenticalChecksum()
    {
        CombatEvent[] events = CreateSampleEvents(500);
        var sb = new StringBuilder(64);
        Span<char> scratch = stackalloc char[64];

        long naiveSum = CombatTextFormatter.FormatBatchNaive(events);
        long sbSum = CombatTextFormatter.FormatBatchStringBuilder(events, sb);
        long spanSum = CombatTextFormatter.FormatBatchSpan(events, scratch);

        Assert.Equal(naiveSum, sbSum);
        Assert.Equal(naiveSum, spanSum);
    }

    [Fact]
    public void FormatBatchSpan_And_CachedLookup_AllocateZeroBytesOnHotPath()
    {
        CombatEvent[] events = CreateSampleEvents(256);
        char[] scratch = new char[64];

        // Warm up JIT and static lookup table
        _ = CombatTextFormatter.FormatBatchSpan(events, scratch);
        _ = CombatTextFormatter.SumDamageLabelsCached(events);

        long before = GC.GetAllocatedBytesForCurrentThread();
        long sum1 = CombatTextFormatter.FormatBatchSpan(events, scratch);
        long sum2 = CombatTextFormatter.SumDamageLabelsCached(events);
        long after = GC.GetAllocatedBytesForCurrentThread();

        Assert.True(sum1 > 0);
        Assert.True(sum2 > 0);
        Assert.Equal(0L, after - before);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(10)]
    [InlineData(15)]
    public void TryFormatEventSpan_ReturnsFalseWhenBufferTooSmall(int bufferSize)
    {
        var ev = new CombatEvent(TargetId: 9999, Damage: 888, RemainingHp: 1234, MaxHp: 5000);
        Span<char> tiny = stackalloc char[bufferSize];

        bool ok = CombatTextFormatter.TryFormatEventSpan(ev, tiny, out int written);

        Assert.False(ok);
        Assert.Equal(0, written);
    }

    [Fact]
    public void GetDamageStringCached_ReturnsCachedForInRangeAndFallbackForOutOfRange()
    {
        string cached1 = CombatTextFormatter.GetDamageStringCached(250);
        string cached2 = CombatTextFormatter.GetDamageStringCached(250);
        string fallback = CombatTextFormatter.GetDamageStringCached(5000);

        Assert.Same(cached1, cached2);
        Assert.Equal("-250", cached1);
        Assert.Equal("-5000", fallback);
    }
}
