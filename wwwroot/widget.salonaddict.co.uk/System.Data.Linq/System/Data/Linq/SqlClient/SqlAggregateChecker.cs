namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlAggregateChecker
    {
        private Visitor visitor = new Visitor();

        internal SqlAggregateChecker()
        {
        }

        internal bool HasAggregates(SqlNode node)
        {
            this.visitor.hasAggregates = false;
            this.visitor.Visit(node);
            return this.visitor.hasAggregates;
        }

        private class Visitor : SqlVisitor
        {
            internal bool hasAggregates;

            internal Visitor()
            {
            }

            internal override SqlSource VisitSource(SqlSource source) => 
                source;

            internal override SqlExpression VisitSubSelect(SqlSubSelect ss) => 
                ss;

            internal override SqlExpression VisitUnaryOperator(SqlUnary uo)
            {
                SqlNodeType nodeType = uo.NodeType;
                if (nodeType <= SqlNodeType.LongCount)
                {
                    switch (nodeType)
                    {
                        case SqlNodeType.Avg:
                        case SqlNodeType.Count:
                        case SqlNodeType.LongCount:
                            goto Label_002B;
                    }
                    goto Label_0034;
                }
                if (((nodeType != SqlNodeType.Max) && (nodeType != SqlNodeType.Min)) && (nodeType != SqlNodeType.Sum))
                {
                    goto Label_0034;
                }
            Label_002B:
                this.hasAggregates = true;
                return uo;
            Label_0034:
                return base.VisitUnaryOperator(uo);
            }
        }
    }
}

