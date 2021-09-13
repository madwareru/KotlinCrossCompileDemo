using KtParser.AST;

namespace KtParser.Writers
{
    public delegate ICodeWriter CodeWriterMutator(ICodeWriter input);

    public struct CodeMutatorBox
    {
        private readonly CodeWriterMutator _writerMutator;

        public CodeMutatorBox(CodeWriterMutator writerMutator)
        {
            _writerMutator = writerMutator;
        }

        public static implicit operator CodeWriterMutator(CodeMutatorBox box)
            => box._writerMutator;

        public static implicit operator CodeMutatorBox(CodeWriterMutator writerMutator)
            => new CodeMutatorBox(writerMutator);

        public static CodeMutatorBox operator +(CodeMutatorBox lhs, CodeWriterMutator rhs) =>
            new CodeMutatorBox(input =>
            {
                lhs._writerMutator(input);
                return rhs(input);
            });
    }

    public static class CodeWriterExt
    {
        public static CodeWriterMutator Entry => i => i;
        public static CodeWriterMutator IncIndent => i => i.IncIndent();
        public static CodeWriterMutator DecIndent => i => i.DecIndent();
        public static CodeWriterMutator AddText(string text = "") => i => i.AddText(text);
        public static CodeWriterMutator AddNewLine => i => i.AddNewLine();
        public static CodeWriterMutator AddIndent => i => i.AddIndent();

        public static CodeWriterMutator AddLine(string line = "") =>
            AddIndent + AddText(line) + AddNewLine;

        public static CodeWriterMutator AddLineWithBracket(string line = "") =>
            AddLine(line) + AddLine("{") + IncIndent;

        public static CodeWriterMutator AddLineWithBracketInline(string line = "") =>
            AddText(line) + AddLine("{") + IncIndent;

        public static CodeWriterMutator CloseBracket(string text = "") =>
            AddLine(text) + DecIndent + AddLine("}");

        public static ICodeWriter Do(this ICodeWriter writer, CodeWriterMutator mutator) =>
            mutator(writer);

        public static CodeWriterMutator GenerateCSharpObject(this IComplexSyntaxNode cGen) =>
            cGen.GenerateCSharpObject;

        public static CodeWriterMutator GenerateKotlinObject(this IComplexSyntaxNode cGen) =>
            cGen.GenerateKotlinObject;

        public static CodeWriterMutator GenerateCSharpProxy(this IComplexSyntaxNode cGen) =>
            cGen.GenerateCSharpProxy;

        public static CodeWriterMutator GenerateKotlinProxy(this IComplexSyntaxNode cGen) =>
            cGen.GenerateKotlinProxy;
    }

    public interface ICodeWriter
    {
        string PrintResult();
        ICodeWriter IncIndent();
        ICodeWriter DecIndent();
        ICodeWriter AddText(string text);
        ICodeWriter AddNewLine();
        ICodeWriter AddIndent();
    }
}