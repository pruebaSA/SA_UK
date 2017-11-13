namespace System.Web
{
    using System;
    using System.IO;

    internal class HttpResponseStream : Stream
    {
        private HttpWriter _writer;

        internal HttpResponseStream(HttpWriter writer)
        {
            this._writer = writer;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    this._writer.Close();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            this._writer.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!this._writer.IgnoringFurtherWrites)
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }
                int num = buffer.Length - offset;
                if ((offset < 0) || (num <= 0))
                {
                    throw new ArgumentOutOfRangeException("offset");
                }
                if (count < 0)
                {
                    throw new ArgumentOutOfRangeException("count");
                }
                if (num < count)
                {
                    count = num;
                }
                this._writer.WriteFromStream(buffer, offset, count);
            }
        }

        public override bool CanRead =>
            false;

        public override bool CanSeek =>
            false;

        public override bool CanWrite =>
            true;

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}

