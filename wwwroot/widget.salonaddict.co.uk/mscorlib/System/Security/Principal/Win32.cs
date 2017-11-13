namespace System.Security.Principal
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal sealed class Win32
    {
        private static bool _ConvertStringSidToSidSupported;
        private static bool _LsaApisSupported;
        private static bool _LsaLookupNames2Supported;
        private static bool _WellKnownSidApisSupported;
        internal const int FALSE = 0;
        internal const int TRUE = 1;

        static Win32()
        {
            Win32Native.OSVERSIONINFO ver = new Win32Native.OSVERSIONINFO();
            if (!Win32Native.GetVersionEx(ver))
            {
                throw new SystemException(Environment.GetResourceString("InvalidOperation_GetVersion"));
            }
            if ((ver.PlatformId == 2) && (ver.MajorVersion >= 5))
            {
                _ConvertStringSidToSidSupported = true;
                _LsaApisSupported = true;
                if ((ver.MajorVersion > 5) || (ver.MinorVersion > 0))
                {
                    _LsaLookupNames2Supported = true;
                    _WellKnownSidApisSupported = true;
                }
                else
                {
                    _LsaLookupNames2Supported = false;
                    Win32Native.OSVERSIONINFOEX osversioninfoex = new Win32Native.OSVERSIONINFOEX();
                    if (!Win32Native.GetVersionEx(osversioninfoex))
                    {
                        throw new SystemException(Environment.GetResourceString("InvalidOperation_GetVersion"));
                    }
                    if (osversioninfoex.ServicePackMajor < 3)
                    {
                        _WellKnownSidApisSupported = false;
                    }
                    else
                    {
                        _WellKnownSidApisSupported = true;
                    }
                }
            }
            else
            {
                _LsaApisSupported = false;
                _LsaLookupNames2Supported = false;
                _ConvertStringSidToSidSupported = false;
                _WellKnownSidApisSupported = false;
            }
        }

        private Win32()
        {
        }

        internal static byte[] ConvertIntPtrSidToByteArraySid(IntPtr binaryForm)
        {
            if (Marshal.ReadByte(binaryForm, 0) != SecurityIdentifier.Revision)
            {
                throw new ArgumentException(Environment.GetResourceString("IdentityReference_InvalidSidRevision"), "binaryForm");
            }
            byte num2 = Marshal.ReadByte(binaryForm, 1);
            if ((num2 < 0) || (num2 > SecurityIdentifier.MaxSubAuthorities))
            {
                throw new ArgumentException(Environment.GetResourceString("IdentityReference_InvalidNumberOfSubauthorities", new object[] { SecurityIdentifier.MaxSubAuthorities }), "binaryForm");
            }
            int length = 8 + (num2 * 4);
            byte[] destination = new byte[length];
            Marshal.Copy(binaryForm, destination, 0, length);
            return destination;
        }

        internal static int CreateSidFromString(string stringSid, out byte[] resultSid)
        {
            IntPtr zero = IntPtr.Zero;
            if (!SddlConversionSupported)
            {
                throw new PlatformNotSupportedException(Environment.GetResourceString("PlatformNotSupported_Win9x"));
            }
            try
            {
                if (1 != Win32Native.ConvertStringSidToSid(stringSid, out zero))
                {
                    int num = Marshal.GetLastWin32Error();
                    resultSid = null;
                    return num;
                }
                resultSid = ConvertIntPtrSidToByteArraySid(zero);
            }
            finally
            {
                Win32Native.LocalFree(zero);
            }
            return 0;
        }

        internal static int CreateWellKnownSid(WellKnownSidType sidType, SecurityIdentifier domainSid, out byte[] resultSid)
        {
            if (!WellKnownSidApisSupported)
            {
                throw new PlatformNotSupportedException(Environment.GetResourceString("PlatformNotSupported_RequiresW2kSP3"));
            }
            uint maxBinaryLength = (uint) SecurityIdentifier.MaxBinaryLength;
            resultSid = new byte[maxBinaryLength];
            if (Win32Native.CreateWellKnownSid((int) sidType, domainSid?.BinaryForm, resultSid, ref maxBinaryLength) != 0)
            {
                return 0;
            }
            resultSid = null;
            return Marshal.GetLastWin32Error();
        }

        internal static int GetWindowsAccountDomainSid(SecurityIdentifier sid, out SecurityIdentifier resultSid)
        {
            if (!WellKnownSidApisSupported)
            {
                throw new PlatformNotSupportedException(Environment.GetResourceString("PlatformNotSupported_RequiresW2kSP3"));
            }
            byte[] binaryForm = new byte[sid.BinaryLength];
            sid.GetBinaryForm(binaryForm, 0);
            uint maxBinaryLength = (uint) SecurityIdentifier.MaxBinaryLength;
            byte[] buffer2 = new byte[maxBinaryLength];
            if (Win32Native.GetWindowsAccountDomainSid(binaryForm, buffer2, ref maxBinaryLength) != 0)
            {
                resultSid = new SecurityIdentifier(buffer2, 0);
                return 0;
            }
            resultSid = null;
            return Marshal.GetLastWin32Error();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int ImpersonateLoggedOnUser(SafeTokenHandle hToken);
        internal static bool IsEqualDomainSid(SecurityIdentifier sid1, SecurityIdentifier sid2)
        {
            bool flag;
            if (!WellKnownSidApisSupported)
            {
                throw new PlatformNotSupportedException(Environment.GetResourceString("PlatformNotSupported_RequiresW2kSP3"));
            }
            if ((sid1 == null) || (sid2 == null))
            {
                return false;
            }
            byte[] binaryForm = new byte[sid1.BinaryLength];
            sid1.GetBinaryForm(binaryForm, 0);
            byte[] buffer2 = new byte[sid2.BinaryLength];
            sid2.GetBinaryForm(buffer2, 0);
            return ((Win32Native.IsEqualDomainSid(binaryForm, buffer2, out flag) != 0) && flag);
        }

        internal static bool IsWellKnownSid(SecurityIdentifier sid, WellKnownSidType type)
        {
            if (!WellKnownSidApisSupported)
            {
                throw new PlatformNotSupportedException(Environment.GetResourceString("PlatformNotSupported_RequiresW2kSP3"));
            }
            byte[] binaryForm = new byte[sid.BinaryLength];
            sid.GetBinaryForm(binaryForm, 0);
            if (Win32Native.IsWellKnownSid(binaryForm, (int) type) == 0)
            {
                return false;
            }
            return true;
        }

        internal static SafeLsaPolicyHandle LsaOpenPolicy(string systemName, PolicyRights rights)
        {
            SafeLsaPolicyHandle handle;
            Win32Native.LSA_OBJECT_ATTRIBUTES lsa_object_attributes;
            if (!LsaApisSupported)
            {
                throw new PlatformNotSupportedException(Environment.GetResourceString("PlatformNotSupported_Win9x"));
            }
            lsa_object_attributes.Length = Marshal.SizeOf(typeof(Win32Native.LSA_OBJECT_ATTRIBUTES));
            lsa_object_attributes.RootDirectory = IntPtr.Zero;
            lsa_object_attributes.ObjectName = IntPtr.Zero;
            lsa_object_attributes.Attributes = 0;
            lsa_object_attributes.SecurityDescriptor = IntPtr.Zero;
            lsa_object_attributes.SecurityQualityOfService = IntPtr.Zero;
            uint num = Win32Native.LsaOpenPolicy(systemName, ref lsa_object_attributes, (int) rights, out handle);
            if (num == 0)
            {
                return handle;
            }
            if (num == 0xc0000022)
            {
                throw new UnauthorizedAccessException();
            }
            if ((num != 0xc000009a) && (num != 0xc0000017))
            {
                throw new SystemException(Win32Native.GetMessage(Win32Native.LsaNtStatusToWinError((int) num)));
            }
            throw new OutOfMemoryException();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int OpenThreadToken(TokenAccessLevels dwDesiredAccess, WinSecurityContext OpenAs, out SafeTokenHandle phThreadToken);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int RevertToSelf();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int SetThreadToken(SafeTokenHandle hToken);

        internal static bool LsaApisSupported =>
            _LsaApisSupported;

        internal static bool LsaLookupNames2Supported =>
            _LsaLookupNames2Supported;

        internal static bool SddlConversionSupported =>
            _ConvertStringSidToSidSupported;

        internal static bool WellKnownSidApisSupported =>
            _WellKnownSidApisSupported;
    }
}

