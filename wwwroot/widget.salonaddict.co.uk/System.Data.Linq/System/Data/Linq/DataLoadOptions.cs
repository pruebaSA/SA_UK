namespace System.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Linq.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public sealed class DataLoadOptions
    {
        private bool frozen;
        private Dictionary<MetaPosition, MemberInfo> includes = new Dictionary<MetaPosition, MemberInfo>();
        private Dictionary<MetaPosition, LambdaExpression> subqueries = new Dictionary<MetaPosition, LambdaExpression>();

        public void AssociateWith<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("expression");
            }
            this.AssociateWithInternal(expression);
        }

        public void AssociateWith(LambdaExpression expression)
        {
            if (expression == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("expression");
            }
            this.AssociateWithInternal(expression);
        }

        private void AssociateWithInternal(LambdaExpression expression)
        {
            Expression body = expression.Body;
            while ((body.NodeType == ExpressionType.Convert) || (body.NodeType == ExpressionType.ConvertChecked))
            {
                body = ((UnaryExpression) body).Operand;
            }
            LambdaExpression lambda = Expression.Lambda(body, expression.Parameters.ToArray<ParameterExpression>());
            MemberInfo association = Searcher.MemberInfoOf(lambda);
            this.Subquery(association, lambda);
        }

        internal void Freeze()
        {
            this.frozen = true;
        }

        internal LambdaExpression GetAssociationSubquery(MemberInfo member)
        {
            if (member == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("member");
            }
            LambdaExpression expression = null;
            this.subqueries.TryGetValue(new MetaPosition(member), out expression);
            return expression;
        }

        private static Type GetIncludeTarget(MemberInfo mi)
        {
            Type memberType = TypeSystem.GetMemberType(mi);
            if (memberType.IsGenericType)
            {
                return memberType.GetGenericArguments()[0];
            }
            return memberType;
        }

        private static MemberInfo GetLoadWithMemberInfo(LambdaExpression lambda)
        {
            Expression body = lambda.Body;
            if ((body != null) && ((body.NodeType == ExpressionType.Convert) || (body.NodeType == ExpressionType.ConvertChecked)))
            {
                body = ((UnaryExpression) body).Operand;
            }
            MemberExpression expression2 = body as MemberExpression;
            if ((expression2 == null) || (expression2.Expression.NodeType != ExpressionType.Parameter))
            {
                throw System.Data.Linq.Error.InvalidLoadOptionsLoadMemberSpecification();
            }
            return expression2.Member;
        }

        internal bool IsPreloaded(MemberInfo member)
        {
            if (member == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("member");
            }
            return this.includes.ContainsKey(new MetaPosition(member));
        }

        public void LoadWith<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("expression");
            }
            MemberInfo loadWithMemberInfo = GetLoadWithMemberInfo(expression);
            this.Preload(loadWithMemberInfo);
        }

        public void LoadWith(LambdaExpression expression)
        {
            if (expression == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("expression");
            }
            MemberInfo loadWithMemberInfo = GetLoadWithMemberInfo(expression);
            this.Preload(loadWithMemberInfo);
        }

        internal void Preload(MemberInfo association)
        {
            if (association == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("association");
            }
            if (this.frozen)
            {
                throw System.Data.Linq.Error.IncludeNotAllowedAfterFreeze();
            }
            this.includes.Add(new MetaPosition(association), association);
            this.ValidateTypeGraphAcyclic();
        }

        private void Subquery(MemberInfo association, LambdaExpression subquery)
        {
            if (this.frozen)
            {
                throw System.Data.Linq.Error.SubqueryNotAllowedAfterFreeze();
            }
            subquery = (LambdaExpression) Funcletizer.Funcletize(subquery);
            ValidateSubqueryMember(association);
            ValidateSubqueryExpression(subquery);
            this.subqueries[new MetaPosition(association)] = subquery;
        }

        private static void ValidateSubqueryExpression(LambdaExpression subquery)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(subquery.Body.Type))
            {
                throw System.Data.Linq.Error.SubqueryMustBeSequence();
            }
            new SubqueryValidator().VisitLambda(subquery);
        }

        private static void ValidateSubqueryMember(MemberInfo mi)
        {
            Type memberType = TypeSystem.GetMemberType(mi);
            if (memberType == null)
            {
                throw System.Data.Linq.Error.SubqueryNotSupportedOn(mi);
            }
            if (!typeof(IEnumerable).IsAssignableFrom(memberType))
            {
                throw System.Data.Linq.Error.SubqueryNotSupportedOnType(mi.Name, mi.DeclaringType);
            }
        }

        private void ValidateTypeGraphAcyclic()
        {
            IEnumerable<MemberInfo> values = this.includes.Values;
            int num = 0;
            for (int i = 0; i < this.includes.Count; i++)
            {
                HashSet<Type> source = new HashSet<Type>();
                foreach (MemberInfo info in values)
                {
                    source.Add(GetIncludeTarget(info));
                }
                List<MemberInfo> list = new List<MemberInfo>();
                bool flag = false;
                using (IEnumerator<MemberInfo> enumerator2 = values.GetEnumerator())
                {
                    Func<Type, bool> predicate = null;
                    MemberInfo edge;
                    while (enumerator2.MoveNext())
                    {
                        edge = enumerator2.Current;
                        if (predicate == null)
                        {
                            predicate = delegate (Type et) {
                                if (!et.IsAssignableFrom(edge.DeclaringType))
                                {
                                    return edge.DeclaringType.IsAssignableFrom(et);
                                }
                                return true;
                            };
                        }
                        if (source.Where<Type>(predicate).Any<Type>())
                        {
                            list.Add(edge);
                        }
                        else
                        {
                            num++;
                            flag = true;
                            if (num == this.includes.Count)
                            {
                                return;
                            }
                        }
                    }
                }
                if (!flag)
                {
                    throw System.Data.Linq.Error.IncludeCycleNotAllowed();
                }
                values = list;
            }
            throw new InvalidOperationException("Bug in ValidateTypeGraphAcyclic");
        }

        internal bool IsEmpty =>
            ((this.includes.Count == 0) && (this.subqueries.Count == 0));

        private static class Searcher
        {
            internal static MemberInfo MemberInfoOf(LambdaExpression lambda)
            {
                Visitor visitor = new Visitor();
                visitor.VisitLambda(lambda);
                return visitor.MemberInfo;
            }

            private class Visitor : System.Data.Linq.SqlClient.ExpressionVisitor
            {
                internal System.Reflection.MemberInfo MemberInfo;

                internal override Expression VisitMemberAccess(MemberExpression m)
                {
                    this.MemberInfo = m.Member;
                    return base.VisitMemberAccess(m);
                }

                internal override Expression VisitMethodCall(MethodCallExpression m)
                {
                    this.Visit(m.Object);
                    foreach (Expression expression in m.Arguments)
                    {
                        this.Visit(expression);
                        return m;
                    }
                    return m;
                }
            }
        }

        private class SubqueryValidator : System.Data.Linq.SqlClient.ExpressionVisitor
        {
            private bool isTopLevel = true;

            internal override Expression VisitMethodCall(MethodCallExpression m)
            {
                Expression expression;
                bool isTopLevel = this.isTopLevel;
                try
                {
                    if (this.isTopLevel && !SubqueryRules.IsSupportedTopLevelMethod(m.Method))
                    {
                        throw System.Data.Linq.Error.SubqueryDoesNotSupportOperator(m.Method.Name);
                    }
                    this.isTopLevel = false;
                    expression = base.VisitMethodCall(m);
                }
                finally
                {
                    this.isTopLevel = isTopLevel;
                }
                return expression;
            }
        }
    }
}

