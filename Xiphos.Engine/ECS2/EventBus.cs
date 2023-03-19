//namespace Xiphos.ECS2
//{
//    internal interface IEventBus { }

//    internal interface IEventBus<T> : IEventBus
//        where T : IEvent, new()
//    { }

//    internal struct EventBus<T> : IEventBus<T>
//        where T : IEvent, new()
//    {
//        private readonly ConcurrentQueue<T> queue;

//        //public void Execute(ref World world)
//        //{
//        //    int[] subscribers = World.eventSubscriptions[TypeIndex<T>.Index];
//        //    Parallel.For(0, subscribers.Length, i =>
//        //    {
//        //        var entityTypeIndex = subscribers[i];
//        //        var cache = world.entityCaches[entityTypeIndex];
//        //        foreach (var entityIdx in cache)
//        //        {

//        //        }
//        //    });
//        //}
//    }
//}
