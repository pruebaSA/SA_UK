namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("d91e12d8-98ed-47fa-9936-39421283d59b")]
    internal interface IDefinitionAppId
    {
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string get_SubscriptionId();
        void put_SubscriptionId([In, MarshalAs(UnmanagedType.LPWStr)] string Subscription);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string get_Codebase();
        void put_Codebase([In, MarshalAs(UnmanagedType.LPWStr)] string CodeBase);
        System.Deployment.Internal.Isolation.IEnumDefinitionIdentity EnumAppPath();
        void SetAppPath([In] uint cIDefinitionIdentity, [In, MarshalAs(UnmanagedType.LPArray)] System.Deployment.Internal.Isolation.IDefinitionIdentity[] DefinitionIdentity);
    }
}

