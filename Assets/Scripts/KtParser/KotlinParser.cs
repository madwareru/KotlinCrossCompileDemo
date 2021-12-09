using System.Collections.Generic;
using System.Linq;
using KtParser.AST;
using KtParser.AST.SyntaxNodes.Arguments;
using KtParser.AST.SyntaxNodes.Impl;
using KtParser.AST.SyntaxNodes.Impl.Arguments;
using KtParser.AST.SyntaxNodes.Impl.Methods;
using KtParser.AST.SyntaxNodes.Impl.Types;
using KtParser.AST.SyntaxNodes.Methods;
using KtParser.AST.SyntaxNodes.Types;
using Sprache;

namespace DefaultNamespace.KtParser
{
    public static class KotlinParser
    {
        public static IRootSyntaxNode ClassFromAbstractClass(string text) =>
            AbstractClassParser.Parse(text);

        public static IRootSyntaxNode ProxyFromInterface(string text) =>
            InterfaceParser.Parse(text);

        #region Suffixes

        public static string ObjectSuffix = "Object";
        public static string ProxySuffix = "Proxy";

        #endregion

        #region Remove prefix functions

        private static string RemoveAbstract(string text) =>
            text.StartsWith("Abstract") ? text.Substring(8) : text;

        private static string RemoveI(string text) =>
            text.StartsWith("I") && char.IsUpper(text[1]) ? text.Substring(1) : text;

        private static string RemovePrefix(string text) =>
            text.StartsWith("Abstract") ? text.Substring(8) : RemoveI(text);

        #endregion

        #region Char Parsers
        
        private static readonly Parser<char> LetterDigitOrDot =
            Parse.Char(c => char.IsLetterOrDigit(c) || c == '.' || c == '_', "s");

        private static readonly Parser<string> WordParser =
            Parse.Identifier(Parse.Letter, Parse.LetterOrDigit);

        private static readonly Parser<IEnumerable<char>> Whitespaces =
            Parse.WhiteSpace.Many();
        #endregion

        #region Blank Parsers

        private static readonly Parser<string> CommentParser =
            from _ in Parse.String("//").Seq(Parse.CharExcept('\n').Many())
            select "";

        private static readonly Parser<string> BlankLines =
            from _ in Whitespaces.Or(Parse.LineTerminator).Or(CommentParser).Many()
            select "";

        #endregion

        #region Class Parsers
        
        private static readonly Parser<bool> AncestorStart =
            from _ in Parse.Char(':').Seq(Whitespaces)
            select true;

        private static readonly Parser<bool> AncestorType =
            from _ in Parse.Letter.Seq(LetterDigitOrDot.Many())
            from comma in Parse.Char(',').Optional()
            from __ in Whitespaces 
            select true;

        private static readonly Parser<bool> Ancestors =
            from _ in AncestorStart.Seq(AncestorType.Many())
            select true;

        private static readonly Parser<IEnumerable<char>> VarianceParser =
            Parse.String("in").Or(Parse.String("out")).Token();
      
        private static readonly Parser<string> PackageParser =
            from _ in Parse.String("package").Seq(Whitespaces)
            from chars in LetterDigitOrDot.Many()
            select string.Join("", chars);

        private static readonly Parser<bool> ImportParser =
            from _ in 
                Parse.String("import")
                    .Seq(Whitespaces)
                    .Seq(LetterDigitOrDot.Many())
                    .Seq(BlankLines)
            select true;

        private static readonly Parser<string> AnnotationParser =
            from _ in Parse.Char('@')
            from identifier in WordParser
            select identifier;

        private static readonly Parser<bool> ImportHeading =
            from _ in BlankLines.Seq(ImportParser.Many()).Seq(BlankLines)
            select true;

        private static readonly Parser<(bool, string)> ArrayParser =
            from _ in
                Parse.String("Array").Token()
                    .Seq(Parse.Char('<').Token())
                    .Seq(VarianceParser.Optional())
                    .Token()
            from type in LetterDigitOrDot.Many()
            from __ in Parse.Char('>').Token()
            select (true, RemovePrefix(new string(type.ToArray())));

        private static readonly Parser<(bool, string)> NonArrayParser =
            from w in WordParser
            select (false, RemovePrefix(w));

