namespace System.Data.Odbc
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;

    [DefaultEvent("RowUpdated"), ToolboxItem("Microsoft.VSDesigner.Data.VS.OdbcDataAdapterToolboxItem, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), Designer("Microsoft.VSDesigner.Data.VS.OdbcDataAdapterDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public sealed class OdbcDataAdapter : DbDataAdapter, IDbDataAdapter, IDataAdapter, ICloneable
    {
        private OdbcCommand _deleteCommand;
        private OdbcCommand _insertCommand;
        private OdbcCommand _selectCommand;
        private OdbcCommand _updateCommand;
        private static readonly object EventRowUpdated = new object();
        private static readonly object EventRowUpdating = new object();

        [ResDescription("DbDataAdapter_RowUpdated"), ResCategory("DataCategory_Update")]
        public event OdbcRowUpdatedEventHandler RowUpdated
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

        [ResDescription("DbDataAdapter_RowUpdating"), ResCategory("DataCategory_Update")]
        public event OdbcRowUpdatingEventHandler RowUpdating
        {
            add
            {
                OdbcRowUpdatingEventHandler mcd = (OdbcRowUpdatingEventHandler) base.Events[EventRowUpdating];
                if ((mcd != null) && (value.Target is OdbcCommandBuilder))
                {
                    OdbcRowUpdatingEventHandler handler = (OdbcRowUpdatingEventHandler) ADP.FindBuilder(mcd);
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

        public OdbcDataAdapter()
        {
            GC.SuppressFinalize(this);
        }

        public OdbcDataAdapter(OdbcCommand selectCommand) : this()
        {
            this.SelectCommand = selectCommand;
        }

        private OdbcDataAdapter(OdbcDataAdapter from) : base(from)
        {
            GC.SuppressFinalize(this);
        }

        public OdbcDataAdapter(string selectCommandText, OdbcConnection selectConnection) : this()
        {
            this.SelectCommand = new OdbcCommand(selectCommandText, selectConnection);
        }

        public OdbcDataAdapter(string selectCommandText, string selectConnectionString) : this()
        {
            OdbcConnection connection = new OdbcConnection(selectConnectionString);
            this.SelectCommand = new OdbcCommand(selectCommandText, connection);
        }

        protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping) => 
            new OdbcRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);

        protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping) => 
            new OdbcRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);

        protected override void OnRowUpdated(RowUpdatedEventArgs value)
        {
            OdbcRowUpdatedEventHandler handler = (OdbcRowUpdatedEventHandler) base.Events[EventRowUpdated];
            if ((handler != null) && (value is OdbcRowUpdatedEventArgs))
            {
                handler(this, (OdbcRowUpdatedEventArgs) value);
            }
            base.OnRowUpdated(value);
        }

        protected override void OnRowUpdating(RowUpdatingEventArgs value)
        {
            OdbcRowUpdatingEventHandler handler = (OdbcRowUpdatingEventHandler) base.Events[EventRowUpdating];
            if ((handler != null) && (value is OdbcRowUpdatingEventArgs))
            {
                handler(this, (OdbcRowUpdatingEventArgs) value);
            }
            base.OnRowUpdating(value);
        }

        object ICloneable.Clone() => 
            new OdbcDataAdapter(this);

        [Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ResCategory("DataCategory_Update"), ResDescription("DbDataAdapter_DeleteCommand"), DefaultValue((string) null)]
        public OdbcCommand DeleteCommand
        {
            get => 
                this._deleteCommand;
            set
            {
                this._deleteCommand = value;
            }
        }

        [ResDescription("DbDataAdapter_InsertCommand"), DefaultValue((string) null), ResCategory("DataCategory_Update"), Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public OdbcCommand InsertCommand
        {
            get => 
                this._insertCommand;
            set
            {
                this._insertCommand = value;
            }
        }

        [ResDescription("DbDataAdapter_SelectCommand"), ResCategory("DataCategory_Fill"), Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultValue((string) null)]
        public OdbcCommand SelectCommand
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
                this._deleteCommand = (OdbcCommand) value;
            }
        }

        IDbCommand IDbDataAdapter.InsertCommand
        {
            get => 
                this._insertCommand;
            set
            {
                this._insertCommand = (OdbcCommand) value;
            }
        }

        IDbCommand IDbDataAdapter.SelectCommand
        {
            get => 
                this._selectCommand;
            set
            {
                this._selectCommand = (OdbcCommand) value;
            }
        }

        IDbCommand IDbDataAdapter.UpdateCommand
        {
            get => 
                this._updateCommand;
            set
            {
                this._updateCommand = (OdbcCommand) value;
            }
        }

        [DefaultValue((string) null), Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ResDescription("DbDataAdapter_UpdateCommand"), ResCategory("DataCategory_Update")]
        public OdbcCommand UpdateCommand
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

