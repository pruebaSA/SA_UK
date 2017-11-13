﻿namespace System.ServiceModel
{
    using System;

    internal static class MsmqAuthenticationModeHelper
    {
        public static bool IsDefined(MsmqAuthenticationMode mode) => 
            ((mode >= MsmqAuthenticationMode.None) && (mode <= MsmqAuthenticationMode.Certificate));
    }
}

