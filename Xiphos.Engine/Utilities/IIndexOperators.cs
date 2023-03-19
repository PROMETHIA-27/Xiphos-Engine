namespace Xiphos.Utilities;

public interface IReadOnlyIndexOperator<TSelf, TIndex, TReturn>
    where TSelf : IReadOnlyIndexOperator<TSelf, TIndex, TReturn>
{
    TReturn this[TIndex index] { get; }
}

public interface IIndexOperator<TSelf, TIndex, TReturn>
    where TSelf : IIndexOperator<TSelf, TIndex, TReturn>
{
    TReturn this[TIndex index] { get; set; }
}

public interface IRefIndexOperator<TSelf, TIndex, TReturn>
    where TSelf : IRefIndexOperator<TSelf, TIndex, TReturn>
{
    ref TReturn this[TIndex index] { get; }
}

public interface ILength
{
    public int Length { get; }
}