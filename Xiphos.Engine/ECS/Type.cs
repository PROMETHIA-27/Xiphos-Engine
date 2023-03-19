namespace Xiphos.ECS
{
    public struct Type<T>
    {
        public static Type<T> Get { get; } = new();
    }
}
