﻿namespace System.IO.Pipes
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    internal sealed class PipeAsyncResult : IAsyncResult
    {
        internal bool _completedSynchronously;
        internal int _EndXxxCalled;
        internal int _errorCode;
        internal SafePipeHandle _handle;
        internal bool _isComplete;
        internal unsafe NativeOverlapped* _overlapped;
        internal AsyncCallback _userCallback;
        internal object _userStateObject;
        internal ManualResetEvent _waitHandle;

        internal void CallUserCallback()
        {
            if (this._userCallback != null)
            {
                this._completedSynchronously = false;
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.CallUserCallbackWorker));
            }
            else
            {
                this._isComplete = true;
                if (this._waitHandle != null)
                {
                    this._waitHandle.Set();
                }
            }
        }

        private void CallUserCallbackWorker(object callbackState)
        {
            this._isComplete = true;
            if (this._waitHandle != null)
            {
                this._waitHandle.Set();
            }
            this._userCallback(this);
        }

        public object AsyncState =>
            this._userStateObject;

        public WaitHandle AsyncWaitHandle
        {
            [SecurityCritical]
            get
            {
                if (this._waitHandle == null)
                {
                    ManualResetEvent event2 = new ManualResetEvent(false);
                    if ((this._overlapped != null) && (this._overlapped.EventHandle != IntPtr.Zero))
                    {
                        event2.SafeWaitHandle = new SafeWaitHandle(this._overlapped.EventHandle, true);
                    }
                    if (this._isComplete)
                    {
                        event2.Set();
                    }
                    this._waitHandle = event2;
                }
                return this._waitHandle;
            }
        }

        public bool CompletedSynchronously =>
            this._completedSynchronously;

        public bool IsCompleted =>
            this._isComplete;
    }
}

