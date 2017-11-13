namespace System.Data.Objects
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct StateManagerValue
    {
        internal StateManagerMemberMetadata memberMetadata;
        internal object userObject;
        internal object originalValue;
        internal StateManagerValue(StateManagerMemberMetadata metadata, object instance, object value)
        {
            this.memberMetadata = metadata;
            this.userObject = instance;
            this.originalValue = value;
        }
    }
}

