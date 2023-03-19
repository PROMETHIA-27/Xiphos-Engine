//namespace Xiphos.ECS2
//{
//    public interface ISystemExecution { }

//    public interface ISystemExecution<T> : ISystemExecution
//    {
//        public T Execute();
//    }

//    public interface ISystemExecution<T, U> : ISystemExecution
//    {
//        public T Execute(U input);
//    }

//    public struct System<T, R>
//        where T : ISystemExecution<R>
//    {
//        internal T Execution { get; init; }

//        public BufferedConcurrentQueue<R> Output { get; init; }

//        public System(T execution)
//        {
//            this.Execution = execution;
//            this.Output = new(4);
//        }

//        internal void TryExecute()
//        {
//            SpinWait wait = default;
//            var execResult = this.Execution.Execute();
//            while (!(this.Output?.TryEnqueue(execResult) ?? false))
//                wait.SpinOnce();
//        }

//        public void LoopExecute()
//        {
//            while (true)
//            {
//                this.TryExecute();
//            }
//        }
//    }

//    public struct System<T, R, U>
//        where T : ISystemExecution<R, U>
//    {
//        internal T Execution { get; init; }

//        internal BufferedConcurrentQueue<R> Output { get; set; }

//        internal LinkOutput<U>? Input { get; set; }

//        public System(T execution)
//        {
//            this.Execution = execution;
//            this.Output = new(4);
//            this.Input = null;
//        }

//        public void SetInput(LinkOutput<U> input)
//            => this.Input = input;

//        internal void TryExecute()
//        {
//            U input = (this.Input ?? throw new Exception("Attempted to execute without input set!")).GetNextOutput();

//            SpinWait wait = default;
//            var execResult = this.Execution.Execute(input);
//            while (!(this.Output?.TryEnqueue(execResult) ?? false))
//                wait.SpinOnce();
//        }

//        public void LoopExecute()
//        {
//            while (true)
//            {
//                this.TryExecute();
//            }
//        }
//    }

//    public interface ISystemLinkAssembler<out T, in U1>
//    {
//        public T Assemble(U1 arg1);
//    }

//    public interface ISystemLinkAssembler<out T, in U1, in U2>
//    {
//        public T Assemble(U1 arg1, U2 arg2);
//    }

//    interface ISystemLinkOut<T>
//    {
//        public BufferedConcurrentQueue<T>?[] OutputQueues { get; init; }

//        public void WaitForNextRound();

//        public ulong AtomicReadRounds();

//        public ulong AtomicExchangeRounds(ulong exchangeWith);

//        public ulong AtomicCompExchangeRounds(ulong exchangeWith, ulong ifEquals);

//        public ulong AtomicIncrementRounds();
//    }

//    internal abstract class SystemLinkImplBase<T, R> : ISystemLinkOut<R>
//    {
//        protected readonly T assembler;
//        protected ulong rounds;
//        protected int waitedOn;
//        protected int lastOutputQueueIdx;

//        public BufferedConcurrentQueue<R>?[] OutputQueues { get; init; }
//        public LinkOutput<R> GetOutput()
//        {
//            var idx = Interlocked.Increment(ref this.lastOutputQueueIdx);

//            if (idx > this.OutputQueues.Length)
//                throw new Exception("Exceeded system link output capacity!");

//            LinkOutput<R> output = new() { link = this, queue = new(4) };

//            Interlocked.Exchange(ref this.OutputQueues[idx], output.queue);

//            return output;
//        }

//        public SystemLinkImplBase(T assembler)
//        {
//            this.assembler = assembler;
//            this.rounds = 0;
//            this.waitedOn = 0;
//            this.lastOutputQueueIdx = -1;
//            this.OutputQueues = new BufferedConcurrentQueue<R>?[16];
//        }

//        public ulong AtomicReadRounds()
//            => Interlocked.Read(ref this.rounds);

//        public ulong AtomicExchangeRounds(ulong exchangeWith)
//            => Interlocked.Exchange(ref this.rounds, exchangeWith);

