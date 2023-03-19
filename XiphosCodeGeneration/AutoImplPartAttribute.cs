using System;

namespace XiphosCodeGeneration;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class AutoImplPartAttribute : Attribute
{
    public AutoImplPartAttribute(params string[] blacklist) { }
}
