#region

using System;

#endregion

namespace BasicSharp
{
    public enum ValueType
    {
        Real, // it's double
        String
    }

    public struct Value
    {
        public static readonly Value Zero = new(0);
        public ValueType Type { get; set; }

        public double Real { get; set; }
        public string String { get; set; }

        public Value(double real) : this()
        {
            Type = ValueType.Real;
            Real = real;
        }

        public Value(string str)
            : this()
        {
            Type = ValueType.String;
            String = str;
        }

        public Value Convert(ValueType type)
        {
            if (Type != type)
                switch (type)
                {
                    case ValueType.Real:
                        Real = double.Parse(String);
                        Type = ValueType.Real;
                        break;
                    case ValueType.String:
                        String = Real.ToString();
                        Type = ValueType.String;
                        break;
                }

            return this;
        }

        public Value UnaryOp(Token tok)
        {
            if (Type != ValueType.Real) throw new Exception("Can only do unary operations on numbers.");

            switch (tok)
            {
                case Token.Plus: return this;
                case Token.Minus: return new Value(-Real);
                case Token.Not: return new Value(Real == 0 ? 1 : 0);
            }

            throw new Exception("Unknown unary operator.");
        }

        public Value BinOp(Value b, Token tok)
        {
            var a = this;
            if (a.Type != b.Type)
            {
                // promote one value to higher type
                if (a.Type > b.Type)
                    b = b.Convert(a.Type);
                else
                    a = a.Convert(b.Type);
            }

            if (tok == Token.Plus)
            {
                if (a.Type == ValueType.Real)
                    return new Value(a.Real + b.Real);
                return new Value(a.String + b.String);
            }

            if (tok == Token.Equal)
            {
                if (a.Type == ValueType.Real)
                    return new Value(a.Real == b.Real ? 1 : 0);
                return new Value(a.String == b.String ? 1 : 0);
            }

            if (tok == Token.NotEqual)
            {
                if (a.Type == ValueType.Real)
                    return new Value(a.Real == b.Real ? 0 : 1);
                return new Value(a.String == b.String ? 0 : 1);
            }

            if (a.Type == ValueType.String)
                throw new Exception("Cannot do binop on strings(except +).");

            switch (tok)
            {
                case Token.Minus: return new Value(a.Real - b.Real);
                case Token.Asterisk: return new Value(a.Real * b.Real);
                case Token.Slash: return new Value(a.Real / b.Real);
                case Token.Caret: return new Value(Math.Pow(a.Real, b.Real));
                case Token.Less: return new Value(a.Real < b.Real ? 1 : 0);
                case Token.More: return new Value(a.Real > b.Real ? 1 : 0);
                case Token.LessEqual: return new Value(a.Real <= b.Real ? 1 : 0);
                case Token.MoreEqual: return new Value(a.Real >= b.Real ? 1 : 0);
                case Token.And: return new Value(a.Real != 0 && b.Real != 0 ? 1 : 0);
                case Token.Or: return new Value(a.Real != 0 || b.Real != 0 ? 1 : 0);
            }

            throw new Exception("Unknown binary operator.");
        }

        public override string ToString()
        {
            if (Type == ValueType.Real)
                return Real.ToString();
            return String;
        }
    }
}