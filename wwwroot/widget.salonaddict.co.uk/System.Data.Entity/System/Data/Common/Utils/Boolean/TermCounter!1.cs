namespace System.Data.Common.Utils.Boolean
{
    using System;

    internal class TermCounter<T_Identifier> : Visitor<T_Identifier, int>
    {
        private static readonly TermCounter<T_Identifier> s_instance;

        static TermCounter()
        {
            TermCounter<T_Identifier>.s_instance = new TermCounter<T_Identifier>();
        }

        internal static int CountTerms(BoolExpr<T_Identifier> expression) => 
            expression.Accept<int>(TermCounter<T_Identifier>.s_instance);

        internal override int VisitAnd(AndExpr<T_Identifier> expression) => 
            this.VisitTree(expression);

        internal override int VisitFalse(FalseExpr<T_Identifier> expression) => 
            0;

        internal override int VisitNot(NotExpr<T_Identifier> expression) => 
            expression.Child.Accept<int>(this);

        internal override int VisitOr(OrExpr<T_Identifier> expression) => 
            this.VisitTree(expression);

        internal override int VisitTerm(TermExpr<T_Identifier> expression) => 
            1;

        private int VisitTree(TreeExpr<T_Identifier> expression)
        {
            int num = 0;
            foreach (BoolExpr<T_Identifier> expr in expression.Children)
            {
                num += expr.Accept<int>(this);
            }
            return num;
        }

        internal override int VisitTrue(TrueExpr<T_Identifier> expression) => 
            0;
    }
}

