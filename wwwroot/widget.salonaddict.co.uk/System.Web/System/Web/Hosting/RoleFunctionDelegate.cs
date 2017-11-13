namespace System.Web.Hosting
{
    using System;
    using System.Runtime.CompilerServices;

    internal delegate bool RoleFunctionDelegate(IntPtr pManagedPrincipal, IntPtr pszRole, int cchRole, bool disposing);
}

