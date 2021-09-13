using KtParser.AST.SyntaxNodes.Types;

namespace KtParser.AST.SyntaxNodes.Impl.Types
{
    public class UnknownClassTypeSyntaxNode : ITypeSyntaxNode
    {
        private readonly string _name;

        public UnknownClassTypeSyntaxNode(string name)
        {
            _name = name;
        }

        public string GenerateCSharp() => "AndroidJavaObject";

        public string GenerateKotlin() => _name;

        public string CSharpGeneric() => "<AndroidJavaObject>";

        public string KotlinReturned() => _name;

        public string CSharpAsArgument(string text) => _name;

        public string CSharpCast(string text) => text;

        public bool IsVoid() => false;
    }
}