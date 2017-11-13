namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.Threading;

    internal sealed class Msmq4PoisonHandler : IPoisonHandlingStrategy, IDisposable
    {
        private bool disposed;
        private MsmqQueue mainQueue;
        private MsmqQueue mainQueueForMove;
        private string mainQueueName;
        private static AsyncCallback onPeekCompleted = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(Msmq4PoisonHandler.OnPeekCompleted));
        private static WaitCallback onStartPeek = new WaitCallback(Msmq4PoisonHandler.StartPeek);
        private MsmqQueue poisonQueue;
        private string poisonQueueName;
        private MsmqReceiveHelper receiver;
        private MsmqQueue retryQueueForMove;
        private MsmqQueue retryQueueForPeek;
        private MsmqRetryQueueMessage retryQueueMessage;
        private string retryQueueName;
        private IOThreadTimer timer;

        public Msmq4PoisonHandler(MsmqReceiveHelper receiver)
        {
            this.receiver = receiver;
            this.timer = new IOThreadTimer(new WaitCallback(this.OnTimer), null, false);
            this.disposed = false;
            this.mainQueueName = this.ReceiveParameters.AddressTranslator.UriToFormatName(this.ListenUri);
            this.poisonQueueName = this.ReceiveParameters.AddressTranslator.UriToFormatName(new Uri(this.ListenUri.AbsoluteUri + ";poison"));
            this.retryQueueName = this.ReceiveParameters.AddressTranslator.UriToFormatName(new Uri(this.ListenUri.AbsoluteUri + ";retry"));
        }

        public bool CheckAndHandlePoisonMessage(MsmqMessageProperty messageProperty)
        {
            if (messageProperty.AbortCount <= this.ReceiveParameters.ReceiveRetryCount)
            {
                return false;
            }
            int num = messageProperty.MoveCount / 2;
            lock (this)
            {
                if (this.disposed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(base.GetType().ToString()));
                }
                if (num >= this.ReceiveParameters.MaxRetryCycles)
                {
                    this.FinalDisposition(messageProperty);
                }
                else
                {
                    MsmqReceiveHelper.MoveReceivedMessage(this.mainQueue, this.retryQueueForMove, messageProperty.LookupId);
                    MsmqDiagnostics.PoisonMessageMoved(messageProperty.MessageId, false, this.receiver.InstanceId);
                }
            }
            return true;
        }

        public void Dispose()
        {
            lock (this)
            {
                if (!this.disposed)
                {
                    this.disposed = true;
                    this.timer.Cancel();
                    if (this.retryQueueForPeek != null)
                    {
                        this.retryQueueForPeek.Dispose();
                    }
                    if (this.retryQueueForMove != null)
                    {
                        this.retryQueueForMove.Dispose();
                    }
                    if (this.poisonQueue != null)
                    {
                        this.poisonQueue.Dispose();
                    }
                    if (this.mainQueueForMove != null)
                    {
                        this.mainQueueForMove.Dispose();
                    }
                }
            }
        }

        public void FinalDisposition(MsmqMessageProperty messageProperty)
        {
            switch (this.ReceiveParameters.ReceiveErrorHandling)
            {
                case ReceiveErrorHandling.Fault:
                    MsmqReceiveHelper.TryAbortTransactionCurrent();
                    if (this.receiver.ChannelListener != null)
                    {
                        this.receiver.ChannelListener.FaultListener();
                    }
                    if (this.receiver.Channel == null)
                    {
                        break;
                    }
                    this.receiver.Channel.FaultChannel();
                    return;

                case ReceiveErrorHandling.Drop:
                    this.receiver.DropOrRejectReceivedMessage(messageProperty, false);
                    return;

                case ReceiveErrorHandling.Reject:
                    this.receiver.DropOrRejectReceivedMessage(messageProperty, true);
                    MsmqDiagnostics.PoisonMessageRejected(messageProperty.MessageId, this.receiver.InstanceId);
                    return;

                case ReceiveErrorHandling.Move:
                    MsmqReceiveHelper.MoveReceivedMessage(this.mainQueue, this.poisonQueue, messageProperty.LookupId);
                    MsmqDiagnostics.PoisonMessageMoved(messageProperty.MessageId, true, this.receiver.InstanceId);
                    break;

                default:
                    return;
            }
        }

        private static void OnPeekCompleted(IAsyncResult result)
        {
            Msmq4PoisonHandler asyncState = result.AsyncState as Msmq4PoisonHandler;
            MsmqQueue.ReceiveResult unknown = MsmqQueue.ReceiveResult.Unknown;
            try
            {
                unknown = asyncState.retryQueueForPeek.EndPeek(result);
            }
            catch (MsmqException exception)
            {
                MsmqDiagnostics.ExpectedException(exception);
            }
            if (MsmqQueue.ReceiveResult.MessageReceived == unknown)
            {
                lock (asyncState)
                {
                    if (!asyncState.disposed)
                    {
                        TimeSpan timeFromNow = (TimeSpan) ((MsmqDateTime.ToDateTime(asyncState.retryQueueMessage.LastMoveTime.Value) + asyncState.ReceiveParameters.RetryCycleDelay) - DateTime.UtcNow);
                        if (timeFromNow < TimeSpan.Zero)
                        {
                            asyncState.OnTimer(asyncState);
                        }
                        else
                        {
                            asyncState.timer.Set(timeFromNow);
                        }
                    }
                }
            }
        }

        private void OnTimer(object state)
        {
            lock (this)
            {
                if (!this.disposed)
                {
                    try
                    {
                        this.retryQueueForPeek.TryMoveMessage(this.retryQueueMessage.LookupId.Value, this.mainQueueForMove, MsmqTransactionMode.Single);
                    }
                    catch (MsmqException exception)
                    {
                        MsmqDiagnostics.ExpectedException(exception);
                    }
                    this.retryQueueForPeek.BeginPeek(this.retryQueueMessage, TimeSpan.MaxValue, onPeekCompleted, this);
                }
            }
        }

        public void Open()
        {
            this.mainQueue = this.receiver.Queue;
            this.mainQueueForMove = new MsmqQueue(this.mainQueueName, 4);
            this.poisonQueue = new MsmqQueue(this.poisonQueueName, 4);
            this.retryQueueForMove = new MsmqQueue(this.retryQueueName, 4);
            this.retryQueueForPeek = new MsmqQueue(this.retryQueueName, 1);
            this.retryQueueMessage = new MsmqRetryQueueMessage();
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                StartPeek(this);
            }
            else
            {
                IOThreadScheduler.ScheduleCallback(onStartPeek, this);
            }
        }

        private static void StartPeek(object state)
        {
            Msmq4PoisonHandler handler = state as Msmq4PoisonHandler;
            lock (handler)
            {
                if (!handler.disposed)
                {
                    handler.retryQueueForPeek.BeginPeek(handler.retryQueueMessage, TimeSpan.MaxValue, onPeekCompleted, handler);
                }
            }
        }

        private Uri ListenUri =>
            this.receiver.ListenUri;

        private MsmqReceiveParameters ReceiveParameters =>
            this.receiver.MsmqReceiveParameters;

        private class MsmqRetryQueueMessage : NativeMsmqMessage
        {
            private NativeMsmqMessage.IntProperty lastMoveTime;
            private NativeMsmqMessage.LongProperty lookupId;

            public MsmqRetryQueueMessage() : base(2)
            {
                this.lookupId = new NativeMsmqMessage.LongProperty(this, 60);
                this.lastMoveTime = new NativeMsmqMessage.IntProperty(this, 0x4b);
            }

            public NativeMsmqMessage.IntProperty LastMoveTime =>
                this.lastMoveTime;

            public NativeMsmqMessage.LongProperty LookupId =>
                this.lookupId;
        }
    }
}

