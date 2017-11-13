namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbComparisonExpression : DbBinaryExpression
    {
        internal DbComparisonExpression(DbCommandTree commandTree, DbExpressionKind kind, DbExpression left, DbExpression right) : base(commandTree, kind)
        {
            base.LeftLink.InitializeValue(left);
            base.RightLink.InitializeValue(right);
            this.CheckComparison(left, right);
            base.LeftLink.ValueChanging += new ExpressionLinkConstraint(this.ArgumentChanging);
            base.RightLink.ValueChanging += new ExpressionLinkConstraint(this.ArgumentChanging);
            base.ResultType = commandTree.TypeHelper.CreateBooleanResultType();
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

        private void ArgumentChanging(ExpressionLink link, DbExpression newValue)
        {
            if (base.LeftLink == link)
            {
                this.CheckComparison(newValue, base.Right);
            }
            else
            {
                this.CheckComparison(base.Left, newValue);
            }
        }

        private void CheckComparison(DbExpression left, DbExpression right)
        {
            bool flag = true;
            bool flag2 = true;
            if ((DbExpressionKind.GreaterThanOrEquals == base.ExpressionKind) || (DbExpressionKind.LessThanOrEquals == base.ExpressionKind))
            {
                flag = TypeSemantics.IsEqualComparableTo(left.ResultType, right.ResultType);
                flag2 = TypeSemantics.IsOrderComparableTo(left.ResultType, right.ResultType);
            }
            else if ((DbExpressionKind.Equals == base.ExpressionKind) || (DbExpressionKind.NotEquals == base.ExpressionKind))
            {
                flag = TypeSemantics.IsEqualComparableTo(left.ResultType, right.ResultType);
            }
            else
            {
                flag2 = TypeSemantics.IsOrderComparableTo(left.ResultType, right.ResultType);
            }
            if (!flag || !flag2)
            {
                throw EntityUtil.Argument(Strings.Cqt_Comparison_ComparableRequired);
            }
        }
    }
}

