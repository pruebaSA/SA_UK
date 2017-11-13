namespace System.ServiceModel.Channels
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;

    internal abstract class DelegatingConnection : IConnection
    {
        private IConnection connection;

        protected DelegatingConnection(IConnection connection)
        {
            this.connection = connection;
        }

        public virtual void Abort()
        {
            this.connection.Abort();
        }

        public virtual AsyncReadResult BeginRead(int offset, int size, TimeSpan timeout, WaitCallback callback, object state) => 
            this.connection.BeginRead(offset, size, timeout, callback, state);

        public virtual IAsyncResult BeginWrite(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, AsyncCallback callback, object state) => 
            this.connection.BeginWrite(buffer, offset, size, immediate, timeout, callback, state);

        public virtual void Close(TimeSpan timeout, bool asyncAndLinger)
        {
            this.connection.Close(timeout, asyncAndLinger);
        }

        public virtual object DuplicateAndClose(int targetProcessId) => 
            this.connection.DuplicateAndClose(targetProcessId);

        public virtual int EndRead() => 
            this.connection.EndRead();

        public virtual void EndWrite(IAsyncResult result)
        {
            this.connection.EndWrite(result);
        }

        public virtual object GetCoreTransport() => 
            this.connection.GetCoreTransport();

        public virtual int Read(byte[] buffer, int offset, int size, TimeSpan timeout) => 
            this.connection.Read(buffer, offset, size, timeout);

        public virtual void Shutdown(TimeSpan timeout)
        {
            this.connection.Shutdown(timeout);
        }

        public virtual bool Validate(Uri uri) => 
            this.connection.Validate(uri);

        public virtual void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout)
        {
            this.connection.Write(buffer, offset, size, immediate, timeout);
        }

        public virtual void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, BufferManager bufferManager)
        {
            this.connection.Write(buffer, offset, size, immediate, timeout, bufferManager);
        }

        public virtual byte[] AsyncReadBuffer =>
            this.connection.AsyncReadBuffer;

        public virtual int AsyncReadBufferSize =>
            this.connection.AsyncReadBufferSize;

        protected IConnection Connection =>
            this.connection;

        public TraceEventType ExceptionEventType
        {
            get => 
                this.connection.ExceptionEventType;
            set
            {
                this.connection.ExceptionEventType = value;
            }
        }

        public IPEndPoint RemoteIPEndPoint =>
            this.connection.RemoteIPEndPoint;
    }
}

