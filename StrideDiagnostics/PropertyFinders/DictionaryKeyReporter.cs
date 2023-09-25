using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrideDiagnostics.PropertyFinders;
internal class DictionaryKeyReporter : IViolationReporter, IPropertyFinder
{
    /// <summary>
    /// Is Always Empty, use <see cref="CollectionPropertyFinder"/> and check against <see cref="PropertyHelper.IsDictionary(IPropertySymbol)"/>
    /// </summary>
    /// <param name="baseType"></param>
    /// <returns></returns>
    public IEnumerable<IPropertySymbol> Find(ref INamedTypeSymbol baseType)
    {
        return Enumerable.Empty<IPropertySymbol>(); ;
    }

    public void ReportViolations(ref INamedTypeSymbol baseType, ClassInfo classInfo)
    {
        if (baseType == null)
            return;
        IEnumerable<IPropertySymbol> violations = baseType.GetMembers().OfType<IPropertySymbol>().Where(property => PropertyHelper.IsDictionary(property, classInfo) && !this.ShouldBeIgnored(property) && HasProperAccess(property) && InvalidDictionaryKey(property, classInfo));
        foreach (IPropertySymbol violation in violations)
        {
            Report(violation, classInfo);
        }
    }

    private bool HasProperAccess(IPropertySymbol property)
    {
        return property.GetMethod?.DeclaredAccessibility == Accessibility.Public ||
                property.GetMethod?.DeclaredAccessibility == Accessibility.Internal;
    }

    private bool InvalidDictionaryKey(IPropertySymbol property, ClassInfo info)
    {
        if (PropertyHelper.IsDictionary(property, info))
        {
            INamedTypeSymbol dictionaryInterface = info.ExecutionContext.Compilation.GetTypeByMetadataName(typeof(IDictionary<,>).FullName);
            SymbolEqualityComparer comparer = SymbolEqualityComparer.Default;
            var interfacly = ((INamedTypeSymbol)property.Type).AllInterfaces.First(x => x.OriginalDefinition.Equals(dictionaryInterface, comparer));
            if (IsPrimitiveType(interfacly.TypeArguments[0]))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }
    private bool IsPrimitiveType(ITypeSymbol type)
    {
        switch (type.SpecialType)
        {
            case SpecialType.System_Boolean:
            case SpecialType.System_Byte:
            case SpecialType.System_SByte:
            case SpecialType.System_Char:
            case SpecialType.System_Int16:
            case SpecialType.System_Int32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt16:
            case SpecialType.System_UInt32:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_String:
            case SpecialType.System_Decimal:
            case SpecialType.System_DateTime:
                return true;
            default:
                return false;
        }
    }
    private void Report(IPropertySymbol property, ClassInfo classInfo)
    {
        DiagnosticDescriptor error = new DiagnosticDescriptor(
    id: ErrorCodes.DictionaryKey,
    title: "Invalid Dictionary Key",
    category: NexGenerator.CompilerServicesDiagnosticCategory,
    defaultSeverity: DiagnosticSeverity.Warning,
    isEnabledByDefault: true,
    messageFormat: $"The Generic Key for '{property.Name}' is invalid, expected for a IDictionary<T,Y> is a struct/simple type Key to use this Property as [DataMember]. Add [DataMemberIgnore] to let Stride Ignore the Member in the [DataContract] or change the Dictionary Key.",
    helpLinkUri: "https://www.stride3d.net"
    );
        Location location = Location.Create(classInfo.TypeSyntax.SyntaxTree, property.DeclaringSyntaxReferences.FirstOrDefault().Span);
        classInfo.ExecutionContext.ReportDiagnostic(Diagnostic.Create(error, location));

    }
}
