﻿namespace System.ServiceModel.Channels
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;

    internal class DoneReceivingAsyncResult : CompletedAsyncResult
    {
        internal DoneReceivingAsyncResult(AsyncCallback callback, object state) : base(callback, state)
        {
        }

        internal static bool End(DoneReceivingAsyncResult result) => 
            true;

        internal static bool End(DoneReceivingAsyncResult result, out Message message)
        {
            message = null;
            return true;
        }

        internal static bool End(DoneReceivingAsyncResult result, out RequestContext requestContext)
        {
            requestContext = null;
            return true;
        }
    }
}

