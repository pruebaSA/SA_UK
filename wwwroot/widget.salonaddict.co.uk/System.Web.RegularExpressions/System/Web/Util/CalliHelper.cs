namespace System.Web.Util
{
    using System;

    internal sealed class CalliHelper
    {
        internal CalliHelper()
        {
        }

        internal static void ArglessFunctionCaller(IntPtr fp, object o)
        {
            *?();
        }

        internal static void EventArgFunctionCaller(IntPtr fp, object o, object t, EventArgs e)
        {
            *?(t, e);
        }
    }
}

