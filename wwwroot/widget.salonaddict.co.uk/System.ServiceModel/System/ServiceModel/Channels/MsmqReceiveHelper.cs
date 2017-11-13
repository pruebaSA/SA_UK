namespace System.ServiceModel.Channels
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Threading;
    using System.Transactions;

    internal sealed class MsmqReceiveHelper
    {
        private ServiceModelActivity activity;
        private MsmqInputChannelBase channel;
        private string instanceId;
        private MsmqChannelListenerBase listener;
        private IPoisonHandlingStrategy poisonHandler;
        private IMsmqMessagePool pool;
        private MsmqQueue queue;
        private string queueName;
        private System.ServiceModel.Channels.MsmqReceiveParameters receiveParameters;
        private Uri uri;

        internal MsmqReceiveHelper(System.ServiceModel.Channels.MsmqReceiveParameters receiveParameters, Uri uri, IMsmqMessagePool messagePool, MsmqInputChannelBase channel, MsmqChannelListenerBase listener)
        {
            this.queueName = receiveParameters.AddressTranslator.UriToFormatName(uri);
            this.receiveParameters = receiveParameters;
            this.uri = uri;
            this.queue = new MsmqQueue(this.receiveParameters.AddressTranslator.UriToFormatName(uri), 1);
            this.instanceId = uri.ToString().ToUpperInvariant();
            this.pool = messagePool;
            this.poisonHandler = Msmq.CreatePoisonHandler(this);
            this.channel = channel;
            this.listener = listener;
        }

        internal IAsyncResult BeginTryReceive(MsmqInputMessage msmqMessage, TimeSpan timeout, MsmqTransactionMode transactionMode, AsyncCallback callback, object state)
        {
            if (this.receiveParameters.ExactlyOnce)
            {
                return new TryTransactedReceiveAsyncResult(this, msmqMessage, timeout, transactionMode, callback, state);
            }
            return new TryNonTransactedReceiveAsyncResult(this, msmqMessage, timeout, callback, state);
        }

        internal IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state) => 
            new WaitForMessageAsyncResult(this.queue, timeout, callback, state);

        internal void Close()
        {
            using (ServiceModelActivity.BoundOperation(this.Activity))
            {
                this.poisonHandler.Dispose();
                this.queue.Dispose();
            }
            ServiceModelActivity.Stop(this.activity);
        }

        internal void DropOrRejectReceivedMessage(MsmqMessageProperty messageProperty, bool reject)
        {
            if (this.Transactional)
            {
                TryAbortTransactionCurrent();
                IPostRollbackErrorStrategy strategy = new SimplePostRollbackErrorStrategy(messageProperty.LookupId);
                MsmqQueue.MoveReceiveResult unknown = MsmqQueue.MoveReceiveResult.Unknown;
                do
                {
                    using (MsmqEmptyMessage message = new MsmqEmptyMessage())
                    {
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            unknown = this.Queue.TryReceiveByLookupId(messageProperty.LookupId, message, MsmqTransactionMode.CurrentOrThrow);
                            if ((MsmqQueue.MoveReceiveResult.Succeeded == unknown) && reject)
                            {
                                this.Queue.MarkMessageRejected(messageProperty.LookupId);
                            }
                            scope.Complete();
                        }
                    }
                    if (unknown == MsmqQueue.MoveReceiveResult.Succeeded)
                    {
                        MsmqDiagnostics.MessageConsumed(this.instanceId, messageProperty.MessageId, Msmq.IsRejectMessageSupported);
                    }
                }
                while ((unknown == MsmqQueue.MoveReceiveResult.MessageLockedUnderTransaction) && strategy.AnotherTryNeeded());
            }
            else
            {
                MsmqDiagnostics.MessageConsumed(this.instanceId, messageProperty.MessageId, false);
            }
        }

        internal bool EndTryReceive(IAsyncResult result, out MsmqInputMessage msmqMessage, out MsmqMessageProperty msmqProperty)
        {
            msmqMessage = null;
            msmqProperty = null;
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
            }
            if (this.receiveParameters.ExactlyOnce)
            {
                TryTransactedReceiveAsyncResult result2 = result as TryTransactedReceiveAsyncResult;
                if (result2 == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.ServiceModel.SR.GetString("InvalidAsyncResult"));
                }
                return TryTransactedReceiveAsyncResult.End(result2, out msmqMessage, out msmqProperty);
            }
            TryNonTransactedReceiveAsyncResult result3 = result as TryNonTransactedReceiveAsyncResult;
            if (result3 == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(System.ServiceModel.SR.GetString("InvalidAsyncResult"));
            }
            return TryNonTransactedReceiveAsyncResult.End(result3, out msmqMessage, out msmqProperty);
        }

        public bool EndWaitForMessage(IAsyncResult result) => 
            TypedAsyncResult<bool>.End(result);

        internal void FinalDisposition(MsmqMessageProperty messageProperty)
        {
            this.poisonHandler.FinalDisposition(messageProperty);
        }

        internal static void MoveReceivedMessage(MsmqQueue queueFrom, MsmqQueue queueTo, long lookupId)
        {
            TryAbortTransactionCurrent();
            IPostRollbackErrorStrategy strategy = new SimplePostRollbackErrorStrategy(lookupId);
            do
            {
                if (queueFrom.TryMoveMessage(lookupId, queueTo, MsmqTransactionMode.Single) != MsmqQueue.MoveReceiveResult.MessageLockedUnderTransaction)
                {
                    return;
                }
            }
            while (strategy.AnotherTryNeeded());
        }

        internal void Open()
        {
            this.activity = MsmqDiagnostics.StartListenAtActivity(this);
            using (MsmqDiagnostics.BoundOpenOperation(this))
            {
                this.queue.EnsureOpen();
                this.poisonHandler.Open();
            }
        }

        internal void ReturnMessage(MsmqInputMessage message)
        {
            this.pool.ReturnMessage(message);
        }

        internal MsmqInputMessage TakeMessage() => 
            this.pool.TakeMessage();

        internal static void TryAbortTransactionCurrent()
        {
            if (null != Transaction.Current)
            {
                try
                {
                    Transaction.Current.Rollback();
                }
                catch (TransactionAbortedException exception)
                {
                    MsmqDiagnostics.ExpectedException(exception);
                }
                catch (ObjectDisposedException exception2)
                {
                    MsmqDiagnostics.ExpectedException(exception2);
                }
            }
        }

        internal bool TryReceive(MsmqInputMessage msmqMessage, TimeSpan timeout, MsmqTransactionMode transactionMode, out MsmqMessageProperty property)
        {
            property = null;
            MsmqQueue.ReceiveResult result = this.Queue.TryReceive(msmqMessage, timeout, transactionMode);
            if (MsmqQueue.ReceiveResult.OperationCancelled != result)
            {
                if (MsmqQueue.ReceiveResult.Timeout == result)
                {
                    return false;
                }
                property = new MsmqMessageProperty(msmqMessage);
                if (this.Transactional && this.PoisonHandler.CheckAndHandlePoisonMessage(property))
                {
                    long lookupId = property.LookupId;
                    property = null;
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCritical(new MsmqPoisonMessageException(lookupId));
                }
            }
            return true;
        }

        internal bool WaitForMessage(TimeSpan timeout)
        {
            using (MsmqEmptyMessage message = new MsmqEmptyMessage())
            {
                return (MsmqQueue.ReceiveResult.Timeout != this.queue.TryPeek(message, timeout));
            }
        }

        internal ServiceModelActivity Activity =>
            this.activity;

        internal MsmqInputChannelBase Channel =>
            this.channel;

        internal MsmqChannelListenerBase ChannelListener =>
            this.listener;

        internal string InstanceId =>
            this.instanceId;

        internal Uri ListenUri =>
            this.uri;

        internal System.ServiceModel.Channels.MsmqReceiveParameters MsmqReceiveParameters =>
            this.receiveParameters;

        private IPoisonHandlingStrategy PoisonHandler =>
            this.poisonHandler;

        internal MsmqQueue Queue =>
            this.queue;

        internal bool Transactional =>
            this.receiveParameters.ExactlyOnce;

        private interface IPostRollbackErrorStrategy
        {
            bool AnotherTryNeeded();
        }

        private class SimplePostRollbackErrorStrategy : MsmqReceiveHelper.IPostRollbackErrorStrategy
        {
            private const int Attempts = 50;
            private int attemptsLeft = 50;
            private long lookupId;
            private const int MillisecondsToSleep = 100;

            internal SimplePostRollbackErrorStrategy(long lookupId)
            {
                this.lookupId = lookupId;
            }

            bool MsmqReceiveHelper.IPostRollbackErrorStrategy.AnotherTryNeeded()
            {
                if (--this.attemptsLeft > 0)
                {
                    if (this.attemptsLeft == 0x31)
                    {
                        MsmqDiagnostics.MessageLockedUnderTheTransaction(this.lookupId);
                    }
                    Thread.Sleep(TimeSpan.FromMilliseconds(100.0));
                    return true;
                }
                MsmqDiagnostics.MoveOrDeleteAttemptFailed(this.lookupId);
                return false;
            }
        }

        private class TryNonTransactedReceiveAsyncResult : AsyncResult
        {
            private MsmqInputMessage msmqMessage;
            private static AsyncCallback onCompleteStatic = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(MsmqReceiveHelper.TryNonTransactedReceiveAsyncResult.OnCompleteStatic));
            private MsmqReceiveHelper receiver;
            private MsmqQueue.ReceiveResult receiveResult;

            internal TryNonTransactedReceiveAsyncResult(MsmqReceiveHelper receiver, MsmqInputMessage msmqMessage, TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
            {
                this.receiver = receiver;
                this.msmqMessage = msmqMessage;
                receiver.Queue.BeginTryReceive(msmqMessage, timeout, onCompleteStatic, this);
            }

            internal static bool End(IAsyncResult result, out MsmqInputMessage msmqMessage, out MsmqMessageProperty property)
            {
                MsmqReceiveHelper.TryNonTransactedReceiveAsyncResult result2 = AsyncResult.End<MsmqReceiveHelper.TryNonTransactedReceiveAsyncResult>(result);
                msmqMessage = result2.msmqMessage;
                property = null;
                if (MsmqQueue.ReceiveResult.Timeout == result2.receiveResult)
                {
                    return false;
                }
                if (MsmqQueue.ReceiveResult.OperationCancelled != result2.receiveResult)
                {
                    property = new MsmqMessageProperty(msmqMessage);
                }
                return true;
            }

            private void OnComplete(IAsyncResult result)
            {
                Exception exception = null;
                try
                {
                    this.receiveResult = this.receiver.Queue.EndTryReceive(result);
                }
                catch (Exception exception2)
                {
                    if (DiagnosticUtility.IsFatal(exception2))
                    {
                        throw;
                    }
                    exception = exception2;
                }
                base.Complete(result.CompletedSynchronously, exception);
            }

            private static void OnCompleteStatic(IAsyncResult result)
            {
                (result.AsyncState as MsmqReceiveHelper.TryNonTransactedReceiveAsyncResult).OnComplete(result);
            }
        }

        private class TryTransactedReceiveAsyncResult : AsyncResult
        {
            private bool expired;
            private MsmqMessageProperty messageProperty;
            private MsmqInputMessage msmqMessage;
            private static WaitCallback onComplete = new WaitCallback(MsmqReceiveHelper.TryTransactedReceiveAsyncResult.OnComplete);
            private MsmqReceiveHelper receiver;
            private TimeoutHelper timeoutHelper;
            private MsmqTransactionMode transactionMode;
            private Transaction txCurrent;

            internal TryTransactedReceiveAsyncResult(MsmqReceiveHelper receiver, MsmqInputMessage msmqMessage, TimeSpan timeout, MsmqTransactionMode transactionMode, AsyncCallback callback, object state) : base(callback, state)
            {
                this.timeoutHelper = new TimeoutHelper(timeout);
                this.txCurrent = Transaction.Current;
                this.receiver = receiver;
                this.msmqMessage = msmqMessage;
                this.transactionMode = transactionMode;
                IOThreadScheduler.ScheduleCallback(onComplete, this);
            }

            internal static bool End(IAsyncResult result, out MsmqInputMessage msmqMessage, out MsmqMessageProperty property)
            {
                MsmqReceiveHelper.TryTransactedReceiveAsyncResult result2 = AsyncResult.End<MsmqReceiveHelper.TryTransactedReceiveAsyncResult>(result);
                msmqMessage = result2.msmqMessage;
                property = result2.messageProperty;
                return !result2.expired;
            }

            private static void OnComplete(object parameter)
            {
                MsmqReceiveHelper.TryTransactedReceiveAsyncResult result = parameter as MsmqReceiveHelper.TryTransactedReceiveAsyncResult;
                Transaction current = Transaction.Current;
                Transaction.Current = result.txCurrent;
                try
                {
                    Exception exception = null;
                    try
                    {
                        result.expired = !result.receiver.TryReceive(result.msmqMessage, result.timeoutHelper.RemainingTime(), result.transactionMode, out result.messageProperty);
                    }
                    catch (Exception exception2)
                    {
                        if (DiagnosticUtility.IsFatal(exception2))
                        {
                            throw;
                        }
                        exception = exception2;
                    }
                    result.Complete(false, exception);
                }
                finally
                {
                    Transaction.Current = current;
                }
            }
        }

        private class WaitForMessageAsyncResult : TypedAsyncResult<bool>
        {
            private MsmqEmptyMessage msmqMessage;
            private MsmqQueue msmqQueue;
            private static AsyncCallback onCompleteStatic = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(MsmqReceiveHelper.WaitForMessageAsyncResult.OnCompleteStatic));

            public WaitForMessageAsyncResult(MsmqQueue msmqQueue, TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
            {
                this.msmqMessage = new MsmqEmptyMessage();
                this.msmqQueue = msmqQueue;
                this.msmqQueue.BeginPeek(this.msmqMessage, timeout, onCompleteStatic, this);
            }

            private void OnComplete(IAsyncResult result)
            {
                this.msmqMessage.Dispose();
                MsmqQueue.ReceiveResult unknown = MsmqQueue.ReceiveResult.Unknown;
                Exception exception = null;
                try
                {
                    unknown = this.msmqQueue.EndPeek(result);
                }
                catch (Exception exception2)
                {
                    if (DiagnosticUtility.IsFatal(exception2))
                    {
                        throw;
                    }
                    exception = exception2;
                }
                if (exception == null)
                {
                    base.Complete(unknown != MsmqQueue.ReceiveResult.Timeout, result.CompletedSynchronously);
                }
                else
                {
                    base.Complete(result.CompletedSynchronously, exception);
                }
            }

            private static void OnCompleteStatic(IAsyncResult result)
            {
                ((MsmqReceiveHelper.WaitForMessageAsyncResult) result.AsyncState).OnComplete(result);
            }
        }
    }
}

