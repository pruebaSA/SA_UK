namespace System.Data.Services.Client
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ProjectionRewriter : System.Data.Services.Client.ExpressionVisitor
    {
        private readonly ParameterExpression newLambdaParameter;
        private ParameterExpression oldLambdaParameter;
        private bool sucessfulRebind;

        private ProjectionRewriter(Type proposedParameterType)
        {
            this.newLambdaParameter = Expression.Parameter(proposedParameterType, "it");
        }

        internal LambdaExpression Rebind(LambdaExpression lambda)
        {
            this.sucessfulRebind = true;
            this.oldLambdaParameter = lambda.Parameters[0];
            Expression body = this.Visit(lambda.Body);
            if (!this.sucessfulRebind)
            {
                throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CanOnlyProjectTheLeaf);
            }
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(new Type[] { this.newLambdaParameter.Type, lambda.Body.Type }), body, new ParameterExpression[] { this.newLambdaParameter });
        }

        internal static LambdaExpression TryToRewrite(LambdaExpression le, Type proposedParameterType)
        {
            if ((!ResourceBinder.PatternRules.MatchSingleArgumentLambda(le, out le) || ClientType.CheckElementTypeIsEntity(le.Parameters[0].Type)) || !le.Parameters[0].Type.GetProperties().Any<PropertyInfo>(p => (p.PropertyType == proposedParameterType)))
            {
                return le;
            }
            ProjectionRewriter rewriter = new ProjectionRewriter(proposedParameterType);
            return rewriter.Rebind(le);
        }

        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression == this.oldLambdaParameter)
            {
                if (m.Type == this.newLambdaParameter.Type)
                {
                    return this.newLambdaParameter;
                }
                this.sucessfulRebind = false;
            }
            return base.VisitMemberAccess(m);
        }
    }
}

