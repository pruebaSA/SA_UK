namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Text;

    internal class KnowledgeBase<T_Identifier>
    {
        private readonly ConversionContext<T_Identifier> _context;
        private readonly List<BoolExpr<T_Identifier>> _facts;
        private Vertex _knowledge;

        internal KnowledgeBase()
        {
            this._facts = new List<BoolExpr<T_Identifier>>();
            this._knowledge = Vertex.One;
            this._context = IdentifierService<T_Identifier>.Instance.CreateConversionContext();
        }

        internal void AddEquivalence(BoolExpr<T_Identifier> left, BoolExpr<T_Identifier> right)
        {
            this.AddFact(new Equivalence<T_Identifier>(left, right));
        }

        internal virtual void AddFact(BoolExpr<T_Identifier> fact)
        {
            this._facts.Add(fact);
            Converter<T_Identifier> converter = new Converter<T_Identifier>(fact, this._context);
            Vertex right = converter.Vertex;
            this._knowledge = this._context.Solver.And(this._knowledge, right);
        }

        internal void AddImplication(BoolExpr<T_Identifier> condition, BoolExpr<T_Identifier> implies)
        {
            this.AddFact(new Implication<T_Identifier>(condition, implies));
        }

        internal void AddKnowledgeBase(KnowledgeBase<T_Identifier> kb)
        {
            foreach (BoolExpr<T_Identifier> expr in kb._facts)
            {
                this.AddFact(expr);
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Facts:");
            foreach (BoolExpr<T_Identifier> expr in this._facts)
            {
                builder.Append("\t").AppendLine(expr.ToString());
            }
            return builder.ToString();
        }

        private class Equivalence : AndExpr<T_Identifier>
        {
            private BoolExpr<T_Identifier> _left;
            private BoolExpr<T_Identifier> _right;

            internal Equivalence(BoolExpr<T_Identifier> left, BoolExpr<T_Identifier> right) : base(new BoolExpr<T_Identifier>[] { new KnowledgeBase<T_Identifier>.Implication(left, right), new KnowledgeBase<T_Identifier>.Implication(right, left) })
            {
                this._left = left;
                this._right = right;
            }

            public override string ToString() => 
                StringUtil.FormatInvariant("{0} <--> {1}", new object[] { this._left, this._right });
        }

        private class Implication : OrExpr<T_Identifier>
        {
            private BoolExpr<T_Identifier> _condition;
            private BoolExpr<T_Identifier> _implies;

            internal Implication(BoolExpr<T_Identifier> condition, BoolExpr<T_Identifier> implies) : base(new BoolExpr<T_Identifier>[] { condition.MakeNegated(), implies })
            {
                this._condition = condition;
                this._implies = implies;
            }

            public override string ToString() => 
                StringUtil.FormatInvariant("{0} --> {1}", new object[] { this._condition, this._implies });
        }
    }
}

