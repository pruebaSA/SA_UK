namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    public class HeadersFooters : DocumentObject, IVisitable
    {
        [DV]
        internal HeaderFooter evenPage;
        [DV]
        internal HeaderFooter firstPage;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal HeaderFooter primary;

        public HeadersFooters()
        {
        }

        public HeadersFooters(DocumentObject parent) : base(parent)
        {
        }

        public HeadersFooters Clone() => 
            ((HeadersFooters) this.DeepCopy());

        protected override object DeepCopy()
        {
            HeadersFooters footers = (HeadersFooters) base.DeepCopy();
            if (footers.evenPage != null)
            {
                footers.evenPage = footers.evenPage.Clone();
                footers.evenPage.parent = footers;
            }
            if (footers.firstPage != null)
            {
                footers.firstPage = footers.firstPage.Clone();
                footers.firstPage.parent = footers;
            }
            if (footers.primary != null)
            {
                footers.primary = footers.primary.Clone();
                footers.primary.parent = footers;
            }
            return footers;
        }

        public bool HasHeaderFooter(HeaderFooterIndex index) => 
            !this.IsNull(index.ToString());

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitHeadersFooters(this);
            if (visitChildren)
            {
                if (this.HasHeaderFooter(HeaderFooterIndex.Primary))
                {
                    ((IVisitable) this.primary).AcceptVisitor(visitor, visitChildren);
                }
                if (this.HasHeaderFooter(HeaderFooterIndex.EvenPage))
                {
                    ((IVisitable) this.evenPage).AcceptVisitor(visitor, visitChildren);
                }
                if (this.HasHeaderFooter(HeaderFooterIndex.FirstPage))
                {
                    ((IVisitable) this.firstPage).AcceptVisitor(visitor, visitChildren);
                }
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            bool flag = this.HasHeaderFooter(HeaderFooterIndex.Primary);
            bool flag2 = this.HasHeaderFooter(HeaderFooterIndex.EvenPage);
            bool flag3 = this.HasHeaderFooter(HeaderFooterIndex.FirstPage);
            if (flag)
            {
                this.Primary.Serialize(serializer, "primary");
            }
            if (flag2)
            {
                this.EvenPage.Serialize(serializer, "evenpage");
            }
            if (flag3)
            {
                this.FirstPage.Serialize(serializer, "firstpage");
            }
        }

        public HeaderFooter EvenPage
        {
            get
            {
                if (this.evenPage == null)
                {
                    this.evenPage = new HeaderFooter(this);
                }
                return this.evenPage;
            }
            set
            {
                base.SetParent(value);
                this.evenPage = value;
            }
        }

        public HeaderFooter FirstPage
        {
            get
            {
                if (this.firstPage == null)
                {
                    this.firstPage = new HeaderFooter(this);
                }
                return this.firstPage;
            }
            set
            {
                base.SetParent(value);
                this.firstPage = value;
            }
        }

        public bool IsFooter =>
            !this.IsHeader;

        public bool IsHeader
        {
            get
            {
                Section parent = (Section) base.parent;
                return (parent.headers == this);
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(HeadersFooters));
                }
                return meta;
            }
        }

        public HeaderFooter Primary
        {
            get
            {
                if (this.primary == null)
                {
                    this.primary = new HeaderFooter(this);
                }
                return this.primary;
            }
            set
            {
                base.SetParent(value);
                this.primary = value;
            }
        }
    }
}

