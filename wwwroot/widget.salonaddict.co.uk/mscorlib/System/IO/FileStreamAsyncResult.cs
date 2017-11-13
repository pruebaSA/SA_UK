namespace System.IO
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Threading;

    internal sealed class FileStreamAsyncResult : IAsyncResult
    {
        internal bool _completedSynchronously;
        internal int _EndXxxCalled;
        internal int _errorCode;
        internal SafeFileHandle _handle;
        internal bool _isComplete;
        internal bool _isWrite;
        internal int _numBufferedBytes;
        internal int _numBytes;
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

        internal static FileStreamAsyncResult CreateBufferedReadResult(int numBufferedBytes, AsyncCallback userCallback, object userStateObject) => 
            new FileStreamAsyncResult { 
                _userCallback = userCallback,
                _userStateObject = userStateObject,
                _isWrite = false,
                _numBufferedBytes = numBufferedBytes
            };

        public object AsyncState =>
            this._userStateObject;

        public WaitHandle AsyncWaitHandle
        {
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

