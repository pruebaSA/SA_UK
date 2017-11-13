﻿namespace System.Data.SqlClient
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Runtime.InteropServices;

    [ToolboxItem("Microsoft.VSDesigner.Data.VS.SqlDataAdapterToolboxItem, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), Designer("Microsoft.VSDesigner.Data.VS.SqlDataAdapterDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultEvent("RowUpdated")]
    public sealed class SqlDataAdapter : DbDataAdapter, IDbDataAdapter, IDataAdapter, ICloneable
    {
        private SqlCommandSet _commandSet;
        private SqlCommand _deleteCommand;
        private SqlCommand _insertCommand;
        private SqlCommand _selectCommand;
        private int _updateBatchSize;
        private SqlCommand _updateCommand;
        private static readonly object EventRowUpdated = new object();
        private static readonly object EventRowUpdating = new object();

        [ResCategory("DataCategory_Update"), ResDescription("DbDataAdapter_RowUpdated")]
        public event SqlRowUpdatedEventHandler RowUpdated
        {
            add
            {
                base.Events.AddHandler(EventRowUpdated, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventRowUpdated, value);
            }
        }

        [ResCategory("DataCategory_Update"), ResDescription("DbDataAdapter_RowUpdating")]
        public event SqlRowUpdatingEventHandler RowUpdating
        {
            add
            {
                SqlRowUpdatingEventHandler mcd = (SqlRowUpdatingEventHandler) base.Events[EventRowUpdating];
                if ((mcd != null) && (value.Target is DbCommandBuilder))
                {
                    SqlRowUpdatingEventHandler handler = (SqlRowUpdatingEventHandler) ADP.FindBuilder(mcd);
                    if (handler != null)
                    {
                        base.Events.RemoveHandler(EventRowUpdating, handler);
                    }
                }
                base.Events.AddHandler(EventRowUpdating, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventRowUpdating, value);
            }
        }

        public SqlDataAdapter()
        {
            this._updateBatchSize = 1;
            GC.SuppressFinalize(this);
        }

        public SqlDataAdapter(SqlCommand selectCommand) : this()
        {
            this.SelectCommand = selectCommand;
        }

        private SqlDataAdapter(SqlDataAdapter from) : base(from)
        {
            this._updateBatchSize = 1;
            GC.SuppressFinalize(this);
        }

        public SqlDataAdapter(string selectCommandText, SqlConnection selectConnection) : this()
        {
            this.SelectCommand = new SqlCommand(selectCommandText, selectConnection);
        }

        public SqlDataAdapter(string selectCommandText, string selectConnectionString) : this()
        {
            SqlConnection connection = new SqlConnection(selectConnectionString);
            this.SelectCommand = new SqlCommand(selectCommandText, connection);
        }

        protected override int AddToBatch(IDbCommand command)
        {
            int commandCount = this._commandSet.CommandCount;
            this._commandSet.Append((SqlCommand) command);
            return commandCount;
        }

        protected override void ClearBatch()
        {
            this._commandSet.Clear();
        }

        protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping) => 
            new SqlRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);

        protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping) => 
            new SqlRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);

        protected override int ExecuteBatch() => 
            this._commandSet.ExecuteNonQuery();

        protected override IDataParameter GetBatchedParameter(int commandIdentifier, int parameterIndex) => 
            this._commandSet.GetParameter(commandIdentifier, parameterIndex);

        protected override bool GetBatchedRecordsAffected(int commandIdentifier, out int recordsAffected, out Exception error) => 
            this._commandSet.GetBatchedAffected(commandIdentifier, out recordsAffected, out error);

        protected override void InitializeBatching()
        {
            Bid.Trace("<sc.SqlDataAdapter.InitializeBatching|API> %d#\n", base.ObjectID);
            this._commandSet = new SqlCommandSet();
            SqlCommand selectCommand = this.SelectCommand;
            if (selectCommand == null)
            {
                selectCommand = this.InsertCommand;
                if (selectCommand == null)
                {
                    selectCommand = this.UpdateCommand;
                    if (selectCommand == null)
                    {
                        selectCommand = this.DeleteCommand;
                    }
                }
            }
            if (selectCommand != null)
            {
                this._commandSet.Connection = selectCommand.Connection;
                this._commandSet.Transaction = selectCommand.Transaction;
                this._commandSet.CommandTimeout = selectCommand.CommandTimeout;
            }
        }

        protected override void OnRowUpdated(RowUpdatedEventArgs value)
        {
            SqlRowUpdatedEventHandler handler = (SqlRowUpdatedEventHandler) base.Events[EventRowUpdated];
            if ((handler != null) && (value is SqlRowUpdatedEventArgs))
            {
                handler(this, (SqlRowUpdatedEventArgs) value);
            }
            base.OnRowUpdated(value);
        }

        protected override void OnRowUpdating(RowUpdatingEventArgs value)
        {
            SqlRowUpdatingEventHandler handler = (SqlRowUpdatingEventHandler) base.Events[EventRowUpdating];
            if ((handler != null) && (value is SqlRowUpdatingEventArgs))
            {
                handler(this, (SqlRowUpdatingEventArgs) value);
            }
            base.OnRowUpdating(value);
        }

        object ICloneable.Clone() => 
            new SqlDataAdapter(this);

        protected override void TerminateBatching()
        {
            if (this._commandSet != null)
            {
                this._commandSet.Dispose();
                this._commandSet = null;
            }
        }

        [DefaultValue((string) null), Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ResCategory("DataCategory_Update"), ResDescription("DbDataAdapter_DeleteCommand")]
        public SqlCommand DeleteCommand
        {
            get => 
                this._deleteCommand;
            set
            {
                this._deleteCommand = value;
            }
        }

        [ResCategory("DataCategory_Update"), ResDescription("DbDataAdapter_InsertCommand"), Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultValue((string) null)]
        public SqlCommand InsertCommand
        {
            get => 
                this._insertCommand;
            set
            {
                this._insertCommand = value;
            }
        }

        [DefaultValue((string) null), Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ResCategory("DataCategory_Fill"), ResDescription("DbDataAdapter_SelectCommand")]
        public SqlCommand SelectCommand
        {
            get => 
                this._selectCommand;
            set
            {
                this._selectCommand = value;
            }
        }

        IDbCommand IDbDataAdapter.DeleteCommand
        {
            get => 
                this._deleteCommand;
            set
            {
                this._deleteCommand = (SqlCommand) value;
            }
        }

        IDbCommand IDbDataAdapter.InsertCommand
        {
            get => 
                this._insertCommand;
            set
            {
                this._insertCommand = (SqlCommand) value;
            }
        }

        IDbCommand IDbDataAdapter.SelectCommand
        {
            get => 
                this._selectCommand;
            set
            {
                this._selectCommand = (SqlCommand) value;
            }
        }

        IDbCommand IDbDataAdapter.UpdateCommand
        {
            get => 
                this._updateCommand;
            set
            {
                this._updateCommand = (SqlCommand) value;
            }
        }

        public override int UpdateBatchSize
        {
            get => 
                this._updateBatchSize;
            set
            {
                if (0 > value)
                {
                    throw ADP.ArgumentOutOfRange("UpdateBatchSize");
                }
                this._updateBatchSize = value;
                Bid.Trace("<sc.SqlDataAdapter.set_UpdateBatchSize|API> %d#, %d\n", base.ObjectID, value);
            }
        }

        [ResCategory("DataCategory_Update"), Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultValue((string) null), ResDescription("DbDataAdapter_UpdateCommand")]
        public SqlCommand UpdateCommand
        {
            get => 
                this._updateCommand;
            set
            {
                this._updateCommand = value;
            }
        }
    }
}

