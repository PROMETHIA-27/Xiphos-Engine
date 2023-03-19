using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace XiphosCodeGeneration;

[Generator]
internal class AutoImplPartGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is AutoImplPartSyntaxReceiver receiver)
        {
            int i = 0;
            foreach (TypeDeclarationSyntax dec in receiver.possibleClasses)
            {
                string[] blacklist = receiver.blacklists[i];
                INamespaceSymbol @namespace = receiver.namespaces[i];
                foreach (INamedTypeSymbol part in receiver.parts[i])
                {
                    string partTypeName = part.TypeArguments[0].ToString();
                    if (blacklist.Contains(part.Name))
                        continue;
                    IFieldSymbol field = null;
                    foreach (IFieldSymbol f in receiver.fields[i])
                    {
                        string fullName = f.Type.ToString();
                        if (fullName == partTypeName)
                        {
                            field = f;
                            break;
                        }
                    }

                    if (field is null)
                        continue;
                    ImplPart(context, dec, @namespace, part, field);
                }

                i++;
            }
        }
    }

    public void Initialize(GeneratorInitializationContext context) => context.RegisterForSyntaxNotifications(() => new AutoImplPartSyntaxReceiver());

    public static void ImplPart(GeneratorExecutionContext context, TypeDeclarationSyntax node, INamespaceSymbol @namespace, INamedTypeSymbol partType, IFieldSymbol field) => context.AddSource($"{@namespace}_{node.Identifier}_{partType.TypeArguments[0]}_{field.Name}",
            $"namespace {@namespace}" + Environment.NewLine +
            $"{{" + Environment.NewLine +
            $"  partial {(node is StructDeclarationSyntax ? "struct" : "class")} {node.Identifier}" + Environment.NewLine +
            $"  {{" + Environment.NewLine +
            $"    public void GetPartRef(out PartRef<{partType.TypeArguments[0]}> part)" + Environment.NewLine +
            $"      => part = Xiphos.ECS2.IEntity.GetPartRef(ref this.{field.Name});" + Environment.NewLine +
            $"  }}" + Environment.NewLine +
            $"}}");
}
