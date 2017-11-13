namespace System.Data.Common.CommandTrees
{
    using System;

    public abstract class DbExpressionVisitor
    {
        protected DbExpressionVisitor()
        {
        }

        public abstract void Visit(DbAndExpression expression);
        public abstract void Visit(DbApplyExpression expression);
        public abstract void Visit(DbArithmeticExpression expression);
        public abstract void Visit(DbCaseExpression expression);
        public abstract void Visit(DbCastExpression expression);
        public abstract void Visit(DbComparisonExpression expression);
        public abstract void Visit(DbConstantExpression expression);
        public abstract void Visit(DbCrossJoinExpression expression);
        public abstract void Visit(DbDerefExpression expression);
        public abstract void Visit(DbDistinctExpression expression);
        public abstract void Visit(DbElementExpression expression);
        public abstract void Visit(DbEntityRefExpression expression);
        public abstract void Visit(DbExceptExpression expression);
        public abstract void Visit(DbExpression expression);
        public abstract void Visit(DbFilterExpression expression);
        public abstract void Visit(DbFunctionExpression expression);
        public abstract void Visit(DbGroupByExpression expression);
        public abstract void Visit(DbIntersectExpression expression);
        public abstract void Visit(DbIsEmptyExpression expression);
        public abstract void Visit(DbIsNullExpression expression);
        public abstract void Visit(DbIsOfExpression expression);
        public abstract void Visit(DbJoinExpression expression);
        public abstract void Visit(DbLikeExpression expression);
        public abstract void Visit(DbLimitExpression expression);
        public abstract void Visit(DbNewInstanceExpression expression);
        public abstract void Visit(DbNotExpression expression);
        public abstract void Visit(DbNullExpression expression);
        public abstract void Visit(DbOfTypeExpression expression);
        public abstract void Visit(DbOrExpression expression);
        public abstract void Visit(DbParameterReferenceExpression expression);
        public abstract void Visit(DbProjectExpression expression);
        public abstract void Visit(DbPropertyExpression expression);
        public abstract void Visit(DbQuantifierExpression expression);
        public abstract void Visit(DbRefExpression expression);
        public abstract void Visit(DbRefKeyExpression expression);
        public abstract void Visit(DbRelationshipNavigationExpression expression);
        public abstract void Visit(DbScanExpression expression);
        public abstract void Visit(DbSkipExpression expression);
        public abstract void Visit(DbSortExpression expression);
        public abstract void Visit(DbTreatExpression expression);
        public abstract void Visit(DbUnionAllExpression expression);
        public abstract void Visit(DbVariableReferenceExpression expression);
    }
}

