using KtParser.AST.SyntaxNodes.Types;

namespace KtParser.AST.SyntaxNodes.Impl.Types
{
    public class ClassTypeSyntaxNode : ITypeSyntaxNode
    {
        private readonly bool _isProxy;
        private readonly string _type;

        public ClassTypeSyntaxNode(RootSyntaxNode rootSyntaxNode, bool isProxy) :
            this(rootSyntaxNode.GetCSharpClassName(), isProxy)
        {
        }

        public ClassTypeSyntaxNode(string type, bool isProxy)
        {
            _isProxy = isProxy;
            _type = type;
        }

        public string GenerateCSharp() => _type;

        public string GenerateKotlin() => $"Abstract{_type}";

        public string CSharpGeneric() => _isProxy ? "<AndroidJavaProxy>" : "<AndroidJavaObject>";

        public string KotlinReturned() => GenerateKotlin();

        public string CSharpAsArgument(string text) => _isProxy ? text : $"{text}.Inner";

        public string CSharpCast(string text) => 
            $"new {GenerateCSharp()}({text})";

        public bool IsVoid() => false;
    }
}