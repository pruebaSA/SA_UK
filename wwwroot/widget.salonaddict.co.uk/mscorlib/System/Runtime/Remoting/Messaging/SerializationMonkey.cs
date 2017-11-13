namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Security.Permissions;

    [Serializable]
    internal class SerializationMonkey : ISerializable, IFieldInfo
    {
        internal ISerializationRootObject _obj;
        internal string[] fieldNames;
        internal Type[] fieldTypes;

        internal SerializationMonkey(SerializationInfo info, StreamingContext ctx)
        {
            this._obj.RootSetObjectData(info, ctx);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_Method"));
        }

        public string[] FieldNames
        {
            get => 
                this.fieldNames;
            set
            {
                this.fieldNames = value;
            }
        }

        public Type[] FieldTypes
        {
            get => 
                this.fieldTypes;
            set
            {
                this.fieldTypes = value;
            }
        }
    }
}

