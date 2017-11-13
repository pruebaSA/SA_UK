namespace System.Data.EntityClient
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.SqlTypes;

    public sealed class EntityParameter : DbParameter, IDbDataParameter, IDataParameter
    {
        private object _coercedValue;
        private System.Data.DbType? _dbType;
        private ParameterDirection _direction;
        private bool _isDirty;
        private bool? _isNullable;
        private string _parameterName;
        private object _parent;
        private byte? _precision;
        private byte? _scale;
        private int? _size;
        private string _sourceColumn;
        private bool _sourceColumnNullMapping;
        private DataRowVersion _sourceVersion;
        private object _value;

        public EntityParameter()
        {
        }

        private EntityParameter(EntityParameter source) : this()
        {
            ADP.CheckArgumentNull(source, "source");
            source.CloneHelper(this);
            ICloneable cloneable = this._value as ICloneable;
            if (cloneable != null)
            {
                this._value = cloneable.Clone();
            }
        }

        public EntityParameter(string parameterName, System.Data.DbType dbType)
        {
            this.SetParameterNameWithValidation(parameterName, "parameterName");
            this.DbType = dbType;
        }

        public EntityParameter(string parameterName, System.Data.DbType dbType, int size)
        {
            this.SetParameterNameWithValidation(parameterName, "parameterName");
            this.DbType = dbType;
            this.Size = size;
        }

        public EntityParameter(string parameterName, System.Data.DbType dbType, int size, string sourceColumn)
        {
            this.SetParameterNameWithValidation(parameterName, "parameterName");
            this.DbType = dbType;
            this.Size = size;
            this.SourceColumn = sourceColumn;
        }

        public EntityParameter(string parameterName, System.Data.DbType dbType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            this.SetParameterNameWithValidation(parameterName, "parameterName");
            this.DbType = dbType;
            this.Size = size;
            this.Direction = direction;
            this.IsNullable = isNullable;
            this.Precision = precision;
            this.Scale = scale;
            this.SourceColumn = sourceColumn;
            this.SourceVersion = sourceVersion;
            this.Value = value;
        }

        internal EntityParameter Clone() => 
            new EntityParameter(this);

        private void CloneHelper(EntityParameter destination)
        {
            this.CloneHelperCore(destination);
            destination._parameterName = this._parameterName;
            destination._dbType = this._dbType;
            destination._precision = this._precision;
            destination._scale = this._scale;
        }

        private void CloneHelperCore(EntityParameter destination)
        {
            destination._value = this._value;
            destination._direction = this._direction;
            destination._size = this._size;
            destination._sourceColumn = this._sourceColumn;
            destination._sourceVersion = this._sourceVersion;
            destination._sourceColumnNullMapping = this._sourceColumnNullMapping;
            destination._isNullable = this._isNullable;
        }

        internal object CompareExchangeParent(object value, object comparand)
        {
            object obj2 = this._parent;
            if (comparand == obj2)
            {
                this._parent = value;
            }
            return obj2;
        }

        internal void CopyTo(DbParameter destination)
        {
            ADP.CheckArgumentNull(destination, "destination");
            this.CloneHelper((EntityParameter) destination);
        }

        internal TypeUsage GetTypeUsage(MetadataWorkspace metadataWorkspace)
        {
            FacetValues values = null;
            System.Data.DbType dbType = this.DbType;
            PrimitiveTypeKind primitiveTypeKind = PrimitiveTypeKind.String;
            switch (dbType)
            {
                case System.Data.DbType.AnsiString:
                    values = new FacetValues {
                        Unicode = 0,
                        FixedLength = 0,
                        MaxLength = 0
                    };
                    primitiveTypeKind = PrimitiveTypeKind.String;
                    break;

                case System.Data.DbType.Binary:
                    values = new FacetValues {
                        MaxLength = 0
                    };
                    primitiveTypeKind = PrimitiveTypeKind.Binary;
                    break;

                case System.Data.DbType.Byte:
                    primitiveTypeKind = PrimitiveTypeKind.Byte;
                    break;

                case System.Data.DbType.Boolean:
                    primitiveTypeKind = PrimitiveTypeKind.Boolean;
                    break;

                case System.Data.DbType.Currency:
                case System.Data.DbType.Decimal:
                    primitiveTypeKind = PrimitiveTypeKind.Decimal;
                    values = new FacetValues {
                        Precision = 0,
                        Scale = 0
                    };
                    break;

                case System.Data.DbType.Date:
                case System.Data.DbType.DateTime:
                    primitiveTypeKind = PrimitiveTypeKind.DateTime;
                    break;

                case System.Data.DbType.Double:
                    primitiveTypeKind = PrimitiveTypeKind.Double;
                    break;

                case System.Data.DbType.Guid:
                    primitiveTypeKind = PrimitiveTypeKind.Guid;
                    break;

                case System.Data.DbType.Int16:
                    primitiveTypeKind = PrimitiveTypeKind.Int16;
                    break;

                case System.Data.DbType.Int32:
                    primitiveTypeKind = PrimitiveTypeKind.Int32;
                    break;

                case System.Data.DbType.Int64:
                    primitiveTypeKind = PrimitiveTypeKind.Int64;
                    break;

                case System.Data.DbType.SByte:
                    primitiveTypeKind = PrimitiveTypeKind.SByte;
                    break;

                case System.Data.DbType.Single:
                    primitiveTypeKind = PrimitiveTypeKind.Single;
                    break;

                case System.Data.DbType.String:
                    values = new FacetValues {
                        Unicode = 1,
                        FixedLength = 0,
                        MaxLength = 0
                    };
                    primitiveTypeKind = PrimitiveTypeKind.String;
                    break;

                case System.Data.DbType.Time:
                    primitiveTypeKind = PrimitiveTypeKind.Time;
                    values = new FacetValues {
                        Precision = 0
                    };
                    break;

                case System.Data.DbType.AnsiStringFixedLength:
                    values = new FacetValues {
                        Unicode = 0,
                        FixedLength = 1,
                        MaxLength = 0
                    };
                    primitiveTypeKind = PrimitiveTypeKind.String;
                    break;

                case System.Data.DbType.StringFixedLength:
                    values = new FacetValues {
                        Unicode = 1,
                        FixedLength = 1,
                        MaxLength = 0
                    };
                    primitiveTypeKind = PrimitiveTypeKind.String;
                    break;

                case System.Data.DbType.Xml:
                    values = new FacetValues {
                        Unicode = 1,
                        FixedLength = 0,
                        MaxLength = 0
                    };
                    primitiveTypeKind = PrimitiveTypeKind.String;
                    break;

                case System.Data.DbType.DateTime2:
                    primitiveTypeKind = PrimitiveTypeKind.DateTime;
                    values = new FacetValues {
                        Precision = 0
                    };
                    break;

                case System.Data.DbType.DateTimeOffset:
                    primitiveTypeKind = PrimitiveTypeKind.DateTimeOffset;
                    values = new FacetValues {
                        Precision = 0
                    };
                    break;

                default:
                    throw EntityUtil.InvalidOperation(Strings.EntityClient_UnsupportedDbType(dbType.ToString(), this.ParameterName));
            }
            PrimitiveType modelPrimitiveType = metadataWorkspace.GetModelPrimitiveType(primitiveTypeKind);
            if (values == null)
            {
                values = new FacetValues();
            }
            TypeUsage usage = TypeUsage.Create(modelPrimitiveType, values);
            if (usage == null)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_NoSuitableType(this.ParameterName));
            }
            return usage;
        }

        private void PropertyChanging()
        {
            this._isDirty = true;
        }

        public override void ResetDbType()
        {
            if (this._dbType.HasValue)
            {
                this.PropertyChanging();
            }
            this._dbType = null;
        }

        internal void ResetIsDirty()
        {
            this._isDirty = false;
        }

        internal void ResetParent()
        {
            this._parent = null;
        }

        private void ResetSize()
        {
            if (this._size.HasValue)
            {
                this.PropertyChanging();
                this._size = null;
            }
        }

        private void SetParameterNameWithValidation(string parameterName, string argumentName)
        {
            if (!string.IsNullOrEmpty(parameterName) && !DbCommandTree.IsValidParameterName(parameterName))
            {
                throw EntityUtil.Argument(Strings.EntityClient_InvalidParameterName(parameterName), argumentName);
            }
            this.PropertyChanging();
            this._parameterName = parameterName;
        }

        private bool ShouldSerializeSize() => 
            (this._size.HasValue && (this._size.Value != 0));

        public override string ToString() => 
            this.ParameterName;

        private byte ValuePrecisionCore(object value)
        {
            if (value is decimal)
            {
                SqlDecimal num = (decimal) value;
                return num.Precision;
            }
            return 0;
        }

        private byte ValueScaleCore(object value)
        {
            if (value is decimal)
            {
                return (byte) ((decimal.GetBits((decimal) value)[3] & 0xff0000) >> 0x10);
            }
            return 0;
        }

        private int ValueSize(object value) => 
            this.ValueSizeCore(value);

        private int ValueSizeCore(object value)
        {
            if (!ADP.IsNull(value))
            {
                string str = value as string;
                if (str != null)
                {
                    return str.Length;
                }
                byte[] buffer = value as byte[];
                if (buffer != null)
                {
                    return buffer.Length;
                }
                char[] chArray = value as char[];
                if (chArray != null)
                {
                    return chArray.Length;
                }
                if ((value is byte) || (value is char))
                {
                    return 1;
                }
            }
            return 0;
        }

        private object CoercedValue
        {
            get => 
                this._coercedValue;
            set
            {
                this._coercedValue = value;
            }
        }

        public override System.Data.DbType DbType
        {
            get
            {
                if (!this._dbType.HasValue)
                {
                    if (this._value == null)
                    {
                        return System.Data.DbType.String;
                    }
                    try
                    {
                        return TypeHelpers.ConvertClrTypeToDbType(this._value.GetType());
                    }
                    catch (ArgumentException exception)
                    {
                        throw EntityUtil.InvalidOperation(Strings.EntityClient_CannotDeduceDbType, exception);
                    }
                }
                return this._dbType.Value;
            }
            set
            {
                this.PropertyChanging();
                this._dbType = new System.Data.DbType?(value);
            }
        }

        [ResDescription("DbParameter_Direction"), ResCategory("DataCategory_Data"), RefreshProperties(RefreshProperties.All)]
        public override ParameterDirection Direction
        {
            get
            {
                ParameterDirection direction = this._direction;
                if (direction == ((ParameterDirection) 0))
                {
                    return ParameterDirection.Input;
                }
                return direction;
            }
            set
            {
                if (this._direction != value)
                {
                    switch (value)
                    {
                        case ParameterDirection.Input:
                        case ParameterDirection.Output:
                        case ParameterDirection.InputOutput:
                        case ParameterDirection.ReturnValue:
                            this.PropertyChanging();
                            this._direction = value;
                            return;
                    }
                    throw ADP.InvalidParameterDirection(value);
                }
            }
        }

        internal bool IsDbTypeSpecified =>
            this._dbType.HasValue;

        internal bool IsDirectionSpecified =>
            (this._direction != ((ParameterDirection) 0));

        internal bool IsDirty =>
            this._isDirty;

        internal bool IsIsNullableSpecified =>
            this._isNullable.HasValue;

        public override bool IsNullable
        {
            get => 
                (this._isNullable.HasValue ? this._isNullable.Value : true);
            set
            {
                this._isNullable = new bool?(value);
            }
        }

        internal bool IsPrecisionSpecified =>
            this._precision.HasValue;

        internal bool IsScaleSpecified =>
            this._scale.HasValue;

        internal bool IsSizeSpecified =>
            this._size.HasValue;

        internal int Offset =>
            0;

        public override string ParameterName
        {
            get => 
                (this._parameterName ?? "");
            set
            {
                this.SetParameterNameWithValidation(value, "value");
            }
        }

        public byte Precision
        {
            get => 
                (this._precision.HasValue ? this._precision.Value : ((byte) 0));
            set
            {
                this.PropertyChanging();
                this._precision = new byte?(value);
            }
        }

        public byte Scale
        {
            get => 
                (this._scale.HasValue ? this._scale.Value : ((byte) 0));
            set
            {
                this.PropertyChanging();
                this._scale = new byte?(value);
            }
        }

        [ResDescription("DbParameter_Size"), ResCategory("DataCategory_Data")]
        public override int Size
        {
            get
            {
                int num = this._size.HasValue ? this._size.Value : 0;
                if (num == 0)
                {
                    num = this.ValueSize(this.Value);
                }
                return num;
            }
            set
            {
                if (!this._size.HasValue || (this._size.Value != value))
                {
                    if (value < -1)
                    {
                        throw ADP.InvalidSizeValue(value);
                    }
                    this.PropertyChanging();
                    if (value == 0)
                    {
                        this._size = null;
                    }
                    else
                    {
                        this._size = new int?(value);
                    }
                }
            }
        }

        [ResCategory("DataCategory_Update"), ResDescription("DbParameter_SourceColumn")]
        public override string SourceColumn
        {
            get
            {
                string str = this._sourceColumn;
                if (str == null)
                {
                    return ADP.StrEmpty;
                }
                return str;
            }
            set
            {
                this._sourceColumn = value;
            }
        }

        public override bool SourceColumnNullMapping
        {
            get => 
                this._sourceColumnNullMapping;
            set
            {
                this._sourceColumnNullMapping = value;
            }
        }

        [ResDescription("DbParameter_SourceVersion"), ResCategory("DataCategory_Update")]
        public override DataRowVersion SourceVersion
        {
            get
            {
                DataRowVersion version = this._sourceVersion;
                if (version == ((DataRowVersion) 0))
                {
                    return DataRowVersion.Current;
                }
                return version;
            }
            set
            {
                DataRowVersion version = value;
                if (version <= DataRowVersion.Current)
                {
                    switch (version)
                    {
                        case DataRowVersion.Original:
                        case DataRowVersion.Current:
                            goto Label_002C;
                    }
                    goto Label_0034;
                }
                if ((version != DataRowVersion.Proposed) && (version != DataRowVersion.Default))
                {
                    goto Label_0034;
                }
            Label_002C:
                this._sourceVersion = value;
                return;
            Label_0034:
                throw ADP.InvalidDataRowVersion(value);
            }
        }

        public override object Value
        {
            get => 
                this._value;
            set
            {
                if (!this._dbType.HasValue)
                {
                    System.Data.DbType type = System.Data.DbType.String;
                    if (this._value != null)
                    {
                        type = TypeHelpers.ConvertClrTypeToDbType(this._value.GetType());
                    }
                    System.Data.DbType type2 = System.Data.DbType.String;
                    if (value != null)
                    {
                        type2 = TypeHelpers.ConvertClrTypeToDbType(value.GetType());
                    }
                    if (type != type2)
                    {
                        this.PropertyChanging();
                    }
                }
                this._value = value;
            }
        }
    }
}

