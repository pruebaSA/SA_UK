namespace System.Diagnostics
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.AccessControl;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;

    internal static class SharedUtils
    {
        private static int environment;
        internal const int NonNtEnvironment = 3;
        internal const int NtEnvironment = 2;
        private static object s_InternalSyncObject;
        internal const int UnknownEnvironment = 0;
        internal const int W2kEnvironment = 1;

        internal static void CheckEnvironment()
        {
            if (CurrentEnvironment == 3)
            {
                throw new PlatformNotSupportedException(SR.GetString("WinNTRequired"));
            }
        }

        internal static void CheckNtEnvironment()
        {
            if (CurrentEnvironment == 2)
            {
                throw new PlatformNotSupportedException(SR.GetString("Win2000Required"));
            }
        }

        internal static Win32Exception CreateSafeWin32Exception() => 
            CreateSafeWin32Exception(0);

        internal static Win32Exception CreateSafeWin32Exception(int error)
        {
            Win32Exception exception = null;
            new SecurityPermission(PermissionState.Unrestricted).Assert();
            try
            {
                if (error == 0)
                {
                    return new Win32Exception();
                }
                exception = new Win32Exception(error);
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            return exception;
        }

        internal static void EnterMutex(string name, ref Mutex mutex)
        {
            string mutexName = null;
            if (CurrentEnvironment == 1)
            {
                mutexName = @"Global\" + name;
            }
            else
            {
                mutexName = name;
            }
            EnterMutexWithoutGlobal(mutexName, ref mutex);
        }

        internal static void EnterMutexWithoutGlobal(string mutexName, ref Mutex mutex)
        {
            bool flag;
            MutexSecurity mutexSecurity = new MutexSecurity();
            SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
            mutexSecurity.AddAccessRule(new MutexAccessRule(identity, MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
            Mutex mutexIn = new Mutex(false, mutexName, out flag, mutexSecurity);
            SafeWaitForMutex(mutexIn, ref mutex);
        }

        internal static string GetLatestBuildDllDirectory(string machineName)
        {
            string str = "";
            RegistryKey key = null;
            RegistryKey key2 = null;
            new RegistryPermission(PermissionState.Unrestricted).Assert();
            try
            {
                if (machineName.Equals("."))
                {
                    return GetLocalBuildDirectory();
                }
                key = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machineName);
                if (key == null)
                {
                    throw new InvalidOperationException(SR.GetString("RegKeyMissingShort", new object[] { "HKEY_LOCAL_MACHINE", machineName }));
                }
                key2 = key.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework");
                if (key2 == null)
                {
                    return str;
                }
                string str2 = (string) key2.GetValue("InstallRoot");
                switch (str2)
                {
                    case null:
                    case string.Empty:
                        return str;
                }
                string str3 = null;
                str3 = "v" + Environment.Version.ToString(2);
                string strB = null;
                RegistryKey key3 = key2.OpenSubKey(@"policy\" + str3);
                if (key3 == null)
                {
                    return str;
                }
                try
                {
                    strB = (string) key3.GetValue("Version");
                    if (strB == null)
                    {
                        string[] valueNames = key3.GetValueNames();
                        for (int i = 0; i < valueNames.Length; i++)
                        {
                            string strA = str3 + "." + valueNames[i].Replace('-', '.');
                            if (string.Compare(strA, strB, StringComparison.Ordinal) > 0)
                            {
                                strB = strA;
                            }
                        }
                    }
                }
                finally
                {
                    key3.Close();
                }
                switch (strB)
                {
                    case null:
                    case string.Empty:
                        return str;
                }
                StringBuilder builder = new StringBuilder();
                builder.Append(str2);
                if (!str2.EndsWith(@"\", StringComparison.Ordinal))
                {
                    builder.Append(@"\");
                }
                builder.Append(strB);
                builder.Append(@"\");
                return builder.ToString();
            }
            catch
            {
            }
            finally
            {
                if (key2 != null)
                {
                    key2.Close();
                }
                if (key != null)
                {
                    key.Close();
                }
                CodeAccessPermission.RevertAssert();
            }
            return str;
        }

        private static string GetLocalBuildDirectory()
        {
            uint num3;
            uint num4;
            int capacity = 0x108;
            int num2 = 0x19;
            StringBuilder pDirectory = new StringBuilder(capacity);
            StringBuilder pVersion = new StringBuilder(num2);
            uint num5 = NativeMethods.GetRequestedRuntimeInfo(null, null, null, 0, 0x41, pDirectory, capacity, out num3, pVersion, num2, out num4);
            while (num5 == 0x7a)
            {
                capacity *= 2;
                num2 *= 2;
                pDirectory = new StringBuilder(capacity);
                pVersion = new StringBuilder(num2);
                num5 = NativeMethods.GetRequestedRuntimeInfo(null, null, null, 0, 0, pDirectory, capacity, out num3, pVersion, num2, out num4);
            }
            if (num5 != 0)
            {
                throw CreateSafeWin32Exception();
            }
            pDirectory.Append(pVersion);
            return pDirectory.ToString();
        }

        private static bool SafeWaitForMutex(Mutex mutexIn, ref Mutex mutexOut)
        {
            while (SafeWaitForMutexOnce(mutexIn, ref mutexOut))
            {
                if (mutexOut != null)
                {
                    return true;
                }
                Thread.Sleep(0);
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool SafeWaitForMutexOnce(Mutex mutexIn, ref Mutex mutexOut)
        {
            bool flag;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                Thread.BeginCriticalRegion();
                Thread.BeginThreadAffinity();
                switch (WaitForSingleObjectDontCallThis(mutexIn.SafeWaitHandle, 500))
                {
                    case 0:
                    case 0x80:
                        mutexOut = mutexIn;
                        flag = true;
                        break;

                    case 0x102:
                        flag = true;
                        break;

                    default:
                        flag = false;
                        break;
                }
                if (mutexOut == null)
                {
                    Thread.EndThreadAffinity();
                    Thread.EndCriticalRegion();
                }
            }
            return flag;
        }

        [SuppressUnmanagedCodeSecurity, ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("kernel32.dll", EntryPoint="WaitForSingleObject", SetLastError=true, ExactSpelling=true)]
        private static extern int WaitForSingleObjectDontCallThis(SafeWaitHandle handle, int timeout);

        internal static int CurrentEnvironment
        {
            get
            {
                if (environment == 0)
                {
                    lock (InternalSyncObject)
                    {
                        if (environment == 0)
                        {
                            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                            {
                                if (Environment.OSVersion.Version.Major >= 5)
                                {
                                    environment = 1;
                                }
                                else
                                {
                                    environment = 2;
                                }
                            }
                            else
                            {
                                environment = 3;
                            }
                        }
                    }
                }
                return environment;
            }
        }

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }
    }
}

