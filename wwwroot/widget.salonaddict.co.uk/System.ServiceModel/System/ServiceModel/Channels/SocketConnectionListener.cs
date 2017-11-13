namespace System.ServiceModel.Channels
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.ServiceModel;
    using System.Threading;

    internal class SocketConnectionListener : IConnectionListener, IDisposable
    {
        private bool isDisposed;
        private bool isListening;
        private Socket listenSocket;
        private IPEndPoint localEndpoint;
        private ISocketListenerSettings settings;
        private bool useOnlyOverlappedIO;

        private SocketConnectionListener(ISocketListenerSettings settings, bool useOnlyOverlappedIO)
        {
            this.settings = settings;
            this.useOnlyOverlappedIO = useOnlyOverlappedIO;
        }

        public SocketConnectionListener(IPEndPoint localEndpoint, ISocketListenerSettings settings, bool useOnlyOverlappedIO) : this(settings, useOnlyOverlappedIO)
        {
            this.localEndpoint = localEndpoint;
        }

        public SocketConnectionListener(Socket listenSocket, ISocketListenerSettings settings, bool useOnlyOverlappedIO) : this(settings, useOnlyOverlappedIO)
        {
            this.listenSocket = listenSocket;
        }

        public IAsyncResult BeginAccept(AsyncCallback callback, object state) => 
            new AcceptAsyncResult(this, callback, state);

        public static Exception ConvertListenException(SocketException socketException, IPEndPoint localEndpoint)
        {
            if (socketException.ErrorCode == 6)
            {
                return new CommunicationObjectAbortedException(socketException.Message, socketException);
            }
            if (socketException.ErrorCode == 0x2740)
            {
                return new AddressAlreadyInUseException(System.ServiceModel.SR.GetString("TcpAddressInUse", new object[] { localEndpoint.ToString() }), socketException);
            }
            return new CommunicationException(System.ServiceModel.SR.GetString("TcpListenError", new object[] { socketException.ErrorCode, socketException.Message, localEndpoint.ToString() }), socketException);
        }

        public void Dispose()
        {
            lock (this.ThisLock)
            {
                if (!this.isDisposed)
                {
                    if (this.listenSocket != null)
                    {
                        this.listenSocket.Close();
                    }
                    this.isDisposed = true;
                }
            }
        }

        public IConnection EndAccept(IAsyncResult result)
        {
            Socket socket = AcceptAsyncResult.End(result);
            if (socket == null)
            {
                return null;
            }
            if (this.useOnlyOverlappedIO)
            {
                socket.UseOnlyOverlappedIO = true;
            }
            return new SocketConnection(socket, this.settings.BufferSize, false);
        }

        private IAsyncResult InternalBeginAccept(AsyncCallback callback, object state)
        {
            lock (this.ThisLock)
            {
                if (this.isDisposed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(base.GetType().ToString(), System.ServiceModel.SR.GetString("SocketListenerDisposed")));
                }
                if (!this.isListening)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SocketListenerNotListening")));
                }
                return this.listenSocket.BeginAccept(callback, state);
            }
        }

        private Socket InternalEndAccept(IAsyncResult result)
        {
            Socket socket = null;
            lock (this.ThisLock)
            {
                if (!this.isDisposed)
                {
                    socket = this.listenSocket.EndAccept(result);
                }
            }
            return socket;
        }

        public void Listen()
        {
            BackoffTimeoutHelper helper = new BackoffTimeoutHelper(TimeSpan.FromSeconds(1.0));
            lock (this.ThisLock)
            {
                if (this.listenSocket != null)
                {
                    this.listenSocket.Listen(this.settings.ListenBacklog);
                    this.isListening = true;
                }
                while (!this.isListening)
                {
                    try
                    {
                        this.listenSocket = new Socket(this.localEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        if ((this.localEndpoint.AddressFamily == AddressFamily.InterNetworkV6) && this.settings.TeredoEnabled)
                        {
                            this.listenSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.HopLimit | SocketOptionName.AcceptConnection, 10);
                        }
                        this.listenSocket.Bind(this.localEndpoint);
                        this.listenSocket.Listen(this.settings.ListenBacklog);
                        this.isListening = true;
                        continue;
                    }
                    catch (SocketException exception)
                    {
                        bool flag = false;
                        if ((exception.ErrorCode == 0x2740) && !helper.IsExpired())
                        {
                            helper.WaitAndBackoff();
                            flag = true;
                        }
                        if (!flag)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertListenException(exception, this.localEndpoint));
                        }
                        continue;
                    }
                }
            }
        }

        private object ThisLock =>
            this;

        private class AcceptAsyncResult : AsyncResult
        {
            private SocketConnectionListener listener;
            private static AsyncCallback onAccept = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(SocketConnectionListener.AcceptAsyncResult.OnAccept));
            private Socket socket;
            private static WaitCallback startAccept;

            public AcceptAsyncResult(SocketConnectionListener listener, AsyncCallback callback, object state) : base(callback, state)
            {
                this.listener = listener;
                if (!Thread.CurrentThread.IsThreadPoolThread)
                {
                    if (startAccept == null)
                    {
                        startAccept = new WaitCallback(SocketConnectionListener.AcceptAsyncResult.StartAccept);
                    }
                    IOThreadScheduler.ScheduleCallback(startAccept, this);
                }
                else if (this.StartAccept())
                {
                    base.Complete(true);
                }
            }

            public static Socket End(IAsyncResult result) => 
                AsyncResult.End<SocketConnectionListener.AcceptAsyncResult>(result).socket;

            private static void OnAccept(IAsyncResult result)
            {
                if (!result.CompletedSynchronously)
                {
                    SocketConnectionListener.AcceptAsyncResult asyncState = (SocketConnectionListener.AcceptAsyncResult) result.AsyncState;
                    Exception exception = null;
                    bool flag = true;
                    try
                    {
                        bool flag2 = false;
                        try
                        {
                            asyncState.socket = asyncState.listener.InternalEndAccept(result);
                            flag = true;
                        }
                        catch (SocketException exception2)
                        {
                            if (ShouldAcceptRecover(exception2))
                            {
                                if (DiagnosticUtility.ShouldTraceWarning)
                                {
                                    DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Warning);
                                }
                                flag2 = true;
                            }
                            else
                            {
                                flag = true;
                                exception = exception2;
                            }
                        }
                        if (flag2)
                        {
                            flag = asyncState.StartAccept();
                        }
                    }
                    catch (Exception exception3)
                    {
                        if (DiagnosticUtility.IsFatal(exception3))
                        {
                            throw;
                        }
                        flag = true;
                        exception = exception3;
                    }
                    if (flag)
                    {
                        asyncState.Complete(false, exception);
                    }
                }
            }

            private static bool ShouldAcceptRecover(SocketException exception)
            {
                if (((exception.ErrorCode != 0x2746) && (exception.ErrorCode != 0x2728)) && (exception.ErrorCode != 0x2747))
                {
                    return (exception.ErrorCode == 0x274c);
                }
                return true;
            }

            private bool StartAccept()
            {
                bool flag;
            Label_0000:
                try
                {
                    IAsyncResult result = this.listener.InternalBeginAccept(onAccept, this);
                    if (!result.CompletedSynchronously)
                    {
                        return false;
                    }
                    this.socket = this.listener.InternalEndAccept(result);
                    flag = true;
                }
                catch (SocketException exception)
                {
                    if (!ShouldAcceptRecover(exception))
                    {
                        throw;
                    }
                    goto Label_0000;
                }
                return flag;
            }

            private static void StartAccept(object state)
            {
                bool flag;
                SocketConnectionListener.AcceptAsyncResult result = (SocketConnectionListener.AcceptAsyncResult) state;
                Exception exception = null;
                try
                {
                    flag = result.StartAccept();
                }
                catch (Exception exception2)
                {
                    if (DiagnosticUtility.IsFatal(exception2))
                    {
                        throw;
                    }
                    flag = true;
                    exception = exception2;
                }
                if (flag)
                {
                    result.Complete(false, exception);
                }
            }
        }
    }
}