        private static readonly Parser<ITypeSyntaxNode> TypeParser = 
            from result in ArrayParser.Or(NonArrayParser)
            let isArray = result.Item1
            let name = result.Item2
            select isArray 
                ? new ArrayTypeSyntaxNode(new SmartTypeSyntaxNode(name)) as ITypeSyntaxNode
                : new SmartTypeSyntaxNode(name);

        private static readonly Parser<IEnumerable<char>> VarValParser =
            Parse.String("val").Or(Parse.String("var")).Token();

        private static readonly Parser<IArgumentSyntaxNode> ArgParser =
            from _ in Whitespaces
            from name in WordParser
            from __ in Whitespaces
                .Seq(Parse.Char(':'))
                .Seq(Whitespaces)
            from type in TypeParser
            from ___ in Parse.Char('?').Many()
                .Seq(Whitespaces)
                .Seq(Parse.Char(',').Many())
                .Seq(Whitespaces)
            select new ArgumentSyntaxNode(type, name);
        
        private static readonly Parser<IArgumentSyntaxNode> ConstructorArgParser =
            from _ in Whitespaces
            from __ in VarValParser
            from name in WordParser
            from ___ in Whitespaces
                .Seq(Parse.Char(':'))
                .Seq(Whitespaces)
            from type in TypeParser
            from ____ in Parse.Char('?').Many()
                .Seq(Whitespaces)
                .Seq(Parse.Char(',').Many())
                .Seq(Whitespaces)
            select new ArgumentSyntaxNode(type, name);

        private static readonly Parser<ITypeSyntaxNode> ReturnTypeParser =
            from t in
                (
                    from _ in Parse.Char(':').Seq(Whitespaces)
                    from returnedType in TypeParser
                    from __ in  Parse.Char('?').Many()
                    select returnedType
                )
                .Optional()
            select t.UnwrapOrDefault(new SmartTypeSyntaxNode("Void"));

        private static readonly Parser<IMethodSyntaxNode> MethodParser =
            from _ in Whitespaces
                .Seq(AnnotationParser.Seq(Whitespaces).Many().Optional())
                .Seq(BlankLines.Optional())
                .Seq(Whitespaces)
                .Seq(Parse.String("abstract").Seq(Whitespaces).Optional())
                .Seq(Parse.String("fun"))
                .Seq(Whitespaces)
            from methodName in WordParser
            from openBraceToken in Parse.Char('(')
            from args in ArgParser.Many()
            from closingBraceToken in Parse.Char(')').Seq(Whitespaces)
            from returnedType in ReturnTypeParser
            from __ in BlankLines.Many()
            select new MethodSyntaxNode(methodName, returnedType,args.ToArray());

        private static readonly Parser<IArgumentsSyntaxNode> ConstructorArgsParser =
            from _ in Parse.Char('(')
            from args in ConstructorArgParser.Many()
            from __ in Parse.Char(')')
            select new ArgumentsSyntaxNode(args.ToArray());

        private static readonly Parser<IMethodSyntaxNode[]> ObjectBodyParser =
            from _ in Whitespaces
                .Seq(Ancestors.Optional())
                .Seq(Parse.Char('{').Once())
                .Seq(BlankLines)
            from methods in MethodParser.Many()
            from __ in Parse.Char('}').Seq(BlankLines)
            select methods.ToArray();
        
        private static readonly IArgumentsSyntaxNode EmptyArgumentsSyntaxNode = new ArgumentsSyntaxNode();

        private static readonly Parser<IRootSyntaxNode> AbstractClassParser =
            from packageName in PackageParser
            from _ in ImportHeading.Seq(Parse.String("abstract class")).Seq(Whitespaces)
            from abstractClassName in WordParser
            from constructorArgs in ConstructorArgsParser.Optional()
            from classMethods in ObjectBodyParser
            let constructorArguments = constructorArgs.UnwrapOrDefault(EmptyArgumentsSyntaxNode)
            let className = $"{packageName}.{RemoveAbstract(abstractClassName)}"
            select new RootSyntaxNode(className, constructorArguments, classMethods);
        
        private static readonly Parser<IRootSyntaxNode> InterfaceParser =
            from packageName in PackageParser
            from _ in ImportHeading.Seq(Parse.String("interface")).Seq(Whitespaces)
            from interfaceName in WordParser
            from interfaceMethods in ObjectBodyParser
            let className = $"{packageName}.{RemoveI(interfaceName)}"
            select new RootSyntaxNode(className, interfaceMethods);

        #endregion
    }
}