﻿namespace System.EnterpriseServices.Internal
{
    using System;
    using System.Runtime.InteropServices;

    [Guid("d8013ef0-730b-45e2-ba24-874b7242c425")]
    public interface IComSoapIISVRoot
    {
        [DispId(1)]
        void Create([MarshalAs(UnmanagedType.BStr)] string RootWeb, [MarshalAs(UnmanagedType.BStr)] string PhysicalDirectory, [MarshalAs(UnmanagedType.BStr)] string VirtualDirectory, [MarshalAs(UnmanagedType.BStr)] out string Error);
        [DispId(2)]
        void Delete([MarshalAs(UnmanagedType.BStr)] string RootWeb, [MarshalAs(UnmanagedType.BStr)] string PhysicalDirectory, [MarshalAs(UnmanagedType.BStr)] string VirtualDirectory, [MarshalAs(UnmanagedType.BStr)] out string Error);
    }
}

