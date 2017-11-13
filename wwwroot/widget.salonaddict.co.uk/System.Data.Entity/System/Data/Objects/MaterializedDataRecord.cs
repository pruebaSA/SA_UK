namespace System.Data.Objects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Reflection;

    internal sealed class MaterializedDataRecord : DbDataRecord, IExtendedDataRecord, IDataRecord, ICustomTypeDescriptor
    {
        private Dictionary<object, AttributeCollection> _attrCache;
        private readonly TypeUsage _edmUsage;
        private FieldNameLookup _fieldNameLookup;
        private FilterCache _filterCache;
        private PropertyDescriptorCollection _propertyDescriptors;
        private System.Data.Common.DataRecordInfo _recordInfo;
        private readonly object[] _values;
        private readonly MetadataWorkspace _workspace;

        internal MaterializedDataRecord(MetadataWorkspace workspace, TypeUsage edmUsage, object[] values)
        {
            this._workspace = workspace;
            this._edmUsage = edmUsage;
            this._values = values;
        }

        internal static PropertyDescriptorCollection CreatePropertyDescriptorCollection(StructuralType structuralType, Type componentType, bool isReadOnly)
        {
            List<PropertyDescriptor> list = new List<PropertyDescriptor>();
            if (structuralType != null)
            {
                foreach (EdmMember member in structuralType.Members)
                {
                    if (member.BuiltInTypeKind == BuiltInTypeKind.EdmProperty)
                    {
                        EdmProperty property = (EdmProperty) member;
                        FieldDescriptor item = new FieldDescriptor(componentType, isReadOnly, property);
                        list.Add(item);
                    }
                }
            }
            return new PropertyDescriptorCollection(list.ToArray());
        }

        public override bool GetBoolean(int ordinal) => 
            ((bool) this._values[ordinal]);

        public override byte GetByte(int ordinal) => 
            ((byte) this._values[ordinal]);

        public override long GetBytes(int ordinal, long fieldOffset, byte[] buffer, int bufferOffset, int length)
        {
            int maxLen = 0;
            byte[] sourceArray = (byte[]) this._values[ordinal];
            maxLen = sourceArray.Length;
            if (fieldOffset > 0x7fffffffL)
            {
                throw EntityUtil.InvalidSourceBufferIndex(maxLen, fieldOffset, "fieldOffset");
            }
            int sourceIndex = (int) fieldOffset;
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
                    Array.Copy(sourceArray, sourceIndex, buffer, bufferOffset, maxLen);
                }
                catch (Exception exception)
                {
                    if (EntityUtil.IsCatchableExceptionType(exception))
                    {
                        maxLen = sourceArray.Length;
                        if (length < 0)
                        {
                            throw EntityUtil.InvalidDataLength((long) length);
                        }
                        if ((bufferOffset < 0) || (bufferOffset >= buffer.Length))
                        {
                            throw EntityUtil.InvalidDestinationBufferIndex(length, bufferOffset, "bufferOffset");
                        }
                        if ((fieldOffset < 0L) || (fieldOffset >= maxLen))
                        {
                            throw EntityUtil.InvalidSourceBufferIndex(length, fieldOffset, "fieldOffset");
                        }
                        if ((maxLen + bufferOffset) > buffer.Length)
                        {
                            throw EntityUtil.InvalidBufferSizeOrIndex(maxLen, bufferOffset);
                        }
                    }
                    throw;
                }
            }
            return (long) maxLen;
        }

        public override char GetChar(int ordinal) => 
            ((string) this.GetValue(ordinal))[0];

        public override long GetChars(int ordinal, long fieldOffset, char[] buffer, int bufferOffset, int length)
        {
            int maxLen = 0;
            string str = (string) this._values[ordinal];
            maxLen = str.Length;
            if (fieldOffset > 0x7fffffffL)
            {
                throw EntityUtil.InvalidSourceBufferIndex(maxLen, fieldOffset, "fieldOffset");
            }
            int sourceIndex = (int) fieldOffset;
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
                    str.CopyTo(sourceIndex, buffer, bufferOffset, maxLen);
                }
                catch (Exception exception)
                {
                    if (EntityUtil.IsCatchableExceptionType(exception))
                    {
                        maxLen = str.Length;
                        if (length < 0)
                        {
                            throw EntityUtil.InvalidDataLength((long) length);
                        }
                        if ((bufferOffset < 0) || (bufferOffset >= buffer.Length))
                        {
                            throw EntityUtil.InvalidDestinationBufferIndex(buffer.Length, bufferOffset, "bufferOffset");
                        }
                        if ((fieldOffset < 0L) || (fieldOffset >= maxLen))
                        {
                            throw EntityUtil.InvalidSourceBufferIndex(maxLen, fieldOffset, "fieldOffset");
                        }
                        if ((maxLen + bufferOffset) > buffer.Length)
                        {
                            throw EntityUtil.InvalidBufferSizeOrIndex(maxLen, bufferOffset);
                        }
                    }
                    throw;
                }
            }
            return (long) maxLen;
        }

        public DbDataReader GetDataReader(int i) => 
            this.GetDbDataReader(i);

        public DbDataRecord GetDataRecord(int ordinal) => 
            ((DbDataRecord) this._values[ordinal]);

        public override string GetDataTypeName(int ordinal) => 
            this.GetMember(ordinal).TypeUsage.EdmType.Name;

        public override DateTime GetDateTime(int ordinal) => 
            ((DateTime) this._values[ordinal]);

        public override decimal GetDecimal(int ordinal) => 
            ((decimal) this._values[ordinal]);

        public override double GetDouble(int ordinal) => 
            ((double) this._values[ordinal]);

        public override Type GetFieldType(int ordinal)
        {
            EdmType edmType = this.GetMember(ordinal).TypeUsage.EdmType;
            if (Helper.IsPrimitiveType(edmType))
            {
                return ((PrimitiveType) edmType).ClrEquivalentType;
            }
            return typeof(object);
        }

        public override float GetFloat(int ordinal) => 
            ((float) this._values[ordinal]);

        public override Guid GetGuid(int ordinal) => 
            ((Guid) this._values[ordinal]);

        public override short GetInt16(int ordinal) => 
            ((short) this._values[ordinal]);

        public override int GetInt32(int ordinal) => 
            ((int) this._values[ordinal]);

        public override long GetInt64(int ordinal) => 
            ((long) this._values[ordinal]);

        private EdmMember GetMember(int ordinal)
        {
            FieldMetadata metadata = this.DataRecordInfo.FieldMetadata[ordinal];
            return metadata.FieldType;
        }

        public override string GetName(int ordinal) => 
            this.GetMember(ordinal).Name;

        public override int GetOrdinal(string name) => 
            this._fieldNameLookup?.GetOrdinal(name);

        public override string GetString(int ordinal) => 
            ((string) this._values[ordinal]);

        public override object GetValue(int ordinal) => 
            this._values[ordinal];

        public override int GetValues(object[] values)
        {
            if (values == null)
            {
                throw EntityUtil.ArgumentNull("values");
            }
            int num = Math.Min(values.Length, this.FieldCount);
            for (int i = 0; i < num; i++)
            {
                values[i] = this._values[i];
            }
            return num;
        }

        private PropertyDescriptorCollection InitializePropertyDescriptors()
        {
            if (this._values == null)
            {
                return null;
            }
            if ((this._propertyDescriptors == null) && (0 < this._values.Length))
            {
                this._propertyDescriptors = CreatePropertyDescriptorCollection(this.DataRecordInfo.RecordType.EdmType as StructuralType, typeof(MaterializedDataRecord), true);
            }
            return this._propertyDescriptors;
        }

        public override bool IsDBNull(int ordinal) => 
            (DBNull.Value == this._values[ordinal]);

        AttributeCollection ICustomTypeDescriptor.GetAttributes() => 
            TypeDescriptor.GetAttributes(this, true);

        string ICustomTypeDescriptor.GetClassName() => 
            null;

        string ICustomTypeDescriptor.GetComponentName() => 
            null;

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => 
            ((ICustomTypeDescriptor) this).GetProperties(null);

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            bool flag = (attributes != null) && (0 < attributes.Length);
            PropertyDescriptorCollection descriptors = this.InitializePropertyDescriptors();
            if (descriptors != null)
            {
                FilterCache cache = this._filterCache;
                if ((flag && (cache != null)) && cache.IsValid(attributes))
                {
                    return cache.FilteredProperties;
                }
                if (!flag && (descriptors != null))
                {
                    return descriptors;
                }
                if (((this._attrCache == null) && (attributes != null)) && (0 < attributes.Length))
                {
                    this._attrCache = new Dictionary<object, AttributeCollection>();
                    foreach (FieldDescriptor descriptor in this._propertyDescriptors)
                    {
                        object[] customAttributes = descriptor.GetValue(this).GetType().GetCustomAttributes(false);
                        Attribute[] array = new Attribute[customAttributes.Length];
                        customAttributes.CopyTo(array, 0);
                        this._attrCache.Add(descriptor, new AttributeCollection(array));
                    }
                }
                descriptors = new PropertyDescriptorCollection(null);
                foreach (PropertyDescriptor descriptor2 in this._propertyDescriptors)
                {
                    if (this._attrCache[descriptor2].Matches(attributes))
                    {
                        descriptors.Add(descriptor2);
                    }
                }
                if (flag)
                {
                    cache = new FilterCache {
                        Attributes = attributes,
                        FilteredProperties = descriptors
                    };
                    this._filterCache = cache;
                }
            }
            return descriptors;
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => 
            this;

        public System.Data.Common.DataRecordInfo DataRecordInfo
        {
            get
            {
                if (this._recordInfo == null)
                {
                    if (this._workspace == null)
                    {
                        this._recordInfo = new System.Data.Common.DataRecordInfo(this._edmUsage);
                    }
                    else
                    {
                        this._recordInfo = new System.Data.Common.DataRecordInfo(this._workspace.GetOSpaceTypeUsage(this._edmUsage));
                    }
                }
                return this._recordInfo;
            }
        }

        public override int FieldCount =>
            this._values.Length;

        public override object this[int ordinal] =>
            this.GetValue(ordinal);

        public override object this[string name] =>
            this.GetValue(this.GetOrdinal(name));

        private class FilterCache
        {
            public Attribute[] Attributes;
            public PropertyDescriptorCollection FilteredProperties;

            public bool IsValid(Attribute[] other)
            {
                if ((other == null) || (this.Attributes == null))
                {
                    return false;
                }
                if (this.Attributes.Length != other.Length)
                {
                    return false;
                }
                for (int i = 0; i < other.Length; i++)
                {
                    if (!this.Attributes[i].Match(other[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}

