namespace System.Data.Objects.ELinq
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal static class ExpressionEvaluator
    {
        private static readonly IQueryProvider s_compilerQueryService = Enumerable.Empty<int>().AsQueryable<int>().Provider;

        [Conditional("DEBUG")]
        private static void DebugOnlyVerifyMethodOverloadCount(Type type, string methodName, int expectedCount)
        {
            int num = 0;
            foreach (MethodInfo info in type.GetMethods())
            {
                if (info.Name == methodName)
                {
                    num++;
                }
            }
        }

        internal static object EvaluateExpression(Expression expression) => 
            s_compilerQueryService.Execute(expression);

        internal static bool IsExpressionNodeAClosure(Expression expression)
        {
            if (ExpressionType.MemberAccess != expression.NodeType)
            {
                return false;
            }
            MemberExpression expression2 = (MemberExpression) expression;
            if (expression2.Member.MemberType == MemberTypes.Property)
            {
                return !ExpressionConverter.CanTranslatePropertyInfo((PropertyInfo) expression2.Member);
            }
            return true;
        }

        internal static bool IsExpressionNodeClientEvaluatable(Expression expression)
        {
            if (expression != null)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Constant:
                        return true;

                    case ExpressionType.Convert:
                        return true;

                    case ExpressionType.New:
                        PrimitiveType type;
                        return ClrProviderManifest.Instance.TryGetPrimitiveType(TypeSystem.GetNonNullableType(expression.Type), out type);

                    case ExpressionType.NewArrayInit:
                        return (typeof(byte[]) == expression.Type);
                }
            }
            return false;
        }

        internal static bool TryEvaluateRootQuery(BindingContext bindingContext, Expression expression, out ObjectQuery rootQuery)
        {
            rootQuery = null;
            if (bindingContext.ObjectContext == null)
            {
                return false;
            }
            if (!typeof(ObjectQuery).IsAssignableFrom(expression.Type))
            {
                return false;
            }
            Expression expression2 = expression;
            if (expression2.NodeType != ExpressionType.MemberAccess)
            {
                return false;
            }
            MemberExpression expression3 = (MemberExpression) expression2;
            expression2 = expression3.Expression;
            if ((expression2 == null) || (expression2.NodeType != ExpressionType.Parameter))
            {
                return false;
            }
            ParameterExpression parameter = (ParameterExpression) expression2;
            if (!bindingContext.IsRootContextParameter(parameter))
            {
                return false;
            }
            rootQuery = ((IRootQueryCreator) Activator.CreateInstance(typeof(RootQueryCreator).MakeGenericType(new Type[] { parameter.Type, expression.Type }))).Invoke(expression, parameter, bindingContext.ObjectContext);
            return true;
        }

        private interface IRootQueryCreator
        {
            ObjectQuery Invoke(Expression body, ParameterExpression parameter, ObjectContext objectContext);
        }

        private class RootQueryCreator<T_Context, T_ObjectQuery> : ExpressionEvaluator.IRootQueryCreator where T_Context: ObjectContext where T_ObjectQuery: ObjectQuery
        {
            public ObjectQuery Invoke(Expression body, ParameterExpression parameter, ObjectContext objectContext) => 
                Expression.Lambda<Func<T_Context, T_ObjectQuery>>(body, new ParameterExpression[] { parameter }).Compile()((T_Context) objectContext);
        }
    }
}

