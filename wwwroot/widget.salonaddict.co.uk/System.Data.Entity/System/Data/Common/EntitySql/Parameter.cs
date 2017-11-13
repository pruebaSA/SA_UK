namespace System.Data.Common.EntitySql
{
    using System;
    using System.Data;
    using System.Data.Entity;

    internal sealed class Parameter : Expr
    {
        private string _name;

        internal Parameter(string parameterName, string query, int inputPos) : base(query, inputPos)
        {
            this._name = parameterName.Substring(1);
            if (this._name.StartsWith("_", StringComparison.OrdinalIgnoreCase) || char.IsDigit(this._name, 0))
            {
                throw EntityUtil.EntitySqlError(base.ErrCtx, Strings.InvalidParameterFormat(this._name));
            }
        }

        internal string Name =>
            this._name;
    }
}

