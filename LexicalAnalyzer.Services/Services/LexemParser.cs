using LexicalAnalyzer.Domain.Models;
using System.Data;

namespace LexicalAnalyzer.Services.Services
{
    public class LexemParser
    {
        protected readonly Queue<LexemeType> inputLexemes;
        protected readonly IReadOnlyCollection<LexemeType> MathOperators = new List<LexemeType> { LexemeType.AddOp, LexemeType.DivideOp, LexemeType.MultiplyOp, LexemeType.SubtractOp };

        public LexemParser(IEnumerable<LexemeType> lexemes)
        {
            inputLexemes = new Queue<LexemeType>(lexemes);
        }

        public void ParseWhileStatement()
        {
            VerifyLexemeType(LexemeType.While);
            VerifyLexemeType(LexemeType.Lp);
            ParseBooleanExpression();
            VerifyLexemeType(LexemeType.Rp);
            SkipIf(LexemeType.Nl);
            ParseStatement();
            SkipIf(LexemeType.Nl);
        }

        public void ParseBooleanExpression()
        {
            ParseBooleanTerm();

            if (!inputLexemes.TryPeek(out var nextLexeme))
            {
                return;
            }

            if (nextLexeme is not LexemeType.RelOp)
            {
                return;
            }

            while (nextLexeme is LexemeType.RelOp)
            {
                GetNext();

                ParseBooleanTerm();

                if (!inputLexemes.TryPeek(out nextLexeme))
                {
                    return;
                }
            }

        }

        private void ParseBooleanTerm()
        {
            var lexeme = inputLexemes.Peek();
            switch (lexeme)
            {
                case LexemeType.StringLiteral:
                case LexemeType.Id:
                case LexemeType.True:
                case LexemeType.False:
                case LexemeType.Undefined:
                case LexemeType.Null:
                    GetNext();
                    return;
                case LexemeType.Number:
                    ParseMathExpression(false);
                    return;
                case LexemeType.Lp:
                    ParseBooleanExpression();
                    VerifyLexemeType(LexemeType.Rp);
                    return;
            }

            throw new InvalidExpressionException($"Invalid boolean statement. Unexpected lexeme: {lexeme}");
        }

        public void ParseStatement()
        {
            if (!inputLexemes.TryPeek(out var lexeme))
            {
                throw new InvalidExpressionException("Unfinished expression. Unexpected EOF");
            }

            switch (lexeme)
            {
                case LexemeType.Lb:
                    ParseBlockStatement();
                    return;
                case LexemeType.Id:
                case LexemeType.ArrElement:
                    ParseAssignmentStatement();
                    return;
                default:
                    throw new InvalidExpressionException("Unexpexcted lexeme in statement");
            }
        }

        private void ParseBlockStatement()
        {
            VerifyLexemeType(LexemeType.Lb);
            SkipIf(LexemeType.Nl);
            while (inputLexemes.TryPeek(out var lexeme) && lexeme != LexemeType.Rb)
            {
                ParseStatement();
            }
            VerifyLexemeType(LexemeType.Rb);
        }

        public void ParseAssignmentStatement()
        {
            VerifyLexemeType(GetNext(), LexemeType.Id, LexemeType.ArrElement);
            VerifyLexemeType(LexemeType.Assign);
            ParseMathExpression();
            ParseEndOfStatement();
        }

        public void ParseMathExpression(bool moveToNext = true)
        {
            ParseMathTerm(moveToNext);

            if (!inputLexemes.TryPeek(out var nextLexeme))
            {
                return;
            }

            if (!MathOperators.Contains(nextLexeme))
            {
                return;
            }

            while (MathOperators.Contains(nextLexeme))
            {
                GetNext();

                ParseMathTerm();

                if (!inputLexemes.TryPeek(out nextLexeme))
                {
                    return;
                }
            }
        }

        private void ParseMathTerm(bool moveToNext = true)
        {
            var lexeme = moveToNext ? GetNext() : inputLexemes.Peek();
            switch (lexeme)
            {
                case LexemeType.Number:
                case LexemeType.StringLiteral:
                case LexemeType.Id:
                case LexemeType.Undefined:
                case LexemeType.Null:
                    if (!moveToNext)
                    {
                        GetNext();
                    }
                    return;
                case LexemeType.Lp:
                    if (!moveToNext)
                    {
                        GetNext();
                    }
                    ParseMathExpression();
                    VerifyLexemeType(LexemeType.Rp);
                    return;
            }

            throw new InvalidExpressionException($"Invalid math statement. Unexpected lexeme: {lexeme}");
        }

        private void ParseEndOfStatement()
        {
            if (!inputLexemes.Any())
            {
                return;
            }

            VerifyLexemeType(GetNext(), LexemeType.Rb, LexemeType.Nl, LexemeType.Sc);
        }


        private void VerifyLexemeType(LexemeType expectedLexeme)
        {
            VerifyLexemeType(GetNext(), expectedLexeme);
        }

        private static void VerifyLexemeType(LexemeType actualLexeme, params LexemeType[] expectedLexemes)
        {
            if (expectedLexemes.All(l => l != actualLexeme))
            {
                throw new InvalidExpressionException($"Unexpected lexeme");
            }
        }

        private LexemeType GetNext()
        {
            if (!inputLexemes.TryDequeue(out var lexeme))
            {
                throw new InvalidExpressionException("Unfinished expression. Unexpected EOF");
            }
            return lexeme;
        }

        private void SkipIf(LexemeType lexemeToSkip)
        {
            if (inputLexemes.TryPeek(out var nextLexeme) && nextLexeme == lexemeToSkip)
            {
                GetNext();
            }
        }
    }
}
