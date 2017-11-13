namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class DotExpr : Expr
    {
        private string _fullName;
        private System.Data.Common.EntitySql.Identifier _identifier;
        private string[] _identifierNames;
        private bool _isDottedIdentifier;
        private Expr _leftExpr;
        private Expr _leftmostExpr;
        private int _length;
        private bool _wasDotIdComputed;

        internal DotExpr(Expr expr, System.Data.Common.EntitySql.Identifier id)
        {
            this._leftExpr = expr;
            this._identifier = id;
        }

        private void CheckIfDotIdentifier()
        {
            if (!this._wasDotIdComputed)
            {
                this._wasDotIdComputed = true;
                this._length = 0;
                this._isDottedIdentifier = false;
                Expr left = this;
                while (left is DotExpr)
                {
                    this._length++;
                    left = ((DotExpr) left).Left;
                }
                if (left is System.Data.Common.EntitySql.Identifier)
                {
                    this._isDottedIdentifier = true;
                    this._length++;
                }
                else
                {
                    this._leftmostExpr = left;
                }
                this._identifierNames = new string[this._length];
                int index = this._length - 1;
                left = this;
                while (left is DotExpr)
                {
                    DotExpr expr2 = (DotExpr) left;
                    this._identifierNames[index--] = expr2.Identifier.Name;
                    left = expr2.Left;
                }
                if ((this._leftmostExpr == null) && (left != null))
                {
                    this._identifierNames[index] = ((System.Data.Common.EntitySql.Identifier) left).Name;
                }
                this._fullName = string.Join(".", this._identifierNames);
            }
        }

        internal string FullName
        {
            get
            {
                this.CheckIfDotIdentifier();
                return this._fullName;
            }
        }

        internal System.Data.Common.EntitySql.Identifier Identifier =>
            this._identifier;

        internal bool IsDottedIdentifier
        {
            get
            {
                this.CheckIfDotIdentifier();
                return this._isDottedIdentifier;
            }
        }

        internal Expr Left =>
            this._leftExpr;

        internal Expr LeftMostExpression
        {
            get
            {
                this.CheckIfDotIdentifier();
                return this._leftmostExpr;
            }
        }

        internal int Length
        {
            get
            {
                this.CheckIfDotIdentifier();
                return this._length;
            }
        }

        internal string[] Names
        {
            get
            {
                this.CheckIfDotIdentifier();
                return this._identifierNames;
            }
        }
    }
}

