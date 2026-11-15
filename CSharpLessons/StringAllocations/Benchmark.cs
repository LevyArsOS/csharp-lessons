using System.Text;
using BenchmarkDotNet.Attributes;
using CSharpLessons.StringAllocations.Data;

namespace CSharpLessons.StringAllocations;

[MemoryDiagnoser]
public class Benchmark
{
    private const int EventCount = 2_000;
    private CombatEvent[] _events = null!;
    private StringBuilder _builder = null!;
    private char[] _scratchBuffer = null!;

    [GlobalSetup]
    public void Setup()
    {
        _events = new CombatEvent[EventCount];
        for (int i = 0; i < EventCount; i++)
        {
            int damage = (i * 13) % 990;
            int maxHp = 1500;
            int remainingHp = maxHp - damage;
            _events[i] = new CombatEvent(i + 1, damage, remainingHp, maxHp);
        }

        _builder = new StringBuilder(64);
        _scratchBuffer = new char[64];
    }

    [Benchmark(Baseline = true)]
    public long FormatNaiveConcatenation()
    {
        return CombatTextFormatter.FormatBatchNaive(_events);
    }

    [Benchmark]
    public long FormatReusableStringBuilder()
    {
        return CombatTextFormatter.FormatBatchStringBuilder(_events, _builder);
    }

    [Benchmark]
    public long FormatSpanTryFormat()
    {
        return CombatTextFormatter.FormatBatchSpan(_events, _scratchBuffer);
    }

    [Benchmark]
    public long LookupPreallocatedDamageStrings()
    {
        return CombatTextFormatter.SumDamageLabelsCached(_events);
    }
}
