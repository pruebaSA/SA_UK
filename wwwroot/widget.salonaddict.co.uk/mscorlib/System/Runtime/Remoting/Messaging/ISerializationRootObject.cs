namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    internal interface ISerializationRootObject
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        void RootSetObjectData(SerializationInfo info, StreamingContext ctx);
    }
}

