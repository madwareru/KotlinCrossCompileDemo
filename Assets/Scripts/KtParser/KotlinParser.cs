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

        #endregion

        #region Char Parsers
        
        private static readonly Parser<char> LetterDigitOrDot =
            Parse.Char(c => char.IsLetterOrDigit(c) || c == '.' || c == '_', "s");

        private static readonly Parser<string> WordParser =
            Parse.Identifier(Parse.Letter, Parse.LetterOrDigit);
        
        #endregion

        #region Blank Parsers

        private static readonly Parser<string> CommentParser =
            Parse.String("//").Seq(Parse.CharExcept('\n').Many()).Map(_ => "");

        private static readonly Parser<string> BlankLines =
            Parse.WhiteSpace.Many().Or(Parse.LineTerminator).Or(CommentParser).Many().Map(_ => "");

        #endregion

        #region Class Parsers
        
        private static readonly Parser<bool> InterfaceAncestorStart =
            Parse.Char(':').Token().Map(_ => true);

        private static readonly Parser<bool> InterfaceAncestorType =
            Parse.Letter.Seq(LetterDigitOrDot.Many()).Seq(Parse.Char(',').Optional()).Token().Map(_ => true);

        private static readonly Parser<bool> InterfaceAncestors =
            InterfaceAncestorStart.Seq(InterfaceAncestorType.Many()).Map(_ => true);

        private static readonly Parser<IEnumerable<char>> VarianceParser =
            Parse.String("in").Or(Parse.String("out")).Token();

        private static readonly Parser<string> PackageParser =
            Parse.String("package").Token()
                .Seq(LetterDigitOrDot.Many())
                .Map(it => string.Join("", it.Item2));

        private static readonly Parser<bool> ImportParser =
            Parse.String("import").Token()
                .Seq(LetterDigitOrDot.Many())
                .Seq(BlankLines)
                .Map(_ => true);

        private static readonly Parser<string> AnnotationParser =
            WordParser.SurroundBy(
                Parse.Char('@'), 
                Parse.Identifier(
                    Parse.Letter, 
                    Parse.LetterOrDigit.Or(Parse.Char('_'))
                ).Braces('(', ')').Optional()
            );
        
        private static Parser<T> Annotated<T>(Parser<T> p) =>
            AnnotationParser.Token().Many().Optional()
                .Seq(BlankLines.Optional())
                .Seq(p)
                .Map(it => it.Item2);

        private static readonly Parser<bool> ImportHeading =
            BlankLines.Seq(ImportParser.Many()).Seq(BlankLines).Map(_ => true);

        private static readonly Parser<(bool, string)> ArrayParser =
            LetterDigitOrDot
                .Many()
                .SurroundBy(
                    Parse.String("Array").Token()
                        .Seq(Parse.Char('<').Token())
                        .Seq(VarianceParser.Optional()).Token(),
                    Parse.Char('>').Token()
                ).Map(it => (true, new string(it.ToArray())));

        private static readonly Parser<(bool, string)> NonArrayParser =
            WordParser.Map(it => (false, it));

        private static readonly Parser<ITypeSyntaxNode> TypeParser = 
            from result in ArrayParser.Or(NonArrayParser)
            let isArray = result.Item1
            let name = result.Item2
            select isArray 
                ? new ArrayTypeSyntaxNode(new SmartTypeSyntaxNode(name)) as ITypeSyntaxNode
                : new SmartTypeSyntaxNode(name);

        private static readonly Parser<IEnumerable<char>> VarValParser =
            Parse.String("val").Or(Parse.String("var"));

        private static readonly Parser<IArgumentSyntaxNode> ArgParser =
            WordParser.Token().Bind( 
                name => TypeParser.SurroundBy(
                    Parse.Char(':').Token(),
                    Parse.Char('?').Many().Token().Seq(Parse.Char(',').Many().Token())
                ).Map(typeInfo => new ArgumentSyntaxNode(typeInfo, name))
            );

        private static readonly Parser<IArgumentSyntaxNode> ConstructorArgParser =
            VarValParser.Token().Seq(ArgParser).Map(it => it.Item2);

        private static readonly Parser<ITypeSyntaxNode> ReturnTypeParser =
            TypeParser
                .SurroundBy(
                    Parse.Char(':').Token(),
                    Parse.Char('?').Many()
                )
                .Optional()
                .Map(it => it.UnwrapOrDefault(new SmartTypeSyntaxNode("Void")));

        private static Parser<IMethodSyntaxNode> MeaningfulMethodIfoParser = 
            from methodName in WordParser.Token()
            from args in ArgParser.Many().Braces('(', ')').Token()
            from returnedType in ReturnTypeParser
            select new MethodSyntaxNode(methodName, returnedType,args.ToArray());

        private static readonly Parser<IMethodSyntaxNode> MethodParser =
            MeaningfulMethodIfoParser
                .SurroundBy(
                    Annotated(
                        Parse.String("abstract").Token().Optional().Seq(Parse.String("fun").Token())
                    ),
                    BlankLines.Many()
                );

        private static readonly Parser<IArgumentsSyntaxNode> ConstructorArgsParser =
            ConstructorArgParser
                .Many()
                .Braces('(', ')')
                .Map(it => new ArgumentsSyntaxNode(it.ToArray()));

        private static readonly Parser<IMethodSyntaxNode[]> SharedBodyParser =
            MethodParser.Many()
                .SurroundBy(
                    InterfaceAncestors.Optional().Seq(Parse.Char('{').Once()).Seq(BlankLines).Token(),
                    Parse.Char('}').Seq(BlankLines)
                )
                .Map(it => it.ToArray());
        
        private static readonly IArgumentsSyntaxNode EmptyArgumentsSyntaxNode = new ArgumentsSyntaxNode();

        private static readonly Parser<IRootSyntaxNode> AbstractClassParser =
            from packageName in PackageParser
            from _ in ImportHeading.Seq(Annotated(Parse.String("abstract class"))).Token()
            from abstractClassName in WordParser.Token()
            from constructorArgs in ConstructorArgsParser.Optional()
            from classMethods in SharedBodyParser
            let constructorArguments = constructorArgs.UnwrapOrDefault(EmptyArgumentsSyntaxNode)
            let className = $"{packageName}.{RemoveAbstract(abstractClassName)}"
            select new RootSyntaxNode(className, constructorArguments, classMethods);

        private static readonly Parser<IRootSyntaxNode> InterfaceParser =
            from packageName in PackageParser
            from _ in ImportHeading.Seq(Annotated(Parse.String("interface"))).Token()
            from interfaceName in WordParser.Token()
            from interfaceMethods in SharedBodyParser
            let className = $"{packageName}.{RemoveI(interfaceName)}"
            select new RootSyntaxNode(className, interfaceMethods);

        private static string RemoveAbstract(string name)
        {
            if (name.StartsWith("Abstract"))
                return name.Substring(8);
            return name;
        }
        
        private static string RemoveI(string name)
        {
            if (name.StartsWith("I"))
                return name.Substring(1);
            return name;
        }
        
        #endregion
    }
}