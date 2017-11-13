namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Objects.ELinq;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public sealed class CompiledQuery
    {
        private readonly Guid _cacheToken = Guid.NewGuid();
        private readonly ReadOnlyCollection<Parameter> _parameters;
        private readonly LambdaExpression _query;

        private CompiledQuery(LambdaExpression query, Type parameterDelegateType)
        {
            query = LockDown(query);
            ReadOnlyCollection<Parameter> onlys = Parameter.FindParameters(query, parameterDelegateType);
            this._query = query;
            this._parameters = onlys;
        }

        public static Func<TArg0, TResult> Compile<TArg0, TResult>(Expression<Func<TArg0, TResult>> query) where TArg0: ObjectContext => 
            new Func<TArg0, TResult>(new CompiledQuery(query, typeof(Func<object>)).Invoke<TArg0, TResult>);

        public static System.Func<TArg0, TArg1, TResult> Compile<TArg0, TArg1, TResult>(Expression<System.Func<TArg0, TArg1, TResult>> query) where TArg0: ObjectContext => 
            new System.Func<TArg0, TArg1, TResult>(new CompiledQuery(query, typeof(Func<TArg1, object>)).Invoke<TArg0, TArg1, TResult>);

        public static Func<TArg0, TArg1, TArg2, TResult> Compile<TArg0, TArg1, TArg2, TResult>(Expression<Func<TArg0, TArg1, TArg2, TResult>> query) where TArg0: ObjectContext => 
            new Func<TArg0, TArg1, TArg2, TResult>(new CompiledQuery(query, typeof(System.Func<TArg1, TArg2, object>)).Invoke<TArg0, TArg1, TArg2, TResult>);

        public static Func<TArg0, TArg1, TArg2, TArg3, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TResult>(Expression<Func<TArg0, TArg1, TArg2, TArg3, TResult>> query) where TArg0: ObjectContext => 
            new Func<TArg0, TArg1, TArg2, TArg3, TResult>(new CompiledQuery(query, typeof(Func<TArg1, TArg2, TArg3, object>)).Invoke<TArg0, TArg1, TArg2, TArg3, TResult>);

        private Dictionary<Expression, ObjectParameter> CreateObjectParameters(object[] parameterValues)
        {
            Dictionary<Expression, ObjectParameter> dictionary = new Dictionary<Expression, ObjectParameter>();
            foreach (Parameter parameter in this._parameters)
            {
                ObjectParameter parameter2 = parameter.CreateObjectParameter(parameterValues);
                dictionary.Add(parameter.ParameterExpression, parameter2);
            }
            return dictionary;
        }

        private TResult ExecuteQuery<TResult>(ObjectContext context, params object[] parameterValues)
        {
            bool flag;
            Dictionary<Expression, ObjectParameter> objectParameters = this.CreateObjectParameters(parameterValues);
            IEnumerable source = CompiledELinqQueryState.Create(GetElementType(typeof(TResult), out flag), context, this._query, objectParameters, this._cacheToken).CreateQuery();
            if (flag)
            {
                return ObjectQueryProvider.ExecuteSingle<TResult>(source.Cast<TResult>(), this._query);
            }
            return (TResult) source;
        }

        private static Type GetElementType(Type resultType, out bool isSingleton)
        {
            Type elementType = TypeSystem.GetElementType(resultType);
            isSingleton = (elementType == resultType) || !resultType.IsAssignableFrom(typeof(ObjectQuery<>).MakeGenericType(new Type[] { elementType }));
            if (isSingleton)
            {
                return resultType;
            }
            return elementType;
        }

        private TResult Invoke<TArg0, TResult>(TArg0 arg0) where TArg0: ObjectContext
        {
            EntityUtil.CheckArgumentNull<TArg0>(arg0, "arg0");
            arg0.MetadataWorkspace.LoadAssemblyForType(typeof(TResult), Assembly.GetCallingAssembly());
            return this.ExecuteQuery<TResult>(arg0, new object[0]);
        }

        private TResult Invoke<TArg0, TArg1, TResult>(TArg0 arg0, TArg1 arg1) where TArg0: ObjectContext
        {
            EntityUtil.CheckArgumentNull<TArg0>(arg0, "arg0");
            arg0.MetadataWorkspace.LoadAssemblyForType(typeof(TResult), Assembly.GetCallingAssembly());
            return this.ExecuteQuery<TResult>(arg0, new object[] { arg1 });
        }

        private TResult Invoke<TArg0, TArg1, TArg2, TResult>(TArg0 arg0, TArg1 arg1, TArg2 arg2) where TArg0: ObjectContext
        {
            EntityUtil.CheckArgumentNull<TArg0>(arg0, "arg0");
            arg0.MetadataWorkspace.LoadAssemblyForType(typeof(TResult), Assembly.GetCallingAssembly());
            return this.ExecuteQuery<TResult>(arg0, new object[] { arg1, arg2 });
        }

        private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TResult>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3) where TArg0: ObjectContext
        {
            EntityUtil.CheckArgumentNull<TArg0>(arg0, "arg0");
            arg0.MetadataWorkspace.LoadAssemblyForType(typeof(TResult), Assembly.GetCallingAssembly());
            return this.ExecuteQuery<TResult>(arg0, new object[] { arg1, arg2, arg3 });
        }

        private static LambdaExpression LockDown(LambdaExpression query) => 
            Expression.Lambda(LinqTreeNodeEvaluator.EvaluateClosuresAndClientEvalNodes(query.Body), query.Parameters.ToArray<ParameterExpression>());

        private sealed class Parameter
        {
            private readonly Delegate _getValue;
            private readonly Expression _parameterExpression;
            private readonly string _parameterName = ClosureBinding.GenerateParameterName();

            private Parameter(Type parameterDelegateType, Expression parameterExpression, System.Linq.Expressions.ParameterExpression[] argumentExpressions)
            {
                this._parameterExpression = parameterExpression;
                parameterExpression = Expression.Convert(parameterExpression, typeof(object));
                this._getValue = Expression.Lambda(parameterDelegateType, parameterExpression, argumentExpressions).Compile();
            }

            internal ObjectParameter CreateObjectParameter(object[] parameterValues) => 
                new ObjectParameter(this._parameterName, this.ParameterExpression.Type) { Value = this._getValue.DynamicInvoke(parameterValues) };

            internal static ReadOnlyCollection<CompiledQuery.Parameter> FindParameters(LambdaExpression query, Type parameterDelegateType)
            {
                System.Linq.Expressions.ParameterExpression[] argumentExpressions = query.Parameters.Skip<System.Linq.Expressions.ParameterExpression>(1).ToArray<System.Linq.Expressions.ParameterExpression>();
                HashSet<Expression> set = LinqMaximalSubtreeNominator.FindMaximalSubtrees(query, e => (ExpressionEvaluator.IsExpressionNodeClientEvaluatable(e) || ExpressionEvaluator.IsExpressionNodeAClosure(e)) || (((e != null) && (e.NodeType == ExpressionType.Parameter)) && argumentExpressions.Contains<System.Linq.Expressions.ParameterExpression>(((System.Linq.Expressions.ParameterExpression) e))));
                List<CompiledQuery.Parameter> list = new List<CompiledQuery.Parameter>(set.Count);
                foreach (Expression expression in set)
                {
                    if (expression.NodeType != ExpressionType.Constant)
                    {
                        list.Add(new CompiledQuery.Parameter(parameterDelegateType, expression, argumentExpressions));
                    }
                }
                return list.AsReadOnly();
            }

            internal Expression ParameterExpression =>
                this._parameterExpression;
        }
    }
}

