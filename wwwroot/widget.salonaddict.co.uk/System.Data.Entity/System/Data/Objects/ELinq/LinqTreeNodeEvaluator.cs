namespace System.Data.Objects.ELinq
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class LinqTreeNodeEvaluator : System.Linq.Expressions.ExpressionVisitor
    {
        private HashSet<Expression> _nodesToEvaluate;

        private LinqTreeNodeEvaluator(HashSet<Expression> nodesToEvaluate)
        {
            this._nodesToEvaluate = nodesToEvaluate;
        }

        internal static Expression Evaluate(Expression expression, HashSet<Expression> nodesToEvaluate)
        {
            if (nodesToEvaluate.Count == 0)
            {
                return expression;
            }
            LinqTreeNodeEvaluator evaluator = new LinqTreeNodeEvaluator(nodesToEvaluate);
            return evaluator.Visit(expression);
        }

        internal static Expression EvaluateClosuresAndClientEvalNodes(Expression expression) => 
            Evaluate(expression, LinqMaximalSubtreeNominator.Nominate(expression, new HashSet<Expression>(), delegate (Expression e) {
                if (!ExpressionEvaluator.IsExpressionNodeAClosure(e))
                {
                    return ExpressionEvaluator.IsExpressionNodeClientEvaluatable(e);
                }
                return true;
            }));

        internal override Expression Visit(Expression exp)
        {
            if (((exp != null) && (exp.NodeType != ExpressionType.Constant)) && this._nodesToEvaluate.Contains(exp))
            {
                return Expression.Constant(ExpressionEvaluator.EvaluateExpression(exp), exp.Type);
            }
            return base.Visit(exp);
        }
    }
}

