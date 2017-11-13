namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Reflection;

    internal class SqlMemberAssign : SqlNode
    {
        private SqlExpression expression;
        private MemberInfo member;

        internal SqlMemberAssign(MemberInfo member, SqlExpression expr) : base(SqlNodeType.MemberAssign, expr.SourceExpression)
        {
            if (member == null)
            {
                throw Error.ArgumentNull("member");
            }
            this.member = member;
            this.Expression = expr;
        }

        internal SqlExpression Expression
        {
            get => 
                this.expression;
            set
            {
                if (value == null)
                {
                    throw Error.ArgumentNull("value");
                }
                this.expression = value;
            }
        }

        internal MemberInfo Member =>
            this.member;
    }
}

