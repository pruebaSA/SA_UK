namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Linq;
    using System.Text;

    internal abstract class Sentence<T_Identifier, T_Clause> : NormalFormNode<T_Identifier> where T_Clause: Clause<T_Identifier>, IEquatable<T_Clause>
    {
        private readonly System.Data.Common.Utils.Set<T_Clause> _clauses;

        protected Sentence(System.Data.Common.Utils.Set<T_Clause> clauses, ExprType treeType) : base(Sentence<T_Identifier, T_Clause>.ConvertClausesToExpr(clauses, treeType))
        {
            this._clauses = clauses.AsReadOnly();
        }

        private static BoolExpr<T_Identifier> ConvertClausesToExpr(System.Data.Common.Utils.Set<T_Clause> clauses, ExprType treeType)
        {
            bool flag = ExprType.And == treeType;
            IEnumerable<BoolExpr<T_Identifier>> children = clauses.Select<T_Clause, BoolExpr<T_Identifier>>(new Func<T_Clause, BoolExpr<T_Identifier>>(NormalFormNode<T_Identifier>.ExprSelector<T_Clause>));
            if (flag)
            {
                return new AndExpr<T_Identifier>(children);
            }
            return new OrExpr<T_Identifier>(children);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Sentence{");
            builder.Append(this._clauses);
            return builder.Append("}").ToString();
        }
    }
}

