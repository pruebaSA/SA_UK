namespace System.ServiceModel.Channels
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.Transactions;

    internal sealed class MsmqInputSessionChannel : InputChannel, IInputSessionChannel, IInputChannel, IChannel, ICommunicationObject, ISessionChannel<IInputSession>
    {
        private Transaction associatedTx;
        private IInputSession session;

        public MsmqInputSessionChannel(MsmqInputSessionChannelListener listener, Transaction associatedTx) : base(listener, new EndpointAddress(listener.Uri, new AddressHeader[0]))
        {
            this.session = new InputSession();
            this.associatedTx = associatedTx;
            this.associatedTx.EnlistVolatile(new TransactionEnlistment(this, this.associatedTx), EnlistmentOptions.None);
        }

        public override IAsyncResult BeginReceive(AsyncCallback callback, object state) => 
            this.BeginReceive(base.DefaultReceiveTimeout, callback, state);

        public override IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state) => 
            InputChannel.HelpBeginReceive(this, timeout, callback, state);

        public override IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            base.ThrowIfFaulted();
            if ((CommunicationState.Closed == base.State) || (CommunicationState.Closing == base.State))
            {
                return new TypedCompletedAsyncResult<bool, Message>(true, null, callback, state);
            }
            this.VerifyTransaction();
            return base.BeginTryReceive(timeout, callback, state);
        }

        public override bool EndTryReceive(IAsyncResult result, out Message message)
        {
            if (result is TypedCompletedAsyncResult<bool, Message>)
            {
                return TypedCompletedAsyncResult<bool, Message>.End(result, out message);
            }
            return base.EndTryReceive(result, out message);
        }

        public void FaultChannel()
        {
            base.Fault();
        }

        protected override void OnAbort()
        {
            this.OnCloseCore(true);
            base.OnAbort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            this.OnCloseCore(false);
            return base.OnBeginClose(timeout, callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            this.OnCloseCore(false);
            base.OnClose(timeout);
        }

        private void OnCloseCore(bool isAborting)
        {
            if (isAborting)
            {
                this.RollbackTransaction();
            }
            else
            {
                this.VerifyTransaction();
                if (base.InternalPendingItems > 0)
                {
                    this.RollbackTransaction();
                    base.Fault();
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("MsmqSessionMessagesNotConsumed")));
                }
            }
        }

        public override Message Receive() => 
            this.Receive(base.DefaultReceiveTimeout);

        public override Message Receive(TimeSpan timeout) => 
            InputChannel.HelpReceive(this, timeout);

        private void RollbackTransaction()
        {
            try
            {
                if (this.associatedTx.TransactionInformation.Status == TransactionStatus.Active)
                {
                    this.associatedTx.Rollback();
                }
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

        public override bool TryReceive(TimeSpan timeout, out Message message)
        {
            base.ThrowIfFaulted();
            if ((CommunicationState.Closed == base.State) || (CommunicationState.Closing == base.State))
            {
                message = null;
                return true;
            }
            this.VerifyTransaction();
            return base.TryReceive(timeout, out message);
        }

        private void VerifyTransaction()
        {
            if (base.InternalPendingItems > 0)
            {
                if (this.associatedTx != Transaction.Current)
                {
                    this.RollbackTransaction();
                    base.Fault();
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCritical(new InvalidOperationException(System.ServiceModel.SR.GetString("MsmqSameTransactionExpected")));
                }
                if (Transaction.Current.TransactionInformation.Status != TransactionStatus.Active)
                {
                    this.RollbackTransaction();
                    base.Fault();
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCritical(new InvalidOperationException(System.ServiceModel.SR.GetString("MsmqTransactionNotActive")));
                }
            }
        }

        public IInputSession Session =>
            this.session;

        private class InputSession : IInputSession, ISession
        {
            private string id = ("uuid://session-gram/" + Guid.NewGuid().ToString());

            public string Id =>
                this.id;
        }

        private class TransactionEnlistment : IEnlistmentNotification
        {
            private MsmqInputSessionChannel channel;
            private Transaction transaction;

            public TransactionEnlistment(MsmqInputSessionChannel channel, Transaction transaction)
            {
                this.channel = channel;
                this.transaction = transaction;
            }

            public void Commit(Enlistment enlistment)
            {
                enlistment.Done();
            }

            public void InDoubt(Enlistment enlistment)
            {
                enlistment.Done();
            }

            public void Prepare(PreparingEnlistment preparingEnlistment)
            {
                if ((this.channel.State == CommunicationState.Opened) && (this.channel.InternalPendingItems > 0))
                {
                    Exception e = DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("MsmqSessionChannelsMustBeClosed")));
                    preparingEnlistment.ForceRollback(e);
                    this.channel.Fault();
                }
                else
                {
                    preparingEnlistment.Done();
                }
            }

            public void Rollback(Enlistment enlistment)
            {
                this.channel.Fault();
                enlistment.Done();
            }
        }
    }
}

