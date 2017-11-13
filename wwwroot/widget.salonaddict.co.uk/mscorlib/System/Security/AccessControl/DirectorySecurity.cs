namespace System.Security.AccessControl
{
    using System;
    using System.IO;
    using System.Security.Permissions;

    public sealed class DirectorySecurity : FileSystemSecurity
    {
        public DirectorySecurity() : base(true)
        {
        }

        [SecurityPermission(SecurityAction.Assert, UnmanagedCode=true)]
        public DirectorySecurity(string name, AccessControlSections includeSections) : base(true, name, includeSections, true)
        {
            string fullPathInternal = Path.GetFullPathInternal(name);
            new FileIOPermission(FileIOPermissionAccess.NoAccess, AccessControlActions.View, fullPathInternal).Demand();
        }
    }
}

