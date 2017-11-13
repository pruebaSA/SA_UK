namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Fields;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    public class FormattedText : DocumentObject, IVisitable
    {
        [DV(ItemType=typeof(DocumentObject))]
        internal ParagraphElements elements;
        [DV]
        internal MigraDoc.DocumentObjectModel.Font font;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString style;

        public FormattedText()
        {
            this.style = NString.NullValue;
        }

        internal FormattedText(DocumentObject parent) : base(parent)
        {
            this.style = NString.NullValue;
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

        public void Add(Hyperlink hyperlink)
        {
            this.Elements.Add(hyperlink);
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

        public Hyperlink AddHyperlink(string name) => 
            this.Elements.AddHyperlink(name);

        public Hyperlink AddHyperlink(string name, HyperlinkType type) => 
            this.Elements.AddHyperlink(name, type);

        public Image AddImage(string fileName) => 
            this.Elements.AddImage(fileName);

        public InfoField AddInfoField(InfoFieldType iType) => 
            this.Elements.AddInfoField(iType);

        public void AddLineBreak()
        {
            this.Elements.AddLineBreak();
        }

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

        public FormattedText Clone() => 
            ((FormattedText) this.DeepCopy());

        protected override object DeepCopy()
        {
            FormattedText text = (FormattedText) base.DeepCopy();
            if (text.font != null)
            {
                text.font = text.font.Clone();
                text.font.parent = text;
            }
            if (text.elements != null)
            {
                text.elements = text.elements.Clone();
                text.elements.parent = text;
            }
            return text;
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitFormattedText(this);
            if (visitChildren && (this.elements != null))
            {
                ((IVisitable) this.elements).AcceptVisitor(visitor, visitChildren);
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            bool flag = false;
            if (!this.IsNull("Font"))
            {
                this.Font.Serialize(serializer);
                flag = true;
            }
            else if (!this.style.IsNull)
            {
                serializer.Write("\\font(\"" + this.Style + "\")");
                flag = true;
            }
            if (flag)
            {
                serializer.Write("{");
            }
            if (!this.IsNull("Elements"))
            {
                this.Elements.Serialize(serializer);
            }
            if (flag)
            {
                serializer.Write("}");
            }
        }

        [DV]
        public bool Bold
        {
            get => 
                this.Font.Bold;
            set
            {
                this.Font.Bold = value;
            }
        }

        [DV]
        public MigraDoc.DocumentObjectModel.Color Color
        {
            get => 
                this.Font.Color;
            set
            {
                this.Font.Color = value;
            }
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

        [DV]
        public string FontName
        {
            get => 
                this.Font.Name;
            set
            {
                this.Font.Name = value;
            }
        }

        [DV]
        public bool Italic
        {
            get => 
                this.Font.Italic;
            set
            {
                this.Font.Italic = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(FormattedText));
                }
                return meta;
            }
        }

        [DV]
        internal string Name
        {
            get => 
                this.Font.Name;
            set
            {
                this.Font.Name = value;
            }
        }

        [DV]
        public Unit Size
        {
            get => 
                this.Font.Size;
            set
            {
                this.Font.Size = value;
            }
        }

        public string Style
        {
            get => 
                this.style.Value;
            set
            {
                this.style.Value = value;
            }
        }

        [DV]
        public bool Subscript
        {
            get => 
                this.Font.Subscript;
            set
            {
                this.Font.Subscript = value;
            }
        }

        [DV]
        public bool Superscript
        {
            get => 
                this.Font.Superscript;
            set
            {
                this.Font.Superscript = value;
            }
        }

        [DV]
        public MigraDoc.DocumentObjectModel.Underline Underline
        {
            get => 
                this.Font.Underline;
            set
            {
                this.Font.Underline = value;
            }
        }
    }
}

