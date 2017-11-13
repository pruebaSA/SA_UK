namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data.Linq.Provider;
    using System.Linq.Expressions;

    internal static class Funcletizer
    {
        internal static Expression Funcletize(Expression expression) => 
            new Localizer(new LocalMapper().MapLocals(expression)).Localize(expression);

        private class DependenceChecker : System.Data.Linq.SqlClient.ExpressionVisitor
        {
            private HashSet<ParameterExpression> inScope = new HashSet<ParameterExpression>();
            private bool isIndependent = true;

            public static bool IsIndependent(Expression expression)
            {
                Funcletizer.DependenceChecker checker = new Funcletizer.DependenceChecker();
                checker.Visit(expression);
                return checker.isIndependent;
            }

            internal override Expression VisitLambda(LambdaExpression lambda)
            {
                foreach (ParameterExpression expression in lambda.Parameters)
                {
                    this.inScope.Add(expression);
                }
                return base.VisitLambda(lambda);
            }

            internal override Expression VisitParameter(ParameterExpression p)
            {
                this.isIndependent &= this.inScope.Contains(p);
                return p;
            }
        }

        private class Localizer : System.Data.Linq.SqlClient.ExpressionVisitor
        {
            private Dictionary<Expression, bool> locals;

            internal Localizer(Dictionary<Expression, bool> locals)
            {
                this.locals = locals;
            }

            internal Expression Localize(Expression expression) => 
                this.Visit(expression);

            private static Expression MakeLocal(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                {
                    return e;
                }
                if ((e.NodeType == ExpressionType.Convert) || (e.NodeType == ExpressionType.ConvertChecked))
                {
                    UnaryExpression expression = (UnaryExpression) e;
                    if (expression.Type == typeof(object))
                    {
                        Expression expression2 = MakeLocal(expression.Operand);
                        if (e.NodeType != ExpressionType.Convert)
                        {
                            return Expression.ConvertChecked(expression2, e.Type);
                        }
                        return Expression.Convert(expression2, e.Type);
                    }
                    if (expression.Operand.NodeType == ExpressionType.Constant)
                    {
                        ConstantExpression operand = (ConstantExpression) expression.Operand;
                        if (operand.Value == null)
                        {
                            return Expression.Constant(null, expression.Type);
                        }
                    }
                }
                return Expression.Invoke(Expression.Constant(Expression.Lambda(e, new ParameterExpression[0]).Compile()), new Expression[0]);
            }

            internal override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }
                if (this.locals.ContainsKey(exp))
                {
                    return MakeLocal(exp);
                }
                if (exp.NodeType == ((ExpressionType) 0x7d0))
                {
                    return exp;
                }
                return base.Visit(exp);
            }
        }

        private class LocalMapper : System.Data.Linq.SqlClient.ExpressionVisitor
        {
            private bool isRemote;
            private Dictionary<Expression, bool> locals;

            internal Dictionary<Expression, bool> MapLocals(Expression expression)
            {
                this.locals = new Dictionary<Expression, bool>();
                this.isRemote = false;
                this.Visit(expression);
                return this.locals;
            }

            internal override Expression Visit(Expression expression)
            {
                if (expression == null)
                {
                    return null;
                }
                bool isRemote = this.isRemote;
                ExpressionType nodeType = expression.NodeType;
                if (nodeType != ExpressionType.Constant)
                {
                    if (nodeType == ((ExpressionType) 0x7d0))
                    {
                        return expression;
                    }
                    this.isRemote = false;
                    base.Visit(expression);
                    if ((!this.isRemote && (expression.NodeType != ExpressionType.Lambda)) && ((expression.NodeType != ExpressionType.Quote) && Funcletizer.DependenceChecker.IsIndependent(expression)))
                    {
                        this.locals[expression] = true;
                    }
                }
                if (typeof(ITable).IsAssignableFrom(expression.Type) || typeof(DataContext).IsAssignableFrom(expression.Type))
                {
                    this.isRemote = true;
                }
                this.isRemote |= isRemote;
                return expression;
            }

            internal override Expression VisitMemberAccess(MemberExpression m)
            {
                base.VisitMemberAccess(m);
                this.isRemote |= (m.Expression != null) && typeof(ITable).IsAssignableFrom(m.Expression.Type);
                return m;
            }

            internal override Expression VisitMethodCall(MethodCallExpression m)
            {
                base.VisitMethodCall(m);
                this.isRemote |= (m.Method.DeclaringType == typeof(DataManipulation)) || Attribute.IsDefined(m.Method, typeof(FunctionAttribute));
                return m;
            }
        }
    }
}

