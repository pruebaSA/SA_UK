namespace System.Net
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Threading;

    [SuppressUnmanagedCodeSecurity]
    internal sealed class SafeCloseHandle : CriticalHandleZeroOrMinusOneIsInvalid
    {
        private int _disposed;
        private const string ADVAPI32 = "advapi32.dll";
        private const string HTTPAPI = "httpapi.dll";
        private const string SECURITY = "security.dll";

        private SafeCloseHandle()
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal void Abort()
        {
            this.ReleaseHandle();
            base.SetHandleAsInvalid();
        }

        internal static SafeCloseHandle CreateRequestQueueHandle()
        {
            SafeCloseHandle pReqQueueHandle = null;
            uint num = UnsafeNclNativeMethods.SafeNetHandles.HttpCreateHttpHandle(out pReqQueueHandle, 0);
            if ((pReqQueueHandle != null) && (num != 0))
            {
                pReqQueueHandle.SetHandleAsInvalid();
                throw new HttpListenerException((int) num);
            }
            return pReqQueueHandle;
        }

        internal IntPtr DangerousGetHandle() => 
            base.handle;

        internal static int GetSecurityContextToken(SafeDeleteContext phContext, out SafeCloseHandle safeHandle)
        {
            int num = -2146893055;
            bool success = false;
            safeHandle = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                phContext.DangerousAddRef(ref success);
            }
            catch (Exception exception)
            {
                if (success)
                {
                    phContext.DangerousRelease();
                    success = false;
                }
                if (!(exception is ObjectDisposedException))
                {
                    throw;
                }
            }
            finally
            {
                if (success)
                {
                    num = UnsafeNclNativeMethods.SafeNetHandles.QuerySecurityContextToken(ref phContext._handle, out safeHandle);
                    phContext.DangerousRelease();
                }
            }
            return num;
        }

        protected override bool ReleaseHandle()
        {
            if (!this.IsInvalid && (Interlocked.Increment(ref this._disposed) == 1))
            {
                return UnsafeNclNativeMethods.SafeNetHandles.CloseHandle(base.handle);
            }
            return true;
        }
    }
}

