namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Linq;
    using System.Text;

    internal abstract class Clause<T_Identifier> : NormalFormNode<T_Identifier>
    {
        private readonly int _hashCode;
        private readonly System.Data.Common.Utils.Set<Literal<T_Identifier>> _literals;

        protected Clause(System.Data.Common.Utils.Set<Literal<T_Identifier>> literals, ExprType treeType) : base(Clause<T_Identifier>.ConvertLiteralsToExpr(literals, treeType))
        {
            this._literals = literals.AsReadOnly();
            this._hashCode = this._literals.GetElementsHashCode();
        }

        private static BoolExpr<T_Identifier> ConvertLiteralsToExpr(System.Data.Common.Utils.Set<Literal<T_Identifier>> literals, ExprType treeType)
        {
            bool flag = ExprType.And == treeType;
            IEnumerable<BoolExpr<T_Identifier>> children = literals.Select<Literal<T_Identifier>, BoolExpr<T_Identifier>>(new Func<Literal<T_Identifier>, BoolExpr<T_Identifier>>(Clause<T_Identifier>.ConvertLiteralToExpression));
            if (flag)
            {
                return new AndExpr<T_Identifier>(children);
            }
            return new OrExpr<T_Identifier>(children);
        }

        private static BoolExpr<T_Identifier> ConvertLiteralToExpression(Literal<T_Identifier> literal) => 
            literal.Expr;

        public override bool Equals(object obj) => 
            base.Equals(obj);

        public override int GetHashCode() => 
            this._hashCode;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Clause{");
            builder.Append(this._literals);
            return builder.Append("}").ToString();
        }

        internal System.Data.Common.Utils.Set<Literal<T_Identifier>> Literals =>
            this._literals;
    }
}

