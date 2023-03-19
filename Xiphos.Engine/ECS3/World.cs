namespace Xiphos.ECS3;

public interface IWorldTag { }

public static class World
{
    public enum AddStateError
    {
        SignatureCollision,
    }
}

public static class World<TWorldTag>
    where TWorldTag : IWorldTag
{
    private static readonly List<EntityState> _entityStates = new();
    private static readonly List<EntityState> _queryList = new(16);
    public static EntityState[] Query<T>(T query)
        where T : IQueryExpression
    {
        _queryList.Clear();
        foreach (EntityState cache in _entityStates)
        {
            if (query.Evaluate(cache))
                _queryList.Add(cache);
        }

        EntityState[] statesArr = new EntityState[_queryList.Count];
        _queryList.CopyTo(statesArr);
        return statesArr;
    }

    public static void AddState(EntityState state)
        => _entityStates.Add(state);
}
