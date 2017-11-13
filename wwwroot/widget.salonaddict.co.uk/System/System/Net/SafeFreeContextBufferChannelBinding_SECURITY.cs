﻿namespace System.Net
{
    using System;
    using System.Security;

    [SuppressUnmanagedCodeSecurity]
    internal sealed class SafeFreeContextBufferChannelBinding_SECURITY : SafeFreeContextBufferChannelBinding
    {
        protected override bool ReleaseHandle() => 
            (UnsafeNclNativeMethods.SafeNetHandles_SECURITY.FreeContextBuffer(base.handle) == 0);
    }
}

