namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlUnion : SqlNode
    {
        private bool all;
        private SqlNode left;
        private SqlNode right;

        internal SqlUnion(SqlNode left, SqlNode right, bool all) : base(SqlNodeType.Union, right.SourceExpression)
        {
            this.Left = left;
            this.Right = right;
            this.All = all;
        }

        internal Type GetClrType()
        {
            SqlExpression left = this.Left as SqlExpression;
            if (left != null)
            {
                return left.ClrType;
            }
            SqlSelect select = this.Left as SqlSelect;
            return select?.Selection.ClrType;
        }

        internal ProviderType GetSqlType()
        {
            SqlExpression left = this.Left as SqlExpression;
            if (left != null)
            {
                return left.SqlType;
            }
            SqlSelect select = this.Left as SqlSelect;
            return select?.Selection.SqlType;
        }

        private void Validate(SqlNode node)
        {
            if (node == null)
            {
                throw Error.ArgumentNull("node");
            }
            if ((!(node is SqlExpression) && !(node is SqlSelect)) && !(node is SqlUnion))
            {
                throw Error.UnexpectedNode(node.NodeType);
            }
        }

        internal bool All
        {
            get => 
                this.all;
            set
            {
                this.all = value;
            }
        }

        internal SqlNode Left
        {
            get => 
                this.left;
            set
            {
                this.Validate(value);
                this.left = value;
            }
        }

        internal SqlNode Right
        {
            get => 
                this.right;
            set
            {
                this.Validate(value);
                this.right = value;
            }
        }
    }
}

