namespace MS.Internal.Security.RightsManagement
{
    using System;
    using System.Runtime.CompilerServices;

    internal delegate int CallbackDelegate(StatusMessage status, int hr, IntPtr pvParam, IntPtr pvContext);
}

