namespace System.Data.Objects.ELinq
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.QueryCache;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Data.Objects.Internal;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;

    internal sealed class CompiledELinqQueryState : ELinqQueryState
    {
        private CompiledQueryCacheEntry _cacheEntry;
        private readonly Guid _cacheToken;
        private readonly CompiledQueryParameter[] _parameters;
        private readonly ParameterExpression _rootContextParameter;

        private CompiledELinqQueryState(Type elementType, ObjectContext context, ObjectParameterCollection objectParameters, CompiledQueryParameter[] compiledParams, LambdaExpression lambda, Guid cacheToken) : base(elementType, context, objectParameters, lambda.Body)
        {
            this._rootContextParameter = lambda.Parameters[0];
            this._parameters = compiledParams;
            this._cacheToken = cacheToken;
            base.EnsureParameters().SetReadOnly(true);
        }

        internal static CompiledELinqQueryState Create(Type elementType, ObjectContext context, LambdaExpression lambda, Dictionary<System.Linq.Expressions.Expression, ObjectParameter> objectParameters, Guid cacheToken)
        {
            EntityUtil.CheckArgumentNull<ObjectContext>(context, "context");
            ObjectParameterCollection parameters = new ObjectParameterCollection(context.Perspective);
            CompiledQueryParameter[] compiledParams = new CompiledQueryParameter[objectParameters.Count];
            int index = 0;
            foreach (KeyValuePair<System.Linq.Expressions.Expression, ObjectParameter> pair in objectParameters)
            {
                compiledParams[index] = new CompiledQueryParameter(pair.Key, pair.Value);
                try
                {
                    parameters.Add(compiledParams[index].ObjectParameter);
                }
                catch (ArgumentOutOfRangeException)
                {
                    HashSet<ParameterExpression> source = ParameterExpressionVisitor.FindAllParametersInExpression(pair.Key);
                    Type type = (source.Count == 1) ? source.Single<ParameterExpression>().Type : pair.Value.ParameterType;
                    throw EntityUtil.NotSupported(System.Data.Entity.Strings.CompiledELinq_UnsupportedParameterType(type.FullName));
                }
                index++;
            }
            return new CompiledELinqQueryState(elementType, context, parameters, compiledParams, lambda, cacheToken);
        }

        protected override ExpressionConverter CreateExpressionConverter()
        {
            DbQueryCommandTree commandTree = new DbQueryCommandTree(base.ObjectContext.MetadataWorkspace, DataSpace.CSpace);
            foreach (CompiledQueryParameter parameter in this._parameters)
            {
                parameter.CreateParameterReferenceAndAddParameterToCommandTree(commandTree, base.ObjectContext.Perspective);
            }
            return new ExpressionConverter(base.ObjectContext, new BindingContext(this._rootContextParameter, base.ObjectContext, this._parameters), commandTree, base.Expression, base.Parameters);
        }

        internal override ObjectQueryExecutionPlan GetExecutionPlan(MergeOption? forMergeOption)
        {
            base.ObjectContext.EnsureMetadata();
            ObjectQueryExecutionPlan newPlan = null;
            CompiledQueryCacheEntry cacheEntry = this._cacheEntry;
            if (cacheEntry != null)
            {
                MergeOption?[] preferredMergeOptions = new MergeOption?[] { forMergeOption, base.UserSpecifiedMergeOption, cacheEntry.PropagatedMergeOption };
                MergeOption mergeOption = ObjectQueryState.EnsureMergeOption(preferredMergeOptions);
                newPlan = cacheEntry.GetExecutionPlan(mergeOption);
                if (newPlan == null)
                {
                    ExpressionConverter converter = this.CreateExpressionConverter();
                    DbExpression expression = converter.Convert();
                    DbQueryCommandTree commandTree = (DbQueryCommandTree) expression.CommandTree;
                    commandTree.Query = expression;
                    newPlan = ObjectQueryExecutionPlan.Prepare(base.ObjectContext, commandTree, base.ElementType, mergeOption, converter.PropagatedSpan);
                    newPlan = cacheEntry.SetExecutionPlan(newPlan);
                }
                return newPlan;
            }
            QueryCacheManager queryCacheManager = base.ObjectContext.MetadataWorkspace.GetQueryCacheManager();
            CompiledQueryCacheKey objectQueryCacheKey = new CompiledQueryCacheKey(this._cacheToken);
            if (queryCacheManager.TryCacheLookup(objectQueryCacheKey, out cacheEntry))
            {
                this._cacheEntry = cacheEntry;
                MergeOption?[] nullableArray2 = new MergeOption?[] { forMergeOption, base.UserSpecifiedMergeOption, cacheEntry.PropagatedMergeOption };
                MergeOption option2 = ObjectQueryState.EnsureMergeOption(nullableArray2);
                newPlan = cacheEntry.GetExecutionPlan(option2);
            }
            if (newPlan == null)
            {
                ExpressionConverter converter2 = this.CreateExpressionConverter();
                DbExpression expression2 = converter2.Convert();
                DbQueryCommandTree tree = (DbQueryCommandTree) expression2.CommandTree;
                tree.Query = expression2;
                if (cacheEntry == null)
                {
                    QueryCacheEntry entry2;
                    cacheEntry = new CompiledQueryCacheEntry(objectQueryCacheKey, converter2.PropagatedMergeOption);
                    if (queryCacheManager.TryLookupAndAdd(cacheEntry, out entry2))
                    {
                        cacheEntry = (CompiledQueryCacheEntry) entry2;
                    }
                    this._cacheEntry = cacheEntry;
                }
                MergeOption?[] nullableArray3 = new MergeOption?[] { forMergeOption, base.UserSpecifiedMergeOption, cacheEntry.PropagatedMergeOption };
                MergeOption option3 = ObjectQueryState.EnsureMergeOption(nullableArray3);
                newPlan = cacheEntry.GetExecutionPlan(option3);
                if (newPlan == null)
                {
                    newPlan = ObjectQueryExecutionPlan.Prepare(base.ObjectContext, tree, base.ElementType, option3, converter2.PropagatedSpan);
                    newPlan = cacheEntry.SetExecutionPlan(newPlan);
                }
            }
            return newPlan;
        }

        protected override TypeUsage GetResultType()
        {
            TypeUsage usage;
            CompiledQueryCacheEntry entry = this._cacheEntry;
            if ((entry != null) && entry.TryGetResultType(out usage))
            {
                return usage;
            }
            return base.GetResultType();
        }

        internal override System.Linq.Expressions.Expression Expression =>
            CreateDonateableExpressionVisitor.Replace(base.Expression, base.ObjectContext, this._rootContextParameter, this._parameters);

        private sealed class CreateDonateableExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
        {
            private readonly BindingContext _bindingContext;
            private readonly Dictionary<Expression, object> _parameterToValueLookup;

            private CreateDonateableExpressionVisitor(BindingContext bindingContext, Dictionary<Expression, object> parameterToValueLookup)
            {
                this._bindingContext = bindingContext;
                this._parameterToValueLookup = parameterToValueLookup;
            }

            internal static Expression Replace(Expression input, ObjectContext objectContext, ParameterExpression rootContextParameter, CompiledQueryParameter[] parameters)
            {
                BindingContext bindingContext = new BindingContext(rootContextParameter, objectContext, parameters);
                Dictionary<Expression, object> parameterToValueLookup = parameters.ToDictionary<CompiledQueryParameter, Expression, object>(p => p.Expression, p => p.ObjectParameter.Value);
                CompiledELinqQueryState.CreateDonateableExpressionVisitor visitor = new CompiledELinqQueryState.CreateDonateableExpressionVisitor(bindingContext, parameterToValueLookup);
                return visitor.Visit(input);
            }

            private bool TryReplaceRootQuery(Expression expression, out Expression newExpression)
            {
                ObjectQuery query;
                if (ExpressionEvaluator.TryEvaluateRootQuery(this._bindingContext, expression, out query))
                {
                    newExpression = Expression.Constant(query, expression.Type);
                    return true;
                }
                newExpression = null;
                return false;
            }

            internal override Expression Visit(Expression expression)
            {
                object obj2;
                if ((expression != null) && this._parameterToValueLookup.TryGetValue(expression, out obj2))
                {
                    return Expression.Constant(obj2, expression.Type);
                }
                return base.Visit(expression);
            }

            internal override Expression VisitMemberAccess(MemberExpression m)
            {
                Expression expression;
                if (this.TryReplaceRootQuery(m, out expression))
                {
                    return expression;
                }
                return base.VisitMemberAccess(m);
            }

            internal override Expression VisitMethodCall(MethodCallExpression m)
            {
                Expression expression;
                if (this.TryReplaceRootQuery(m, out expression))
                {
                    return expression;
                }
                return base.VisitMethodCall(m);
            }
        }

        private sealed class ParameterExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
        {
            internal readonly HashSet<ParameterExpression> _parameters = new HashSet<ParameterExpression>();

            private ParameterExpressionVisitor()
            {
            }

            internal static HashSet<ParameterExpression> FindAllParametersInExpression(Expression expression)
            {
                CompiledELinqQueryState.ParameterExpressionVisitor visitor = new CompiledELinqQueryState.ParameterExpressionVisitor();
                visitor.Visit(expression);
                return visitor._parameters;
            }

            internal override Expression VisitParameter(ParameterExpression p)
            {
                this._parameters.Add(p);
                return base.VisitParameter(p);
            }
        }
    }
}

