using Microsoft.CodeAnalysis;
using System.Linq;

namespace StrideDiagnostics.PropertyFinders;

internal static class PropertyHelper
{
    public static bool IsArray(IPropertySymbol propertyInfo)
    {
        var propertyType = propertyInfo.Type;

        if (propertyType.TypeKind == TypeKind.Array)
        {
            return true;
        }
        return false;
    }
    public static bool ImplementsICollectionT(ITypeSymbol type)
    {
        if (type.AllInterfaces.Any(i =>
            (i.OriginalDefinition is INamedTypeSymbol namedType && namedType.ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_ICollection_T)))
        {
            return true;
        }

        return false;
    }

}