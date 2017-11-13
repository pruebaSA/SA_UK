namespace System.Security.Principal
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.AccessControl;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;

    [Serializable, ComVisible(true)]
    public class WindowsIdentity : IIdentity, ISerializable, IDeserializationCallback, IDisposable
    {
        private string m_authType;
        private object m_groups;
        private int m_isAuthenticated;
        private string m_name;
        private SecurityIdentifier m_owner;
        private SafeTokenHandle m_safeTokenHandle;
        private SecurityIdentifier m_user;
        private static int s_runningOnWin2K = -1;

        private WindowsIdentity()
        {
            this.m_safeTokenHandle = SafeTokenHandle.InvalidHandle;
            this.m_isAuthenticated = -1;
        }

        internal WindowsIdentity(SafeTokenHandle safeTokenHandle) : this(safeTokenHandle.DangerousGetHandle())
        {
            GC.KeepAlive(safeTokenHandle);
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
        public WindowsIdentity(IntPtr userToken) : this(userToken, null, -1)
        {
        }

        private WindowsIdentity(SerializationInfo info)
        {
            this.m_safeTokenHandle = SafeTokenHandle.InvalidHandle;
            this.m_isAuthenticated = -1;
            IntPtr userToken = (IntPtr) info.GetValue("m_userToken", typeof(IntPtr));
            if (userToken != IntPtr.Zero)
            {
                this.CreateFromToken(userToken);
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
        public WindowsIdentity(string sUserPrincipalName) : this(sUserPrincipalName, null)
        {
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
        public WindowsIdentity(IntPtr userToken, string type) : this(userToken, type, -1)
        {
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
        public WindowsIdentity(SerializationInfo info, StreamingContext context) : this(info)
        {
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
        public WindowsIdentity(string sUserPrincipalName, string type)
        {
            this.m_safeTokenHandle = SafeTokenHandle.InvalidHandle;
            this.m_isAuthenticated = -1;
            this.m_safeTokenHandle = KerbS4ULogon(sUserPrincipalName);
        }

        private WindowsIdentity(IntPtr userToken, string authType, int isAuthenticated)
        {
            this.m_safeTokenHandle = SafeTokenHandle.InvalidHandle;
            this.m_isAuthenticated = -1;
            this.CreateFromToken(userToken);
            this.m_authType = authType;
            this.m_isAuthenticated = isAuthenticated;
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
        public WindowsIdentity(IntPtr userToken, string type, WindowsAccountType acctType) : this(userToken, type, -1)
        {
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
        public WindowsIdentity(IntPtr userToken, string type, WindowsAccountType acctType, bool isAuthenticated) : this(userToken, type, isAuthenticated ? 1 : 0)
        {
        }

        private void CreateFromToken(IntPtr userToken)
        {
            if (userToken == IntPtr.Zero)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_TokenZero"));
            }
            uint returnLength = (uint) Marshal.SizeOf(typeof(uint));
            Win32Native.GetTokenInformation(userToken, 8, SafeLocalAllocHandle.InvalidHandle, 0, out returnLength);
            if (Marshal.GetLastWin32Error() == 6)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidImpersonationToken"));
            }
            if (!Win32Native.DuplicateHandle(Win32Native.GetCurrentProcess(), userToken, Win32Native.GetCurrentProcess(), ref this.m_safeTokenHandle, 0, true, 2))
            {
                throw new SecurityException(Win32Native.GetMessage(Marshal.GetLastWin32Error()));
            }
        }

        [ComVisible(false)]
        public void Dispose()
        {
            this.Dispose(true);
        }

        [ComVisible(false)]
        protected virtual void Dispose(bool disposing)
        {
            if ((disposing && (this.m_safeTokenHandle != null)) && !this.m_safeTokenHandle.IsClosed)
            {
                this.m_safeTokenHandle.Dispose();
            }
            this.m_name = null;
            this.m_owner = null;
            this.m_user = null;
        }

        public static WindowsIdentity GetAnonymous() => 
            new WindowsIdentity();

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
        public static WindowsIdentity GetCurrent() => 
            GetCurrentInternal(TokenAccessLevels.MaximumAllowed, false);

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
        public static WindowsIdentity GetCurrent(bool ifImpersonating) => 
            GetCurrentInternal(TokenAccessLevels.MaximumAllowed, ifImpersonating);

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
        public static WindowsIdentity GetCurrent(TokenAccessLevels desiredAccess) => 
            GetCurrentInternal(desiredAccess, false);

        internal static WindowsIdentity GetCurrentInternal(TokenAccessLevels desiredAccess, bool threadOnly)
        {
            WindowsIdentity identity = null;
            bool flag;
            if (!RunningOnWin2K)
            {
                if (!threadOnly)
                {
                    identity = new WindowsIdentity {
                        m_name = string.Empty
                    };
                }
                return identity;
            }
            int hr = 0;
            SafeTokenHandle handle = GetCurrentToken(desiredAccess, threadOnly, out flag, out hr);
            if ((handle == null) || handle.IsInvalid)
            {
                if (!threadOnly || flag)
                {
                    throw new SecurityException(Win32Native.GetMessage(hr));
                }
                return identity;
            }
            identity = new WindowsIdentity();
            identity.m_safeTokenHandle.Dispose();
            identity.m_safeTokenHandle = handle;
            return identity;
        }

        private static SafeTokenHandle GetCurrentProcessToken(TokenAccessLevels desiredAccess, out int hr)
        {
            hr = 0;
            SafeTokenHandle invalidHandle = SafeTokenHandle.InvalidHandle;
            if (!Win32Native.OpenProcessToken(Win32Native.GetCurrentProcess(), desiredAccess, ref invalidHandle))
            {
                hr = GetHRForWin32Error(Marshal.GetLastWin32Error());
            }
            return invalidHandle;
        }

        internal static SafeTokenHandle GetCurrentThreadToken(TokenAccessLevels desiredAccess, out int hr)
        {
            SafeTokenHandle handle;
            hr = System.Security.Principal.Win32.OpenThreadToken(desiredAccess, WinSecurityContext.Both, out handle);
            return handle;
        }

        internal static WindowsIdentity GetCurrentThreadWI() => 
            SecurityContext.GetCurrentWI(Thread.CurrentThread.GetExecutionContextNoCreate());

        private static SafeTokenHandle GetCurrentToken(TokenAccessLevels desiredAccess, bool threadOnly, out bool isImpersonating, out int hr)
        {
            isImpersonating = true;
            SafeTokenHandle currentThreadToken = GetCurrentThreadToken(desiredAccess, out hr);
            if ((currentThreadToken == null) && (hr == GetHRForWin32Error(0x3f0)))
            {
                isImpersonating = false;
                if (!threadOnly)
                {
                    currentThreadToken = GetCurrentProcessToken(desiredAccess, out hr);
                }
            }
            return currentThreadToken;
        }

        private static Exception GetExceptionFromNtStatus(int status)
        {
            if (status == -1073741790)
            {
                return new UnauthorizedAccessException();
            }
            if ((status != -1073741670) && (status != -1073741801))
            {
                return new SecurityException(Win32Native.GetMessage(Win32Native.LsaNtStatusToWinError(status)));
            }
            return new OutOfMemoryException();
        }

        private static int GetHRForWin32Error(int dwLastError)
        {
            if ((dwLastError & 0x80000000L) == 0x80000000L)
            {
                return dwLastError;
            }
            return ((dwLastError & 0xffff) | -2147024896);
        }

        private static Win32Native.LUID GetLogonAuthId(SafeTokenHandle safeTokenHandle)
        {
            uint dwLength = 0;
            SafeLocalAllocHandle handle = GetTokenInformation(safeTokenHandle, TokenInformationClass.TokenStatistics, out dwLength);
            Win32Native.TOKEN_STATISTICS token_statistics = (Win32Native.TOKEN_STATISTICS) Marshal.PtrToStructure(handle.DangerousGetHandle(), typeof(Win32Native.TOKEN_STATISTICS));
            handle.Dispose();
            return token_statistics.AuthenticationId;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal string GetName()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            if (this.m_safeTokenHandle.IsInvalid)
            {
                return string.Empty;
            }
            if (this.m_name == null)
            {
                using (SafeImpersonate(SafeTokenHandle.InvalidHandle, null, ref lookForMyCaller))
                {
                    this.m_name = (this.User.Translate(typeof(NTAccount)) as NTAccount).ToString();
                }
            }
            return this.m_name;
        }

        private static SafeLocalAllocHandle GetTokenInformation(SafeTokenHandle tokenHandle, TokenInformationClass tokenInformationClass, out uint dwLength)
        {
            SafeLocalAllocHandle invalidHandle = SafeLocalAllocHandle.InvalidHandle;
            dwLength = (uint) Marshal.SizeOf(typeof(uint));
            bool flag = Win32Native.GetTokenInformation(tokenHandle, (uint) tokenInformationClass, invalidHandle, 0, out dwLength);
            int errorCode = Marshal.GetLastWin32Error();
            int num2 = errorCode;
            if (num2 == 6)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidImpersonationToken"));
            }
            if ((num2 != 0x18) && (num2 != 0x7a))
            {
                throw new SecurityException(Win32Native.GetMessage(errorCode));
            }
            IntPtr sizetdwBytes = new IntPtr((long) ((ulong) dwLength));
            invalidHandle.Dispose();
            invalidHandle = Win32Native.LocalAlloc(0, sizetdwBytes);
            if ((invalidHandle == null) || invalidHandle.IsInvalid)
            {
                throw new OutOfMemoryException();
            }
            if (!Win32Native.GetTokenInformation(tokenHandle, (uint) tokenInformationClass, invalidHandle, dwLength, out dwLength))
            {
                throw new SecurityException(Win32Native.GetMessage(Marshal.GetLastWin32Error()));
            }
            return invalidHandle;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual WindowsImpersonationContext Impersonate()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return this.Impersonate(ref lookForMyCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining), SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlPrincipal)]
        public static WindowsImpersonationContext Impersonate(IntPtr userToken)
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            if (userToken == IntPtr.Zero)
            {
                return SafeImpersonate(SafeTokenHandle.InvalidHandle, null, ref lookForMyCaller);
            }
            WindowsIdentity identity = new WindowsIdentity(userToken);
            return identity.Impersonate(ref lookForMyCaller);
        }

        internal WindowsImpersonationContext Impersonate(ref StackCrawlMark stackMark)
        {
            if (!RunningOnWin2K)
            {
                return new WindowsImpersonationContext(SafeTokenHandle.InvalidHandle, GetCurrentThreadWI(), false, null);
            }
            if (this.m_safeTokenHandle.IsInvalid)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_AnonymousCannotImpersonate"));
            }
            return SafeImpersonate(this.m_safeTokenHandle, this, ref stackMark);
        }

        private static unsafe SafeTokenHandle KerbS4ULogon(string upn)
        {
            int num;
            byte[] source = new byte[] { 0x43, 0x4c, 0x52 };
            IntPtr sizetdwBytes = new IntPtr((long) ((ulong) (source.Length + 1)));
            SafeLocalAllocHandle handle = Win32Native.LocalAlloc(0x40, sizetdwBytes);
            Marshal.Copy(source, 0, handle.DangerousGetHandle(), source.Length);
            Win32Native.UNICODE_INTPTR_STRING logonProcessName = new Win32Native.UNICODE_INTPTR_STRING(source.Length, source.Length + 1, handle.DangerousGetHandle());
            SafeLsaLogonProcessHandle invalidHandle = SafeLsaLogonProcessHandle.InvalidHandle;
            Privilege privilege = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                try
                {
                    privilege = new Privilege("SeTcbPrivilege");
                    privilege.Enable();
                }
                catch (PrivilegeNotHeldException)
                {
                }
                IntPtr zero = IntPtr.Zero;
                num = Win32Native.LsaRegisterLogonProcess(ref logonProcessName, ref invalidHandle, ref zero);
                if (5 == Win32Native.LsaNtStatusToWinError(num))
                {
                    num = Win32Native.LsaConnectUntrusted(ref invalidHandle);
                }
            }
            catch
            {
                if (privilege != null)
                {
                    privilege.Revert();
                }
                throw;
            }
            finally
            {
                if (privilege != null)
                {
                    privilege.Revert();
                }
            }
            if (num < 0)
            {
                throw GetExceptionFromNtStatus(num);
            }
            byte[] bytes = new byte["Kerberos".Length + 1];
            Encoding.ASCII.GetBytes("Kerberos", 0, "Kerberos".Length, bytes, 0);
            sizetdwBytes = new IntPtr((long) ((ulong) bytes.Length));
            SafeLocalAllocHandle handle3 = Win32Native.LocalAlloc(0, sizetdwBytes);
            if ((handle3 == null) || handle3.IsInvalid)
            {
                throw new OutOfMemoryException();
            }
            Marshal.Copy(bytes, 0, handle3.DangerousGetHandle(), bytes.Length);
            Win32Native.UNICODE_INTPTR_STRING packageName = new Win32Native.UNICODE_INTPTR_STRING("Kerberos".Length, "Kerberos".Length + 1, handle3.DangerousGetHandle());
            uint authenticationPackage = 0;
            num = Win32Native.LsaLookupAuthenticationPackage(invalidHandle, ref packageName, ref authenticationPackage);
            if (num < 0)
            {
                throw GetExceptionFromNtStatus(num);
            }
            Win32Native.TOKEN_SOURCE sourceContext = new Win32Native.TOKEN_SOURCE();
            if (!Win32Native.AllocateLocallyUniqueId(ref sourceContext.SourceIdentifier))
            {
                throw new SecurityException(Win32Native.GetMessage(Marshal.GetLastWin32Error()));
            }
            sourceContext.Name = new char[8];
            sourceContext.Name[0] = 'C';
            sourceContext.Name[1] = 'L';
            sourceContext.Name[2] = 'R';
            uint profileBufferLength = 0;
            SafeLsaReturnBufferHandle profileBuffer = SafeLsaReturnBufferHandle.InvalidHandle;
            Win32Native.LUID logonId = new Win32Native.LUID();
            Win32Native.QUOTA_LIMITS quotas = new Win32Native.QUOTA_LIMITS();
            int subStatus = 0;
            SafeTokenHandle token = SafeTokenHandle.InvalidHandle;
            int num5 = Marshal.SizeOf(typeof(Win32Native.KERB_S4U_LOGON)) + (2 * (upn.Length + 1));
            byte[] dst = new byte[num5];
            fixed (byte* numRef = dst)
            {
                byte[] buffer4 = new byte[2 * (upn.Length + 1)];
                Encoding.Unicode.GetBytes(upn, 0, upn.Length, buffer4, 0);
                Buffer.BlockCopy(buffer4, 0, dst, Marshal.SizeOf(typeof(Win32Native.KERB_S4U_LOGON)), buffer4.Length);
                Win32Native.KERB_S4U_LOGON* kerb_su_logonPtr = (Win32Native.KERB_S4U_LOGON*) numRef;
                kerb_su_logonPtr->MessageType = 12;
                kerb_su_logonPtr->Flags = 0;
                kerb_su_logonPtr->ClientUpn.Length = (ushort) (2 * upn.Length);
                kerb_su_logonPtr->ClientUpn.MaxLength = (ushort) (2 * (upn.Length + 1));
                kerb_su_logonPtr->ClientUpn.Buffer = new IntPtr((void*) (kerb_su_logonPtr + 1));
                num = Win32Native.LsaLogonUser(invalidHandle, ref logonProcessName, 3, authenticationPackage, new IntPtr((void*) numRef), (uint) dst.Length, IntPtr.Zero, ref sourceContext, ref profileBuffer, ref profileBufferLength, ref logonId, ref token, ref quotas, ref subStatus);
            }
            if ((num == -1073741714) && (subStatus < 0))
            {
                num = subStatus;
            }
            if (num < 0)
            {
                throw GetExceptionFromNtStatus(num);
            }
            if (subStatus < 0)
            {
                throw GetExceptionFromNtStatus(subStatus);
            }
            profileBuffer.Dispose();
            handle.Dispose();
            handle3.Dispose();
            invalidHandle.Dispose();
            return token;
        }

        internal static WindowsImpersonationContext SafeImpersonate(SafeTokenHandle userToken, WindowsIdentity wi, ref StackCrawlMark stackMark)
        {
            bool flag;
            if (!RunningOnWin2K)
            {
                return new WindowsImpersonationContext(SafeTokenHandle.InvalidHandle, GetCurrentThreadWI(), false, null);
            }
            int hr = 0;
            SafeTokenHandle safeTokenHandle = GetCurrentToken(TokenAccessLevels.MaximumAllowed, false, out flag, out hr);
            if ((safeTokenHandle == null) || safeTokenHandle.IsInvalid)
            {
                throw new SecurityException(Win32Native.GetMessage(hr));
            }
            FrameSecurityDescriptor securityObjectForFrame = SecurityRuntime.GetSecurityObjectForFrame(ref stackMark, true);
            if ((securityObjectForFrame == null) && SecurityManager._IsSecurityOn())
            {
                throw new SecurityException(Environment.GetResourceString("ExecutionEngine_MissingSecurityDescriptor"));
            }
            WindowsImpersonationContext context = new WindowsImpersonationContext(safeTokenHandle, GetCurrentThreadWI(), flag, securityObjectForFrame);
            if (userToken.IsInvalid)
            {
                hr = System.Security.Principal.Win32.RevertToSelf();
                if (hr < 0)
                {
                    throw new SecurityException(Win32Native.GetMessage(hr));
                }
                UpdateThreadWI(wi);
                securityObjectForFrame.SetTokenHandles(safeTokenHandle, wi?.TokenHandle);
                return context;
            }
            hr = System.Security.Principal.Win32.RevertToSelf();
            if (hr < 0)
            {
                throw new SecurityException(Win32Native.GetMessage(hr));
            }
            if (System.Security.Principal.Win32.ImpersonateLoggedOnUser(userToken) < 0)
            {
                context.Undo();
                throw new SecurityException(Environment.GetResourceString("Argument_ImpersonateUser"));
            }
            UpdateThreadWI(wi);
            securityObjectForFrame.SetTokenHandles(safeTokenHandle, wi?.TokenHandle);
            return context;
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_userToken", this.m_safeTokenHandle.DangerousGetHandle());
        }

        internal static void UpdateThreadWI(WindowsIdentity wi)
        {
            SecurityContext currentSecurityContextNoCreate = SecurityContext.GetCurrentSecurityContextNoCreate();
            if ((wi != null) && (currentSecurityContextNoCreate == null))
            {
                currentSecurityContextNoCreate = new SecurityContext();
                Thread.CurrentThread.ExecutionContext.SecurityContext = currentSecurityContextNoCreate;
            }
            if (currentSecurityContextNoCreate != null)
            {
                currentSecurityContextNoCreate.WindowsIdentity = wi;
            }
        }

        public string AuthenticationType
        {
            get
            {
                if (this.m_safeTokenHandle.IsInvalid)
                {
                    return string.Empty;
                }
                if (this.m_authType != null)
                {
                    return this.m_authType;
                }
                Win32Native.LUID logonAuthId = GetLogonAuthId(this.m_safeTokenHandle);
                if (logonAuthId.LowPart == 0x3e6)
                {
                    return string.Empty;
                }
                SafeLsaReturnBufferHandle invalidHandle = SafeLsaReturnBufferHandle.InvalidHandle;
                int status = Win32Native.LsaGetLogonSessionData(ref logonAuthId, ref invalidHandle);
                if (status < 0)
                {
                    throw GetExceptionFromNtStatus(status);
                }
                Win32Native.SECURITY_LOGON_SESSION_DATA security_logon_session_data = (Win32Native.SECURITY_LOGON_SESSION_DATA) Marshal.PtrToStructure(invalidHandle.DangerousGetHandle(), typeof(Win32Native.SECURITY_LOGON_SESSION_DATA));
                string str = Marshal.PtrToStringUni(security_logon_session_data.AuthenticationPackage.Buffer);
                invalidHandle.Dispose();
                return str;
            }
        }

        public IdentityReferenceCollection Groups
        {
            get
            {
                if (this.m_safeTokenHandle.IsInvalid)
                {
                    return null;
                }
                if (this.m_groups == null)
                {
                    IdentityReferenceCollection references = new IdentityReferenceCollection();
                    uint dwLength = 0;
                    using (SafeLocalAllocHandle handle = GetTokenInformation(this.m_safeTokenHandle, TokenInformationClass.TokenGroups, out dwLength))
                    {
                        int num2 = Marshal.ReadInt32(handle.DangerousGetHandle());
                        IntPtr ptr = new IntPtr(((long) handle.DangerousGetHandle()) + ((long) Marshal.OffsetOf(typeof(Win32Native.TOKEN_GROUPS), "Groups")));
                        for (int i = 0; i < num2; i++)
                        {
                            Win32Native.SID_AND_ATTRIBUTES sid_and_attributes = (Win32Native.SID_AND_ATTRIBUTES) Marshal.PtrToStructure(ptr, typeof(Win32Native.SID_AND_ATTRIBUTES));
                            uint num4 = 0xc0000014;
                            if ((sid_and_attributes.Attributes & num4) == 4)
                            {
                                references.Add(new SecurityIdentifier(sid_and_attributes.Sid, true));
                            }
                            ptr = new IntPtr(((long) ptr) + Marshal.SizeOf(typeof(Win32Native.SID_AND_ATTRIBUTES)));
                        }
                    }
                    Interlocked.CompareExchange(ref this.m_groups, references, null);
                }
                return (this.m_groups as IdentityReferenceCollection);
            }
        }

        [ComVisible(false)]
        public TokenImpersonationLevel ImpersonationLevel
        {
            get
            {
                if (this.m_safeTokenHandle.IsInvalid)
                {
                    return TokenImpersonationLevel.Anonymous;
                }
                uint dwLength = 0;
                SafeLocalAllocHandle handle = GetTokenInformation(this.m_safeTokenHandle, TokenInformationClass.TokenType, out dwLength);
                if (Marshal.ReadInt32(handle.DangerousGetHandle()) == 1)
                {
                    return TokenImpersonationLevel.None;
                }
                SafeLocalAllocHandle handle2 = GetTokenInformation(this.m_safeTokenHandle, TokenInformationClass.TokenImpersonationLevel, out dwLength);
                int num2 = Marshal.ReadInt32(handle2.DangerousGetHandle());
                handle.Dispose();
                handle2.Dispose();
                return (TokenImpersonationLevel) (num2 + 1);
            }
        }

        public virtual bool IsAnonymous
        {
            get
            {
                if (this.m_safeTokenHandle.IsInvalid)
                {
                    return true;
                }
                SecurityIdentifier identifier = new SecurityIdentifier(IdentifierAuthority.NTAuthority, new int[] { 7 });
                return (this.User == identifier);
            }
        }

        public virtual bool IsAuthenticated
        {
            get
            {
                if (!RunningOnWin2K)
                {
                    return false;
                }
                if (this.m_isAuthenticated == -1)
                {
                    WindowsPrincipal principal = new WindowsPrincipal(this);
                    SecurityIdentifier sid = new SecurityIdentifier(IdentifierAuthority.NTAuthority, new int[] { 11 });
                    this.m_isAuthenticated = principal.IsInRole(sid) ? 1 : 0;
                }
                return (this.m_isAuthenticated == 1);
            }
        }

        public virtual bool IsGuest
        {
            get
            {
                if (this.m_safeTokenHandle.IsInvalid)
                {
                    return false;
                }
                SecurityIdentifier identifier = new SecurityIdentifier(IdentifierAuthority.NTAuthority, new int[] { 0x20, 0x1f5 });
                return (this.User == identifier);
            }
        }

        public virtual bool IsSystem
        {
            get
            {
                if (this.m_safeTokenHandle.IsInvalid)
                {
                    return false;
                }
                SecurityIdentifier identifier = new SecurityIdentifier(IdentifierAuthority.NTAuthority, new int[] { 0x12 });
                return (this.User == identifier);
            }
        }

        public virtual string Name =>
            this.GetName();

        [ComVisible(false)]
        public SecurityIdentifier Owner
        {
            get
            {
                if (this.m_safeTokenHandle.IsInvalid)
                {
                    return null;
                }
                if (this.m_owner == null)
                {
                    uint dwLength = 0;
                    SafeLocalAllocHandle handle = GetTokenInformation(this.m_safeTokenHandle, TokenInformationClass.TokenOwner, out dwLength);
                    this.m_owner = new SecurityIdentifier(Marshal.ReadIntPtr(handle.DangerousGetHandle()), true);
                    handle.Dispose();
                }
                return this.m_owner;
            }
        }

        internal static bool RunningOnWin2K
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            get
            {
                if (s_runningOnWin2K == -1)
                {
                    Win32Native.OSVERSIONINFO ver = new Win32Native.OSVERSIONINFO();
                    s_runningOnWin2K = ((Win32Native.GetVersionEx(ver) && (ver.PlatformId == 2)) && (ver.MajorVersion >= 5)) ? 1 : 0;
                }
                return (s_runningOnWin2K == 1);
            }
        }

        public virtual IntPtr Token =>
            this.m_safeTokenHandle.DangerousGetHandle();

        internal SafeTokenHandle TokenHandle =>
            this.m_safeTokenHandle;

        [ComVisible(false)]
        public SecurityIdentifier User
        {
            get
            {
                if (this.m_safeTokenHandle.IsInvalid)
                {
                    return null;
                }
                if (this.m_user == null)
                {
                    uint dwLength = 0;
                    SafeLocalAllocHandle handle = GetTokenInformation(this.m_safeTokenHandle, TokenInformationClass.TokenUser, out dwLength);
                    this.m_user = new SecurityIdentifier(Marshal.ReadIntPtr(handle.DangerousGetHandle()), true);
                    handle.Dispose();
                }
                return this.m_user;
            }
        }
    }
}

