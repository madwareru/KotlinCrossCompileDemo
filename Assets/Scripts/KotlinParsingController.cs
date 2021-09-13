using System;
using KtParser.AST;
using KtParser.Writers;

namespace DefaultNamespace
{
    public class KotlinParsingController
    {
        public readonly IInputOutputProvider InputOutputProvider;

        public KotlinParsingController(IInputOutputProvider inputOutputProvider)
        {
            InputOutputProvider = inputOutputProvider;
        }

        public void PrintObject()
        {
            var content = InputOutputProvider.InputField.text;
            SyntaxTree syntaxTree;
            try
            {
                syntaxTree = SyntaxTree.Parse(content, false);
            }
            catch (Exception ex)
            {
                InputOutputProvider.OutputField.text = ex.Message;
                return;
            }
            var result = CodeWriter
                .Create()
                .Do(syntaxTree.Syntax.GenerateCSharpObject())
                .PrintResult();
            InputOutputProvider.OutputField.text = result;
        }
        
        public void PrintProxy()
        {
            var content = InputOutputProvider.InputField.text;
            SyntaxTree syntaxTree;
            try
            {
                syntaxTree = SyntaxTree.Parse(content, true);
            }
            catch (Exception ex)
            {
                InputOutputProvider.OutputField.text = ex.Message;
                return;
            }
            var result = CodeWriter
                .Create()
                .Do(syntaxTree.Syntax.GenerateCSharpProxy())
                .PrintResult();
            InputOutputProvider.OutputField.text = result;
        }
    }
}