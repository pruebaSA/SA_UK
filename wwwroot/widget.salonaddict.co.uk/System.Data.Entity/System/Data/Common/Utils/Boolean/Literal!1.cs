namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Data.Common.Utils;

    internal sealed class Literal<T_Identifier> : NormalFormNode<T_Identifier>, IEquatable<Literal<T_Identifier>>
    {
        private readonly bool _isTermPositive;
        private readonly TermExpr<T_Identifier> _term;

        internal Literal(TermExpr<T_Identifier> term, bool isTermPositive) : base(isTermPositive ? ((BoolExpr<T_Identifier>) term) : ((BoolExpr<T_Identifier>) new NotExpr<T_Identifier>(term)))
        {
            this._term = term;
            this._isTermPositive = isTermPositive;
        }

        public bool Equals(Literal<T_Identifier> other) => 
            (((other != null) && (other._isTermPositive == this._isTermPositive)) && other._term.Equals(this._term));

        public override bool Equals(object obj) => 
            this.Equals(obj as Literal<T_Identifier>);

        public override int GetHashCode() => 
            this._term.GetHashCode();

        internal Literal<T_Identifier> MakeNegated() => 
            IdentifierService<T_Identifier>.Instance.NegateLiteral((Literal<T_Identifier>) this);

        public override string ToString() => 
            StringUtil.FormatInvariant("{0}{1}", new object[] { this._isTermPositive ? string.Empty : "!", this._term });

        internal bool IsTermPositive =>
            this._isTermPositive;

        internal TermExpr<T_Identifier> Term =>
            this._term;
    }
}

