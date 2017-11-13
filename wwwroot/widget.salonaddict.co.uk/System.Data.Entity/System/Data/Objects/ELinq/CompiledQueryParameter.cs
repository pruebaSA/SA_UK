namespace System.Data.Objects.ELinq
{
    using System;
    using System.Data.Common.CommandTrees;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Linq.Expressions;

    internal sealed class CompiledQueryParameter
    {
        private readonly System.Linq.Expressions.Expression _expression;
        private readonly System.Data.Objects.ObjectParameter _parameter;
        private DbParameterReferenceExpression _parameterReference;

        internal CompiledQueryParameter(System.Linq.Expressions.Expression expression, System.Data.Objects.ObjectParameter parameter)
        {
            this._expression = expression;
            this._parameter = parameter;
        }

        internal void CreateParameterReferenceAndAddParameterToCommandTree(DbCommandTree commandTree, ClrPerspective perspective)
        {
            TypeUsage usage;
            if (ClosureBinding.TryGetTypeUsageForObjectParameter(this._parameter, perspective, out usage))
            {
                commandTree.AddParameter(this._parameter.Name, usage);
                this._parameterReference = commandTree.CreateParameterReferenceExpression(this._parameter.Name);
            }
        }

        internal System.Linq.Expressions.Expression Expression =>
            this._expression;

        internal System.Data.Objects.ObjectParameter ObjectParameter =>
            this._parameter;

        internal DbParameterReferenceExpression ParameterReference =>
            this._parameterReference;
    }
}

