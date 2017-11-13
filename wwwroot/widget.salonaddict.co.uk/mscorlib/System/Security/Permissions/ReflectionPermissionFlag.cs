namespace System.Security.Permissions
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), Flags]
    public enum ReflectionPermissionFlag
    {
        AllFlags = 7,
        MemberAccess = 2,
        NoFlags = 0,
        ReflectionEmit = 4,
        [ComVisible(false)]
        RestrictedMemberAccess = 8,
        [Obsolete("This API has been deprecated. http://go.microsoft.com/fwlink/?linkid=14202")]
        TypeInformation = 1
    }
}

