﻿namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Security;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000011a-0000-0000-C000-000000000046"), SuppressUnmanagedCodeSecurity]
    internal interface IParseDisplayName
    {
        void ParseDisplayName(IBindCtx pbc, [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, IntPtr pchEaten, IntPtr ppmkOut);
    }
}

