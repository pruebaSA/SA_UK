namespace System.Data.Common
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Reflection;

    public abstract class DbDataRecord : ICustomTypeDescriptor, IDataRecord
    {
        protected DbDataRecord()
        {
        }

        public abstract bool GetBoolean(int i);
        public abstract byte GetByte(int i);
        public abstract long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length);
        public abstract char GetChar(int i);
        public abstract long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length);
        public IDataReader GetData(int i) => 
            this.GetDbDataReader(i);

        public abstract string GetDataTypeName(int i);
        public abstract DateTime GetDateTime(int i);
        protected virtual DbDataReader GetDbDataReader(int i)
        {
            throw ADP.NotSupported();
        }

        public abstract decimal GetDecimal(int i);
        public abstract double GetDouble(int i);
        public abstract Type GetFieldType(int i);
        public abstract float GetFloat(int i);
        public abstract Guid GetGuid(int i);
        public abstract short GetInt16(int i);
        public abstract int GetInt32(int i);
        public abstract long GetInt64(int i);
        public abstract string GetName(int i);
        public abstract int GetOrdinal(string name);
        public abstract string GetString(int i);
        public abstract object GetValue(int i);
        public abstract int GetValues(object[] values);
        public abstract bool IsDBNull(int i);
        AttributeCollection ICustomTypeDescriptor.GetAttributes() => 
            new AttributeCollection(null);

        string ICustomTypeDescriptor.GetClassName() => 
            null;

        string ICustomTypeDescriptor.GetComponentName() => 
            null;

        TypeConverter ICustomTypeDescriptor.GetConverter() => 
            null;

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => 
            null;

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => 
            null;

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => 
            null;

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => 
            new EventDescriptorCollection(null);

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => 
            new EventDescriptorCollection(null);

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => 
            ((ICustomTypeDescriptor) this).GetProperties(null);

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) => 
            new PropertyDescriptorCollection(null);

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => 
            this;

        public abstract int FieldCount { get; }

        public abstract object this[int i] { get; }

        public abstract object this[string name] { get; }
    }
}

