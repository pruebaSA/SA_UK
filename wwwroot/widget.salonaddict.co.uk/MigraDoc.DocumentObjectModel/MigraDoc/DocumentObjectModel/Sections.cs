namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;
    using System.Reflection;

    public class Sections : DocumentObjectCollection, IVisitable
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public Sections()
        {
        }

        internal Sections(DocumentObject parent) : base(parent)
        {
        }

        public Section AddSection()
        {
            Section section = new Section();
            this.Add(section);
            return section;
        }

        public Sections Clone() => 
            ((Sections) this.DeepCopy());

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitSections(this);
            foreach (Section section in this)
            {
                ((IVisitable) section).AcceptVisitor(visitor, visitChildren);
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                this[i].Serialize(serializer);
            }
        }

        public Section this[int index] =>
            (base[index] as Section);

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Sections));
                }
                return meta;
            }
        }
    }
}

