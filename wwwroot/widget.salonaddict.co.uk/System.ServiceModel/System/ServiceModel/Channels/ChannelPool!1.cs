namespace System.ServiceModel.Channels
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal class ChannelPool<TChannel> : IdlingCommunicationPool<ChannelPoolKey, TChannel> where TChannel: class, IChannel
    {
        private static AsyncCallback onCloseComplete;

        static ChannelPool()
        {
            ChannelPool<TChannel>.onCloseComplete = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(ChannelPool<TChannel>.OnCloseComplete));
        }

        public ChannelPool(ChannelPoolSettings settings) : base(settings.MaxOutboundChannelsPerEndpoint, settings.IdleTimeout, settings.LeaseTimeout)
        {
        }

        protected override void AbortItem(TChannel item)
        {
            item.Abort();
        }

        protected override void CloseItem(TChannel item, TimeSpan timeout)
        {
            item.Close(timeout);
        }

        protected override void CloseItemAsync(TChannel item, TimeSpan timeout)
        {
            bool flag = false;
            try
            {
                IAsyncResult result = item.BeginClose(timeout, ChannelPool<TChannel>.onCloseComplete, item);
                if (result.CompletedSynchronously)
                {
                    item.EndClose(result);
                }
                flag = true;
            }
            finally
            {
                if (!flag)
                {
                    item.Abort();
                }
            }
        }

        protected override ChannelPoolKey GetPoolKey(EndpointAddress address, Uri via) => 
            new ChannelPoolKey(address, via);

        private static void OnCloseComplete(IAsyncResult result)
        {
            if (!result.CompletedSynchronously)
            {
                TChannel asyncState = (TChannel) result.AsyncState;
                bool flag = false;
                try
                {
                    asyncState.EndClose(result);
                    flag = true;
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.TraceHandledException, exception, null);
                    }
                }
                finally
                {
                    if (!flag)
                    {
                        asyncState.Abort();
                    }
                }
            }
        }
    }
}

