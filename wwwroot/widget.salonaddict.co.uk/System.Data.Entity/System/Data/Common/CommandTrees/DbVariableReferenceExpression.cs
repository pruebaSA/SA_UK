namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;

    public sealed class DbVariableReferenceExpression : DbExpression
    {
        private string _name;

        internal DbVariableReferenceExpression(DbCommandTree cmdTree, TypeUsage type, string name) : base(cmdTree, DbExpressionKind.VariableReference)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw EntityUtil.ArgumentNull("name");
            }
            cmdTree.TypeHelper.CheckType(type);
            this._name = name;
            base.ResultType = type;
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

        public string VariableName =>
            this._name;
    }
}

