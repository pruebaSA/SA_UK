namespace System.Data.SqlClient.SqlGen
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Runtime.CompilerServices;

    internal class Sql8ConformanceChecker : DbExpressionVisitor<bool>
    {
        private Sql8ConformanceChecker()
        {
        }

        internal static bool NeedsRewrite(DbExpression expr)
        {
            Sql8ConformanceChecker visitor = new Sql8ConformanceChecker();
            return expr.Accept<bool>(visitor);
        }

        public override bool Visit(DbAndExpression expression) => 
            this.VisitBinaryExpression(expression);

        public override bool Visit(DbApplyExpression expression)
        {
            throw EntityUtil.NotSupported(Strings.ApplyNotSupportedOnSql8);
        }

        public override bool Visit(DbArithmeticExpression expression) => 
            this.VisitExpressionList(expression.Arguments);

        public override bool Visit(DbCaseExpression expression)
        {
            bool flag = this.VisitExpressionList(expression.When);
            bool flag2 = this.VisitExpressionList(expression.Then);
            bool flag3 = this.VisitExpression(expression.Else);
            if (!flag && !flag2)
            {
                return flag3;
            }
            return true;
        }

        public override bool Visit(DbCastExpression expression) => 
            this.VisitUnaryExpression(expression);

        public override bool Visit(DbComparisonExpression expression) => 
            this.VisitBinaryExpression(expression);

        public override bool Visit(DbConstantExpression expression) => 
            false;

        public override bool Visit(DbCrossJoinExpression expression) => 
            this.VisitExpressionBindingList(expression.Inputs);

        public override bool Visit(DbDerefExpression expression) => 
            this.VisitUnaryExpression(expression);

        public override bool Visit(DbDistinctExpression expression) => 
            this.VisitUnaryExpression(expression);

        public override bool Visit(DbElementExpression expression) => 
            this.VisitUnaryExpression(expression);

        public override bool Visit(DbEntityRefExpression expression) => 
            this.VisitUnaryExpression(expression);

        public override bool Visit(DbExceptExpression expression)
        {
            this.VisitExpression(expression.Left);
            this.VisitExpression(expression.Right);
            return true;
        }

        public override bool Visit(DbExpression expression)
        {
            throw EntityUtil.NotSupported(Strings.Cqt_General_UnsupportedExpression(expression.GetType().FullName));
        }

        public override bool Visit(DbFilterExpression expression)
        {
            bool flag = this.VisitExpressionBinding(expression.Input);
            bool flag2 = this.VisitExpression(expression.Predicate);
            if (!flag)
            {
                return flag2;
            }
            return true;
        }

        public override bool Visit(DbFunctionExpression expression) => 
            this.VisitExpressionList(expression.Arguments);

        public override bool Visit(DbGroupByExpression expression)
        {
            bool flag = this.VisitExpression(expression.Input.Expression);
            bool flag2 = this.VisitExpressionList(expression.Keys);
            bool flag3 = this.VisitAggregateList(expression.Aggregates);
            if (!flag && !flag2)
            {
                return flag3;
            }
            return true;
        }

        public override bool Visit(DbIntersectExpression expression)
        {
            this.VisitExpression(expression.Left);
            this.VisitExpression(expression.Right);
            return true;
        }

        public override bool Visit(DbIsEmptyExpression expression) => 
            this.VisitUnaryExpression(expression);

        public override bool Visit(DbIsNullExpression expression) => 
            this.VisitUnaryExpression(expression);

        public override bool Visit(DbIsOfExpression expression) => 
            this.VisitUnaryExpression(expression);

        public override bool Visit(DbJoinExpression expression)
        {
            bool flag = this.VisitExpressionBinding(expression.Left);
            bool flag2 = this.VisitExpressionBinding(expression.Right);
            bool flag3 = this.VisitExpression(expression.JoinCondition);
            if (!flag && !flag2)
            {
                return flag3;
            }
            return true;
        }

        public override bool Visit(DbLikeExpression expression)
        {
            bool flag = this.VisitExpression(expression.Argument);
            bool flag2 = this.VisitExpression(expression.Pattern);
            bool flag3 = this.VisitExpression(expression.Escape);
            if (!flag && !flag2)
            {
                return flag3;
            }
            return true;
        }

        public override bool Visit(DbLimitExpression expression)
        {
            if (expression.Limit is DbParameterReferenceExpression)
            {
                throw EntityUtil.NotSupported(Strings.ParameterForLimitNotSupportedOnSql8);
            }
            return this.VisitExpression(expression.Argument);
        }

        public override bool Visit(DbNewInstanceExpression expression) => 
            this.VisitExpressionList(expression.Arguments);

        public override bool Visit(DbNotExpression expression) => 
            this.VisitUnaryExpression(expression);

        public override bool Visit(DbNullExpression expression) => 
            false;

        public override bool Visit(DbOfTypeExpression expression) => 
            this.VisitUnaryExpression(expression);

        public override bool Visit(DbOrExpression expression) => 
            this.VisitBinaryExpression(expression);

        public override bool Visit(DbParameterReferenceExpression expression) => 
            false;

        public override bool Visit(DbProjectExpression expression)
        {
            bool flag = this.VisitExpressionBinding(expression.Input);
            bool flag2 = this.VisitExpression(expression.Projection);
            if (!flag)
            {
                return flag2;
            }
            return true;
        }

        public override bool Visit(DbPropertyExpression expression) => 
            this.VisitExpression(expression.Instance);

        public override bool Visit(DbQuantifierExpression expression)
        {
            bool flag = this.VisitExpressionBinding(expression.Input);
            bool flag2 = this.VisitExpression(expression.Predicate);
            if (!flag)
            {
                return flag2;
            }
            return true;
        }

        public override bool Visit(DbRefExpression expression) => 
            this.VisitUnaryExpression(expression);

        public override bool Visit(DbRefKeyExpression expression) => 
            this.VisitUnaryExpression(expression);

        public override bool Visit(DbRelationshipNavigationExpression expression) => 
            this.VisitExpression(expression.NavigationSource);

        public override bool Visit(DbScanExpression expression) => 
            false;

        public override bool Visit(DbSkipExpression expression)
        {
            if (expression.Count is DbParameterReferenceExpression)
            {
                throw EntityUtil.NotSupported(Strings.ParameterForSkipNotSupportedOnSql8);
            }
            this.VisitExpressionBinding(expression.Input);
            this.VisitSortClauseList(expression.SortOrder);
            this.VisitExpression(expression.Count);
            return true;
        }

        public override bool Visit(DbSortExpression expression)
        {
            bool flag = this.VisitExpressionBinding(expression.Input);
            bool flag2 = this.VisitSortClauseList(expression.SortOrder);
            if (!flag)
            {
                return flag2;
            }
            return true;
        }

        public override bool Visit(DbTreatExpression expression) => 
            this.VisitUnaryExpression(expression);

        public override bool Visit(DbUnionAllExpression expression) => 
            this.VisitBinaryExpression(expression);

        public override bool Visit(DbVariableReferenceExpression expression) => 
            false;

        private bool VisitAggregate(DbAggregate aggregate) => 
            this.VisitExpressionList(aggregate.ArgumentList);

        private bool VisitAggregateList(IList<DbAggregate> list) => 
            VisitList<DbAggregate>(new ListElementHandler<DbAggregate>(this.VisitAggregate), list);

        private bool VisitBinaryExpression(DbBinaryExpression expr)
        {
            bool flag = this.VisitExpression(expr.Left);
            bool flag2 = this.VisitExpression(expr.Right);
            if (!flag)
            {
                return flag2;
            }
            return true;
        }

        private bool VisitExpression(DbExpression expression) => 
            expression?.Accept<bool>(this);

        private bool VisitExpressionBinding(DbExpressionBinding expressionBinding) => 
            this.VisitExpression(expressionBinding.Expression);

        private bool VisitExpressionBindingList(IList<DbExpressionBinding> list) => 
            VisitList<DbExpressionBinding>(new ListElementHandler<DbExpressionBinding>(this.VisitExpressionBinding), list);

        private bool VisitExpressionList(IList<DbExpression> list) => 
            VisitList<DbExpression>(new ListElementHandler<DbExpression>(this.VisitExpression), list);

        private static bool VisitList<TElementType>(ListElementHandler<TElementType> handler, IList<TElementType> list)
        {
            bool flag = false;
            foreach (TElementType local in list)
            {
                bool flag2 = handler(local);
                flag = flag || flag2;
            }
            return flag;
        }

        private bool VisitSortClause(DbSortClause sortClause) => 
            this.VisitExpression(sortClause.Expression);

        private bool VisitSortClauseList(IList<DbSortClause> list) => 
            VisitList<DbSortClause>(new ListElementHandler<DbSortClause>(this.VisitSortClause), list);

        private bool VisitUnaryExpression(DbUnaryExpression expr) => 
            this.VisitExpression(expr.Argument);

        private delegate bool ListElementHandler<TElementType>(TElementType element);
    }
}

