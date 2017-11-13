namespace System.Net
{
    using System;

    internal static class IntPtrHelper
    {
        internal static IntPtr Add(IntPtr a, int b) => 
            ((IntPtr) (((long) a) + b));

        internal static long Subtract(IntPtr a, IntPtr b) => 
            (((long) a) - ((long) b));
    }
}

