namespace System.Net.NetworkInformation
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Threading;

    public class Ping : Component, IDisposable
    {
        private ManualResetEvent asyncFinished;
        private AsyncOperation asyncOp;
        private bool cancelled;
        private byte[] defaultSendBuffer;
        private const int DefaultSendBufferSize = 0x20;
        private const int DefaultTimeout = 0x1388;
        private const int Disposed = 2;
        private bool disposeRequested;
        private byte[] downlevelReplyBuffer;
        private const int Free = 0;
        private SafeCloseIcmpHandle handlePingV4;
        private SafeCloseIcmpHandle handlePingV6;
        private const int InProgress = 1;
        private bool ipv6;
        private int llTimeout;
        private const int MaxBufferSize = 0xffdc;
        private const int MaxUdpPacket = 0x100ff;
        private SendOrPostCallback onPingCompletedDelegate;
        private IcmpPacket packet;
        private const int PacketTooBigErrorCode = 0x2738;
        internal ManualResetEvent pingEvent;
        private Socket pingSocket;
        private RegisteredWaitHandle registeredWait;
        private SafeLocalFree replyBuffer;
        private SafeLocalFree requestBuffer;
        private int sendSize;
        private int startTime;
        private int status;
        private const int TimeoutErrorCode = 0x274c;

        public event PingCompletedEventHandler PingCompleted;

        public Ping()
        {
            this.onPingCompletedDelegate = new SendOrPostCallback(this.PingCompletedWaitCallback);
        }

        private void CheckStart(bool async)
        {
            if (this.disposeRequested)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
            switch (Interlocked.CompareExchange(ref this.status, 1, 0))
            {
                case 1:
                    throw new InvalidOperationException(SR.GetString("net_inasync"));

                case 2:
                    throw new ObjectDisposedException(base.GetType().FullName);
            }
            if (async)
            {
                this.InAsyncCall = true;
            }
        }

        private void ContinueAsyncSend(object state)
        {
            AsyncStateObject obj2 = (AsyncStateObject) state;
            try
            {
                IPAddress address = Dns.GetHostAddresses(obj2.hostName)[0];
                new NetworkInformationPermission(NetworkInformationAccess.Ping).Demand();
                this.InternalSend(address, obj2.buffer, obj2.timeout, obj2.options, true);
            }
            catch (Exception exception)
            {
                PingException error = new PingException(SR.GetString("net_ping"), exception);
                PingCompletedEventArgs arg = new PingCompletedEventArgs(null, error, false, this.asyncOp.UserSuppliedState);
                this.Finish(true);
                this.asyncOp.PostOperationCompleted(this.onPingCompletedDelegate, arg);
            }
            catch
            {
                PingException exception3 = new PingException(SR.GetString("net_ping"), new Exception(SR.GetString("net_nonClsCompliantException")));
                PingCompletedEventArgs args2 = new PingCompletedEventArgs(null, exception3, false, this.asyncOp.UserSuppliedState);
                this.Finish(true);
                this.asyncOp.PostOperationCompleted(this.onPingCompletedDelegate, args2);
            }
        }

        internal static bool CorrectPacket(byte[] buffer, IcmpPacket packet)
        {
            if ((buffer[20] == 0) && (buffer[0x15] == 0))
            {
                if ((((buffer[0x19] << 8) | buffer[0x18]) == packet.Identifier) && (((buffer[0x1b] << 8) | buffer[0x1a]) == packet.sequenceNumber))
                {
                    return true;
                }
            }
            else if ((((buffer[0x35] << 8) | buffer[0x34]) == packet.Identifier) && (((buffer[0x37] << 8) | buffer[0x36]) == packet.sequenceNumber))
            {
                return true;
            }
            return false;
        }

        private void Finish(bool async)
        {
            this.status = 0;
            if (async)
            {
                this.InAsyncCall = false;
            }
            if (this.disposeRequested)
            {
                this.InternalDispose();
            }
        }

        private void FreeUnmanagedStructures()
        {
            if (this.requestBuffer != null)
            {
                this.requestBuffer.Close();
                this.requestBuffer = null;
            }
        }

        private void InternalDispose()
        {
            this.disposeRequested = true;
            if (Interlocked.CompareExchange(ref this.status, 2, 0) == 0)
            {
                if (this.pingSocket != null)
                {
                    this.pingSocket.Close();
                    this.pingSocket = null;
                }
                if (this.handlePingV4 != null)
                {
                    this.handlePingV4.Close();
                    this.handlePingV4 = null;
                }
                if (this.handlePingV6 != null)
                {
                    this.handlePingV6.Close();
                    this.handlePingV6 = null;
                }
                if (this.registeredWait != null)
                {
                    this.registeredWait.Unregister(null);
                }
                if (this.pingEvent != null)
                {
                    this.pingEvent.Close();
                }
                if (this.replyBuffer != null)
                {
                    this.replyBuffer.Close();
                }
            }
        }

        private PingReply InternalDownLevelSend(IPAddress address, byte[] buffer, int timeout, PingOptions options, bool async)
        {
            try
            {
                if (options == null)
                {
                    options = new PingOptions();
                }
                if (this.downlevelReplyBuffer == null)
                {
                    this.downlevelReplyBuffer = new byte[0xfa00];
                }
                this.llTimeout = timeout;
                this.packet = new IcmpPacket(buffer);
                byte[] bytes = this.packet.GetBytes();
                IPEndPoint remoteEP = new IPEndPoint(address, 0);
                EndPoint point2 = new IPEndPoint(IPAddress.Any, 0);
                if (this.pingSocket == null)
                {
                    this.pingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
                }
                this.pingSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
                this.pingSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);
                this.pingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, options.Ttl);
                this.pingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DontFragment, options.DontFragment);
                int dataLength = 0;
                int time = 0;
                this.startTime = Environment.TickCount;
                if (async)
                {
                    this.pingSocket.BeginSendTo(bytes, 0, bytes.Length, SocketFlags.None, remoteEP, new AsyncCallback(Ping.PingSendCallback), this);
                    return null;
                }
                this.pingSocket.SendTo(bytes, bytes.Length, SocketFlags.None, remoteEP);
                do
                {
                    dataLength = this.pingSocket.ReceiveFrom(this.downlevelReplyBuffer, ref point2);
                    if (CorrectPacket(this.downlevelReplyBuffer, this.packet))
                    {
                        goto Label_015B;
                    }
                }
                while ((Environment.TickCount - this.startTime) <= this.llTimeout);
                return new PingReply(IPStatus.TimedOut);
            Label_015B:
                time = Environment.TickCount - this.startTime;
                return new PingReply(this.downlevelReplyBuffer, dataLength, ((IPEndPoint) point2).Address, time);
            }
            catch (SocketException exception)
            {
                if (exception.ErrorCode == 0x274c)
                {
                    return new PingReply(IPStatus.TimedOut);
                }
                if (exception.ErrorCode != 0x2738)
                {
                    throw exception;
                }
                PingReply reply = new PingReply(IPStatus.PacketTooBig);
                if (!async)
                {
                    return reply;
                }
                PingCompletedEventArgs arg = new PingCompletedEventArgs(reply, null, false, this.asyncOp.UserSuppliedState);
                this.asyncOp.PostOperationCompleted(this.onPingCompletedDelegate, arg);
                return null;
            }
        }

        private PingReply InternalSend(IPAddress address, byte[] buffer, int timeout, PingOptions options, bool async)
        {
            int num;
            PingReply reply;
            this.cancelled = false;
            if ((address.AddressFamily == AddressFamily.InterNetworkV6) && !ComNetOS.IsPostWin2K)
            {
                throw new PlatformNotSupportedException(SR.GetString("WinXPRequired"));
            }
            if (!ComNetOS.IsWin2K)
            {
                return this.InternalDownLevelSend(address, buffer, timeout, options, async);
            }
            this.ipv6 = address.AddressFamily == AddressFamily.InterNetworkV6;
            this.sendSize = buffer.Length;
            if (!this.ipv6 && (this.handlePingV4 == null))
            {
                if (ComNetOS.IsPostWin2K)
                {
                    this.handlePingV4 = UnsafeNetInfoNativeMethods.IcmpCreateFile();
                }
                else
                {
                    this.handlePingV4 = UnsafeIcmpNativeMethods.IcmpCreateFile();
                }
            }
            else if (this.ipv6 && (this.handlePingV6 == null))
            {
                this.handlePingV6 = UnsafeNetInfoNativeMethods.Icmp6CreateFile();
            }
            IPOptions options2 = new IPOptions(options);
            if (this.replyBuffer == null)
            {
                this.replyBuffer = SafeLocalFree.LocalAlloc(0x100ff);
            }
            if (this.registeredWait != null)
            {
                this.registeredWait.Unregister(null);
                this.registeredWait = null;
            }
            try
            {
                if (async)
                {
                    if (this.pingEvent == null)
                    {
                        this.pingEvent = new ManualResetEvent(false);
                    }
                    else
                    {
                        this.pingEvent.Reset();
                    }
                    this.registeredWait = ThreadPool.RegisterWaitForSingleObject(this.pingEvent, new WaitOrTimerCallback(Ping.PingCallback), this, -1, true);
                }
                this.SetUnmanagedStructures(buffer);
                if (!this.ipv6)
                {
                    if (ComNetOS.IsPostWin2K)
                    {
                        if (async)
                        {
                            num = (int) UnsafeNetInfoNativeMethods.IcmpSendEcho2(this.handlePingV4, this.pingEvent.SafeWaitHandle, IntPtr.Zero, IntPtr.Zero, (uint) address.m_Address, this.requestBuffer, (ushort) buffer.Length, ref options2, this.replyBuffer, 0x100ff, (uint) timeout);
                        }
                        else
                        {
                            num = (int) UnsafeNetInfoNativeMethods.IcmpSendEcho2(this.handlePingV4, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, (uint) address.m_Address, this.requestBuffer, (ushort) buffer.Length, ref options2, this.replyBuffer, 0x100ff, (uint) timeout);
                        }
                    }
                    else if (async)
                    {
                        num = (int) UnsafeIcmpNativeMethods.IcmpSendEcho2(this.handlePingV4, this.pingEvent.SafeWaitHandle, IntPtr.Zero, IntPtr.Zero, (uint) address.m_Address, this.requestBuffer, (ushort) buffer.Length, ref options2, this.replyBuffer, 0x100ff, (uint) timeout);
                    }
                    else
                    {
                        num = (int) UnsafeIcmpNativeMethods.IcmpSendEcho2(this.handlePingV4, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, (uint) address.m_Address, this.requestBuffer, (ushort) buffer.Length, ref options2, this.replyBuffer, 0x100ff, (uint) timeout);
                    }
                }
                else
                {
                    SocketAddress address2 = new IPEndPoint(address, 0).Serialize();
                    byte[] sourceSocketAddress = new byte[0x1c];
                    if (async)
                    {
                        num = (int) UnsafeNetInfoNativeMethods.Icmp6SendEcho2(this.handlePingV6, this.pingEvent.SafeWaitHandle, IntPtr.Zero, IntPtr.Zero, sourceSocketAddress, address2.m_Buffer, this.requestBuffer, (ushort) buffer.Length, ref options2, this.replyBuffer, 0x100ff, (uint) timeout);
                    }
                    else
                    {
                        num = (int) UnsafeNetInfoNativeMethods.Icmp6SendEcho2(this.handlePingV6, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, sourceSocketAddress, address2.m_Buffer, this.requestBuffer, (ushort) buffer.Length, ref options2, this.replyBuffer, 0x100ff, (uint) timeout);
                    }
                }
            }
            catch
            {
                if (this.registeredWait != null)
                {
                    this.registeredWait.Unregister(null);
                }
                throw;
            }
            if (num == 0)
            {
                num = Marshal.GetLastWin32Error();
                if (num != 0)
                {
                    this.FreeUnmanagedStructures();
                    return new PingReply((IPStatus) num);
                }
            }
            if (async)
            {
                return null;
            }
            this.FreeUnmanagedStructures();
            if (this.ipv6)
            {
                Icmp6EchoReply reply2 = (Icmp6EchoReply) Marshal.PtrToStructure(this.replyBuffer.DangerousGetHandle(), typeof(Icmp6EchoReply));
                reply = new PingReply(reply2, this.replyBuffer.DangerousGetHandle(), this.sendSize);
            }
            else
            {
                IcmpEchoReply reply3 = (IcmpEchoReply) Marshal.PtrToStructure(this.replyBuffer.DangerousGetHandle(), typeof(IcmpEchoReply));
                reply = new PingReply(reply3);
            }
            GC.KeepAlive(this.replyBuffer);
            return reply;
        }

        protected void OnPingCompleted(PingCompletedEventArgs e)
        {
            if (this.PingCompleted != null)
            {
                this.PingCompleted(this, e);
            }
        }

        private static void PingCallback(object state, bool signaled)
        {
            Ping ping = (Ping) state;
            PingCompletedEventArgs arg = null;
            bool cancelled = false;
            AsyncOperation asyncOp = null;
            SendOrPostCallback d = null;
            try
            {
                lock (ping)
                {
                    cancelled = ping.cancelled;
                    asyncOp = ping.asyncOp;
                    d = ping.onPingCompletedDelegate;
                    if (!cancelled)
                    {
                        PingReply reply;
                        SafeLocalFree replyBuffer = ping.replyBuffer;
                        if (ping.ipv6)
                        {
                            UnsafeNetInfoNativeMethods.Icmp6ParseReplies(replyBuffer.DangerousGetHandle(), 0x100ff);
                        }
                        else if (ComNetOS.IsPostWin2K)
                        {
                            UnsafeNetInfoNativeMethods.IcmpParseReplies(replyBuffer.DangerousGetHandle(), 0x100ff);
                        }
                        else
                        {
                            UnsafeIcmpNativeMethods.IcmpParseReplies(replyBuffer.DangerousGetHandle(), 0x100ff);
                        }
                        if (ping.ipv6)
                        {
                            Icmp6EchoReply reply2 = (Icmp6EchoReply) Marshal.PtrToStructure(replyBuffer.DangerousGetHandle(), typeof(Icmp6EchoReply));
                            reply = new PingReply(reply2, replyBuffer.DangerousGetHandle(), ping.sendSize);
                        }
                        else
                        {
                            IcmpEchoReply reply3 = (IcmpEchoReply) Marshal.PtrToStructure(replyBuffer.DangerousGetHandle(), typeof(IcmpEchoReply));
                            reply = new PingReply(reply3);
                        }
                        arg = new PingCompletedEventArgs(reply, null, false, asyncOp.UserSuppliedState);
                    }
                }
            }
            catch (Exception exception)
            {
                PingException error = new PingException(SR.GetString("net_ping"), exception);
                arg = new PingCompletedEventArgs(null, error, false, asyncOp.UserSuppliedState);
            }
            catch
            {
                PingException exception3 = new PingException(SR.GetString("net_ping"), new Exception(SR.GetString("net_nonClsCompliantException")));
                arg = new PingCompletedEventArgs(null, exception3, false, asyncOp.UserSuppliedState);
            }
            finally
            {
                ping.FreeUnmanagedStructures();
                ping.Finish(true);
            }
            if (cancelled)
            {
                arg = new PingCompletedEventArgs(null, null, true, asyncOp.UserSuppliedState);
            }
            asyncOp.PostOperationCompleted(d, arg);
        }

        private void PingCompletedWaitCallback(object operationState)
        {
            this.OnPingCompleted((PingCompletedEventArgs) operationState);
        }

        private static void PingSendCallback(IAsyncResult result)
        {
            Ping asyncState = (Ping) result.AsyncState;
            PingCompletedEventArgs arg = null;
            try
            {
                int num2;
                asyncState.pingSocket.EndSendTo(result);
                PingReply reply = null;
                if (asyncState.cancelled)
                {
                    goto Label_0172;
                }
                EndPoint remoteEP = new IPEndPoint(0L, 0);
                int dataLength = 0;
                do
                {
                    dataLength = asyncState.pingSocket.ReceiveFrom(asyncState.downlevelReplyBuffer, ref remoteEP);
                    if (CorrectPacket(asyncState.downlevelReplyBuffer, asyncState.packet))
                    {
                        goto Label_007B;
                    }
                }
                while ((Environment.TickCount - asyncState.startTime) <= asyncState.llTimeout);
                reply = new PingReply(IPStatus.TimedOut);
            Label_007B:
                num2 = Environment.TickCount - asyncState.startTime;
                if (reply == null)
                {
                    reply = new PingReply(asyncState.downlevelReplyBuffer, dataLength, ((IPEndPoint) remoteEP).Address, num2);
                }
                arg = new PingCompletedEventArgs(reply, null, false, asyncState.asyncOp.UserSuppliedState);
            }
            catch (Exception exception)
            {
                PingReply reply2 = null;
                PingException error = null;
                SocketException exception3 = exception as SocketException;
                if (exception3 != null)
                {
                    if (exception3.ErrorCode == 0x274c)
                    {
                        reply2 = new PingReply(IPStatus.TimedOut);
                    }
                    else if (exception3.ErrorCode == 0x2738)
                    {
                        reply2 = new PingReply(IPStatus.PacketTooBig);
                    }
                }
                if (reply2 == null)
                {
                    error = new PingException(SR.GetString("net_ping"), exception);
                }
                arg = new PingCompletedEventArgs(reply2, error, false, asyncState.asyncOp.UserSuppliedState);
            }
            catch
            {
                PingException exception4 = new PingException(SR.GetString("net_ping"), new Exception(SR.GetString("net_nonClsCompliantException")));
                arg = new PingCompletedEventArgs(null, exception4, false, asyncState.asyncOp.UserSuppliedState);
            }
        Label_0172:
            try
            {
                if (asyncState.cancelled)
                {
                    arg = new PingCompletedEventArgs(null, null, true, asyncState.asyncOp.UserSuppliedState);
                }
                asyncState.asyncOp.PostOperationCompleted(asyncState.onPingCompletedDelegate, arg);
            }
            finally
            {
                asyncState.Finish(true);
            }
        }

        public PingReply Send(IPAddress address) => 
            this.Send(address, 0x1388, this.DefaultSendBuffer, null);

        public PingReply Send(string hostNameOrAddress) => 
            this.Send(hostNameOrAddress, 0x1388, this.DefaultSendBuffer, null);

        public PingReply Send(IPAddress address, int timeout) => 
            this.Send(address, timeout, this.DefaultSendBuffer, null);

        public PingReply Send(string hostNameOrAddress, int timeout) => 
            this.Send(hostNameOrAddress, timeout, this.DefaultSendBuffer, null);

        public PingReply Send(IPAddress address, int timeout, byte[] buffer) => 
            this.Send(address, timeout, buffer, null);

        public PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer) => 
            this.Send(hostNameOrAddress, timeout, buffer, null);

        public PingReply Send(IPAddress address, int timeout, byte[] buffer, PingOptions options)
        {
            IPAddress address2;
            PingReply reply;
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (buffer.Length > 0xffdc)
            {
                throw new ArgumentException(SR.GetString("net_invalidPingBufferSize"), "buffer");
            }
            if (timeout < 0)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
            {
                throw new ArgumentException(SR.GetString("net_invalid_ip_addr"), "address");
            }
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                address2 = new IPAddress(address.GetAddressBytes());
            }
            else
            {
                address2 = new IPAddress(address.GetAddressBytes(), address.ScopeId);
            }
            new NetworkInformationPermission(NetworkInformationAccess.Ping).Demand();
            this.CheckStart(false);
            try
            {
                reply = this.InternalSend(address2, buffer, timeout, options, false);
            }
            catch (Exception exception)
            {
                throw new PingException(SR.GetString("net_ping"), exception);
            }
            catch
            {
                throw new PingException(SR.GetString("net_ping"), new Exception(SR.GetString("net_nonClsCompliantException")));
            }
            finally
            {
                this.Finish(false);
            }
            return reply;
        }

        public PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options)
        {
            IPAddress address;
            if (ValidationHelper.IsBlankString(hostNameOrAddress))
            {
                throw new ArgumentNullException("hostNameOrAddress");
            }
            try
            {
                address = Dns.GetHostAddresses(hostNameOrAddress)[0];
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new PingException(SR.GetString("net_ping"), exception);
            }
            return this.Send(address, timeout, buffer, options);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public void SendAsync(IPAddress address, object userToken)
        {
            this.SendAsync(address, 0x1388, this.DefaultSendBuffer, userToken);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public void SendAsync(string hostNameOrAddress, object userToken)
        {
            this.SendAsync(hostNameOrAddress, 0x1388, this.DefaultSendBuffer, userToken);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public void SendAsync(IPAddress address, int timeout, object userToken)
        {
            this.SendAsync(address, timeout, this.DefaultSendBuffer, userToken);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public void SendAsync(string hostNameOrAddress, int timeout, object userToken)
        {
            this.SendAsync(hostNameOrAddress, timeout, this.DefaultSendBuffer, userToken);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public void SendAsync(IPAddress address, int timeout, byte[] buffer, object userToken)
        {
            this.SendAsync(address, timeout, buffer, null, userToken);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, object userToken)
        {
            this.SendAsync(hostNameOrAddress, timeout, buffer, null, userToken);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public void SendAsync(IPAddress address, int timeout, byte[] buffer, PingOptions options, object userToken)
        {
            IPAddress address2;
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (buffer.Length > 0xffdc)
            {
                throw new ArgumentException(SR.GetString("net_invalidPingBufferSize"), "buffer");
            }
            if (timeout < 0)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
            {
                throw new ArgumentException(SR.GetString("net_invalid_ip_addr"), "address");
            }
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                address2 = new IPAddress(address.GetAddressBytes());
            }
            else
            {
                address2 = new IPAddress(address.GetAddressBytes(), address.ScopeId);
            }
            new NetworkInformationPermission(NetworkInformationAccess.Ping).Demand();
            this.CheckStart(true);
            try
            {
                this.asyncOp = AsyncOperationManager.CreateOperation(userToken);
                this.InternalSend(address2, buffer, timeout, options, true);
            }
            catch (Exception exception)
            {
                this.Finish(true);
                throw new PingException(SR.GetString("net_ping"), exception);
            }
            catch
            {
                this.Finish(true);
                throw new PingException(SR.GetString("net_ping"), new Exception(SR.GetString("net_nonClsCompliantException")));
            }
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options, object userToken)
        {
            IPAddress address;
            if (ValidationHelper.IsBlankString(hostNameOrAddress))
            {
                throw new ArgumentNullException("hostNameOrAddress");
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (buffer.Length > 0xffdc)
            {
                throw new ArgumentException(SR.GetString("net_invalidPingBufferSize"), "buffer");
            }
            if (timeout < 0)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
            if (IPAddress.TryParse(hostNameOrAddress, out address))
            {
                this.SendAsync(address, timeout, buffer, options, userToken);
            }
            else
            {
                this.CheckStart(true);
                try
                {
                    this.asyncOp = AsyncOperationManager.CreateOperation(userToken);
                    AsyncStateObject state = new AsyncStateObject(hostNameOrAddress, buffer, timeout, options, userToken);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.ContinueAsyncSend), state);
                }
                catch (Exception exception)
                {
                    this.Finish(true);
                    throw new PingException(SR.GetString("net_ping"), exception);
                }
            }
        }

        public void SendAsyncCancel()
        {
            lock (this)
            {
                if (!this.InAsyncCall)
                {
                    return;
                }
                this.cancelled = true;
                if (this.pingSocket != null)
                {
                    this.pingSocket.Close();
                    this.pingSocket = null;
                }
            }
            this.asyncFinished.WaitOne();
        }

        private unsafe void SetUnmanagedStructures(byte[] buffer)
        {
            this.requestBuffer = SafeLocalFree.LocalAlloc(buffer.Length);
            byte* handle = (byte*) this.requestBuffer.DangerousGetHandle();
            for (int i = 0; i < buffer.Length; i++)
            {
                handle[i] = buffer[i];
            }
        }

        void IDisposable.Dispose()
        {
            this.InternalDispose();
        }

        private byte[] DefaultSendBuffer
        {
            get
            {
                if (this.defaultSendBuffer == null)
                {
                    this.defaultSendBuffer = new byte[0x20];
                    for (int i = 0; i < 0x20; i++)
                    {
                        this.defaultSendBuffer[i] = (byte) (0x61 + (i % 0x17));
                    }
                }
                return this.defaultSendBuffer;
            }
        }

        private bool InAsyncCall
        {
            get
            {
                if (this.asyncFinished == null)
                {
                    return false;
                }
                return !this.asyncFinished.WaitOne(0);
            }
            set
            {
                if (this.asyncFinished == null)
                {
                    this.asyncFinished = new ManualResetEvent(!value);
                }
                else if (value)
                {
                    this.asyncFinished.Reset();
                }
                else
                {
                    this.asyncFinished.Set();
                }
            }
        }

        internal class AsyncStateObject
        {
            internal byte[] buffer;
            internal string hostName;
            internal PingOptions options;
            internal int timeout;
            internal object userToken;

            internal AsyncStateObject(string hostName, byte[] buffer, int timeout, PingOptions options, object userToken)
            {
                this.hostName = hostName;
                this.buffer = buffer;
                this.timeout = timeout;
                this.options = options;
                this.userToken = userToken;
            }
        }
    }
}

