namespace Xiphos.Utilities;

public interface IDeepCopyable<TSelf>
    where TSelf : IDeepCopyable<TSelf>
{
    public TSelf DeepCopy();
}

