namespace KtParser.AST.SyntaxNodes.Arguments
{
    public interface IArgumentsSyntaxNode: ISimpleSyntaxNode
    {
        string CSharpNamesWithComma();
        string CSharpNames();
    }
}