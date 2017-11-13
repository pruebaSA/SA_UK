namespace System.Net
{
    using System;
    using System.IO;

    internal sealed class SyncMemoryStream : MemoryStream
    {
        private int m_ReadTimeout;
        private int m_WriteTimeout;

        internal SyncMemoryStream(byte[] bytes) : base(bytes, false)
        {
            this.m_ReadTimeout = this.m_WriteTimeout = -1;
        }

        internal SyncMemoryStream(int initialCapacity) : base(initialCapacity)
        {
            this.m_ReadTimeout = this.m_WriteTimeout = -1;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => 
            new LazyAsyncResult(null, state, callback, this.Read(buffer, offset, count));

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            this.Write(buffer, offset, count);
            return new LazyAsyncResult(null, state, callback, null);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            LazyAsyncResult result = (LazyAsyncResult) asyncResult;
            return (int) result.InternalWaitForCompletion();
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            ((LazyAsyncResult) asyncResult).InternalWaitForCompletion();
        }

        public override bool CanTimeout =>
            true;

        public override int ReadTimeout
        {
            get => 
                this.m_ReadTimeout;
            set
            {
                this.m_ReadTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get => 
                this.m_WriteTimeout;
            set
            {
                this.m_WriteTimeout = value;
            }
        }
    }
}

