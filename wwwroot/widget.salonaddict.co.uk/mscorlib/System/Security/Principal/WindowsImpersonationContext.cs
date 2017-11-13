namespace System.Security.Principal
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    [ComVisible(true)]
    public class WindowsImpersonationContext : IDisposable
    {
        private FrameSecurityDescriptor m_fsd;
        private SafeTokenHandle m_safeTokenHandle;
        private WindowsIdentity m_wi;

        private WindowsImpersonationContext()
        {
            this.m_safeTokenHandle = SafeTokenHandle.InvalidHandle;
        }

        internal WindowsImpersonationContext(SafeTokenHandle safeTokenHandle, WindowsIdentity wi, bool isImpersonating, FrameSecurityDescriptor fsd)
        {
            this.m_safeTokenHandle = SafeTokenHandle.InvalidHandle;
            if (WindowsIdentity.RunningOnWin2K)
            {
                if (safeTokenHandle.IsInvalid)
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_InvalidImpersonationToken"));
                }
                if (isImpersonating)
                {
                    if (!Win32Native.DuplicateHandle(Win32Native.GetCurrentProcess(), safeTokenHandle, Win32Native.GetCurrentProcess(), ref this.m_safeTokenHandle, 0, true, 2))
                    {
                        throw new SecurityException(Win32Native.GetMessage(Marshal.GetLastWin32Error()));
                    }
                    this.m_wi = wi;
                }
                this.m_fsd = fsd;
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
                this.Undo();
                this.m_safeTokenHandle.Dispose();
            }
        }

        public void Undo()
        {
            if (WindowsIdentity.RunningOnWin2K)
            {
                int errorCode = 0;
                if (this.m_safeTokenHandle.IsInvalid)
                {
                    errorCode = Win32.RevertToSelf();
                    if (errorCode < 0)
                    {
                        throw new SecurityException(Win32Native.GetMessage(errorCode));
                    }
                }
                else
                {
                    errorCode = Win32.RevertToSelf();
                    if (errorCode < 0)
                    {
                        throw new SecurityException(Win32Native.GetMessage(errorCode));
                    }
                    errorCode = Win32.ImpersonateLoggedOnUser(this.m_safeTokenHandle);
                    if (errorCode < 0)
                    {
                        throw new SecurityException(Win32Native.GetMessage(errorCode));
                    }
                }
                WindowsIdentity.UpdateThreadWI(this.m_wi);
                if (this.m_fsd != null)
                {
                    this.m_fsd.SetTokenHandles(null, null);
                }
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal bool UndoNoThrow()
        {
            bool flag = false;
            try
            {
                if (!WindowsIdentity.RunningOnWin2K)
                {
                    return true;
                }
                int num = 0;
                if (this.m_safeTokenHandle.IsInvalid)
                {
                    num = Win32.RevertToSelf();
                }
                else
                {
                    num = Win32.RevertToSelf();
                    if (num >= 0)
                    {
                        num = Win32.ImpersonateLoggedOnUser(this.m_safeTokenHandle);
                    }
                }
                flag = num >= 0;
                if (this.m_fsd != null)
                {
                    this.m_fsd.SetTokenHandles(null, null);
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
    }
}

