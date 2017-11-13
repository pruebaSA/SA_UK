namespace System.Runtime.Remoting
{
    using System;

    [Serializable]
    internal class DynamicTypeInfo : TypeInfo
    {
        internal DynamicTypeInfo(Type typeOfObj) : base(typeOfObj)
        {
        }

        public override bool CanCastTo(Type castType, object o) => 
            ((MarshalByRefObject) o).IsInstanceOfType(castType);
    }
}

