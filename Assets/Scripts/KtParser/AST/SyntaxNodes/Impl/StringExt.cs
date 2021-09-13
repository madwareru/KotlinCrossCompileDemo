namespace KtParser.AST.SyntaxNodes.Impl
{
    public static class StringExt
    {
        public static string ToUpperFirst(this string s) =>
            $"{s.Substring(0, 1).ToUpper()}{s.Substring(1)}";
        
        public static string ToLowerFirst(this string s) =>
            $"{s.Substring(0, 1).ToLower()}{s.Substring(1)}";
    }
}