namespace System.Data.Objects.ELinq
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Data.Objects.Internal;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class ELinqQueryState : ObjectQueryState
    {
        private List<ClosureBinding> _closureBindings;
        private readonly System.Linq.Expressions.Expression _expression;

        internal ELinqQueryState(Type elementType, ObjectContext context, System.Linq.Expressions.Expression expression) : this(elementType, context, null, expression)
        {
        }

        protected ELinqQueryState(Type elementType, ObjectContext context, ObjectParameterCollection parameters, System.Linq.Expressions.Expression expression) : base(elementType, context, parameters, null)
        {
            EntityUtil.CheckArgumentNull<System.Linq.Expressions.Expression>(expression, "expression");
            this._expression = expression;
        }

        protected virtual ExpressionConverter CreateExpressionConverter() => 
            new ExpressionConverter(base.ObjectContext, new BindingContext(), new DbQueryCommandTree(base.ObjectContext.MetadataWorkspace, DataSpace.CSpace), this._expression, null);

        internal override ObjectQueryExecutionPlan GetExecutionPlan(MergeOption? forMergeOption)
        {
            ObjectQueryExecutionPlan plan = base._cachedPlan;
            if (plan != null)
            {
                MergeOption?[] preferredMergeOptions = new MergeOption?[] { forMergeOption, base.UserSpecifiedMergeOption };
                MergeOption? mergeOption = ObjectQueryState.GetMergeOption(preferredMergeOptions);
                if (mergeOption.HasValue && (((MergeOption) mergeOption.Value) != plan.MergeOption))
                {
                    plan = null;
                }
                else if (this._closureBindings != null)
                {
                    bool flag = false;
                    foreach (ClosureBinding binding in this._closureBindings)
                    {
                        flag |= binding.EvaluateBinding();
                    }
                    if (flag)
                    {
                        plan = null;
                    }
                }
            }
            if (plan == null)
            {
                base.ObjectContext.EnsureMetadata();
                this._closureBindings = null;
                this.ResetParameters();
                ExpressionConverter converter = this.CreateExpressionConverter();
                DbExpression expression = converter.Convert();
                DbQueryCommandTree commandTree = (DbQueryCommandTree) expression.CommandTree;
                commandTree.Query = expression;
                this._closureBindings = converter.ClosureBindings;
                MergeOption?[] nullableArray2 = new MergeOption?[] { forMergeOption, base.UserSpecifiedMergeOption, converter.PropagatedMergeOption };
                MergeOption option = ObjectQueryState.EnsureMergeOption(nullableArray2);
                if ((converter.Parameters != null) && (converter.Parameters.Count > 0))
                {
                    ObjectParameterCollection parameters = base.EnsureParameters();
                    parameters.SetReadOnly(false);
                    foreach (ObjectParameter parameter in (IEnumerable<ObjectParameter>) converter.Parameters)
                    {
                        parameters.Add(parameter);
                    }
                    parameters.SetReadOnly(true);
                }
                plan = ObjectQueryExecutionPlan.Prepare(base.ObjectContext, commandTree, base.ElementType, option, converter.PropagatedSpan);
                base._cachedPlan = plan;
            }
            if (base.Parameters != null)
            {
                base.Parameters.SetReadOnly(true);
            }
            return plan;
        }

        protected override TypeUsage GetResultType() => 
            this.CreateExpressionConverter().Convert().ResultType;

        internal override ObjectQueryState Include<TElementType>(ObjectQuery<TElementType> sourceQuery, string includePath)
        {
            MethodInfo method = sourceQuery.GetType().GetMethod("Include", BindingFlags.Public | BindingFlags.Instance);
            System.Linq.Expressions.Expression expression = System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(sourceQuery), method, new System.Linq.Expressions.Expression[] { System.Linq.Expressions.Expression.Constant(includePath, typeof(string)) });
            ObjectQueryState other = new ELinqQueryState(base.ElementType, base.ObjectContext, expression);
            base.ApplySettingsTo(other);
            return other;
        }

        private void ResetParameters()
        {
            if (base.Parameters != null)
            {
                bool isReadOnly = base.Parameters.IsReadOnly;
                if (isReadOnly)
                {
                    base.Parameters.SetReadOnly(false);
                }
                base.Parameters.Clear();
                if (isReadOnly)
                {
                    base.Parameters.SetReadOnly(true);
                }
            }
        }

        internal override bool TryGetCommandText(out string commandText)
        {
            commandText = null;
            return false;
        }

        internal override bool TryGetExpression(out System.Linq.Expressions.Expression expression)
        {
            expression = this.Expression;
            return true;
        }

        internal virtual System.Linq.Expressions.Expression Expression =>
            this._expression;
    }
}

