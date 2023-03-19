namespace Xiphos.Monads;

public static class TupleExtensions
{
    public static (T[], U[]) Flatten<T, U>(this (T, U)[] tupArr)
    {
        T[] tarr = new T[tupArr.Length];
        U[] uarr = new U[tupArr.Length];
        for (int i = 0; i < tupArr.Length; i++)
            (tarr[i], uarr[i]) = (tupArr[i].Item1, tupArr[i].Item2);
        return (tarr, uarr);
    }
}
