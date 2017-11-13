namespace System.Web.UI.Design
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Reflection;

    public sealed class DesignerAutoFormatCollection : IList, ICollection, IEnumerable
    {
        private ArrayList _autoFormats = new ArrayList();

        public int Add(DesignerAutoFormat format) => 
            this._autoFormats.Add(format);

        public void Clear()
        {
            this._autoFormats.Clear();
        }

        public bool Contains(DesignerAutoFormat format) => 
            this._autoFormats.Contains(format);

        public int IndexOf(DesignerAutoFormat format) => 
            this._autoFormats.IndexOf(format);

        public void Insert(int index, DesignerAutoFormat format)
        {
            this._autoFormats.Insert(index, format);
        }

        public void Remove(DesignerAutoFormat format)
        {
            this._autoFormats.Remove(format);
        }

        public void RemoveAt(int index)
        {
            this._autoFormats.RemoveAt(index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this._autoFormats.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this._autoFormats.GetEnumerator();

        int IList.Add(object value)
        {
            if (value is DesignerAutoFormat)
            {
                return this.Add((DesignerAutoFormat) value);
            }
            return -1;
        }

        bool IList.Contains(object value) => 
            ((value is DesignerAutoFormat) && this.Contains((DesignerAutoFormat) value));

        int IList.IndexOf(object value) => 
            this.IndexOf((DesignerAutoFormat) value);

        void IList.Insert(int index, object value)
        {
            if (value is DesignerAutoFormat)
            {
                this.Insert(index, (DesignerAutoFormat) value);
            }
        }

        void IList.Remove(object value)
        {
            if (value is DesignerAutoFormat)
            {
                this.Remove((DesignerAutoFormat) value);
            }
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        public int Count =>
            this._autoFormats.Count;

        public DesignerAutoFormat this[int index] =>
            ((DesignerAutoFormat) this._autoFormats[index]);

        public Size PreviewSize
        {
            get
            {
                int height = 200;
                int width = 200;
                foreach (DesignerAutoFormat format in this._autoFormats)
                {
                    int num3 = (int) format.Style.Height.Value;
                    if (num3 > height)
                    {
                        height = num3;
                    }
                    int num4 = (int) format.Style.Width.Value;
                    if (num4 > width)
                    {
                        width = num4;
                    }
                }
                return new Size(width, height);
            }
        }

        public object SyncRoot =>
            this;

        int ICollection.Count =>
            this.Count;

        bool ICollection.IsSynchronized =>
            false;

        bool IList.IsFixedSize =>
            false;

        bool IList.IsReadOnly =>
            false;

        object IList.this[int index]
        {
            get => 
                this._autoFormats[index];
            set
            {
                if (value is DesignerAutoFormat)
                {
                    this._autoFormats[index] = value;
                }
            }
        }
    }
}

