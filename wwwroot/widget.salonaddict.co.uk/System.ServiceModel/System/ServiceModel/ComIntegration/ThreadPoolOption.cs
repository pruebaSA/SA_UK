﻿namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(false)]
    internal enum ThreadPoolOption
    {
        None,
        Inherit,
        STA,
        MTA
    }
}

