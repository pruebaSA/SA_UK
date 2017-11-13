namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbCaseExpression : DbExpression
    {
        private ExpressionLink _else;
        private ExpressionList _then;
        private ExpressionList _when;

        internal DbCaseExpression(DbCommandTree cmdTree, IList<DbExpression> whens, IList<DbExpression> thens, DbExpression elseExpr) : base(cmdTree, DbExpressionKind.Case)
        {
            EntityUtil.CheckArgumentNull<IList<DbExpression>>(whens, "whens");
            EntityUtil.CheckArgumentNull<IList<DbExpression>>(thens, "thens");
            if (whens.Count != thens.Count)
            {
                throw EntityUtil.Argument(Strings.Cqt_Case_WhensMustEqualThens);
            }
            if (whens.Count == 0)
            {
                throw EntityUtil.Argument(Strings.Cqt_Case_AtLeastOneClause);
            }
            TypeUsage resultType = null;
            for (int i = 0; i < whens.Count; i++)
            {
                if (thens[i] == null)
                {
                    throw EntityUtil.ArgumentNull(CommandTreeUtils.FormatIndex("Thens", i));
                }
                if (resultType == null)
                {
                    resultType = thens[i].ResultType;
                }
                else
                {
                    resultType = TypeHelpers.GetCommonTypeUsage(thens[i].ResultType, resultType);
                    if (resultType == null)
                    {
                        break;
                    }
                }
            }
            if ((resultType != null) && (elseExpr != null))
            {
                resultType = TypeHelpers.GetCommonTypeUsage(elseExpr.ResultType, resultType);
            }
            if (TypeSemantics.IsNullOrNullType(resultType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Case_InvalidResultType);
            }
            this._when = new ExpressionList("When", cmdTree, PrimitiveTypeKind.Boolean, whens);
            this._then = new ExpressionList("Then", cmdTree, resultType, thens);
            this._else = new ExpressionLink("Else", cmdTree, resultType, elseExpr);
            base.ResultType = resultType;
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

        public DbExpression Else
        {
            get => 
                this._else.Expression;
            internal set
            {
                this._else.Expression = value;
            }
        }

        public IList<DbExpression> Then =>
            this._then;

        public IList<DbExpression> When =>
            this._when;
    }
}

