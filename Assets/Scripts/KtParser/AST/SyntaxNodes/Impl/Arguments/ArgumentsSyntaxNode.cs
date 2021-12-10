using System.Linq;
using KtParser.AST.SyntaxNodes.Arguments;

namespace KtParser.AST.SyntaxNodes.Impl.Arguments
{
    public class ArgumentsSyntaxNode: IArgumentsSyntaxNode
    {
        private readonly IArgumentSyntaxNode[] _arguments;
        
        public ArgumentsSyntaxNode(params IArgumentSyntaxNode[] arguments)
        {
            _arguments = arguments;
        }
        
        public string GenerateCSharp() =>
            string.Join(", ", _arguments.Select(s => s.GenerateCSharp()));

        public string GenerateKotlin() => 
            string.Join(", ", _arguments.Select(a => a.GenerateKotlin()));

        public string CSharpNames() =>
            string.Join(", ", _arguments.Select(a => a.GenCSharpName()));
        
        public string CSharpNamesWithComma() =>
            string.Join("", _arguments.Select(a => ", " + a.GenCSharpName().ToLowerFirst()));
    }
}