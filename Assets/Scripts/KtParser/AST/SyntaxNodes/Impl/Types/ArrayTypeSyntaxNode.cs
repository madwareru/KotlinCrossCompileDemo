using KtParser.AST.SyntaxNodes.Types;

namespace KtParser.AST.SyntaxNodes.Impl.Types
{
    public class ArrayTypeSyntaxNode: ITypeSyntaxNode
    {
        private readonly ITypeSyntaxNode _innerType;

        public ArrayTypeSyntaxNode(ITypeSyntaxNode innerType)
        {
            _innerType = innerType;
        }
        
        public string GenerateCSharp() => $"{_innerType.GenerateCSharp()}[]";

        public string GenerateKotlin() => $"Array<{_innerType.GenerateKotlin()}>";

        public string CSharpGeneric() => $"<{_innerType.GenerateCSharp()}[]>";

        public string KotlinReturned() => GenerateKotlin();

        public string CSharpAsArgument(string text) => text;

        public string CSharpCast(string text) => text;

        public bool IsVoid() => false;
    }
}