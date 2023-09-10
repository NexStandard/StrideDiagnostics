using Microsoft.CodeAnalysis;

namespace StrideDiagnostics.PropertyFinder;

public interface IViolationReporter
{
    public void ReportViolations(ref INamedTypeSymbol baseType, ClassInfo classInfo);
}