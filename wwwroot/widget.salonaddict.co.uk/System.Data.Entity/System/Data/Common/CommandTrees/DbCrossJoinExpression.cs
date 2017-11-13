namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;

    public sealed class DbCrossJoinExpression : DbExpression
    {
        private IList<DbExpressionBinding> _inputs;

        internal DbCrossJoinExpression(DbCommandTree cmdTree, IList<DbExpressionBinding> inputs) : base(cmdTree, DbExpressionKind.CrossJoin)
        {
            EntityUtil.CheckArgumentNull<IList<DbExpressionBinding>>(inputs, "inputs");
            List<DbExpressionBinding> list = new List<DbExpressionBinding>(inputs.Count);
            List<KeyValuePair<string, TypeUsage>> columns = new List<KeyValuePair<string, TypeUsage>>(inputs.Count);
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < inputs.Count; i++)
            {
                DbExpressionBinding binding = inputs[i];
                DbExpressionBinding.Check(CommandTreeUtils.FormatIndex("Inputs", i), binding, cmdTree);
                int num2 = -1;
                if (dictionary.TryGetValue(binding.VariableName, out num2))
                {
                    throw EntityUtil.Argument(Strings.Cqt_CrossJoin_DuplicateVariableNames(num2, i, binding.VariableName));
                }
                list.Add(binding);
                dictionary.Add(binding.VariableName, i);
                columns.Add(new KeyValuePair<string, TypeUsage>(binding.VariableName, binding.VariableType));
            }
            if (list.Count < 2)
            {
                throw EntityUtil.Argument(Strings.Cqt_CrossJoin_AtLeastTwoInputs, "inputs");
            }
            this._inputs = list.AsReadOnly();
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

        public IList<DbExpressionBinding> Inputs =>
            this._inputs;
    }
}

