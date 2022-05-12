namespace LexicalAnalyzer.Domain.Models
{
    public enum State
    {
        S,
        Num,
        Id,
        Op,
        F,
        Err,
        Literal,
        Assign,
        RelOp
    }
}
