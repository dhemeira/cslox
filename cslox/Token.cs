﻿namespace cslox
{
    public class Token
    {
        public readonly TokenType type;
        public readonly string lexeme;
        public readonly object? literal;
        public readonly int line;

        public Token(TokenType type, string lexeme, object? literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public override string ToString()
        {
            return $"{type} {lexeme} {literal}";
        }
    }
}
