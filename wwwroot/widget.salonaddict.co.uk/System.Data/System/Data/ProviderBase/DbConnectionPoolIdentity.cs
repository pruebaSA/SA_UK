﻿namespace System.Data.ProviderBase
{
    using System;
    using System.Data.Common;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Security.Principal;

    [Serializable]
    internal sealed class DbConnectionPoolIdentity
    {
        private readonly bool _isNetwork;
        private readonly bool _isRestricted;
        private readonly string _sidString;
        private const int E_NotImpersonationToken = -2147023587;
        private static readonly byte[] NetworkSid = (ADP.IsWindowsNT ? CreateWellKnownSid(WellKnownSidType.NetworkSid) : null);
        public static readonly DbConnectionPoolIdentity NoIdentity = new DbConnectionPoolIdentity(string.Empty, false, true);
        private const int Win32_CheckTokenMembership = 1;
        private const int Win32_ConvertSidToStringSidW = 4;
        private const int Win32_CreateWellKnownSid = 5;
        private const int Win32_GetTokenInformation_1 = 2;
        private const int Win32_GetTokenInformation_2 = 3;

        private DbConnectionPoolIdentity(string sidString, bool isRestricted, bool isNetwork)
        {
            this._sidString = sidString;
            this._isRestricted = isRestricted;
            this._isNetwork = isNetwork;
        }

        private static byte[] CreateWellKnownSid(WellKnownSidType sidType)
        {
            uint maxBinaryLength = (uint) SecurityIdentifier.MaxBinaryLength;
            byte[] resultSid = new byte[maxBinaryLength];
            if (System.Data.Common.UnsafeNativeMethods.CreateWellKnownSid((int) sidType, null, resultSid, ref maxBinaryLength) == 0)
            {
                IntegratedSecurityError(5);
            }
            return resultSid;
        }

        public override bool Equals(object value)
        {
            bool flag = (this == NoIdentity) || (this == value);
            if (!flag && (value != null))
            {
                DbConnectionPoolIdentity identity = (DbConnectionPoolIdentity) value;
                flag = ((this._sidString == identity._sidString) && (this._isRestricted == identity._isRestricted)) && (this._isNetwork == identity._isNetwork);
            }
            return flag;
        }

        internal static DbConnectionPoolIdentity GetCurrent()
        {
            if (!ADP.IsWindowsNT)
            {
                return NoIdentity;
            }
            WindowsIdentity currentWindowsIdentity = GetCurrentWindowsIdentity();
            IntPtr windowsIdentityToken = GetWindowsIdentityToken(currentWindowsIdentity);
            uint tokenInformationLength = 0x800;
            uint tokenString = 0;
            IntPtr zero = IntPtr.Zero;
            IntPtr stringSid = IntPtr.Zero;
            System.Data.Common.UnsafeNativeMethods.SetLastError(0);
            bool isRestricted = System.Data.Common.UnsafeNativeMethods.IsTokenRestricted(windowsIdentityToken);
            if (Marshal.GetLastWin32Error() != 0)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            DbConnectionPoolIdentity identity = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                bool flag;
                if (!System.Data.Common.UnsafeNativeMethods.CheckTokenMembership(windowsIdentityToken, NetworkSid, out flag))
                {
                    IntegratedSecurityError(1);
                }
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    zero = SafeNativeMethods.LocalAlloc(0, (IntPtr) tokenInformationLength);
                }
                if (IntPtr.Zero == zero)
                {
                    throw new OutOfMemoryException();
                }
                if (!System.Data.Common.UnsafeNativeMethods.GetTokenInformation(windowsIdentityToken, 1, zero, tokenInformationLength, ref tokenString))
                {
                    if (tokenString > tokenInformationLength)
                    {
                        tokenInformationLength = tokenString;
                        RuntimeHelpers.PrepareConstrainedRegions();
                        try
                        {
                        }
                        finally
                        {
                            SafeNativeMethods.LocalFree(zero);
                            zero = IntPtr.Zero;
                            zero = SafeNativeMethods.LocalAlloc(0, (IntPtr) tokenInformationLength);
                        }
                        if (IntPtr.Zero == zero)
                        {
                            throw new OutOfMemoryException();
                        }
                        if (!System.Data.Common.UnsafeNativeMethods.GetTokenInformation(windowsIdentityToken, 1, zero, tokenInformationLength, ref tokenString))
                        {
                            IntegratedSecurityError(2);
                        }
                    }
                    else
                    {
                        IntegratedSecurityError(3);
                    }
                }
                currentWindowsIdentity.Dispose();
                if (!System.Data.Common.UnsafeNativeMethods.ConvertSidToStringSidW(Marshal.ReadIntPtr(zero, 0), out stringSid))
                {
                    IntegratedSecurityError(4);
                }
                if (IntPtr.Zero == stringSid)
                {
                    throw ADP.InternalError(ADP.InternalErrorCode.ConvertSidToStringSidWReturnedNull);
                }
                identity = new DbConnectionPoolIdentity(Marshal.PtrToStringUni(stringSid), isRestricted, flag);
            }
            finally
            {
                if (IntPtr.Zero != zero)
                {
                    SafeNativeMethods.LocalFree(zero);
                    zero = IntPtr.Zero;
                }
                if (IntPtr.Zero != stringSid)
                {
                    SafeNativeMethods.LocalFree(stringSid);
                    stringSid = IntPtr.Zero;
                }
            }
            return identity;
        }

        [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.ControlPrincipal)]
        internal static WindowsIdentity GetCurrentWindowsIdentity() => 
            WindowsIdentity.GetCurrent();

        public override int GetHashCode() => 
            this._sidString?.GetHashCode();

        [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode)]
        private static IntPtr GetWindowsIdentityToken(WindowsIdentity identity) => 
            identity.Token;

        private static void IntegratedSecurityError(int caller)
        {
            int errorCode = Marshal.GetHRForLastWin32Error();
            if ((1 != caller) || (-2147023587 != errorCode))
            {
                Marshal.ThrowExceptionForHR(errorCode);
            }
        }

        internal bool IsNetwork =>
            this._isNetwork;

        internal bool IsRestricted =>
            this._isRestricted;
    }
}

