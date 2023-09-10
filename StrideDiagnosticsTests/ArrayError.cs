using Microsoft.CodeAnalysis;
using System.Runtime.Serialization;
using Xunit;

namespace StrideDiagnosticsTests;

[DataContract]
public class ArrayError
{
    [Fact]
    public void ErrorOnInvalidArrayAccess()
    {
        // Define the source code for the Class1 class with an invalid property
        string sourceCode = @"
using System.Runtime.Serialization;

[DataContract]
public class ArrayError
{
    public ArrayError[] Array { private get; set; }
}";
        IEnumerable<Diagnostic> generatedDiagnostics = DiagnosticsHelper.GetDiagnostics(sourceCode);
        // Check if there are any diagnostics with the expected ID
        bool hasError = generatedDiagnostics.Any(diagnostic =>
            diagnostic.Severity == DiagnosticSeverity.Error &&
            diagnostic.Id == "STRD001");

        // Assert that there is an error
        Assert.True(hasError, "The 'Array' property should generate an error.");

    }
}