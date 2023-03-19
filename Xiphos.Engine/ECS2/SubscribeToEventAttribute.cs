namespace Xiphos.ECS2
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class SubscribeToEventAttribute : Attribute
    {
        public SubscribeToEventAttribute(params Type[] types) { }
    }
}
