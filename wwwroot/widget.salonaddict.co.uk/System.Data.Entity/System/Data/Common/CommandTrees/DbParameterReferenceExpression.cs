namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;

    public sealed class DbParameterReferenceExpression : DbExpression
    {
        private string _name;

        internal DbParameterReferenceExpression(DbCommandTree cmdTree, TypeUsage type, string name) : base(cmdTree, DbExpressionKind.ParameterReference)
        {
            EntityUtil.CheckArgumentNull<string>(name, "name");
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

        public string ParameterName =>
            this._name;
    }
}

