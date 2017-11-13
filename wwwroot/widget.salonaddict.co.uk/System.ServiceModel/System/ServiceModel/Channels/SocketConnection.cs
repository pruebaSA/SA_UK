namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Threading;

    internal class SocketConnection : IConnection, IDisposable
    {
        private bool aborted;
        private int asyncReadBufferSize;
        private WaitCallback asyncReadCallback;
        private Exception asyncReadException;
        private OverlappedContext asyncReadOverlapped;
        private bool asyncReadPending;
        private int asyncReadSize;
        private object asyncReadState;
        private bool asyncWritePending;
        private static int bytesTransferred;
        private CloseState closeState;
        private TimeoutHelper closeTimeoutHelper;
        private TraceEventType exceptionEventType;
        private bool isShutdown;
        private bool noDelay;
        private AsyncCallback onReceive;
        private static WaitCallback onReceiveTimeout;
        private static WaitCallback onSendTimeout;
        private static WaitCallback onWaitForFinComplete = new WaitCallback(SocketConnection.OnWaitForFinComplete);
        private byte[] readBuffer;
        private OverlappedIOCompleteCallback readCallback;
        private TimeSpan readFinTimeout;
        private TimeSpan receiveTimeout;
        private IOThreadTimer receiveTimer;
        private IPEndPoint remoteEndpoint;
        private TimeSpan sendTimeout;
        private IOThreadTimer sendTimer;
        private Socket socket;
        private static int socketFlags;
        private string timeoutErrorString;
        private TransferOperation timeoutErrorTransferOperation;

        public SocketConnection(Socket socket, int asyncReadBufferSize, bool autoBindToCompletionPort)
        {
            if (socket == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("socket");
            }
            this.closeState = CloseState.Open;
            this.exceptionEventType = TraceEventType.Error;
            this.socket = socket;
            this.socket.SendBufferSize = this.socket.ReceiveBufferSize = asyncReadBufferSize;
            this.sendTimeout = this.receiveTimeout = TimeSpan.MaxValue;
            this.asyncReadBufferSize = asyncReadBufferSize;
            this.onReceive = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(this.OnReceive));
            if (autoBindToCompletionPort)
            {
                this.socket.UseOnlyOverlappedIO = false;
            }
            this.TraceSocketInfo(socket, TraceCode.SocketConnectionCreate, null);
        }

        public void Abort()
        {
            this.Abort(null, TransferOperation.Undefined);
        }

        private void Abort(TraceEventType traceEventType)
        {
            this.Abort(traceEventType, null, TransferOperation.Undefined);
        }

        private void Abort(string timeoutErrorString, TransferOperation transferOperation)
        {
            TraceEventType warning = TraceEventType.Warning;
            if (this.ExceptionEventType == TraceEventType.Information)
            {
                warning = this.ExceptionEventType;
            }
            this.Abort(warning, timeoutErrorString, transferOperation);
        }

        private void Abort(TraceEventType traceEventType, string timeoutErrorString, TransferOperation transferOperation)
        {
            lock (this.ThisLock)
            {
                if (this.closeState == CloseState.Closed)
                {
                    return;
                }
                this.timeoutErrorString = timeoutErrorString;
                this.timeoutErrorTransferOperation = transferOperation;
                this.aborted = true;
                this.closeState = CloseState.Closed;
                if (!this.asyncReadPending && (this.asyncReadOverlapped != null))
                {
                    this.asyncReadOverlapped.FreeOrDefer();
                }
                if (this.asyncReadPending)
                {
                    this.CancelReceiveTimer();
                }
                if (this.asyncWritePending)
                {
                    this.CancelSendTimer();
                }
            }
            if (DiagnosticUtility.ShouldTrace(traceEventType))
            {
                TraceUtility.TraceEvent(traceEventType, TraceCode.SocketConnectionAbort, this);
            }
            this.socket.Close(0);
        }

        private void AbortRead()
        {
            lock (this.ThisLock)
            {
                if (this.asyncReadPending)
                {
                    if (this.closeState != CloseState.Closed)
                    {
                        this.asyncReadPending = false;
                        this.CancelReceiveTimer();
                    }
                    else if (this.asyncReadOverlapped != null)
                    {
                        this.asyncReadOverlapped.FreeOrDefer();
                    }
                }
            }
        }

        private unsafe void AsyncReadCallback(bool haveResult, int error, int bytesRead)
        {
            if (!haveResult)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            this.CancelReceiveTimer();
            if (error != 0)
            {
                if (error != 0x3e3)
                {
                    lock (this.ThisLock)
                    {
                        if ((this.closeState == CloseState.Closing) || (this.closeState == CloseState.Closed))
                        {
                            error = 0x3e3;
                        }
                        else
                        {
                            uint num;
                            UnsafeNativeMethods.WSAGetOverlappedResult(this.socket.Handle, this.asyncReadOverlapped.NativeOverlapped, out bytesRead, false, out num);
                            error = Marshal.GetLastWin32Error();
                        }
                    }
                }
                this.asyncReadException = this.ConvertReceiveException(new SocketException(error), TimeoutHelper.Infinite);
            }
            this.asyncReadSize = bytesRead;
            this.asyncReadOverlapped.FreeIfDeferred();
            this.FinishRead();
        }

        public virtual AsyncReadResult BeginRead(int offset, int size, TimeSpan timeout, WaitCallback callback, object state)
        {
            ConnectionUtilities.ValidateBufferBounds(this.AsyncReadBufferSize, offset, size);
            this.ThrowIfNotOpen();
            return this.BeginReadCore(offset, size, timeout, callback, state);
        }

        private unsafe AsyncReadResult BeginReadCore(int offset, int size, TimeSpan timeout, WaitCallback callback, object state)
        {
            bool flag = true;
            this.asyncReadState = state;
            this.asyncReadCallback = callback;
            this.asyncReadPending = true;
            this.SetReadTimeout(timeout, false, false);
            try
            {
                if (this.socket.UseOnlyOverlappedIO)
                {
                    try
                    {
                        IAsyncResult asyncResult = this.socket.BeginReceive(this.AsyncReadBuffer, offset, size, SocketFlags.None, this.onReceive, null);
                        if (!asyncResult.CompletedSynchronously)
                        {
                            flag = false;
                            return AsyncReadResult.Queued;
                        }
                        this.asyncReadSize = this.socket.EndReceive(asyncResult);
                        flag = false;
                        return AsyncReadResult.Completed;
                    }
                    catch (SocketException exception)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(this.ConvertReceiveException(exception, TimeoutHelper.Infinite), this.ExceptionEventType);
                    }
                }
                if (this.readCallback == null)
                {
                    try
                    {
                        this.socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, null, null);
                    }
                    catch (SocketException exception2)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(this.ConvertReceiveException(exception2, TimeoutHelper.Infinite), this.ExceptionEventType);
                    }
                    this.readCallback = new OverlappedIOCompleteCallback(this.AsyncReadCallback);
                }
                bool flag2 = true;
                try
                {
                    int num = 0;
                    int errorCode = 0;
                    lock (this.ThisLock)
                    {
                        UnsafeNativeMethods.WSABuffer buffer;
                        this.ThrowIfClosed();
                        buffer.length = Math.Min(this.AsyncReadBuffer.Length - offset, size);
                        this.asyncReadOverlapped.StartAsyncOperation(this.AsyncReadBuffer, this.readCallback, true);
                        buffer.buffer = (IntPtr) (this.asyncReadOverlapped.BufferPtr + offset);
                        num = UnsafeNativeMethods.WSARecv(this.socket.Handle, &buffer, 1, out bytesTransferred, ref socketFlags, this.asyncReadOverlapped.NativeOverlapped, IntPtr.Zero);
                        if (num == -1)
                        {
                            errorCode = Marshal.GetLastWin32Error();
                        }
                    }
                    if (((num == -1) && (errorCode != 0x3e5)) && (errorCode != 0xea))
                    {
                        flag2 = false;
                        SocketException socketException = new SocketException(errorCode);
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(this.ConvertReceiveException(socketException, TimeoutHelper.Infinite), this.ExceptionEventType);
                    }
                }
                finally
                {
                    if (!flag2)
                    {
                        this.asyncReadOverlapped.CancelAsyncOperation();
                    }
                }
                flag = false;
            }
            catch (ObjectDisposedException exception4)
            {
                Exception objA = this.ConvertObjectDisposedException(exception4, TransferOperation.Read);
                if (object.ReferenceEquals(objA, exception4))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(objA, this.ExceptionEventType);
            }
            finally
            {
                if (flag)
                {
                    this.AbortRead();
                }
            }
            return AsyncReadResult.Queued;
        }

        public IAsyncResult BeginWrite(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, AsyncCallback callback, object state)
        {
            IAsyncResult result2;
            ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);
            try
            {
                lock (this.ThisLock)
                {
                    this.SetImmediate(immediate);
                    this.SetWriteTimeout(timeout, false);
                    this.asyncWritePending = true;
                }
                result2 = this.socket.BeginSend(buffer, offset, size, SocketFlags.None, callback, state);
            }
            catch (SocketException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(this.ConvertSendException(exception, TimeoutHelper.Infinite), this.ExceptionEventType);
            }
            catch (ObjectDisposedException exception2)
            {
                Exception objA = this.ConvertObjectDisposedException(exception2, TransferOperation.Write);
                if (object.ReferenceEquals(objA, exception2))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(objA, this.ExceptionEventType);
            }
            return result2;
        }

        private void CancelReceiveTimer()
        {
            IOThreadTimer receiveTimer = this.receiveTimer;
            this.receiveTimer = null;
            if (receiveTimer != null)
            {
                receiveTimer.Cancel();
            }
        }

        private void CancelSendTimer()
        {
            if (this.sendTimer != null)
            {
                this.sendTimer.Cancel();
                this.sendTimer = null;
            }
        }

        public void Close(TimeSpan timeout, bool asyncAndLinger)
        {
            lock (this.ThisLock)
            {
                if ((this.closeState == CloseState.Closing) || (this.closeState == CloseState.Closed))
                {
                    return;
                }
                this.TraceSocketInfo(this.socket, TraceCode.SocketConnectionClose, timeout.ToString());
                this.closeState = CloseState.Closing;
            }
            this.closeTimeoutHelper = new TimeoutHelper(timeout);
            this.Shutdown(this.closeTimeoutHelper.RemainingTime());
            if (asyncAndLinger)
            {
                this.CloseAsyncAndLinger();
            }
            else
            {
                this.CloseSync();
            }
        }

        private void CloseAsyncAndLinger()
        {
            this.readFinTimeout = this.closeTimeoutHelper.RemainingTime();
            try
            {
                if (this.BeginReadCore(0, 1, this.readFinTimeout, onWaitForFinComplete, this) == AsyncReadResult.Queued)
                {
                    return;
                }
                if (this.EndRead() > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new CommunicationException(System.ServiceModel.SR.GetString("SocketCloseReadReceivedData", new object[] { this.socket.RemoteEndPoint })), this.ExceptionEventType);
                }
            }
            catch (TimeoutException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new TimeoutException(System.ServiceModel.SR.GetString("SocketCloseReadTimeout", new object[] { this.socket.RemoteEndPoint, this.readFinTimeout }), exception), this.ExceptionEventType);
            }
            this.ContinueClose(this.closeTimeoutHelper.RemainingTime());
        }

        private void CloseSync()
        {
            byte[] buffer = new byte[1];
            this.readFinTimeout = this.closeTimeoutHelper.RemainingTime();
            try
            {
                if (this.ReadCore(buffer, 0, 1, this.readFinTimeout, true) > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new CommunicationException(System.ServiceModel.SR.GetString("SocketCloseReadReceivedData", new object[] { this.socket.RemoteEndPoint })), this.ExceptionEventType);
                }
            }
            catch (TimeoutException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new TimeoutException(System.ServiceModel.SR.GetString("SocketCloseReadTimeout", new object[] { this.socket.RemoteEndPoint, this.readFinTimeout }), exception), this.ExceptionEventType);
            }
            this.ContinueClose(this.closeTimeoutHelper.RemainingTime());
        }

        public void ContinueClose(TimeSpan timeout)
        {
            if ((timeout <= TimeSpan.Zero) && DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.SocketConnectionAbortClose, this);
            }
            this.socket.Close(TimeoutHelper.ToMilliseconds(timeout));
            lock (this.ThisLock)
            {
                if ((!this.asyncReadPending && (this.closeState != CloseState.Closed)) && (this.asyncReadOverlapped != null))
                {
                    this.asyncReadOverlapped.FreeOrDefer();
                }
                this.closeState = CloseState.Closed;
            }
        }

        private Exception ConvertObjectDisposedException(ObjectDisposedException originalException, TransferOperation transferOperation)
        {
            if (this.timeoutErrorString != null)
            {
                return ConvertTimeoutErrorException(originalException, transferOperation, this.timeoutErrorString, this.timeoutErrorTransferOperation);
            }
            if (this.aborted)
            {
                return new CommunicationObjectAbortedException(System.ServiceModel.SR.GetString("SocketConnectionDisposed"), originalException);
            }
            return originalException;
        }

        private Exception ConvertReceiveException(SocketException socketException, TimeSpan remainingTime) => 
            ConvertTransferException(socketException, this.receiveTimeout, socketException, TransferOperation.Read, this.aborted, this.timeoutErrorString, this.timeoutErrorTransferOperation, remainingTime);

        private Exception ConvertSendException(SocketException socketException, TimeSpan remainingTime) => 
            ConvertTransferException(socketException, this.sendTimeout, socketException, TransferOperation.Write, this.aborted, this.timeoutErrorString, this.timeoutErrorTransferOperation, remainingTime);

        private static Exception ConvertTimeoutErrorException(Exception originalException, TransferOperation transferOperation, string timeoutErrorString, TransferOperation timeoutErrorTransferOperation)
        {
            if (transferOperation == timeoutErrorTransferOperation)
            {
                return new TimeoutException(timeoutErrorString, originalException);
            }
            return new CommunicationException(timeoutErrorString, originalException);
        }

        internal static Exception ConvertTransferException(SocketException socketException, TimeSpan timeout, Exception originalException) => 
            ConvertTransferException(socketException, timeout, originalException, TransferOperation.Undefined, false, null, TransferOperation.Undefined, TimeoutHelper.Infinite);

        private static Exception ConvertTransferException(SocketException socketException, TimeSpan timeout, Exception originalException, TransferOperation transferOperation, bool aborted, string timeoutErrorString, TransferOperation timeoutErrorTransferOperation, TimeSpan remainingTime)
        {
            if (socketException.ErrorCode == 6)
            {
                return new CommunicationObjectAbortedException(socketException.Message, socketException);
            }
            if (timeoutErrorString != null)
            {
                return ConvertTimeoutErrorException(originalException, transferOperation, timeoutErrorString, timeoutErrorTransferOperation);
            }
            if ((socketException.ErrorCode == 0x2745) && (remainingTime <= TimeSpan.Zero))
            {
                return new TimeoutException(System.ServiceModel.SR.GetString("TcpConnectionTimedOut", new object[] { timeout }), originalException);
            }
            if (((socketException.ErrorCode == 0x2744) || (socketException.ErrorCode == 0x2745)) || (socketException.ErrorCode == 0x2746))
            {
                if (aborted)
                {
                    return new CommunicationObjectAbortedException(System.ServiceModel.SR.GetString("TcpLocalConnectionAborted"), originalException);
                }
                return new CommunicationException(System.ServiceModel.SR.GetString("TcpConnectionResetError", new object[] { timeout }), originalException);
            }
            if (socketException.ErrorCode == 0x274c)
            {
                return new TimeoutException(System.ServiceModel.SR.GetString("TcpConnectionTimedOut", new object[] { timeout }), originalException);
            }
            if (aborted)
            {
                return new CommunicationObjectAbortedException(System.ServiceModel.SR.GetString("TcpTransferError", new object[] { socketException.ErrorCode, socketException.Message }), originalException);
            }
            return new CommunicationException(System.ServiceModel.SR.GetString("TcpTransferError", new object[] { socketException.ErrorCode, socketException.Message }), originalException);
        }

        public void Dispose()
        {
            this.Abort();
        }

        public object DuplicateAndClose(int targetProcessId)
        {
            object obj2 = this.socket.DuplicateAndClose(targetProcessId);
            this.Abort(TraceEventType.Information);
            return obj2;
        }

        public int EndRead()
        {
            if (this.asyncReadException != null)
            {
                this.AbortRead();
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(this.asyncReadException, this.ExceptionEventType);
            }
            lock (this.ThisLock)
            {
                if (!this.asyncReadPending)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                }
                this.asyncReadPending = false;
                if ((this.closeState == CloseState.Closed) && (this.asyncReadOverlapped != null))
                {
                    this.asyncReadOverlapped.FreeOrDefer();
                }
            }
            return this.asyncReadSize;
        }

        public void EndWrite(IAsyncResult result)
        {
            try
            {
                bool flag;
                this.CancelSendTimer();
                lock (this.ThisLock)
                {
                    this.asyncWritePending = false;
                    flag = this.closeState != CloseState.Closed;
                }
                if (flag)
                {
                    this.socket.EndSend(result);
                }
            }
            catch (SocketException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(this.ConvertSendException(exception, TimeoutHelper.Infinite), this.ExceptionEventType);
            }
            catch (ObjectDisposedException exception2)
            {
                Exception objA = this.ConvertObjectDisposedException(exception2, TransferOperation.Write);
                if (object.ReferenceEquals(objA, exception2))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(objA, this.ExceptionEventType);
            }
        }

        private void FinishRead()
        {
            WaitCallback asyncReadCallback = this.asyncReadCallback;
            object asyncReadState = this.asyncReadState;
            this.asyncReadState = null;
            this.asyncReadCallback = null;
            asyncReadCallback(asyncReadState);
        }

        public object GetCoreTransport() => 
            this.socket;

        private void OnReceive(IAsyncResult result)
        {
            this.CancelReceiveTimer();
            if (!result.CompletedSynchronously)
            {
                try
                {
                    this.asyncReadSize = this.socket.EndReceive(result);
                }
                catch (SocketException exception)
                {
                    this.asyncReadException = this.ConvertReceiveException(exception, TimeoutHelper.Infinite);
                }
                catch (ObjectDisposedException exception2)
                {
                    this.asyncReadException = this.ConvertObjectDisposedException(exception2, TransferOperation.Read);
                }
                catch (Exception exception3)
                {
                    if (DiagnosticUtility.IsFatal(exception3))
                    {
                        throw;
                    }
                    this.asyncReadException = exception3;
                }
                this.FinishRead();
            }
        }

        private static void OnReceiveTimeout(object state)
        {
            SocketConnection connection = (SocketConnection) state;
            connection.Abort(System.ServiceModel.SR.GetString("SocketAbortedReceiveTimedOut", new object[] { connection.receiveTimeout }), TransferOperation.Read);
        }

        private static void OnSendTimeout(object state)
        {
            SocketConnection connection = (SocketConnection) state;
            connection.Abort(TraceEventType.Warning, System.ServiceModel.SR.GetString("SocketAbortedSendTimedOut", new object[] { connection.sendTimeout }), TransferOperation.Write);
        }

        private static void OnWaitForFinComplete(object state)
        {
            SocketConnection connection = (SocketConnection) state;
            try
            {
                try
                {
                    if (connection.EndRead() > 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new CommunicationException(System.ServiceModel.SR.GetString("SocketCloseReadReceivedData", new object[] { connection.socket.RemoteEndPoint })), connection.ExceptionEventType);
                    }
                }
                catch (TimeoutException exception)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new TimeoutException(System.ServiceModel.SR.GetString("SocketCloseReadTimeout", new object[] { connection.socket.RemoteEndPoint, connection.readFinTimeout }), exception), connection.ExceptionEventType);
                }
                connection.ContinueClose(connection.closeTimeoutHelper.RemainingTime());
            }
            catch (Exception exception2)
            {
                if (DiagnosticUtility.IsFatal(exception2))
                {
                    throw;
                }
                if (DiagnosticUtility.ShouldTraceWarning)
                {
                    DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Warning);
                }
            }
        }

        public int Read(byte[] buffer, int offset, int size, TimeSpan timeout)
        {
            ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);
            this.ThrowIfNotOpen();
            return this.ReadCore(buffer, offset, size, timeout, false);
        }

        private int ReadCore(byte[] buffer, int offset, int size, TimeSpan timeout, bool closing)
        {
            int num = 0;
            TimeoutHelper helper = new TimeoutHelper(timeout);
            try
            {
                this.SetReadTimeout(helper.RemainingTime(), true, closing);
                num = this.socket.Receive(buffer, offset, size, SocketFlags.None);
            }
            catch (SocketException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(this.ConvertReceiveException(exception, helper.RemainingTime()), this.ExceptionEventType);
            }
            catch (ObjectDisposedException exception2)
            {
                Exception objA = this.ConvertObjectDisposedException(exception2, TransferOperation.Read);
                if (object.ReferenceEquals(objA, exception2))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(objA, this.ExceptionEventType);
            }
            return num;
        }

        protected void SetAsyncBytesRead(int bytesRead)
        {
            this.asyncReadSize = bytesRead;
        }

        private void SetImmediate(bool immediate)
        {
            if (immediate != this.noDelay)
            {
                lock (this.ThisLock)
                {
                    this.ThrowIfNotOpen();
                    this.socket.NoDelay = immediate;
                }
                this.noDelay = immediate;
            }
        }

        private void SetReadTimeout(TimeSpan timeout, bool synchronous, bool closing)
        {
            if (synchronous)
            {
                this.CancelReceiveTimer();
                if (timeout <= TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new TimeoutException(System.ServiceModel.SR.GetString("TcpConnectionTimedOut", new object[] { timeout })), this.ExceptionEventType);
                }
                if (this.UpdateTimeout(this.receiveTimeout, timeout))
                {
                    lock (this.ThisLock)
                    {
                        if (!closing || (this.closeState != CloseState.Closing))
                        {
                            this.ThrowIfNotOpen();
                        }
                        this.socket.ReceiveTimeout = TimeoutHelper.ToMilliseconds(timeout);
                    }
                    this.receiveTimeout = timeout;
                }
            }
            else
            {
                this.receiveTimeout = timeout;
                if (timeout == TimeSpan.MaxValue)
                {
                    this.CancelReceiveTimer();
                }
                else
                {
                    this.ReceiveTimer.Set(timeout);
                }
            }
        }

        private void SetWriteTimeout(TimeSpan timeout, bool synchronous)
        {
            if (synchronous)
            {
                this.CancelSendTimer();
                if (timeout <= TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new TimeoutException(System.ServiceModel.SR.GetString("TcpConnectionTimedOut", new object[] { timeout })), this.ExceptionEventType);
                }
                if (this.UpdateTimeout(this.sendTimeout, timeout))
                {
                    lock (this.ThisLock)
                    {
                        this.ThrowIfNotOpen();
                        this.socket.SendTimeout = TimeoutHelper.ToMilliseconds(timeout);
                    }
                    this.sendTimeout = timeout;
                }
            }
            else
            {
                this.sendTimeout = timeout;
                if (timeout == TimeSpan.MaxValue)
                {
                    this.CancelSendTimer();
                }
                else
                {
                    this.SendTimer.Set(timeout);
                }
            }
        }

        public void Shutdown(TimeSpan timeout)
        {
            lock (this.ThisLock)
            {
                if (this.isShutdown)
                {
                    return;
                }
                this.isShutdown = true;
            }
            try
            {
                this.socket.Shutdown(SocketShutdown.Send);
            }
            catch (SocketException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(this.ConvertSendException(exception, TimeoutHelper.Infinite), this.ExceptionEventType);
            }
            catch (ObjectDisposedException exception2)
            {
                Exception objA = this.ConvertObjectDisposedException(exception2, TransferOperation.Undefined);
                if (object.ReferenceEquals(objA, exception2))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(objA, this.ExceptionEventType);
            }
        }

        private void ThrowIfClosed()
        {
            if (this.closeState == CloseState.Closed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(this.ConvertObjectDisposedException(new ObjectDisposedException(base.GetType().ToString(), System.ServiceModel.SR.GetString("SocketConnectionDisposed")), TransferOperation.Undefined), this.ExceptionEventType);
            }
        }

        private void ThrowIfNotOpen()
        {
            if ((this.closeState == CloseState.Closing) || (this.closeState == CloseState.Closed))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(this.ConvertObjectDisposedException(new ObjectDisposedException(base.GetType().ToString(), System.ServiceModel.SR.GetString("SocketConnectionDisposed")), TransferOperation.Undefined), this.ExceptionEventType);
            }
        }

        private void TraceSocketInfo(Socket socket, TraceCode traceCode, string timeoutString)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>(4) {
                    ["State"] = this.closeState.ToString()
                };
                if (timeoutString != null)
                {
                    dictionary["Timeout"] = timeoutString;
                }
                if ((socket != null) && (this.closeState != CloseState.Closing))
                {
                    if (socket.LocalEndPoint != null)
                    {
                        dictionary["LocalEndpoint"] = socket.LocalEndPoint.ToString();
                    }
                    if (socket.RemoteEndPoint != null)
                    {
                        dictionary["RemoteEndPoint"] = socket.RemoteEndPoint.ToString();
                    }
                }
                TraceUtility.TraceEvent(TraceEventType.Information, traceCode, new DictionaryTraceRecord(dictionary), this, null);
            }
        }

        private bool UpdateTimeout(TimeSpan oldTimeout, TimeSpan newTimeout)
        {
            if (oldTimeout == newTimeout)
            {
                return false;
            }
            long num = oldTimeout.Ticks / 10L;
            long num2 = Math.Max(oldTimeout.Ticks, newTimeout.Ticks) - Math.Min(oldTimeout.Ticks, newTimeout.Ticks);
            return (num2 > num);
        }

        public bool Validate(Uri uri) => 
            true;

        public void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout)
        {
            ConnectionUtilities.ValidateBufferBounds(buffer, offset, size);
            TimeoutHelper helper = new TimeoutHelper(timeout);
            try
            {
                this.SetImmediate(immediate);
                int num = size;
                while (num > 0)
                {
                    this.SetWriteTimeout(helper.RemainingTime(), true);
                    size = Math.Min(num, 0x10000);
                    this.socket.Send(buffer, offset, size, SocketFlags.None);
                    num -= size;
                    offset += size;
                    timeout = helper.RemainingTime();
                }
            }
            catch (SocketException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(this.ConvertSendException(exception, helper.RemainingTime()), this.ExceptionEventType);
            }
            catch (ObjectDisposedException exception2)
            {
                Exception objA = this.ConvertObjectDisposedException(exception2, TransferOperation.Write);
                if (object.ReferenceEquals(objA, exception2))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(objA, this.ExceptionEventType);
            }
        }

        public void Write(byte[] buffer, int offset, int size, bool immediate, TimeSpan timeout, BufferManager bufferManager)
        {
            try
            {
                this.Write(buffer, offset, size, immediate, timeout);
            }
            finally
            {
                bufferManager.ReturnBuffer(buffer);
            }
        }

        public byte[] AsyncReadBuffer
        {
            get
            {
                if (this.readBuffer == null)
                {
                    lock (this.ThisLock)
                    {
                        this.ThrowIfClosed();
                        if (this.readBuffer == null)
                        {
                            this.asyncReadOverlapped = new OverlappedContext();
                            this.readBuffer = DiagnosticUtility.Utility.AllocateByteArray(this.asyncReadBufferSize);
                        }
                    }
                }
                return this.readBuffer;
            }
        }

        public int AsyncReadBufferSize =>
            this.asyncReadBufferSize;

        public TraceEventType ExceptionEventType
        {
            get => 
                this.exceptionEventType;
            set
            {
                this.exceptionEventType = value;
            }
        }

        private IOThreadTimer ReceiveTimer
        {
            get
            {
                if (this.receiveTimer == null)
                {
                    if (onReceiveTimeout == null)
                    {
                        onReceiveTimeout = new WaitCallback(SocketConnection.OnReceiveTimeout);
                    }
                    this.receiveTimer = new IOThreadTimer(onReceiveTimeout, this, false);
                }
                return this.receiveTimer;
            }
        }

        public IPEndPoint RemoteIPEndPoint
        {
            get
            {
                if ((this.remoteEndpoint == null) && (this.closeState == CloseState.Open))
                {
                    try
                    {
                        this.remoteEndpoint = (IPEndPoint) this.socket.RemoteEndPoint;
                    }
                    catch (SocketException exception)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(this.ConvertReceiveException(exception, TimeSpan.Zero), this.ExceptionEventType);
                    }
                    catch (ObjectDisposedException exception2)
                    {
                        Exception objA = this.ConvertObjectDisposedException(exception2, TransferOperation.Undefined);
                        if (object.ReferenceEquals(objA, exception2))
                        {
                            throw;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(objA, this.ExceptionEventType);
                    }
                }
                return this.remoteEndpoint;
            }
        }

        private IOThreadTimer SendTimer
        {
            get
            {
                if (this.sendTimer == null)
                {
                    if (onSendTimeout == null)
                    {
                        onSendTimeout = new WaitCallback(SocketConnection.OnSendTimeout);
                    }
                    this.sendTimer = new IOThreadTimer(onSendTimeout, this, false);
                }
                return this.sendTimer;
            }
        }

        private object ThisLock =>
            this;

        private enum CloseState
        {
            Open,
            Closing,
            Closed
        }

        private enum TransferOperation
        {
            Write,
            Read,
            Undefined
        }
    }
}

