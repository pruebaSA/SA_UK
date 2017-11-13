﻿namespace System.Data.Common
{
    using System;
    using System.ComponentModel;
    using System.Data.ProviderBase;
    using System.Reflection;

    internal sealed class DataRecordInternal : DbDataRecord, ICustomTypeDescriptor
    {
        private FieldNameLookup _fieldNameLookup;
        private PropertyDescriptorCollection _propertyDescriptors;
        private SchemaInfo[] _schemaInfo;
        private object[] _values;

        internal DataRecordInternal(object[] values, PropertyDescriptorCollection descriptors, FieldNameLookup fieldNameLookup)
        {
            this._values = values;
            this._propertyDescriptors = descriptors;
            this._fieldNameLookup = fieldNameLookup;
        }

        internal DataRecordInternal(SchemaInfo[] schemaInfo, object[] values, PropertyDescriptorCollection descriptors, FieldNameLookup fieldNameLookup)
        {
            this._schemaInfo = schemaInfo;
            this._values = values;
            this._propertyDescriptors = descriptors;
            this._fieldNameLookup = fieldNameLookup;
        }

        public override bool GetBoolean(int i) => 
            ((bool) this._values[i]);

        public override byte GetByte(int i) => 
            ((byte) this._values[i]);

        public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
        {
            int maxLen = 0;
            byte[] sourceArray = (byte[]) this._values[i];
            maxLen = sourceArray.Length;
            if (dataIndex > 0x7fffffffL)
            {
                throw ADP.InvalidSourceBufferIndex(maxLen, dataIndex, "dataIndex");
            }
            int sourceIndex = (int) dataIndex;
            if (buffer != null)
            {
                try
                {
                    if (sourceIndex < maxLen)
                    {
                        if ((sourceIndex + length) > maxLen)
                        {
                            maxLen -= sourceIndex;
                        }
                        else
                        {
                            maxLen = length;
                        }
                    }
                    Array.Copy(sourceArray, sourceIndex, buffer, bufferIndex, maxLen);
                }
                catch (Exception exception)
                {
                    if (ADP.IsCatchableExceptionType(exception))
                    {
                        maxLen = sourceArray.Length;
                        if (length < 0)
                        {
                            throw ADP.InvalidDataLength((long) length);
                        }
                        if ((bufferIndex < 0) || (bufferIndex >= buffer.Length))
                        {
                            throw ADP.InvalidDestinationBufferIndex(length, bufferIndex, "bufferIndex");
                        }
                        if ((dataIndex < 0L) || (dataIndex >= maxLen))
                        {
                            throw ADP.InvalidSourceBufferIndex(length, dataIndex, "dataIndex");
                        }
                        if ((maxLen + bufferIndex) > buffer.Length)
                        {
                            throw ADP.InvalidBufferSizeOrIndex(maxLen, bufferIndex);
                        }
                    }
                    throw;
                }
            }
            return (long) maxLen;
        }

        public override char GetChar(int i)
        {
            string str = (string) this._values[i];
            return str.ToCharArray()[0];
        }

        public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
        {
            int maxLen = 0;
            char[] sourceArray = ((string) this._values[i]).ToCharArray();
            maxLen = sourceArray.Length;
            if (dataIndex > 0x7fffffffL)
            {
                throw ADP.InvalidSourceBufferIndex(maxLen, dataIndex, "dataIndex");
            }
            int sourceIndex = (int) dataIndex;
            if (buffer != null)
            {
                try
                {
                    if (sourceIndex < maxLen)
                    {
                        if ((sourceIndex + length) > maxLen)
                        {
                            maxLen -= sourceIndex;
                        }
                        else
                        {
                            maxLen = length;
                        }
                    }
                    Array.Copy(sourceArray, sourceIndex, buffer, bufferIndex, maxLen);
                }
                catch (Exception exception)
                {
                    if (ADP.IsCatchableExceptionType(exception))
                    {
                        maxLen = sourceArray.Length;
                        if (length < 0)
                        {
                            throw ADP.InvalidDataLength((long) length);
                        }
                        if ((bufferIndex < 0) || (bufferIndex >= buffer.Length))
                        {
                            throw ADP.InvalidDestinationBufferIndex(buffer.Length, bufferIndex, "bufferIndex");
                        }
                        if ((sourceIndex < 0) || (sourceIndex >= maxLen))
                        {
                            throw ADP.InvalidSourceBufferIndex(maxLen, dataIndex, "dataIndex");
                        }
                        if ((maxLen + bufferIndex) > buffer.Length)
                        {
                            throw ADP.InvalidBufferSizeOrIndex(maxLen, bufferIndex);
                        }
                    }
                    throw;
                }
            }
            return (long) maxLen;
        }

        public override string GetDataTypeName(int i) => 
            this._schemaInfo[i].typeName;

        public override DateTime GetDateTime(int i) => 
            ((DateTime) this._values[i]);

        public override decimal GetDecimal(int i) => 
            ((decimal) this._values[i]);

        public override double GetDouble(int i) => 
            ((double) this._values[i]);

        public override Type GetFieldType(int i) => 
            this._schemaInfo[i].type;

        public override float GetFloat(int i) => 
            ((float) this._values[i]);

        public override Guid GetGuid(int i) => 
            ((Guid) this._values[i]);

        public override short GetInt16(int i) => 
            ((short) this._values[i]);

        public override int GetInt32(int i) => 
            ((int) this._values[i]);

        public override long GetInt64(int i) => 
            ((long) this._values[i]);

        public override string GetName(int i) => 
            this._schemaInfo[i].name;

        public override int GetOrdinal(string name) => 
            this._fieldNameLookup.GetOrdinal(name);

        public override string GetString(int i) => 
            ((string) this._values[i]);

        public override object GetValue(int i) => 
            this._values[i];

        public override int GetValues(object[] values)
        {
            if (values == null)
            {
                throw ADP.ArgumentNull("values");
            }
            int num2 = (values.Length < this._schemaInfo.Length) ? values.Length : this._schemaInfo.Length;
            for (int i = 0; i < num2; i++)
            {
                values[i] = this._values[i];
            }
            return num2;
        }

        public override bool IsDBNull(int i)
        {
            object obj2 = this._values[i];
            if (obj2 != null)
            {
                return Convert.IsDBNull(obj2);
            }
            return true;
        }

        internal void SetSchemaInfo(SchemaInfo[] schemaInfo)
        {
            this._schemaInfo = schemaInfo;
        }

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

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            if (this._propertyDescriptors == null)
            {
                this._propertyDescriptors = new PropertyDescriptorCollection(null);
            }
            return this._propertyDescriptors;
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => 
            this;

        public override int FieldCount =>
            this._schemaInfo.Length;

        public override object this[int i] =>
            this.GetValue(i);

        public override object this[string name] =>
            this.GetValue(this.GetOrdinal(name));
    }
}

