namespace System.Data.Objects.ELinq
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class LinqMaximalSubtreeNominator : System.Linq.Expressions.ExpressionVisitor
    {
        private readonly HashSet<Expression> _candidates;
        private bool _cannotBeNominated;
        private readonly Func<Expression, bool> _shouldBeNominatedDelegate;

        private LinqMaximalSubtreeNominator(HashSet<Expression> candidates, Func<Expression, bool> shouldBeNominatedDelegate)
        {
            this._candidates = candidates;
            this._shouldBeNominatedDelegate = shouldBeNominatedDelegate;
        }

        internal static HashSet<Expression> FindMaximalSubtrees(Expression expression, Func<Expression, bool> shouldBeNominatedDelegate) => 
            MaximalSubtreeVisitor.FindMaximalSubtrees(Nominate(expression, new HashSet<Expression>(), shouldBeNominatedDelegate), expression);

        internal static HashSet<Expression> Nominate(Expression expression, HashSet<Expression> candidates, Func<Expression, bool> shouldBeNominatedDelegate)
        {
            LinqMaximalSubtreeNominator nominator = new LinqMaximalSubtreeNominator(candidates, shouldBeNominatedDelegate);
            nominator.Visit(expression);
            return nominator._candidates;
        }

        internal override Expression Visit(Expression exp)
        {
            if (exp != null)
            {
                bool flag = this._cannotBeNominated;
                this._cannotBeNominated = false;
                base.Visit(exp);
                if (!this._cannotBeNominated)
                {
                    if (this._shouldBeNominatedDelegate(exp))
                    {
                        this._candidates.Add(exp);
                    }
                    else
                    {
                        this._cannotBeNominated = true;
                    }
                }
                this._cannotBeNominated |= flag;
            }
            return exp;
        }

        private sealed class MaximalSubtreeVisitor : System.Linq.Expressions.ExpressionVisitor
        {
            private readonly HashSet<Expression> _nominees;
            private readonly HashSet<Expression> _subtrees;

            private MaximalSubtreeVisitor(HashSet<Expression> nominees)
            {
                this._nominees = nominees;
                this._subtrees = new HashSet<Expression>();
            }

            internal static HashSet<Expression> FindMaximalSubtrees(HashSet<Expression> nominees, Expression query)
            {
                LinqMaximalSubtreeNominator.MaximalSubtreeVisitor visitor = new LinqMaximalSubtreeNominator.MaximalSubtreeVisitor(nominees);
                visitor.Visit(query);
                return visitor._subtrees;
            }

            internal override Expression Visit(Expression exp)
            {
                if ((exp != null) && this._nominees.Contains(exp))
                {
                    this._subtrees.Add(exp);
                    return exp;
                }
                return base.Visit(exp);
            }
        }
    }
}

