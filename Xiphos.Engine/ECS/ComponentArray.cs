namespace Xiphos.ECS
{
    internal interface IComponentArray : IDisposable
    {
        public void AddComponent();

        public void ResetComponent(int index);

        public void RemoveComponent(int index);

        public void CopyComponent(int srcIdx, in IComponentArray destArray, int destIdx);

        public T GetComponent<T>(int index)
            where T : new();

        public bool TryGetComponent<T>(int index, out T? component)
            where T : new();

        public void SetComponent<T>(int index, in T component)
            where T : new();

        public bool TrySetComponent<T>(int index, in T component)
            where T : new();

        public void ClearComponents();
    }

    internal readonly struct ComponentArray<T> : IComponentArray
        where T : new()
    {
        private readonly List<T> components;

        public ComponentArray(int initialSize)
            => this.components = new(initialSize);

        public void AddComponent()
            => this.components.Add(new T());

        public void ResetComponent(int index)
            => this.components[index] = new T();

        public void RemoveComponent(int index)
            => this.components.RemoveAt(index);

        public void CopyComponent(int srcIdx, in IComponentArray destArray, int destIdx)
        {
            if (destArray is ComponentArray<T> castedArray)
                castedArray[destIdx] = this[srcIdx];
            else
                throw new Exception("Attempt to copy component between incompatible component arrays");
        }

        public void CopyComponent(int srcIdx, in ComponentArray<T> destArray, int destIdx)
            => destArray[destIdx] = this[srcIdx];

        public U GetComponent<U>(int index)
            where U : new()
        {
            if (this is ComponentArray<U> castArray)
                return castArray[index];
            else
                throw new ArgumentException("Attempted to get incorrect type from component array!");
        }

        public T GetComponent(int index)
            => this.components[index];

        public bool TryGetComponent<U>(int index, out U? component)
            where U : new()
        {
            if (this is ComponentArray<U> castArray)
            {
                component = castArray[index];
                return true;
            }
            else
            {
                component = default;
                return false;
            }
        }

        public void SetComponent<U>(int index, in U component)
            where U : new()
        {
            if (this is ComponentArray<U> castArray)
                castArray[index] = component;
            else
                throw new ArgumentException("Attempted to set incorrect type to component array!");
        }

        public void SetComponent(int index, in T component)
            => this.components[index] = component;

        public bool TrySetComponent<U>(int index, in U component)
            where U : new()
        {
            if (this is ComponentArray<U> castArray)
            {
                castArray[index] = component;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ClearComponents()
            => this.components.Clear();

        public T this[int index]
        {
            get => this.components[index];

            set => this.components[index] = value;
        }

        public void Dispose()
        {
            this.components.Clear();
            ThreadSafePool<XiphosStatic>.Add(this.components);
        }
    }
}
