namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    public sealed class Document : DocumentObject, IVisitable
    {
        [DV]
        internal NString comment = NString.NullValue;
        internal string ddlFile = "";
        [DV]
        internal Unit defaultTabStop = Unit.NullValue;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.FootnoteLocation))]
        internal NEnum footnoteLocation = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.FootnoteLocation));
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.FootnoteNumberingRule))]
        internal NEnum footnoteNumberingRule = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.FootnoteNumberingRule));
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.FootnoteNumberStyle))]
        internal NEnum footnoteNumberStyle = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.FootnoteNumberStyle));
        [DV]
        internal NInt footnoteStartingNumber = NInt.NullValue;
        [DV]
        internal NString imagePath = NString.NullValue;
        [DV]
        internal DocumentInfo info;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        private object renderer;
        [DV]
        internal MigraDoc.DocumentObjectModel.Sections sections;
        [DV]
        internal MigraDoc.DocumentObjectModel.Styles styles;
        [DV]
        internal NBool useCmykColor = NBool.NullValue;

        public Document()
        {
            this.styles = new MigraDoc.DocumentObjectModel.Styles(this);
        }

        public void Add(Section section)
        {
            this.Sections.Add(section);
        }

        public void Add(Style style)
        {
            this.Styles.Add(style);
        }

        public Section AddSection() => 
            this.Sections.AddSection();

        public Style AddStyle(string name, string baseStyle)
        {
            if ((name == null) || (baseStyle == null))
            {
                throw new ArgumentNullException((name == null) ? "name" : "baseStyle");
            }
            if ((name == "") || (baseStyle == ""))
            {
                throw new ArgumentException((name == "") ? "name" : "baseStyle");
            }
            return this.Styles.AddStyle(name, baseStyle);
        }

        public void BindToRenderer(object renderer)
        {
            if (((this.renderer != null) && (renderer != null)) && !object.ReferenceEquals(this.renderer, renderer))
            {
                throw new InvalidOperationException("The document is already bound to another renderer. A MigraDoc document can be rendered by only one renderer, because the rendering process modifies its internal structure. If you want to render a MigraDoc document  on different renderers, you must create a copy of it using the Clone function.");
            }
            this.renderer = renderer;
        }

        public Document Clone() => 
            ((Document) this.DeepCopy());

        protected override object DeepCopy()
        {
            Document document = (Document) base.DeepCopy();
            if (document.info != null)
            {
                document.info = document.info.Clone();
                document.info.parent = document;
            }
            if (document.styles != null)
            {
                document.styles = document.styles.Clone();
                document.styles.parent = document;
            }
            if (document.sections != null)
            {
                document.sections = document.sections.Clone();
                document.sections.parent = document;
            }
            return document;
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitDocument(this);
            if (visitChildren)
            {
                ((IVisitable) this.Styles).AcceptVisitor(visitor, visitChildren);
                ((IVisitable) this.Sections).AcceptVisitor(visitor, visitChildren);
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            serializer.WriteLine(@"\document");
            int pos = serializer.BeginAttributes();
            if (!this.IsNull("Info"))
            {
                this.Info.Serialize(serializer);
            }
            if (!this.defaultTabStop.IsNull)
            {
                serializer.WriteSimpleAttribute("DefaultTabStop", this.DefaultTabStop);
            }
            if (!this.footnoteLocation.IsNull)
            {
                serializer.WriteSimpleAttribute("FootnoteLocation", this.FootnoteLocation);
            }
            if (!this.footnoteNumberingRule.IsNull)
            {
                serializer.WriteSimpleAttribute("FootnoteNumberingRule", this.FootnoteNumberingRule);
            }
            if (!this.footnoteNumberStyle.IsNull)
            {
                serializer.WriteSimpleAttribute("FootnoteNumberStyle", this.FootnoteNumberStyle);
            }
            if (!this.footnoteStartingNumber.IsNull)
            {
                serializer.WriteSimpleAttribute("FootnoteStartingNumber", this.FootnoteStartingNumber);
            }
            if (!this.imagePath.IsNull)
            {
                serializer.WriteSimpleAttribute("ImagePath", this.ImagePath);
            }
            if (!this.useCmykColor.IsNull)
            {
                serializer.WriteSimpleAttribute("UseCmykColor", this.UseCmykColor);
            }
            serializer.EndAttributes(pos);
            serializer.BeginContent();
            this.Styles.Serialize(serializer);
            if (!this.IsNull("Sections"))
            {
                this.Sections.Serialize(serializer);
            }
            serializer.EndContent();
            serializer.Flush();
        }

        public string Comment
        {
            get => 
                this.comment.Value;
            set
            {
                this.comment.Value = value;
            }
        }

        public string DdlFile =>
            this.ddlFile;

        public PageSetup DefaultPageSetup =>
            PageSetup.DefaultPageSetup;

        public Unit DefaultTabStop
        {
            get => 
                this.defaultTabStop;
            set
            {
                this.defaultTabStop = value;
            }
        }

        public MigraDoc.DocumentObjectModel.FootnoteLocation FootnoteLocation
        {
            get => 
                ((MigraDoc.DocumentObjectModel.FootnoteLocation) this.footnoteLocation.Value);
            set
            {
                this.footnoteLocation.Value = (int) value;
            }
        }

        public MigraDoc.DocumentObjectModel.FootnoteNumberingRule FootnoteNumberingRule
        {
            get => 
                ((MigraDoc.DocumentObjectModel.FootnoteNumberingRule) this.footnoteNumberingRule.Value);
            set
            {
                this.footnoteNumberingRule.Value = (int) value;
            }
        }

        public MigraDoc.DocumentObjectModel.FootnoteNumberStyle FootnoteNumberStyle
        {
            get => 
                ((MigraDoc.DocumentObjectModel.FootnoteNumberStyle) this.footnoteNumberStyle.Value);
            set
            {
                this.footnoteNumberStyle.Value = (int) value;
            }
        }

        public int FootnoteStartingNumber
        {
            get => 
                this.footnoteStartingNumber.Value;
            set
            {
                this.footnoteStartingNumber.Value = value;
            }
        }

        public string ImagePath
        {
            get => 
                this.imagePath.Value;
            set
            {
                this.imagePath.Value = value;
            }
        }

        public DocumentInfo Info
        {
            get
            {
                if (this.info == null)
                {
                    this.info = new DocumentInfo(this);
                }
                return this.info;
            }
            set
            {
                base.SetParent(value);
                this.info = value;
            }
        }

        public bool IsBoundToRenderer =>
            (this.renderer != null);

        public Section LastSection
        {
            get
            {
                if ((this.sections != null) && (this.sections.Count > 0))
                {
                    return (this.sections.LastObject as Section);
                }
                return null;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Document));
                }
                return meta;
            }
        }

        public MigraDoc.DocumentObjectModel.Sections Sections
        {
            get
            {
                if (this.sections == null)
                {
                    this.sections = new MigraDoc.DocumentObjectModel.Sections(this);
                }
                return this.sections;
            }
            set
            {
                base.SetParent(value);
                this.sections = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Styles Styles
        {
            get
            {
                if (this.styles == null)
                {
                    this.styles = new MigraDoc.DocumentObjectModel.Styles(this);
                }
                return this.styles;
            }
            set
            {
                base.SetParent(value);
                this.styles = value;
            }
        }

        public bool UseCmykColor
        {
            get => 
                this.useCmykColor.Value;
            set
            {
                this.useCmykColor.Value = value;
            }
        }
    }
}

