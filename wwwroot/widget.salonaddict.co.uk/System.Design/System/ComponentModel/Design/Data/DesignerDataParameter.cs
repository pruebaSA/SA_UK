namespace System.ComponentModel.Design.Data
{
    using System;
    using System.Data;

    public sealed class DesignerDataParameter
    {
        private DbType _dataType;
        private ParameterDirection _direction;
        private string _name;

        public DesignerDataParameter(string name, DbType dataType, ParameterDirection direction)
        {
            this._dataType = dataType;
            this._direction = direction;
            this._name = name;
        }

        public DbType DataType =>
            this._dataType;

        public ParameterDirection Direction =>
            this._direction;

        public string Name =>
            this._name;
    }
}

