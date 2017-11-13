namespace System.Runtime.Serialization.Formatters
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true), StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey="0x00000000000000000400000000000000", Name="System.Runtime.Remoting")]
    public sealed class InternalRM
    {
        [Conditional("_LOGGING")]
        public static void InfoSoap(params object[] messages)
        {
        }

        public static bool SoapCheckEnabled() => 
            BCLDebug.CheckEnabled("SOAP");
    }
}

