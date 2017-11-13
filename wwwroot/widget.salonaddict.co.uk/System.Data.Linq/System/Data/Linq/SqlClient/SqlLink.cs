namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Linq.Expressions;

    internal class SqlLink : SqlSimpleTypeExpression
    {
        private SqlExpression expansion;
        private SqlExpression expression;
        private object id;
        private List<SqlExpression> keyExpressions;
        private MetaDataMember member;
        private MetaType rowType;

        internal SqlLink(object id, MetaType rowType, Type clrType, ProviderType sqlType, SqlExpression expression, MetaDataMember member, IEnumerable<SqlExpression> keyExpressions, SqlExpression expansion, System.Linq.Expressions.Expression sourceExpression) : base(SqlNodeType.Link, clrType, sqlType, sourceExpression)
        {
            this.id = id;
            this.rowType = rowType;
            this.expansion = expansion;
            this.expression = expression;
            this.member = member;
            this.keyExpressions = new List<SqlExpression>();
            if (keyExpressions != null)
            {
                this.keyExpressions.AddRange(keyExpressions);
            }
        }

        internal SqlExpression Expansion
        {
            get => 
                this.expansion;
            set
            {
                this.expansion = value;
            }
        }

        internal SqlExpression Expression
        {
            get => 
                this.expression;
            set
            {
                this.expression = value;
            }
        }

        internal object Id =>
            this.id;

        internal List<SqlExpression> KeyExpressions =>
            this.keyExpressions;

        internal MetaDataMember Member =>
            this.member;

        internal MetaType RowType =>
            this.rowType;
    }
}

