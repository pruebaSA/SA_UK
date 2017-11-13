namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;

    internal sealed class TermExpr<T_Identifier> : BoolExpr<T_Identifier>, IEquatable<TermExpr<T_Identifier>>
    {
        private readonly IEqualityComparer<T_Identifier> _comparer;
        private readonly T_Identifier _identifier;

        internal TermExpr(T_Identifier identifier) : this(null, identifier)
        {
        }

        internal TermExpr(IEqualityComparer<T_Identifier> comparer, T_Identifier identifier)
        {
            this._identifier = identifier;
            if (comparer == null)
            {
                this._comparer = EqualityComparer<T_Identifier>.Default;
            }
            else
            {
                this._comparer = comparer;
            }
        }

        internal override T_Return Accept<T_Return>(Visitor<T_Identifier, T_Return> visitor) => 
            visitor.VisitTerm((TermExpr<T_Identifier>) this);

        public bool Equals(TermExpr<T_Identifier> other) => 
            this._comparer.Equals(this._identifier, other._identifier);

        public override bool Equals(object obj) => 
            this.Equals(obj as TermExpr<T_Identifier>);

        protected override bool EquivalentTypeEquals(BoolExpr<T_Identifier> other) => 
            this._comparer.Equals(this._identifier, ((TermExpr<T_Identifier>) other)._identifier);

        public override int GetHashCode() => 
            this._comparer.GetHashCode(this._identifier);

        internal override BoolExpr<T_Identifier> MakeNegated()
        {
            Literal<T_Identifier> literal2 = new Literal<T_Identifier>((TermExpr<T_Identifier>) this, true).MakeNegated();
            if (literal2.IsTermPositive)
            {
                return literal2.Term;
            }
            return new NotExpr<T_Identifier>(literal2.Term);
        }

        public override string ToString() => 
            StringUtil.FormatInvariant("{0}", new object[] { this._identifier });

        internal override System.Data.Common.Utils.Boolean.ExprType ExprType =>
            System.Data.Common.Utils.Boolean.ExprType.Term;

        internal T_Identifier Identifier =>
            this._identifier;
    }
}

