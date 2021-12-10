using KtParser.AST.SyntaxNodes.Types;
using UnityEngine;

namespace KtParser.AST.SyntaxNodes.Impl.Types
{
    public class UnknownClassTypeSyntaxNode : ITypeSyntaxNode
    {
        private readonly string _name;

        public UnknownClassTypeSyntaxNode(string name)
        {
            Debug.Log(name);
            _name = name;
        }

        public string GenerateCSharp() => 
            _name.StartsWith("I") 
                ? "AndroidJavaProxy"
                :"AndroidJavaObject";

        public string GenerateKotlin() => _name;

        public string CSharpGeneric() => 
            _name.StartsWith("I") 
                ? "<AndroidJavaProxy>"
                :"<AndroidJavaObject>";

        public string KotlinReturned() => _name;

        public string CSharpAsArgument(string text) => _name;

        public string CSharpCast(string text) => text;

        public bool IsVoid() => false;
    }
}