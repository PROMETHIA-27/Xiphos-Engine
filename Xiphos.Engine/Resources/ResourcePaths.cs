namespace Xiphos.Resources;

public static class ResourcePaths
{
    private static readonly Dictionary<FileInfo, RID> _pathRids = new();
    internal static ulong _lastResourceId = 0;

    internal static ulong NextResourceId => _lastResourceId++ is var rid && rid != ulong.MaxValue ? rid : throw new Exception("Reached maximum RID value!");

    public static Either<GetRIDError, RID> GetPathRID(FileInfo path, bool pathAlreadyCanon = false)
    {
        SanitizeError err;
        FileInfo? canonPath;
        if (!pathAlreadyCanon)
            err = SanitizePath(path, out canonPath);
        else
            (err, canonPath) = (SanitizeError.None, path);

        return err switch
        {
            SanitizeError.FileNotFound => new(GetRIDError.FileNotFound),
            SanitizeError.DirectoryNotFound => new(GetRIDError.DirectoryNotFound),
            SanitizeError.UnauthorizedAccess => new(GetRIDError.UnauthorizedAccess),
            SanitizeError.None => _pathRids.TryGetValue(canonPath!, out RID rid) switch
            {
                true => new(rid),
                false when _pathRids.TryAdd(canonPath!, rid = new() { ID = NextResourceId }) => new(rid),
                _ => throw new Exception("Invalid RID value"),
            },
            _ => throw new NotImplementedException(),
        };
    }

    internal static SanitizeError SanitizePath(FileInfo path, out FileInfo? canonPath)
    {
        try
        {
            path = path.ResolveLinkTarget(true) is FileInfo inf ? inf : path;
        }
        catch (DirectoryNotFoundException)
        {
            canonPath = null;
            return SanitizeError.DirectoryNotFound;
        }
        catch (FileNotFoundException)
        {
            canonPath = null;
            return SanitizeError.FileNotFound;
        }
        catch (UnauthorizedAccessException)
        {
            canonPath = null;
            return SanitizeError.UnauthorizedAccess;
        }

        DirectoryInfo dir = path.Directory!;

        StringBuilder builder = new(64);
        while (dir is not null)
        {
            if (dir.Attributes.HasFlag(FileAttributes.ReparsePoint))
                dir = (dir.ResolveLinkTarget(true) as DirectoryInfo)!;

            if (dir.Parent is not null) _ = builder.Insert(0, '/');
            _ = builder.Insert(0, dir.Name);

            dir = dir.Parent!;
        }

        string fullPath = builder.ToString();
        canonPath = new(fullPath + path.Name);

        return SanitizeError.None;
    }

    public enum GetRIDError
    {
        DirectoryNotFound,
        FileNotFound,
        UnauthorizedAccess,
    }

    public enum SanitizeError : sbyte
    {
        None,
        DirectoryNotFound,
        FileNotFound,
        UnauthorizedAccess,
    }
}

public readonly record struct RID(ulong ID);