namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;

    internal abstract class UpdateExpressionVisitor<TReturn> : DbExpressionVisitor<TReturn>
    {
        protected UpdateExpressionVisitor()
        {
        }

        protected NotSupportedException ConstructNotSupportedException(DbExpression node)
        {
            string str = node?.ExpressionKind.ToString();
            return EntityUtil.NotSupported(Strings.Update_UnsupportedExpressionKind(str, this.VisitorName));
        }

        public override TReturn Visit(DbAndExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbApplyExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbArithmeticExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbCaseExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbCastExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbComparisonExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbConstantExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbCrossJoinExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbDerefExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbDistinctExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbElementExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbEntityRefExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbExceptExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbExpression expression) => 
            expression?.Accept<TReturn>(this);

        public override TReturn Visit(DbFilterExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbFunctionExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbGroupByExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbIntersectExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbIsEmptyExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbIsNullExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbIsOfExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbJoinExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbLikeExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbLimitExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbNewInstanceExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbNotExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbNullExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbOfTypeExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbOrExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbParameterReferenceExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbProjectExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbPropertyExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbQuantifierExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbRefExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbRefKeyExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbRelationshipNavigationExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbScanExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbSkipExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbSortExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbTreatExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbUnionAllExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        public override TReturn Visit(DbVariableReferenceExpression expression)
        {
            throw this.ConstructNotSupportedException(expression);
        }

        protected abstract string VisitorName { get; }
    }
}

