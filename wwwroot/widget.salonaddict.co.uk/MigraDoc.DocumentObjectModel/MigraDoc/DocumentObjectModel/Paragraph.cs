namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Fields;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;
    using System.Collections;

    public class Paragraph : DocumentObject, IVisitable
    {
        [DV]
        internal NString comment;
        [DV]
        internal ParagraphElements elements;
        [DV]
        internal ParagraphFormat format;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        private bool serializeContentOnly;
        [DV]
        internal NString style;

        public Paragraph()
        {
            this.style = NString.NullValue;
            this.comment = NString.NullValue;
        }

        internal Paragraph(DocumentObject parent) : base(parent)
        {
            this.style = NString.NullValue;
            this.comment = NString.NullValue;
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

        public FormattedText AddFormattedText(Font font) => 
            this.Elements.AddFormattedText(font);

        public FormattedText AddFormattedText(TextFormat textFormat) => 
            this.Elements.AddFormattedText(textFormat);

        public FormattedText AddFormattedText(string text) => 
            this.Elements.AddFormattedText(text);

        public FormattedText AddFormattedText(string text, Font font) => 
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

        public Paragraph Clone() => 
            ((Paragraph) this.DeepCopy());

        protected override object DeepCopy()
        {
            Paragraph paragraph = (Paragraph) base.DeepCopy();
            if (paragraph.format != null)
            {
                paragraph.format = paragraph.format.Clone();
                paragraph.format.parent = paragraph;
            }
            if (paragraph.elements != null)
            {
                paragraph.elements = paragraph.elements.Clone();
                paragraph.elements.parent = paragraph;
            }
            return paragraph;
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitParagraph(this);
            if (visitChildren && (this.elements != null))
            {
                ((IVisitable) this.elements).AcceptVisitor(visitor, visitChildren);
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            if (!this.serializeContentOnly)
            {
                serializer.WriteComment(this.comment.Value);
                serializer.WriteLine(@"\paragraph");
                int pos = serializer.BeginAttributes();
                if (this.style.Value != "")
                {
                    serializer.WriteLine("Style = \"" + this.style.Value + "\"");
                }
                if (!this.IsNull("Format"))
                {
                    this.format.Serialize(serializer, "Format", null);
                }
                serializer.EndAttributes(pos);
                serializer.BeginContent();
                if (!this.IsNull("Elements"))
                {
                    this.Elements.Serialize(serializer);
                }
                serializer.CloseUpLine();
                serializer.EndContent();
            }
            else
            {
                this.Elements.Serialize(serializer);
                serializer.CloseUpLine();
            }
        }

        internal Paragraph[] SplitOnParaBreak()
        {
            if (this.elements == null)
            {
                return null;
            }
            int startIdx = 0;
            ArrayList list = new ArrayList();
            for (int i = 0; i < this.Elements.Count; i++)
            {
                DocumentObject obj2 = this.Elements[i];
                if (obj2 is Character)
                {
                    Character character = (Character) obj2;
                    if (character.SymbolName == ((SymbolName) (-201326585)))
                    {
                        Paragraph paragraph = new Paragraph {
                            Format = this.Format.Clone(),
                            Style = this.Style,
                            Elements = this.SubsetElements(startIdx, i - 1)
                        };
                        startIdx = i + 1;
                        list.Add(paragraph);
                    }
                }
            }
            if (startIdx == 0)
            {
                return null;
            }
            Paragraph paragraph2 = new Paragraph {
                Format = this.Format.Clone(),
                Style = this.Style,
                Elements = this.SubsetElements(startIdx, this.elements.Count - 1)
            };
            list.Add(paragraph2);
            return (Paragraph[]) list.ToArray(typeof(Paragraph));
        }

        private ParagraphElements SubsetElements(int startIdx, int endIdx)
        {
            ParagraphElements elements = new ParagraphElements();
            for (int i = startIdx; i <= endIdx; i++)
            {
                elements.Add((DocumentObject) this.elements[i].Clone());
            }
            return elements;
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

        public ParagraphFormat Format
        {
            get
            {
                if (this.format == null)
                {
                    this.format = new ParagraphFormat(this);
                }
                return this.format;
            }
            set
            {
                base.SetParent(value);
                this.format = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Paragraph));
                }
                return meta;
            }
        }

        internal bool SerializeContentOnly
        {
            get => 
                this.serializeContentOnly;
            set
            {
                this.serializeContentOnly = value;
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
    }
}

