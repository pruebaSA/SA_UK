﻿namespace System.Runtime.Remoting.Proxies
{
    using System;

    internal sealed class __TransparentProxy
    {
        private IntPtr _pInterfaceMT;
        private IntPtr _pMT;
        private RealProxy _rp;
        private IntPtr _stub;
        private object _stubData;

        private __TransparentProxy()
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_Constructor"));
        }
    }
}

