using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Sources;

namespace StrideDiagnostics.PropertyFinders;
internal class DoubledAnnotationReporter : IViolationReporter, IPropertyFinder
{
    /// <summary>
    /// Is always Empty
    /// </summary>
    /// <param name="baseType"></param>
    /// <returns></returns>
    public IEnumerable<IPropertySymbol> Find(ref INamedTypeSymbol baseType)
    {
        return Enumerable.Empty<IPropertySymbol>();
    }

    public void ReportViolations(ref INamedTypeSymbol baseType, ClassInfo classInfo)
    {
        if (baseType == null)
            return;
        var violations = baseType.GetMembers().OfType<IPropertySymbol>().ToList();

        var violations3 = violations.Where(property => this.ShouldBeIgnored(property)).ToList();
        var violations2 = violations3.Where(property => HasDataMemberAnnotation(property)).ToList();
        foreach (var violation in violations2)
        {

            Report(violation, classInfo);


        }
    }
    private bool HasDataMemberAnnotation(IPropertySymbol property)
    {
        var attributes = property.GetAttributes();
        foreach (var attribute in attributes)
        {
            var attributeType = attribute.AttributeClass;
            if (attributeType != null)
            {
                if (attributeType.Name == "DataMember" ||
                    (attributeType.Name == "DataMemberAttribute"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static void Report(IPropertySymbol property, ClassInfo classInfo)
    {
        DiagnosticDescriptor error = new DiagnosticDescriptor(
            id: ErrorCodes.DoubledAnnotation,
            title: "Invalid Annotations",
            category: NexGenerator.CompilerServicesDiagnosticCategory,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            messageFormat: $"The Property has a contradiction in the Annotations, there can't be [DataMember] and [DataMemberIgnore] on the same Property.",
            helpLinkUri: "https://www.stride3d.net"
        );
        Location location = Location.Create(classInfo.TypeSyntax.SyntaxTree, property.DeclaringSyntaxReferences.FirstOrDefault().Span);
        classInfo.ExecutionContext.ReportDiagnostic(Diagnostic.Create(error, location));
    }
}
