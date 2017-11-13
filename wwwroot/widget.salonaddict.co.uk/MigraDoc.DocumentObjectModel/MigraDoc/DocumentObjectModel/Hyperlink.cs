namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Fields;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    public class Hyperlink : DocumentObject, IVisitable
    {
        [DV(ItemType=typeof(DocumentObject))]
        internal ParagraphElements elements;
        [DV]
        internal MigraDoc.DocumentObjectModel.Font font;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString name;
        [DV(Type=typeof(HyperlinkType))]
        internal NEnum type;

        public Hyperlink()
        {
            this.name = NString.NullValue;
            this.type = NEnum.NullValue(typeof(HyperlinkType));
        }

        internal Hyperlink(DocumentObject parent) : base(parent)
        {
            this.name = NString.NullValue;
            this.type = NEnum.NullValue(typeof(HyperlinkType));
        }

        internal Hyperlink(string name, string text) : this()
        {
            this.Name = name;
            this.Elements.AddText(text);
        }

        internal Hyperlink(string name, HyperlinkType type, string text) : this()
        {
            this.Name = name;
            this.Type = type;
            this.Elements.AddText(text);
        }

        public void AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitHyperlink(this);
            if (visitChildren && (this.elements != null))
            {
                ((IVisitable) this.elements).AcceptVisitor(visitor, visitChildren);
            }
        }

        public void Add(Character character)
        {
            this.Elements.Add(character);
        }

        public void Add(BookmarkField bookmark)
        {
            this.Elements.Add(bookmark);
        }

        public void Add(DateField dateField)
        {
            this.Elements.Add(dateField);
        }

        public void Add(InfoField infoField)
        {
            this.Elements.Add(infoField);
        }

        public void Add(NumPagesField numPagesField)
        {
            this.Elements.Add(numPagesField);
        }

        public void Add(PageField pageField)
        {
            this.Elements.Add(pageField);
        }

        public void Add(PageRefField pageRefField)
        {
            this.Elements.Add(pageRefField);
        }

        public void Add(SectionField sectionField)
        {
            this.Elements.Add(sectionField);
        }

        public void Add(SectionPagesField sectionPagesField)
        {
            this.Elements.Add(sectionPagesField);
        }

        public void Add(Footnote footnote)
        {
            this.Elements.Add(footnote);
        }

        public void Add(FormattedText formattedText)
        {
            this.Elements.Add(formattedText);
        }

        public void Add(Image image)
        {
            this.Elements.Add(image);
        }

        public void Add(Text text)
        {
            this.Elements.Add(text);
        }

        public BookmarkField AddBookmark(string name) => 
            this.Elements.AddBookmark(name);

        public Text AddChar(char ch) => 
            this.Elements.AddChar(ch);

        public Text AddChar(char ch, int count) => 
            this.Elements.AddChar(ch, count);

        public Character AddCharacter(SymbolName symbolType) => 
            this.Elements.AddCharacter(symbolType);

        public Character AddCharacter(char ch) => 
            this.Elements.AddCharacter(ch);

        public Character AddCharacter(SymbolName symbolType, int count) => 
            this.Elements.AddCharacter(symbolType, count);

        public Character AddCharacter(char ch, int count) => 
            this.Elements.AddCharacter(ch, count);

        public DateField AddDateField() => 
            this.Elements.AddDateField();

        public DateField AddDateField(string format) => 
            this.Elements.AddDateField(format);

        public Footnote AddFootnote() => 
            this.Elements.AddFootnote();

        public Footnote AddFootnote(string text) => 
            this.Elements.AddFootnote(text);

        public FormattedText AddFormattedText() => 
            this.Elements.AddFormattedText();

        public FormattedText AddFormattedText(MigraDoc.DocumentObjectModel.Font font) => 
            this.Elements.AddFormattedText(font);

        public FormattedText AddFormattedText(TextFormat textFormat) => 
            this.Elements.AddFormattedText(textFormat);

        public FormattedText AddFormattedText(string text) => 
            this.Elements.AddFormattedText(text);

        public FormattedText AddFormattedText(string text, MigraDoc.DocumentObjectModel.Font font) => 
            this.Elements.AddFormattedText(text, font);

        public FormattedText AddFormattedText(string text, TextFormat textFormat) => 
            this.Elements.AddFormattedText(text, textFormat);

        public FormattedText AddFormattedText(string text, string style) => 
            this.Elements.AddFormattedText(text, style);

        public Image AddImage(string fileName) => 
            this.Elements.AddImage(fileName);

        public InfoField AddInfoField(InfoFieldType iType) => 
            this.Elements.AddInfoField(iType);

        public NumPagesField AddNumPagesField() => 
            this.Elements.AddNumPagesField();

        public PageField AddPageField() => 
            this.Elements.AddPageField();

        public PageRefField AddPageRefField(string name) => 
            this.Elements.AddPageRefField(name);

        public SectionField AddSectionField() => 
            this.Elements.AddSectionField();

        public SectionPagesField AddSectionPagesField() => 
            this.Elements.AddSectionPagesField();

        public Character AddSpace(int count) => 
            this.Elements.AddSpace(count);

        public void AddTab()
        {
            this.Elements.AddTab();
        }

        public Text AddText(string text) => 
            this.Elements.AddText(text);

        public Hyperlink Clone() => 
            ((Hyperlink) this.DeepCopy());

        protected override object DeepCopy()
        {
            Hyperlink hyperlink = (Hyperlink) base.DeepCopy();
            if (hyperlink.elements != null)
            {
                hyperlink.elements = hyperlink.elements.Clone();
                hyperlink.elements.parent = hyperlink;
            }
            return hyperlink;
        }

        internal override void Serialize(Serializer serializer)
        {
            if (this.name.Value == string.Empty)
            {
                throw new InvalidOperationException(DomSR.MissingObligatoryProperty("Name", "Hyperlink"));
            }
            serializer.Write(@"\hyperlink");
            string str = "[Name = \"" + this.Name.Replace(@"\", @"\\").Replace("\"", "\\\"") + "\"";
            if (!this.type.IsNull)
            {
                str = str + " Type = " + this.Type;
            }
            str = str + "]";
            serializer.Write(str);
            serializer.Write("{");
            if (this.elements != null)
            {
                this.elements.Serialize(serializer);
            }
            serializer.Write("}");
        }

        public ParagraphElements Elements
        {
            get
            {
                if (this.elements == null)
                {
                    this.elements = new ParagraphElements(this);
                }
                return this.elements;
            }
            set
            {
                base.SetParent(value);
                this.elements = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Font Font
        {
            get
            {
                if (this.font == null)
                {
                    this.font = new MigraDoc.DocumentObjectModel.Font(this);
                }
                return this.font;
            }
            set
            {
                base.SetParent(value);
                this.font = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Hyperlink));
                }
                return meta;
            }
        }

        public string Name
        {
            get => 
                this.name.Value;
            set
            {
                this.name.Value = value;
            }
        }

        public HyperlinkType Type
        {
            get => 
                ((HyperlinkType) this.type.Value);
            set
            {
                this.type.Value = (int) value;
            }
        }
    }
}

