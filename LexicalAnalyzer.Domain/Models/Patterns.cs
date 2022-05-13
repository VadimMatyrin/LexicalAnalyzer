namespace LexicalAnalyzer.Domain.Models
{
    public static class Patterns
    {
        public static string VariableNamePattern = "^(?!(do|if|in|for|let|new|try|var|case|else|enum|eval|false|null|undefined|NaN|this|true|void|with|break|catch|class|const|super|throw|while|yield|delete|export|import|public|return|static|switch|typeof|default|extends|finally|package|private|continue|debugger|function|arguments|interface|protected|implements|instanceof)$)[a-zA-Z_$][0-9a-zA-Z_$]*$";
        public static string NumberPattern = @"^[\+\-]?\d*\.?\d*(?:[Ee][\+\-]?\d+)?$";

        public static readonly IReadOnlyDictionary<string, LexemeType> KeywordLexemeTypes = new Dictionary<string, LexemeType>
        {
            {"if", LexemeType.If},
            {"else", LexemeType.Else},
            {"undefined", LexemeType.Undefined},
            {"null", LexemeType.Null},
            {"true", LexemeType.True},
            {"false", LexemeType.False},
            {"+", LexemeType.AddOp},
            {"-", LexemeType.SubtractOp},
            {"*", LexemeType.MultiplyOp},
            {"/", LexemeType.DivideOp},
            {"=", LexemeType.Assign},
            {"==", LexemeType.RelOp},
            {"===", LexemeType.RelOp},
            {"!=", LexemeType.RelOp},
            {"!==", LexemeType.RelOp},
            {">=", LexemeType.RelOp},
            {"<=", LexemeType.RelOp},
            {"(", LexemeType.Lp},
            {")", LexemeType.Rp},
        };
    }
}
