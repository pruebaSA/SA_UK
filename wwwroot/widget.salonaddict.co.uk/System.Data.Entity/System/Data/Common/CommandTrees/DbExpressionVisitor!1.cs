namespace System.Data.Common.CommandTrees
{
    using System;

    public abstract class DbExpressionVisitor<TResultType>
    {
        protected DbExpressionVisitor()
        {
        }

        public abstract TResultType Visit(DbAndExpression expression);
        public abstract TResultType Visit(DbApplyExpression expression);
        public abstract TResultType Visit(DbArithmeticExpression expression);
        public abstract TResultType Visit(DbCaseExpression expression);
        public abstract TResultType Visit(DbCastExpression expression);
        public abstract TResultType Visit(DbComparisonExpression expression);
        public abstract TResultType Visit(DbConstantExpression expression);
        public abstract TResultType Visit(DbCrossJoinExpression expression);
        public abstract TResultType Visit(DbDerefExpression expression);
        public abstract TResultType Visit(DbDistinctExpression expression);
        public abstract TResultType Visit(DbElementExpression expression);
        public abstract TResultType Visit(DbEntityRefExpression expression);
        public abstract TResultType Visit(DbExceptExpression expression);
        public abstract TResultType Visit(DbExpression expression);
        public abstract TResultType Visit(DbFilterExpression expression);
        public abstract TResultType Visit(DbFunctionExpression expression);
        public abstract TResultType Visit(DbGroupByExpression expression);
        public abstract TResultType Visit(DbIntersectExpression expression);
        public abstract TResultType Visit(DbIsEmptyExpression expression);
        public abstract TResultType Visit(DbIsNullExpression expression);
        public abstract TResultType Visit(DbIsOfExpression expression);
        public abstract TResultType Visit(DbJoinExpression expression);
        public abstract TResultType Visit(DbLikeExpression expression);
        public abstract TResultType Visit(DbLimitExpression expression);
        public abstract TResultType Visit(DbNewInstanceExpression expression);
        public abstract TResultType Visit(DbNotExpression expression);
        public abstract TResultType Visit(DbNullExpression expression);
        public abstract TResultType Visit(DbOfTypeExpression expression);
        public abstract TResultType Visit(DbOrExpression expression);
        public abstract TResultType Visit(DbParameterReferenceExpression expression);
        public abstract TResultType Visit(DbProjectExpression expression);
        public abstract TResultType Visit(DbPropertyExpression expression);
        public abstract TResultType Visit(DbQuantifierExpression expression);
        public abstract TResultType Visit(DbRefExpression expression);
        public abstract TResultType Visit(DbRefKeyExpression expression);
        public abstract TResultType Visit(DbRelationshipNavigationExpression expression);
        public abstract TResultType Visit(DbScanExpression expression);
        public abstract TResultType Visit(DbSkipExpression expression);
        public abstract TResultType Visit(DbSortExpression expression);
        public abstract TResultType Visit(DbTreatExpression expression);
        public abstract TResultType Visit(DbUnionAllExpression expression);
        public abstract TResultType Visit(DbVariableReferenceExpression expression);
    }
}

