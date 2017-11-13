namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbPropertyExpression : DbExpression
    {
        private ExpressionLink _instance;
        private EdmMember _property;

        internal DbPropertyExpression(DbCommandTree cmdTree, EdmProperty propertyInfo, DbExpression instance) : base(cmdTree, DbExpressionKind.Property)
        {
            cmdTree.TypeHelper.CheckMember(propertyInfo, "propertyInfo");
            this.InitializeFromMember(cmdTree, propertyInfo, instance);
        }

        internal DbPropertyExpression(DbCommandTree cmdTree, NavigationProperty propertyInfo, DbExpression instance) : base(cmdTree, DbExpressionKind.Property)
        {
            cmdTree.TypeHelper.CheckMember(propertyInfo, "propertyInfo");
            this.InitializeFromMember(cmdTree, propertyInfo, instance);
        }

        internal DbPropertyExpression(DbCommandTree cmdTree, RelationshipEndMember memberInfo, DbExpression instance) : base(cmdTree, DbExpressionKind.Property)
        {
            cmdTree.TypeHelper.CheckMember(memberInfo, "memberInfo");
            this.InitializeFromMember(cmdTree, memberInfo, instance);
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

        private void InitializeFromMember(DbCommandTree tree, EdmMember member, DbExpression instance)
        {
            if (instance == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_Property_InstanceRequiredForInstance, "instance");
            }
            TypeUsage expectedType = TypeUsage.Create(member.DeclaringType);
            this._instance = new ExpressionLink("Instance", tree, expectedType, instance);
            this._property = member;
            base.ResultType = Helper.GetModelTypeUsage(member);
        }

        public DbExpression Instance
        {
            get => 
                this._instance.Expression;
            internal set
            {
                this._instance.Expression = value;
            }
        }

        public EdmMember Property =>
            this._property;
    }
}

