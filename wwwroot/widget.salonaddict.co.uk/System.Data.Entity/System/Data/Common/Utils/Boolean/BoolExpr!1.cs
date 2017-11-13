namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    internal abstract class BoolExpr<T_Identifier> : IEquatable<BoolExpr<T_Identifier>>
    {
        protected BoolExpr()
        {
        }

        internal abstract T_Return Accept<T_Return>(Visitor<T_Identifier, T_Return> visitor);
        private static BoolExpr<T_Identifier> ChooseCandidate(params BoolExpr<T_Identifier>[] candidates)
        {
            int num = 0;
            int num2 = 0;
            BoolExpr<T_Identifier> expr = null;
            foreach (BoolExpr<T_Identifier> expr2 in candidates)
            {
                BoolExpr<T_Identifier> expr3 = expr2.Simplify();
                int num3 = expr3.GetTerms().Distinct<TermExpr<T_Identifier>>().Count<TermExpr<T_Identifier>>();
                int num4 = expr3.CountTerms();
                if (((expr == null) || (num3 < num)) || ((num3 == num) && (num4 < num2)))
                {
                    expr = expr3;
                    num = num3;
                    num2 = num4;
                }
            }
            return expr;
        }

        internal int CountTerms() => 
            TermCounter<T_Identifier>.CountTerms((BoolExpr<T_Identifier>) this);

        public bool Equals(BoolExpr<T_Identifier> other) => 
            (((other != null) && (this.ExprType == other.ExprType)) && this.EquivalentTypeEquals(other));

        protected abstract bool EquivalentTypeEquals(BoolExpr<T_Identifier> other);
        internal BoolExpr<T_Identifier> ExpensiveSimplify(out Converter<T_Identifier> converter)
        {
            ConversionContext<T_Identifier> context = IdentifierService<T_Identifier>.Instance.CreateConversionContext();
            converter = new Converter<T_Identifier>((BoolExpr<T_Identifier>) this, context);
            if (converter.Vertex.IsOne())
            {
                return TrueExpr<T_Identifier>.Value;
            }
            if (converter.Vertex.IsZero())
            {
                return FalseExpr<T_Identifier>.Value;
            }
            return BoolExpr<T_Identifier>.ChooseCandidate(new BoolExpr<T_Identifier>[] { this, converter.Cnf.Expr, converter.Dnf.Expr });
        }

        internal List<TermExpr<T_Identifier>> GetTerms() => 
            LeafVisitor<T_Identifier>.GetTerms((BoolExpr<T_Identifier>) this);

        internal virtual BoolExpr<T_Identifier> MakeNegated() => 
            new NotExpr<T_Identifier>((BoolExpr<T_Identifier>) this);

        public static implicit operator BoolExpr<T_Identifier>(T_Identifier value) => 
            new TermExpr<T_Identifier>(value);

        internal BoolExpr<T_Identifier> Simplify() => 
            IdentifierService<T_Identifier>.Instance.LocalSimplify((BoolExpr<T_Identifier>) this);

        public override string ToString() => 
            this.ExprType.ToString();

        internal abstract System.Data.Common.Utils.Boolean.ExprType ExprType { get; }
    }
}

