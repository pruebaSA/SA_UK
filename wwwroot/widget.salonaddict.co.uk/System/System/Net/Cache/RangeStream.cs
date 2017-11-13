namespace System.Net.Cache
{
    using System;
    using System.IO;
    using System.Net;

    internal class RangeStream : Stream, ICloseEx
    {
        private long m_Offset;
        private Stream m_ParentStream;
        private long m_Position;
        private long m_Size;

        internal RangeStream(Stream parentStream, long offset, long size)
        {
            this.m_ParentStream = parentStream;
            this.m_Offset = offset;
            this.m_Size = size;
            if (!this.m_ParentStream.CanSeek)
            {
                throw new NotSupportedException(SR.GetString("net_cache_non_seekable_stream_not_supported"));
            }
            this.m_ParentStream.Position = offset;
            this.m_Position = offset;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (this.m_Position >= (this.m_Offset + this.m_Size))
            {
                count = 0;
            }
            else if ((this.m_Position + count) > (this.m_Offset + this.m_Size))
            {
                count = (int) ((this.m_Offset + this.m_Size) - this.m_Position);
            }
            return this.m_ParentStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if ((this.m_Position + offset) > (this.m_Offset + this.m_Size))
            {
                throw new NotSupportedException(SR.GetString("net_cache_unsupported_partial_stream"));
            }
            return this.m_ParentStream.BeginWrite(buffer, offset, count, callback, state);
        }

        protected sealed override void Dispose(bool disposing)
        {
            this.Dispose(disposing, CloseExState.Normal);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing, CloseExState closeState)
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
            base.Dispose(disposing);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            int num = this.m_ParentStream.EndRead(asyncResult);
            this.m_Position += num;
            return num;
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.m_ParentStream.EndWrite(asyncResult);
            this.m_Position = this.m_ParentStream.Position;
        }

        public override void Flush()
        {
            this.m_ParentStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.m_Position >= (this.m_Offset + this.m_Size))
            {
                return 0;
            }
            if ((this.m_Position + count) > (this.m_Offset + this.m_Size))
            {
                count = (int) ((this.m_Offset + this.m_Size) - this.m_Position);
            }
            int num = this.m_ParentStream.Read(buffer, offset, count);
            this.m_Position += num;
            return num;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    offset += this.m_Offset;
                    if (offset > (this.m_Offset + this.m_Size))
                    {
                        offset = this.m_Offset + this.m_Size;
                    }
                    if (offset < this.m_Offset)
                    {
                        offset = this.m_Offset;
                    }
                    break;

                case SeekOrigin.End:
                    offset -= this.m_Offset + this.m_Size;
                    if (offset > 0L)
                    {
                        offset = 0L;
                    }
                    if (offset < -this.m_Size)
                    {
                        offset = -this.m_Size;
                    }
                    break;

                default:
                    if ((this.m_Position + offset) > (this.m_Offset + this.m_Size))
                    {
                        offset = (this.m_Offset + this.m_Size) - this.m_Position;
                    }
                    if ((this.m_Position + offset) < this.m_Offset)
                    {
                        offset = this.m_Offset - this.m_Position;
                    }
                    break;
            }
            this.m_Position = this.m_ParentStream.Seek(offset, origin);
            return (this.m_Position - this.m_Offset);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(SR.GetString("net_cache_unsupported_partial_stream"));
        }

        void ICloseEx.CloseEx(CloseExState closeState)
        {
            this.Dispose(true, closeState);
            GC.SuppressFinalize(this);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if ((this.m_Position + count) > (this.m_Offset + this.m_Size))
            {
                throw new NotSupportedException(SR.GetString("net_cache_unsupported_partial_stream"));
            }
            this.m_ParentStream.Write(buffer, offset, count);
            this.m_Position += count;
        }

        public override bool CanRead =>
            this.m_ParentStream.CanRead;

        public override bool CanSeek =>
            this.m_ParentStream.CanSeek;

        public override bool CanTimeout =>
            this.m_ParentStream.CanTimeout;

        public override bool CanWrite =>
            this.m_ParentStream.CanWrite;

        public override long Length
        {
            get
            {
                long length = this.m_ParentStream.Length;
                return this.m_Size;
            }
        }

        public override long Position
        {
            get => 
                (this.m_ParentStream.Position - this.m_Offset);
            set
            {
                value += this.m_Offset;
                if (value > (this.m_Offset + this.m_Size))
                {
                    value = this.m_Offset + this.m_Size;
                }
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

