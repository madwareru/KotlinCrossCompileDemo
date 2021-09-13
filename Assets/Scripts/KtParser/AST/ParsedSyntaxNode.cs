using DefaultNamespace.KtParser;
using KtParser.Writers;

namespace KtParser.AST
{
    public class ParsedSyntaxNode: IRootSyntaxNode
    {
        private readonly IRootSyntaxNode _wrapped;
        
        public ParsedSyntaxNode(string text, bool isProxy):
            this(isProxy ? KotlinParser.ProxyFromInterface(text) : KotlinParser.ClassFromAbstractClass(text))
        {
        }

        private ParsedSyntaxNode(IRootSyntaxNode wrapped)
        {
            _wrapped = wrapped;
        }

        public string GetCSharpClassName() => _wrapped.GetCSharpClassName();

        public string GetKotlinClassName() => _wrapped.GetKotlinClassName();

        public string GetKotlinPackage() => _wrapped.GetKotlinPackage();

        public ICodeWriter GenerateCSharpObject(ICodeWriter cb) => _wrapped.GenerateCSharpObject(cb);

        public ICodeWriter GenerateKotlinObject(ICodeWriter cb) => _wrapped.GenerateKotlinObject(cb);

        public ICodeWriter GenerateCSharpProxy(ICodeWriter cb) => _wrapped.GenerateCSharpProxy(cb);

        public ICodeWriter GenerateKotlinProxy(ICodeWriter cb) => _wrapped.GenerateKotlinProxy(cb);
    }
}