namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;

    public abstract class DocumentObject : ICloneable
    {
        [DV(RefOnly=true)]
        protected internal DocumentObject parent;
        private object tag;

        internal DocumentObject()
        {
        }

        internal DocumentObject(DocumentObject parent)
        {
            this.parent = parent;
        }

        public object Clone() => 
            this.DeepCopy();

        public object CreateValue(string name)
        {
            ValueDescriptor descriptor = this.Meta[name];
            if (descriptor != null)
            {
                return descriptor.CreateValue();
            }
            return null;
        }

        protected virtual object DeepCopy()
        {
            DocumentObject obj2 = (DocumentObject) base.MemberwiseClone();
            obj2.parent = null;
            return obj2;
        }

        public virtual object GetValue(string name) => 
            this.GetValue(name, GV.ReadWrite);

        public virtual object GetValue(string name, GV flags) => 
            this.Meta.GetValue(this, name, flags);

        public virtual bool HasValue(string name) => 
            this.Meta.HasValue(name);

        public virtual bool IsNull() => 
            this.Meta.IsNull(this);

        public virtual bool IsNull(string name) => 
            this.Meta.IsNull(this, name);

        internal virtual void ResetCachedValues()
        {
        }

        internal abstract void Serialize(Serializer serializer);
        public virtual void SetNull()
        {
            this.Meta.SetNull(this);
        }

        public virtual void SetNull(string name)
        {
            this.Meta.SetNull(this, name);
        }

        protected void SetParent(DocumentObject val)
        {
            if (val != null)
            {
                if (val.Parent != null)
                {
                    throw new ArgumentException(DomSR.ParentAlreadySet(val, this));
                }
                val.parent = this;
            }
        }

        public virtual void SetValue(string name, object val)
        {
            this.Meta.SetValue(this, name, val);
            if (val is DocumentObject)
            {
                ((DocumentObject) val).parent = this;
            }
        }

        public MigraDoc.DocumentObjectModel.Document Document
        {
            get
            {
                for (DocumentObject obj2 = this.Parent; obj2 != null; obj2 = obj2.parent)
                {
                    MigraDoc.DocumentObjectModel.Document document = obj2 as MigraDoc.DocumentObjectModel.Document;
                    if (document != null)
                    {
                        return document;
                    }
                }
                return null;
            }
        }

        internal abstract MigraDoc.DocumentObjectModel.Internals.Meta Meta { get; }

        internal DocumentObject Parent =>
            this.parent;

        public MigraDoc.DocumentObjectModel.Section Section
        {
            get
            {
                for (DocumentObject obj2 = this.Parent; obj2 != null; obj2 = obj2.parent)
                {
                    MigraDoc.DocumentObjectModel.Section section = obj2 as MigraDoc.DocumentObjectModel.Section;
                    if (section != null)
                    {
                        return section;
                    }
                }
                return null;
            }
        }

        public object Tag
        {
            get => 
                this.tag;
            set
            {
                this.tag = value;
            }
        }
    }
}

