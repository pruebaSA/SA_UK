namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;

    internal class SqlRemoveConstantOrderBy
    {
        internal static SqlNode Remove(SqlNode node) => 
            new Visitor().Visit(node);

        private class Visitor : SqlVisitor
        {
            internal override SqlSelect VisitSelect(SqlSelect select)
            {
                int index = 0;
                List<SqlOrderExpression> orderBy = select.OrderBy;
                while (index < orderBy.Count)
                {
                    SqlExpression discriminator = orderBy[index].Expression;
                    while (discriminator.NodeType == SqlNodeType.DiscriminatedType)
                    {
                        discriminator = ((SqlDiscriminatedType) discriminator).Discriminator;
                    }
                    switch (discriminator.NodeType)
                    {
                        case SqlNodeType.Parameter:
                        case SqlNodeType.Value:
                        {
                            orderBy.RemoveAt(index);
                            continue;
                        }
                    }
                    index++;
                }
                return base.VisitSelect(select);
            }
        }
    }
}