//        public ulong AtomicCompExchangeRounds(ulong exchangeWith, ulong ifEquals)
//            => Interlocked.CompareExchange(ref this.rounds, exchangeWith, ifEquals);

//        public ulong AtomicIncrementRounds()
//            => Interlocked.Increment(ref this.rounds);

//        public abstract void WaitForNextRound();
//    }

//    internal class SystemLinkImpl<T, R, U1> : SystemLinkImplBase<T, R>
//        where T : ISystemLinkAssembler<R, U1>
//    {
//        internal BufferedConcurrentQueue<U1>? InputQueue { get; set; }

//        public SystemLinkImpl(T assembler) : base(assembler) 
//            => this.InputQueue = null;

//        /// <summary>
//        /// IS NOT THREAD-SAFE
//        /// </summary>
//        /// <param name="pipe"></param>
//        public void AddInput(BufferedConcurrentQueue<U1> pipe)
//            => this.InputQueue = pipe;

//        public override void WaitForNextRound()
//        {
//            if (Interlocked.CompareExchange(ref this.waitedOn, 1, 0) == 0)
//            {
//                SpinWait wait = default;

//                while (this.InputQueue?.Count == 0)
//                    wait.SpinOnce();

//                if (!this.InputQueue!.TryDequeue(out U1 arg1))
//                    throw new Exception("System link error! Input queues are being modified unsafely");

//                try
//                {
//                    R assembled = this.assembler.Assemble(arg1);

//                    var length = Volatile.Read(ref this.lastOutputQueueIdx);
//                    for (int i = 0; i < length + 1; i++)
//                        while (!(this.OutputQueues[i]?.TryEnqueue(assembled) ?? false))
//                            wait.SpinOnce();

//                    Interlocked.Increment(ref this.rounds);
//                }
//                catch
//                {
//                    // Logging, etc.
//                }

//                Interlocked.Exchange(ref this.waitedOn, 0);
//            }
//        }
//    }

//    internal class SystemLinkImpl<T, R, U1, U2> : SystemLinkImplBase<T, R>
//        where T : ISystemLinkAssembler<R, U1, U2>
//    {
//        internal BufferedConcurrentQueue<U1>? InputQueue1 { get; set; }
//        internal BufferedConcurrentQueue<U2>? InputQueue2 { get; set; }

//        public SystemLinkImpl(T assembler) : base(assembler)
//        {
//            this.InputQueue1 = null;
//            this.InputQueue2 = null;
//        }

//        /// <summary>
//        /// IS NOT THREAD-SAFE
//        /// </summary>
//        /// <param name="pipe"></param>
//        public void AddInput(BufferedConcurrentQueue<U1> pipe)
//            => this.InputQueue1 = pipe;

//        /// <summary>
//        /// IS NOT THREAD-SAFE
//        /// </summary>
//        /// <param name="pipe"></param>
//        public void AddInput(BufferedConcurrentQueue<U2> pipe)
//            => this.InputQueue2 = pipe;

//        public override void WaitForNextRound()
//        {
//            if (Interlocked.CompareExchange(ref this.waitedOn, 1, 0) == 0)
//            {
//                SpinWait wait = default;

//                while ((this.InputQueue1?.Count == 0) || (this.InputQueue2?.Count == 0))
//                    wait.SpinOnce();

//                if (!this.InputQueue1!.TryDequeue(out U1 arg1) || !this.InputQueue2!.TryDequeue(out U2 arg2))
//                    throw new Exception("System link error! Input queues are being modified unsafely");

//                try
//                {
//                    R assembled = this.assembler.Assemble(arg1, arg2);

//                    var length = Volatile.Read(ref this.lastOutputQueueIdx);
//                    for (int i = 0; i < length + 1; i++)
//                        while (!(this.OutputQueues[i]?.TryEnqueue(assembled) ?? false))
//                            wait.SpinOnce();

//                    Interlocked.Increment(ref this.rounds);
//                }
//                catch
//                {
//                    // Logging, etc.
//                }

