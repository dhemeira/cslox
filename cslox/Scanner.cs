using System.Globalization;

namespace cslox
{
    class Scanner
    {
        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;
        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
        {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE }
        };

        public Scanner(string source)
        {
            this.source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        private bool IsAtEnd() => current >= source.Length;

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                // check if next char matches = to determine if it's a single char token
                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case '/':
                    if (Match('/'))
                        // go until we reach end of line or end of file
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    else
                        AddToken(TokenType.SLASH);
                    break;
                case '"': String(); break;
                case ' ':
                case '\r':
                case '\t':
                    // ignore whitespace
                    break;
                case '\n':
                    line++;
                    break;
                default:
                    if (char.IsDigit(c))
                        Number();
                    else if (char.IsLetter(c))
                        Identifier();
                    else
                        Lox.Error(line, "Unexpected character.");
                    break;
            }
        }

        private void Identifier()
        {
            while (char.IsLetterOrDigit(Peek())) Advance();

            string text = source[start..current];
            // check if it's a keyword
            if (keywords.TryGetValue(text, out TokenType type))
                AddToken(type);
            else
                AddToken(TokenType.IDENTIFIER);
        }

        private void Number()
        {
            while (char.IsDigit(Peek())) Advance();

            // look for fractional part
            if (Peek() == '.' && char.IsDigit(PeekNext()))
            {
                // consume the .
                Advance();
                while (char.IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER, double.Parse(source[start..current], CultureInfo.InvariantCulture));
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                // handle multi-line strings
                if (Peek() == '\n') line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(line, "Unterminated string.");
                return;
            }

            // closing "
            Advance();

            // trim surrounding quotes

            double value = double.Parse(source[start..current]);
            AddToken(TokenType.STRING, value);
        }


        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return source[current];
        }

        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;
            current++;
            return true;
        }

        private char Advance() => source[current++];

        private void AddToken(TokenType type) => AddToken(type, null);

        private void AddToken(TokenType type, object? literal)
        {
            string text = source[start..current];
            tokens.Add(new Token(type, text, literal, line));
        }
    }
}
