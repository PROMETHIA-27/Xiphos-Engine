//namespace Xiphos.ECS
//{
//    internal class SystemGraph : IDisposable
//    {
//        private readonly HashSet<ComponentSystem> systemSet = ThreadSafePool<HashSet<ComponentSystem>, XiphosStatic>.Take();
//        private readonly List<ComponentSystem> systems = ThreadSafePool<List<ComponentSystem>, XiphosStatic>.Take();
//        private readonly List<List<ComponentSystem>> rankedSystems = new();
//        private readonly ConcurrentDictionary<ComponentSystem, Task> systemTasks = new();
//        private long inProgress;
//        private long ordered;
//        private long orderingInProgress;
//        private long waitingForArchetype;

//        public bool InProgress => Interlocked.Read(ref this.inProgress) != 0;
//        public bool WaitingForArchetype => Interlocked.Read(ref this.waitingForArchetype) != 0;

//        public void WaitForArchetype(bool wait)
//            => Interlocked.Exchange(ref this.waitingForArchetype, wait ? 1 : 0);

//        /// <summary>
//        /// Ensures that system dependencies are properly ordered, and sets up the system graph to be able to execute.
//        /// Checks:
//        /// -If graph is already successfully ordered (early escape)
//        /// -If graph is running, but not ordered (early error)
//        /// -If the system graph contains no circular dependencies
//        /// -If all write-access systems have a deterministic order between them, and all read-only systems have a deterministic order to all write-access systems
//        /// </summary>
//        /// <returns>In a single-threaded context (this function is only called from one thread), returns true if the system is correctly ordered. 
//        /// In a multi-threaded context, returns true if the system is correctly ordered, or if this call ordered it. Returns false in the case that the graph is not orderable due to dependency errors, or if the graph is currently being ordered on another thread.</returns>
//        internal bool OrderSystemExecution()
//        {
//            if (Interlocked.Read(ref this.ordered) == 1)
//                return true;
//            if (Interlocked.Read(ref this.inProgress) == 1)
//                throw new Exception("System is running while unordered. This should not happen, please contact the XIPHOS engine author.");
//            if (Interlocked.CompareExchange(ref this.orderingInProgress, 1, 0) == 0)
//            {
//                if (!this.systems.IsNodeGraphAcyclic())
//                    return false;
//                if (!this.systems.VerifyStrictDependencyChains())
//                    return false;

//                for (int i = 0; i < this.rankedSystems.Count; i++)
//                {
//                    this.rankedSystems[i].Clear();
//                    ThreadSafePool<XiphosStatic>.Add(this.rankedSystems[i]);
//                }
//                this.rankedSystems.Clear();
//                for (int i = 0; i < this.systems.Count; i++)
//                {
//                    ComponentSystem? currSys = this.systems[i];
//                    while (this.rankedSystems.Count < currSys.Rank + 1)
//                        this.rankedSystems.Add(ThreadSafePool<List<ComponentSystem>, XiphosStatic>.Take());
//                    this.rankedSystems[currSys.Rank].Add(currSys);
//                }
//                Interlocked.Exchange(ref this.orderingInProgress, 0);
//                Interlocked.Exchange(ref this.ordered, 1);
//                return true;
//            }

//            return false;
//        }

//        /// <summary>
//        /// If the system graph is ordered correctly and not already executing, will start execution and return an task that will complete when all systems in the graph are complete.
//        /// This method is thread-safe.
//        /// </summary>
//        /// <returns>A task that completes when all systems in the graph are complete.</returns>
//        internal Task? Start()
//        {
//            if (Interlocked.Read(ref this.ordered) == 1 && !this.WaitingForArchetype && Interlocked.CompareExchange(ref this.inProgress, 1, 0) == 0)
//            {
//                this.systemTasks.Clear();

//                for (int i = 0; i < this.rankedSystems[0].Count; i++)
//                {
//                    ComponentSystem? targetSys = this.rankedSystems[0][i];
//                    if (!this.systemTasks.TryAdd(this.rankedSystems[0][i], new(() => targetSys.Invoke(new()))))
//                        throw new Exception("Strange threading error. This should not happen, please contact the XIPHOS engine author.");
//                }

//                List<Task> tasks = ThreadSafePool<List<Task>, XiphosStatic>.Take();
//                for (int i = 1; i < this.rankedSystems.Count; i++)
//                {
//                    List<ComponentSystem>? sysRank = this.rankedSystems[i];
//                    for (int j = 0; j < sysRank.Count; j++)
//                    {
//                        ComponentSystem? currSys = sysRank[j];
//                        tasks.Clear();
//                        for (int k = 0; k < currSys.Before.Count; k++)
//                            tasks.Add(this.systemTasks[currSys.Before[k]]);

//                        if (!this.systemTasks.TryAdd(currSys, Task.WhenAll(tasks).ContinueWith(t => currSys.Invoke(new()))))
//                            throw new Exception("Strange threading error. This should not happen, please contact the XIPHOS engine author.");
//                    }
//                }
//                tasks.Clear();
//                ThreadSafePool<XiphosStatic>.Add(tasks);

//                for (int i = 0; i < this.rankedSystems[0].Count; i++)
//                    this.systemTasks[this.rankedSystems[0][i]].Start();

//                return Task.WhenAll(this.systemTasks.Values).ContinueWith(t => Interlocked.Exchange(ref this.inProgress, 0));
//            }
//            return null;
//        }

//        /// <summary>
//        /// Collects all systems directly or indirectly connected to the given system via dependencies, and adds them all to the system graph.
//        /// </summary>
//        /// <typeparam name="T">The concrete type of the system.</typeparam>
//        /// <param name="system">The system to get connected systems of.</param>
//        public void AddSystemGraph<T>(in T system)
//            where T : ComponentSystem
//        {
//            Interlocked.Exchange(ref this.ordered, 0);
//            HashSet<ComponentSystem> systemGraph = ThreadSafePool<HashSet<ComponentSystem>, XiphosStatic>.Take();
//            system.GetConnectedSystems(systemGraph);
//            foreach (ComponentSystem? currSys in systemGraph)
//            {
//                if (this.systemSet.Add(currSys))
//                    this.systems.Add(currSys);
//            }

//            systemGraph.Clear();
//            ThreadSafePool<XiphosStatic>.Add(systemGraph);
//        }

//        /// <summary>
//        /// Distributes given archetype to every system's component pack
//        /// </summary>
//        /// <param name="archetype">Archetype to distribute to system component packs</param>
//        public void DistributeArchetype(in Archetype archetype)
//        {
//            for (int i = 0; i < this.systems.Count; i++)
//            {
//                ComponentSystem? currSys = this.systems[i];
//                bool addArchetype = true;
//                for (int j = 0; j < currSys.ComponentDependencies.Count; j++)
//                {
//                    ulong currComp = currSys.ComponentDependencies[j];
//                    if (!archetype.componentArrays.ContainsKey(currComp))
//                        addArchetype = false;
//                }
//                if (addArchetype)
//                    currSys.Pack.archetypes.Add(archetype);
//            }
//        }

//        public void Dispose()
//        {
//            this.systemSet.Clear();
//            ThreadSafePool<XiphosStatic>.Add(this.systemSet);
//            this.systems.Clear();
//            ThreadSafePool<XiphosStatic>.Add(this.systems);
//            for (int i = 0; i < this.rankedSystems.Count; i++)
//            {
//                this.rankedSystems[i].Clear();
//                ThreadSafePool<XiphosStatic>.Add(this.rankedSystems[i]);
//            }
//        }
//    }
//}
