namespace Xiphos.Utilities;

public static class SpanLinq
{
    public static T First<T>(this ReadOnlySpan<T> span, Func<T, bool> comparator)
    {
        for (int i = 0; i < span.Length; i++)
            if (comparator(span[i]))
                return span[i];
        throw new InvalidOperationException("No matching element");
    }

    public static T First<T>(this ReadOnlySpan<T> span)
        => span.First(t => true);

    public static T First<T>(this Span<T> span, Func<T, bool> comparator)
    {
        for (int i = 0; i < span.Length; i++)
            if (comparator(span[i]))
                return span[i];
        throw new InvalidOperationException("No matching element");
    }

    public static T First<T>(this Span<T> span)
        => span.First(t => true);

    public static int FirstIndex<T>(this ReadOnlySpan<T> span, Func<T, bool> comparator)
    {
        for (int i = 0; i < span.Length; i++)
            if (comparator(span[i]))
                return i;
        return -1;
    }

    public static int FirstIndex<T>(this Span<T> span, Func<T, bool> comparator)
    {
        for (int i = 0; i < span.Length; i++)
            if (comparator(span[i]))
                return i;
        return -1;
    }

    public static Span<T> Until<T>(this Span<T> span, Func<T, bool> comparator) 
        => span.FirstIndex(comparator) is not -1 and var idx ? span[..idx] : span;

    public static ReadOnlySpan<T> Until<T>(this ReadOnlySpan<T> span, Func<T, bool> comparator)
        => span.FirstIndex(comparator) is not -1 and var idx ? span[..idx] : span;

    public static Span<T> After<T>(this Span<T> span, Func<T, bool> comparator)
        => span.FirstIndex(comparator) is not -1 and var idx ? span[(idx + 1)..] : Span<T>.Empty;

    public static ReadOnlySpan<T> After<T>(this ReadOnlySpan<T> span, Func<T, bool> comparator)
        => span.FirstIndex(comparator) is not -1 and var idx ? span[(idx + 1)..] : Span<T>.Empty;

    public static void Split<T>(this Span<T> span, Func<T, bool> comparator, out Span<T> left, out Span<T> right)
    {
        int index = span.FirstIndex(comparator);
        left = span[..index];
        right = span[(index + 1)..];
    }

    public static void Split<T>(this ReadOnlySpan<T> span, Func<T, bool> comparator, out ReadOnlySpan<T> left, out ReadOnlySpan<T> right)
    {
        int index = span.FirstIndex(comparator);
        left = span[..index];
        right = span[(index + 1)..];
    }
}
