namespace Xiphos.ECS2.System
{
    public interface ISystemExecution<TOutput>
    {
        public TOutput Execute();
    }

    public interface ISystemExecution<TOutput, TInput>
    {
        public TOutput Execute(TInput input);
    }

    public class System<TExecution, TOutput>
        where TExecution : ISystemExecution<TOutput>
    {
        private TExecution execution;

        private ref TExecution Execution => ref this.execution;

        private IInputHandle<TOutput>[] Outputs { get; init; }

        public System(TExecution execution, params IInputHandle<TOutput>[] outputs)
        {
            this.execution = execution;
            this.Outputs = outputs;
        }

        public async ValueTask TryExecute()
        {
            TOutput? exec = this.Execution.Execute();

            for (int i = 0; i < this.Outputs.Length; i++)
                await this.Outputs[i].PassInput(exec);
        }

        public async ValueTask LoopExecute()
        {
            while (true)
                await this.TryExecute();
        }
    }

    public class System<TExecution, TOutput, TInput>
        where TExecution : ISystemExecution<TOutput, TInput>
    {
        private TExecution execution;

        private ref TExecution Execution => ref this.execution;

        private Channel<TInput> InputChannel { get; init; }
        public ChannelWriter<TInput> Input { get; init; }
        private IInputHandle<TOutput>[] Outputs { get; init; }

        public System(TExecution execution, params IInputHandle<TOutput>[] outputs)
        {
            this.execution = execution;
            this.InputChannel = Channel.CreateBounded<TInput>(new BoundedChannelOptions(8) { SingleReader = true });
            this.Input = this.InputChannel.Writer;
            this.Outputs = outputs;
        }

        public async ValueTask TryExecute()
        {
            TInput? input = await this.InputChannel.Reader.ReadAsync();

            TOutput? exec = this.Execution.Execute(input);

            for (int i = 0; i < this.Outputs.Length; i++)
                await this.Outputs[i].PassInput(exec);
        }

        public async ValueTask LoopExecute()
        {
            while (true)
                await this.TryExecute();
        }
    }

    public interface ISystemAssembler<TOutput, TInput1>
    {
        public TOutput Assemble(TInput1 input1);
    }

    public interface ISystemAssembler<TOutput, TInput1, TInput2>
    {
        public TOutput Assemble(TInput1 input1, TInput2 input2);
    }

    internal interface ISystemLink<TInput>
    {
        public ValueTask PassInput(TInput input);
    }

    public class SystemLink<TAssembler, TOutput, TInput1> : ISystemLink<TInput1>
        where TAssembler : ISystemAssembler<TOutput, TInput1>
    {
        private TAssembler assembler;

        private ref TAssembler Assembler => ref this.assembler;
        public ChannelWriter<TOutput>[] Outputs { get; init; }
        public InputHandle Input1 { get; init; }

        public SystemLink(TAssembler assembler, params ChannelWriter<TOutput>[] outputs)
        {
            this.assembler = assembler;
            this.Outputs = outputs;
            this.Input1 = new(this);
        }

        public async ValueTask PassInput(TInput1 input)
        {
            TOutput? output = this.Assembler.Assemble(input);
            for (int i = 0; i < this.Outputs.Length; i++)
                await this.Outputs[i].WriteAsync(output);
        }

        public class InputHandle : IInputHandle<TInput1>
        {
            internal SystemLink<TAssembler, TOutput, TInput1> link;

            public InputHandle(SystemLink<TAssembler, TOutput, TInput1> link)
                => this.link = link;

            public async ValueTask PassInput(TInput1 input)
                => await this.link.PassInput(input);
        }
    }

    public class SystemLink<TAssembler, TOutput, TInput1, TInput2>
        where TAssembler : ISystemAssembler<TOutput, TInput1, TInput2>
    {
        private TAssembler assembler;

        private ref TAssembler Assembler => ref this.assembler;
        public ChannelWriter<TOutput>[] Outputs { get; init; }
        public InputHandle1 Input1 { get; init; }
        public InputHandle2 Input2 { get; init; }
        private Channel<TInput1> InputChannel1 { get; init; }
        private Channel<TInput2> InputChannel2 { get; init; }

        private long inputLock;

        public SystemLink(TAssembler assembler, params ChannelWriter<TOutput>[] outputs)
        {
            this.assembler = assembler;
            this.Outputs = outputs;
            this.Input1 = new(this);
            this.Input2 = new(this);
            this.InputChannel1 = Channel.CreateBounded<TInput1>(new BoundedChannelOptions(8) { SingleReader = true });
            this.InputChannel2 = Channel.CreateBounded<TInput2>(new BoundedChannelOptions(8) { SingleReader = true });
        }

        public async ValueTask PassInput(TInput1 input)
        {
            SpinWait wait = default;
            while (Interlocked.CompareExchange(ref this.inputLock, 1, 0) == 1)
                wait.SpinOnce();

            if (this.InputChannel2.Reader.Count > 0)
                await this.CombineInput(input, await this.InputChannel2.Reader.ReadAsync());
            else
                await this.InputChannel1.Writer.WriteAsync(input);

            Interlocked.Exchange(ref this.inputLock, 0);
        }

        public async ValueTask PassInput(TInput2 input)
        {
            SpinWait wait = default;
            while (Interlocked.CompareExchange(ref this.inputLock, 1, 0) == 1)
                wait.SpinOnce();

            if (this.InputChannel1.Reader.Count > 0)
                await this.CombineInput(await this.InputChannel1.Reader.ReadAsync(), input);
            else
                await this.InputChannel2.Writer.WriteAsync(input);

            Interlocked.Exchange(ref this.inputLock, 0);
        }

        private async ValueTask CombineInput(TInput1 input1, TInput2 input2)
        {
            TOutput? output = this.Assembler.Assemble(input1, input2);
            for (int i = 0; i < this.Outputs.Length; i++)
                await this.Outputs[i].WriteAsync(output);
        }

        public class InputHandle1 : IInputHandle<TInput1>
        {
            internal SystemLink<TAssembler, TOutput, TInput1, TInput2> link;

            public InputHandle1(SystemLink<TAssembler, TOutput, TInput1, TInput2> link)
                => this.link = link;

            public async ValueTask PassInput(TInput1 input)
                => await this.link.PassInput(input);
        }

        public class InputHandle2 : IInputHandle<TInput2>
        {
            internal SystemLink<TAssembler, TOutput, TInput1, TInput2> link;

            public InputHandle2(SystemLink<TAssembler, TOutput, TInput1, TInput2> link)
                => this.link = link;

            public async ValueTask PassInput(TInput2 input)
                => await this.link.PassInput(input);
        }
    }

    public interface IInputHandle<TInput>
    {
        public ValueTask PassInput(TInput input);
    }
}
