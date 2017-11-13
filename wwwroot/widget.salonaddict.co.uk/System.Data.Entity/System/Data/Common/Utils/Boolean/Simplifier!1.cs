namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;

    internal class Simplifier<T_Identifier> : BasicVisitor<T_Identifier>
    {
        internal static readonly Simplifier<T_Identifier> Instance;

        static Simplifier()
        {
            Simplifier<T_Identifier>.Instance = new Simplifier<T_Identifier>();
        }

        protected Simplifier()
        {
        }

        private BoolExpr<T_Identifier> SimplifyTree(TreeExpr<T_Identifier> tree)
        {
            bool flag = ExprType.And == tree.ExprType;
            List<BoolExpr<T_Identifier>> list = new List<BoolExpr<T_Identifier>>(tree.Children.Count);
            foreach (BoolExpr<T_Identifier> expr in tree.Children)
            {
                BoolExpr<T_Identifier> item = expr.Accept<BoolExpr<T_Identifier>>(this);
                if (item.ExprType == tree.ExprType)
                {
                    list.AddRange(((TreeExpr<T_Identifier>) item).Children);
                }
                else
                {
                    list.Add(item);
                }
            }
            Dictionary<BoolExpr<T_Identifier>, bool> dictionary = new Dictionary<BoolExpr<T_Identifier>, bool>(tree.Children.Count);
            List<BoolExpr<T_Identifier>> list2 = new List<BoolExpr<T_Identifier>>(tree.Children.Count);
            foreach (BoolExpr<T_Identifier> expr3 in list)
            {
                switch (expr3.ExprType)
                {
                    case ExprType.Not:
                    {
                        dictionary[((NotExpr<T_Identifier>) expr3).Child] = true;
                        continue;
                    }
                    case ExprType.True:
                        if (flag)
                        {
                            continue;
                        }
                        return TrueExpr<T_Identifier>.Value;

                    case ExprType.False:
                        if (!flag)
                        {
                            continue;
                        }
                        return FalseExpr<T_Identifier>.Value;
                }
                list2.Add(expr3);
            }
            List<BoolExpr<T_Identifier>> children = new List<BoolExpr<T_Identifier>>();
            foreach (BoolExpr<T_Identifier> expr4 in list2)
            {
                if (dictionary.ContainsKey(expr4))
                {
                    if (flag)
                    {
                        return FalseExpr<T_Identifier>.Value;
                    }
                    return TrueExpr<T_Identifier>.Value;
                }
                children.Add(expr4);
            }
            foreach (BoolExpr<T_Identifier> expr5 in dictionary.Keys)
            {
                children.Add(expr5.MakeNegated());
            }
            if (children.Count == 0)
            {
                if (flag)
                {
                    return TrueExpr<T_Identifier>.Value;
                }
                return FalseExpr<T_Identifier>.Value;
            }
            if (1 == children.Count)
            {
                return children[0];
            }
            if (flag)
            {
                return new AndExpr<T_Identifier>(children);
            }
            return new OrExpr<T_Identifier>(children);
        }

        internal override BoolExpr<T_Identifier> VisitAnd(AndExpr<T_Identifier> expression) => 
            this.SimplifyTree(expression);

        internal override BoolExpr<T_Identifier> VisitNot(NotExpr<T_Identifier> expression)
        {
            BoolExpr<T_Identifier> expr = expression.Child.Accept<BoolExpr<T_Identifier>>(this);
            switch (expr.ExprType)
            {
                case ExprType.Not:
                    return ((NotExpr<T_Identifier>) expr).Child;

                case ExprType.True:
                    return FalseExpr<T_Identifier>.Value;

                case ExprType.False:
                    return TrueExpr<T_Identifier>.Value;
            }
            return base.VisitNot(expression);
        }

        internal override BoolExpr<T_Identifier> VisitOr(OrExpr<T_Identifier> expression) => 
            this.SimplifyTree(expression);
    }
}

