using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Token_Class
{
    // Keywords
    Int, Float, String,
    Read, Write,
    Repeat, Until,
    If, ElseIf, Else, Then,
    Return, Endl, Main, End,
    // Operators
    Plus, Minus, Multiply, Divide,
    Assign,
    LessT, GreaterT, Equal, NotEqual,
    And, Or,
    // Symbols
    LParanthesis, RParanthesis,
    LCurly, RCurly,
    Semicolon, Comma,
    // Literals
    Identifier,
    Number,
    StringValue,
    Constant,
    StringLiteral
}

namespace JASON_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {    ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.ElseIf);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("main", Token_Class.Main);
            Operators.Add("+", Token_Class.Plus);
            Operators.Add("-", Token_Class.Minus);
            Operators.Add("*", Token_Class.Multiply);
            Operators.Add("/", Token_Class.Divide);
            Operators.Add(":=", Token_Class.Assign);
            Operators.Add("<", Token_Class.LessT);
            Operators.Add(">", Token_Class.GreaterT);
            Operators.Add("=", Token_Class.Equal);
            Operators.Add("<>", Token_Class.NotEqual);
            Operators.Add("&&", Token_Class.And);
            Operators.Add("||", Token_Class.Or);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("{", Token_Class.LCurly);
            Operators.Add("}", Token_Class.RCurly);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
      
        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (i + 1 < SourceCode.Length)
                {
                    string twoChar = "" + SourceCode[i] + SourceCode[i + 1];
                    if (Operators.ContainsKey(twoChar))
                    {
                        FindTokenClass(twoChar);
                        i++;
                        continue;
                    }
                }

                if ((CurrentChar >= 'A' && CurrentChar <= 'Z') ||
                    (CurrentChar >= 'a' && CurrentChar <= 'z'))
                {
                    j = i + 1;
                    if (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        while ((CurrentChar >= 'A' && CurrentChar <= 'Z') ||
                               (CurrentChar >= 'a' && CurrentChar <= 'z') ||
                               (CurrentChar >= '0' && CurrentChar <= '9') ||
                               CurrentChar == '_')
                        {
                            CurrentLexeme += CurrentChar.ToString();
                            j++;
                            if (j < SourceCode.Length)
                                CurrentChar = SourceCode[j];
                            else break;
                        }
                    }

                    // Case 1: immediately followed by "
                    if (j < SourceCode.Length && SourceCode[j] == '"')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                        // consume everything until closing " or end of line
                        while (j < SourceCode.Length &&
                               SourceCode[j] != '"' &&
                               SourceCode[j] != '\n' &&
                               SourceCode[j] != '\r')
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        // consume closing " if found
                        if (j < SourceCode.Length && SourceCode[j] == '"')
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        Errors.Error_List.Add("Invalid string: " + CurrentLexeme);
                        i = j - 1;
                    }
                    // Case 2: immediately followed by other invalid character
                    else if (j < SourceCode.Length &&
                        SourceCode[j] != ' ' &&
                        SourceCode[j] != '\r' &&
                        SourceCode[j] != '\n' &&
                        SourceCode[j] != ';' &&
                        SourceCode[j] != ',' &&
                        SourceCode[j] != '(' &&
                        SourceCode[j] != ')' &&
                        SourceCode[j] != '+' &&
                        SourceCode[j] != '-' &&
                        SourceCode[j] != '*' &&
                        SourceCode[j] != '/' &&
                        SourceCode[j] != ':' &&
                        SourceCode[j] != '<' &&
                        SourceCode[j] != '>' &&
                        SourceCode[j] != '=' &&
                        SourceCode[j] != '{' &&
                        SourceCode[j] != '}')
                    {
                        while (j < SourceCode.Length &&
                               SourceCode[j] != ' ' &&
                               SourceCode[j] != '\r' &&
                               SourceCode[j] != '\n' &&
                               SourceCode[j] != ';' &&
                               SourceCode[j] != ',' &&
                               SourceCode[j] != '(' &&
                               SourceCode[j] != ')' &&
                               SourceCode[j] != '+' &&
                               SourceCode[j] != '-' &&
                               SourceCode[j] != '*' &&
                               SourceCode[j] != '/' &&
                               SourceCode[j] != ':' &&
                               SourceCode[j] != '<' &&
                               SourceCode[j] != '>' &&
                               SourceCode[j] != '=' &&
                               SourceCode[j] != '{' &&
                               SourceCode[j] != '}')
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        Errors.Error_List.Add("Unidentified Token " + CurrentLexeme);
                        i = j - 1;
                    }
                    // Case 3: valid
                    else
                    {
                        FindTokenClass(CurrentLexeme);
                        i = j - 1;
                    }
                }

                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    j = i + 1;
                    bool hasDot = false;

                    if (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        while (j < SourceCode.Length &&
                              ((CurrentChar >= '0' && CurrentChar <= '9') ||
                               (CurrentChar == '.' && !hasDot)))
                        {
                            if (CurrentChar == '.') hasDot = true;
                            CurrentLexeme += CurrentChar;
                            j++;
                            if (j < SourceCode.Length)
                                CurrentChar = SourceCode[j];
                            else break;
                        }
                    }
                    if (j < SourceCode.Length && ((SourceCode[j] >= 'A' && SourceCode[j] <= 'Z') ||(SourceCode[j] >= 'a' && SourceCode[j] <= 'z') ||
                         SourceCode[j] == '_'))
                    {
                        while (j < SourceCode.Length &&
                              ((SourceCode[j] >= 'A' && SourceCode[j] <= 'Z') ||
                               (SourceCode[j] >= 'a' && SourceCode[j] <= 'z') ||
                               (SourceCode[j] >= '0' && SourceCode[j] <= '9') ||
                               SourceCode[j] == '_'))
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }

                        Errors.Error_List.Add("Invalid identifier: " + CurrentLexeme);
                        i = j - 1;
                    }

                    if (j < SourceCode.Length && SourceCode[j] == '.')
                    {
                        while (j < SourceCode.Length &&
                              (SourceCode[j] == '.' || (SourceCode[j] >= '0' && SourceCode[j] <= '9')))
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        Errors.Error_List.Add("Invalid number: " + CurrentLexeme);
                        i = j - 1;
                    }
                    else if (hasDot && CurrentLexeme.EndsWith("."))
                    {
                        Errors.Error_List.Add("Invalid number: " + CurrentLexeme);
                        i = j - 1;
                    }
                    else
                    {
                        FindTokenClass(CurrentLexeme);
                        i = j - 1;
                    }
                }

                else if (CurrentChar == '/' && i + 1 < SourceCode.Length && SourceCode[i + 1] == '*')
                {
                    j = i + 2;
                    while (j < SourceCode.Length - 1 &&
                          !(SourceCode[j] == '*' && SourceCode[j + 1] == '/'))
                    {
                        j++;
                    }
                    i = j + 1;
                }

                else if (CurrentChar == '"')
                {
                    j = i + 1;
                    while (j < SourceCode.Length && SourceCode[j] != '"')
                    {
                        if (SourceCode[j] == '\n' || SourceCode[j] == '\r')
                        {
                            // build error from current line content
                            string errorLexeme = SourceCode.Substring(i, j - i);
                            // skip newline and consume next line too
                            j++;
                            while (j < SourceCode.Length &&
                                   SourceCode[j] != '\n' && SourceCode[j] != '\r')
                            {
                                errorLexeme += SourceCode[j];
                                j++;
                            }
                            Errors.Error_List.Add("Unterminated string: " + errorLexeme);
                            i = j - 1;
                            goto continueOuter;
                        }
                        if (SourceCode[j] == '\\' && j + 1 < SourceCode.Length &&
                           (SourceCode[j + 1] == '"' || SourceCode[j + 1] == '\\'))
                            j++;
                        j++;
                    }

                    if (j >= SourceCode.Length)
                    {
                        Errors.Error_List.Add("Unterminated string: " + SourceCode.Substring(i));
                        i = SourceCode.Length - 1;
                        goto continueOuter;
                    }
                    else
                    {
                        j++;
                        CurrentLexeme = SourceCode.Substring(i, j - i);
                        FindTokenClass(CurrentLexeme);
                        CurrentLexeme = CurrentLexeme.Replace("\\\\", "\\")
                                                     .Replace("\\\"", "\"");
                        Tokens[Tokens.Count - 1].lex = CurrentLexeme;
                        i = j - 1;
                    }
                }

                else
                {
                    if (CurrentChar == '_')
                    {
                        j = i + 1;
                        if (j < SourceCode.Length)
                        {
                            CurrentChar = SourceCode[j];
                            while ((CurrentChar >= 'A' && CurrentChar <= 'Z') ||
                                   (CurrentChar >= 'a' && CurrentChar <= 'z') ||
                                   (CurrentChar >= '0' && CurrentChar <= '9') ||
                                   CurrentChar == '_')
                            {
                                CurrentLexeme += CurrentChar.ToString();
                                j++;
                                if (j < SourceCode.Length)
                                    CurrentChar = SourceCode[j];
                                else break;
                            }
                        }
                        Errors.Error_List.Add("Invalid identifier: " + CurrentLexeme);
                        i = j - 1;
                    }
                    else
                    {
                        FindTokenClass(CurrentLexeme);
                    }
                }

            continueOuter:;
            }

            JASON_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;

            if (ReservedWords.ContainsKey(Lex))
            {
                TC = ReservedWords[Lex];
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            else if (isIdentifier(Lex))
            {
                TC = Token_Class.Idenifier;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            else if (Operators.ContainsKey(Lex))
            {
                TC = Operators[Lex];
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            else if (isConstant(Lex))
            {
                TC = Token_Class.Constant;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            else if (isStringLiteral(Lex))
            {
                TC = Token_Class.StringLiteral;
                Tok.token_type = TC;
                Tokens.Add(Tok);
            }
            else
            {
                Errors.Error_List.Add("Unidentified Token " + Lex);
            }
        }

        bool isIdentifier(string lex)
        {
            Regex regex = new Regex(@"^[A-Za-z][A-Za-z0-9_]*$", RegexOptions.Compiled);
            return regex.IsMatch(lex);
        }

        bool isConstant(string lex)
        {
            Regex regex = new Regex(@"^[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled);
            return regex.IsMatch(lex);
        }

        bool isStringLiteral(string lex)
        {
            Regex regex = new Regex(@"^""([^""\\]|\\.)*""$", RegexOptions.Compiled);
            return regex.IsMatch(lex);
        }
    }
}
