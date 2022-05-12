namespace LexicalAnalyzer.Domain.Models
{
    public enum LexemeType
    {
        Id,
        Lp, // left parenthesis
        Rp, // right parenthesis
        Lb, // left bracket
        Rb, // right bracket
        Sc, // semi column
        Nl, // new line
        KeyWord,
        Assign,
        RelOp,
        If,
        Else,
        AddOp,
        SubtractOp,
        DivideOp,
        MultiplyOp,
        Number,
        Negate,
        StringLiteral,
        Null,
        Undefined,
        InvalidLexeme,
        True,
        False
    }
}
