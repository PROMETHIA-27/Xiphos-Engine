//namespace Xiphos.ECS
//{
//    public class ComponentSystem
//    {
//        internal List<ulong> ComponentDependencies { get; } = ThreadSafePool<XiphosStatic>.Take<List<ulong>>();
//        internal List<bool> ComponentsWritten { get; } = ThreadSafePool<XiphosStatic>.Take<List<bool>>();
//        internal List<ComponentSystem> Before { get; } = ThreadSafePool<XiphosStatic>.Take<List<ComponentSystem>>();
//        internal List<ComponentSystem> After { get; } = ThreadSafePool<XiphosStatic>.Take<List<ComponentSystem>>();
//        internal int Rank { get; set; }
//        internal ComponentPack Pack { get; set; }

//        public bool HasDependencies => this.Before.Count > 0;
//        public bool HasDependants => this.After.Count > 0;

//        public Action<ComponentPack> Invoke;

//        public ComponentSystem(Action<ComponentPack> action)
//            => this.Invoke = action;
//    }

//    public static class SystemExtensions
//    {
//        public static ComponentSystem AddDependency(this ComponentSystem system, in ComponentSystem before)
//        {
//            system.Before.Add(before);
//            before.After.Add(system);
//            if (system.Rank <= before.Rank)
//                system.Rank = before.Rank + 1;
//            for (int i = 0; i < system.After.Count; i++)
//                system.After[i].UpdateRank(system.Rank);
//            return system;
//        }

//        public static ComponentSystem NeedsComponent<T>(this ComponentSystem system, Type<T> compType, bool writeAccess = false)
//        {
//            system.ComponentDependencies.Add(TypeIndex<T>.Index);
//            system.ComponentsWritten.Add(writeAccess);
//            return system;
//        }
//    }

//    internal static class InternalSystemExtensions
//    {
//        public static void UpdateRank(this ComponentSystem system, int rank)
//        {
//            if (system.Rank <= rank)
//            {
//                system.Rank = rank + 1;
//                for (int i = 0; i < system.After.Count; i++)
//                    system.After[i].UpdateRank(system.Rank);
//            }
//        }

//        public static void GetConnectedSystems<T>(this ComponentSystem system, in T collection)
//            where T : ICollection<ComponentSystem>
//        {
//            collection.Add(system);
//            AddConnected(system, collection);

//            static void AddConnected(ComponentSystem system, T collection)
//            {
//                for (int i = 0; i < system.Before.Count; i++)
//                {
//                    if (!collection.Contains(system.Before[i]))
//                    {
//                        collection.Add(system.Before[i]);
//                        AddConnected(system.Before[i], collection);
//                    }
//                }
//                for (int i = 0; i < system.After.Count; i++)
//                {
//                    if (!collection.Contains(system.After[i]))
//                    {
//                        collection.Add(system.After[i]);
//                        AddConnected(system.After[i], collection);
//                    }
//                }
//            }
//        }

//        public static bool IsNodeGraphAcyclic<T>(this T inputNodes)
//            where T : IEnumerable<ComponentSystem>, ICollection<ComponentSystem>
//        {
//            HashSet<ComponentSystem> nodes = new(inputNodes.Count);
//            nodes.UnionWith(inputNodes);
//            List<(ComponentSystem, ComponentSystem)> edges = new();

//            foreach (ComponentSystem? node in nodes)
//            {
//                for (int i = 0; i < node.Before.Count; i++)
//                    edges.Add((node, node.Before[i]));
//            }

//            List<int> removals = new(4);
//            while (nodes.Count > 0)
//            {
//                removals.Clear();
//                ComponentSystem? leaf = FindLeaf(nodes, edges);
//                if (leaf == null)
//                    return false;
//                nodes.Remove(leaf);
//                for (int i = 0; i < edges.Count; i++)
//                {
//                    if (edges[i].Item2 == leaf)
//                        removals.Add(i);
//                }

//                for (int i = 0; i < removals.Count; i++)
//                    edges.RemoveAt(removals[i]);
//            }

//            return true;

//            static bool IsLeaf(ComponentSystem system, List<(ComponentSystem, ComponentSystem)> edges)
//            {
//                for (int i = 0; i < edges.Count; i++)
//                {
//                    if (edges[i].Item1 == system)
//                        return false;
//                }

//                return true;
//            }

//            static ComponentSystem? FindLeaf(HashSet<ComponentSystem> nodes, List<(ComponentSystem, ComponentSystem)> edges)
//            {
//                foreach (ComponentSystem? node in nodes)
//                {
//                    if (IsLeaf(node, edges))
//                        return node;
//                }

//                return null;
//            }
//        }

