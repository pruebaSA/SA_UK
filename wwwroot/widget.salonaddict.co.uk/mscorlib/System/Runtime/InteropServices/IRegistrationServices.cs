namespace System.Runtime.InteropServices
{
    using System;
    using System.Reflection;
    using System.Security.Permissions;

    [ComVisible(true), Guid("CCBD682C-73A5-4568-B8B0-C7007E11ABA2")]
    public interface IRegistrationServices
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        bool RegisterAssembly(Assembly assembly, AssemblyRegistrationFlags flags);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        bool UnregisterAssembly(Assembly assembly);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        Type[] GetRegistrableTypesInAssembly(Assembly assembly);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        string GetProgIdForType(Type type);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        void RegisterTypeForComClients(Type type, ref Guid g);
        Guid GetManagedCategoryGuid();
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        bool TypeRequiresRegistration(Type type);
        bool TypeRepresentsComType(Type type);
    }
}

