namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    internal abstract class BasicExpressionVisitor : DbExpressionVisitor
    {
        protected BasicExpressionVisitor()
        {
        }

        public override void Visit(DbAndExpression expression)
        {
            this.VisitBinaryExpression(expression);
        }

        public override void Visit(DbApplyExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbApplyExpression>(expression, "expression");
            this.VisitExpressionBindingPre(expression.Input);
            if (expression.Apply != null)
            {
                this.VisitExpression(expression.Apply.Expression);
            }
            this.VisitExpressionBindingPost(expression.Input);
        }

        public override void Visit(DbArithmeticExpression expression)
        {
            this.VisitExpressionList(EntityUtil.CheckArgumentNull<DbArithmeticExpression>(expression, "expression").Arguments);
        }

        public override void Visit(DbCaseExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbCaseExpression>(expression, "expression");
            this.VisitExpressionList(expression.When);
            this.VisitExpressionList(expression.Then);
            this.VisitExpression(expression.Else);
        }

        public override void Visit(DbCastExpression expression)
        {
            this.VisitUnaryExpression(expression);
        }

        public override void Visit(DbComparisonExpression expression)
        {
            this.VisitBinaryExpression(expression);
        }

        public override void Visit(DbConstantExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbConstantExpression>(expression, "expression");
        }

        public override void Visit(DbCrossJoinExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbCrossJoinExpression>(expression, "expression");
            foreach (DbExpressionBinding binding in expression.Inputs)
            {
                this.VisitExpressionBindingPre(binding);
            }
            foreach (DbExpressionBinding binding2 in expression.Inputs)
            {
                this.VisitExpressionBindingPost(binding2);
            }
        }

        public override void Visit(DbDerefExpression expression)
        {
            this.VisitUnaryExpression(expression);
        }

        public override void Visit(DbDistinctExpression expression)
        {
            this.VisitUnaryExpression(expression);
        }

        public override void Visit(DbElementExpression expression)
        {
            this.VisitUnaryExpression(expression);
        }

        public override void Visit(DbEntityRefExpression expression)
        {
            this.VisitUnaryExpression(expression);
        }

        public override void Visit(DbExceptExpression expression)
        {
            this.VisitBinaryExpression(expression);
        }

        public override void Visit(DbExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbExpression>(expression, "expression");
            throw EntityUtil.NotSupported(Strings.Cqt_General_UnsupportedExpression(expression.GetType().FullName));
        }

        public override void Visit(DbFilterExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbFilterExpression>(expression, "expression");
            this.VisitExpressionBindingPre(expression.Input);
            this.VisitExpression(expression.Predicate);
            this.VisitExpressionBindingPost(expression.Input);
        }

        public override void Visit(DbFunctionExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbFunctionExpression>(expression, "expression");
            this.VisitExpressionList(expression.Arguments);
            if (expression.IsLambda)
            {
                this.VisitLambdaFunctionPre(expression.Function, expression.LambdaBody);
                this.VisitExpression(expression.LambdaBody);
                this.VisitLambdaFunctionPost(expression.Function, expression.LambdaBody);
            }
        }

        public override void Visit(DbGroupByExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbGroupByExpression>(expression, "expression");
            this.VisitGroupExpressionBindingPre(expression.Input);
            this.VisitExpressionList(expression.Keys);
            this.VisitGroupExpressionBindingMid(expression.Input);
            this.VisitAggregateList(expression.Aggregates);
            this.VisitGroupExpressionBindingPost(expression.Input);
        }

        public override void Visit(DbIntersectExpression expression)
        {
            this.VisitBinaryExpression(expression);
        }

        public override void Visit(DbIsEmptyExpression expression)
        {
            this.VisitUnaryExpression(expression);
        }

        public override void Visit(DbIsNullExpression expression)
        {
            this.VisitUnaryExpression(expression);
        }

        public override void Visit(DbIsOfExpression expression)
        {
            this.VisitUnaryExpression(expression);
        }

        public override void Visit(DbJoinExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbJoinExpression>(expression, "expression");
            this.VisitExpressionBindingPre(expression.Left);
            this.VisitExpressionBindingPre(expression.Right);
            this.VisitExpression(expression.JoinCondition);
            this.VisitExpressionBindingPost(expression.Left);
            this.VisitExpressionBindingPost(expression.Right);
        }

        public override void Visit(DbLikeExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbLikeExpression>(expression, "expression");
            this.VisitExpression(expression.Argument);
            this.VisitExpression(expression.Pattern);
            this.VisitExpression(expression.Escape);
        }

        public override void Visit(DbLimitExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbLimitExpression>(expression, "expression");
            this.VisitExpression(expression.Argument);
            this.VisitExpression(expression.Limit);
        }

        public override void Visit(DbNewInstanceExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbNewInstanceExpression>(expression, "expression");
            this.VisitExpressionList(expression.Arguments);
            if (expression.HasRelatedEntityReferences)
            {
                this.VisitRelatedEntityReferenceList(expression.RelatedEntityReferences);
            }
        }

        public override void Visit(DbNotExpression expression)
        {
            this.VisitUnaryExpression(expression);
        }

        public override void Visit(DbNullExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbNullExpression>(expression, "expression");
        }

        public override void Visit(DbOfTypeExpression expression)
        {
            this.VisitUnaryExpression(expression);
        }

        public override void Visit(DbOrExpression expression)
        {
            this.VisitBinaryExpression(expression);
        }

        public override void Visit(DbParameterReferenceExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbParameterReferenceExpression>(expression, "expression");
        }

        public override void Visit(DbProjectExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbProjectExpression>(expression, "expression");
            this.VisitExpressionBindingPre(expression.Input);
            this.VisitExpression(expression.Projection);
            this.VisitExpressionBindingPost(expression.Input);
        }

        public override void Visit(DbPropertyExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbPropertyExpression>(expression, "expression");
            if (expression.Instance != null)
            {
                this.VisitExpression(expression.Instance);
            }
        }

        public override void Visit(DbQuantifierExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbQuantifierExpression>(expression, "expression");
            this.VisitExpressionBindingPre(expression.Input);
            this.VisitExpression(expression.Predicate);
            this.VisitExpressionBindingPost(expression.Input);
        }

        public override void Visit(DbRefExpression expression)
        {
            this.VisitUnaryExpression(expression);
        }

        public override void Visit(DbRefKeyExpression expression)
        {
            this.VisitUnaryExpression(expression);
        }

        public override void Visit(DbRelationshipNavigationExpression expression)
        {
            this.VisitExpression(EntityUtil.CheckArgumentNull<DbRelationshipNavigationExpression>(expression, "expression").NavigationSource);
        }

        public override void Visit(DbScanExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbScanExpression>(expression, "expression");
        }

        public override void Visit(DbSkipExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbSkipExpression>(expression, "expression");
            this.VisitExpressionBindingPre(expression.Input);
            foreach (DbSortClause clause in expression.SortOrder)
            {
                this.VisitExpression(clause.Expression);
            }
            this.VisitExpressionBindingPost(expression.Input);
            this.VisitExpression(expression.Count);
        }

        public override void Visit(DbSortExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbSortExpression>(expression, "expression");
            this.VisitExpressionBindingPre(expression.Input);
            for (int i = 0; i < expression.SortOrder.Count; i++)
            {
                this.VisitExpression(expression.SortOrder[i].Expression);
            }
            this.VisitExpressionBindingPost(expression.Input);
        }

        public override void Visit(DbTreatExpression expression)
        {
            this.VisitUnaryExpression(expression);
        }

        public override void Visit(DbUnionAllExpression expression)
        {
            this.VisitBinaryExpression(expression);
        }

        public override void Visit(DbVariableReferenceExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbVariableReferenceExpression>(expression, "expression");
        }

        public virtual void VisitAggregate(DbAggregate aggregate)
        {
            this.VisitExpressionList(EntityUtil.CheckArgumentNull<DbAggregate>(aggregate, "aggregate").Arguments);
        }

        public virtual void VisitAggregateList(IList<DbAggregate> aggregates)
        {
            EntityUtil.CheckArgumentNull<IList<DbAggregate>>(aggregates, "aggregates");
            for (int i = 0; i < aggregates.Count; i++)
            {
                this.VisitAggregate(aggregates[i]);
            }
        }

        protected virtual void VisitBinaryExpression(DbBinaryExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbBinaryExpression>(expression, "expression");
            this.VisitExpression(expression.Left);
            this.VisitExpression(expression.Right);
        }

        public virtual void VisitExpression(DbExpression expression)
        {
            EntityUtil.CheckArgumentNull<DbExpression>(expression, "expression").Accept(this);
        }

        protected virtual void VisitExpressionBindingPost(DbExpressionBinding binding)
        {
        }

        protected virtual void VisitExpressionBindingPre(DbExpressionBinding binding)
        {
            EntityUtil.CheckArgumentNull<DbExpressionBinding>(binding, "binding");
            this.VisitExpression(binding.Expression);
        }

        public virtual void VisitExpressionList(IList<DbExpression> expressionList)
        {
            EntityUtil.CheckArgumentNull<IList<DbExpression>>(expressionList, "expressionList");
            for (int i = 0; i < expressionList.Count; i++)
            {
                this.VisitExpression(expressionList[i]);
            }
        }

        protected virtual void VisitGroupExpressionBindingMid(DbGroupExpressionBinding binding)
        {
        }

        protected virtual void VisitGroupExpressionBindingPost(DbGroupExpressionBinding binding)
        {
        }

        protected virtual void VisitGroupExpressionBindingPre(DbGroupExpressionBinding binding)
        {
            EntityUtil.CheckArgumentNull<DbGroupExpressionBinding>(binding, "binding");
            this.VisitExpression(binding.Expression);
        }

        protected virtual void VisitLambdaFunctionPost(EdmFunction function, DbExpression body)
        {
        }

        protected virtual void VisitLambdaFunctionPre(EdmFunction function, DbExpression body)
        {
            EntityUtil.CheckArgumentNull<EdmFunction>(function, "function");
            EntityUtil.CheckArgumentNull<DbExpression>(body, "body");
        }

        internal virtual void VisitRelatedEntityReference(DbRelatedEntityRef relatedEntityRef)
        {
            this.VisitExpression(relatedEntityRef.TargetEntityReference);
        }

        internal virtual void VisitRelatedEntityReferenceList(IList<DbRelatedEntityRef> relatedEntityReferences)
        {
            for (int i = 0; i < relatedEntityReferences.Count; i++)
            {
                this.VisitRelatedEntityReference(relatedEntityReferences[i]);
            }
        }

        protected virtual void VisitUnaryExpression(DbUnaryExpression expression)
        {
            this.VisitExpression(EntityUtil.CheckArgumentNull<DbUnaryExpression>(expression, "expression").Argument);
        }
    }
}

