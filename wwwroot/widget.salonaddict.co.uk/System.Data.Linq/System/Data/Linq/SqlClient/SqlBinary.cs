namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Reflection;

    internal class SqlBinary : SqlSimpleTypeExpression
    {
        private SqlExpression left;
        private MethodInfo method;
        private SqlExpression right;

        internal SqlBinary(SqlNodeType nt, Type clrType, ProviderType sqlType, SqlExpression left, SqlExpression right) : this(nt, clrType, sqlType, left, right, null)
        {
        }

        internal SqlBinary(SqlNodeType nt, Type clrType, ProviderType sqlType, SqlExpression left, SqlExpression right, MethodInfo method) : base(nt, clrType, sqlType, right.SourceExpression)
        {
            switch (nt)
            {
                case SqlNodeType.BitAnd:
                case SqlNodeType.BitOr:
                case SqlNodeType.BitXor:
                case SqlNodeType.And:
                case SqlNodeType.Add:
                case SqlNodeType.Coalesce:
                case SqlNodeType.Concat:
                case SqlNodeType.Div:
                case SqlNodeType.EQ:
                case SqlNodeType.EQ2V:
                case SqlNodeType.LE:
                case SqlNodeType.LT:
                case SqlNodeType.GE:
                case SqlNodeType.GT:
                case SqlNodeType.Mod:
                case SqlNodeType.Mul:
                case SqlNodeType.NE:
                case SqlNodeType.NE2V:
                case SqlNodeType.Or:
                case SqlNodeType.Sub:
                    this.Left = left;
                    this.Right = right;
                    this.method = method;
                    return;
            }
            throw Error.UnexpectedNode(nt);
        }

        internal SqlExpression Left
        {
            get => 
                this.left;
            set
            {
                if (value == null)
                {
                    throw Error.ArgumentNull("value");
                }
                this.left = value;
            }
        }

        internal MethodInfo Method =>
            this.method;

        internal SqlExpression Right
        {
            get => 
                this.right;
            set
            {
                if (value == null)
                {
                    throw Error.ArgumentNull("value");
                }
                this.right = value;
            }
        }
    }
}

