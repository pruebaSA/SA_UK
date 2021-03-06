﻿namespace System.Runtime.Remoting.Contexts
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface IDynamicProperty
    {
        string Name { [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)] get; }
    }
}

