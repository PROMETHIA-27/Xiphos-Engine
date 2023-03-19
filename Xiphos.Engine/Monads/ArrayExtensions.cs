namespace Xiphos.Monads;

public static class ArrayExtensions
{
    public static void Deconstruct<T>(this T[] array, out T first, out ArraySegment<T> rest)
    {
        first = array[0];
        rest = new ArraySegment<T>(array, 1, array.Length - 1);
    }

    public static void Deconstruct<T>(this ArraySegment<T> array, out T first, out ArraySegment<T> rest)
    {
        first = array[0];
        rest = array.Slice(1);
    }

    public static void Deconstruct<T>(this Span<T> array, out T first, out Span<T> rest)
    {
        first = array[0];
        rest = array[1..^0];
    }

    public static T[] Flatten<T>(this T[][] array)
        => array.SelectMany(arr => arr.AsEnumerable()).ToArray();

    public static U[] Map<T, U>(this T[] array, Func<T, U> map)
        => array.Select(t => map(t)).ToArray();
}
