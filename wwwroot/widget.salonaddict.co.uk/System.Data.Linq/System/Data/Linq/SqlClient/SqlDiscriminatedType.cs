namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data.Linq.Mapping;
    using System.Linq.Expressions;

    internal class SqlDiscriminatedType : SqlExpression
    {
        private SqlExpression discriminator;
        private ProviderType sqlType;
        private MetaType targetType;

        internal SqlDiscriminatedType(ProviderType sqlType, SqlExpression discriminator, MetaType targetType, Expression sourceExpression) : base(SqlNodeType.DiscriminatedType, typeof(Type), sourceExpression)
        {
            if (discriminator == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("discriminator");
            }
            this.discriminator = discriminator;
            this.targetType = targetType;
            this.sqlType = sqlType;
        }

        internal SqlExpression Discriminator
        {
            get => 
                this.discriminator;
            set
            {
                this.discriminator = value;
            }
        }

        internal override ProviderType SqlType =>
            this.sqlType;

        internal MetaType TargetType =>
            this.targetType;
    }
}

