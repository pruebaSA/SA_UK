namespace System.Data.Common.EntitySql
{
    using System;
    using System.Data;
    using System.Data.Entity;

    internal sealed class Identifier : Expr
    {
        private bool _isEscaped;
        private string _name;

        internal Identifier(string symbol, bool isEscaped, string query, int inputPos) : base(query, inputPos)
        {
            this.ValidateIdentifier(symbol, isEscaped);
            this._name = symbol;
            this._isEscaped = isEscaped;
        }

        private void ValidateIdentifier(string symbol, bool isEscaped)
        {
            if (isEscaped)
            {
                if ((symbol[0] != '[') || (symbol[symbol.Length - 1] != ']'))
                {
                    throw EntityUtil.EntitySqlError(base.ErrCtx, Strings.InvalidEscapedIdentifier(symbol));
                }
            }
            else
            {
                if (((symbol.Length > 1) && (symbol[symbol.Length - 2] == '[')) && (symbol[symbol.Length - 1] == ']'))
                {
                    symbol = symbol.Substring(0, symbol.Length - 2);
                }
                bool isIdentifierASCII = true;
                if (!CqlLexer.IsLetterOrDigitOrUnderscore(symbol, out isIdentifierASCII))
                {
                    if (isIdentifierASCII)
                    {
                        throw EntityUtil.EntitySqlError(base.ErrCtx, Strings.InvalidSimpleIdentifier(symbol));
                    }
                    throw EntityUtil.EntitySqlError(base.ErrCtx, Strings.InvalidSimpleIdentifierNonASCII(symbol));
                }
            }
        }

        internal bool IsEscaped =>
            this._isEscaped;

        internal string Name
        {
            get
            {
                if (!this.IsEscaped)
                {
                    return this._name;
                }
                return this._name.Substring(1, this._name.Length - 2);
            }
        }

        internal string OriginalName =>
            this._name;
    }
}

