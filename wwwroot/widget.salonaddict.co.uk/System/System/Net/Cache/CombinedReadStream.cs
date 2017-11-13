﻿namespace System.Net.Cache
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;

    internal class CombinedReadStream : Stream, ICloseEx
    {
        private bool m_HeadEOF;
        private long m_HeadLength;
        private Stream m_HeadStream;
        private AsyncCallback m_ReadCallback;
        private int m_ReadNesting;
        private Stream m_TailStream;

        internal CombinedReadStream(Stream headStream, Stream tailStream)
        {
            this.m_HeadStream = headStream;
            this.m_TailStream = tailStream;
            this.m_HeadEOF = headStream == Stream.Null;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            IAsyncResult result3;
            try
            {
                if (Interlocked.Increment(ref this.m_ReadNesting) != 1)
                {
                    throw new NotSupportedException(SR.GetString("net_io_invalidnestedcall", new object[] { "BeginRead", "read" }));
                }
                if (this.m_ReadCallback == null)
                {
                    this.m_ReadCallback = new AsyncCallback(this.ReadCallback);
                }
                if (this.m_HeadEOF)
                {
                    return this.m_TailStream.BeginRead(buffer, offset, count, callback, state);
                }
                InnerAsyncResult result = new InnerAsyncResult(state, callback, buffer, offset, count);
                IAsyncResult asyncResult = this.m_HeadStream.BeginRead(buffer, offset, count, this.m_ReadCallback, result);
                if (!asyncResult.CompletedSynchronously)
                {
                    return result;
                }
                int num = this.m_HeadStream.EndRead(asyncResult);
                this.m_HeadLength += num;
                if ((num == 0) && (result.Count != 0))
                {
                    this.m_HeadEOF = true;
                    this.m_HeadStream.Close();
                    return this.m_TailStream.BeginRead(buffer, offset, count, callback, state);
                }
                result.Buffer = null;
                result.InvokeCallback(count);
                result3 = result;
            }
            catch
            {
                Interlocked.Decrement(ref this.m_ReadNesting);
                throw;
            }
            return result3;
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException(SR.GetString("net_noseek"));
        }

        protected sealed override void Dispose(bool disposing)
        {
            this.Dispose(disposing, CloseExState.Normal);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing, CloseExState closeState)
        {
            try
            {
                if (!this.m_HeadEOF)
                {
                    ICloseEx headStream = this.m_HeadStream as ICloseEx;
                    if (headStream != null)
                    {
                        headStream.CloseEx(closeState);
                    }
                    else
                    {
                        this.m_HeadStream.Close();
                    }
                }
            }
            finally
            {
                ICloseEx tailStream = this.m_TailStream as ICloseEx;
                if (tailStream != null)
                {
                    tailStream.CloseEx(closeState);
                }
                else
                {
                    this.m_TailStream.Close();
                }
            }
            if (!disposing)
            {
                this.m_HeadStream = null;
                this.m_TailStream = null;
            }
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            if (Interlocked.Decrement(ref this.m_ReadNesting) != 0)
            {
                Interlocked.Increment(ref this.m_ReadNesting);
                throw new InvalidOperationException(SR.GetString("net_io_invalidendcall", new object[] { "EndRead" }));
            }
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }
            InnerAsyncResult result = asyncResult as InnerAsyncResult;
            if (result == null)
            {
                if (!this.m_HeadEOF)
                {
                    return this.m_HeadStream.EndRead(asyncResult);
                }
                return this.m_TailStream.EndRead(asyncResult);
            }
            result.InternalWaitForCompletion();
            if (result.Result is Exception)
            {
                throw ((Exception) result.Result);
            }
            return (int) result.Result;
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotSupportedException(SR.GetString("net_noseek"));
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int num2;
            try
            {
                if (Interlocked.Increment(ref this.m_ReadNesting) != 1)
                {
                    throw new NotSupportedException(SR.GetString("net_io_invalidnestedcall", new object[] { "Read", "read" }));
                }
                if (this.m_HeadEOF)
                {
                    return this.m_TailStream.Read(buffer, offset, count);
                }
                int num = this.m_HeadStream.Read(buffer, offset, count);
                this.m_HeadLength += num;
                if ((num == 0) && (count != 0))
                {
                    this.m_HeadEOF = true;
                    this.m_HeadStream.Close();
                    num = this.m_TailStream.Read(buffer, offset, count);
                }
                num2 = num;
            }
            finally
            {
                Interlocked.Decrement(ref this.m_ReadNesting);
            }
            return num2;
        }

        private void ReadCallback(IAsyncResult transportResult)
        {
            if (!transportResult.CompletedSynchronously)
            {
                InnerAsyncResult asyncState = transportResult.AsyncState as InnerAsyncResult;
                try
                {
                    int num;
                    if (!this.m_HeadEOF)
                    {
                        num = this.m_HeadStream.EndRead(transportResult);
                        this.m_HeadLength += num;
                    }
                    else
                    {
                        num = this.m_TailStream.EndRead(transportResult);
                    }
                    if ((!this.m_HeadEOF && (num == 0)) && (asyncState.Count != 0))
                    {
                        this.m_HeadEOF = true;
                        this.m_HeadStream.Close();
                        IAsyncResult asyncResult = this.m_TailStream.BeginRead(asyncState.Buffer, asyncState.Offset, asyncState.Count, this.m_ReadCallback, asyncState);
                        if (!asyncResult.CompletedSynchronously)
                        {
                            return;
                        }
                        num = this.m_TailStream.EndRead(asyncResult);
                    }
                    asyncState.Buffer = null;
                    asyncState.InvokeCallback(num);
                }
                catch (Exception exception)
                {
                    if (asyncState.InternalPeekCompleted)
                    {
                        throw;
                    }
                    asyncState.InvokeCallback(exception);
                }
                catch
                {
                    if (asyncState.InternalPeekCompleted)
                    {
                        throw;
                    }
                    asyncState.InvokeCallback(new Exception(SR.GetString("net_nonClsCompliantException")));
                }
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(SR.GetString("net_noseek"));
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(SR.GetString("net_noseek"));
        }

        void ICloseEx.CloseEx(CloseExState closeState)
        {
            this.Dispose(true, closeState);
            GC.SuppressFinalize(this);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException(SR.GetString("net_noseek"));
        }

        public override bool CanRead
        {
            get
            {
                if (!this.m_HeadEOF)
                {
                    return this.m_HeadStream.CanRead;
                }
                return this.m_TailStream.CanRead;
            }
        }

        public override bool CanSeek =>
            false;

        public override bool CanTimeout =>
            (this.m_TailStream.CanTimeout && this.m_HeadStream.CanTimeout);

        public override bool CanWrite =>
            false;

        public override long Length =>
            (this.m_TailStream.Length + (this.m_HeadEOF ? this.m_HeadLength : this.m_HeadStream.Length));

        public override long Position
        {
            get => 
                (this.m_TailStream.Position + (this.m_HeadEOF ? this.m_HeadLength : this.m_HeadStream.Position));
            set
            {
                throw new NotSupportedException(SR.GetString("net_noseek"));
            }
        }

        public override int ReadTimeout
        {
            get
            {
                if (!this.m_HeadEOF)
                {
                    return this.m_HeadStream.ReadTimeout;
                }
                return this.m_TailStream.ReadTimeout;
            }
            set
            {
                this.m_TailStream.ReadTimeout = this.m_HeadStream.ReadTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                if (!this.m_HeadEOF)
                {
                    return this.m_HeadStream.WriteTimeout;
                }
                return this.m_TailStream.WriteTimeout;
            }
            set
            {
                this.m_TailStream.WriteTimeout = this.m_HeadStream.WriteTimeout = value;
            }
        }

        private class InnerAsyncResult : LazyAsyncResult
        {
            public byte[] Buffer;
            public int Count;
            public int Offset;

            public InnerAsyncResult(object userState, AsyncCallback userCallback, byte[] buffer, int offset, int count) : base(null, userState, userCallback)
            {
                this.Buffer = buffer;
                this.Offset = offset;
                this.Count = count;
            }
        }
    }
}

