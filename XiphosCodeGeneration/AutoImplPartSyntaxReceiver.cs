using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace XiphosCodeGeneration;

public class AutoImplPartSyntaxReceiver : ISyntaxContextReceiver
{
    public List<TypeDeclarationSyntax> possibleClasses = new();
    public List<string[]> blacklists = new();
    public List<INamedTypeSymbol[]> parts = new();
    public List<IFieldSymbol[]> fields = new();
    public List<INamespaceSymbol> namespaces = new();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        //#if DEBUG
        //            if (!Debugger.IsAttached)
        //            {
        //                Debugger.Launch();
        //            }
        //#endif

        SyntaxNode node = context.Node;
        if (node is StructDeclarationSyntax dec && dec.Modifiers.Any(s => s.Kind() == SyntaxKind.PartialKeyword))
        {
            INamedTypeSymbol semNode = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node);
            System.Collections.Immutable.ImmutableArray<AttributeData> attrs = semNode.GetAttributes();
            AttributeData attr = attrs.FirstOrDefault(a => a.AttributeClass.OriginalDefinition.ToString() == "XiphosCodeGeneration.AutoImplPartAttribute");

            if (attr is not null)
            {
                this.possibleClasses.Add(dec);

                TypedConstant? blacklist = attr.ConstructorArguments.Count() > 0 ? attr.ConstructorArguments.First() : null;
                if (blacklist is not null)
                    this.blacklists.Add(blacklist.Value.Values.Select(c => (string)c.Value).ToArray());
                else
                    this.blacklists.Add(new string[0]);

                this.parts.Add(semNode.AllInterfaces.Where(s => s.OriginalDefinition.ToString() == "Xiphos.ECS2.IHasPart<T>").ToArray());

                this.fields.Add(semNode.GetMembers().Where(s => s is IFieldSymbol).Cast<IFieldSymbol>().ToArray());

                this.namespaces.Add(semNode.ContainingNamespace);
            }
        }
    }
}
