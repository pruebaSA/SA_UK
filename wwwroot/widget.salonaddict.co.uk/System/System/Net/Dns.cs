namespace System.Net
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;

    public static class Dns
    {
        private const int HostNameBufferLength = 0x100;
        private const int MaxHostName = 0x7e;
        private static WaitCallback resolveCallback = new WaitCallback(Dns.ResolveCallback);
        private static DnsPermission s_DnsPermission = new DnsPermission(PermissionState.Unrestricted);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public static IAsyncResult BeginGetHostAddresses(string hostNameOrAddress, AsyncCallback requestCallback, object state)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "BeginGetHostAddresses", hostNameOrAddress);
            }
            IAsyncResult retObject = HostResolutionBeginHelper(hostNameOrAddress, true, true, true, true, requestCallback, state);
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "BeginGetHostAddresses", retObject);
            }
            return retObject;
        }

        [Obsolete("BeginGetHostByName is obsoleted for this type, please use BeginGetHostEntry instead. http://go.microsoft.com/fwlink/?linkid=14202"), HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public static IAsyncResult BeginGetHostByName(string hostName, AsyncCallback requestCallback, object stateObject)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "BeginGetHostByName", hostName);
            }
            IAsyncResult retObject = HostResolutionBeginHelper(hostName, true, true, false, false, requestCallback, stateObject);
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "BeginGetHostByName", retObject);
            }
            return retObject;
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public static IAsyncResult BeginGetHostEntry(IPAddress address, AsyncCallback requestCallback, object stateObject)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "BeginGetHostEntry", address);
            }
            IAsyncResult retObject = HostResolutionBeginHelper(address, true, true, requestCallback, stateObject);
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "BeginGetHostEntry", retObject);
            }
            return retObject;
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public static IAsyncResult BeginGetHostEntry(string hostNameOrAddress, AsyncCallback requestCallback, object stateObject)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "BeginGetHostEntry", hostNameOrAddress);
            }
            IAsyncResult retObject = HostResolutionBeginHelper(hostNameOrAddress, false, true, true, true, requestCallback, stateObject);
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "BeginGetHostEntry", retObject);
            }
            return retObject;
        }

        [Obsolete("BeginResolve is obsoleted for this type, please use BeginGetHostEntry instead. http://go.microsoft.com/fwlink/?linkid=14202"), HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public static IAsyncResult BeginResolve(string hostName, AsyncCallback requestCallback, object stateObject)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "BeginResolve", hostName);
            }
            IAsyncResult retObject = HostResolutionBeginHelper(hostName, false, true, false, false, requestCallback, stateObject);
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "BeginResolve", retObject);
            }
            return retObject;
        }

        public static IPAddress[] EndGetHostAddresses(IAsyncResult asyncResult)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "EndGetHostAddresses", asyncResult);
            }
            IPHostEntry retObject = HostResolutionEndHelper(asyncResult);
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "EndGetHostAddresses", retObject);
            }
            return retObject.AddressList;
        }

        [Obsolete("EndGetHostByName is obsoleted for this type, please use EndGetHostEntry instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public static IPHostEntry EndGetHostByName(IAsyncResult asyncResult)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "EndGetHostByName", asyncResult);
            }
            IPHostEntry retObject = HostResolutionEndHelper(asyncResult);
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "EndGetHostByName", retObject);
            }
            return retObject;
        }

        public static IPHostEntry EndGetHostEntry(IAsyncResult asyncResult)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "EndGetHostEntry", asyncResult);
            }
            IPHostEntry retObject = HostResolutionEndHelper(asyncResult);
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "EndGetHostEntry", retObject);
            }
            return retObject;
        }

        [Obsolete("EndResolve is obsoleted for this type, please use EndGetHostEntry instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public static IPHostEntry EndResolve(IAsyncResult asyncResult)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "EndResolve", asyncResult);
            }
            IPHostEntry retObject = HostResolutionEndHelper(asyncResult);
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "EndResolve", retObject);
            }
            return retObject;
        }

        private static unsafe IPHostEntry GetAddrInfo(string name)
        {
            if (!ComNetOS.IsPostWin2K)
            {
                throw new SocketException(SocketError.OperationNotSupported);
            }
            SafeFreeAddrInfo outAddrInfo = null;
            ArrayList list = new ArrayList();
            string str = null;
            AddressInfo hints = new AddressInfo {
                ai_flags = AddressInfoHints.AI_CANONNAME,
                ai_family = AddressFamily.Unspecified
            };
            try
            {
                if (SafeFreeAddrInfo.GetAddrInfo(name, null, ref hints, out outAddrInfo) != 0)
                {
                    throw new SocketException();
                }
                for (AddressInfo* infoPtr = (AddressInfo*) outAddrInfo.DangerousGetHandle(); infoPtr != null; infoPtr = infoPtr->ai_next)
                {
                    if ((str == null) && (infoPtr->ai_canonname != null))
                    {
                        str = new string(infoPtr->ai_canonname);
                    }
                    if (((infoPtr->ai_family == AddressFamily.InterNetwork) && Socket.SupportsIPv4) || ((infoPtr->ai_family == AddressFamily.InterNetworkV6) && Socket.OSSupportsIPv6))
                    {
                        SocketAddress socketAddress = new SocketAddress(infoPtr->ai_family, infoPtr->ai_addrlen);
                        for (int i = 0; i < infoPtr->ai_addrlen; i++)
                        {
                            socketAddress.m_Buffer[i] = infoPtr->ai_addr[i];
                        }
                        if (infoPtr->ai_family == AddressFamily.InterNetwork)
                        {
                            list.Add(((IPEndPoint) IPEndPoint.Any.Create(socketAddress)).Address);
                        }
                        else
                        {
                            list.Add(((IPEndPoint) IPEndPoint.IPv6Any.Create(socketAddress)).Address);
                        }
                    }
                }
            }
            finally
            {
                if (outAddrInfo != null)
                {
                    outAddrInfo.Close();
                }
            }
            IPHostEntry entry = new IPHostEntry {
                HostName = (str != null) ? str : name,
                Aliases = new string[0],
                AddressList = new IPAddress[list.Count]
            };
            list.CopyTo(entry.AddressList);
            return entry;
        }

        public static IPAddress[] GetHostAddresses(string hostNameOrAddress)
        {
            IPAddress address;
            IPAddress[] addressList;
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "GetHostAddresses", hostNameOrAddress);
            }
            s_DnsPermission.Demand();
            if (hostNameOrAddress == null)
            {
                throw new ArgumentNullException("hostNameOrAddress");
            }
            if (TryParseAsIP(hostNameOrAddress, out address))
            {
                if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
                {
                    throw new ArgumentException(SR.GetString("net_invalid_ip_addr"), "hostNameOrAddress");
                }
                addressList = new IPAddress[] { address };
            }
            else
            {
                addressList = InternalGetHostByName(hostNameOrAddress, true).AddressList;
            }
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "GetHostAddresses", addressList);
            }
            return addressList;
        }

        [Obsolete("GetHostByAddress is obsoleted for this type, please use GetHostEntry instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public static IPHostEntry GetHostByAddress(IPAddress address)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "GetHostByAddress", "");
            }
            s_DnsPermission.Demand();
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            IPHostEntry retObject = InternalGetHostByAddress(address, false, true);
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "GetHostByAddress", retObject);
            }
            return retObject;
        }

        [Obsolete("GetHostByAddress is obsoleted for this type, please use GetHostEntry instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public static IPHostEntry GetHostByAddress(string address)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "GetHostByAddress", address);
            }
            s_DnsPermission.Demand();
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            IPHostEntry retObject = InternalGetHostByAddress(IPAddress.Parse(address), false, true);
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "GetHostByAddress", retObject);
            }
            return retObject;
        }

        [Obsolete("GetHostByName is obsoleted for this type, please use GetHostEntry instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public static IPHostEntry GetHostByName(string hostName)
        {
            if (hostName == null)
            {
                throw new ArgumentNullException("hostName");
            }
            s_DnsPermission.Demand();
            return InternalGetHostByName(hostName, false);
        }

        public static IPHostEntry GetHostEntry(IPAddress address)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "GetHostEntry", "");
            }
            s_DnsPermission.Demand();
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
            {
                throw new ArgumentException(SR.GetString("net_invalid_ip_addr"), "address");
            }
            IPHostEntry retObject = InternalGetHostByAddress(address, true, false);
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "GetHostEntry", retObject);
            }
            return retObject;
        }

        public static IPHostEntry GetHostEntry(string hostNameOrAddress)
        {
            IPAddress address;
            IPHostEntry hostByName;
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "GetHostEntry", hostNameOrAddress);
            }
            s_DnsPermission.Demand();
            if (hostNameOrAddress == null)
            {
                throw new ArgumentNullException("hostNameOrAddress");
            }
            if (TryParseAsIP(hostNameOrAddress, out address))
            {
                if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
                {
                    throw new ArgumentException(SR.GetString("net_invalid_ip_addr"), "hostNameOrAddress");
                }
                hostByName = InternalGetHostByAddress(address, true, false);
            }
            else
            {
                hostByName = InternalGetHostByName(hostNameOrAddress, true);
            }
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "GetHostEntry", hostByName);
            }
            return hostByName;
        }

        public static string GetHostName()
        {
            s_DnsPermission.Demand();
            Socket.InitializeSockets();
            StringBuilder hostName = new StringBuilder(0x100);
            if (UnsafeNclNativeMethods.OSSOCK.gethostname(hostName, 0x100) != SocketError.Success)
            {
                throw new SocketException();
            }
            return hostName.ToString();
        }

        private static IAsyncResult HostResolutionBeginHelper(IPAddress address, bool flowContext, bool includeIPv6, AsyncCallback requestCallback, object state)
        {
            s_DnsPermission.Demand();
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
            {
                throw new ArgumentException(SR.GetString("net_invalid_ip_addr"), "address");
            }
            ResolveAsyncResult result = new ResolveAsyncResult(address, null, includeIPv6, state, requestCallback);
            if (flowContext)
            {
                result.StartPostingAsyncOp(false);
            }
            ThreadPool.UnsafeQueueUserWorkItem(resolveCallback, result);
            result.FinishPostingAsyncOp();
            return result;
        }

        private static IAsyncResult HostResolutionBeginHelper(string hostName, bool useGetHostByName, bool flowContext, bool includeIPv6, bool throwOnIPAny, AsyncCallback requestCallback, object state)
        {
            IPAddress address;
            ResolveAsyncResult result;
            s_DnsPermission.Demand();
            if (hostName == null)
            {
                throw new ArgumentNullException("hostName");
            }
            if (TryParseAsIP(hostName, out address))
            {
                if (throwOnIPAny && (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any)))
                {
                    throw new ArgumentException(SR.GetString("net_invalid_ip_addr"), "hostNameOrAddress");
                }
                result = new ResolveAsyncResult(address, null, includeIPv6, state, requestCallback);
                if (useGetHostByName)
                {
                    IPHostEntry entry = new IPHostEntry {
                        AddressList = new IPAddress[] { address },
                        Aliases = new string[0],
                        HostName = address.ToString()
                    };
                    result.StartPostingAsyncOp(false);
                    result.InvokeCallback(entry);
                    result.FinishPostingAsyncOp();
                    return result;
                }
            }
            else
            {
                result = new ResolveAsyncResult(hostName, null, includeIPv6, state, requestCallback);
            }
            if (flowContext)
            {
                result.StartPostingAsyncOp(false);
            }
            ThreadPool.UnsafeQueueUserWorkItem(resolveCallback, result);
            result.FinishPostingAsyncOp();
            return result;
        }

        private static IPHostEntry HostResolutionEndHelper(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }
            ResolveAsyncResult result = asyncResult as ResolveAsyncResult;
            if (result == null)
            {
                throw new ArgumentException(SR.GetString("net_io_invalidasyncresult"), "asyncResult");
            }
            if (result.EndCalled)
            {
                throw new InvalidOperationException(SR.GetString("net_io_invalidendcall", new object[] { "EndResolve" }));
            }
            result.InternalWaitForCompletion();
            result.EndCalled = true;
            Exception exception = result.Result as Exception;
            if (exception != null)
            {
                throw exception;
            }
            return (IPHostEntry) result.Result;
        }

        internal static IPHostEntry InternalGetHostByAddress(IPAddress address, bool includeIPv6, bool throwOnFailure)
        {
            SocketError success = SocketError.Success;
            Exception exception = null;
            if (Socket.LegacySupportsIPv6 || (includeIPv6 && ComNetOS.IsPostWin2K))
            {
                string name = TryGetNameInfo(address, out success);
                if (success == SocketError.Success)
                {
                    return GetAddrInfo(name);
                }
                exception = new SocketException();
            }
            else
            {
                if (address.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    throw new SocketException(SocketError.ProtocolNotSupported);
                }
                int addr = (int) address.m_Address;
                IntPtr nativePointer = UnsafeNclNativeMethods.OSSOCK.gethostbyaddr(ref addr, Marshal.SizeOf(typeof(int)), ProtocolFamily.InterNetwork);
                if (nativePointer != IntPtr.Zero)
                {
                    return NativeToHostEntry(nativePointer);
                }
                exception = new SocketException();
            }
            if (throwOnFailure)
            {
                throw exception;
            }
            IPHostEntry entry = new IPHostEntry();
            try
            {
                entry.HostName = address.ToString();
                entry.Aliases = new string[0];
                entry.AddressList = new IPAddress[] { address };
            }
            catch
            {
                throw exception;
            }
            return entry;
        }

        internal static IPHostEntry InternalGetHostByName(string hostName) => 
            InternalGetHostByName(hostName, true);

        internal static IPHostEntry InternalGetHostByName(string hostName, bool includeIPv6)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "GetHostByName", hostName);
            }
            IPHostEntry retObject = null;
            if (hostName.Length > 0x7e)
            {
                object[] args = new object[] { "hostName", 0x7e.ToString(NumberFormatInfo.CurrentInfo) };
                throw new ArgumentOutOfRangeException(SR.GetString("net_toolong", args));
            }
            if (Socket.LegacySupportsIPv6 || (includeIPv6 && ComNetOS.IsPostWin2K))
            {
                retObject = GetAddrInfo(hostName);
            }
            else
            {
                IntPtr nativePointer = UnsafeNclNativeMethods.OSSOCK.gethostbyname(hostName);
                if (nativePointer == IntPtr.Zero)
                {
                    IPAddress address;
                    SocketException exception = new SocketException();
                    if (!IPAddress.TryParse(hostName, out address))
                    {
                        throw exception;
                    }
                    retObject = new IPHostEntry {
                        HostName = address.ToString(),
                        Aliases = new string[0],
                        AddressList = new IPAddress[] { address }
                    };
                    if (Logging.On)
                    {
                        Logging.Exit(Logging.Sockets, "DNS", "GetHostByName", retObject);
                    }
                    return retObject;
                }
                retObject = NativeToHostEntry(nativePointer);
            }
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "GetHostByName", retObject);
            }
            return retObject;
        }

        internal static IPHostEntry InternalResolveFast(string hostName, int timeout, out bool timedOut)
        {
            timedOut = false;
            if ((hostName.Length > 0) && (hostName.Length <= 0x7e))
            {
                IPAddress address;
                if (TryParseAsIP(hostName, out address))
                {
                    return new IPHostEntry { 
                        HostName = address.ToString(),
                        Aliases = new string[0],
                        AddressList = new IPAddress[] { address }
                    };
                }
                if (Socket.OSSupportsIPv6)
                {
                    try
                    {
                        return GetAddrInfo(hostName);
                    }
                    catch (Exception)
                    {
                        goto Label_0083;
                    }
                }
                IntPtr nativePointer = UnsafeNclNativeMethods.OSSOCK.gethostbyname(hostName);
                if (nativePointer != IntPtr.Zero)
                {
                    return NativeToHostEntry(nativePointer);
                }
            }
        Label_0083:
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "InternalResolveFast", (string) null);
            }
            return null;
        }

        private static IPHostEntry NativeToHostEntry(IntPtr nativePointer)
        {
            hostent hostent = (hostent) Marshal.PtrToStructure(nativePointer, typeof(hostent));
            IPHostEntry entry = new IPHostEntry();
            if (hostent.h_name != IntPtr.Zero)
            {
                entry.HostName = Marshal.PtrToStringAnsi(hostent.h_name);
            }
            ArrayList list = new ArrayList();
            IntPtr ptr = hostent.h_addr_list;
            nativePointer = Marshal.ReadIntPtr(ptr);
            while (nativePointer != IntPtr.Zero)
            {
                int newAddress = Marshal.ReadInt32(nativePointer);
                list.Add(new IPAddress(newAddress));
                ptr = IntPtrHelper.Add(ptr, IntPtr.Size);
                nativePointer = Marshal.ReadIntPtr(ptr);
            }
            entry.AddressList = new IPAddress[list.Count];
            list.CopyTo(entry.AddressList, 0);
            list.Clear();
            ptr = hostent.h_aliases;
            nativePointer = Marshal.ReadIntPtr(ptr);
            while (nativePointer != IntPtr.Zero)
            {
                string str = Marshal.PtrToStringAnsi(nativePointer);
                list.Add(str);
                ptr = IntPtrHelper.Add(ptr, IntPtr.Size);
                nativePointer = Marshal.ReadIntPtr(ptr);
            }
            entry.Aliases = new string[list.Count];
            list.CopyTo(entry.Aliases, 0);
            return entry;
        }

        [Obsolete("Resolve is obsoleted for this type, please use GetHostEntry instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public static IPHostEntry Resolve(string hostName)
        {
            IPAddress address;
            IPHostEntry hostByName;
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "Resolve", hostName);
            }
            s_DnsPermission.Demand();
            if (hostName == null)
            {
                throw new ArgumentNullException("hostName");
            }
            if (TryParseAsIP(hostName, out address) && ((address.AddressFamily != AddressFamily.InterNetworkV6) || Socket.LegacySupportsIPv6))
            {
                hostByName = InternalGetHostByAddress(address, false, false);
            }
            else
            {
                hostByName = InternalGetHostByName(hostName, false);
            }
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "Resolve", hostByName);
            }
            return hostByName;
        }

        private static void ResolveCallback(object context)
        {
            IPHostEntry hostByName;
            ResolveAsyncResult result = (ResolveAsyncResult) context;
            try
            {
                if (result.address != null)
                {
                    hostByName = InternalGetHostByAddress(result.address, result.includeIPv6, false);
                }
                else
                {
                    hostByName = InternalGetHostByName(result.hostName, result.includeIPv6);
                }
            }
            catch (Exception exception)
            {
                if (((exception is OutOfMemoryException) || (exception is ThreadAbortException)) || (exception is StackOverflowException))
                {
                    throw;
                }
                result.InvokeCallback(exception);
                return;
            }
            result.InvokeCallback(hostByName);
        }

        internal static string TryGetNameInfo(IPAddress addr, out SocketError errorCode)
        {
            if (!ComNetOS.IsPostWin2K)
            {
                throw new SocketException(SocketError.OperationNotSupported);
            }
            SocketAddress address = new IPEndPoint(addr, 0).Serialize();
            StringBuilder host = new StringBuilder(0x401);
            Socket.InitializeSockets();
            errorCode = UnsafeNclNativeMethods.OSSOCK.getnameinfo(address.m_Buffer, address.m_Size, host, host.Capacity, null, 0, 4);
            if (errorCode != SocketError.Success)
            {
                return null;
            }
            return host.ToString();
        }

        private static bool TryParseAsIP(string address, out IPAddress ip)
        {
            if (!IPAddress.TryParse(address, out ip))
            {
                return false;
            }
            return (((ip.AddressFamily == AddressFamily.InterNetwork) && Socket.SupportsIPv4) || ((ip.AddressFamily == AddressFamily.InterNetworkV6) && Socket.OSSupportsIPv6));
        }

        internal static IAsyncResult UnsafeBeginGetHostAddresses(string hostName, AsyncCallback requestCallback, object state)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.Sockets, "DNS", "UnsafeBeginGetHostAddresses", hostName);
            }
            IAsyncResult retObject = HostResolutionBeginHelper(hostName, true, false, true, true, requestCallback, state);
            if (Logging.On)
            {
                Logging.Exit(Logging.Sockets, "DNS", "UnsafeBeginGetHostAddresses", retObject);
            }
            return retObject;
        }

        private class ResolveAsyncResult : ContextAwareResult
        {
            internal IPAddress address;
            internal readonly string hostName;
            internal bool includeIPv6;

            internal ResolveAsyncResult(IPAddress address, object myObject, bool includeIPv6, object myState, AsyncCallback myCallBack) : base(myObject, myState, myCallBack)
            {
                this.includeIPv6 = includeIPv6;
                this.address = address;
            }

            internal ResolveAsyncResult(string hostName, object myObject, bool includeIPv6, object myState, AsyncCallback myCallBack) : base(myObject, myState, myCallBack)
            {
                this.hostName = hostName;
                this.includeIPv6 = includeIPv6;
            }
        }
    }
}

