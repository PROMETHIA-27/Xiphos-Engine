using Pidgin;

namespace Xiphos.Utilities;

public struct URI
{
    public readonly string Backing { get; init; }
    public string? Scheme { get; private set; }
    public string? Authority { get; private set; }
    public string? Path { get; private set; }
    public string? Query { get; private set; }
    public string? Fragment { get; private set; }

    public bool IsValid => throw new NotImplementedException();
    public bool IsRelative => throw new NotImplementedException();

    public URI(ReadOnlySpan<char> backing)
    {
        this.Backing = new(backing);
        this.Scheme = null;
        this.Authority = null;
        this.Path = null;
        this.Query = null;
        this.Fragment = null;

        this.ParsePath(backing);
    }

    private void ParsePath(ReadOnlySpan<char> pathInput)
    {
        Span<char> path = pathInput.Length <= 256 
            ? stackalloc char[pathInput.Length]
            : new char[pathInput.Length];
        pathInput.CopyTo(path);

        if (path.Contains(':'))
        {
            this.Scheme = new(path.Until(c => c is ':' or '/'));

            path = path.After(c => c is ':' or '/');
        }

        if (path.StartsWith("//"))
        {
            this.Authority = new(path[2..].Until(c => c is '/'));

            path = path[2..].After(c => c is '/');
        }

        this.Path = new(path.Until(c => c is '?'));

        path = path.After(c => c is '?');

        if (!path.IsEmpty)
        {
            Span<char> querySpan = path.Until(c => c is '#');
            if (!querySpan.IsEmpty)
            {
                this.Query = new(querySpan);

                path = path.After(c => c is '#');
            }
        }

        if (!path.IsEmpty)
            this.Fragment = new(path);        
    }
}
