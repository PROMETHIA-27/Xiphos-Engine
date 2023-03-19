namespace Xiphos.Utilities;

public interface IEnum<TBacking, TSelf>
    where TSelf : IEnum<TBacking, TSelf>, IEqualityOperators<TSelf, TSelf>
    where TBacking : IBinaryInteger<TBacking>
{
    static abstract explicit operator TBacking(TSelf @this);
    static abstract explicit operator TSelf(TBacking @this);
}

public interface IMaybeEnum<TBacking, TSelf> : IEnum<TBacking, TSelf>
    where TSelf : IMaybeEnum<TBacking, TSelf>, IEqualityOperators<TSelf, TSelf>
    where TBacking : IBinaryInteger<TBacking>
{
    static abstract TSelf None { get; }
}
