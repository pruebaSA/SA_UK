namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    internal sealed class MsmqInputSessionChannelListener : MsmqChannelListenerBase<IInputSessionChannel>
    {
        private System.ServiceModel.Channels.MsmqReceiveHelper receiver;

        internal MsmqInputSessionChannelListener(MsmqBindingElementBase bindingElement, BindingContext context, MsmqReceiveParameters receiveParameters) : base(bindingElement, context, receiveParameters, TransportDefaults.GetDefaultMessageEncoderFactory())
        {
            base.SetSecurityTokenAuthenticator(MsmqUri.NetMsmqAddressTranslator.Scheme, context);
            this.receiver = new System.ServiceModel.Channels.MsmqReceiveHelper(base.ReceiveParameters, this.Uri, new MsmqInputMessagePool((base.ReceiveParameters as MsmqTransportReceiveParameters).MaxPoolSize), null, this);
        }

        public override IInputSessionChannel AcceptChannel() => 
            this.AcceptChannel(this.DefaultReceiveTimeout);

        public override IInputSessionChannel AcceptChannel(TimeSpan timeout)
        {
            IInputSessionChannel channel;
            if (base.DoneReceivingInCurrentState())
            {
                return null;
            }
            MsmqInputMessage msmqMessage = this.receiver.TakeMessage();
            try
            {
                MsmqMessageProperty property;
                if (!this.receiver.TryReceive(msmqMessage, timeout, MsmqTransactionMode.CurrentOrThrow, out property))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException());
                }
                if (property != null)
                {
                    return MsmqDecodeHelper.DecodeTransportSessiongram(this, msmqMessage, property);
                }
                if (CommunicationState.Opened == base.State)
                {
                    base.Fault();
                }
                channel = null;
            }
            catch (MsmqException exception)
            {
                if (exception.FaultReceiver)
                {
                    base.Fault();
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception.Normalized);
            }
            finally
            {
                this.receiver.ReturnMessage(msmqMessage);
            }
            return channel;
        }

        public override IAsyncResult BeginAcceptChannel(AsyncCallback callback, object state) => 
            this.BeginAcceptChannel(this.DefaultReceiveTimeout, callback, state);

        public override IAsyncResult BeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (base.DoneReceivingInCurrentState())
            {
                return new DoneReceivingAsyncResult(callback, state);
            }
            MsmqInputMessage msmqMessage = this.receiver.TakeMessage();
            return this.receiver.BeginTryReceive(msmqMessage, timeout, MsmqTransactionMode.CurrentOrThrow, callback, state);
        }

        public override IInputSessionChannel EndAcceptChannel(IAsyncResult result)
        {
            IInputSessionChannel channel;
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
            }
            DoneReceivingAsyncResult result2 = result as DoneReceivingAsyncResult;
            if (result2 != null)
            {
                DoneReceivingAsyncResult.End(result2);
                return null;
            }
            MsmqInputMessage msmqMessage = null;
            MsmqMessageProperty msmqProperty = null;
            try
            {
                if (!this.receiver.EndTryReceive(result, out msmqMessage, out msmqProperty))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException());
                }
                if (msmqProperty != null)
                {
                    return MsmqDecodeHelper.DecodeTransportSessiongram(this, msmqMessage, msmqProperty);
                }
                if (CommunicationState.Opened == base.State)
                {
                    base.Fault();
                }
                channel = null;
            }
            catch (MsmqException exception)
            {
                if (exception.FaultReceiver)
                {
                    base.Fault();
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception.Normalized);
            }
            finally
            {
                if (msmqMessage != null)
                {
                    this.receiver.ReturnMessage(msmqMessage);
                }
            }
            return channel;
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (base.DoneReceivingInCurrentState())
            {
                return new DoneAsyncResult(true, callback, state);
            }
            return this.receiver.BeginWaitForMessage(timeout, callback, state);
        }

        protected override void OnCloseCore(bool aborting)
        {
            if (this.receiver != null)
            {
                this.receiver.Close();
            }
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            bool flag;
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
            }
            if (result is DoneAsyncResult)
            {
                return TypedCompletedAsyncResult<bool>.End(result);
            }
            try
            {
                flag = this.receiver.EndWaitForMessage(result);
            }
            catch (MsmqException exception)
            {
                if (exception.FaultReceiver)
                {
                    base.Fault();
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception.Normalized);
            }
            return flag;
        }

        protected override void OnOpenCore(TimeSpan timeout)
        {
            base.OnOpenCore(timeout);
            try
            {
                this.receiver.Open();
            }
            catch (MsmqException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception.Normalized);
            }
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            bool flag;
            if (base.DoneReceivingInCurrentState())
            {
                return true;
            }
            try
            {
                flag = this.receiver.WaitForMessage(timeout);
            }
            catch (MsmqException exception)
            {
                if (exception.FaultReceiver)
                {
                    base.Fault();
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception.Normalized);
            }
            return flag;
        }

        internal System.ServiceModel.Channels.MsmqReceiveHelper MsmqReceiveHelper =>
            this.receiver;

        private class DoneAsyncResult : TypedCompletedAsyncResult<bool>
        {
            internal DoneAsyncResult(bool data, AsyncCallback callback, object state) : base(data, callback, state)
            {
            }
        }
    }
}

