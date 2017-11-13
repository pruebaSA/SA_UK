namespace System.Messaging
{
    using System;

    public class ReceiveCompletedEventArgs : EventArgs
    {
        private System.Messaging.Message message;
        private IAsyncResult result;
        private MessageQueue sender;

        internal ReceiveCompletedEventArgs(MessageQueue sender, IAsyncResult result)
        {
            this.result = result;
            this.sender = sender;
        }

        public IAsyncResult AsyncResult
        {
            get => 
                this.result;
            set
            {
                this.result = value;
            }
        }

        public System.Messaging.Message Message
        {
            get
            {
                if (this.message == null)
                {
                    try
                    {
                        this.message = this.sender.EndReceive(this.result);
                    }
                    catch
                    {
                        throw;
                    }
                }
                return this.message;
            }
        }
    }
}

