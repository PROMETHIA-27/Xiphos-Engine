//namespace Xiphos.ECS
//{
//    public struct ArchetypeBuilder
//    {
//        private readonly Archetype archetype;
//        private bool created;

//        internal ArchetypeBuilder(Archetype archetype)
//        {
//            this.archetype = archetype;
//            this.archetype.world._systemGraph.WaitForArchetype(true);
//            this.created = false;
//        }

//        public Archetype Create()
//        {
//            if (this.archetype.world._systemGraph.InProgress)
//                throw new Exception("Cannot create an archetype while its world's system graph is running!");
//            this.created = true;
//            this.archetype.world._archetypes.Add(this.archetype);
//            this.archetype.world._archetypesToSort.Enqueue(this.archetype);
//            this.archetype.world._systemGraph.WaitForArchetype(false);
//            return this.archetype;
//        }

//        public ArchetypeBuilder AddComponent<T>()
//            where T : new()
//        {
//            if (this.created)
//                throw new Exception("Archetype already finalized");

//            ulong typeIndex = TypeIndex<T>.Index;
//            ComponentArray<T> compArray = new(16);

//            this.archetype.componentArrays.Add(typeIndex, compArray);

//            return this;
//        }
//    }
//}
