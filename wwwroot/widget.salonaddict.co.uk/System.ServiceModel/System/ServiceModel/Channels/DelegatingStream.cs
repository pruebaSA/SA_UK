﻿namespace System.ServiceModel.Channels
{
    using System;
    using System.IO;
    using System.ServiceModel;

    internal abstract class DelegatingStream : Stream
    {
        private Stream stream;

        protected DelegatingStream(Stream stream)
        {
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
            }
            this.stream = stream;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => 
            this.stream.BeginRead(buffer, offset, count, callback, state);

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => 
            this.stream.BeginWrite(buffer, offset, count, callback, state);

        public override void Close()
        {
            this.stream.Close();
        }

        public override int EndRead(IAsyncResult result) => 
            this.stream.EndRead(result);

        public override void EndWrite(IAsyncResult result)
        {
            this.stream.EndWrite(result);
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

        protected Stream BaseStream =>
            this.stream;

        public override bool CanRead =>
            this.stream.CanRead;

        public override bool CanSeek =>
            this.stream.CanSeek;

        public override bool CanTimeout =>
            this.stream.CanTimeout;

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

        public override int ReadTimeout
        {
            get => 
                this.stream.ReadTimeout;
            set
            {
                this.stream.ReadTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get => 
                this.stream.WriteTimeout;
            set
            {
                this.stream.WriteTimeout = value;
            }
        }
    }
}

