namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    [FriendAccessAllowed]
    internal class ReaderWriterLockWrapper
    {
        private AutoReaderRelease _arr;
        private AutoWriterRelease _awr;
        private ReaderWriterLock _rwLock = new ReaderWriterLock();

        internal ReaderWriterLockWrapper()
        {
            this._awr = new AutoWriterRelease(this._rwLock);
            this._arr = new AutoReaderRelease(this._rwLock);
        }

        internal IDisposable ReadLock
        {
            get
            {
                this._rwLock.AcquireReaderLock(-1);
                return this._arr;
            }
        }

        internal IDisposable WriteLock
        {
            get
            {
                this._rwLock.AcquireWriterLock(-1);
                return this._awr;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AutoReaderRelease : IDisposable
        {
            private ReaderWriterLock _lock;
            public AutoReaderRelease(ReaderWriterLock rwLock)
            {
                this._lock = rwLock;
            }

            public void Dispose()
            {
                this._lock.ReleaseReaderLock();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AutoWriterRelease : IDisposable
        {
            private ReaderWriterLock _lock;
            public AutoWriterRelease(ReaderWriterLock rwLock)
            {
                this._lock = rwLock;
            }

            public void Dispose()
            {
                this._lock.ReleaseWriterLock();
            }
        }
    }
}

