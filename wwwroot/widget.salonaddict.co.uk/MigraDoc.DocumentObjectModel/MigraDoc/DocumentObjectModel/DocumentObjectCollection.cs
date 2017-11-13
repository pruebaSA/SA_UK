namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;
    using System.Collections;
    using System.Reflection;

    public abstract class DocumentObjectCollection : DocumentObject, IList, ICollection, IEnumerable, IVisitable
    {
        private ArrayList elements;

        internal DocumentObjectCollection()
        {
            this.elements = new ArrayList();
        }

        internal DocumentObjectCollection(DocumentObject parent) : base(parent)
        {
            this.elements = new ArrayList();
        }

        public virtual void Add(DocumentObject value)
        {
            base.SetParent(value);
            this.elements.Add(value);
        }

        public void Clear()
        {
            ((IList) this).Clear();
        }

        public DocumentObjectCollection Clone() => 
            ((DocumentObjectCollection) this.DeepCopy());

        public void CopyTo(Array array, int index)
        {
            this.elements.CopyTo(array, index);
        }

        protected override object DeepCopy()
        {
            DocumentObjectCollection objects = (DocumentObjectCollection) base.DeepCopy();
            int count = this.Count;
            objects.elements = new ArrayList(count);
            for (int i = 0; i < count; i++)
            {
                DocumentObject obj2 = this[i];
                if (obj2 != null)
                {
                    obj2 = obj2.Clone() as DocumentObject;
                    obj2.parent = objects;
                }
                objects.elements.Add(obj2);
            }
            return objects;
        }

        public IEnumerator GetEnumerator() => 
            this.elements.GetEnumerator();

        public int IndexOf(DocumentObject val) => 
            ((IList) this).IndexOf(val);

        public virtual void InsertObject(int index, DocumentObject val)
        {
            base.SetParent(val);
            ((IList) this).Insert(index, val);
            int count = this.Count;
            for (int i = index + 1; i < count; i++)
            {
                ((DocumentObject) ((IList) this)[i]).ResetCachedValues();
            }
        }

        public override bool IsNull()
        {
            if (!this.Meta.IsNull(this))
            {
                return false;
            }
            if (this.elements != null)
            {
                foreach (DocumentObject obj2 in this.elements)
                {
                    if ((obj2 != null) && !obj2.IsNull())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitDocumentObjectCollection(this);
            foreach (DocumentObject obj2 in this)
            {
                IVisitable visitable = obj2 as IVisitable;
                if (visitable != null)
                {
                    visitable.AcceptVisitor(visitor, visitChildren);
                }
            }
        }

        public void RemoveObjectAt(int index)
        {
            ((IList) this).RemoveAt(index);
            int count = this.Count;
            for (int i = index; i < count; i++)
            {
                ((DocumentObject) ((IList) this)[i]).ResetCachedValues();
            }
        }

        int IList.Add(object value) => 
            this.elements.Add(value);

        void IList.Clear()
        {
            this.elements.Clear();
        }

        bool IList.Contains(object value) => 
            this.elements.Contains(value);

        int IList.IndexOf(object value) => 
            this.elements.IndexOf(value);

        void IList.Insert(int index, object value)
        {
            this.elements.Insert(index, value);
        }

        void IList.Remove(object value)
        {
            this.elements.Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            this.elements.RemoveAt(index);
        }

        public int Count =>
            this.elements.Count;

        public DocumentObject First
        {
            get
            {
                if (this.Count > 0)
                {
                    return this[0];
                }
                return null;
            }
        }

        public virtual DocumentObject this[int index]
        {
            get => 
                (this.elements[index] as DocumentObject);
            set
            {
                base.SetParent(value);
                this.elements[index] = value;
            }
        }

        public DocumentObject LastObject
        {
            get
            {
                int count = this.elements.Count;
                if (count > 0)
                {
                    return (DocumentObject) this.elements[count - 1];
                }
                return null;
            }
        }

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot =>
            null;

        bool IList.IsFixedSize =>
            false;

        bool IList.IsReadOnly =>
            false;

        object IList.this[int index]
        {
            get => 
                this.elements[index];
            set
            {
                this.elements[index] = value;
            }
        }
    }
}

