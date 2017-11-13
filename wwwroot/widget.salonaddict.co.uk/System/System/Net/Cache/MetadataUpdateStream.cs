namespace System.Net.Cache
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Threading;

    internal class MetadataUpdateStream : Stream, ICloseEx
    {
        private int _Disposed;
        private RequestCache m_Cache;
        private bool m_CacheDestroy;
        private StringCollection m_EntryMetadata;
        private DateTime m_Expires;
        private bool m_IsStrictCacheErrors;
        private string m_Key;
        private DateTime m_LastModified;
        private DateTime m_LastSynchronized;
        private TimeSpan m_MaxStale;
        private Stream m_ParentStream;
        private StringCollection m_SystemMetadata;

        private MetadataUpdateStream(Stream parentStream, RequestCache cache, string key, bool isStrictCacheErrors)
        {
            if (parentStream == null)
            {
                throw new ArgumentNullException("parentStream");
            }
            this.m_ParentStream = parentStream;
            this.m_Cache = cache;
            this.m_Key = key;
            this.m_CacheDestroy = true;
            this.m_IsStrictCacheErrors = isStrictCacheErrors;
        }

        internal MetadataUpdateStream(Stream parentStream, RequestCache cache, string key, DateTime expiresGMT, DateTime lastModifiedGMT, DateTime lastSynchronizedGMT, TimeSpan maxStale, StringCollection entryMetadata, StringCollection systemMetadata, bool isStrictCacheErrors)
        {
            if (parentStream == null)
            {
                throw new ArgumentNullException("parentStream");
            }
            this.m_ParentStream = parentStream;
            this.m_Cache = cache;
            this.m_Key = key;
            this.m_Expires = expiresGMT;
            this.m_LastModified = lastModifiedGMT;
            this.m_LastSynchronized = lastSynchronizedGMT;
            this.m_MaxStale = maxStale;
            this.m_EntryMetadata = entryMetadata;
            this.m_SystemMetadata = systemMetadata;
            this.m_IsStrictCacheErrors = isStrictCacheErrors;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => 
            this.m_ParentStream.BeginRead(buffer, offset, count, callback, state);

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => 
            this.m_ParentStream.BeginWrite(buffer, offset, count, callback, state);

        protected sealed override void Dispose(bool disposing)
        {
            this.Dispose(disposing, CloseExState.Normal);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing, CloseExState closeState)
        {
            if (Interlocked.Increment(ref this._Disposed) == 1)
            {
                ICloseEx parentStream = this.m_ParentStream as ICloseEx;
                if (parentStream != null)
                {
                    parentStream.CloseEx(closeState);
                }
                else
                {
                    this.m_ParentStream.Close();
                }
                if (this.m_CacheDestroy)
                {
                    if (this.m_IsStrictCacheErrors)
                    {
                        this.m_Cache.Remove(this.m_Key);
                    }
                    else
                    {
                        this.m_Cache.TryRemove(this.m_Key);
                    }
                }
                else if (this.m_IsStrictCacheErrors)
                {
                    this.m_Cache.Update(this.m_Key, this.m_Expires, this.m_LastModified, this.m_LastSynchronized, this.m_MaxStale, this.m_EntryMetadata, this.m_SystemMetadata);
                }
                else
                {
                    this.m_Cache.TryUpdate(this.m_Key, this.m_Expires, this.m_LastModified, this.m_LastSynchronized, this.m_MaxStale, this.m_EntryMetadata, this.m_SystemMetadata);
                }
                if (!disposing)
                {
                    this.m_Cache = null;
                    this.m_Key = null;
                    this.m_EntryMetadata = null;
                    this.m_SystemMetadata = null;
                }
            }
            base.Dispose(disposing);
        }

        public override int EndRead(IAsyncResult asyncResult) => 
            this.m_ParentStream.EndRead(asyncResult);

        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.m_ParentStream.EndWrite(asyncResult);
        }

        public override void Flush()
        {
            this.m_ParentStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count) => 
            this.m_ParentStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => 
            this.m_ParentStream.Seek(offset, origin);

        public override void SetLength(long value)
        {
            this.m_ParentStream.SetLength(value);
        }

        void ICloseEx.CloseEx(CloseExState closeState)
        {
            this.Dispose(true, closeState);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.m_ParentStream.Write(buffer, offset, count);
        }

        public override bool CanRead =>
            this.m_ParentStream.CanRead;

        public override bool CanSeek =>
            this.m_ParentStream.CanSeek;

        public override bool CanTimeout =>
            this.m_ParentStream.CanTimeout;

        public override bool CanWrite =>
            this.m_ParentStream.CanWrite;

        public override long Length =>
            this.m_ParentStream.Length;

        public override long Position
        {
            get => 
                this.m_ParentStream.Position;
            set
            {
                this.m_ParentStream.Position = value;
            }
        }

        public override int ReadTimeout
        {
            get => 
                this.m_ParentStream.ReadTimeout;
            set
            {
                this.m_ParentStream.ReadTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get => 
                this.m_ParentStream.WriteTimeout;
            set
            {
                this.m_ParentStream.WriteTimeout = value;
            }
        }
    }
}

