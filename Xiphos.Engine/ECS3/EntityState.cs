using System.Runtime.CompilerServices;

using CompFunctions = System.Collections.Generic.Dictionary<Xiphos.ECS3.EntityState.ComponentFunctions, System.IntPtr>;
using EventResponses = System.Collections.Generic.Dictionary<ulong, System.IntPtr>;

namespace Xiphos.ECS3;

public readonly unsafe struct EntityState : IDisposable
{
    private readonly IntPtr _data;
    private readonly Dictionary<ulong, ComponentInfo> _componentInfo;
    private readonly DynamicBitflags _signature;
    private readonly DynamicBitflags _eventSignature;
    private readonly ulong _entityCount;

    #region OffsetHelpers
    private int CountSize => sizeof(ulong);
    private int FirstFreeMapIndexSize => sizeof(ulong);
    private int MapSize => sizeof(MapSlot) * (int)this._entityCount;
    private int RevMapSize => sizeof(uint) * (int)this._entityCount;
    private int FirstFreeMapIndexOffset => this.CountSize;
    private int MapOffset => this.FirstFreeMapIndexOffset + this.FirstFreeMapIndexSize;
    private int RevMapOffset => this.MapOffset + this.MapSize;
    private int FilterValuesOffset => this.RevMapOffset + this.RevMapSize;
    #endregion

    public ref readonly ulong Count
        => ref Unsafe.AsRef<ulong>((void*)this._data);

    private ref ulong MutCount
        => ref Unsafe.AsRef<ulong>((void*)this._data);

    private ref ulong FirstFreeMapIndex
        => ref Unsafe.AsRef<ulong>((void*)(this._data + this.FirstFreeMapIndexOffset));

    private MapSlot* Map
        => (MapSlot*)(this._data + this.MapOffset);

    private uint* RevMap
        => (uint*)(this._data + this.RevMapOffset);

    public EntityState(EntityStateDescription desc)
    {
        int countSize = sizeof(ulong);
        int firstFreeMapIndexSize = sizeof(ulong);
        int mapSize = sizeof(MapSlot) * (int)desc._entityCount;
        int revMapSize = sizeof(uint) * (int)desc._entityCount;
        int filterValuesSize = (int)desc._filterValuesSize;

        int firstFreeMapIndexOffset = countSize;
        int mapOffset = firstFreeMapIndexOffset + firstFreeMapIndexSize;
        int revMapOffset = mapOffset + mapSize;
        int filterValuesOffset = revMapOffset + revMapSize;

        this._data = Marshal.AllocHGlobal(countSize + firstFreeMapIndexSize + mapSize + revMapSize + filterValuesSize);
        this._entityCount = desc._entityCount;

        // Initialize properties
        *(ulong*)this._data = 0; // Count
        *(ulong*)(this._data + countSize) = 0; // FirstFreeMapIndex

        // Initialize map
        MapSlot* mapPtr = (MapSlot*)(this._data + mapOffset);
        for (uint i = 0; i < desc._entityCount; i++)
            mapPtr[i] = new() { Index = i + 1, Version = 0 };

        // Initialize revMap
        uint* revMapPtr = (uint*)(this._data + revMapOffset);
        for (uint i = 0; i < desc._entityCount; i++)
            revMapPtr[i] = i;

        // Initialize componentInfo
        this._componentInfo = new(desc._typeInfo.Count);
        IntPtr filterValuePtr = this._data + filterValuesOffset;
        foreach ((ulong index, ComponentInfo info) in desc._typeInfo)
        {
            IntPtr bufferPtr = Marshal.AllocHGlobal((int)(info.ComponentSize * desc._entityCount));

            // If this component has a filtervalue, copy that value into filtervalues, grab current pointer and increment for next filtervalue
            IntPtr filterPtr = IntPtr.Zero;
            if (info.FilterValue != IntPtr.Zero)
            {
                Heap.CopyBoxValue(desc._filterValues[index], (void*)filterValuePtr);
                filterPtr = filterValuePtr;
                filterValuePtr += (int)info.FilterValue;
            }

            this._componentInfo.Add(index, info with { BufferPtr = bufferPtr, FilterValue = filterPtr });
        }

        // Initialize component signature
        ulong highest = 0UL;
        foreach (ulong index in desc._typeInfo.Keys)
            if (index > highest)
                highest = index;

        ulong flagsLength = (ulong)Math.Ceiling((double)(highest + 1) / 8);
        DynamicBitflags newSignature = new(flagsLength);
        foreach ((ulong index, _) in desc._typeInfo)
            newSignature[index] = 1;

        this._signature = newSignature;

        // Initialize event signature
        highest = 0UL;
        foreach ((ulong index, ComponentInfo info) in this._componentInfo)
            foreach ((ulong eventIdx, IntPtr _) in info.Responses)
                if (eventIdx > highest)
                    highest = eventIdx;

        flagsLength = (ulong)Math.Ceiling((double)(highest + 1) / 8);
        DynamicBitflags newEventSignature = new(flagsLength);
        foreach ((ulong index, ComponentInfo info) in this._componentInfo)
            foreach ((ulong eventIdx, IntPtr _) in info.Responses)
                newEventSignature[eventIdx] = 1;

        this._eventSignature = newEventSignature;
    }

    // TODO: Should buffers be publicly accessible? And should I switch to span instead of jank view?
    internal Either<BufferError, View<T>> GetBuffer<T>()
        where T : unmanaged, IComponent
        => this._componentInfo.TryGetValue(ComponentIndex<T>.Index, out ComponentInfo compInfo) switch
        {
            true => new(new View<T>((void*)compInfo.BufferPtr, 0, this.Count)),
            false => new(BufferError.TypeNotPresent),
        };

    internal Either<BufferError, ReadOnlyView<T>> GetReadOnlyBuffer<T>()
        where T : unmanaged, IComponent
        => this.GetBuffer<T>().Map(v => v.AsReadOnly());

    public Either<CreationError, EntityIndex> CreateEntity()
    {
        // MaxValue is "null" for mapIdx
        if (this.Count == this._entityCount || this.FirstFreeMapIndex == ulong.MaxValue)
            return new(CreationError.CapacityFull);

        Span<MapSlot> map = this.GetMap();

        // Set up a map slot to point at new entity in packed arrays
        ref MapSlot firstFreeMapSlot = ref map[(int)this.FirstFreeMapIndex];
        uint nextFree = firstFreeMapSlot.Index;
        firstFreeMapSlot = new() { Index = (uint)this.Count, Version = firstFreeMapSlot.Version + 1 };
        this.MutCount++;

        // Set up a reverse map slot to point at the new map slot
        Span<uint> revMap = this.GetRevMap();
        revMap[(int)firstFreeMapSlot.Index] = (uint)this.FirstFreeMapIndex;

        // For each component type, find the slot for it, and either initialize it with its function or zero it
        foreach ((ulong cIdx, ComponentInfo compInfo) in this._componentInfo)
        {
            void* compPtr = (void*)(compInfo.BufferPtr + ((int)firstFreeMapSlot.Index * (int)compInfo.ComponentSize));
            void* init = (void*)compInfo.Functions[ComponentFunctions.Initialize];
            if (init != null)
                ((delegate*<void*, void>)init)(compPtr);
            else
                Unsafe.InitBlock(compPtr, 0, compInfo.ComponentSize);
        }

        // Construct entityindex to return and update first free map index
        EntityIndex index = new((uint)this.FirstFreeMapIndex, firstFreeMapSlot.Version);
        this.FirstFreeMapIndex = nextFree;
        return new(index);
    }

    // TODO: Should I just change all these integer types to int? It feels like there's too much casting
    public Maybe<DestructionError> DestroyEntity(EntityIndex index)
    {
        Span<MapSlot> map = this.GetMap();
        Span<uint> revMap = this.GetRevMap();

        // Grab map slot entityindex points to and increment its version
        ref MapSlot delEntitySlot = ref map[(int)index.Index];
        if (index.Version != delEntitySlot.Version)
            return new(DestructionError.OutdatedIndex);
        delEntitySlot = delEntitySlot with { Version = delEntitySlot.Version + 1 };

        // For each component type, call cleanup if necessary and copy last component value onto delcomponent
        foreach (ComponentInfo compInfo in this._componentInfo.Values)
        {
            byte* buffer = (byte*)compInfo.BufferPtr;

            byte* delComponentPtr = buffer + (delEntitySlot.Index * compInfo.ComponentSize);

            void* cleanup = (void*)compInfo.Functions[ComponentFunctions.Cleanup];
            if (cleanup != null)
                ((delegate*<void*, void>)cleanup)(delComponentPtr);

            byte* lastComponentPtr = buffer + ((this.Count - 1) * compInfo.ComponentSize);

            Unsafe.CopyBlock(delComponentPtr, lastComponentPtr, compInfo.ComponentSize);
        }

        // Update last entity's mapslot/revmap to point to its new location
        revMap[(int)delEntitySlot.Index] = revMap[(int)this.Count - 1];
        ref MapSlot lastEntitySlot = ref map[(int)revMap[(int)this.Count - 1]];
        lastEntitySlot = lastEntitySlot with { Index = delEntitySlot.Index };

        // Update free slots linked list appropriately
        if (index.Index < this.FirstFreeMapIndex)
        {
            delEntitySlot = delEntitySlot with { Index = (uint)this.FirstFreeMapIndex };
            this.FirstFreeMapIndex = index.Index;
        }
        else
        {
            // Traverse the linked list to look for the slots surrounding delEntitySlot
            ulong lastSpot = ulong.MaxValue;
            ulong nextSpot = this.FirstFreeMapIndex;
            while (nextSpot < index.Index)
            {
                lastSpot = nextSpot;
                nextSpot = map[(int)nextSpot].Index;
            }

            delEntitySlot = delEntitySlot with { Index = (uint)nextSpot };
            ref MapSlot lastSpotSlot = ref map[(int)lastSpot];
            lastSpotSlot = lastSpotSlot with { Index = index.Index };
        }

        this.MutCount--;

        return new();
    }

    public Either<AccessError, T> GetComponent<T>(EntityIndex entity)
        where T : unmanaged, IComponent
    {
        ref MapSlot entitySlot = ref this.Map[entity.Index];

        if (entitySlot.Version != entity.Version)
            return new(AccessError.OutdatedIndex);

        Maybe<View<T>> bufferMaybe = this.GetBuffer<T>().RightMaybe;
        if (!bufferMaybe)
            return new(AccessError.TypeNotPresent);
        View<T> buffer = bufferMaybe.Some;

        T component = buffer[entitySlot.Index];
        return new(component);
    }

    public Maybe<U> GetFilterValue<T, U>()
        where T : unmanaged, IComponent, IFilter<U, T>
        where U : unmanaged
        => this._componentInfo.TryGetValue(ComponentIndex<T>.Index, out ComponentInfo info)
        && info.FilterValue != IntPtr.Zero
        ? new(*(U*)info.FilterValue)
        : new();

    public bool HandleEvent<T>(T e)
        where T : unmanaged, IComponentEvent
    {
        bool ran = false;
        foreach ((ulong index, ComponentInfo compInfo) in this._componentInfo)
            if (compInfo.Responses.TryGetValue(EventIndex<T>.Index, out IntPtr ptr))
            {
                ran = true;

                delegate*<ref T, void*, void> eventPtr = (delegate*<ref T, void*, void>)ptr;

                byte* instPtr = (byte*)compInfo.BufferPtr;

                for (ulong i = 0; i < this.Count; i++)
                {
                    eventPtr(ref e, instPtr);

                    instPtr += compInfo.ComponentSize;
                }
            }

        return ran;
    }

    public bool HandlesEvent<T>()
        where T : unmanaged, IComponentEvent
        => this._eventSignature.GetOrFalse(EventIndex<T>.Index) != 0;

    public bool HasComponent<T>()
        where T : unmanaged, IComponent
        => this._signature.GetOrFalse(ComponentIndex<T>.Index) != 0;

    public Maybe<AccessError> SendMessage<T>(T m, EntityIndex target)
        where T : unmanaged, IComponentMessage
    {
        foreach ((ulong index, ComponentInfo compInfo) in this._componentInfo)
            if (compInfo.Responses.TryGetValue(MessageIndex<T>.Index, out IntPtr ptr))
            {
                delegate*<ref T, void*, void> eventPtr = (delegate*<ref T, void*, void>)ptr;

                ref MapSlot mapIndex = ref this.Map[target.Index];

                if (mapIndex.Version != target.Version)
                    return new(AccessError.OutdatedIndex);

                void* compRef = (void*)(compInfo.BufferPtr + (int)(compInfo.ComponentSize * mapIndex.Index));

                eventPtr(ref m, compRef);
            }

        return new();
    }

    public Maybe<AccessError> SetComponent<T>(EntityIndex entity, T component)
        where T : unmanaged, IComponent
    {
        ref MapSlot mapIndex = ref this.Map[entity.Index];

        if (mapIndex.Version != entity.Version)
            return new(AccessError.OutdatedIndex);

        if (!this._componentInfo.TryGetValue(ComponentIndex<T>.Index, out ComponentInfo info))
            return new(AccessError.TypeNotPresent);
        if (info.Functions[ComponentFunctions.Filter] is IntPtr filterPtr && filterPtr != IntPtr.Zero && info.FilterValue != IntPtr.Zero)
            if (!((delegate*<void*, void*, bool>)filterPtr)((void*)info.FilterValue, &component))
                return new(AccessError.ComponentDoesNotMatchFilter);

        View<T> buffer = this.GetBuffer<T>().RightProjection;
        buffer[mapIndex.Index] = component;
        return new();
    }

    public void Dispose() //TODO: Dispose of memory
    {

    }

    internal IntPtr GetDataPtr()
        => this._data;

    internal ulong SizeOfCache<T>()
        where T : unmanaged
        => (ulong)sizeof(T) * this._entityCount;

    internal Span<MapSlot> GetMap()
        => new(this.Map, (int)this._entityCount);

    internal Span<uint> GetRevMap()
        => new(this.RevMap, (int)this._entityCount);

    internal readonly struct MapSlot
    {
        public uint Index { get; init; }
        public uint Version { get; init; }

        public bool IsAlive => this.Version % 2 == 1;
    }

    internal enum ComponentFunctions
    {
        Initialize,
        Cleanup,
        Filter,
    }

    public enum AccessError : byte
    {
        OutdatedIndex,
        TypeNotPresent,
        ComponentDoesNotMatchFilter,
    }

    public enum BufferError : byte
    {
        TypeNotPresent,
    }

    public enum CreationError : byte
    {
        CapacityFull,
        OutdatedIndex,
    }

    public enum DestructionError : byte
    {
        OutdatedIndex,
    }
}

