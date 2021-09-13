using KtParser.AST.SyntaxNodes.Arguments;
using KtParser.AST.SyntaxNodes.Impl.Arguments;
using KtParser.AST.SyntaxNodes.Impl.Types;
using KtParser.AST.SyntaxNodes.Methods;
using KtParser.AST.SyntaxNodes.Types;
using KtParser.Writers;
using static KtParser.Writers.CodeWriterExt;

namespace KtParser.AST.SyntaxNodes.Impl.Methods
{
    public class MethodSyntaxNode : IMethodSyntaxNode
    {
        private readonly string _name;
        private readonly ITypeSyntaxNode _returnType;
        private readonly IArgumentsSyntaxNode _arguments;

        public MethodSyntaxNode(string name, params IArgumentSyntaxNode[] arguments) :
            this(name, new SimpleTypeSyntaxNode("void"), new ArgumentsSyntaxNode(arguments))
        {
        }

        public MethodSyntaxNode(string name, IArgumentsSyntaxNode arguments) :
            this(name, new SimpleTypeSyntaxNode("void"), arguments)
        {
        }

        public MethodSyntaxNode(string name, ITypeSyntaxNode returnType, params IArgumentSyntaxNode[] arguments) :
            this(name, returnType, new ArgumentsSyntaxNode(arguments))
        {
        }

        private MethodSyntaxNode(string name, ITypeSyntaxNode returnType, IArgumentsSyntaxNode arguments)
        {
            _name = name;
            _returnType = returnType;
            _arguments = arguments;
        }
        
        public ICodeWriter GenerateCSharpObject(ICodeWriter cb) =>
            cb.Do(
                AddLine() +
                AddLine($"public {CSharpSignature()} =>") +
                AddLine($"    {CSharpCastReturnType()};")
            );

        public ICodeWriter GenerateKotlinObject(ICodeWriter cb) =>
            cb.Do(AddLine($"abstract fun {_name.ToLowerFirst()}({_arguments.GenerateKotlin()}) {_returnType.KotlinReturned()}"));
    
        public ICodeWriter GenerateKotlinProxy(ICodeWriter cb) =>
            cb.Do(AddLine($"fun {_name.ToLowerFirst()}({_arguments.GenerateKotlin()}) {_returnType.KotlinReturned()}"));

        public string CSharpInnerInterface() => 
            $"{_returnType.GenerateCSharp()} {_name.ToUpperFirst()}({_arguments.GenerateCSharp()});";

        public ICodeWriter GenerateCSharpProxy(ICodeWriter cb)
        {
            var innerCall = $"_inner.{_name.ToUpperFirst()}({_arguments.CSharpNames()});";
            return cb.Do(
                AddLine("// ReSharper disable once InconsistentNaming, UnusedMember.Global") +
                AddLineWithBracket($"public {_returnType.GenerateCSharp()} {_name}({_arguments.GenerateCSharp()})") +
                (_returnType.IsVoid() 
                    ?
                        AddLineWithBracket("AsyncManager.ExecuteOnMainThread(() =>") +
                        AddLine(innerCall) +
                        CloseBracket()
                    : 
                        AddLine($"return {innerCall}")
                ) +
                CloseBracket()
            );
        }

        private string CSharpCastReturnType() => 
            _returnType.CSharpCast($"_androidJavaObject.Call{_returnType.CSharpGeneric()}(\"{_name.ToLowerFirst()}\"{_arguments.CSharpNamesWithComma()})");

        private string CSharpSignature() =>
            $"{_returnType.GenerateCSharp()} {_name.ToUpperFirst()}({_arguments.GenerateCSharp()})";
    }
}