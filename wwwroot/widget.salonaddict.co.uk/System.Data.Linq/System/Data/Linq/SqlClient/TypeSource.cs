namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data.Linq.Mapping;
    using System.Runtime.CompilerServices;

    internal static class TypeSource
    {
        internal static MetaType GetSourceMetaType(SqlNode node, MetaModel model)
        {
            Visitor visitor = new Visitor();
            visitor.Visit(node);
            Type nonNullableType = TypeSystem.GetNonNullableType(visitor.sourceType);
            return model.GetMetaType(nonNullableType);
        }

        internal static SqlExpression GetTypeSource(SqlExpression expr)
        {
            Visitor visitor = new Visitor();
            visitor.Visit(expr);
            return visitor.sourceExpression;
        }

        private class Visitor : SqlVisitor
        {
            internal SqlExpression sourceExpression;
            internal Type sourceType;
            private UnwrapStack UnwrapSequences;

            internal override SqlNode Visit(SqlNode node)
            {
                if (node == null)
                {
                    return null;
                }
                this.sourceExpression = node as SqlExpression;
                if (this.sourceExpression != null)
                {
                    Type clrType = this.sourceExpression.ClrType;
                    for (UnwrapStack stack = this.UnwrapSequences; stack != null; stack = stack.Last)
                    {
                        if (stack.Unwrap)
                        {
                            clrType = TypeSystem.GetElementType(clrType);
                        }
                    }
                    this.sourceType = clrType;
                }
                if ((this.sourceType != null) && TypeSystem.GetNonNullableType(this.sourceType).IsValueType)
                {
                    return node;
                }
                if ((this.sourceType != null) && TypeSystem.HasIEnumerable(this.sourceType))
                {
                    return node;
                }
                switch (node.NodeType)
                {
                    case SqlNodeType.ClientCase:
                    case SqlNodeType.Convert:
                    case SqlNodeType.DiscriminatedType:
                        return node;

                    case SqlNodeType.MethodCall:
                    case SqlNodeType.Member:
                        return node;

                    case SqlNodeType.Link:
                        this.sourceType = ((SqlLink) node).RowType.Type;
                        return node;

                    case SqlNodeType.Element:
                    case SqlNodeType.FunctionCall:
                        return node;

                    case SqlNodeType.ScalarSubSelect:
                    case SqlNodeType.SearchedCase:
                        return node;

                    case SqlNodeType.New:
                        return node;

                    case SqlNodeType.Multiset:
                        return node;

                    case SqlNodeType.TypeCase:
                        this.sourceType = ((SqlTypeCase) node).RowType.Type;
                        return node;

                    case SqlNodeType.Value:
                    {
                        SqlValue value2 = (SqlValue) node;
                        if (value2.Value != null)
                        {
                            this.sourceType = value2.Value.GetType();
                        }
                        return node;
                    }
                    case SqlNodeType.SimpleCase:
                        return node;

                    case SqlNodeType.Table:
                        this.sourceType = ((SqlTable) node).RowType.Type;
                        return node;
                }
                return base.Visit(node);
            }

            internal override SqlExpression VisitAliasRef(SqlAliasRef aref)
            {
                if ((this.UnwrapSequences != null) && this.UnwrapSequences.Unwrap)
                {
                    this.UnwrapSequences = new UnwrapStack(this.UnwrapSequences, false);
                    this.VisitAlias(aref.Alias);
                    this.UnwrapSequences = this.UnwrapSequences.Last;
                    return aref;
                }
                this.VisitAlias(aref.Alias);
                return aref;
            }

            internal override SqlExpression VisitColumnRef(SqlColumnRef cref)
            {
                this.VisitColumn(cref.Column);
                return cref;
            }

            internal override SqlSelect VisitSelect(SqlSelect select)
            {
                this.UnwrapSequences = new UnwrapStack(this.UnwrapSequences, true);
                this.VisitExpression(select.Selection);
                this.UnwrapSequences = this.UnwrapSequences.Last;
                return select;
            }

            private class UnwrapStack
            {
                public UnwrapStack(TypeSource.Visitor.UnwrapStack last, bool unwrap)
                {
                    this.Last = last;
                    this.Unwrap = unwrap;
                }

                public TypeSource.Visitor.UnwrapStack Last { get; private set; }

                public bool Unwrap { get; private set; }
            }
        }
    }
}

