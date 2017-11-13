namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class ProjectionAnalyzer
    {
        internal static void Analyze(LambdaExpression e, PathBox pb)
        {
            bool flag = ClientType.CheckElementTypeIsEntity(e.Body.Type);
            pb.PushParamExpression(e.Parameters.Last<ParameterExpression>());
            if (!flag)
            {
                NonEntityProjectionAnalyzer.Analyze(e.Body, pb);
            }
            else
            {
                switch (e.Body.NodeType)
                {
                    case ExpressionType.Constant:
                        throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CannotCreateConstantEntity);

                    case ExpressionType.MemberInit:
                        EntityProjectionAnalyzer.Analyze((MemberInitExpression) e.Body, pb);
                        goto Label_0085;

                    case ExpressionType.New:
                        throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CannotConstructKnownEntityTypes);
                }
                NonEntityProjectionAnalyzer.Analyze(e.Body, pb);
            }
        Label_0085:
            pb.PopParamExpression();
        }

        private static void Analyze(MemberInitExpression mie, PathBox pb)
        {
            if (ClientType.CheckElementTypeIsEntity(mie.Type))
            {
                EntityProjectionAnalyzer.Analyze(mie, pb);
            }
            else
            {
                NonEntityProjectionAnalyzer.Analyze(mie, pb);
            }
        }

        internal static bool Analyze(LambdaExpression le, ResourceExpression re, bool matchMembers)
        {
            if (le.Body.NodeType == ExpressionType.Constant)
            {
                if (ClientType.CheckElementTypeIsEntity(le.Body.Type))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CannotCreateConstantEntity);
                }
                re.Projection = new ProjectionQueryOptionExpression(le.Body.Type, le, new List<string>());
                return true;
            }
            if ((le.Body.NodeType == ExpressionType.MemberInit) || (le.Body.NodeType == ExpressionType.New))
            {
                AnalyzeResourceExpression(le, re);
                return true;
            }
            if (matchMembers && (SkipConverts(le.Body).NodeType == ExpressionType.MemberAccess))
            {
                AnalyzeResourceExpression(le, re);
                return true;
            }
            return false;
        }

        private static void AnalyzeResourceExpression(LambdaExpression lambda, ResourceExpression resource)
        {
            PathBox pb = new PathBox();
            Analyze(lambda, pb);
            resource.Projection = new ProjectionQueryOptionExpression(lambda.Body.Type, lambda, pb.ProjectionPaths.ToList<string>());
            resource.ExpandPaths = pb.ExpandPaths.Union<string>(resource.ExpandPaths, StringComparer.Ordinal).ToList<string>();
        }

        internal static void CheckChainedSequence(MethodCallExpression call, Type type)
        {
            if (ReflectionUtil.IsSequenceMethod(call.Method, SequenceMethod.Select))
            {
                MethodCallExpression expression = ResourceBinder.StripTo<MethodCallExpression>(call.Arguments[0]);
                if ((expression != null) && ReflectionUtil.IsSequenceMethod(expression.Method, SequenceMethod.Select))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(type, call.ToString()));
                }
            }
        }

        internal static bool IsCollectionProducingExpression(Expression e)
        {
            if (TypeSystem.FindIEnumerable(e.Type) != null)
            {
                Type elementType = TypeSystem.GetElementType(e.Type);
                Type dataServiceCollectionOfT = WebUtil.GetDataServiceCollectionOfT(new Type[] { elementType });
                if (typeof(List<>).MakeGenericType(new Type[] { elementType }).IsAssignableFrom(e.Type) || ((dataServiceCollectionOfT != null) && dataServiceCollectionOfT.IsAssignableFrom(e.Type)))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsDisallowedExpressionForMethodCall(Expression e)
        {
            MemberExpression expression = e as MemberExpression;
            if ((expression != null) && ClientType.Create(expression.Expression.Type, false).IsEntityType)
            {
                return false;
            }
            return IsCollectionProducingExpression(e);
        }

        internal static bool IsMethodCallAllowedEntitySequence(MethodCallExpression call)
        {
            if (!ReflectionUtil.IsSequenceMethod(call.Method, SequenceMethod.ToList))
            {
                return ReflectionUtil.IsSequenceMethod(call.Method, SequenceMethod.Select);
            }
            return true;
        }

        private static Expression SkipConverts(Expression expression)
        {
            Expression operand = expression;
            while ((operand.NodeType == ExpressionType.Convert) || (operand.NodeType == ExpressionType.ConvertChecked))
            {
                operand = ((UnaryExpression) operand).Operand;
            }
            return operand;
        }

        private class EntityProjectionAnalyzer : System.Data.Services.Client.ExpressionVisitor
        {
            private readonly PathBox box;
            private readonly Type type;

            private EntityProjectionAnalyzer(PathBox pb, Type type)
            {
                this.box = pb;
                this.type = type;
            }

            internal static void Analyze(MemberInitExpression mie, PathBox pb)
            {
                ProjectionAnalyzer.EntityProjectionAnalyzer analyzer = new ProjectionAnalyzer.EntityProjectionAnalyzer(pb, mie.Type);
                MemberAssignmentAnalysis previous = null;
                foreach (MemberBinding binding in mie.Bindings)
                {
                    MemberAssignment assignment = binding as MemberAssignment;
                    analyzer.Visit(assignment.Expression);
                    if (assignment != null)
                    {
                        MemberAssignmentAnalysis analysis2 = MemberAssignmentAnalysis.Analyze(pb.ParamExpressionInScope, assignment.Expression);
                        if (analysis2.IncompatibleAssignmentsException != null)
                        {
                            throw analysis2.IncompatibleAssignmentsException;
                        }
                        Type memberType = GetMemberType(assignment.Member);
                        Expression[] expressionsBeyondTargetEntity = analysis2.GetExpressionsBeyondTargetEntity();
                        if (expressionsBeyondTargetEntity.Length == 0)
                        {
                            throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjectionToEntity(memberType, assignment.Expression));
                        }
                        MemberExpression expression = expressionsBeyondTargetEntity[expressionsBeyondTargetEntity.Length - 1] as MemberExpression;
                        if ((expression != null) && (expression.Member.Name != assignment.Member.Name))
                        {
                            throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_PropertyNamesMustMatchInProjections(expression.Member.Name, assignment.Member.Name));
                        }
                        analysis2.CheckCompatibleAssignments(mie.Type, ref previous);
                        bool flag = ClientType.CheckElementTypeIsEntity(memberType);
                        if (ClientType.CheckElementTypeIsEntity(expression.Type) && !flag)
                        {
                            throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(memberType, assignment.Expression));
                        }
                    }
                }
            }

            private static Type GetMemberType(MemberInfo member)
            {
                PropertyInfo info = member as PropertyInfo;
                if (info != null)
                {
                    return info.PropertyType;
                }
                FieldInfo info2 = member as FieldInfo;
                return info2.FieldType;
            }

            internal override Expression VisitBinary(BinaryExpression b)
            {
                throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjectionToEntity(this.type, b.ToString()));
            }

            internal override Expression VisitConditional(ConditionalExpression c)
            {
                ResourceBinder.PatternRules.MatchNullCheckResult result = ResourceBinder.PatternRules.MatchNullCheck(this.box.ParamExpressionInScope, c);
                if (!result.Match)
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjectionToEntity(this.type, c.ToString()));
                }
                this.Visit(result.AssignExpression);
                return c;
            }

            internal override Expression VisitConstant(ConstantExpression c)
            {
                throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjectionToEntity(this.type, c.ToString()));
            }

            internal override Expression VisitInvocation(InvocationExpression iv)
            {
                throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjectionToEntity(this.type, iv.ToString()));
            }

            internal override Expression VisitLambda(LambdaExpression lambda)
            {
                ProjectionAnalyzer.Analyze(lambda, this.box);
                return lambda;
            }

            internal override Expression VisitListInit(ListInitExpression init)
            {
                throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjectionToEntity(this.type, init.ToString()));
            }

            internal override Expression VisitMemberAccess(MemberExpression m)
            {
                if (!ClientType.CheckElementTypeIsEntity(m.Expression.Type) || ProjectionAnalyzer.IsCollectionProducingExpression(m.Expression))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjectionToEntity(this.type, m.ToString()));
                }
                PropertyInfo propInfo = null;
                if (!ResourceBinder.PatternRules.MatchNonPrivateReadableProperty(m, out propInfo))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjectionToEntity(this.type, m.ToString()));
                }
                Expression expression = base.VisitMemberAccess(m);
                this.box.AppendToPath(propInfo);
                return expression;
            }

            internal override Expression VisitMemberInit(MemberInitExpression init)
            {
                ProjectionAnalyzer.Analyze(init, this.box);
                return init;
            }

            internal override Expression VisitMethodCall(MethodCallExpression m)
            {
                if (((m.Object != null) && ProjectionAnalyzer.IsDisallowedExpressionForMethodCall(m.Object)) || m.Arguments.Any<Expression>(a => ProjectionAnalyzer.IsDisallowedExpressionForMethodCall(a)))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(this.type, m.ToString()));
                }
                if (!ProjectionAnalyzer.IsMethodCallAllowedEntitySequence(m))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjectionToEntity(this.type, m.ToString()));
                }
                ProjectionAnalyzer.CheckChainedSequence(m, this.type);
                return base.VisitMethodCall(m);
            }

            internal override NewExpression VisitNew(NewExpression nex)
            {
                if (!ResourceBinder.PatternRules.MatchNewDataServiceCollectionOfT(nex) || !ClientType.CheckElementTypeIsEntity(nex.Type))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjectionToEntity(this.type, nex.ToString()));
                }
                foreach (Expression expression in nex.Arguments)
                {
                    if (expression.NodeType != ExpressionType.Constant)
                    {
                        base.Visit(expression);
                    }
                }
                return nex;
            }

            internal override Expression VisitNewArray(NewArrayExpression na)
            {
                throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjectionToEntity(this.type, na.ToString()));
            }

            internal override Expression VisitParameter(ParameterExpression p)
            {
                if (p != this.box.ParamExpressionInScope)
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CanOnlyProjectTheLeaf);
                }
                this.box.StartNewPath();
                return p;
            }

            internal override Expression VisitTypeIs(TypeBinaryExpression b)
            {
                throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjectionToEntity(this.type, b.ToString()));
            }

            internal override Expression VisitUnary(UnaryExpression u)
            {
                if (ResourceBinder.PatternRules.MatchConvertToAssignable(u))
                {
                    return base.VisitUnary(u);
                }
                if ((u.NodeType == ExpressionType.Convert) || (u.NodeType == ExpressionType.ConvertChecked))
                {
                    Type type = Nullable.GetUnderlyingType(u.Operand.Type) ?? u.Operand.Type;
                    Type type2 = Nullable.GetUnderlyingType(u.Type) ?? u.Type;
                    if (ClientConvert.IsKnownType(type) && ClientConvert.IsKnownType(type2))
                    {
                        return base.Visit(u.Operand);
                    }
                }
                throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjectionToEntity(this.type, u.ToString()));
            }
        }

        private class NonEntityProjectionAnalyzer : DataServiceExpressionVisitor
        {
            private PathBox box;
            private Type type;

            private NonEntityProjectionAnalyzer(PathBox pb, Type type)
            {
                this.box = pb;
                this.type = type;
            }

            internal static void Analyze(Expression e, PathBox pb)
            {
                ProjectionAnalyzer.NonEntityProjectionAnalyzer analyzer = new ProjectionAnalyzer.NonEntityProjectionAnalyzer(pb, e.Type);
                MemberInitExpression expression = e as MemberInitExpression;
                if (expression != null)
                {
                    foreach (MemberBinding binding in expression.Bindings)
                    {
                        MemberAssignment assignment = binding as MemberAssignment;
                        if (assignment != null)
                        {
                            analyzer.Visit(assignment.Expression);
                        }
                    }
                }
                else
                {
                    analyzer.Visit(e);
                }
            }

            internal override Expression VisitBinary(BinaryExpression b)
            {
                if ((ClientType.CheckElementTypeIsEntity(b.Left.Type) || ClientType.CheckElementTypeIsEntity(b.Right.Type)) || (ProjectionAnalyzer.IsCollectionProducingExpression(b.Left) || ProjectionAnalyzer.IsCollectionProducingExpression(b.Right)))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(this.type, b.ToString()));
                }
                return base.VisitBinary(b);
            }

            internal override Expression VisitConditional(ConditionalExpression c)
            {
                ResourceBinder.PatternRules.MatchNullCheckResult result = ResourceBinder.PatternRules.MatchNullCheck(this.box.ParamExpressionInScope, c);
                if (result.Match)
                {
                    this.Visit(result.AssignExpression);
                    return c;
                }
                if (((ClientType.CheckElementTypeIsEntity(c.Test.Type) || ClientType.CheckElementTypeIsEntity(c.IfTrue.Type)) || (ClientType.CheckElementTypeIsEntity(c.IfFalse.Type) || ProjectionAnalyzer.IsCollectionProducingExpression(c.Test))) || (ProjectionAnalyzer.IsCollectionProducingExpression(c.IfTrue) || ProjectionAnalyzer.IsCollectionProducingExpression(c.IfFalse)))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(this.type, c.ToString()));
                }
                return base.VisitConditional(c);
            }

            internal override Expression VisitConstant(ConstantExpression c)
            {
                if (ClientType.CheckElementTypeIsEntity(c.Type))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(this.type, c.ToString()));
                }
                return base.VisitConstant(c);
            }

            internal override Expression VisitInvocation(InvocationExpression iv)
            {
                if ((ClientType.CheckElementTypeIsEntity(iv.Expression.Type) || ProjectionAnalyzer.IsCollectionProducingExpression(iv.Expression)) || iv.Arguments.Any<Expression>(delegate (Expression a) {
                    if (!ClientType.CheckElementTypeIsEntity(a.Type))
                    {
                        return ProjectionAnalyzer.IsCollectionProducingExpression(a);
                    }
                    return true;
                }))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(this.type, iv.ToString()));
                }
                return base.VisitInvocation(iv);
            }

            internal override Expression VisitLambda(LambdaExpression lambda)
            {
                ProjectionAnalyzer.Analyze(lambda, this.box);
                return lambda;
            }

            internal override Expression VisitMemberAccess(MemberExpression m)
            {
                if (ClientConvert.IsKnownNullableType(m.Expression.Type))
                {
                    return base.VisitMemberAccess(m);
                }
                if (!ClientType.CheckElementTypeIsEntity(m.Expression.Type) || ProjectionAnalyzer.IsCollectionProducingExpression(m.Expression))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(this.type, m.ToString()));
                }
                PropertyInfo propInfo = null;
                if (!ResourceBinder.PatternRules.MatchNonPrivateReadableProperty(m, out propInfo))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(this.type, m.ToString()));
                }
                Expression expression = base.VisitMemberAccess(m);
                this.box.AppendToPath(propInfo);
                return expression;
            }

            internal override Expression VisitMemberInit(MemberInitExpression init)
            {
                ProjectionAnalyzer.Analyze(init, this.box);
                return init;
            }

            internal override Expression VisitMethodCall(MethodCallExpression m)
            {
                if (((m.Object != null) && ProjectionAnalyzer.IsDisallowedExpressionForMethodCall(m.Object)) || m.Arguments.Any<Expression>(a => ProjectionAnalyzer.IsDisallowedExpressionForMethodCall(a)))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(this.type, m.ToString()));
                }
                if (ProjectionAnalyzer.IsMethodCallAllowedEntitySequence(m))
                {
                    ProjectionAnalyzer.CheckChainedSequence(m, this.type);
                    return base.VisitMethodCall(m);
                }
                if (((m.Object != null) ? ClientType.CheckElementTypeIsEntity(m.Object.Type) : false) || m.Arguments.Any<Expression>(a => ClientType.CheckElementTypeIsEntity(a.Type)))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(this.type, m.ToString()));
                }
                return base.VisitMethodCall(m);
            }

            internal override NewExpression VisitNew(NewExpression nex)
            {
                if (ClientType.CheckElementTypeIsEntity(nex.Type) && !ResourceBinder.PatternRules.MatchNewDataServiceCollectionOfT(nex))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(this.type, nex.ToString()));
                }
                return base.VisitNew(nex);
            }

            internal override Expression VisitParameter(ParameterExpression p)
            {
                if (p != this.box.ParamExpressionInScope)
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(this.type, p.ToString()));
                }
                this.box.StartNewPath();
                return p;
            }

            internal override Expression VisitTypeIs(TypeBinaryExpression b)
            {
                if (ClientType.CheckElementTypeIsEntity(b.Expression.Type) || ProjectionAnalyzer.IsCollectionProducingExpression(b.Expression))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(this.type, b.ToString()));
                }
                return base.VisitTypeIs(b);
            }

            internal override Expression VisitUnary(UnaryExpression u)
            {
                if (!ResourceBinder.PatternRules.MatchConvertToAssignable(u) && ClientType.CheckElementTypeIsEntity(u.Operand.Type))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ExpressionNotSupportedInProjection(this.type, u.ToString()));
                }
                return base.VisitUnary(u);
            }
        }
    }
}

