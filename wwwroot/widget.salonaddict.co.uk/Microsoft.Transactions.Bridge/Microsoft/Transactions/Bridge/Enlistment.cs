namespace Microsoft.Transactions.Bridge
{
    using System;

    internal sealed class Enlistment
    {
        private Guid enlistmentId;
        private Microsoft.Transactions.Bridge.EnlistmentOptions enlistmentOptions;
        private Guid localTransactionId;
        private Notifications notificationMask;
        private object protocolProviderContext;
        private byte[] recoveryData;
        private string remoteTransactionId;
        private object transactionManagerContext;

        public Enlistment()
        {
            this.localTransactionId = Guid.Empty;
            this.enlistmentId = Guid.NewGuid();
            this.remoteTransactionId = null;
            this.recoveryData = new byte[0];
            this.transactionManagerContext = null;
            this.protocolProviderContext = null;
            this.notificationMask = Notifications.AllProtocols;
        }

        public Enlistment(Guid enlistmentId)
        {
            this.localTransactionId = Guid.Empty;
            this.enlistmentId = enlistmentId;
            this.remoteTransactionId = null;
            this.recoveryData = new byte[0];
            this.transactionManagerContext = null;
            this.protocolProviderContext = null;
            this.notificationMask = Notifications.AllProtocols;
        }

        public byte[] GetRecoveryData() => 
            ((byte[]) this.recoveryData.Clone());

        public void SetRecoveryData(byte[] data)
        {
            if (data != null)
            {
                this.recoveryData = (byte[]) data.Clone();
            }
            else
            {
                this.recoveryData = new byte[0];
            }
        }

        public override string ToString() => 
            (base.GetType().ToString() + " enlistment ID = " + this.enlistmentId.ToString("B", null) + " transaction ID = " + this.localTransactionId.ToString("B", null));

        public Guid EnlistmentId =>
            this.enlistmentId;

        public Microsoft.Transactions.Bridge.EnlistmentOptions EnlistmentOptions
        {
            get => 
                this.enlistmentOptions;
            set
            {
                this.enlistmentOptions = value;
            }
        }

        public Guid LocalTransactionId
        {
            get => 
                this.localTransactionId;
            set
            {
                this.localTransactionId = value;
            }
        }

        public Notifications NotificationMask
        {
            get => 
                this.notificationMask;
            set
            {
                this.notificationMask = value;
            }
        }

        public object ProtocolProviderContext
        {
            get => 
                this.protocolProviderContext;
            set
            {
                this.protocolProviderContext = value;
            }
        }

        public string RemoteTransactionId
        {
            get => 
                this.remoteTransactionId;
            set
            {
                this.remoteTransactionId = value;
            }
        }

        public object TransactionManagerContext
        {
            get => 
                this.transactionManagerContext;
            set
            {
                this.transactionManagerContext = value;
            }
        }
    }
}

