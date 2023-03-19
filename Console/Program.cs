//using Xiphos.ECS.Components;
//using Xiphos.ECS.Systems;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Xiphos.ECS2.System;
using Xiphos.Monads;

namespace Testing;

interface IBruh<T, TSelf>
    where TSelf : IBruh<T, TSelf>
{
    static abstract void Test(T @in);
}

struct Bruh : IBruh<int, Bruh>, IBruh<float, Bruh>
{
    public static void Test(int @in) { }

    public static void Test(float @in) { }
}

internal unsafe class Program
{
    private static void Main(string[] args)
    {
        var interfaces = typeof(Bruh).GetInterfaces();

        var method = typeof(Bruh)
            .FindInterfaces(
            (type, criteria) => type.GetGenericTypeDefinition() == typeof(IBruh<,>), 
            null);

        return;
    }

    public static void SimdCopy256Blocks(void* dest, void* src, ulong blockCount)
    {
        while (blockCount-- > 0)
            Avx.Store((byte*)((Vector256<byte>*)dest + blockCount), Avx.LoadAlignedVector256((byte*)((Vector256<byte>*)src + blockCount)));
    }
}

internal struct Unit { }

internal struct PhysicsSetupExecution : ISystemExecution<PhysicsInput, Unit>
{
    private int _index;
    private static readonly string[] _labels = new[] { "Broad phase", "Narrow phase", "Calculating" };

    public PhysicsInput Execute(Unit input)
    {
        this._index = (this._index + 1) % _labels.Length;
        return new() { phase = _labels[this._index] };
    }
}

internal struct TimingSetupExecution : ISystemExecution<double, Unit>
{
    private int _counter;

    public double Execute(Unit input) => this._counter++;
}

internal struct PhysicsInput { public string phase; }

internal struct PhysicsOutput { public string result; }

internal struct PhysicsExecution : ISystemExecution<PhysicsOutput, PhysicsInput>
{
    private bool _op;

    public PhysicsOutput Execute(PhysicsInput input)
    {
        this._op = !this._op;
        return new() { result = this._op ? input.phase.ToUpper() : input.phase.ToLower() };
    }
}

internal struct FunkyOutput { public string final; }

internal struct FunkyExecution : ISystemExecution<FunkyOutput, (double, PhysicsOutput)>
{
    public FunkyOutput Execute((double, PhysicsOutput) input) => new() { final = input.Item2.result + input.Item1 };
}

internal struct FunkyAssembler : ISystemAssembler<(double, PhysicsOutput), double, PhysicsOutput>
{
    public (double, PhysicsOutput) Assemble(double arg1, PhysicsOutput arg2) => (arg1, arg2);
}

internal struct DirectAssembler<T> : ISystemAssembler<T, T>
{
    public T Assemble(T input) => input;
}
