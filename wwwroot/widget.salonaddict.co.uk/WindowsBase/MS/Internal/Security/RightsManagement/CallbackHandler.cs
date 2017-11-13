namespace MS.Internal.Security.RightsManagement
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Threading;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal sealed class CallbackHandler : IDisposable
    {
        private string _callbackData;
        private MS.Internal.Security.RightsManagement.CallbackDelegate _callbackDelegate;
        private Exception _exception;
        private int _hr;
        private AutoResetEvent _resetEvent = new AutoResetEvent(false);
        private const uint S_DRM_COMPLETED = 0x4cf04;

        internal CallbackHandler()
        {
            this._callbackDelegate = new MS.Internal.Security.RightsManagement.CallbackDelegate(this.OnStatus);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && (this._resetEvent != null))
            {
                this._resetEvent.Set();
                ((IDisposable) this._resetEvent).Dispose();
            }
        }

        private int OnStatus(StatusMessage status, int hr, IntPtr pvParam, IntPtr pvContext)
        {
            if ((hr == 0x4cf04L) || (hr < 0))
            {
                this._exception = null;
                try
                {
                    this._hr = hr;
                    if (pvParam != IntPtr.Zero)
                    {
                        this._callbackData = Marshal.PtrToStringUni(pvParam);
                    }
                }
                catch (Exception exception)
                {
                    this._exception = exception;
                }
                finally
                {
                    this._resetEvent.Set();
                }
            }
            return 0;
        }

        internal void WaitForCompletion()
        {
            this._resetEvent.WaitOne();
            if (this._exception != null)
            {
                throw this._exception;
            }
            Errors.ThrowOnErrorCode(this._hr);
        }

        internal string CallbackData =>
            this._callbackData;

        internal MS.Internal.Security.RightsManagement.CallbackDelegate CallbackDelegate =>
            this._callbackDelegate;
    }
}

