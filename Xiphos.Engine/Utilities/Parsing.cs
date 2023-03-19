namespace Xiphos.Utilities;

public ref struct SpanStream<TToken>
{
    public ReadOnlySpan<TToken> span;
    int _index;

    public SpanStream(ReadOnlySpan<TToken> span)
    {
        this.span = span;
        this._index = 0;
    }

    public TToken Consume() => span[_index++];
    public void Seek(int offset) => _index += offset;
    public int Length => span.Length - _index;
    public int Position => _index;

    public SpanStream<TToken> SubStream(int start, int length) 
        => new() { span = this.span[start..(start + length)], _index = 0 };
}

public interface IPattern<TToken, TReturn>
    where TToken : IEqualityOperators<TToken, TToken>
{
    TReturn Parse(ref SpanStream<TToken> stream, out bool succeeded, out bool consumed);
}

public readonly struct Token<TToken> : IPattern<TToken, TToken>
    where TToken : IEqualityOperators<TToken, TToken>
{
    readonly TToken _token;

    public Token(TToken token)
        => this._token = token;

    public TToken Parse(ref SpanStream<TToken> stream, out bool succeeded, out bool consumed)
    {
        TToken token;
        if (stream.Length > 0 && (token = stream.Consume()) == this._token)
        {
            succeeded = true;
            consumed = true;
            return token;
        }

        succeeded = false;
        consumed = false;
        return default!;
    }
}

public readonly struct Any<TToken> : IPattern<TToken, TToken>
    where TToken : IEqualityOperators<TToken, TToken>
{
    public TToken Parse(ref SpanStream<TToken> stream, out bool succeeded, out bool consumed)
    {
        if (stream.Length > 0)
        {
            TToken tok = stream.Consume();
            succeeded = true;
            consumed = true;
            return tok;
        }
        succeeded = false;
        consumed = false;
        return default!;
    }
}

public readonly struct None<TToken> : IPattern<TToken, Unit>
    where TToken : IEqualityOperators<TToken, TToken>
{
    public Unit Parse(ref SpanStream<TToken> stream, out bool succeeded, out bool consumed)
    {
        if (stream.Length > 0)
        {
            _ = stream.Consume();
            succeeded = false;
            consumed = true;
            return default;
        }
        succeeded = false;
        consumed = false;
        return default;
    }
}

public readonly struct Sequence<TToken> : IPattern<TToken, IEnumerable<TToken>>
    where TToken : IEqualityOperators<TToken, TToken>
{
    readonly IEnumerable<TToken> _pattern;

    public Sequence(IEnumerable<TToken> pattern)
        => this._pattern = pattern;

    public IEnumerable<TToken> Parse(ref SpanStream<TToken> stream, out bool succeeded, out bool consumed)
    {
        if (stream.Length >= this._pattern.Count())
            foreach (TToken tok in this._pattern)
                if (stream.Consume() != tok)
                {
                    succeeded = false;
                    consumed = true;
                    return default!;
                }

        succeeded = true;
        consumed = true;
        return this._pattern.Select(t => t);
    }
}

public readonly struct Many<TToken, TReturn, TPattern> : IPattern<TToken, IEnumerable<TReturn>>
    where TToken :  IEqualityOperators<TToken, TToken>
    where TPattern : IPattern<TToken, TReturn>
{
    readonly TPattern _pattern;

    public Many(TPattern pattern)
        => this._pattern = pattern;

    public IEnumerable<TReturn> Parse(ref SpanStream<TToken> stream, out bool succeeded, out bool consumed)
    {
        List<TReturn> returns = new();
        consumed = false;
        bool success;
        do
        {
            TReturn result = this._pattern.Parse(ref stream, out success, out bool localConsumed);
            if (localConsumed)
                consumed = true;
            if (success)
                returns.Add(result);
        }
        while (success);

        succeeded = success || !consumed;
        return returns;
    }
}

public readonly struct Backtracker<TToken, TReturn, TPattern> : IPattern<TToken, TReturn>
    where TToken : IEqualityOperators<TToken, TToken>
    where TPattern : IPattern<TToken, TReturn>
{
    readonly TPattern _pattern;

    public Backtracker(TPattern pattern)
        => this._pattern = pattern;

    public TReturn Parse(ref SpanStream<TToken> stream, out bool succeeded, out bool consumed)
    {
        int startPosition = stream.Position;
        TReturn result = this._pattern.Parse(ref stream, out bool success, out bool localConsumed);
        if (!success)
        {
            stream.Seek(startPosition - stream.Position);
            succeeded = false;
            consumed = false;
            return default!;
        }

        succeeded = true;
        consumed = localConsumed;
        return result;
    }
}

public readonly struct AnyExcept<TToken> : IPattern<TToken, TToken>
    where TToken : IEqualityOperators<TToken, TToken>
{
    readonly ImmutableArray<TToken> _exceptions;

    public AnyExcept(params TToken[] tokens) 
        => this._exceptions = tokens.ToImmutableArray();

    public TToken Parse(ref SpanStream<TToken> stream, out bool succeeded, out bool consumed)
    {
        TToken token = stream.Consume();
        for (int i = 0; i < this._exceptions.Length; i++)
            if (token == this._exceptions[i])
            {
                succeeded = false;
                consumed = true;
                return default!;
            }

        succeeded = true;
        consumed = true;
        return token;
    }
}

