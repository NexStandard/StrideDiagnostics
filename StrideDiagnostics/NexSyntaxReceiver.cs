using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace StrideDiagnostics;

public class NexSyntaxReceiver : ISyntaxReceiver
{
    TypeAttributeFinder _typeFinder = new();
    AttributeContextValidator _attributeContextValidator = new();
    public List<TypeDeclarationSyntax> TypeDeclarations { get; private set; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        TypeDeclarationSyntax result = _typeFinder.FindAttribute(syntaxNode);

        if (result != null && (HasDataContractAttribute(result) || HasStrideCoreDataContractAttribute(result)))
        {
            TypeDeclarations.Add(result);
        }
    }
    /// <summary>
    /// Checks if the <see cref="TypeDeclarationSyntax"/> has a [DataContract] attribute or [Stride.Core.DataContract]
    /// As there is no validation possible to decide if its from System or Stride this must be validated later again.
    /// </summary>
    /// <param name="info">The Type to check</param>
    /// <returns>True if it has a DataContract Attribute, but its not sure if its the one from Stride</returns>
    private bool HasDataContractAttribute(TypeDeclarationSyntax info)
    {
        return info.AttributeLists
            .SelectMany(attributeList => attributeList.Attributes)
            .Any(attribute => attribute.Name.ToString().Contains("DataContract"));
    }

    private bool HasStrideCoreDataContractAttribute(TypeDeclarationSyntax typeDeclaration)
    {
        // Check if the type declaration has the [Stride.Core.DataContract] attribute
        return typeDeclaration.AttributeLists
            .SelectMany(attributeList => attributeList.Attributes)
            .Any(attribute => attribute.Name.ToString() == "DataContract" && attribute.Name.ToString() == "Stride.Core.DataContract");
    }
}
