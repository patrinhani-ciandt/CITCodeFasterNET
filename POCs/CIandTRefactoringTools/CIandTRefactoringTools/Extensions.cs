﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;

namespace CIandTRefactoringTools
{
    public static class Extensions
    {
        public static CompilationUnitSyntax WithUsing(this CompilationUnitSyntax root, string name)
        {
            if (!root.Usings.Any(u => u.Name.ToString() == name))
            {
                root = root.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(name)));
            }

            return root;
        }

        public static IList<string> GetUsings(this CompilationUnitSyntax root)
        {
            return root.Usings.Select(u => u.Name.ToString()).ToList();
        }

        public static TypeInfo GetTypeInfoFromIdentifier(this IdentifierNameSyntax identifier, Document document)
        {
            var semanticModel = document.GetSemanticModelAsync().Result;
            return semanticModel.GetTypeInfo(identifier);
        }

        public static INamespaceSymbol GetNamespaceSymbol(this TypeInfo typeInfo)
        {
            if (typeInfo.Type is IArrayTypeSymbol)
                return (typeInfo.Type as IArrayTypeSymbol).ElementType.ContainingNamespace;

            return typeInfo.Type.ContainingNamespace;
        }

        public static Document SimplifyDocument(this Document document)
        {
            return Simplifier.ReduceAsync(document, Simplifier.Annotation).Result;
        }

        public static Document FormatDocument(this Document document)
        {
            return Formatter.FormatAsync(document).Result;
        }
    }
}