//                Interlocked.Exchange(ref this.waitedOn, 0);
//            }
//        }
//    }

//    public struct SystemLink<T, R, U1>
//        where T : ISystemLinkAssembler<R, U1>
//    {
//        internal SystemLinkImpl<T, R, U1> impl;

//        public LinkOutput<R> GetOutput()
//            => this.impl.GetOutput();

//        public SystemLink(T assembler)
//            => this.impl = new(assembler);

//        /// <summary>
//        /// IS NOT THREAD-SAFE
//        /// </summary>
//        /// <param name="queue"></param>
//        public void AddInput(BufferedConcurrentQueue<U1> pipe)
//            => this.impl.AddInput(pipe);
//    }

//    public struct SystemLink<T, R, U1, U2>
//        where T : ISystemLinkAssembler<R, U1, U2>
//    {
//        internal SystemLinkImpl<T, R, U1, U2> impl;

//        public LinkOutput<R> GetOutput() 
//            => this.impl.GetOutput();

//        public SystemLink(T assembler) 
//            => this.impl = new(assembler);

//        /// <summary>
//        /// IS NOT THREAD-SAFE
//        /// </summary>
//        /// <param name="queue"></param>
//        public void AddInput(BufferedConcurrentQueue<U1> pipe)
//            => this.impl.AddInput(pipe);

//        /// <summary>
//        /// IS NOT THREAD-SAFE
//        /// </summary>
//        /// <param name="queue"></param>
//        //public void AddInput(BufferedConcurrentQueue<U2> pipe)
//            => this.impl.AddInput(pipe);
//    }

//    public struct LinkOutput<T>
//    {
//        internal ISystemLinkOut<T> link;
//        ulong lastReadRound;
//        internal BufferedConcurrentQueue<T> queue;

//        /// <summary>
//        /// Only one thread should call this at a time per instance
//        /// </summary>
//        /// <returns></returns>
//        internal T GetNextOutput()
//        {
//            SpinWait wait = default;

//            this.link.WaitForNextRound();

//            ulong pushedRounds = this.link.AtomicReadRounds();
//            if (pushedRounds <= this.lastReadRound)
//            {
//                while ((pushedRounds = this.link.AtomicReadRounds()) <= this.lastReadRound)
//                    wait.SpinOnce();
//            }

//            T result;
//            while (!this.queue.TryDequeue(out result!))
//                wait.SpinOnce();
//            this.lastReadRound = pushedRounds;
//            return result;
//        }
//    }

//    public class LinkExtractor<T>
//    {
//        internal readonly LinkOutput<T> output;

//        public LinkExtractor(LinkOutput<T> output)
//            => this.output = output;

//        /// <summary>
//        /// Only call this from one thread at a time! It is not thread safe.
//        /// Blocks and waits for the next pushed result from the link.
//        /// </summary>
//        /// <returns></returns>
//        public T ExtractOutput() 
//            => this.output.GetNextOutput();
//    }



//    public struct PassthroughAssembler<T> : ISystemLinkAssembler<T, T>
//    {
//        public T Assemble(T arg1) => arg1;

//        public SystemLink<PassthroughAssembler<T>, T, T> ToLink()
//            => new(this);
//    }


//    public static class SystemExtensions
//    {
//        public static System<T, R> ToSystem<T, R>(this T execution)
//            where T : ISystemExecution<R>
//            => new(execution);

//        public static System<T, R, U1> ToSystem<T, R, U1>(this T execution)
//            where T : ISystemExecution<R, U1>
//            => new(execution);

//        public static SystemLink<T, R, U1> ToLink<T, R, U1>(this T assembler)
//            where T : ISystemLinkAssembler<R, U1>
//            => new(assembler);

//        public static SystemLink<T, R, U1, U2> ToLink<T, R, U1, U2>(this T assembler)
//            where T : ISystemLinkAssembler<R, U1, U2>
//            => new(assembler);
//    }
//}
