namespace System.Data.Objects.ELinq
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal abstract class ClosureBinding
    {
        private readonly System.Linq.Expressions.Expression _sourceExpression;
        private static long s_parameterNumber;
        private static readonly string s_parameterPrefix = "p__linq__";

        private ClosureBinding(System.Linq.Expressions.Expression sourceExpression)
        {
            this._sourceExpression = sourceExpression;
        }

        internal abstract ClosureBinding CopyToContext(ExpressionConverter context);
        internal abstract bool EvaluateBinding();
        internal static string GenerateParameterName() => 
            string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[] { s_parameterPrefix, Interlocked.Increment(ref s_parameterNumber) });

        internal static bool TryCreateClosureBinding(System.Linq.Expressions.Expression expression, ClrPerspective perspective, bool allowLambda, HashSet<System.Linq.Expressions.Expression> closureCandidates, out ClosureBinding binding, out TypeUsage typeUsage)
        {
            if (ExpressionEvaluator.IsExpressionNodeAClosure(expression) && closureCandidates.Contains(expression))
            {
                ObjectParameter parameter = new ObjectParameter(GenerateParameterName(), expression.Type);
                if (TryGetTypeUsageForObjectParameter(parameter, perspective, out typeUsage))
                {
                    binding = new ParameterBinding(expression, parameter);
                    binding.EvaluateBinding();
                    return true;
                }
                object nestedLogic = ExpressionEvaluator.EvaluateExpression(expression);
                ObjectQuery query = nestedLogic as ObjectQuery;
                if (query != null)
                {
                    binding = new NestedLogicBinding(expression, nestedLogic, query.QueryState.UserSpecifiedMergeOption);
                    return true;
                }
                if (allowLambda && (nestedLogic is LambdaExpression))
                {
                    binding = new NestedLogicBinding(expression, nestedLogic, null);
                    return true;
                }
            }
            binding = null;
            typeUsage = null;
            return false;
        }

        internal static bool TryGetTypeUsageForObjectParameter(ObjectParameter parameter, ClrPerspective perspective, out TypeUsage typeUsage)
        {
            if (perspective.TryGetTypeByName(parameter.MappableType.FullName, false, out typeUsage) && TypeSemantics.IsPrimitiveType(typeUsage))
            {
                return true;
            }
            typeUsage = null;
            return false;
        }

        internal abstract System.Linq.Expressions.Expression Expression { get; }

        internal abstract ObjectParameter Parameter { get; }

        internal abstract ObjectQuery Query { get; }

        private class NestedLogicBinding : ClosureBinding
        {
            private MergeOption? _mergeOption;
            private object _nestedLogic;

            internal NestedLogicBinding(System.Linq.Expressions.Expression sourceExpression, object nestedLogic, MergeOption? mergeOption) : base(sourceExpression)
            {
                this._nestedLogic = nestedLogic;
                this._mergeOption = mergeOption;
            }

            internal override ClosureBinding CopyToContext(ExpressionConverter context) => 
                this;

            internal override bool EvaluateBinding()
            {
                object objB = ExpressionEvaluator.EvaluateExpression(base._sourceExpression);
                if (object.ReferenceEquals(this._nestedLogic, objB))
                {
                    ObjectQuery query = objB as ObjectQuery;
                    if (query == null)
                    {
                        return false;
                    }
                    MergeOption? userSpecifiedMergeOption = query.QueryState.UserSpecifiedMergeOption;
                    if (!userSpecifiedMergeOption.HasValue && !this._mergeOption.HasValue)
                    {
                        return false;
                    }
                    if ((userSpecifiedMergeOption.HasValue && this._mergeOption.HasValue) && (userSpecifiedMergeOption.Value == this._mergeOption.Value))
                    {
                        return false;
                    }
                }
                return true;
            }

            internal override System.Linq.Expressions.Expression Expression =>
                (this._nestedLogic as System.Linq.Expressions.Expression);

            internal override ObjectParameter Parameter =>
                null;

            internal override ObjectQuery Query =>
                (this._nestedLogic as ObjectQuery);
        }

        private class ParameterBinding : ClosureBinding
        {
            private ObjectParameter _parameter;

            internal ParameterBinding(System.Linq.Expressions.Expression sourceExpression, ObjectParameter parameter) : base(sourceExpression)
            {
                this._parameter = parameter;
            }

            internal override ClosureBinding CopyToContext(ExpressionConverter context)
            {
                ObjectParameter parameter = null;
                if (context.Parameters != null)
                {
                    parameter = context.Parameters[this._parameter.Name];
                }
                return new ClosureBinding.ParameterBinding(base._sourceExpression, parameter);
            }

            internal override bool EvaluateBinding()
            {
                object obj2 = ExpressionEvaluator.EvaluateExpression(base._sourceExpression);
                this._parameter.Value = obj2;
                return false;
            }

            internal override System.Linq.Expressions.Expression Expression =>
                null;

            internal override ObjectParameter Parameter =>
                this._parameter;

            internal override ObjectQuery Query =>
                null;
        }
    }
}

