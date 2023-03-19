namespace Xiphos.ECS
{
    //public interface ISystem
    //{
    //    List<ulong> ComponentDependencies { get; }
    //    List<bool> ComponentsWritten { get; }
    //    List<ISystem> Before { get; }
    //    List<ISystem> After { get; }
    //    int Rank { get; set; }

    //    public bool HasDependencies => this.Before.Count > 0;
    //    public bool HasDependants => this.After.Count > 0;

    //    public void Invoke(in ComponentPack pack);
    //}

    //public static class SystemExtensions
    //{
    //    public static T AddDependency<T, U>(this T system, in U before)
    //        where T : ISystem
    //        where U : ISystem
    //    {
    //        system.Before.Add(before);
    //        before.After.Add(system);
    //        if (system.Rank <= before.Rank)
    //            system.Rank = before.Rank + 1;
    //        for (int i = 0; i < system.After.Count; i++)
    //            system.After[i].UpdateRank(system.Rank);
    //        return system;
    //    }

    //    public static T NeedsComponent<T, U>(this T system, Type<U> compType, bool writeAccess = false)
    //        where T : ISystem
    //    {
    //        system.ComponentDependencies.Add(TypeIndex<U>.Index);
    //        system.ComponentsWritten.Add(writeAccess);
    //        return system;
    //    }
    //}

    //internal static class InternalSystemExtensions
    //{
    //    public static void UpdateRank<T>(this T system, int rank)
    //        where T : ISystem
    //    {
    //        if (system.Rank <= rank)
    //        {
    //            system.Rank = rank + 1;
    //            for (int i = 0; i < system.After.Count; i++)
    //                system.After[i].UpdateRank(system.Rank);
    //        }
    //    }

    //    public static void GetConnectedSystems<T, U>(this T system, in U collection)
    //        where T : ISystem
    //        where U : ICollection<ISystem>
    //    {
    //        collection.Add(system);
    //        AddConnected(system, collection);

    //        static void AddConnected(ISystem system, U collection)
    //        {
    //            for (int i = 0; i < system.Before.Count; i++)
    //            {
    //                if (!collection.Contains(system.Before[i]))
    //                {
    //                    collection.Add(system.Before[i]);
    //                    AddConnected(system.Before[i], collection);
    //                }
    //            }
    //            for (int i = 0; i < system.After.Count; i++)
    //            {
    //                if (!collection.Contains(system.After[i]))
    //                {
    //                    collection.Add(system.After[i]);
    //                    AddConnected(system.After[i], collection);
    //                }
    //            }
    //        }
    //    }

    //    public static bool IsNodeGraphAcyclic<T>(this T inputNodes)
    //        where T : IEnumerable<ISystem>, ICollection<ISystem>
    //    {
    //        HashSet<ISystem> nodes = new(inputNodes.Count);
    //        nodes.UnionWith(inputNodes);
    //        List<(ISystem, ISystem)> edges = new();

    //        foreach (var node in nodes)
    //            for (int i = 0; i < node.Before.Count; i++)
    //                edges.Add((node, node.Before[i]));

    //        List<int> removals = new(4);
    //        while (nodes.Count > 0)
    //        {
    //            removals.Clear();
    //            var leaf = FindLeaf(nodes, edges);
    //            if (leaf == null)
    //                return false;
    //            nodes.Remove(leaf);
    //            for (int i = 0; i < edges.Count; i++)
    //                if (edges[i].Item2 == leaf)
    //                    removals.Add(i);
    //            for (int i = 0; i < removals.Count; i++)
    //                edges.RemoveAt(removals[i]);
    //        }

    //        return true;

    //        static bool IsLeaf(ISystem system, List<(ISystem, ISystem)> edges)
    //        {
    //            for (int i = 0; i < edges.Count; i++)
    //                if (edges[i].Item1 == system)
    //                    return false;
    //            return true;
    //        }

    //        static ISystem? FindLeaf(HashSet<ISystem> nodes, List<(ISystem, ISystem)> edges)
    //        {
    //            foreach (var node in nodes)
    //                if (IsLeaf(node, edges))
    //                    return node;
    //            return null;
    //        }
    //    }

    //    public static bool IsDistantDependantOf<T, U>(this T system, in U other)
    //        where T : ISystem
    //        where U : ISystem
    //    {
    //        return CheckDependencies(system, other);

    //        static bool CheckDependencies<V, Y>(V system, Y target)
    //            where V : ISystem
    //            where Y : ISystem
    //        {
    //            if (system.Equals(target))
    //                return true;
    //            for (int i = 0; i < system.Before.Count; i++)
    //            {
    //                if (CheckDependencies(system.Before[i], target))
    //                    return true;
    //            }
    //            return false;
    //        }
    //    }

    //    public static bool IsDistantDependencyOf<T, U>(this T system, in U other)
    //        where T : ISystem
    //        where U : ISystem
    //    {
    //        return CheckDependants(system, other);

    //        static bool CheckDependants<V, Y>(V system, Y target)
    //            where V : ISystem
    //            where Y : ISystem
    //        {
    //            if (system.Equals(target))
    //                return true;
    //            for (int i = 0; i < system.After.Count; i++)
    //            {
    //                if (CheckDependants(system.After[i], target))
    //                    return true;
    //            }
    //            return false;
    //        }
    //    }

    //    public static bool VerifyStrictDependencyChains<T>(this T nodes)
    //        where T : IEnumerable<ISystem>
    //    {
    //        Dictionary<ulong, (List<ISystem> channel, List<bool> componentWriters)> channels = new();
    //        foreach (var node in nodes)
    //            for (int i = 0; i < node.ComponentDependencies.Count; i++)
    //            {
    //                channels.TryAdd(node.ComponentDependencies[i], (new(4), new(4)));
    //                channels[node.ComponentDependencies[i]].channel.Add(node);
    //                channels[node.ComponentDependencies[i]].componentWriters.Add(node.ComponentsWritten[i]);
    //            }

    //        bool result = true;
    //        foreach (var pair in channels)
    //            if (!VerifyStrictDependencyChain(pair.Value.channel, pair.Value.componentWriters, pair.Key))
    //                result = false;

    //        return result;
    //    }

    //    public static bool VerifyStrictDependencyChain(this List<ISystem> channel, List<bool> componentWriters, ulong compIndex)
    //    {
    //        List<ISystem> readers = new();
    //        List<ISystem> writers = new();

    //        for (int i = 0; i < channel.Count; i++)
    //        {
    //            if (componentWriters[i])
    //                writers.Add(channel[i]);
    //            else
    //                readers.Add(channel[i]);
    //        }

    //        HashSet<ISystem> set = new();
    //        for (int i = 0; i < writers.Count; i++)
    //        {
    //            set.Clear();
    //            writers[i].GetAllDependants(set, compIndex);
    //            writers[i].GetAllDependencies(set, compIndex);

    //            for (int j = i + 1; j < writers.Count; j++)
    //            {
    //                if (!set.Contains(writers[j]))
    //                    return false;
    //            }
    //        }
    //        for (int i = 0; i < readers.Count; i++)
    //        {
    //            set.Clear();
    //            readers[i].GetAllDependants(set, compIndex);
    //            readers[i].GetAllDependencies(set, compIndex);

    //            for (int j = 0; j < writers.Count; j++)
    //            {
    //                if (!set.Contains(writers[j]))
    //                    return false;
    //            }
    //        }

    //        return true;
    //    }

    //    public static void GetAllDependants<T, U>(this T system, in U collection, ulong compIdxFilter = ulong.MaxValue)
    //        where T : ISystem
    //        where U : ICollection<ISystem>
    //    {
    //        AddDependantsRecursive(system, collection, compIdxFilter);

    //        static void AddDependantsRecursive(ISystem system, U collection, ulong filter)
    //        {
    //            for (int i = 0; i < system.After.Count; i++)
    //            {
    //                AddDependantsRecursive(system.After[i], collection, filter);
    //                if (!collection.Contains(system.After[i]) && (filter == ulong.MaxValue || system.After[i].ComponentDependencies.Contains(filter)))
    //                    collection.Add(system.After[i]);
    //            }
    //        }
    //    }

    //    public static void GetAllDependencies<T, U>(this T system, in U collection, ulong compIdxFilter = ulong.MaxValue)
    //        where T : ISystem
    //        where U : ICollection<ISystem>
    //    {
    //        AddDependenciesRecursive(system, collection, compIdxFilter);

    //        static void AddDependenciesRecursive(ISystem system, U collection, ulong filter)
    //        {
    //            for (int i = 0; i < system.Before.Count; i++)
    //            {
    //                AddDependenciesRecursive(system.Before[i], collection, filter);
    //                if (!collection.Contains(system.Before[i]) && (filter == ulong.MaxValue || system.Before[i].ComponentDependencies.Contains(filter)))
    //                    collection.Add(system.Before[i]);
    //            }
    //        }
    //    }
    //}
}
