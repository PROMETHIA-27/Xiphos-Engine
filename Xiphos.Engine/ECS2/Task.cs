//namespace Xiphos.ECS2
//{
//    public unsafe struct Task
//    {
//        internal Task(int id, Action work, int parentId, int workItems, int currentWorkItems, int priority, int dependency)
//            => (this.id, this.work, this.parentId, this.workItems, this.currentWorkItems, this.priority, this.dependency) =
//                (id, work, parentId, workItems, currentWorkItems, priority, dependency);

//        internal int id;
//        internal Action work;
//        internal int parentId;
//        internal int workItems;
//        internal int currentWorkItems;
//        internal int priority;
//        internal int dependency;

//        public void SetDependency(ref Task task)
//            => this.dependency = task.id;

//        public void SetParent(ref Task task)
//        {
//            this.parentId = task.id;
//            task.workItems++;
//        }
//    }

//    public struct TaskGraph : IDisposable // Lock before/after execution
//    {
//        internal Task[] _tasks;
//        internal int[] _rootTasks;
//        internal int _count;
//        internal int _rootTaskCount;
//        internal int _workerCount;
//        internal ConcurrentQueue<int> queued;
//        internal bool[] workerShouldExit;
//        internal int tasksToComplete;
//        internal ManualResetEventSlim resetEvent;
//        internal CountdownEvent tasksComplete;

//        public TaskGraph(int maxTaskCount, int workerCount)
//        {
//            this._tasks = new Task[maxTaskCount];
//            this._rootTasks = new int[maxTaskCount];
//            this._count = 0;
//            this._rootTaskCount = 0;
//            this._workerCount = workerCount;
//            this.queued = new();
//            this.workerShouldExit = new bool[workerCount];
//            this.tasksToComplete = 0;
//            this.resetEvent = new();
//            this.tasksComplete = new(0);

//            for (int i = 0; i < maxTaskCount; i++)
//            {
//                this._tasks[i] = new()
//                {
//                    id = -1,
//                    work = Empty,
//                    parentId = -1,
//                    workItems = -1,
//                    currentWorkItems = -1,
//                    priority = -1,
//                    dependency = -1,
//                };
//                this._rootTasks[i] = -1;
//            }

//            Task[] tasks = this._tasks;
//            bool[] workerExitArr = this.workerShouldExit;
//            ConcurrentQueue<int> queue = this.queued;
//            ManualResetEventSlim resetEvent = this.resetEvent;
//            CountdownEvent tasksComplete = this.tasksComplete;
//            for (int i = 0; i < this._workerCount; i++)
//            {
//                int workerIdx = i;
//                ThreadPool.QueueUserWorkItem(x =>
//                {
//                    while (!Volatile.Read(ref workerExitArr[workerIdx]))
//                    {
//                        if (!queue.TryDequeue(out int index))
//                        {
//                            Thread.Yield();
//                        }
//                        else
//                        {
//                            ref Task task = ref tasks[index];
//                            task.work();

//                            if (Interlocked.Decrement(ref task.currentWorkItems) == 0)
//                            {
//                                QueueDependents(tasks, queue, ref task);
//                                Interlocked.Exchange(ref task.currentWorkItems, task.workItems);
//                                tasksComplete.Signal();
//                            }

//                            if (task.parentId != -1)
//                            {
//                                ref Task parent = ref tasks[task.parentId];
//                                if (Interlocked.Decrement(ref parent.currentWorkItems) == 0)
//                                {
//                                    QueueDependents(tasks, queue, ref parent);
//                                    Interlocked.Exchange(ref parent.currentWorkItems, parent.workItems);
//                                    tasksComplete.Signal();
//                                }
//                            }
//                        }
//                    }
//                });
//            }

//            static void QueueDependents(Task[] tasks, ConcurrentQueue<int> queue, ref Task task)
//            {
//                for (int i = 0; i < tasks.Length; i++)
//                {
//                    ref Task potentialTask = ref tasks[i];
//                    if (potentialTask.dependency == task.id)
//                        queue.Enqueue(potentialTask.id);
//                }
//            }
//        }

//        public ref Task AddTask(Action work, int priority = 0)
//        {
//            this._tasks[this._count] = new()
//            {
//                id = this._count,
//                work = work,
//                parentId = -1,
//                workItems = 1,
//                currentWorkItems = 1,
//                priority = priority,
//                dependency = -1
//            };
//            this._rootTasks[this._rootTaskCount] = this._count;
//            this._count++;
//            this._rootTaskCount++;
//            return ref this._tasks[this._count - 1];
//        }

//        public void RemoveTask(in Task task) => this._tasks[task.id] = new()
//        {
//            id = -1,
//            work = Empty,
//            parentId = -1,
//            workItems = -1,
//            currentWorkItems = -1,
//            priority = -1,
//            dependency = -1,
//        };

//        public ref Task FindTaskByAction(Action work, ref Task @default)
//        {
//            for (int i = 0; i < this._count; i++)
//            {
//                ref Task task = ref this._tasks[i];
//                if (task.work == work)
//                    return ref this._tasks[i];
//            }
//            return ref @default;
//        }

//        public void Execute()
//        {
//            this.CalculateRootTasks();

//            this.tasksComplete.Reset(this._count);

//            Interlocked.Exchange(ref this.tasksToComplete, this._count);
//            for (int i = 0; i < this._rootTaskCount; i++)
//                this.queued.Enqueue(this._rootTasks[i]);

//            this.tasksComplete.Wait();
//        }

//        private void CalculateRootTasks()
//        {
//            this._rootTaskCount = 0;
//            for (int i = 0; i < this._count; i++)
//            {
//                if (this._tasks[i].dependency == -1)
//                    this._rootTasks[this._rootTaskCount++] = i;
//            }
//        }

//        public void Dispose()
//        {
//            for (int i = 0; i < this._workerCount; i++)
//                Volatile.Write(ref this.workerShouldExit[i], true);

//            this.resetEvent.Dispose();
//            this.tasksComplete.Dispose();
//        }

//        private static void Empty() { }
//    }
//}
