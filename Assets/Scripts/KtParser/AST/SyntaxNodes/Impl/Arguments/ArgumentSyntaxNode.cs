using KtParser.AST.SyntaxNodes.Arguments;
using KtParser.AST.SyntaxNodes.Impl.Types;
using KtParser.AST.SyntaxNodes.Types;

namespace KtParser.AST.SyntaxNodes.Impl.Arguments
{
    public class ArgumentSyntaxNode: IArgumentSyntaxNode
    {
        private readonly ITypeSyntaxNode _type;
        private readonly string _name;

        public ArgumentSyntaxNode(string type, string name, bool fromKotlin = false):
            this(new SmartTypeSyntaxNode(type), name)
        {
        }
        
        public ArgumentSyntaxNode(ITypeSyntaxNode type, string name)
        {
            _type = type;
            _name = name;
        }

        public string GenerateCSharp() =>
            $"{_type.GenerateCSharp()} {_name}";

        public string GenerateKotlin() =>
            $"{_name}: {_type.GenerateKotlin()}";

        public string GenCSharpName() => _name;
    }
}