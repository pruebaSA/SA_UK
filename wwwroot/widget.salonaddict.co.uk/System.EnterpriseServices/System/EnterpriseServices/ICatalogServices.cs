namespace System.EnterpriseServices
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("04C6BE1E-1DB1-4058-AB7A-700CCCFBF254")]
    internal interface ICatalogServices
    {
        [AutoComplete(true)]
        void Autodone();
        [AutoComplete(false)]
        void NotAutodone();
    }
}

