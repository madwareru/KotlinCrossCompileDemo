namespace KtParser.AST
{
    public interface ISimpleSyntaxNode
    {
        string GenerateCSharp();
        string GenerateKotlin();
    }
}