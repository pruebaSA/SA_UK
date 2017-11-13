namespace PdfSharp.Pdf.Content.Objects
{
    using PdfSharp.Pdf.Content;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;

    [DebuggerDisplay("(count={Count})")]
    public class CSequence : CObject, IList<CObject>, ICollection<CObject>, IEnumerable<CObject>, IEnumerable
    {
        private List<CObject> items = new List<CObject>();

        public void Add(CObject value)
        {
            this.items.Add(value);
        }

        public void Add(CSequence sequence)
        {
            int count = sequence.Count;
            for (int i = 0; i < count; i++)
            {
                this.items.Add(sequence[i]);
            }
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public CSequence Clone() => 
            ((CSequence) this.Copy());

        public bool Contains(CObject value) => 
            this.items.Contains(value);

        protected override CObject Copy()
        {
            CObject obj2 = base.Copy();
            this.items = new List<CObject>(this.items);
            for (int i = 0; i < this.items.Count; i++)
            {
                this.items[i] = this.items[i].Clone();
            }
            return obj2;
        }

        public void CopyTo(CObject[] array, int index)
        {
            this.items.CopyTo(array, index);
        }

        public IEnumerator<CObject> GetEnumerator() => 
            this.items.GetEnumerator();

        public int IndexOf(CObject value) => 
            this.items.IndexOf(value);

        public void Insert(int index, CObject value)
        {
            this.items.Insert(index, value);
        }

        public bool Remove(CObject value) => 
            this.items.Remove(value);

        public void RemoveAt(int index)
        {
            this.items.RemoveAt(index);
        }

        void ICollection<CObject>.Add(CObject item)
        {
            throw new NotImplementedException();
        }

        void ICollection<CObject>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<CObject>.Contains(CObject item)
        {
            throw new NotImplementedException();
        }

        void ICollection<CObject>.CopyTo(CObject[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<CObject>.Remove(CObject item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<CObject> IEnumerable<CObject>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        int IList<CObject>.IndexOf(CObject item)
        {
            throw new NotImplementedException();
        }

        void IList<CObject>.Insert(int index, CObject item)
        {
            throw new NotImplementedException();
        }

        void IList<CObject>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public byte[] ToContent()
        {
            Stream contentStream = new MemoryStream();
            ContentWriter writer = new ContentWriter(contentStream);
            this.WriteObject(writer);
            writer.Close(false);
            contentStream.Position = 0L;
            int length = (int) contentStream.Length;
            byte[] buffer = new byte[length];
            contentStream.Read(buffer, 0, length);
            contentStream.Close();
            return buffer;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < this.items.Count; i++)
            {
                builder.Append(this.items[i].ToString());
            }
            return builder.ToString();
        }

        internal override void WriteObject(ContentWriter writer)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                this.items[i].WriteObject(writer);
            }
        }

        public int Count =>
            this.items.Count;

        public CObject this[int index]
        {
            get => 
                this.items[index];
            set
            {
                this.items[index] = value;
            }
        }

        int ICollection<CObject>.Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection<CObject>.IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        CObject IList<CObject>.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}

