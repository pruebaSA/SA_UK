namespace System.ComponentModel.Design.Data
{
    using System;
    using System.Data;

    public sealed class DesignerDataColumn
    {
        private DbType _dataType;
        private object _defaultValue;
        private bool _identity;
        private int _length;
        private string _name;
        private bool _nullable;
        private int _precision;
        private bool _primaryKey;
        private int _scale;

        public DesignerDataColumn(string name, DbType dataType) : this(name, dataType, null, false, false, false, -1, -1, -1)
        {
        }

        public DesignerDataColumn(string name, DbType dataType, object defaultValue) : this(name, dataType, defaultValue, false, false, false, -1, -1, -1)
        {
        }

        public DesignerDataColumn(string name, DbType dataType, object defaultValue, bool identity, bool nullable, bool primaryKey, int precision, int scale, int length)
        {
            this._dataType = dataType;
            this._defaultValue = defaultValue;
            this._identity = identity;
            this._length = length;
            this._name = name;
            this._nullable = nullable;
            this._precision = precision;
            this._primaryKey = primaryKey;
            this._scale = scale;
        }

        public DbType DataType =>
            this._dataType;

        public object DefaultValue =>
            this._defaultValue;

        public bool Identity =>
            this._identity;

        public int Length =>
            this._length;

        public string Name =>
            this._name;

        public bool Nullable =>
            this._nullable;

        public int Precision =>
            this._precision;

        public bool PrimaryKey =>
            this._primaryKey;

        public int Scale =>
            this._scale;
    }
}

