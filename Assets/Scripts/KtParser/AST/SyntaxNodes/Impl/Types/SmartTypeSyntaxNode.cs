using DefaultNamespace.KtParser;
using KtParser.AST.SyntaxNodes.Types;

namespace KtParser.AST.SyntaxNodes.Impl.Types
{
    public class SmartTypeSyntaxNode : ITypeSyntaxNode
    {
        private readonly ITypeSyntaxNode _wrapped;

        public SmartTypeSyntaxNode(string type)
        {
            if (SimpleTypeSyntaxNode.IsSimple(type.ToLowerFirst()))
                _wrapped = new SimpleTypeSyntaxNode(string.Equals(type, "Boolean") ? "bool" : type.ToLowerFirst());
            else if (type.Contains(KotlinParser.ProxySuffix))
                _wrapped = new ClassTypeSyntaxNode(type, true);
            else if (type.Contains(KotlinParser.ObjectSuffix))
                _wrapped = new ClassTypeSyntaxNode(type, false);
            else if (type.Contains("Array"))
            {
                _wrapped = new ArrayTypeSyntaxNode(
                    new SimpleTypeSyntaxNode(type.Substring(0, type.Length - 5).ToLowerFirst())
                );
            }
            else
                _wrapped = new UnknownClassTypeSyntaxNode(type);
        }

        public string GenerateCSharp() => _wrapped.GenerateCSharp();

        public string GenerateKotlin() => _wrapped.GenerateKotlin();

        public string CSharpGeneric() => _wrapped.CSharpGeneric();

        public string KotlinReturned() => _wrapped.KotlinReturned();

        public string CSharpAsArgument(string text) => _wrapped.CSharpAsArgument(text);
    
        public string CSharpCast(string text) => _wrapped.CSharpCast(text);
     
        public bool IsVoid() => _wrapped.IsVoid();
    }
}