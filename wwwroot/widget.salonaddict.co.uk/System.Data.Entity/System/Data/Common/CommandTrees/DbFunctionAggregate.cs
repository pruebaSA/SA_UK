namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbFunctionAggregate : DbAggregate
    {
        private EdmFunction _aggregateFunction;
        private bool _distinct;

        internal DbFunctionAggregate(DbCommandTree commandTree, EdmFunction function, DbExpression arg, bool isDistinct) : base(commandTree)
        {
            EntityUtil.CheckArgumentNull<DbExpression>(arg, "arg");
            this._distinct = isDistinct;
            commandTree.TypeHelper.CheckFunction(function);
            if ((!TypeSemantics.IsAggregateFunction(function) || (function.ReturnParameter == null)) || TypeSemantics.IsNullOrNullType(function.ReturnParameter.TypeUsage))
            {
                throw EntityUtil.Argument(Strings.Cqt_Aggregate_InvalidFunction, "function");
            }
            this._aggregateFunction = function;
            base.ArgumentList = new ExpressionList("Arguments", commandTree, function.Parameters.Count);
            List<DbExpression> list = CommandTreeUtils.CreateList<DbExpression>(arg);
            for (int i = 0; i < list.Count; i++)
            {
                TypeUsage typeUsage = function.Parameters[i].TypeUsage;
                TypeUsage elementType = null;
                if (TypeHelpers.TryGetCollectionElementType(typeUsage, out elementType))
                {
                    typeUsage = elementType;
                }
                base.ArgumentList.ExpressionLinks[i].SetExpectedType(typeUsage);
                base.ArgumentList.ExpressionLinks[i].InitializeValue(list[i]);
            }
            base.ResultType = function.ReturnParameter.TypeUsage;
        }

        public bool Distinct =>
            this._distinct;

        public EdmFunction Function =>
            this._aggregateFunction;
    }
}

