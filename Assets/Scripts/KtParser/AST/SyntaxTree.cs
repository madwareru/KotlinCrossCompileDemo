namespace KtParser.AST
{
    public class SyntaxTree
    {
        public readonly IRootSyntaxNode Syntax;

        public static SyntaxTree Parse(string content, bool isProxy)
        {   
            var rootSyntaxNode = new ParsedSyntaxNode(content, isProxy);
            return new SyntaxTree(rootSyntaxNode);
        }

        private SyntaxTree(IRootSyntaxNode syntax)
        {
            Syntax = syntax;
        }
    }
}