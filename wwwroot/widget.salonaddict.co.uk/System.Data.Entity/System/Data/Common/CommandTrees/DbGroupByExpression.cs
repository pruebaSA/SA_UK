namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;

    public sealed class DbGroupByExpression : DbExpression
    {
        private IList<DbAggregate> _aggregates;
        private DbGroupExpressionBinding _input;
        private IList<DbExpression> _keys;

        internal DbGroupByExpression(DbCommandTree cmdTree, DbGroupExpressionBinding input, IList<KeyValuePair<string, DbExpression>> groupKeys, IList<KeyValuePair<string, DbAggregate>> aggregates) : base(cmdTree, DbExpressionKind.GroupBy)
        {
            List<string> colNames = new List<string>();
            List<KeyValuePair<string, TypeUsage>> columns = new List<KeyValuePair<string, TypeUsage>>();
            List<DbExpression> keys = new List<DbExpression>();
            List<DbAggregate> aggs = new List<DbAggregate>();
            DbGroupExpressionBinding.Check("Input", input, cmdTree);
            CommandTreeUtils.CheckNamedList<DbExpression>("groupKeys", groupKeys, true, delegate (KeyValuePair<string, DbExpression> keyInfo, int index) {
                if (!TypeHelpers.IsValidGroupKeyType(keyInfo.Value.ResultType))
                {
                    throw EntityUtil.Argument(Strings.Cqt_GroupBy_KeyNotEqualityComparable(keyInfo.Key));
                }
                keys.Add(keyInfo.Value);
                colNames.Add(keyInfo.Key);
                columns.Add(new KeyValuePair<string, TypeUsage>(keyInfo.Key, keyInfo.Value.ResultType));
            });
            CommandTreeUtils.CheckNamedList<DbAggregate>("aggregates", aggregates, true, delegate (KeyValuePair<string, DbAggregate> aggInfo, int idx) {
                if (aggInfo.Value.CommandTree != cmdTree)
                {
                    throw EntityUtil.Argument(Strings.Cqt_GroupBy_AggregateTreeMismatch);
                }
                if (colNames.Contains(aggInfo.Key))
                {
                    throw EntityUtil.Argument(Strings.Cqt_GroupBy_AggregateColumnExistsAsGroupColumn(aggInfo.Key));
                }
                aggs.Add(aggInfo.Value);
                columns.Add(new KeyValuePair<string, TypeUsage>(aggInfo.Key, aggInfo.Value.ResultType));
            });
            if ((keys.Count == 0) && (aggs.Count == 0))
            {
                throw EntityUtil.Argument(Strings.Cqt_GroupBy_AtLeastOneKeyOrAggregate);
            }
            this._input = input;
            this._keys = new ExpressionList("Keys", cmdTree, keys);
            this._aggregates = aggs.AsReadOnly();
            base.ResultType = CommandTreeTypeHelper.CreateCollectionOfRowResultType(columns);
        }

        public override void Accept(DbExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw EntityUtil.ArgumentNull("visitor");
            }
            visitor.Visit(this);
        }

        public override TResultType Accept<TResultType>(DbExpressionVisitor<TResultType> visitor) => 
            visitor?.Visit(this);

        public IList<DbAggregate> Aggregates =>
            this._aggregates;

        public DbGroupExpressionBinding Input =>
            this._input;

        public IList<DbExpression> Keys =>
            this._keys;
    }
}

