namespace KtParser.AST.SyntaxNodes.Arguments
{
    public interface IArgumentSyntaxNode : ISimpleSyntaxNode
    {
        string GenCSharpArgumentName();
        string GenCSharpName();
    }
}