public unsafe struct EntityStateDescription
{
    internal Dictionary<ulong, ComponentInfo> _typeInfo;
    internal Dictionary<ulong, HeapBox> _filterValues;
    internal ulong _entityCount;
    internal ulong _filterValuesSize;

    public EntityStateDescription(ulong entityCount)
    {
        this._typeInfo = new(4);
        this._filterValues = new();
        this._entityCount = entityCount;
        this._filterValuesSize = 0;
    }

    public EntityStateDescription AddComponent<T>()
        where T : unmanaged, IComponent
    {
        if (ComponentFunctions<T>.Functions[EntityState.ComponentFunctions.Filter] != IntPtr.Zero)
            throw new InvalidOperationException($"You must use the filtered overload of {nameof(AddComponent)} for components that implement {typeof(IFilter<,>).Name!}");
        this._typeInfo.Add(
            ComponentIndex<T>.Index,
            new() 
            {
                ComponentSize = (uint)sizeof(T),
                Functions = ComponentFunctions<T>.Functions,
                Responses = ComponentEventResponses<T>.responses,
                FilterValue = IntPtr.Zero,
            });

        return this;
    }

    public EntityStateDescription AddComponent<T, U>(U filterValue)
        where T : unmanaged, IFilter<U, T>, IComponent
        where U : unmanaged
    { // TODO: Handle duplicate additions better
        this._typeInfo.Add(
            ComponentIndex<T>.Index,
            new()
            {
                ComponentSize = (uint)sizeof(T),
                Functions = ComponentFunctions<T>.Functions,
                Responses = ComponentEventResponses<T>.responses,
                FilterValue = (IntPtr)sizeof(U),
            });
        _filterValuesSize += (uint)sizeof(U);
        _filterValues.Add(ComponentIndex<T>.Index, Heap.CreateBox(filterValue));

        return this;
    }
}

