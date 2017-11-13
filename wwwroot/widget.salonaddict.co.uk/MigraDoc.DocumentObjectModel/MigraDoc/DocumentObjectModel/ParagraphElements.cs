namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Fields;
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using System;
    using System.Reflection;

    public class ParagraphElements : DocumentObjectCollection
    {
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public ParagraphElements()
        {
        }

        internal ParagraphElements(DocumentObject parent) : base(parent)
        {
        }

        public override void Add(DocumentObject docObj)
        {
            base.Add(docObj);
        }

        public BookmarkField AddBookmark(string name)
        {
            BookmarkField field = new BookmarkField {
                Name = name
            };
            this.Add(field);
            return field;
        }

        public Text AddChar(char ch) => 
            this.AddText(new string(ch, 1));

        public Text AddChar(char ch, int count) => 
            this.AddText(new string(ch, count));

        public Character AddCharacter(SymbolName symbolType) => 
            this.AddCharacter(symbolType, 1);

        public Character AddCharacter(char ch) => 
            this.AddCharacter((SymbolName) ch, 1);

        public Character AddCharacter(SymbolName symbolType, int count)
        {
            Character character = new Character();
            this.Add(character);
            character.SymbolName = symbolType;
            character.Count = count;
            return character;
        }

        public Character AddCharacter(char ch, int count) => 
            this.AddCharacter((SymbolName) ch, count);

        public DateField AddDateField()
        {
            DateField field = new DateField();
            this.Add(field);
            return field;
        }

        public DateField AddDateField(string format)
        {
            DateField field = new DateField {
                Format = format
            };
            this.Add(field);
            return field;
        }

        public Footnote AddFootnote()
        {
            Footnote footnote = new Footnote();
            this.Add(footnote);
            return footnote;
        }

        public Footnote AddFootnote(string text)
        {
            Footnote footnote = new Footnote();
            footnote.Elements.AddParagraph().AddText(text);
            this.Add(footnote);
            return footnote;
        }

        public FormattedText AddFormattedText()
        {
            FormattedText text = new FormattedText();
            this.Add(text);
            return text;
        }

        public FormattedText AddFormattedText(Font font)
        {
            FormattedText text = new FormattedText();
            text.Font.ApplyFont(font);
            this.Add(text);
            return text;
        }

        public FormattedText AddFormattedText(TextFormat textFormat)
        {
            FormattedText text = this.AddFormattedText();
            if ((textFormat & TextFormat.Bold) == TextFormat.Bold)
            {
                text.Bold = true;
            }
            if ((textFormat & TextFormat.NotBold) == TextFormat.NotBold)
            {
                text.Bold = false;
            }
            if ((textFormat & TextFormat.Italic) == TextFormat.Italic)
            {
                text.Italic = true;
            }
            if ((textFormat & TextFormat.NotItalic) == TextFormat.NotItalic)
            {
                text.Italic = false;
            }
            if ((textFormat & TextFormat.Underline) == TextFormat.Underline)
            {
                text.Underline = Underline.Single;
            }
            if ((textFormat & TextFormat.NoUnderline) == TextFormat.NoUnderline)
            {
                text.Underline = Underline.None;
            }
            return text;
        }

        public FormattedText AddFormattedText(string text)
        {
            FormattedText text2 = new FormattedText();
            text2.AddText(text);
            this.Add(text2);
            return text2;
        }

        public FormattedText AddFormattedText(string text, Font font)
        {
            FormattedText text2 = this.AddFormattedText(font);
            text2.AddText(text);
            return text2;
        }

        public FormattedText AddFormattedText(string text, TextFormat textFormat)
        {
            FormattedText text2 = this.AddFormattedText(textFormat);
            text2.AddText(text);
            return text2;
        }

        public FormattedText AddFormattedText(string text, string style)
        {
            FormattedText text2 = this.AddFormattedText(text);
            text2.Style = style;
            return text2;
        }

        public Hyperlink AddHyperlink(string name)
        {
            Hyperlink hyperlink = new Hyperlink {
                Name = name
            };
            this.Add(hyperlink);
            return hyperlink;
        }

        public Hyperlink AddHyperlink(string name, HyperlinkType type)
        {
            Hyperlink hyperlink = new Hyperlink {
                Name = name,
                Type = type
            };
            this.Add(hyperlink);
            return hyperlink;
        }

        public Image AddImage(string name)
        {
            Image image = new Image {
                Name = name
            };
            this.Add(image);
            return image;
        }

        public InfoField AddInfoField(InfoFieldType iType)
        {
            InfoField field = new InfoField {
                Name = iType.ToString()
            };
            this.Add(field);
            return field;
        }

        public Character AddLineBreak() => 
            this.AddCharacter((SymbolName) (-201326591), 1);

        public NumPagesField AddNumPagesField()
        {
            NumPagesField field = new NumPagesField();
            this.Add(field);
            return field;
        }

        public PageField AddPageField()
        {
            PageField field = new PageField();
            this.Add(field);
            return field;
        }

        public PageRefField AddPageRefField(string name)
        {
            PageRefField field = new PageRefField {
                Name = name
            };
            this.Add(field);
            return field;
        }

        public SectionField AddSectionField()
        {
            SectionField field = new SectionField();
            this.Add(field);
            return field;
        }

        public SectionPagesField AddSectionPagesField()
        {
            SectionPagesField field = new SectionPagesField();
            this.Add(field);
            return field;
        }

        public Character AddSpace(int count) => 
            this.AddCharacter((SymbolName) (-251658239), count);

        public Character AddTab() => 
            this.AddCharacter((SymbolName) (-234881023), 1);

        public Text AddText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            Text text2 = null;
            string[] strArray = text.Split(new char[] { '\n' });
            int length = strArray.Length;
            for (int i = 0; i < length; i++)
            {
                string[] strArray2 = strArray[i].Split(new char[] { '\t' });
                int num3 = strArray2.Length;
                for (int j = 0; j < num3; j++)
                {
                    if (strArray2[j].Length != 0)
                    {
                        text2 = new Text(strArray2[j]);
                        this.Add(text2);
                    }
                    if (j < (num3 - 1))
                    {
                        this.AddTab();
                    }
                }
                if (i < (length - 1))
                {
                    this.AddLineBreak();
                }
            }
            return text2;
        }

        public ParagraphElements Clone() => 
            ((ParagraphElements) this.DeepCopy());

        internal override void Serialize(Serializer serializer)
        {
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                this[i].Serialize(serializer);
            }
        }

        public DocumentObject this[int index] =>
            base[index];

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(ParagraphElements));
                }
                return meta;
            }
        }
    }
}

