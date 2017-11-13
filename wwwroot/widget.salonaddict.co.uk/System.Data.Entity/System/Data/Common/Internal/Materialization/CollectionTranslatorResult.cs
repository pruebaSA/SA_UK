namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Data.Query.InternalTrees;
    using System.Linq.Expressions;

    internal class CollectionTranslatorResult : TranslatorResult
    {
        internal readonly Expression ExpressionToGetCoordinator;

        internal CollectionTranslatorResult(Expression returnedExpression, ColumnMap columnMap, Type requestedType, Expression expressionToGetCoordinator) : base(returnedExpression, requestedType)
        {
            this.ExpressionToGetCoordinator = expressionToGetCoordinator;
        }
    }
}

