namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class BuiltInExpr : Expr
    {
        private ExprList<Expr> _argList;
        private BuiltInKind _kind;
        private string _name;

        private BuiltInExpr(BuiltInKind kind, string name)
        {
            this._argList = new ExprList<Expr>();
            this._kind = kind;
            this._name = name.ToUpperInvariant();
        }

        internal BuiltInExpr(BuiltInKind kind, string name, Expr arg1) : this(kind, name)
        {
            this._argList.Add(arg1);
        }

        internal BuiltInExpr(BuiltInKind kind, string name, Expr arg1, Expr arg2) : this(kind, name, arg1)
        {
            this._argList.Add(arg2);
        }

        internal BuiltInExpr(BuiltInKind kind, string name, Expr arg1, Expr arg2, Expr arg3) : this(kind, name, arg1, arg2)
        {
            this._argList.Add(arg3);
        }

        internal BuiltInExpr(BuiltInKind kind, string name, Expr arg1, Expr arg2, Expr arg3, Expr arg4) : this(kind, name, arg1, arg2, arg3)
        {
            this._argList.Add(arg4);
        }

        internal Expr Arg1
        {
            get
            {
                if (this._argList.Count < 1)
                {
                    return null;
                }
                return this._argList[0];
            }
        }

        internal Expr Arg2
        {
            get
            {
                if (this._argList.Count < 2)
                {
                    return null;
                }
                return this._argList[1];
            }
        }

        internal int ArgCount =>
            this.ArgList.Count;

        internal ExprList<Expr> ArgList =>
            this._argList;

        internal BuiltInKind Kind
        {
            get => 
                this._kind;
            set
            {
                this._kind = value;
            }
        }

        internal string Name =>
            this._name;
    }
}

