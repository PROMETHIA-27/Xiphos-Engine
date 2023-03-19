//namespace Xiphos.ECS
//{
//    public struct ComponentPack
//    {
//        internal List<Archetype> archetypes;
//        private int currentArchetype;
//        private int currentEntityInArchetype;

//        internal void SetComponentArrays(List<Archetype> archetypes)
//            => this.archetypes = archetypes;

//        /// <summary>
//        /// Increments the current entity being processed across all contained archetypes.
//        /// </summary>
//        /// <returns>False if incrementing rolled over to the very first entity.</returns>
//        internal bool IncrementEntity()
//        {
//            this.currentEntityInArchetype++;
//            if (this.archetypes[this.currentArchetype].Count <= this.currentEntityInArchetype)
//            {
//                this.currentArchetype++;
//                this.currentEntityInArchetype = 0;
//                if (this.archetypes.Count <= this.currentArchetype)
//                {
//                    this.currentArchetype = 0;
//                    return false;
//                }
//            }
//            return true;
//        }

//        public T GetComponent<T>()
//            where T : new()
//            => this.archetypes[this.currentArchetype].GetComponent<T>(this.currentEntityInArchetype);

//        public void SetComponent<T>(in T component)
//            where T : new()
//            => this.archetypes[this.currentArchetype].SetComponent(this.currentEntityInArchetype, component);
//    }
//}
