namespace Xiphos.ECS2
{
    internal static class World<TWorldTag>
    {
        private static readonly Dictionary<DynamicBitflags, EntityStateCache> entityStates = new();
        private static readonly List<EntityStateCache> queryList = new(16);
        public static EntityStateCache[] QueryEntityStates<T>()
            where T : IHasExpression, new()
        {
            queryList.Clear();
            T? query = new T();
            foreach (EntityStateCache cache in entityStates.Values)
            {
                if (query.Evaluate(cache))
                    queryList.Add(cache);
            }

            EntityStateCache[] statesArr = new EntityStateCache[queryList.Count];
            queryList.CopyTo(statesArr);
            return statesArr;
        }
    }
}
