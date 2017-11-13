namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.XPath;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class XmlQueryNodeSequence : XmlQuerySequence<XPathNavigator>, IList<XPathItem>, ICollection<XPathItem>, IEnumerable<XPathItem>, IEnumerable
    {
        private XmlQueryNodeSequence docOrderDistinct;
        public static readonly XmlQueryNodeSequence Empty = new XmlQueryNodeSequence();

        public XmlQueryNodeSequence()
        {
        }

        public XmlQueryNodeSequence(IList<XPathNavigator> list) : base(list.Count)
        {
            for (int i = 0; i < list.Count; i++)
            {
                this.AddClone(list[i]);
            }
        }

        public XmlQueryNodeSequence(int capacity) : base(capacity)
        {
        }

        public XmlQueryNodeSequence(XPathNavigator navigator) : base(1)
        {
            this.AddClone(navigator);
        }

        public XmlQueryNodeSequence(XPathNavigator[] array, int size) : base(array, size)
        {
        }

        public void AddClone(XPathNavigator navigator)
        {
            base.Add(navigator.Clone());
        }

        public static XmlQueryNodeSequence CreateOrReuse(XmlQueryNodeSequence seq)
        {
            if (seq != null)
            {
                seq.Clear();
                return seq;
            }
            return new XmlQueryNodeSequence();
        }

        public static XmlQueryNodeSequence CreateOrReuse(XmlQueryNodeSequence seq, XPathNavigator navigator)
        {
            if (seq != null)
            {
                seq.Clear();
                seq.Add(navigator);
                return seq;
            }
            return new XmlQueryNodeSequence(navigator);
        }

        public XmlQueryNodeSequence DocOrderDistinct(IComparer<XPathNavigator> comparer)
        {
            if (this.docOrderDistinct == null)
            {
                int num;
                if (base.Count <= 1)
                {
                    return this;
                }
                XPathNavigator[] array = new XPathNavigator[base.Count];
                for (num = 0; num < array.Length; num++)
                {
                    array[num] = base[num];
                }
                Array.Sort<XPathNavigator>(array, 0, base.Count, comparer);
                int index = 0;
                for (num = 1; num < array.Length; num++)
                {
                    if (!array[index].IsSamePosition(array[num]))
                    {
                        index++;
                        if (index != num)
                        {
                            array[index] = array[num];
                        }
                    }
                }
                this.docOrderDistinct = new XmlQueryNodeSequence(array, index + 1);
                this.docOrderDistinct.docOrderDistinct = this.docOrderDistinct;
            }
            return this.docOrderDistinct;
        }

        protected override void OnItemsChanged()
        {
            this.docOrderDistinct = null;
        }

        void ICollection<XPathItem>.Add(XPathItem value)
        {
            throw new NotSupportedException();
        }

        void ICollection<XPathItem>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<XPathItem>.Contains(XPathItem value) => 
            (base.IndexOf((XPathNavigator) value) != -1);

        void ICollection<XPathItem>.CopyTo(XPathItem[] array, int index)
        {
            for (int i = 0; i < base.Count; i++)
            {
                array[index + i] = base[i];
            }
        }

        bool ICollection<XPathItem>.Remove(XPathItem value)
        {
            throw new NotSupportedException();
        }

        IEnumerator<XPathItem> IEnumerable<XPathItem>.GetEnumerator() => 
            new IListEnumerator<XPathItem>(this);

        int IList<XPathItem>.IndexOf(XPathItem value) => 
            base.IndexOf((XPathNavigator) value);

        void IList<XPathItem>.Insert(int index, XPathItem value)
        {
            throw new NotSupportedException();
        }

        void IList<XPathItem>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public bool IsDocOrderDistinct
        {
            get
            {
                if (this.docOrderDistinct != this)
                {
                    return (base.Count <= 1);
                }
                return true;
            }
            set
            {
                this.docOrderDistinct = value ? this : null;
            }
        }

        bool ICollection<XPathItem>.IsReadOnly =>
            true;

        XPathItem IList<XPathItem>.this[int index]
        {
            get
            {
                if (index >= base.Count)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return base[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}

