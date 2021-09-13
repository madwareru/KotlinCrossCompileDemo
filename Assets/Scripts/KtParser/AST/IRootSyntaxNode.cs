namespace KtParser.AST
{
    public interface IRootSyntaxNode: IComplexSyntaxNode
    {
        string GetCSharpClassName();
        string GetKotlinClassName();
        string GetKotlinPackage();
    }
}