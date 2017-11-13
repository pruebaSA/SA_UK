namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public abstract class DbBinaryExpression : DbExpression
    {
        private ExpressionLink _left;
        private ExpressionLink _right;

        internal DbBinaryExpression(DbCommandTree commandTree, DbExpressionKind kind) : base(commandTree, kind)
        {
            this._left = new ExpressionLink("Left", commandTree);
            this._right = new ExpressionLink("Right", commandTree);
        }

        internal DbBinaryExpression(DbCommandTree commandTree, DbExpressionKind kind, DbExpression left, DbExpression right) : base(commandTree, kind)
        {
            this._left = new ExpressionLink("Left", commandTree, left);
            this._right = new ExpressionLink("Right", commandTree, right);
        }

        internal TypeUsage CheckCollectionArguments()
        {
            if (!TypeSemantics.IsCollectionType(this.Left.ResultType) || !TypeSemantics.IsCollectionType(this.Right.ResultType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Binary_CollectionsRequired(base.GetType().Name));
            }
            TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(this.Left.ResultType, this.Right.ResultType);
            if (commonTypeUsage == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_Binary_CollectionsRequired(base.GetType().Name));
            }
            return commonTypeUsage;
        }

        internal void CheckComparableSetArguments()
        {
            if (!TypeHelpers.IsSetComparableOpType(TypeHelpers.GetElementTypeUsage(this.Left.ResultType)))
            {
                throw EntityUtil.Argument(Strings.Cqt_InvalidTypeForSetOperation(TypeHelpers.GetElementTypeUsage(this.Left.ResultType).Identity, base.GetType().Name), this._left.Name);
            }
            if (!TypeHelpers.IsSetComparableOpType(TypeHelpers.GetElementTypeUsage(this.Right.ResultType)))
            {
                throw EntityUtil.Argument(Strings.Cqt_InvalidTypeForSetOperation(TypeHelpers.GetElementTypeUsage(this.Right.ResultType).Identity, base.GetType().Name), this._right.Name);
            }
        }

        public DbExpression Left
        {
            get => 
                this._left.Expression;
            internal set
            {
                this._left.Expression = value;
            }
        }

        internal ExpressionLink LeftLink =>
            this._left;

        public DbExpression Right
        {
            get => 
                this._right.Expression;
            internal set
            {
                this._right.Expression = value;
            }
        }

        internal ExpressionLink RightLink =>
            this._right;
    }
}

