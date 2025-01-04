using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    class Interpreter : IVisitor<object>
    {
        public void Interpret(Expr expression)
        {
            try
            {
                object value = Evaluate(expression);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        private string Stringify(object value)
        {
            if (value == null) return "nil";
            if (value is double d)
            {
                string text = d.ToString(CultureInfo.InvariantCulture);
                if (text.EndsWith(".0"))
                    text = text[..^2];

                return text;
            }
            return value.ToString();
        }

        public object VisitBinaryExpr(Binary expr)
        {
            object left = Evaluate(expr.left);
            object right = Evaluate(expr.right);

            switch (expr.op.type)
            {
                case TokenType.MINUS:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left - (double)right;
                case TokenType.SLASH:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left * (double)right;
                case TokenType.PLUS:
                    if (left is double && right is double)
                        return (double)left + (double)right;
                    if (left is string && right is string)
                        return (string)left + (string)right;
                    throw new RuntimeError(expr.op, "Operands must be two numbers or two strings.");
                case TokenType.GREATER:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left <= (double)right;
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
            }

            // Unreachable
            return null;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            return a.Equals(b);
        }

        public object VisitGroupingExpr(Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        public object VisitLiteralExpr(Literal expr)
        {
            return expr.value;
        }

        public object VisitUnaryExpr(Unary expr)
        {
            object right = Evaluate(expr.right);
            switch (expr.op.type)
            {
                case TokenType.MINUS:
                    CheckNumberOperand(expr.op, right);
                    return -(double)right;
                case TokenType.BANG:
                    return !IsTruthy(right);
            }

            // Unreachable
            return null;
        }

        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(op, "Operand must be a number.");
        }

        private void CheckNumberOperand(Token op, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(op, "Operands must be numbers.");
        }

        private bool IsTruthy(object @object)
        {
            if (@object == null) return false;
            if (@object is bool b) return b;
            return true;
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }
    }
}
