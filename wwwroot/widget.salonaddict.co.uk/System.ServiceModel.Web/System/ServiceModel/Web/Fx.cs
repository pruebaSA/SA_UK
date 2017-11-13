namespace System.ServiceModel.Web
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.ServiceModel.Diagnostics;

    internal static class Fx
    {
        [MethodImpl(MethodImplOptions.NoInlining), Conditional("DEBUG")]
        internal static void Assert(string message)
        {
            AssertUtility.DebugAssertCore(message);
        }

        [Conditional("DEBUG")]
        internal static void Assert(bool condition, string message)
        {
        }
    }
}

