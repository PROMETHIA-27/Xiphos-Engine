namespace Xiphos.ECS2
{
    public struct Type<T>
    {
        public static Type<T> Get { get; } = new();
    }
}
