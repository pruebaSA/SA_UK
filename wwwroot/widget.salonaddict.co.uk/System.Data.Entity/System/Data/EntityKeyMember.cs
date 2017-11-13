namespace System.Data
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    [Serializable, DataContract]
    public class EntityKeyMember
    {
        private string _keyName;
        private object _keyValue;

        public EntityKeyMember()
        {
        }

        public EntityKeyMember(string keyName, object keyValue)
        {
            EntityUtil.CheckArgumentNull<string>(keyName, "keyName");
            EntityUtil.CheckArgumentNull<object>(keyValue, "keyValue");
            this._keyName = keyName;
            this._keyValue = keyValue;
        }

        public override string ToString() => 
            string.Format(CultureInfo.CurrentCulture, "[{0}, {1}]", new object[] { this._keyName, this._keyValue });

        private void ValidateWritable(object instance)
        {
            if (instance != null)
            {
                throw EntityUtil.CannotChangeEntityKey();
            }
        }

        [DataMember]
        public string Key
        {
            get => 
                this._keyName;
            set
            {
                this.ValidateWritable(this._keyName);
                EntityUtil.CheckArgumentNull<string>(value, "value");
                this._keyName = value;
            }
        }

        [DataMember]
        public object Value
        {
            get => 
                this._keyValue;
            set
            {
                this.ValidateWritable(this._keyValue);
                EntityUtil.CheckArgumentNull<object>(value, "value");
                this._keyValue = value;
            }
        }
    }
}

