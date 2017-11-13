namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Threading;

    internal class TransportManagerContainer
    {
        private bool closed;
        private TransportChannelListener listener;
        private object tableLock;
        private IList<TransportManager> transportManagers;

        public TransportManagerContainer(TransportChannelListener listener)
        {
            this.listener = listener;
            this.tableLock = listener.TransportManagerTable;
            this.transportManagers = new List<TransportManager>();
        }

        private TransportManagerContainer(TransportManagerContainer source)
        {
            this.listener = source.listener;
            this.tableLock = source.tableLock;
            this.transportManagers = new List<TransportManager>();
            for (int i = 0; i < source.transportManagers.Count; i++)
            {
                this.transportManagers.Add(source.transportManagers[i]);
            }
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state) => 
            new CloseAsyncResult(this, callback, state);

        public IAsyncResult BeginOpen(SelectTransportManagersCallback selectTransportManagerCallback, AsyncCallback callback, object state) => 
            new OpenAsyncResult(selectTransportManagerCallback, this, callback, state);

        public void Close()
        {
            if (!this.closed)
            {
                lock (this.tableLock)
                {
                    if (!this.closed)
                    {
                        this.closed = true;
                        foreach (TransportManager manager in this.transportManagers)
                        {
                            manager.Close(this.listener);
                        }
                        this.transportManagers.Clear();
                    }
                }
            }
        }

        public void EndClose(IAsyncResult result)
        {
            CloseAsyncResult.End(result);
        }

        public void EndOpen(IAsyncResult result)
        {
            OpenAsyncResult.End(result);
        }

        public void Open(SelectTransportManagersCallback selectTransportManagerCallback)
        {
            lock (this.tableLock)
            {
                if (!this.closed)
                {
                    IList<TransportManager> list = selectTransportManagerCallback();
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            TransportManager item = list[i];
                            item.Open(this.listener);
                            this.transportManagers.Add(item);
                        }
                    }
                }
            }
        }

        public static TransportManagerContainer TransferTransportManagers(TransportManagerContainer source)
        {
            TransportManagerContainer container = null;
            lock (source.tableLock)
            {
                if (source.transportManagers.Count > 0)
                {
                    container = new TransportManagerContainer(source);
                    source.transportManagers.Clear();
                }
            }
            return container;
        }

        private sealed class CloseAsyncResult : TransportManagerContainer.OpenOrCloseAsyncResult
        {
            public CloseAsyncResult(TransportManagerContainer parent, AsyncCallback callback, object state) : base(parent, callback, state)
            {
                base.Begin();
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<TransportManagerContainer.CloseAsyncResult>(result);
            }

            protected override void OnScheduled(TransportManagerContainer parent)
            {
                parent.Close();
            }
        }

        private sealed class OpenAsyncResult : TransportManagerContainer.OpenOrCloseAsyncResult
        {
            private SelectTransportManagersCallback selectTransportManagerCallback;

            public OpenAsyncResult(SelectTransportManagersCallback selectTransportManagerCallback, TransportManagerContainer parent, AsyncCallback callback, object state) : base(parent, callback, state)
            {
                this.selectTransportManagerCallback = selectTransportManagerCallback;
                base.Begin();
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<TransportManagerContainer.OpenAsyncResult>(result);
            }

            protected override void OnScheduled(TransportManagerContainer parent)
            {
                parent.Open(this.selectTransportManagerCallback);
            }
        }

        private abstract class OpenOrCloseAsyncResult : TraceAsyncResult
        {
            private TransportManagerContainer parent;
            private static WaitCallback scheduledCallback = new WaitCallback(TransportManagerContainer.OpenOrCloseAsyncResult.OnScheduled);

            protected OpenOrCloseAsyncResult(TransportManagerContainer parent, AsyncCallback callback, object state) : base(callback, state)
            {
                this.parent = parent;
            }

            protected void Begin()
            {
                IOThreadScheduler.ScheduleCallback(scheduledCallback, this);
            }

            private void OnScheduled()
            {
                using (ServiceModelActivity.BoundOperation(base.CallbackActivity))
                {
                    Exception exception = null;
                    try
                    {
                        this.OnScheduled(this.parent);
                    }
                    catch (Exception exception2)
                    {
                        if (DiagnosticUtility.IsFatal(exception2))
                        {
                            throw;
                        }
                        exception = exception2;
                    }
                    base.Complete(false, exception);
                }
            }

            private static void OnScheduled(object state)
            {
                ((TransportManagerContainer.OpenOrCloseAsyncResult) state).OnScheduled();
            }

            protected abstract void OnScheduled(TransportManagerContainer parent);
        }
    }
}