public readonly struct LeftSequence<TToken, TPattern, TReturn, UPattern, UReturn> : IPattern<TToken, TReturn>
    where TToken : IEqualityOperators<TToken, TToken>
    where TPattern : IPattern<TToken, TReturn>
    where UPattern : IPattern<TToken, UReturn>
{
    readonly TPattern _left;
    readonly UPattern _right;

    public LeftSequence(TPattern left, UPattern right)
        => (this._left, this._right) = (left, right);

    public TReturn Parse(ref SpanStream<TToken> stream, out bool succeeded, out bool consumed)
    {
        TReturn result = this._left.Parse(ref stream, out bool leftSucceeded, out bool leftConsumed);
        if (!leftSucceeded)
        {
            succeeded = false;
            consumed = leftConsumed;
            return default!;
        }

        _ = this._right.Parse(ref stream, out bool rightSucceeded, out bool rightConsumed);
        if (!rightSucceeded)
        {
            succeeded = false;
            consumed = leftConsumed || rightConsumed;
            return default!;
        }

        succeeded = true;
        consumed = leftConsumed || rightConsumed;
        return result;
    }
}

public readonly struct RightSequence<TToken, TPattern, TReturn, UPattern, UReturn> : IPattern<TToken, UReturn>
    where TToken : IEqualityOperators<TToken, TToken>
    where TPattern : IPattern<TToken, TReturn>
    where UPattern : IPattern<TToken, UReturn>
{
    readonly TPattern _left;
    readonly UPattern _right;

    public RightSequence(TPattern left, UPattern right)
        => (this._left, this._right) = (left, right);

    public UReturn Parse(ref SpanStream<TToken> stream, out bool succeeded, out bool consumed)
    {
        _ = this._left.Parse(ref stream, out bool leftSucceeded, out bool leftConsumed);
        if (!leftSucceeded)
        {
            succeeded = false;
            consumed = leftConsumed;
            return default!;
        }

        UReturn result = this._right.Parse(ref stream, out bool rightSucceeded, out bool rightConsumed);
        if (!rightSucceeded)
        {
            succeeded = false;
            consumed = leftConsumed || rightConsumed;
            return default!;
        }

        succeeded = true;
        consumed = leftConsumed || rightConsumed;
        return result;
    }
}

public readonly struct Optional<TToken, TReturn, TPattern> : IPattern<TToken, Maybe<TReturn>>
    where TToken : IEqualityOperators<TToken, TToken>
    where TPattern : IPattern<TToken, TReturn>
{
    readonly TPattern _pattern;

    public Optional(TPattern pattern)
        => this._pattern = pattern;

    public Maybe<TReturn> Parse(ref SpanStream<TToken> stream, out bool succeeded, out bool consumed)
    {
        TReturn result = this._pattern.Parse(ref stream, out bool success, out bool localConsumed);
        if (!success && !localConsumed)
        {
            succeeded = true;
            consumed = false;
            return new();
        }
        else if (localConsumed)
        {
            succeeded = false;
            consumed = true;
            return default;
        }
        else
        {
            succeeded = true;
            consumed = localConsumed;
            return new(result);
        }
    }
}

public readonly struct Map<TToken, TReturn, UReturn, TPattern> : IPattern<TToken, UReturn>
    where TToken : IEqualityOperators<TToken, TToken>
    where TPattern : IPattern<TToken, TReturn>
{
    readonly TPattern _pattern;
    readonly Func<TReturn, UReturn> _map;

    public Map(TPattern pattern, Func<TReturn, UReturn> map)
        => (this._pattern, this._map) = (pattern, map);

    public UReturn Parse(ref SpanStream<TToken> stream, out bool succeeded, out bool consumed)
    {
        TReturn premapResult = this._pattern.Parse(ref stream, out bool success, out bool localConsumed);
        if (success)
        {
            succeeded = true;
            consumed = localConsumed;
            return this._map(premapResult);
        }
        else
        {
            succeeded = false;
            consumed = localConsumed;
            return default!;
        }
    }
}

public static class Parsing
{
    public static Backtracker<TToken, TReturn, TPattern> Try<TToken, TReturn, TPattern>(this TPattern pattern)
        where TToken : IEqualityOperators<TToken, TToken>
        where TPattern : IPattern<TToken, TReturn>
        => new(pattern);

    public static Many<TToken, TReturn, TPattern> Many<TToken, TReturn, TPattern>(this TPattern pattern)
        where TToken : IEqualityOperators<TToken, TToken>
        where TPattern : IPattern<TToken, TReturn>
        => new(pattern);
}

public static class Parsing<TToken>
    where TToken : IEqualityOperators<TToken, TToken>
{
    public static None<TToken> None() => new();

    public static Any<TToken> Any() => new();

    public static Token<TToken> Token(TToken token) => new(token);

    public static Sequence<TToken> Sequence(IEnumerable<TToken> sequence) => new(sequence);
}