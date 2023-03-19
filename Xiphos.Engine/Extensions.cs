namespace Xiphos;

public static class Extensions
{
    //public static void AddMany<T, U>(this T collection, params U[] items)
    //    where T : ICollection<U>
    //{
    //    for (int i = 0; i < items.Length; i++)
    //        collection.Add(items[i]);
    //}

    public static IEnumerator<int> GetEnumerator(this Range range)
    {
        for (int i = range.Start.Value; i < range.End.Value; i++)
            yield return i;
    }

    public static TEnumerable Append<TEnumerable, T>(this TEnumerable e, TEnumerable other)
        where TEnumerable : IEnumerable<T>
    {

    }
}
