using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrideDiagnostics.PropertyFinder;
public interface IPropertyFinder
{
    public IEnumerable<IPropertySymbol> Find(ref INamedTypeSymbol baseType);
}
