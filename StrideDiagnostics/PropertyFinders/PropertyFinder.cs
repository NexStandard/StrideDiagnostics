using Microsoft.CodeAnalysis;
using StrideDiagnostics.PropertyFinder;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrideDiagnostics.PropertyFinders;
public class PropertyFinder : IPropertyFinder, IViolationReporter
{
    public IEnumerable<IPropertySymbol> Find(ref INamedTypeSymbol baseType)
    {
        throw new NotImplementedException();
    }

    public void ReportViolations(ref INamedTypeSymbol baseType, ClassInfo classInfo)
    {
        throw new NotImplementedException();
    }
}
