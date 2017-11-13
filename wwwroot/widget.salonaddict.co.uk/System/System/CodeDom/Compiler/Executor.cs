namespace System.CodeDom.Compiler
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Text;

    [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust"), PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
    public sealed class Executor
    {
        private const int ProcessTimeOut = 0x927c0;

        private Executor()
        {
        }

        private static FileStream CreateInheritedFile(string file) => 
            new FileStream(file, FileMode.CreateNew, FileAccess.Write, FileShare.Inheritable | FileShare.Read);

        public static void ExecWait(string cmd, TempFileCollection tempFiles)
        {
            string outputName = null;
            string errorName = null;
            ExecWaitWithCapture((string) null, cmd, tempFiles, ref outputName, ref errorName);
        }

        public static int ExecWaitWithCapture(string cmd, TempFileCollection tempFiles, ref string outputName, ref string errorName) => 
            ExecWaitWithCapture(null, cmd, Environment.CurrentDirectory, tempFiles, ref outputName, ref errorName, null);

        public static int ExecWaitWithCapture(IntPtr userToken, string cmd, TempFileCollection tempFiles, ref string outputName, ref string errorName) => 
            ExecWaitWithCapture(new SafeUserTokenHandle(userToken, false), cmd, Environment.CurrentDirectory, tempFiles, ref outputName, ref errorName, null);

        public static int ExecWaitWithCapture(string cmd, string currentDir, TempFileCollection tempFiles, ref string outputName, ref string errorName) => 
            ExecWaitWithCapture(null, cmd, currentDir, tempFiles, ref outputName, ref errorName, null);

        public static int ExecWaitWithCapture(IntPtr userToken, string cmd, string currentDir, TempFileCollection tempFiles, ref string outputName, ref string errorName) => 
            ExecWaitWithCapture(new SafeUserTokenHandle(userToken, false), cmd, Environment.CurrentDirectory, tempFiles, ref outputName, ref errorName, null);

        internal static int ExecWaitWithCapture(SafeUserTokenHandle userToken, string cmd, string currentDir, TempFileCollection tempFiles, ref string outputName, ref string errorName, string trueCmdLine)
        {
            int num = 0;
            try
            {
                WindowsImpersonationContext impersonation = RevertImpersonation();
                try
                {
                    num = ExecWaitWithCaptureUnimpersonated(userToken, cmd, currentDir, tempFiles, ref outputName, ref errorName, trueCmdLine);
                }
                finally
                {
                    ReImpersonate(impersonation);
                }
            }
            catch
            {
                throw;
            }
            return num;
        }

        private static unsafe int ExecWaitWithCaptureUnimpersonated(SafeUserTokenHandle userToken, string cmd, string currentDir, TempFileCollection tempFiles, ref string outputName, ref string errorName, string trueCmdLine)
        {
            IntSecurity.UnmanagedCode.Demand();
            if ((outputName == null) || (outputName.Length == 0))
            {
                outputName = tempFiles.AddExtension("out");
            }
            if ((errorName == null) || (errorName.Length == 0))
            {
                errorName = tempFiles.AddExtension("err");
            }
            FileStream stream = CreateInheritedFile(outputName);
            FileStream stream2 = CreateInheritedFile(errorName);
            bool flag = false;
            Microsoft.Win32.SafeNativeMethods.PROCESS_INFORMATION lpProcessInformation = new Microsoft.Win32.SafeNativeMethods.PROCESS_INFORMATION();
            Microsoft.Win32.SafeHandles.SafeProcessHandle handle = new Microsoft.Win32.SafeHandles.SafeProcessHandle();
            Microsoft.Win32.SafeHandles.SafeThreadHandle handle2 = new Microsoft.Win32.SafeHandles.SafeThreadHandle();
            SafeUserTokenHandle hNewToken = null;
            try
            {
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
                writer.Write(currentDir);
                writer.Write("> ");
                writer.WriteLine((trueCmdLine != null) ? trueCmdLine : cmd);
                writer.WriteLine();
                writer.WriteLine();
                writer.Flush();
                NativeMethods.STARTUPINFO structure = new NativeMethods.STARTUPINFO();
                structure.cb = Marshal.SizeOf(structure);
                structure.dwFlags = 0x101;
                structure.wShowWindow = 0;
                structure.hStdOutput = stream.SafeFileHandle;
                structure.hStdError = stream2.SafeFileHandle;
                structure.hStdInput = new SafeFileHandle(Microsoft.Win32.UnsafeNativeMethods.GetStdHandle(-10), false);
                StringDictionary sd = new StringDictionary();
                foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
                {
                    sd.Add((string) entry.Key, (string) entry.Value);
                }
                sd["_ClrRestrictSecAttributes"] = "1";
                byte[] buffer = EnvironmentBlock.ToByteArray(sd, false);
                try
                {
                    fixed (byte* numRef = buffer)
                    {
                        IntPtr lpEnvironment = new IntPtr((void*) numRef);
                        if ((userToken == null) || userToken.IsInvalid)
                        {
                            RuntimeHelpers.PrepareConstrainedRegions();
                            try
                            {
                                goto Label_0325;
                            }
                            finally
                            {
                                flag = NativeMethods.CreateProcess(null, new StringBuilder(cmd), null, null, true, 0, lpEnvironment, currentDir, structure, lpProcessInformation);
                                if ((lpProcessInformation.hProcess != IntPtr.Zero) && (lpProcessInformation.hProcess != NativeMethods.INVALID_HANDLE_VALUE))
                                {
                                    handle.InitialSetHandle(lpProcessInformation.hProcess);
                                }
                                if ((lpProcessInformation.hThread != IntPtr.Zero) && (lpProcessInformation.hThread != NativeMethods.INVALID_HANDLE_VALUE))
                                {
                                    handle2.InitialSetHandle(lpProcessInformation.hThread);
                                }
                            }
                        }
                        flag = SafeUserTokenHandle.DuplicateTokenEx(userToken, 0xf01ff, null, 2, 1, out hNewToken);
                        if (flag)
                        {
                            RuntimeHelpers.PrepareConstrainedRegions();
                            try
                            {
                            }
                            finally
                            {
                                flag = NativeMethods.CreateProcessAsUser(hNewToken, null, cmd, null, null, true, 0, new HandleRef(null, lpEnvironment), currentDir, structure, lpProcessInformation);
                                if ((lpProcessInformation.hProcess != IntPtr.Zero) && (lpProcessInformation.hProcess != NativeMethods.INVALID_HANDLE_VALUE))
                                {
                                    handle.InitialSetHandle(lpProcessInformation.hProcess);
                                }
                                if ((lpProcessInformation.hThread != IntPtr.Zero) && (lpProcessInformation.hThread != NativeMethods.INVALID_HANDLE_VALUE))
                                {
                                    handle2.InitialSetHandle(lpProcessInformation.hThread);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    numRef = null;
                }
            }
            finally
            {
                if ((!flag && (hNewToken != null)) && !hNewToken.IsInvalid)
                {
                    hNewToken.Close();
                    hNewToken = null;
                }
                stream.Close();
                stream2.Close();
            }
        Label_0325:
            if (flag)
            {
                try
                {
                    int num2 = NativeMethods.WaitForSingleObject(handle, 0x927c0);
                    if (num2 == 0x102)
                    {
                        throw new ExternalException(SR.GetString("ExecTimeout", new object[] { cmd }), 0x102);
                    }
                    if (num2 != 0)
                    {
                        throw new ExternalException(SR.GetString("ExecBadreturn", new object[] { cmd }), Marshal.GetLastWin32Error());
                    }
                    int exitCode = 0x103;
                    if (!NativeMethods.GetExitCodeProcess(handle, out exitCode))
                    {
                        throw new ExternalException(SR.GetString("ExecCantGetRetCode", new object[] { cmd }), Marshal.GetLastWin32Error());
                    }
                    return exitCode;
                }
                finally
                {
                    handle.Close();
                    handle2.Close();
                    if ((hNewToken != null) && !hNewToken.IsInvalid)
                    {
                        hNewToken.Close();
                    }
                }
            }
            throw new ExternalException(SR.GetString("ExecCantExec", new object[] { cmd }), Marshal.GetLastWin32Error());
        }

        internal static string GetRuntimeInstallDirectory() => 
            RuntimeEnvironment.GetRuntimeDirectory();

        internal static void ReImpersonate(WindowsImpersonationContext impersonation)
        {
            impersonation.Undo();
        }

        internal static WindowsImpersonationContext RevertImpersonation()
        {
            new SecurityPermission(SecurityPermissionFlag.ControlPrincipal).Assert();
            return WindowsIdentity.Impersonate(new IntPtr(0));
        }
    }
}

