using System;
using System.Globalization;
using System.Text;

namespace CSharpLessons.StringAllocations.Data;

/// <summary>
/// Compares per-frame string concatenation against reusable StringBuilder,
/// zero-allocation Span&lt;char&gt; formatting, and pre-allocated string lookup tables.
/// </summary>
public static class CombatTextFormatter
{
    public const int MaxCachedDamage = 1000;

    private static readonly string[] CachedDamageStrings = CreateDamageTable();

    private static string[] CreateDamageTable()
    {
        var table = new string[MaxCachedDamage];
        for (int i = 0; i < table.Length; i++)
        {
            table[i] = string.Concat("-", i.ToString(CultureInfo.InvariantCulture));
        }

        return table;
    }

    public static string FormatEventNaive(CombatEvent ev)
    {
        return "#" + ev.TargetId.ToString(CultureInfo.InvariantCulture)
            + " -" + ev.Damage.ToString(CultureInfo.InvariantCulture)
            + " (" + ev.RemainingHp.ToString(CultureInfo.InvariantCulture)
            + "/" + ev.MaxHp.ToString(CultureInfo.InvariantCulture) + ")";
    }

    public static long FormatBatchNaive(ReadOnlySpan<CombatEvent> events)
    {
        long checksum = 0;
        for (int i = 0; i < events.Length; i++)
        {
            string text = FormatEventNaive(events[i]);
            for (int c = 0; c < text.Length; c++)
            {
                checksum += text[c];
            }
        }

        return checksum;
    }

    public static long FormatBatchStringBuilder(ReadOnlySpan<CombatEvent> events, StringBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        long checksum = 0;
        for (int i = 0; i < events.Length; i++)
        {
            CombatEvent ev = events[i];
            builder.Clear();
            builder.Append('#')
                .Append(ev.TargetId)
                .Append(" -")
                .Append(ev.Damage)
                .Append(" (")
                .Append(ev.RemainingHp)
                .Append('/')
                .Append(ev.MaxHp)
                .Append(')');

            for (int c = 0; c < builder.Length; c++)
            {
                checksum += builder[c];
            }
        }

        return checksum;
    }

    public static bool TryFormatEventSpan(CombatEvent ev, Span<char> destination, out int charsWritten)
    {
        int pos = 0;
        if (destination.Length < 8)
        {
            charsWritten = 0;
            return false;
        }

        destination[pos++] = '#';
        if (!ev.TargetId.TryFormat(destination.Slice(pos), out int n, default, CultureInfo.InvariantCulture))
        {
            charsWritten = 0;
            return false;
        }
        pos += n;

        if (pos + 2 > destination.Length)
        {
            charsWritten = 0;
            return false;
        }
        destination[pos++] = ' ';
        destination[pos++] = '-';

        if (!ev.Damage.TryFormat(destination.Slice(pos), out n, default, CultureInfo.InvariantCulture))
        {
            charsWritten = 0;
            return false;
        }
        pos += n;

        if (pos + 2 > destination.Length)
        {
            charsWritten = 0;
            return false;
        }
        destination[pos++] = ' ';
        destination[pos++] = '(';

        if (!ev.RemainingHp.TryFormat(destination.Slice(pos), out n, default, CultureInfo.InvariantCulture))
        {
            charsWritten = 0;
            return false;
        }
        pos += n;

        if (pos + 1 > destination.Length)
        {
            charsWritten = 0;
            return false;
        }
        destination[pos++] = '/';

        if (!ev.MaxHp.TryFormat(destination.Slice(pos), out n, default, CultureInfo.InvariantCulture))
        {
            charsWritten = 0;
            return false;
        }
        pos += n;

        if (pos + 1 > destination.Length)
        {
            charsWritten = 0;
            return false;
        }
        destination[pos++] = ')';

        charsWritten = pos;
        return true;
    }

    public static long FormatBatchSpan(ReadOnlySpan<CombatEvent> events, Span<char> scratchBuffer)
    {
        long checksum = 0;
        for (int i = 0; i < events.Length; i++)
        {
            if (!TryFormatEventSpan(events[i], scratchBuffer, out int written))
            {
                throw new ArgumentException("Scratch buffer is too small for CombatEvent.", nameof(scratchBuffer));
            }

            for (int c = 0; c < written; c++)
            {
                checksum += scratchBuffer[c];
            }
        }

        return checksum;
    }

    public static string GetDamageStringCached(int damage)
    {
        if ((uint)damage < (uint)CachedDamageStrings.Length)
        {
            return CachedDamageStrings[damage];
        }

        return string.Concat("-", damage.ToString(CultureInfo.InvariantCulture));
    }

    public static long SumDamageLabelsCached(ReadOnlySpan<CombatEvent> events)
    {
        long checksum = 0;
        for (int i = 0; i < events.Length; i++)
        {
            string label = GetDamageStringCached(events[i].Damage);
            checksum += label.Length;
        }

        return checksum;
    }
}
