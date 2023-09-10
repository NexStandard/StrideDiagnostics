using Microsoft.CodeAnalysis;
using StrideDiagnostics.PropertyFinder;
using StrideDiagnostics.PropertyFinders;
using System.Collections.Generic;
using System.Linq;

namespace StrideDiagnostics;

internal class Diagnoser
{
    internal void StartCreation(ClassInfo info)
    {
        DiagnoseDataMember(info);
    }
    private void DiagnoseDataMember(ClassInfo info)
    {
        IViolationReporter arrayReporter = new ArrayPropertyFinder();
        var symbol = info.Symbol;
        arrayReporter.ReportViolations(ref symbol, info);
    }



    public static IEnumerable<IPropertySymbol> SearchIgnoredProperties(INamedTypeSymbol currentBaseType)
    {
        return currentBaseType.GetMembers().OfType<IPropertySymbol>().Where(property => !HasDataMemberIgnoreAttribute(property));
    }
    private static bool HasDataMemberIgnoreAttribute(IPropertySymbol property)
    {
        var attributes = property.GetAttributes();
        foreach (var attribute in attributes)
        {
            var attributeType = attribute.AttributeClass;
            if (attributeType != null)
            {
                if (attributeType.Name == "DataMemberIgnore" &&
                    (attributeType.Name == "Stride.Core.DataMemberIgnore"))
                {
                    // Check if it's the desired attribute
                    return true;
                }
            }
        }
        return false;
    }
    public static bool HasValidAccessOnArrayOrCollection(IPropertySymbol property)
    {
        return
                (property.GetMethod?.DeclaredAccessibility == Accessibility.Public ||
                property.GetMethod?.DeclaredAccessibility == Accessibility.Internal);
    }
    public static bool IsCollectionOrArray(IPropertySymbol propertyInfo)
    {
        var propertyType = propertyInfo.Type;

        // Check if it's an array
        if (propertyType.TypeKind == TypeKind.Array)
        {
            return true;
        }

        // Check if it's a generic ICollection<T>
        if (propertyType is INamedTypeSymbol namedTypeSymbol &&
            namedTypeSymbol.IsGenericType &&
            namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_ICollection_T)
        {
            return true;
        }

        return false;
    }
}