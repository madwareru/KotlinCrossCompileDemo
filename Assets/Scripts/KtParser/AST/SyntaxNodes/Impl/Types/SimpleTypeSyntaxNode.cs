using System;
using System.Collections;
using KtParser.AST.SyntaxNodes.Types;

namespace KtParser.AST.SyntaxNodes.Impl.Types
{
    public class SimpleTypeSyntaxNode: ITypeSyntaxNode
    {

        private static readonly string[] _simpleTypes =
            {"void", "int", "long", "string", "char", "byte", "float", "double", "bool"};

        private readonly string _type;

        public SimpleTypeSyntaxNode(string type)
        {
            if (!IsSimple(type))
                throw new ArgumentException("not simple type");

            _type = type;
        }
        
        public string GenerateCSharp() => _type;

        public string GenerateKotlin() => _type.ToUpperFirst();

        public string CSharpGeneric() => IsVoid() ? "" : $"<{_type}>";

        public string KotlinReturned() => 
            IsVoid() ? "" : $": {GenerateKotlin()}";

        public string CSharpAsArgument(string text) => text;
        
        public string CSharpCast(string text) => text;

        public bool IsVoid() => string.Equals(_type, "void");
        
        public static bool IsSimple(string type) =>
            ((IList) _simpleTypes).Contains(type) || string.Equals(type, "boolean");
    }
}