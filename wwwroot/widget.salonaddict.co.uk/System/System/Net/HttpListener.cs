namespace System.Net
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Authentication.ExtendedProtection;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;

    public sealed class HttpListener : IDisposable
    {
        private static readonly Type ChannelBindingStatusType = typeof(UnsafeNclNativeMethods.HttpApi.HTTP_REQUEST_CHANNEL_BIND_STATUS);
        private const int DigestLifetimeSeconds = 300;
        private AuthenticationSelectorInfo m_AuthenticationDelegate;
        private System.Net.AuthenticationSchemes m_AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous;
        private ServiceNameStore m_DefaultServiceNames;
        private Hashtable m_DisconnectResults;
        private System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy m_ExtendedProtectionPolicy;
        private ExtendedProtectionSelector m_ExtendedProtectionSelectorDelegate;
        private ArrayList m_ExtraSavedDigests;
        private ArrayList m_ExtraSavedDigestsBaking;
        private int m_ExtraSavedDigestsTimestamp;
        private bool m_IgnoreWriteExceptions;
        private object m_InternalLock;
        private int m_NewestContext;
        private int m_OldestContext;
        private HttpListenerPrefixCollection m_Prefixes;
        private string m_Realm;
        private bool m_RequestHandleBound;
        private SafeCloseHandle m_RequestQueueHandle;
        private DigestContext[] m_SavedDigests;
        private SecurityException m_SecurityException;
        private State m_State;
        private bool m_UnsafeConnectionNtlmAuthentication;
        internal Hashtable m_UriPrefixes = new Hashtable();
        private const int MaximumDigests = 0x400;
        private const int MinimumDigestLifetimeSeconds = 10;
        private static readonly int RequestChannelBindStatusSize = Marshal.SizeOf(typeof(UnsafeNclNativeMethods.HttpApi.HTTP_REQUEST_CHANNEL_BIND_STATUS));
        private static byte[] s_WwwAuthenticateBytes = new byte[] { 0x57, 0x57, 0x57, 0x2d, 0x41, 0x75, 0x74, 0x68, 0x65, 110, 0x74, 0x69, 0x63, 0x61, 0x74, 0x65 };

        public HttpListener()
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "HttpListener", "");
            }
            if (!UnsafeNclNativeMethods.HttpApi.Supported)
            {
                throw new PlatformNotSupportedException();
            }
            this.m_State = State.Stopped;
            this.m_InternalLock = new object();
            this.m_DefaultServiceNames = new ServiceNameStore();
            this.m_ExtendedProtectionPolicy = new System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy(PolicyEnforcement.Never);
            if (Logging.On)
            {
                Logging.Exit(Logging.HttpListener, this, "HttpListener", "");
            }
        }

        public void Abort()
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "Abort", "");
            }
            try
            {
                if (this.m_RequestQueueHandle != null)
                {
                    this.m_RequestQueueHandle.Abort();
                }
                this.m_RequestHandleBound = false;
                this.m_State = State.Closed;
                this.ClearDigestCache();
            }
            catch (Exception exception)
            {
                if (Logging.On)
                {
                    Logging.Exception(Logging.HttpListener, this, "Abort", exception);
                }
                throw;
            }
            finally
            {
                if (Logging.On)
                {
                    Logging.Exit(Logging.HttpListener, this, "Abort", "");
                }
            }
        }

        private void AddAll()
        {
            if (this.m_UriPrefixes.Count > 0)
            {
                foreach (string str in this.m_UriPrefixes.Values)
                {
                    uint num = this.InternalAddPrefix(str);
                    if (num != 0)
                    {
                        this.Abort();
                        if (num == 0xb7)
                        {
                            throw new HttpListenerException((int) num, SR.GetString("net_listener_already", new object[] { str }));
                        }
                        throw new HttpListenerException((int) num);
                    }
                }
            }
        }

        private static void AddChallenge(ref ArrayList challenges, string challenge)
        {
            if (challenge != null)
            {
                challenge = challenge.Trim();
                if (challenge.Length > 0)
                {
                    if (challenges == null)
                    {
                        challenges = new ArrayList(4);
                    }
                    challenges.Add(challenge);
                }
            }
        }

        internal unsafe void AddPrefix(string uriPrefix)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "AddPrefix", "uriPrefix:" + uriPrefix);
            }
            string str = null;
            try
            {
                int num;
                if (uriPrefix == null)
                {
                    throw new ArgumentNullException("uriPrefix");
                }
                new WebPermission(NetworkAccess.Accept, uriPrefix).Demand();
                this.CheckDisposed();
                if (string.Compare(uriPrefix, 0, "http://", 0, 7, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    num = 7;
                }
                else
                {
                    if (string.Compare(uriPrefix, 0, "https://", 0, 8, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        throw new ArgumentException(SR.GetString("net_listener_scheme"), "uriPrefix");
                    }
                    num = 8;
                }
                bool flag = false;
                int length = num;
                while (((length < uriPrefix.Length) && (uriPrefix[length] != '/')) && ((uriPrefix[length] != ':') || flag))
                {
                    if (uriPrefix[length] == '[')
                    {
                        if (flag)
                        {
                            length = num;
                            break;
                        }
                        flag = true;
                    }
                    if (flag && (uriPrefix[length] == ']'))
                    {
                        flag = false;
                    }
                    length++;
                }
                if (num == length)
                {
                    throw new ArgumentException(SR.GetString("net_listener_host"), "uriPrefix");
                }
                if (uriPrefix[uriPrefix.Length - 1] != '/')
                {
                    throw new ArgumentException(SR.GetString("net_listener_slash"), "uriPrefix");
                }
                str = (uriPrefix[length] == ':') ? string.Copy(uriPrefix) : (uriPrefix.Substring(0, length) + ((num == 7) ? ":80" : ":443") + uriPrefix.Substring(length));
                fixed (char* str2 = ((char*) str))
                {
                    char* chPtr = str2;
                    for (num = 0; chPtr[num] != ':'; num++)
                    {
                        chPtr[num] = (char) CaseInsensitiveAscii.AsciiToLower[(byte) chPtr[num]];
                    }
                }
                if (this.m_State == State.Started)
                {
                    uint num3 = this.InternalAddPrefix(str);
                    switch (num3)
                    {
                        case 0:
                            goto Label_01D9;

                        case 0xb7:
                            throw new HttpListenerException((int) num3, SR.GetString("net_listener_already", new object[] { str }));
                    }
                    throw new HttpListenerException((int) num3);
                }
            Label_01D9:
                this.m_UriPrefixes[uriPrefix] = str;
                this.m_DefaultServiceNames.Add(uriPrefix);
            }
            catch (Exception exception)
            {
                if (Logging.On)
                {
                    Logging.Exception(Logging.HttpListener, this, "AddPrefix", exception);
                }
                throw;
            }
            finally
            {
                if (Logging.On)
                {
                    Logging.Exit(Logging.HttpListener, this, "AddPrefix", "prefix:" + str);
                }
            }
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public IAsyncResult BeginGetContext(AsyncCallback callback, object state)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "BeginGetContext", "");
            }
            ListenerAsyncResult objectValue = null;
            try
            {
                this.CheckDisposed();
                if (this.m_State == State.Stopped)
                {
                    throw new InvalidOperationException(SR.GetString("net_listener_mustcall", new object[] { "Start()" }));
                }
                objectValue = new ListenerAsyncResult(this, state, callback);
                uint num = objectValue.QueueBeginGetContext();
                if ((num != 0) && (num != 0x3e5))
                {
                    throw new HttpListenerException((int) num);
                }
            }
            catch (Exception exception)
            {
                if (Logging.On)
                {
                    Logging.Exception(Logging.HttpListener, this, "BeginGetContext", exception);
                }
                throw;
            }
            finally
            {
                if (Logging.On)
                {
                    Logging.Enter(Logging.HttpListener, this, "BeginGetContext", "IAsyncResult#" + ValidationHelper.HashString(objectValue));
                }
            }
            return objectValue;
        }

        private ArrayList BuildChallenge(System.Net.AuthenticationSchemes authenticationScheme, ulong connectionId, out NTAuthentication newContext, System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy policy, bool isSecureConnection)
        {
            ArrayList challenges = null;
            newContext = null;
            if ((authenticationScheme & System.Net.AuthenticationSchemes.Negotiate) != System.Net.AuthenticationSchemes.None)
            {
                AddChallenge(ref challenges, "Negotiate");
            }
            if ((authenticationScheme & System.Net.AuthenticationSchemes.Ntlm) != System.Net.AuthenticationSchemes.None)
            {
                AddChallenge(ref challenges, "NTLM");
            }
            if ((authenticationScheme & System.Net.AuthenticationSchemes.Digest) != System.Net.AuthenticationSchemes.None)
            {
                NTAuthentication authentication = null;
                try
                {
                    bool flag;
                    string str = null;
                    ChannelBinding channelBinding = this.GetChannelBinding(connectionId, isSecureConnection, policy, out flag);
                    if (!flag)
                    {
                        SecurityStatus status;
                        authentication = new NTAuthentication(true, "WDigest", null, this.GetContextFlags(policy, isSecureConnection), channelBinding);
                        str = authentication.GetOutgoingDigestBlob(null, null, null, this.Realm, false, false, out status);
                        if (authentication.IsValidContext)
                        {
                            newContext = authentication;
                        }
                        AddChallenge(ref challenges, "Digest" + (string.IsNullOrEmpty(str) ? "" : (" " + str)));
                    }
                }
                finally
                {
                    if ((authentication != null) && (newContext != authentication))
                    {
                        authentication.CloseContext();
                    }
                }
            }
            if ((authenticationScheme & System.Net.AuthenticationSchemes.Basic) != System.Net.AuthenticationSchemes.None)
            {
                AddChallenge(ref challenges, "Basic realm=\"" + this.Realm + "\"");
            }
            return challenges;
        }

        internal void CheckDisposed()
        {
            if (this.m_State == State.Closed)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
        }

        private bool CheckSpn(NTAuthentication context, bool isSecureConnection, System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy policy)
        {
            if (context.IsKerberos)
            {
                if (Logging.On)
                {
                    Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_no_spn_kerberos"));
                }
                return true;
            }
            if ((policy.PolicyEnforcement == PolicyEnforcement.Never) || ScenarioChecksChannelBinding(isSecureConnection, policy.ProtectionScenario))
            {
                if (Logging.On)
                {
                    Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_no_spn_disabled"));
                }
                return true;
            }
            if (ScenarioChecksChannelBinding(isSecureConnection, policy.ProtectionScenario))
            {
                if (Logging.On)
                {
                    Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_no_spn_cbt"));
                }
                return true;
            }
            if (!AuthenticationManager.OSSupportsExtendedProtection)
            {
                if (Logging.On)
                {
                    Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_no_spn_platform"));
                }
                return true;
            }
            string clientSpecifiedSpn = context.ClientSpecifiedSpn;
            if (string.IsNullOrEmpty(clientSpecifiedSpn))
            {
                bool flag = false;
                if (policy.PolicyEnforcement == PolicyEnforcement.WhenSupported)
                {
                    flag = true;
                    if (Logging.On)
                    {
                        Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_no_spn_whensupported"));
                    }
                    return flag;
                }
                if (Logging.On)
                {
                    Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_spn_failed_always"));
                }
                return false;
            }
            if (string.Compare(clientSpecifiedSpn, "http/localhost", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (Logging.On)
                {
                    Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_no_spn_loopback"));
                }
                return true;
            }
            if (Logging.On)
            {
                Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_spn", new object[] { clientSpecifiedSpn }));
            }
            ServiceNameCollection serviceNames = this.GetServiceNames(policy);
            bool flag2 = false;
            foreach (string str2 in serviceNames)
            {
                if (string.Compare(clientSpecifiedSpn, str2, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    flag2 = true;
                    if (Logging.On)
                    {
                        Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_spn_passed"));
                    }
                    break;
                }
            }
            if (Logging.On && !flag2)
            {
                Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_spn_failed"));
                if (serviceNames.Count == 0)
                {
                    Logging.PrintWarning(Logging.HttpListener, this, "CheckSpn", SR.GetString("net_log_listener_spn_failed_empty"));
                    return flag2;
                }
                Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_spn_failed_dump"));
                foreach (string str3 in serviceNames)
                {
                    Logging.PrintInfo(Logging.HttpListener, this, "\t" + str3);
                }
            }
            return flag2;
        }

        private void ClearDigestCache()
        {
            if (this.m_SavedDigests != null)
            {
                ArrayList[] listArray = new ArrayList[3];
                lock (this.m_SavedDigests)
                {
                    listArray[0] = this.m_ExtraSavedDigestsBaking;
                    this.m_ExtraSavedDigestsBaking = null;
                    listArray[1] = this.m_ExtraSavedDigests;
                    this.m_ExtraSavedDigests = null;
                    this.m_NewestContext = 0;
                    this.m_OldestContext = 0;
                    listArray[2] = new ArrayList();
                    for (int j = 0; j < 0x400; j++)
                    {
                        if (this.m_SavedDigests[j].context != null)
                        {
                            listArray[2].Add(this.m_SavedDigests[j].context);
                            this.m_SavedDigests[j].context = null;
                        }
                        this.m_SavedDigests[j].timestamp = 0;
                    }
                }
                for (int i = 0; i < listArray.Length; i++)
                {
                    if (listArray[i] != null)
                    {
                        for (int k = 0; k < listArray[i].Count; k++)
                        {
                            ((NTAuthentication) listArray[i][k]).CloseContext();
                        }
                    }
                }
            }
        }

        public void Close()
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "Close", "");
            }
            try
            {
                ((IDisposable) this).Dispose();
            }
            catch (Exception exception)
            {
                if (Logging.On)
                {
                    Logging.Exception(Logging.HttpListener, this, "Close", exception);
                }
                throw;
            }
            finally
            {
                if (Logging.On)
                {
                    Logging.Exit(Logging.HttpListener, this, "Close", "");
                }
            }
        }

        [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.ControlPrincipal), SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode)]
        private WindowsIdentity CreateWindowsIdentity(IntPtr userToken, string type, WindowsAccountType acctType, bool isAuthenticated) => 
            new WindowsIdentity(userToken, type, acctType, isAuthenticated);

        private void Dispose(bool disposing)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "Dispose", "");
            }
            try
            {
                if (this.m_State != State.Closed)
                {
                    this.Stop();
                    this.m_RequestHandleBound = false;
                    this.m_State = State.Closed;
                }
            }
            catch (Exception exception)
            {
                if (Logging.On)
                {
                    Logging.Exception(Logging.HttpListener, this, "Dispose", exception);
                }
                throw;
            }
            finally
            {
                if (Logging.On)
                {
                    Logging.Exit(Logging.HttpListener, this, "Dispose", "");
                }
            }
        }

        public HttpListenerContext EndGetContext(IAsyncResult asyncResult)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "EndGetContext", "IAsyncResult#" + ValidationHelper.HashString(asyncResult));
            }
            HttpListenerContext objectValue = null;
            try
            {
                this.CheckDisposed();
                if (asyncResult == null)
                {
                    throw new ArgumentNullException("asyncResult");
                }
                ListenerAsyncResult result = asyncResult as ListenerAsyncResult;
                if ((result == null) || (result.AsyncObject != this))
                {
                    throw new ArgumentException(SR.GetString("net_io_invalidasyncresult"), "asyncResult");
                }
                if (result.EndCalled)
                {
                    throw new InvalidOperationException(SR.GetString("net_io_invalidendcall", new object[] { "EndGetContext" }));
                }
                result.EndCalled = true;
                objectValue = result.InternalWaitForCompletion() as HttpListenerContext;
                if (objectValue == null)
                {
                    throw (result.Result as Exception);
                }
            }
            catch (Exception exception)
            {
                if (Logging.On)
                {
                    Logging.Exception(Logging.HttpListener, this, "EndGetContext", exception);
                }
                throw;
            }
            finally
            {
                if (Logging.On)
                {
                    Logging.Exit(Logging.HttpListener, this, "EndGetContext", (objectValue == null) ? "<no context>" : string.Concat(new object[] { "HttpListenerContext#", ValidationHelper.HashString(objectValue), " RequestTraceIdentifier#", objectValue.Request.RequestTraceIdentifier }));
                }
            }
            return objectValue;
        }

        [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode)]
        internal void EnsureBoundHandle()
        {
            if (!this.m_RequestHandleBound)
            {
                lock (this.m_InternalLock)
                {
                    if (!this.m_RequestHandleBound)
                    {
                        ThreadPool.BindHandle(this.m_RequestQueueHandle.DangerousGetHandle());
                        this.m_RequestHandleBound = true;
                    }
                }
            }
        }

        private ChannelBinding GetChannelBinding(ulong connectionId, bool isSecureConnection, System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy policy, out bool extendedProtectionFailure)
        {
            extendedProtectionFailure = false;
            if (policy.PolicyEnforcement == PolicyEnforcement.Never)
            {
                if (Logging.On)
                {
                    Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_no_cbt_disabled"));
                }
                return null;
            }
            if (!isSecureConnection)
            {
                if (Logging.On)
                {
                    Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_no_cbt_http"));
                }
                return null;
            }
            if (!AuthenticationManager.OSSupportsExtendedProtection)
            {
                if (Logging.On)
                {
                    Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_no_cbt_platform"));
                }
                return null;
            }
            if (policy.ProtectionScenario == ProtectionScenario.TrustedProxy)
            {
                if (Logging.On)
                {
                    Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_no_cbt_trustedproxy"));
                }
                return null;
            }
            ChannelBinding channelBindingFromTls = this.GetChannelBindingFromTls(connectionId);
            if (channelBindingFromTls == null)
            {
                if (Logging.On)
                {
                    Logging.PrintInfo(Logging.HttpListener, this, SR.GetString("net_log_listener_cbt"));
                }
                extendedProtectionFailure = true;
            }
            return channelBindingFromTls;
        }

        internal unsafe ChannelBinding GetChannelBindingFromTls(ulong connectionId)
        {
            uint num3;
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, "HttpListener#" + ValidationHelper.HashString(this) + "::GetChannelBindingFromTls() connectionId: " + connectionId.ToString());
            }
            int num = RequestChannelBindStatusSize + 0x80;
            byte[] source = null;
            SafeLocalFreeChannelBinding binding = null;
            uint pBytesReceived = 0;
            do
            {
                source = new byte[num];
                fixed (byte* numRef = source)
                {
                    num3 = UnsafeNclNativeMethods.HttpApi.HttpReceiveClientCertificate(this.RequestQueueHandle, connectionId, 1, numRef, (uint) num, &pBytesReceived, null);
                    if (num3 == 0)
                    {
                        int tokenOffsetFromBlob = GetTokenOffsetFromBlob((IntPtr) numRef);
                        int tokenSizeFromBlob = GetTokenSizeFromBlob((IntPtr) numRef);
                        binding = SafeLocalFreeChannelBinding.LocalAlloc(tokenSizeFromBlob);
                        if (binding.IsInvalid)
                        {
                            throw new OutOfMemoryException();
                        }
                        Marshal.Copy(source, tokenOffsetFromBlob, binding.DangerousGetHandle(), tokenSizeFromBlob);
                    }
                    else if (num3 == 0xea)
                    {
                        int num6 = GetTokenSizeFromBlob((IntPtr) numRef);
                        num = RequestChannelBindStatusSize + num6;
                    }
                    else
                    {
                        if (num3 != 0x57)
                        {
                            throw new HttpListenerException((int) num3);
                        }
                        if (Logging.On)
                        {
                            Logging.PrintError(Logging.HttpListener, "HttpListener#" + ValidationHelper.HashString(this) + "::GetChannelBindingFromTls() Can't retrieve CBT from TLS: ERROR_INVALID_PARAMETER");
                        }
                        return null;
                    }
                }
            }
            while (num3 != 0);
            return binding;
        }

        public unsafe HttpListenerContext GetContext()
        {
            HttpListenerContext context3;
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "GetContext", "");
            }
            SyncRequestContext memoryBlob = null;
            HttpListenerContext objectValue = null;
            bool stoleBlob = false;
            try
            {
                uint num4;
                this.CheckDisposed();
                if (this.m_State == State.Stopped)
                {
                    throw new InvalidOperationException(SR.GetString("net_listener_mustcall", new object[] { "Start()" }));
                }
                if (this.m_UriPrefixes.Count == 0)
                {
                    throw new InvalidOperationException(SR.GetString("net_listener_mustcall", new object[] { "AddPrefix()" }));
                }
                uint num = 0;
                uint requestBufferLength = 0x1000;
                ulong requestId = 0L;
                memoryBlob = new SyncRequestContext((int) requestBufferLength);
            Label_0098:
                num4 = 0;
                num = UnsafeNclNativeMethods.HttpApi.HttpReceiveHttpRequest(this.m_RequestQueueHandle, requestId, 1, memoryBlob.RequestBlob, requestBufferLength, &num4, null);
                if ((num == 0x57) && (requestId != 0L))
                {
                    requestId = 0L;
                    goto Label_0098;
                }
                if (num == 0xea)
                {
                    requestBufferLength = num4;
                    requestId = memoryBlob.RequestBlob.RequestId;
                    memoryBlob.Reset((int) requestBufferLength);
                    goto Label_0098;
                }
                if (num != 0)
                {
                    throw new HttpListenerException((int) num);
                }
                objectValue = this.HandleAuthentication(memoryBlob, out stoleBlob);
                if (stoleBlob)
                {
                    memoryBlob = null;
                    stoleBlob = false;
                }
                if (objectValue != null)
                {
                    context3 = objectValue;
                }
                else
                {
                    if (memoryBlob == null)
                    {
                        memoryBlob = new SyncRequestContext((int) requestBufferLength);
                    }
                    requestId = 0L;
                    goto Label_0098;
                }
            }
            catch (Exception exception)
            {
                if (Logging.On)
                {
                    Logging.Exception(Logging.HttpListener, this, "GetContext", exception);
                }
                throw;
            }
            finally
            {
                if ((memoryBlob != null) && !stoleBlob)
                {
                    memoryBlob.ReleasePins();
                    memoryBlob.Close();
                }
                if (Logging.On)
                {
                    Logging.Exit(Logging.HttpListener, this, "GetContext", string.Concat(new object[] { "HttpListenerContext#", ValidationHelper.HashString(objectValue), " RequestTraceIdentifier#", objectValue.Request.RequestTraceIdentifier }));
                }
            }
            return context3;
        }

        private ContextFlags GetContextFlags(System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy policy, bool isSecureConnection)
        {
            ContextFlags connection = ContextFlags.Connection;
            if (policy.PolicyEnforcement != PolicyEnforcement.Never)
            {
                if (policy.PolicyEnforcement == PolicyEnforcement.WhenSupported)
                {
                    connection |= ContextFlags.AllowMissingBindings;
                }
                if (policy.ProtectionScenario == ProtectionScenario.TrustedProxy)
                {
                    connection |= ContextFlags.ProxyBindings;
                }
            }
            return connection;
        }

        private ServiceNameCollection GetServiceNames(System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy policy)
        {
            if (policy.CustomServiceNames == null)
            {
                if (this.m_DefaultServiceNames.ServiceNames.Count == 0)
                {
                    throw new InvalidOperationException(SR.GetString("net_listener_no_spns"));
                }
                return this.m_DefaultServiceNames.ServiceNames;
            }
            return policy.CustomServiceNames;
        }

        private static int GetTokenOffsetFromBlob(IntPtr blob) => 
            ((int) IntPtrHelper.Subtract(Marshal.ReadIntPtr(blob, (int) Marshal.OffsetOf(ChannelBindingStatusType, "ChannelToken")), blob));

        private static int GetTokenSizeFromBlob(IntPtr blob) => 
            Marshal.ReadInt32(blob, (int) Marshal.OffsetOf(ChannelBindingStatusType, "ChannelTokenSize"));

        internal unsafe HttpListenerContext HandleAuthentication(RequestContextBase memoryBlob, out bool stoleBlob)
        {
            string challenge = null;
            HttpListenerContext context3;
            stoleBlob = false;
            string verb = UnsafeNclNativeMethods.HttpApi.GetVerb(memoryBlob.RequestBlob);
            string knownHeader = UnsafeNclNativeMethods.HttpApi.GetKnownHeader(memoryBlob.RequestBlob, 0x18);
            ulong connectionId = memoryBlob.RequestBlob.ConnectionId;
            ulong requestId = memoryBlob.RequestBlob.RequestId;
            bool isSecureConnection = memoryBlob.RequestBlob.pSslInfo != null;
            DisconnectAsyncResult disconnectResult = (DisconnectAsyncResult) this.DisconnectResults[connectionId];
            if (this.UnsafeConnectionNtlmAuthentication)
            {
                if (knownHeader == null)
                {
                    WindowsPrincipal authenticatedConnection = disconnectResult?.AuthenticatedConnection;
                    if (authenticatedConnection != null)
                    {
                        stoleBlob = true;
                        HttpListenerContext context = new HttpListenerContext(this, memoryBlob);
                        context.SetIdentity(authenticatedConnection, null);
                        context.Request.ReleasePins();
                        return context;
                    }
                }
                else if (disconnectResult != null)
                {
                    disconnectResult.AuthenticatedConnection = null;
                }
            }
            stoleBlob = true;
            HttpListenerContext context2 = null;
            NTAuthentication digestContext = null;
            NTAuthentication newContext = null;
            NTAuthentication authentication3 = null;
            System.Net.AuthenticationSchemes none = System.Net.AuthenticationSchemes.None;
            System.Net.AuthenticationSchemes authenticationSchemes = this.AuthenticationSchemes;
            System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy extendedProtectionPolicy = this.m_ExtendedProtectionPolicy;
            try
            {
                ExtendedProtectionSelector selector;
                SecurityStatus invalidToken;
                ChannelBinding binding;
                string str6;
                ArrayList list;
                if ((disconnectResult != null) && !disconnectResult.StartOwningDisconnectHandling())
                {
                    disconnectResult = null;
                }
                if (disconnectResult != null)
                {
                    digestContext = disconnectResult.Session;
                }
                context2 = new HttpListenerContext(this, memoryBlob);
                AuthenticationSelectorInfo authenticationDelegate = this.m_AuthenticationDelegate;
                if (authenticationDelegate != null)
                {
                    try
                    {
                        context2.Request.ReleasePins();
                        authenticationSchemes = authenticationDelegate.Delegate(context2.Request);
                        if (!authenticationDelegate.AdvancedAuth && ((authenticationSchemes & (System.Net.AuthenticationSchemes.IntegratedWindowsAuthentication | System.Net.AuthenticationSchemes.Digest)) != System.Net.AuthenticationSchemes.None))
                        {
                            throw this.m_SecurityException;
                        }
                        goto Label_01A2;
                    }
                    catch (Exception exception)
                    {
                        if (NclUtilities.IsFatal(exception))
                        {
                            throw;
                        }
                        if (Logging.On)
                        {
                            Logging.PrintError(Logging.HttpListener, this, "HandleAuthentication", SR.GetString("net_log_listener_delegate_exception", new object[] { exception }));
                        }
                        this.SendError(requestId, HttpStatusCode.InternalServerError, null);
                        context2.Close();
                        return null;
                    }
                }
                stoleBlob = false;
            Label_01A2:
                selector = this.m_ExtendedProtectionSelectorDelegate;
                if (selector != null)
                {
                    extendedProtectionPolicy = selector(context2.Request);
                    if (extendedProtectionPolicy == null)
                    {
                        extendedProtectionPolicy = new System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy(PolicyEnforcement.Never);
                    }
                }
                int length = -1;
                if ((knownHeader != null) && ((authenticationSchemes & ~System.Net.AuthenticationSchemes.Anonymous) != System.Net.AuthenticationSchemes.None))
                {
                    length = 0;
                    while (length < knownHeader.Length)
                    {
                        if (((knownHeader[length] == ' ') || (knownHeader[length] == '\t')) || ((knownHeader[length] == '\r') || (knownHeader[length] == '\n')))
                        {
                            break;
                        }
                        length++;
                    }
                    if (length < knownHeader.Length)
                    {
                        if (((authenticationSchemes & System.Net.AuthenticationSchemes.Negotiate) != System.Net.AuthenticationSchemes.None) && (string.Compare(knownHeader, 0, "Negotiate", 0, length, StringComparison.OrdinalIgnoreCase) == 0))
                        {
                            none = System.Net.AuthenticationSchemes.Negotiate;
                        }
                        else if (((authenticationSchemes & System.Net.AuthenticationSchemes.Ntlm) != System.Net.AuthenticationSchemes.None) && (string.Compare(knownHeader, 0, "NTLM", 0, length, StringComparison.OrdinalIgnoreCase) == 0))
                        {
                            none = System.Net.AuthenticationSchemes.Ntlm;
                        }
                        else if (((authenticationSchemes & System.Net.AuthenticationSchemes.Digest) != System.Net.AuthenticationSchemes.None) && (string.Compare(knownHeader, 0, "Digest", 0, length, StringComparison.OrdinalIgnoreCase) == 0))
                        {
                            none = System.Net.AuthenticationSchemes.Digest;
                        }
                        else if (((authenticationSchemes & System.Net.AuthenticationSchemes.Basic) != System.Net.AuthenticationSchemes.None) && (string.Compare(knownHeader, 0, "Basic", 0, length, StringComparison.OrdinalIgnoreCase) == 0))
                        {
                            none = System.Net.AuthenticationSchemes.Basic;
                        }
                        else if (Logging.On)
                        {
                            Logging.PrintWarning(Logging.HttpListener, this, "HandleAuthentication", SR.GetString("net_log_listener_unsupported_authentication_scheme", new object[] { knownHeader, authenticationSchemes }));
                        }
                    }
                }
                HttpStatusCode internalServerError = HttpStatusCode.InternalServerError;
                bool flag2 = false;
                if (none == System.Net.AuthenticationSchemes.None)
                {
                    if (Logging.On)
                    {
                        Logging.PrintWarning(Logging.HttpListener, this, "HandleAuthentication", SR.GetString("net_log_listener_unmatched_authentication_scheme", new object[] { ValidationHelper.ToString(authenticationSchemes), (knownHeader == null) ? "<null>" : knownHeader }));
                    }
                    if ((authenticationSchemes & System.Net.AuthenticationSchemes.Anonymous) != System.Net.AuthenticationSchemes.None)
                    {
                        if (!stoleBlob)
                        {
                            stoleBlob = true;
                            context2.Request.ReleasePins();
                        }
                        return context2;
                    }
                    internalServerError = HttpStatusCode.Unauthorized;
                    context2.Request.DetachBlob(memoryBlob);
                    context2.Close();
                    context2 = null;
                    goto Label_07CC;
                }
                byte[] bytes = null;
                byte[] inArray = null;
                string str4 = null;
                length++;
                while (length < knownHeader.Length)
                {
                    if (((knownHeader[length] != ' ') && (knownHeader[length] != '\t')) && ((knownHeader[length] != '\r') && (knownHeader[length] != '\n')))
                    {
                        break;
                    }
                    length++;
                }
                string incomingBlob = (length < knownHeader.Length) ? knownHeader.Substring(length) : "";
                IPrincipal principal = null;
                bool extendedProtectionFailure = false;
                switch (none)
                {
                    case System.Net.AuthenticationSchemes.Digest:
                    {
                        binding = this.GetChannelBinding(connectionId, isSecureConnection, extendedProtectionPolicy, out extendedProtectionFailure);
                        if (extendedProtectionFailure)
                        {
                            goto Label_0528;
                        }
                        authentication3 = new NTAuthentication(true, "WDigest", null, this.GetContextFlags(extendedProtectionPolicy, isSecureConnection), binding);
                        str4 = authentication3.GetOutgoingDigestBlob(incomingBlob, verb, null, this.Realm, false, false, out invalidToken);
                        if (invalidToken == SecurityStatus.OK)
                        {
                            str4 = null;
                        }
                        if (!authentication3.IsValidContext)
                        {
                            break;
                        }
                        SafeCloseHandle contextToken = null;
                        try
                        {
                            if (!this.CheckSpn(authentication3, isSecureConnection, extendedProtectionPolicy))
                            {
                                internalServerError = HttpStatusCode.Unauthorized;
                            }
                            else
                            {
                                context2.Request.ServiceName = authentication3.ClientSpecifiedSpn;
                                contextToken = authentication3.GetContextToken(out invalidToken);
                                if (invalidToken != SecurityStatus.OK)
                                {
                                    internalServerError = this.HttpStatusFromSecurityStatus(invalidToken);
                                }
                                else if (contextToken == null)
                                {
                                    internalServerError = HttpStatusCode.Unauthorized;
                                }
                                else
                                {
                                    principal = new WindowsPrincipal(this.CreateWindowsIdentity(contextToken.DangerousGetHandle(), "Digest", WindowsAccountType.Normal, true));
                                }
                            }
                        }
                        finally
                        {
                            if (contextToken != null)
                            {
                                contextToken.Close();
                            }
                        }
                        newContext = authentication3;
                        if (str4 != null)
                        {
                            challenge = "Digest " + str4;
                        }
                        goto Label_0783;
                    }
                    case System.Net.AuthenticationSchemes.Negotiate:
                    case System.Net.AuthenticationSchemes.Ntlm:
                        str6 = (none == System.Net.AuthenticationSchemes.Ntlm) ? "NTLM" : "Negotiate";
                        if ((digestContext == null) || (digestContext.Package != str6))
                        {
                            goto Label_0561;
                        }
                        authentication3 = digestContext;
                        goto Label_058B;

                    case System.Net.AuthenticationSchemes.Basic:
                        try
                        {
                            bytes = Convert.FromBase64String(incomingBlob);
                            incomingBlob = WebHeaderCollection.HeaderEncoding.GetString(bytes, 0, bytes.Length);
                            length = incomingBlob.IndexOf(':');
                            if (length != -1)
                            {
                                string username = incomingBlob.Substring(0, length);
                                string password = incomingBlob.Substring(length + 1);
                                principal = new GenericPrincipal(new HttpListenerBasicIdentity(username, password), null);
                            }
                            else
                            {
                                internalServerError = HttpStatusCode.BadRequest;
                            }
                        }
                        catch (FormatException)
                        {
                        }
                        goto Label_0783;

                    default:
                        goto Label_0783;
                }
                internalServerError = this.HttpStatusFromSecurityStatus(invalidToken);
                goto Label_0783;
            Label_0528:
                internalServerError = HttpStatusCode.Unauthorized;
                goto Label_0783;
            Label_0561:
                binding = this.GetChannelBinding(connectionId, isSecureConnection, extendedProtectionPolicy, out extendedProtectionFailure);
                if (!extendedProtectionFailure)
                {
                    authentication3 = new NTAuthentication(true, str6, null, this.GetContextFlags(extendedProtectionPolicy, isSecureConnection), binding);
                }
            Label_058B:
                if (!extendedProtectionFailure)
                {
                    try
                    {
                        bytes = Convert.FromBase64String(incomingBlob);
                    }
                    catch (FormatException)
                    {
                        internalServerError = HttpStatusCode.BadRequest;
                        flag2 = true;
                    }
                    if (!flag2)
                    {
                        inArray = authentication3.GetOutgoingBlob(bytes, false, out invalidToken);
                        flag2 = !authentication3.IsValidContext;
                        if (flag2)
                        {
                            if (((invalidToken == SecurityStatus.InvalidHandle) && (digestContext == null)) && ((bytes != null) && (bytes.Length > 0)))
                            {
                                invalidToken = SecurityStatus.InvalidToken;
                            }
                            internalServerError = this.HttpStatusFromSecurityStatus(invalidToken);
                        }
                    }
                    if (inArray != null)
                    {
                        str4 = Convert.ToBase64String(inArray);
                    }
                    if (!flag2)
                    {
                        if (authentication3.IsCompleted)
                        {
                            SafeCloseHandle handle2 = null;
                            try
                            {
                                if (!this.CheckSpn(authentication3, isSecureConnection, extendedProtectionPolicy))
                                {
                                    internalServerError = HttpStatusCode.Unauthorized;
                                }
                                else
                                {
                                    context2.Request.ServiceName = authentication3.ClientSpecifiedSpn;
                                    handle2 = authentication3.GetContextToken(out invalidToken);
                                    if (invalidToken != SecurityStatus.OK)
                                    {
                                        internalServerError = this.HttpStatusFromSecurityStatus(invalidToken);
                                    }
                                    else
                                    {
                                        WindowsPrincipal principal3 = new WindowsPrincipal(this.CreateWindowsIdentity(handle2.DangerousGetHandle(), authentication3.ProtocolName, WindowsAccountType.Normal, true));
                                        principal = principal3;
                                        if (this.UnsafeConnectionNtlmAuthentication && (authentication3.ProtocolName == "NTLM"))
                                        {
                                            if (disconnectResult == null)
                                            {
                                                this.RegisterForDisconnectNotification(connectionId, ref disconnectResult);
                                            }
                                            if (disconnectResult != null)
                                            {
                                                lock (this.DisconnectResults.SyncRoot)
                                                {
                                                    if (this.UnsafeConnectionNtlmAuthentication)
                                                    {
                                                        disconnectResult.AuthenticatedConnection = principal3;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                goto Label_0783;
                            }
                            finally
                            {
                                if (handle2 != null)
                                {
                                    handle2.Close();
                                }
                            }
                        }
                        newContext = authentication3;
                        challenge = (none == System.Net.AuthenticationSchemes.Ntlm) ? "NTLM" : "Negotiate";
                        if (!string.IsNullOrEmpty(str4))
                        {
                            challenge = challenge + " " + str4;
                        }
                    }
                }
                else
                {
                    internalServerError = HttpStatusCode.Unauthorized;
                }
            Label_0783:
                if (principal != null)
                {
                    context2.SetIdentity(principal, str4);
                }
                else
                {
                    if (Logging.On)
                    {
                        Logging.PrintWarning(Logging.HttpListener, this, "HandleAuthentication", SR.GetString("net_log_listener_create_valid_identity_failed"));
                    }
                    context2.Request.DetachBlob(memoryBlob);
                    context2.Close();
                    context2 = null;
                }
            Label_07CC:
                list = null;
                if (context2 == null)
                {
                    if (challenge != null)
                    {
                        AddChallenge(ref list, challenge);
                    }
                    else
                    {
                        if (newContext != null)
                        {
                            if (newContext == authentication3)
                            {
                                authentication3 = null;
                            }
                            if (newContext != digestContext)
                            {
                                NTAuthentication authentication4 = newContext;
                                newContext = null;
                                authentication4.CloseContext();
                            }
                            else
                            {
                                newContext = null;
                            }
                        }
                        if (internalServerError != HttpStatusCode.Unauthorized)
                        {
                            this.SendError(requestId, internalServerError, null);
                            return null;
                        }
                        list = this.BuildChallenge(authenticationSchemes, connectionId, out newContext, extendedProtectionPolicy, isSecureConnection);
                    }
                }
                if ((disconnectResult == null) && (newContext != null))
                {
                    this.RegisterForDisconnectNotification(connectionId, ref disconnectResult);
                    if (disconnectResult == null)
                    {
                        if (newContext != null)
                        {
                            if (newContext == authentication3)
                            {
                                authentication3 = null;
                            }
                            if (newContext != digestContext)
                            {
                                NTAuthentication authentication5 = newContext;
                                newContext = null;
                                authentication5.CloseContext();
                            }
                            else
                            {
                                newContext = null;
                            }
                        }
                        this.SendError(requestId, HttpStatusCode.InternalServerError, null);
                        context2.Request.DetachBlob(memoryBlob);
                        context2.Close();
                        return null;
                    }
                }
                if (digestContext != newContext)
                {
                    if (digestContext == authentication3)
                    {
                        authentication3 = null;
                    }
                    NTAuthentication authentication6 = digestContext;
                    digestContext = newContext;
                    disconnectResult.Session = newContext;
                    if (authentication6 != null)
                    {
                        if ((authenticationSchemes & System.Net.AuthenticationSchemes.Digest) != System.Net.AuthenticationSchemes.None)
                        {
                            this.SaveDigestContext(authentication6);
                        }
                        else
                        {
                            authentication6.CloseContext();
                        }
                    }
                }
                if (context2 == null)
                {
                    this.SendError(requestId, ((list != null) && (list.Count > 0)) ? HttpStatusCode.Unauthorized : HttpStatusCode.Forbidden, list);
                    return null;
                }
                if (!stoleBlob)
                {
                    stoleBlob = true;
                    context2.Request.ReleasePins();
                }
                context3 = context2;
            }
            catch
            {
                if (context2 != null)
                {
                    context2.Request.DetachBlob(memoryBlob);
                    context2.Close();
                }
                if (newContext != null)
                {
                    if (newContext == authentication3)
                    {
                        authentication3 = null;
                    }
                    if (newContext != digestContext)
                    {
                        NTAuthentication authentication7 = newContext;
                        newContext = null;
                        authentication7.CloseContext();
                    }
                    else
                    {
                        newContext = null;
                    }
                }
                throw;
            }
            finally
            {
                try
                {
                    if ((digestContext != null) && (digestContext != newContext))
                    {
                        if ((newContext == null) && (disconnectResult != null))
                        {
                            disconnectResult.Session = null;
                        }
                        if ((authenticationSchemes & System.Net.AuthenticationSchemes.Digest) != System.Net.AuthenticationSchemes.None)
                        {
                            this.SaveDigestContext(digestContext);
                        }
                        else
                        {
                            digestContext.CloseContext();
                        }
                    }
                    if (((authentication3 != null) && (digestContext != authentication3)) && (newContext != authentication3))
                    {
                        authentication3.CloseContext();
                    }
                }
                finally
                {
                    if (disconnectResult != null)
                    {
                        disconnectResult.FinishOwningDisconnectHandling();
                    }
                }
            }
            return context3;
        }

        private HttpStatusCode HttpStatusFromSecurityStatus(SecurityStatus status)
        {
            if (NclUtilities.IsCredentialFailure(status))
            {
                return HttpStatusCode.Unauthorized;
            }
            if (NclUtilities.IsClientFault(status))
            {
                return HttpStatusCode.BadRequest;
            }
            return HttpStatusCode.InternalServerError;
        }

        private unsafe uint InternalAddPrefix(string uriPrefix)
        {
            uint num = 0;
            fixed (char* str = ((char*) uriPrefix))
            {
                char* chPtr = str;
                num = UnsafeNclNativeMethods.HttpApi.HttpAddUrl(this.m_RequestQueueHandle, (ushort*) chPtr, null);
            }
            return num;
        }

        private unsafe bool InternalRemovePrefix(string uriPrefix)
        {
            uint num = 0;
            fixed (char* str = ((char*) uriPrefix))
            {
                char* chPtr = str;
                num = UnsafeNclNativeMethods.HttpApi.HttpRemoveUrl(this.m_RequestQueueHandle, (ushort*) chPtr);
            }
            if (num == 0x490)
            {
                return false;
            }
            return true;
        }

        private unsafe void RegisterForDisconnectNotification(ulong connectionId, ref DisconnectAsyncResult disconnectResult)
        {
            try
            {
                DisconnectAsyncResult result = new DisconnectAsyncResult(this, connectionId);
                this.EnsureBoundHandle();
                switch (UnsafeNclNativeMethods.HttpApi.HttpWaitForDisconnect(this.m_RequestQueueHandle, connectionId, result.NativeOverlapped))
                {
                    case 0:
                    case 0x3e5:
                        disconnectResult = result;
                        this.DisconnectResults[connectionId] = disconnectResult;
                        break;
                }
            }
            catch (Win32Exception exception)
            {
                int nativeErrorCode = exception.NativeErrorCode;
            }
        }

        internal void RemoveAll(bool clear)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "RemoveAll", "");
            }
            try
            {
                this.CheckDisposed();
                if (this.m_UriPrefixes.Count > 0)
                {
                    if (this.m_State == State.Started)
                    {
                        foreach (string str in this.m_UriPrefixes.Values)
                        {
                            this.InternalRemovePrefix(str);
                        }
                    }
                    if (clear)
                    {
                        this.m_UriPrefixes.Clear();
                        this.m_DefaultServiceNames.Clear();
                    }
                }
            }
            finally
            {
                if (Logging.On)
                {
                    Logging.Exit(Logging.HttpListener, this, "RemoveAll", "");
                }
            }
        }

        internal bool RemovePrefix(string uriPrefix)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "RemovePrefix", "uriPrefix:" + uriPrefix);
            }
            try
            {
                this.CheckDisposed();
                if (uriPrefix == null)
                {
                    throw new ArgumentNullException("uriPrefix");
                }
                if (!this.m_UriPrefixes.Contains(uriPrefix))
                {
                    return false;
                }
                if (this.m_State == State.Started)
                {
                    this.InternalRemovePrefix((string) this.m_UriPrefixes[uriPrefix]);
                }
                this.m_UriPrefixes.Remove(uriPrefix);
                this.m_DefaultServiceNames.Remove(uriPrefix);
            }
            catch (Exception exception)
            {
                if (Logging.On)
                {
                    Logging.Exception(Logging.HttpListener, this, "RemovePrefix", exception);
                }
                throw;
            }
            finally
            {
                if (Logging.On)
                {
                    Logging.Exit(Logging.HttpListener, this, "RemovePrefix", "uriPrefix:" + uriPrefix);
                }
            }
            return true;
        }

        private void SaveDigestContext(NTAuthentication digestContext)
        {
            if (this.m_SavedDigests == null)
            {
                Interlocked.CompareExchange<DigestContext[]>(ref this.m_SavedDigests, new DigestContext[0x400], null);
            }
            NTAuthentication context = null;
            ArrayList extraSavedDigestsBaking = null;
            lock (this.m_SavedDigests)
            {
                int num;
                if (!this.IsListening)
                {
                    digestContext.CloseContext();
                    return;
                }
                num = ((num = Environment.TickCount) == 0) ? 1 : num;
                this.m_NewestContext = (this.m_NewestContext + 1) & 0x3ff;
                int timestamp = this.m_SavedDigests[this.m_NewestContext].timestamp;
                context = this.m_SavedDigests[this.m_NewestContext].context;
                this.m_SavedDigests[this.m_NewestContext].timestamp = num;
                this.m_SavedDigests[this.m_NewestContext].context = digestContext;
                if (this.m_OldestContext == this.m_NewestContext)
                {
                    this.m_OldestContext = (this.m_NewestContext + 1) & 0x3ff;
                }
                while (((num - this.m_SavedDigests[this.m_OldestContext].timestamp) >= 300) && (this.m_SavedDigests[this.m_OldestContext].context != null))
                {
                    if (extraSavedDigestsBaking == null)
                    {
                        extraSavedDigestsBaking = new ArrayList();
                    }
                    extraSavedDigestsBaking.Add(this.m_SavedDigests[this.m_OldestContext].context);
                    this.m_SavedDigests[this.m_OldestContext].context = null;
                    this.m_OldestContext = (this.m_OldestContext + 1) & 0x3ff;
                }
                if ((context != null) && ((num - timestamp) <= 0x2710))
                {
                    if ((this.m_ExtraSavedDigests == null) || ((num - this.m_ExtraSavedDigestsTimestamp) > 0x2710))
                    {
                        extraSavedDigestsBaking = this.m_ExtraSavedDigestsBaking;
                        this.m_ExtraSavedDigestsBaking = this.m_ExtraSavedDigests;
                        this.m_ExtraSavedDigestsTimestamp = num;
                        this.m_ExtraSavedDigests = new ArrayList();
                    }
                    this.m_ExtraSavedDigests.Add(context);
                    context = null;
                }
            }
            if (context != null)
            {
                context.CloseContext();
            }
            if (extraSavedDigestsBaking != null)
            {
                for (int i = 0; i < extraSavedDigestsBaking.Count; i++)
                {
                    ((NTAuthentication) extraSavedDigestsBaking[i]).CloseContext();
                }
            }
        }

        private static bool ScenarioChecksChannelBinding(bool isSecureConnection, ProtectionScenario scenario) => 
            (isSecureConnection && (scenario == ProtectionScenario.TransportSelected));

        private unsafe void SendError(ulong requestId, HttpStatusCode httpStatusCode, ArrayList challenges)
        {
            uint num2;
            byte[] buffer4;
            UnsafeNclNativeMethods.HttpApi.HTTP_RESPONSE pHttpResponse = new UnsafeNclNativeMethods.HttpApi.HTTP_RESPONSE {
                Version = new UnsafeNclNativeMethods.HttpApi.HTTP_VERSION()
            };
            pHttpResponse.Version.MajorVersion = 1;
            pHttpResponse.Version.MinorVersion = 1;
            pHttpResponse.StatusCode = (ushort) httpStatusCode;
            string statusDescription = HttpListenerResponse.GetStatusDescription((int) httpStatusCode);
            uint pBytesSent = 0;
            byte[] bytes = Encoding.Default.GetBytes(statusDescription);
            if (((buffer4 = bytes) == null) || (buffer4.Length == 0))
            {
                numRef = null;
                goto Label_006B;
            }
            fixed (byte* numRef = buffer4)
            {
                byte[] buffer5;
            Label_006B:
                pHttpResponse.pReason = (sbyte*) numRef;
                pHttpResponse.ReasonLength = (ushort) bytes.Length;
                byte[] buffer2 = Encoding.Default.GetBytes("0");
                if (((buffer5 = buffer2) == null) || (buffer5.Length == 0))
                {
                    numRef2 = null;
                    goto Label_00AF;
                }
                fixed (byte* numRef2 = buffer5)
                {
                Label_00AF:
                    &pHttpResponse.Headers.KnownHeaders[11].pRawValue = (sbyte*) numRef2;
                    &pHttpResponse.Headers.KnownHeaders[11].RawValueLength = (ushort) buffer2.Length;
                    pHttpResponse.Headers.UnknownHeaderCount = (challenges == null) ? ((ushort) 0) : ((ushort) challenges.Count);
                    GCHandle[] handleArray = null;
                    UnsafeNclNativeMethods.HttpApi.HTTP_UNKNOWN_HEADER[] http_unknown_headerArray = null;
                    GCHandle handle = new GCHandle();
                    GCHandle handle2 = new GCHandle();
                    if (pHttpResponse.Headers.UnknownHeaderCount > 0)
                    {
                        handleArray = new GCHandle[pHttpResponse.Headers.UnknownHeaderCount];
                        http_unknown_headerArray = new UnsafeNclNativeMethods.HttpApi.HTTP_UNKNOWN_HEADER[pHttpResponse.Headers.UnknownHeaderCount];
                    }
                    try
                    {
                        if (pHttpResponse.Headers.UnknownHeaderCount > 0)
                        {
                            handle = GCHandle.Alloc(http_unknown_headerArray, GCHandleType.Pinned);
                            pHttpResponse.Headers.pUnknownHeaders = (UnsafeNclNativeMethods.HttpApi.HTTP_UNKNOWN_HEADER*) Marshal.UnsafeAddrOfPinnedArrayElement(http_unknown_headerArray, 0);
                            handle2 = GCHandle.Alloc(s_WwwAuthenticateBytes, GCHandleType.Pinned);
                            sbyte* numPtr = (sbyte*) Marshal.UnsafeAddrOfPinnedArrayElement(s_WwwAuthenticateBytes, 0);
                            for (int i = 0; i < handleArray.Length; i++)
                            {
                                byte[] buffer3 = Encoding.Default.GetBytes((string) challenges[i]);
                                handleArray[i] = GCHandle.Alloc(buffer3, GCHandleType.Pinned);
                                http_unknown_headerArray[i].pName = numPtr;
                                http_unknown_headerArray[i].NameLength = (ushort) s_WwwAuthenticateBytes.Length;
                                http_unknown_headerArray[i].pRawValue = (sbyte*) Marshal.UnsafeAddrOfPinnedArrayElement(buffer3, 0);
                                http_unknown_headerArray[i].RawValueLength = (ushort) buffer3.Length;
                            }
                        }
                        num2 = UnsafeNclNativeMethods.HttpApi.HttpSendHttpResponse(this.m_RequestQueueHandle, requestId, 0, &pHttpResponse, null, &pBytesSent, SafeLocalFree.Zero, 0, null, null);
                    }
                    finally
                    {
                        if (handle.IsAllocated)
                        {
                            handle.Free();
                        }
                        if (handle2.IsAllocated)
                        {
                            handle2.Free();
                        }
                        if (handleArray != null)
                        {
                            for (int j = 0; j < handleArray.Length; j++)
                            {
                                if (handleArray[j].IsAllocated)
                                {
                                    handleArray[j].Free();
                                }
                            }
                        }
                    }
                }
            }
            if (num2 != 0)
            {
                HttpListenerContext.CancelRequest(this.m_RequestQueueHandle, requestId);
            }
        }

        public void Start()
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "Start", "");
            }
            try
            {
                this.CheckDisposed();
                if (this.m_State != State.Started)
                {
                    this.m_RequestQueueHandle = SafeCloseHandle.CreateRequestQueueHandle();
                    this.AddAll();
                    this.m_State = State.Started;
                }
            }
            catch (Exception exception)
            {
                if (Logging.On)
                {
                    Logging.Exception(Logging.HttpListener, this, "Start", exception);
                }
                throw;
            }
            finally
            {
                if (Logging.On)
                {
                    Logging.Exit(Logging.HttpListener, this, "Start", "");
                }
            }
        }

        public void Stop()
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "Stop", "");
            }
            try
            {
                this.CheckDisposed();
                if (this.m_State != State.Stopped)
                {
                    this.RemoveAll(false);
                    this.m_RequestQueueHandle.Close();
                    this.m_RequestHandleBound = false;
                    this.m_State = State.Stopped;
                    this.ClearDigestCache();
                }
            }
            catch (Exception exception)
            {
                if (Logging.On)
                {
                    Logging.Exception(Logging.HttpListener, this, "Stop", exception);
                }
                throw;
            }
            finally
            {
                if (Logging.On)
                {
                    Logging.Exit(Logging.HttpListener, this, "Stop", "");
                }
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        public System.Net.AuthenticationSchemes AuthenticationSchemes
        {
            get => 
                this.m_AuthenticationScheme;
            set
            {
                this.CheckDisposed();
                if ((value & (System.Net.AuthenticationSchemes.IntegratedWindowsAuthentication | System.Net.AuthenticationSchemes.Digest)) != System.Net.AuthenticationSchemes.None)
                {
                    new SecurityPermission(SecurityPermissionFlag.ControlPrincipal).Demand();
                }
                this.m_AuthenticationScheme = value;
            }
        }

        public AuthenticationSchemeSelector AuthenticationSchemeSelectorDelegate
        {
            get
            {
                AuthenticationSelectorInfo authenticationDelegate = this.m_AuthenticationDelegate;
                if (authenticationDelegate != null)
                {
                    return authenticationDelegate.Delegate;
                }
                return null;
            }
            set
            {
                this.CheckDisposed();
                try
                {
                    new SecurityPermission(SecurityPermissionFlag.ControlPrincipal).Demand();
                    this.m_AuthenticationDelegate = new AuthenticationSelectorInfo(value, true);
                }
                catch (SecurityException exception)
                {
                    this.m_SecurityException = exception;
                    this.m_AuthenticationDelegate = new AuthenticationSelectorInfo(value, false);
                }
            }
        }

        public ServiceNameCollection DefaultServiceNames =>
            this.m_DefaultServiceNames.ServiceNames;

        private Hashtable DisconnectResults
        {
            get
            {
                if (this.m_DisconnectResults == null)
                {
                    lock (this.m_InternalLock)
                    {
                        if (this.m_DisconnectResults == null)
                        {
                            this.m_DisconnectResults = Hashtable.Synchronized(new Hashtable());
                        }
                    }
                }
                return this.m_DisconnectResults;
            }
        }

        public System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy ExtendedProtectionPolicy
        {
            get => 
                this.m_ExtendedProtectionPolicy;
            set
            {
                this.CheckDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (!AuthenticationManager.OSSupportsExtendedProtection && (value.PolicyEnforcement == PolicyEnforcement.Always))
                {
                    throw new PlatformNotSupportedException(SR.GetString("security_ExtendedProtection_NoOSSupport"));
                }
                if (value.CustomChannelBinding != null)
                {
                    throw new ArgumentException(SR.GetString("net_listener_cannot_set_custom_cbt"), "CustomChannelBinding");
                }
                this.m_ExtendedProtectionPolicy = value;
            }
        }

        public ExtendedProtectionSelector ExtendedProtectionSelectorDelegate
        {
            get => 
                this.m_ExtendedProtectionSelectorDelegate;
            set
            {
                this.CheckDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                if (!AuthenticationManager.OSSupportsExtendedProtection)
                {
                    throw new PlatformNotSupportedException(SR.GetString("security_ExtendedProtection_NoOSSupport"));
                }
                this.m_ExtendedProtectionSelectorDelegate = value;
            }
        }

        public bool IgnoreWriteExceptions
        {
            get => 
                this.m_IgnoreWriteExceptions;
            set
            {
                this.CheckDisposed();
                this.m_IgnoreWriteExceptions = value;
            }
        }

        public bool IsListening =>
            (this.m_State == State.Started);

        public static bool IsSupported =>
            UnsafeNclNativeMethods.HttpApi.Supported;

        public HttpListenerPrefixCollection Prefixes
        {
            get
            {
                if (Logging.On)
                {
                    Logging.Enter(Logging.HttpListener, this, "Prefixes_get", "");
                }
                this.CheckDisposed();
                if (this.m_Prefixes == null)
                {
                    this.m_Prefixes = new HttpListenerPrefixCollection(this);
                }
                return this.m_Prefixes;
            }
        }

        public string Realm
        {
            get => 
                this.m_Realm;
            set
            {
                this.CheckDisposed();
                this.m_Realm = value;
            }
        }

        internal SafeCloseHandle RequestQueueHandle =>
            this.m_RequestQueueHandle;

        public bool UnsafeConnectionNtlmAuthentication
        {
            get => 
                this.m_UnsafeConnectionNtlmAuthentication;
            set
            {
                this.CheckDisposed();
                if (this.m_UnsafeConnectionNtlmAuthentication != value)
                {
                    lock (this.DisconnectResults.SyncRoot)
                    {
                        if (this.m_UnsafeConnectionNtlmAuthentication != value)
                        {
                            this.m_UnsafeConnectionNtlmAuthentication = value;
                            if (!value)
                            {
                                foreach (DisconnectAsyncResult result in this.DisconnectResults.Values)
                                {
                                    result.AuthenticatedConnection = null;
                                }
                            }
                        }
                    }
                }
            }
        }

        private class AuthenticationSelectorInfo
        {
            private bool m_CanUseAdvancedAuth;
            private AuthenticationSchemeSelector m_SelectorDelegate;

            internal AuthenticationSelectorInfo(AuthenticationSchemeSelector selectorDelegate, bool canUseAdvancedAuth)
            {
                this.m_SelectorDelegate = selectorDelegate;
                this.m_CanUseAdvancedAuth = canUseAdvancedAuth;
            }

            internal bool AdvancedAuth =>
                this.m_CanUseAdvancedAuth;

            internal AuthenticationSchemeSelector Delegate =>
                this.m_SelectorDelegate;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DigestContext
        {
            internal NTAuthentication context;
            internal int timestamp;
        }

        private class DisconnectAsyncResult : IAsyncResult
        {
            private WindowsPrincipal m_AuthenticatedConnection;
            private ulong m_ConnectionId;
            private HttpListener m_HttpListener;
            private unsafe System.Threading.NativeOverlapped* m_NativeOverlapped;
            private int m_OwnershipState = 1;
            private NTAuthentication m_Session;
            internal const string NTLM = "NTLM";
            private static readonly IOCompletionCallback s_IOCallback = new IOCompletionCallback(HttpListener.DisconnectAsyncResult.WaitCallback);

            internal unsafe DisconnectAsyncResult(HttpListener httpListener, ulong connectionId)
            {
                this.m_HttpListener = httpListener;
                this.m_ConnectionId = connectionId;
                this.m_NativeOverlapped = new Overlapped { AsyncResult = this }.UnsafePack(s_IOCallback, null);
            }

            internal void FinishOwningDisconnectHandling()
            {
                if (Interlocked.CompareExchange(ref this.m_OwnershipState, 0, 1) == 2)
                {
                    this.HandleDisconnect();
                }
            }

            private void HandleDisconnect()
            {
                this.m_HttpListener.DisconnectResults.Remove(this.m_ConnectionId);
                if (this.m_Session != null)
                {
                    if (this.m_Session.Package == "WDigest")
                    {
                        this.m_HttpListener.SaveDigestContext(this.m_Session);
                    }
                    else
                    {
                        this.m_Session.CloseContext();
                    }
                }
                IDisposable disposable = (this.m_AuthenticatedConnection == null) ? null : (this.m_AuthenticatedConnection.Identity as IDisposable);
                if (((disposable != null) && (this.m_AuthenticatedConnection.Identity.AuthenticationType == "NTLM")) && this.m_HttpListener.UnsafeConnectionNtlmAuthentication)
                {
                    disposable.Dispose();
                }
                Interlocked.Exchange(ref this.m_OwnershipState, 3);
            }

            internal bool StartOwningDisconnectHandling()
            {
                int num;
                while ((num = Interlocked.CompareExchange(ref this.m_OwnershipState, 1, 0)) == 2)
                {
                    Thread.SpinWait(1);
                }
                return (num < 2);
            }

            private static unsafe void WaitCallback(uint errorCode, uint numBytes, System.Threading.NativeOverlapped* nativeOverlapped)
            {
                HttpListener.DisconnectAsyncResult asyncResult = (HttpListener.DisconnectAsyncResult) Overlapped.Unpack(nativeOverlapped).AsyncResult;
                Overlapped.Free(nativeOverlapped);
                if (Interlocked.Exchange(ref asyncResult.m_OwnershipState, 2) == 0)
                {
                    asyncResult.HandleDisconnect();
                }
            }

            public object AsyncState
            {
                get
                {
                    throw ExceptionHelper.PropertyNotImplementedException;
                }
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    throw ExceptionHelper.PropertyNotImplementedException;
                }
            }

            internal WindowsPrincipal AuthenticatedConnection
            {
                get => 
                    this.m_AuthenticatedConnection;
                set
                {
                    this.m_AuthenticatedConnection = value;
                }
            }

            public bool CompletedSynchronously
            {
                get
                {
                    throw ExceptionHelper.PropertyNotImplementedException;
                }
            }

            public bool IsCompleted
            {
                get
                {
                    throw ExceptionHelper.PropertyNotImplementedException;
                }
            }

            internal System.Threading.NativeOverlapped* NativeOverlapped =>
                this.m_NativeOverlapped;

            internal NTAuthentication Session
            {
                get => 
                    this.m_Session;
                set
                {
                    this.m_Session = value;
                }
            }
        }

        public delegate ExtendedProtectionPolicy ExtendedProtectionSelector(HttpListenerRequest request);

        private enum State
        {
            Stopped,
            Started,
            Closed
        }
    }
}