//        public static bool IsDistantDependantOf(this ComponentSystem system, in ComponentSystem other)
//        {
//            return CheckDependencies(system, other);

//            static bool CheckDependencies(ComponentSystem system, ComponentSystem target)
//            {
//                if (system.Equals(target))
//                    return true;
//                for (int i = 0; i < system.Before.Count; i++)
//                {
//                    if (CheckDependencies(system.Before[i], target))
//                        return true;
//                }
//                return false;
//            }
//        }

//        public static bool IsDistantDependencyOf(this ComponentSystem system, in ComponentSystem other)
//        {
//            return CheckDependants(system, other);

//            static bool CheckDependants(ComponentSystem system, ComponentSystem target)
//            {
//                if (system.Equals(target))
//                    return true;
//                for (int i = 0; i < system.After.Count; i++)
//                {
//                    if (CheckDependants(system.After[i], target))
//                        return true;
//                }
//                return false;
//            }
//        }

//        public static bool VerifyStrictDependencyChains<T>(this T nodes)
//            where T : IEnumerable<ComponentSystem>
//        {
//            Dictionary<ulong, (List<ComponentSystem> channel, List<bool> componentWriters)> channels = new();
//            foreach (ComponentSystem? node in nodes)
//            {
//                for (int i = 0; i < node.ComponentDependencies.Count; i++)
//                {
//                    channels.TryAdd(node.ComponentDependencies[i], (new(4), new(4)));
//                    channels[node.ComponentDependencies[i]].channel.Add(node);
//                    channels[node.ComponentDependencies[i]].componentWriters.Add(node.ComponentsWritten[i]);
//                }
//            }

//            bool result = true;
//            foreach (KeyValuePair<ulong, (List<ComponentSystem> channel, List<bool> componentWriters)> pair in channels)
//            {
//                if (!VerifyStrictDependencyChain(pair.Value.channel, pair.Value.componentWriters, pair.Key))
//                    result = false;
//            }

//            return result;
//        }

//        public static bool VerifyStrictDependencyChain(this List<ComponentSystem> channel, List<bool> componentWriters, ulong compIndex)
//        {
//            List<ComponentSystem> readers = new();
//            List<ComponentSystem> writers = new();

//            for (int i = 0; i < channel.Count; i++)
//            {
//                if (componentWriters[i])
//                    writers.Add(channel[i]);
//                else
//                    readers.Add(channel[i]);
//            }

//            HashSet<ComponentSystem> set = new();
//            for (int i = 0; i < writers.Count; i++)
//            {
//                set.Clear();
//                writers[i].GetAllDependants(set, compIndex);
//                writers[i].GetAllDependencies(set, compIndex);

//                for (int j = i + 1; j < writers.Count; j++)
//                {
//                    if (!set.Contains(writers[j]))
//                        return false;
//                }
//            }
//            for (int i = 0; i < readers.Count; i++)
//            {
//                set.Clear();
//                readers[i].GetAllDependants(set, compIndex);
//                readers[i].GetAllDependencies(set, compIndex);

//                for (int j = 0; j < writers.Count; j++)
//                {
//                    if (!set.Contains(writers[j]))
//                        return false;
//                }
//            }

//            return true;
//        }

//        public static void GetAllDependants<T>(this ComponentSystem system, in T collection, ulong compIdxFilter = ulong.MaxValue)
//            where T : ICollection<ComponentSystem>
//        {
//            AddDependantsRecursive(system, collection, compIdxFilter);

//            static void AddDependantsRecursive(ComponentSystem system, T collection, ulong filter)
//            {
//                for (int i = 0; i < system.After.Count; i++)
//                {
//                    AddDependantsRecursive(system.After[i], collection, filter);
//                    if (!collection.Contains(system.After[i]) && (filter == ulong.MaxValue || system.After[i].ComponentDependencies.Contains(filter)))
//                        collection.Add(system.After[i]);
//                }
//            }
//        }

//        public static void GetAllDependencies<T>(this ComponentSystem system, in T collection, ulong compIdxFilter = ulong.MaxValue)
//            where T : ICollection<ComponentSystem>
//        {
//            AddDependenciesRecursive(system, collection, compIdxFilter);

//            static void AddDependenciesRecursive(ComponentSystem system, T collection, ulong filter)
//            {
//                for (int i = 0; i < system.Before.Count; i++)
//                {
//                    AddDependenciesRecursive(system.Before[i], collection, filter);
//                    if (!collection.Contains(system.Before[i]) && (filter == ulong.MaxValue || system.Before[i].ComponentDependencies.Contains(filter)))
//                        collection.Add(system.Before[i]);
//                }
//            }
//        }
//    }
//}
