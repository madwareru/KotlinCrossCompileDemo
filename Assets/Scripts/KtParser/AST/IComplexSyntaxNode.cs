using KtParser.Writers;

namespace KtParser.AST
{
    public interface IComplexSyntaxNode
    {
        ICodeWriter GenerateCSharpObject(ICodeWriter cb);
        ICodeWriter GenerateKotlinObject(ICodeWriter cb);
        ICodeWriter GenerateCSharpProxy(ICodeWriter cb);
        ICodeWriter GenerateKotlinProxy(ICodeWriter cb);
    }
}