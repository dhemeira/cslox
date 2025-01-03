namespace cslox
{
    class Parser
    {
        private class ParseError : Exception { }

        private readonly List<Token> tokens;
        private int current = 0;
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expr Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParseError)
            {
                return null;
            }
        }

        private Expr Expression() => Equality();

        // Parse left-associative binary operators
        private Expr LeftAssociativeBinary(Func<Expr> higherPrecedence, params TokenType[] operators)
        {
            Expr expr = higherPrecedence();
            // Parse the rest of the comparison operators
            while (Match(operators))
            {
                // The operator we just matched
                Token op = Previous();
                Expr right = higherPrecedence();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        private Expr Equality() => LeftAssociativeBinary(Comparison, TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL);

        private Expr Comparison() => LeftAssociativeBinary(Term, TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL);

        private Expr Term() => LeftAssociativeBinary(Factor, TokenType.MINUS, TokenType.PLUS);

        private Expr Factor() => LeftAssociativeBinary(Unary, TokenType.SLASH, TokenType.STAR);

        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token op = Previous();
                Expr right = Unary();
                return new Unary(op, right);
            }
            return Primary();
        }

        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new Literal(false);
            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.NIL)) return new Literal(null);
            if (Match(TokenType.NUMBER, TokenType.STRING)) return new Literal(Previous().literal);
            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type))
                return Advance();
            throw Error(Peek(), message);
        }

        private static ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().type == TokenType.SEMICOLON) return;

                switch (Peek().type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }

        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd())
                return false;
            return Peek().type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd())
                current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().type == TokenType.EOF;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }
    }
}
