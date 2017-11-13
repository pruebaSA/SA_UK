namespace System.Data.Common
{
    using System;
    using System.ComponentModel;
    using System.Data;

    public abstract class DbParameter : MarshalByRefObject, IDbDataParameter, IDataParameter
    {
        protected DbParameter()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract void ResetDbType();

        [RefreshProperties(RefreshProperties.All), ResCategory("DataCategory_Data"), ResDescription("DbParameter_DbType"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public abstract System.Data.DbType DbType { get; set; }

        [DefaultValue(1), ResDescription("DbParameter_Direction"), RefreshProperties(RefreshProperties.All), ResCategory("DataCategory_Data")]
        public abstract ParameterDirection Direction { get; set; }

        [Browsable(false), DesignOnly(true), EditorBrowsable(EditorBrowsableState.Never)]
        public abstract bool IsNullable { get; set; }

        [ResCategory("DataCategory_Data"), DefaultValue(""), ResDescription("DbParameter_ParameterName")]
        public abstract string ParameterName { get; set; }

        [ResCategory("DataCategory_Data"), ResDescription("DbParameter_Size")]
        public abstract int Size { get; set; }

        [ResDescription("DbParameter_SourceColumn"), DefaultValue(""), ResCategory("DataCategory_Update")]
        public abstract string SourceColumn { get; set; }

        [DefaultValue(false), RefreshProperties(RefreshProperties.All), EditorBrowsable(EditorBrowsableState.Advanced), ResDescription("DbParameter_SourceColumnNullMapping"), ResCategory("DataCategory_Update")]
        public abstract bool SourceColumnNullMapping { get; set; }

        [ResDescription("DbParameter_SourceVersion"), ResCategory("DataCategory_Update"), DefaultValue(0x200)]
        public abstract DataRowVersion SourceVersion { get; set; }

        byte IDbDataParameter.Precision
        {
            get => 
                0;
            set
            {
            }
        }

        byte IDbDataParameter.Scale
        {
            get => 
                0;
            set
            {
            }
        }

        [ResDescription("DbParameter_Value"), ResCategory("DataCategory_Data"), RefreshProperties(RefreshProperties.All), DefaultValue((string) null)]
        public abstract object Value { get; set; }
    }
}

