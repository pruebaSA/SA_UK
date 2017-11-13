﻿namespace System.Xml
{
    using System;
    using System.IO;

    internal class XmlRegisteredNonCachedStream : Stream
    {
        private XmlDownloadManager downloadManager;
        private string host;
        protected Stream stream;

        internal XmlRegisteredNonCachedStream(Stream stream, XmlDownloadManager downloadManager, string host)
        {
            this.stream = stream;
            this.downloadManager = downloadManager;
            this.host = host;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => 
            this.stream.BeginRead(buffer, offset, count, callback, state);

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => 
            this.BeginWrite(buffer, offset, count, callback, state);

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this.stream != null))
                {
                    if (this.downloadManager != null)
                    {
                        this.downloadManager.Remove(this.host);
                    }
                    this.stream.Close();
                }
                this.stream = null;
                GC.SuppressFinalize(this);
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override int EndRead(IAsyncResult asyncResult) => 
            this.stream.EndRead(asyncResult);

        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.stream.EndWrite(asyncResult);
        }

        ~XmlRegisteredNonCachedStream()
        {
            if (this.downloadManager != null)
            {
                this.downloadManager.Remove(this.host);
            }
            this.stream = null;
        }

        public override void Flush()
        {
            this.stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count) => 
            this.stream.Read(buffer, offset, count);

        public override int ReadByte() => 
            this.stream.ReadByte();

        public override long Seek(long offset, SeekOrigin origin) => 
            this.stream.Seek(offset, origin);

        public override void SetLength(long value)
        {
            this.stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            this.stream.WriteByte(value);
        }

        public override bool CanRead =>
            this.stream.CanRead;

        public override bool CanSeek =>
            this.stream.CanSeek;

        public override bool CanWrite =>
            this.stream.CanWrite;

        public override long Length =>
            this.stream.Length;

        public override long Position
        {
            get => 
                this.stream.Position;
            set
            {
                this.stream.Position = value;
            }
        }
    }
}

