﻿namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("C7CD7379-F3F2-4634-811B-703281D73E08")]
    internal interface IServiceSxsConfig
    {
        void SxsConfig(CSC_SxsConfig sxsConfig);
        void SxsName([MarshalAs(UnmanagedType.LPWStr)] string szSxsName);
        void SxsDirectory([MarshalAs(UnmanagedType.LPWStr)] string szSxsDirectory);
    }
}

