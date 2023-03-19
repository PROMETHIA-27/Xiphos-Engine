namespace Xiphos.ECS2
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    internal class InitialEntitiesAttribute : Attribute
    {
        public int InitialEntities { get; init; }

        public InitialEntitiesAttribute(int initialEntities)
            => this.InitialEntities = initialEntities;
    }
}
