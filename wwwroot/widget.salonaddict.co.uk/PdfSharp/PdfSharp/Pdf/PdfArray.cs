namespace PdfSharp.Pdf
{
    using PdfSharp;
    using PdfSharp.Pdf.Advanced;
    using PdfSharp.Pdf.IO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    [DebuggerDisplay("(elements={Elements.Count})")]
    public class PdfArray : PdfObject, IEnumerable<PdfItem>, IEnumerable
    {
        private ArrayElements elements;

        public PdfArray()
        {
        }

        protected PdfArray(PdfArray array) : base(array)
        {
            if (array.elements != null)
            {
                array.elements.SetOwner(this);
            }
        }

        public PdfArray(PdfDocument document) : base(document)
        {
        }

        public PdfArray(PdfDocument document, params PdfItem[] items) : base(document)
        {
            foreach (PdfItem item in items)
            {
                this.Elements.Add(item);
            }
        }

        public PdfArray Clone() => 
            ((PdfArray) this.Copy());

        protected override object Copy()
        {
            PdfArray array = (PdfArray) base.Copy();
            if (array.elements != null)
            {
                array.elements = array.elements.Clone();
                int count = array.elements.Count;
                for (int i = 0; i < count; i++)
                {
                    PdfItem item = array.elements[i];
                    if (item is PdfObject)
                    {
                        array.elements[i] = item.Clone();
                    }
                }
            }
            return array;
        }

        public virtual IEnumerator<PdfItem> GetEnumerator() => 
            this.Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[ ");
            int count = this.Elements.Count;
            for (int i = 0; i < count; i++)
            {
                builder.Append(this.Elements[i].ToString() + " ");
            }
            builder.Append("]");
            return builder.ToString();
        }

        internal override void WriteObject(PdfWriter writer)
        {
            writer.WriteBeginObject(this);
            int count = this.Elements.Count;
            for (int i = 0; i < count; i++)
            {
                this.Elements[i].WriteObject(writer);
            }
            writer.WriteEndObject();
        }

        public ArrayElements Elements
        {
            get
            {
                if (this.elements == null)
                {
                    this.elements = new ArrayElements(this);
                }
                return this.elements;
            }
        }

        public sealed class ArrayElements : IList<PdfItem>, ICollection<PdfItem>, IEnumerable<PdfItem>, IEnumerable, ICloneable
        {
            private List<PdfItem> elements = new List<PdfItem>();
            private PdfArray owner;

            internal ArrayElements(PdfArray array)
            {
                this.owner = array;
            }

            public void Add(PdfItem value)
            {
                PdfObject obj2 = value as PdfObject;
                if ((obj2 != null) && obj2.IsIndirect)
                {
                    this.elements.Add(obj2.Reference);
                }
                else
                {
                    this.elements.Add(value);
                }
            }

            public void Clear()
            {
                this.elements.Clear();
            }

            public PdfArray.ArrayElements Clone() => 
                ((PdfArray.ArrayElements) ((ICloneable) this).Clone());

            public bool Contains(PdfItem value) => 
                this.elements.Contains(value);

            public void CopyTo(PdfItem[] array, int index)
            {
                this.elements.CopyTo(array, index);
            }

            public PdfArray GetArray(int index) => 
                (this.GetObject(index) as PdfArray);

            public bool GetBoolean(int index)
            {
                if ((index < 0) || (index >= this.Count))
                {
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);
                }
                object obj2 = this[index];
                if (obj2 == null)
                {
                    return false;
                }
                if (obj2 is PdfBoolean)
                {
                    return ((PdfBoolean) obj2).Value;
                }
                if (!(obj2 is PdfBooleanObject))
                {
                    throw new InvalidCastException("GetBoolean: Object is not a boolean.");
                }
                return ((PdfBooleanObject) obj2).Value;
            }

            public PdfDictionary GetDictionary(int index) => 
                (this.GetObject(index) as PdfDictionary);

            public IEnumerator<PdfItem> GetEnumerator() => 
                this.elements.GetEnumerator();

            [Obsolete("Use GetObject, GetDictionary, GetArray, or GetReference")]
            public PdfObject GetIndirectObject(int index)
            {
                if ((index < 0) || (index >= this.Count))
                {
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);
                }
                PdfItem item = this[index];
                if (item is PdfReference)
                {
                    return ((PdfReference) item).Value;
                }
                return null;
            }

            public int GetInteger(int index)
            {
                if ((index < 0) || (index >= this.Count))
                {
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);
                }
                object obj2 = this[index];
                if (obj2 == null)
                {
                    return 0;
                }
                if (obj2 is PdfInteger)
                {
                    return ((PdfInteger) obj2).Value;
                }
                if (!(obj2 is PdfIntegerObject))
                {
                    throw new InvalidCastException("GetInteger: Object is not an integer.");
                }
                return ((PdfIntegerObject) obj2).Value;
            }

            public string GetName(int index)
            {
                if ((index < 0) || (index >= this.Count))
                {
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);
                }
                object obj2 = this[index];
                if (obj2 == null)
                {
                    return "";
                }
                if (obj2 is PdfName)
                {
                    return ((PdfName) obj2).Value;
                }
                if (!(obj2 is PdfNameObject))
                {
                    throw new InvalidCastException("GetName: Object is not an integer.");
                }
                return ((PdfNameObject) obj2).Value;
            }

            public PdfObject GetObject(int index)
            {
                if ((index < 0) || (index >= this.Count))
                {
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);
                }
                PdfItem item = this[index];
                if (item is PdfReference)
                {
                    return ((PdfReference) item).Value;
                }
                return (item as PdfObject);
            }

            public double GetReal(int index)
            {
                if ((index < 0) || (index >= this.Count))
                {
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);
                }
                object obj2 = this[index];
                if (obj2 == null)
                {
                    return 0.0;
                }
                if (obj2 is PdfReal)
                {
                    return ((PdfReal) obj2).Value;
                }
                if (obj2 is PdfRealObject)
                {
                    return ((PdfRealObject) obj2).Value;
                }
                if (obj2 is PdfInteger)
                {
                    return (double) ((PdfInteger) obj2).Value;
                }
                if (!(obj2 is PdfIntegerObject))
                {
                    throw new InvalidCastException("GetReal: Object is not a number.");
                }
                return (double) ((PdfIntegerObject) obj2).Value;
            }

            public PdfReference GetReference(int index)
            {
                PdfItem item = this[index];
                return (item as PdfReference);
            }

            public string GetString(int index)
            {
                if ((index < 0) || (index >= this.Count))
                {
                    throw new ArgumentOutOfRangeException("index", index, PSSR.IndexOutOfRange);
                }
                object obj2 = this[index];
                if (obj2 == null)
                {
                    return "";
                }
                if (obj2 is PdfString)
                {
                    return ((PdfString) obj2).Value;
                }
                if (!(obj2 is PdfStringObject))
                {
                    throw new InvalidCastException("GetString: Object is not an integer.");
                }
                return ((PdfStringObject) obj2).Value;
            }

            public int IndexOf(PdfItem value) => 
                this.elements.IndexOf(value);

            public void Insert(int index, PdfItem value)
            {
                this.elements.Insert(index, value);
            }

            public bool Remove(PdfItem item) => 
                this.elements.Remove(item);

            public void RemoveAt(int index)
            {
                this.elements.RemoveAt(index);
            }

            internal void SetOwner(PdfArray array)
            {
                this.owner = array;
                array.elements = this;
            }

            IEnumerator IEnumerable.GetEnumerator() => 
                this.elements.GetEnumerator();

            object ICloneable.Clone()
            {
                PdfArray.ArrayElements elements = (PdfArray.ArrayElements) base.MemberwiseClone();
                elements.elements = new List<PdfItem>(elements.elements);
                elements.owner = null;
                return elements;
            }

            public int Count =>
                this.elements.Count;

            public bool IsFixedSize =>
                false;

            public bool IsReadOnly =>
                false;

            public bool IsSynchronized =>
                false;

            public PdfItem this[int index]
            {
                get => 
                    this.elements[index];
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("value");
                    }
                    this.elements[index] = value;
                }
            }

            public PdfItem[] Items =>
                this.elements.ToArray();

            public object SyncRoot =>
                null;
        }
    }
}

