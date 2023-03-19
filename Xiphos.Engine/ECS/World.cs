//namespace Xiphos.ECS;

//public class World : IDisposable
//{
//    private static int _nextWorldIndex = 0;

//    internal readonly List<Archetype> _archetypes = ThreadSafePool<List<Archetype>, XiphosStatic>.Take();
//    internal readonly Queue<Archetype> _archetypesToSort = ThreadSafePool<Queue<Archetype>, XiphosStatic>.Take();
//    internal readonly SystemGraph _systemGraph = new();
//    internal int Index { get; init; }

//    public World()
//    {
//        this.Index = _nextWorldIndex;
//        _nextWorldIndex++;
//    }

//    public ArchetypeBuilder GetArchetypeBuilder()
//    {
//        Archetype archetype = new Archetype(this, this._archetypes.Count);
//        return new(archetype);
//    }

//    public void AddSystemGraph(in ComponentSystem system)
//        => this._systemGraph.AddSystemGraph(system);

//    public Task? RunSystems()
//    {
//        this._systemGraph.OrderSystemExecution();
//        if (this._archetypesToSort.Count > 0)
//        {
//            for (int i = 0; i < this._archetypesToSort.Count; i++)
//            {
//                Archetype? archetype = this._archetypesToSort.Dequeue();
//                this._systemGraph.DistributeArchetype(archetype);
//            }
//        }

//        return this._systemGraph.Start();
//    }

//    public void Dispose()
//    {
//        this._systemGraph.Dispose();
//        foreach (Archetype? archetype in this._archetypes)
//            archetype.Dispose();
//        this._archetypes.Clear();
//        ThreadSafePool<XiphosStatic>.Add(this._archetypes);
//        this._archetypesToSort.Clear();
//        ThreadSafePool<XiphosStatic>.Add(this._archetypesToSort);
//    }
//}
