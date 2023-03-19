namespace Xiphos.ECS.Systems
{
    //public class ApplyVelocitySystem : ISystem, IDisposable
    //{
    //    public List<ISystem> Before { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<ISystem> After { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<bool> ComponentsWritten { get; } = ThreadSafePool<List<bool>, XiphosStatic>.Take();
    //    public List<ulong> ComponentDependencies { get; } = ThreadSafePool<List<ulong>, XiphosStatic>.Take();
    //    public int Rank { get; set; }

    //    public void Invoke(ComponentPack pack, Entity entity)
    //    {
    //    }

    //    public void Dispose()
    //    {
    //        this.Before.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.Before);
    //        this.After.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.After);
    //        this.ComponentsWritten.Clear();
    //        ThreadSafePool<List<bool>, XiphosStatic>.Add(this.ComponentsWritten);
    //        this.ComponentDependencies.Clear();
    //        ThreadSafePool<List<ulong>, XiphosStatic>.Add(this.ComponentDependencies);
    //    }
    //}

    //public class PhysicsSystem : ISystem, IDisposable
    //{
    //    public List<ISystem> Before { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<ISystem> After { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<bool> ComponentsWritten { get; } = ThreadSafePool<List<bool>, XiphosStatic>.Take();
    //    public List<ulong> ComponentDependencies { get; } = ThreadSafePool<List<ulong>, XiphosStatic>.Take();
    //    public int Rank { get; set; }

    //    public void Invoke(ComponentPack pack, Entity entity)
    //    {
    //    }

    //    public void Dispose()
    //    {
    //        this.Before.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.Before);
    //        this.After.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.After);
    //        this.ComponentsWritten.Clear();
    //        ThreadSafePool<List<bool>, XiphosStatic>.Add(this.ComponentsWritten);
    //        this.ComponentDependencies.Clear();
    //        ThreadSafePool<List<ulong>, XiphosStatic>.Add(this.ComponentDependencies);
    //    }
    //}

    //public class InputSystem : ISystem, IDisposable
    //{
    //    public List<ISystem> Before { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<ISystem> After { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<bool> ComponentsWritten { get; } = ThreadSafePool<List<bool>, XiphosStatic>.Take();
    //    public List<ulong> ComponentDependencies { get; } = ThreadSafePool<List<ulong>, XiphosStatic>.Take();
    //    public int Rank { get; set; }

    //    public void Invoke(ComponentPack pack, Entity entity)
    //    {
    //    }

    //    public void Dispose()
    //    {
    //        this.Before.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.Before);
    //        this.After.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.After);
    //        this.ComponentsWritten.Clear();
    //        ThreadSafePool<List<bool>, XiphosStatic>.Add(this.ComponentsWritten);
    //        this.ComponentDependencies.Clear();
    //        ThreadSafePool<List<ulong>, XiphosStatic>.Add(this.ComponentDependencies);
    //    }
    //}

    //public class CharacterControllerSystem : ISystem, IDisposable
    //{
    //    public List<ISystem> Before { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<ISystem> After { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<bool> ComponentsWritten { get; } = ThreadSafePool<List<bool>, XiphosStatic>.Take();
    //    public List<ulong> ComponentDependencies { get; } = ThreadSafePool<List<ulong>, XiphosStatic>.Take();
    //    public int Rank { get; set; }

    //    public void Invoke(ComponentPack pack, Entity entity)
    //    {
    //    }

    //    public void Dispose()
    //    {
    //        this.Before.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.Before);
    //        this.After.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.After);
    //        this.ComponentsWritten.Clear();
    //        ThreadSafePool<List<bool>, XiphosStatic>.Add(this.ComponentsWritten);
    //        this.ComponentDependencies.Clear();
    //        ThreadSafePool<List<ulong>, XiphosStatic>.Add(this.ComponentDependencies);
    //    }
    //}

    //public class CollisionSystem : ISystem, IDisposable
    //{
    //    public List<ISystem> Before { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<ISystem> After { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<bool> ComponentsWritten { get; } = ThreadSafePool<List<bool>, XiphosStatic>.Take();
    //    public List<ulong> ComponentDependencies { get; } = ThreadSafePool<List<ulong>, XiphosStatic>.Take();
    //    public int Rank { get; set; }

    //    public void Invoke(ComponentPack pack, Entity entity)
    //    {
    //    }

    //    public void Dispose()
    //    {
    //        this.Before.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.Before);
    //        this.After.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.After);
    //        this.ComponentsWritten.Clear();
    //        ThreadSafePool<List<bool>, XiphosStatic>.Add(this.ComponentsWritten);
    //        this.ComponentDependencies.Clear();
    //        ThreadSafePool<List<ulong>, XiphosStatic>.Add(this.ComponentDependencies);
    //    }
    //}

    //public class EquipmentUseSystem : ISystem, IDisposable
    //{
    //    public List<ISystem> Before { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<ISystem> After { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<bool> ComponentsWritten { get; } = ThreadSafePool<List<bool>, XiphosStatic>.Take();
    //    public List<ulong> ComponentDependencies { get; } = ThreadSafePool<List<ulong>, XiphosStatic>.Take();
    //    public int Rank { get; set; }

    //    public void Invoke(ComponentPack pack, Entity entity)
    //    {
    //    }

    //    public void Dispose()
    //    {
    //        this.Before.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.Before);
    //        this.After.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.After);
    //        this.ComponentsWritten.Clear();
    //        ThreadSafePool<List<bool>, XiphosStatic>.Add(this.ComponentsWritten);
    //        this.ComponentDependencies.Clear();
    //        ThreadSafePool<List<ulong>, XiphosStatic>.Add(this.ComponentDependencies);
    //    }
    //}

    //public class RenderingSystem : ISystem, IDisposable
    //{
    //    public List<ISystem> Before { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<ISystem> After { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<bool> ComponentsWritten { get; } = ThreadSafePool<List<bool>, XiphosStatic>.Take();
    //    public List<ulong> ComponentDependencies { get; } = ThreadSafePool<List<ulong>, XiphosStatic>.Take();
    //    public int Rank { get; set; }

    //    public void Invoke(ComponentPack pack, Entity entity)
    //    {
    //    }

    //    public void Dispose()
    //    {
    //        this.Before.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.Before);
    //        this.After.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.After);
    //        this.ComponentsWritten.Clear();
    //        ThreadSafePool<List<bool>, XiphosStatic>.Add(this.ComponentsWritten);
    //        this.ComponentDependencies.Clear();
    //        ThreadSafePool<List<ulong>, XiphosStatic>.Add(this.ComponentDependencies);
    //    }
    //}

    //public class InterfaceSystem : ISystem, IDisposable
    //{
    //    public List<ISystem> Before { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<ISystem> After { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<bool> ComponentsWritten { get; } = ThreadSafePool<List<bool>, XiphosStatic>.Take();
    //    public List<ulong> ComponentDependencies { get; } = ThreadSafePool<List<ulong>, XiphosStatic>.Take();
    //    public int Rank { get; set; }

    //    public void Invoke(ComponentPack pack, Entity entity)
    //    {
    //    }

    //    public void Dispose()
    //    {
    //        this.Before.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.Before);
    //        this.After.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.After);
    //        this.ComponentsWritten.Clear();
    //        ThreadSafePool<List<bool>, XiphosStatic>.Add(this.ComponentsWritten);
    //        this.ComponentDependencies.Clear();
    //        ThreadSafePool<List<ulong>, XiphosStatic>.Add(this.ComponentDependencies);
    //    }
    //}

    //public class IntelligenceSystem : ISystem, IDisposable
    //{
    //    public List<ISystem> Before { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<ISystem> After { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<bool> ComponentsWritten { get; } = ThreadSafePool<List<bool>, XiphosStatic>.Take();
    //    public List<ulong> ComponentDependencies { get; } = ThreadSafePool<List<ulong>, XiphosStatic>.Take();
    //    public int Rank { get; set; }

    //    public void Invoke(ComponentPack pack, Entity entity)
    //    {
    //    }

    //    public void Dispose()
    //    {
    //        this.Before.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.Before);
    //        this.After.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.After);
    //        this.ComponentsWritten.Clear();
    //        ThreadSafePool<List<bool>, XiphosStatic>.Add(this.ComponentsWritten);
    //        this.ComponentDependencies.Clear();
    //        ThreadSafePool<List<ulong>, XiphosStatic>.Add(this.ComponentDependencies);
    //    }
    //}

    //public class NumberedSystem : ISystem, IDisposable
    //{
    //    public List<ISystem> Before { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<ISystem> After { get; } = ThreadSafePool<List<ISystem>, XiphosStatic>.Take();
    //    public List<bool> ComponentsWritten { get; } = ThreadSafePool<List<bool>, XiphosStatic>.Take();
    //    public List<ulong> ComponentDependencies { get; } = ThreadSafePool<List<ulong>, XiphosStatic>.Take();
    //    public int Rank { get; set; }
    //    public int id;

    //    public NumberedSystem(int id = 0) 
    //        => this.id = id;

    //    public void Invoke(ComponentPack pack, Entity entity)
    //    {

    //    }

    //    public void Dispose()
    //    {
    //        this.Before.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.Before);
    //        this.After.Clear();
    //        ThreadSafePool<List<ISystem>, XiphosStatic>.Add(this.After);
    //        this.ComponentsWritten.Clear();
    //        ThreadSafePool<List<bool>, XiphosStatic>.Add(this.ComponentsWritten);
    //        this.ComponentDependencies.Clear();
    //        ThreadSafePool<List<ulong>, XiphosStatic>.Add(this.ComponentDependencies);
    //    }
    //}
}
