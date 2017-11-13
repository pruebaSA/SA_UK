namespace System.Data.Services.Common
{
    using System;
    using System.Data.Services.Client;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("EntityPropertyMappingInfo {DefiningType}")]
    internal sealed class EntityPropertyMappingInfo
    {
        public ClientType ActualType { get; set; }

        public EntityPropertyMappingAttribute Attribute { get; set; }

        public Type DefiningType { get; set; }
    }
}

