using static Xiphos.Utilities.Parsing;
using static Xiphos.Utilities.Parsing<char>;

namespace Xiphos.Utilities;

public static class StringParsing
{
    public static Token<char> Char(char c) => new(c);
    public static Sequence<char> String(string s) => new(s);

    public static Many<char, char, Any<char>> AnyString() => Any().Many();
}

//public readonly struct CharSequence : IReadOnlyIndexOperator<CharSequence, int, char>, ILength, IDeepCopyable<CharSequence>
//{
//    public string Backing { get; init; } 

//    public char this[int index] => this.Backing[index];

//    public int Length => this.Backing.Length;

//    public CharSequence(string backing)
//        => this.Backing = backing;

//    public CharSequence DeepCopy()
//        => new() { Backing = this.Backing };

//    public override string ToString() => this.Backing;
//}