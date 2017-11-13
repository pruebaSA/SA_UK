namespace System.Data.Common.EntitySql
{
    using System;
    using System.Data.Common.CommandTrees;

    internal sealed class GroupAggregateInfo
    {
        private DbAggregate _aggregateExpression;
        private string _aggregateName;

        internal GroupAggregateInfo(string aggregateName, DbAggregate aggregateExpression)
        {
            this._aggregateName = aggregateName;
            this._aggregateExpression = aggregateExpression;
        }

        internal DbAggregate AggregateExpression =>
            this._aggregateExpression;

        internal string AggregateName =>
            this._aggregateName;
    }
}

