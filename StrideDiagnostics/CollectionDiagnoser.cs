using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrideDiagnostics;
public class CollectionDiagnoser
{
    public bool Diagnose(IPropertySymbol propertySymbol, ClassInfo classInfo, bool dontDiagnose = false)
    {
        if (!HasValidAccessOnArrayOrCollection(propertySymbol))
        {
            if (!dontDiagnose)
            {
                Report(propertySymbol, classInfo);
            }
            return false;
        }
        return true;
    }
    public static bool HasValidAccessOnArrayOrCollection(IPropertySymbol property)
    {
        return (property.GetMethod?.DeclaredAccessibility == Accessibility.Public ||
                property.GetMethod?.DeclaredAccessibility == Accessibility.Internal);
    }
    private static void Report(IPropertySymbol property, ClassInfo classInfo)
    {
        DiagnosticDescriptor error = new DiagnosticDescriptor(
            id: string.Format(NexGenerator.CompilerServicesDiagnosticIdFormat, 1),
            title: "Faulty Access",
            category: NexGenerator.CompilerServicesDiagnosticCategory,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            messageFormat: "The Property {property.Name} has an invalid Access Type for an Collection/Array, expected for Collections is a public/internal Get; Method, Stride will not be able to use this Property as DataMember. Add [DataMemberIgnore] to let Stride Ignore the Member in the DataContract or change the Get; accesibility.",
            helpLinkUri: "https://www.stride3d.net"
        );
        Location location = Location.Create(classInfo.TypeSyntax.SyntaxTree, property.DeclaringSyntaxReferences.FirstOrDefault().Span);
        classInfo.ExecutionContext.ReportDiagnostic(Diagnostic.Create(error, location));
    }
}