internal readonly struct ComponentInfo
{
    public IntPtr BufferPtr { get; init; }
    public uint ComponentSize { get; init; }
    public CompFunctions Functions { get; init; }
    public EventResponses Responses { get; init; }
    public IntPtr FilterValue { get; init; }
}

public readonly struct EntityIndex
{
    internal uint Index { get; init; }
    internal uint Version { get; init; }
    public EntityIndex(uint index, uint version)
        => (this.Index, this.Version) = (index, version);
}

public readonly struct QualifiedEntityIndex
{
    internal static Dictionary<IntPtr, EntityState> QualifiedEntityStates { get; } = new(128);

    internal uint Index { get; init; }
    internal uint Version { get; init; }
    internal IntPtr StateDataPtr { get; init; }

    internal QualifiedEntityIndex(uint index, uint version, EntityState state)
    {
        (this.Index, this.Version, this.StateDataPtr) = (index, version, state.GetDataPtr());
        if (!QualifiedEntityStates.ContainsKey(this.StateDataPtr))
            QualifiedEntityStates.Add(this.StateDataPtr, state);
    }

    public QualifiedEntityIndex(EntityIndex index, EntityState state)
    {
        (this.Index, this.Version, this.StateDataPtr) = (index.Index, index.Version, state.GetDataPtr());
        if (!QualifiedEntityStates.ContainsKey(this.StateDataPtr))
            QualifiedEntityStates.Add(this.StateDataPtr, state);
    }

    public EntityState GetState()
        => QualifiedEntityStates[this.StateDataPtr];

    public Either<EntityState.AccessError, T> GetComponent<T>()
        where T : unmanaged, IComponent
        => this.GetState().GetComponent<T>(new(this.Index, this.Version));
}