using LexicalAnalyzer.Domain.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace LexicalAnalyzer.Services
{
    public static class AnalyzerService
    {
        public static IEnumerable<(LexemeType lexeme, string value)> Analyze(string str)
        {
            using var reader = new StringReader(str);
            return Analyze(reader);
        }

        private static IEnumerable<(LexemeType lexeme, string value)> Analyze(TextReader reader)
        {
            var result = new List<(LexemeType lexeme, string value)>();

            var (isEndOfString, currentChar) = TryReadNextChar(reader);
            void ReadNextChar() => (isEndOfString, currentChar) = TryReadNextChar(reader);

            var state = isEndOfString ? State.F : State.S;
            var buffer = new StringBuilder();

            void StateReset(LexemeType lexemeType)
            {
                result.Add((lexemeType, buffer.ToString()));
                state = State.S;
                buffer.Clear();
            }

            while (state != State.F)
            {
                switch (state)
                {
                    case State.S:
                        if (!IsWhitespaceOrEmpty(currentChar))
                        {
                            if (char.IsDigit(currentChar))
                            {
                                buffer.Append(currentChar);
                                state = State.Num;
                            }
                            else if (char.IsLetter(currentChar))
                            {
                                buffer.Append(currentChar);
                                state = State.Id;
                            }
                            else if (Regex.IsMatch(currentChar.ToString(), @"^[+\-/*]"))
                            {
                                buffer.Append(currentChar);
                                state = State.Op;
                            }
                            else if (currentChar == '"')
                            {
                                buffer.Append(currentChar);
                                state = State.Literal;
                            }
                            else if (currentChar == '=')
                            {
                                buffer.Append(currentChar);
                                state = State.Assign;
                            }
                            else if (currentChar == '>' || currentChar == '<' || currentChar == '!')
                            {
                                buffer.Append(currentChar);
                                state = State.RelOp;
                            }
                            else
                            {
                                var lexeme = currentChar switch
                                {
                                    '=' => LexemeType.Assign,
                                    ';' => LexemeType.Sc,
                                    '{' => LexemeType.Lb,
                                    '}' => LexemeType.Rb,
                                    '(' => LexemeType.Lp,
                                    ')' => LexemeType.Rp,
                                    '\n' => LexemeType.Nl,
                                    _ => LexemeType.InvalidLexeme
                                };
                                if (lexeme is LexemeType.InvalidLexeme)
                                {
                                    state = State.Err;
                                    break;
                                }
                                buffer.Append(currentChar);
                                StateReset(lexeme);
                            }
                        }
                        else if (isEndOfString)
                        {
                            state = State.F;
                        }
                        break;
                    case State.Num:
                        if (IsWhitespaceOrEmpty(currentChar))
                        {
                            StateReset(LexemeType.Number);
                            if (isEndOfString)
                            {
                                state = State.F;
                            }
                        }
                        else if (char.IsDigit(currentChar))
                        {
                            buffer.Append(currentChar);
                        }
                        else
                        {
                            StateReset(LexemeType.Number);
                            var lexeme = currentChar switch
                            {
                                '=' => LexemeType.Assign,
                                ';' => LexemeType.Sc,
                                '{' => LexemeType.Lb,
                                '}' => LexemeType.Rb,
                                '(' => LexemeType.Lp,
                                ')' => LexemeType.Rp,
                                '\n' => LexemeType.Nl,
                                _ => LexemeType.InvalidLexeme
                            };
                            if (lexeme is LexemeType.InvalidLexeme)
                            {
                                state = State.Err;
                                break;
                            }
                            buffer.Append(currentChar);
                            StateReset(lexeme);
                        }
                        break;
                    case State.Id:
                        if (IsWhitespaceOrEmpty(currentChar))
                        {
                            string token = buffer.ToString();
                            if (Regex.IsMatch(token, Patterns.VariableNamePattern))
                            {
                                StateReset(LexemeType.Id);
                            }
                            else
                            {
                                if (Patterns.KeywordLexemeTypes.TryGetValue(token, out LexemeType lexemeType))
                                {
                                    StateReset(lexemeType);
                                }
                                else
                                {
                                    state = State.Err;
                                }
                            }
                            if (isEndOfString)
                            {
                                state = State.F;
                            }
                        }
                        else if (char.IsDigit(currentChar))
                        {
                            buffer.Append(currentChar);
                        }
                        else if (char.IsLetter(currentChar))
                        {
                            buffer.Append(currentChar);
                        }
                        else if (char.IsLetter(currentChar))
                        {
                            buffer.Append(currentChar);
                        }
                        else
                        {
                            StateReset(LexemeType.Id);
                            var lexeme = currentChar switch
                            {
                                '=' => LexemeType.Assign,
                                ';' => LexemeType.Sc,
                                '{' => LexemeType.Lb,
                                '}' => LexemeType.Rb,
                                '(' => LexemeType.Lp,
                                ')' => LexemeType.Rp,
                                '\n' => LexemeType.Nl,
                                _ => LexemeType.InvalidLexeme
                            };
                            if (lexeme is LexemeType.InvalidLexeme)
                            {
                                state = State.Err;
                                break;
                            }
                            buffer.Append(currentChar);
                            StateReset(lexeme);
                        }
                        break;
                    case State.Op:
                        if (IsWhitespaceOrEmpty(currentChar))
                        {
                            string token = buffer.ToString();
                            StateReset(Patterns.KeywordLexemeTypes[token]);
                        }
                        break;
                    case State.Literal:
                        if (currentChar != '"')
                        {
                            buffer.Append(currentChar);
                        }
                        else
                        {
                            buffer.Append(currentChar);
                            StateReset(LexemeType.StringLiteral);
                        }
                        break;
                    case State.Assign:
                        if (IsWhitespaceOrEmpty(currentChar))
                        {
                            StateReset(LexemeType.Assign);
                            if (isEndOfString)
                            {
                                state = State.F;
                            }
                        }
                        else if (currentChar == '=')
                        {
                            buffer.Append(currentChar);
                            state = State.RelOp;
                        }
                        else if (Patterns.KeywordLexemeTypes.ContainsKey(buffer.ToString() + currentChar))
                        {
                            buffer.Append(currentChar);
                            state = State.RelOp;
                        }
                        else
                        {
                            state = State.Err;
                        }
                        break;
                    case State.RelOp:
                        if (IsWhitespaceOrEmpty(currentChar))
                        {
                            string token = buffer.ToString();
                            StateReset(Patterns.KeywordLexemeTypes[token]);
                            if (isEndOfString)
                            {
                                state = State.F;
                            }
                        }
                        else if (currentChar == '=' && Patterns.KeywordLexemeTypes.ContainsKey(buffer.ToString() + "="))
                        {
                            buffer.Append(currentChar);
                            state = State.RelOp;
                        }
                        else
                        {
                            state = State.Err;
                        }
                        break;
                    case State.F:
                        buffer.ToString();
                        StateReset(LexemeType.Assign);
                        break;
                    case State.Err:
                        throw new ArgumentException(
                            $"Invalid value reached. Character: {currentChar}. {(buffer.Length != 0 ? $"Statement: {buffer}" : string.Empty)}");
                    default:
                        throw new ArgumentOutOfRangeException(nameof(currentChar), currentChar, "Cannot handle the value");
                }
                ReadNextChar();
            }

            return result;
        }

        private static bool IsWhitespaceOrEmpty(char ch)
        {
            return ch == default || Regex.IsMatch(ch.ToString(), "^ |\t|\r", RegexOptions.Compiled);
        }

        private static (bool, char) TryReadNextChar(TextReader reader)
        {
            var nextValue = reader.Read();
            var isEnd = nextValue == -1;
            return (isEnd, isEnd ? default : (char)nextValue);
        }
    }
}