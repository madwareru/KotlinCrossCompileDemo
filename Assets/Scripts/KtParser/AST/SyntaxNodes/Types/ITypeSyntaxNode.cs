namespace KtParser.AST.SyntaxNodes.Types
{
    public interface ITypeSyntaxNode: ISimpleSyntaxNode
    {
        string CSharpGeneric();
        string KotlinReturned();
        string CSharpAsArgument(string text);
        string CSharpCast(string text);
        bool IsVoid();
    }
}