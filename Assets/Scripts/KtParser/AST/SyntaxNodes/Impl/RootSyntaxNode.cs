using System.Linq;
using KtParser.AST.SyntaxNodes.Arguments;
using KtParser.AST.SyntaxNodes.Impl.Arguments;
using KtParser.AST.SyntaxNodes.Methods;
using KtParser.Writers;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using static KtParser.Writers.CodeWriterExt;

namespace KtParser.AST.SyntaxNodes.Impl
{
    public class RootSyntaxNode: IRootSyntaxNode
    {
        private readonly string _className;
        private readonly IArgumentsSyntaxNode _constructorArguments;
        private readonly IMethodSyntaxNode[] _methods;

        public RootSyntaxNode(string className): this(className, new ArgumentsSyntaxNode())
        {
        }

        public RootSyntaxNode(string className, params IMethodSyntaxNode[] methods)
            : this(className, new ArgumentsSyntaxNode(), methods)
        {
        }
        
        public RootSyntaxNode(string className, IArgumentsSyntaxNode constructorArguments, params IMethodSyntaxNode[] methods)
        {
            _className = className;
            _constructorArguments = constructorArguments;
            _methods = methods;
        }        

        public ICodeWriter GenerateKotlinObject(ICodeWriter cb) =>
            cb.Do(
                AddLine($"package {GetKotlinPackage()}\n\n") +
                AddLineWithBracketInline($"abstract class {GetKotlinClassName()}({_constructorArguments.GenerateKotlin()})") +
                
                _methods.Aggregate(Entry, (x, y) => x + y.GenerateKotlinObject) +
                
                CloseBracket()
            );

        public ICodeWriter GenerateCSharpObject(ICodeWriter cb) =>
            cb.Do(
                AddLine($"using {nameof(UnityEngine)};\n\n") +

                AddLineWithBracket("namespace Wrapped.Native") +
                AddLineWithBracket($"public class {GetCSharpClassName()}") +

                AddLine("private readonly AndroidJavaObject _androidJavaObject;\n") +
                AddLine("public AndroidJavaObject Inner => _androidJavaObject;\n") +

                AddLineWithBracket($"public {GetCSharpClassName()}({_constructorArguments.GenerateCSharp()})") +
                AddLine(
                    $"_androidJavaObject = new AndroidJavaObject(\"{KotlinFullClassName()}\"{_constructorArguments.CSharpNamesWithComma()});") +
                CloseBracket() +
                AddLine() +
                AddLineWithBracket($"public {GetCSharpClassName()}(AndroidJavaObject androidJavaObject)") +
                AddLine("_androidJavaObject = androidJavaObject;") +
                CloseBracket() +
                
                _methods.Aggregate(Entry, (x, y) => x + y.GenerateCSharpObject) +
                
                CloseBracket() + 
                CloseBracket()
            );

        public ICodeWriter GenerateCSharpProxy(ICodeWriter cb)
        {
            var innerInterfaceType = $"I{GetCSharpClassName()}";
            return cb.Do(
                AddLine($"using Madware.Utils;\n\n") +
                AddLine($"using {nameof(UnityEngine)};\n\n") +

                AddLineWithBracket("namespace Wrapped.Native") +
                
                AddLineWithBracket($"public class {GetCSharpClassName()}: AndroidJavaProxy") +
                
                AddLine($"private readonly {innerInterfaceType} _inner;") +
                AddLineWithBracket($"public {GetCSharpClassName()}({innerInterfaceType} inner) : base(\"{KotlinFullProxyName()}\")") +
                AddLine("_inner = inner;") +
                CloseBracket() +
                
                _methods.Aggregate(Entry, (x, y) => x + y.GenerateCSharpProxy) +
                
                CloseBracket() +
            
                AddLine() +
                AddLineWithBracket($"public interface {innerInterfaceType}") +
                
                _methods.Aggregate(Entry, (x, y) => x + AddLine(y.CSharpInnerInterface())) +
                
                CloseBracket() +
                CloseBracket()
            );
        }
        
        public ICodeWriter GenerateKotlinProxy(ICodeWriter cb) =>
            cb.Do(
                AddLine($"package {GetKotlinPackage()}\n\n") +
                AddLineWithBracketInline($"interface {GetKotlinClassName()}") +
                _methods.Aggregate(Entry, (x, y) => x + y.GenerateKotlinProxy) +
                CloseBracket()
            );

        public string GetCSharpClassName() =>
            _className.Split('.').Last();

        public string GetKotlinClassName() =>
            $"Abstract{_className.Split('.').Last()}";

        public string GetKotlinPackage()
        {
            var all = _className.Split('.').ToList();
            return string.Join(".", all.GetRange(0, all.Count - 1));
        }

        private string KotlinFullClassName() =>
            _className;
        private string KotlinFullProxyName()
        {
            var className  = $"I{_className.Split('.').Last()}";
            return $"{_className.Substring(0, _className.Length - className.Length + 1)}{className}";
        }
    }
}