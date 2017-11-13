namespace System.Data.Common
{
    using System;
    using System.ComponentModel;
    using System.Data;

    public abstract class DbCommand : Component, IDbCommand, IDisposable
    {
        protected DbCommand()
        {
        }

        public abstract void Cancel();
        protected abstract DbParameter CreateDbParameter();
        public DbParameter CreateParameter() => 
            this.CreateDbParameter();

        protected abstract DbDataReader ExecuteDbDataReader(CommandBehavior behavior);
        public abstract int ExecuteNonQuery();
        public DbDataReader ExecuteReader() => 
            this.ExecuteDbDataReader(CommandBehavior.Default);

        public DbDataReader ExecuteReader(CommandBehavior behavior) => 
            this.ExecuteDbDataReader(behavior);

        public abstract object ExecuteScalar();
        public abstract void Prepare();
        IDbDataParameter IDbCommand.CreateParameter() => 
            this.CreateDbParameter();

        IDataReader IDbCommand.ExecuteReader() => 
            this.ExecuteDbDataReader(CommandBehavior.Default);

        IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior) => 
            this.ExecuteDbDataReader(behavior);

        [DefaultValue(""), ResCategory("DataCategory_Data"), ResDescription("DbCommand_CommandText"), RefreshProperties(RefreshProperties.All)]
        public abstract string CommandText { get; set; }

        [ResDescription("DbCommand_CommandTimeout"), ResCategory("DataCategory_Data")]
        public abstract int CommandTimeout { get; set; }

        [DefaultValue(1), ResCategory("DataCategory_Data"), RefreshProperties(RefreshProperties.All), ResDescription("DbCommand_CommandType")]
        public abstract System.Data.CommandType CommandType { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), ResCategory("DataCategory_Data"), ResDescription("DbCommand_Connection"), DefaultValue((string) null)]
        public System.Data.Common.DbConnection Connection
        {
            get => 
                this.DbConnection;
            set
            {
                this.DbConnection = value;
            }
        }

        protected abstract System.Data.Common.DbConnection DbConnection { get; set; }

        protected abstract System.Data.Common.DbParameterCollection DbParameterCollection { get; }

        protected abstract System.Data.Common.DbTransaction DbTransaction { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never), DesignOnly(true), DefaultValue(true), Browsable(false)]
        public abstract bool DesignTimeVisible { get; set; }

        [Browsable(false), ResCategory("DataCategory_Data"), ResDescription("DbCommand_Parameters"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Data.Common.DbParameterCollection Parameters =>
            this.DbParameterCollection;

        IDbConnection IDbCommand.Connection
        {
            get => 
                this.DbConnection;
            set
            {
                this.DbConnection = (System.Data.Common.DbConnection) value;
            }
        }

        IDataParameterCollection IDbCommand.Parameters =>
            this.DbParameterCollection;

        IDbTransaction IDbCommand.Transaction
        {
            get => 
                this.DbTransaction;
            set
            {
                this.DbTransaction = (System.Data.Common.DbTransaction) value;
            }
        }

        [ResDescription("DbCommand_Transaction"), Browsable(false), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Data.Common.DbTransaction Transaction
        {
            get => 
                this.DbTransaction;
            set
            {
                this.DbTransaction = value;
            }
        }

        [DefaultValue(3), ResCategory("DataCategory_Update"), ResDescription("DbCommand_UpdatedRowSource")]
        public abstract UpdateRowSource UpdatedRowSource { get; set; }
    }
}

