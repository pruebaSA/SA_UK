namespace MS.Internal.IO.Packaging.CompoundFile
{
    using System;
    using System.Collections;
    using System.IO;

    internal class StreamWithDictionary : Stream, IDictionary, ICollection, IEnumerable
    {
        private bool _disposed;
        private IDictionary baseDictionary;
        private Stream baseStream;

        internal StreamWithDictionary(Stream wrappedStream, IDictionary wrappedDictionary)
        {
            this.baseStream = wrappedStream;
            this.baseDictionary = wrappedDictionary;
        }

        private void CheckDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException("Stream");
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && !this._disposed)
                {
                    this._disposed = true;
                    this.baseStream.Close();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            this.CheckDisposed();
            this.baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            return this.baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.CheckDisposed();
            return this.baseStream.Seek(offset, origin);
        }

        public override void SetLength(long newLength)
        {
            this.CheckDisposed();
            this.baseStream.SetLength(newLength);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.CheckDisposed();
            this.baseDictionary.CopyTo(array, index);
        }

        void IDictionary.Add(object key, object val)
        {
            this.CheckDisposed();
            this.baseDictionary.Add(key, val);
        }

        void IDictionary.Clear()
        {
            this.CheckDisposed();
            this.baseDictionary.Clear();
        }

        bool IDictionary.Contains(object key)
        {
            this.CheckDisposed();
            return this.baseDictionary.Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            this.CheckDisposed();
            return this.baseDictionary.GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            this.CheckDisposed();
            this.baseDictionary.Remove(key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.CheckDisposed();
            return this.baseDictionary.GetEnumerator();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckDisposed();
            this.baseStream.Write(buffer, offset, count);
        }

        public override bool CanRead =>
            (!this._disposed && this.baseStream.CanRead);

        public override bool CanSeek =>
            (!this._disposed && this.baseStream.CanSeek);

        public override bool CanWrite =>
            (!this._disposed && this.baseStream.CanWrite);

        internal bool Disposed =>
            this._disposed;

        public override long Length
        {
            get
            {
                this.CheckDisposed();
                return this.baseStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                this.CheckDisposed();
                return this.baseStream.Position;
            }
            set
            {
                this.CheckDisposed();
                this.baseStream.Position = value;
            }
        }

        int ICollection.Count
        {
            get
            {
                this.CheckDisposed();
                return this.baseDictionary.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                this.CheckDisposed();
                return this.baseDictionary.IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                this.CheckDisposed();
                return this.baseDictionary.SyncRoot;
            }
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                this.CheckDisposed();
                return this.baseDictionary.IsFixedSize;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                this.CheckDisposed();
                return this.baseDictionary.IsReadOnly;
            }
        }

        object IDictionary.this[object index]
        {
            get
            {
                this.CheckDisposed();
                return this.baseDictionary[index];
            }
            set
            {
                this.CheckDisposed();
                this.baseDictionary[index] = value;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                this.CheckDisposed();
                return this.baseDictionary.Keys;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                this.CheckDisposed();
                return this.baseDictionary.Values;
            }
        }
    }
}

