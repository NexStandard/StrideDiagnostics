using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace StrideDiagnostics;

[Generator]
public class NexGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Debugger.Launch();
        context.RegisterForSyntaxNotifications(() => new NexSyntaxReceiver());
    }
    private Diagnoser classGenerator { get; set; } = new();
    public void Execute(GeneratorExecutionContext context)
    {
        NexSyntaxReceiver syntaxReceiver = (NexSyntaxReceiver)context.SyntaxReceiver;

        foreach (TypeDeclarationSyntax classDeclaration in syntaxReceiver.TypeDeclarations)
        {
            SemanticModel semanticModel = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            ClassInfo info = new()
            {
                ExecutionContext = context,
                TypeSyntax = classDeclaration,
                SyntaxReceiver = syntaxReceiver,
                Symbol = semanticModel.GetDeclaredSymbol(classDeclaration),

            };
            classGenerator.StartCreation(info);
        }
    }

    public const string CompilerServicesDiagnosticIdFormat = "STRD{0:000}";

    public const string CompilerServicesDiagnosticCategory = "Stride.CompilerServices";
}
