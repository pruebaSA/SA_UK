namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;
    using System.Collections;
    using System.Reflection;

    public class Styles : DocumentObjectCollection, IVisitable
    {
        internal static readonly Styles BuildInStyles = new Styles();
        [DV]
        internal NString comment;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;

        public Styles()
        {
            this.comment = NString.NullValue;
            this.SetupStyles();
        }

        internal Styles(DocumentObject parent) : base(parent)
        {
            this.comment = NString.NullValue;
            this.SetupStyles();
        }

        public override void Add(DocumentObject value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            Style style = value as Style;
            if (style == null)
            {
                throw new InvalidOperationException(DomSR.StyleExpected);
            }
            bool isRootStyle = style.IsRootStyle;
            if ((style.BaseStyle == "") && !isRootStyle)
            {
                throw new ArgumentException(DomSR.UndefinedBaseStyle(style.BaseStyle));
            }
            Style style2 = null;
            int index = this.GetIndex(style.BaseStyle);
            if (index != -1)
            {
                style2 = this[index];
            }
            else if (!isRootStyle)
            {
                throw new ArgumentException(DomSR.UndefinedBaseStyle(style.BaseStyle));
            }
            if (style2 != null)
            {
                style.styleType.Value = (int) style2.Type;
            }
            int num2 = this.GetIndex(style.Name);
            if (num2 >= 0)
            {
                style = style.Clone();
                style.parent = this;
                this[num2] = style;
            }
            else
            {
                base.Add(value);
            }
        }

        public Style AddStyle(string name, string baseStyleName)
        {
            if ((name == null) || (baseStyleName == null))
            {
                throw new ArgumentNullException((name == null) ? "name" : "baseStyleName");
            }
            if ((name == "") || (baseStyleName == ""))
            {
                throw new ArgumentException((name == "") ? "name" : "baseStyleName");
            }
            Style style = new Style {
                name = { Value = name },
                baseStyle = { Value = baseStyleName }
            };
            this.Add(style);
            return style;
        }

        public Styles Clone() => 
            ((Styles) base.DeepCopy());

        public int GetIndex(string styleName)
        {
            if (styleName == null)
            {
                throw new ArgumentNullException("styleName");
            }
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                Style style = this[i];
                if (string.Compare(style.Name, styleName, true) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitStyles(this);
            Hashtable visitedStyles = new Hashtable();
            foreach (Style style in this)
            {
                this.VisitStyle(visitedStyles, style, visitor, visitChildren);
            }
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            int pos = serializer.BeginContent(@"\styles");
            int count = base.Count;
            bool[] fSerialized = new bool[count];
            fSerialized[0] = true;
            bool[] fSerializePending = new bool[count];
            bool newLine = false;
            for (int i = 1; i < count; i++)
            {
                if (!fSerialized[i])
                {
                    Style style1 = this[i];
                    this.SerializeStyle(serializer, i, ref fSerialized, ref fSerializePending, ref newLine);
                }
            }
            serializer.EndContent(pos);
        }

        private void SerializeStyle(Serializer serializer, int index, ref bool[] fSerialized, ref bool[] fSerializePending, ref bool newLine)
        {
            Style style = this[index];
            if (style.Name != "DefaultParagraphFont")
            {
                if (fSerializePending[index])
                {
                    throw new ApplicationException($"Circular dependency detected according to style '{style.Name}'.");
                }
                if (style.BaseStyle != "")
                {
                    int num = this.GetIndex(style.BaseStyle);
                    if ((num != -1) && !fSerialized[num])
                    {
                        fSerializePending[index] = true;
                        this.SerializeStyle(serializer, num, ref fSerialized, ref fSerializePending, ref newLine);
                        fSerializePending[index] = false;
                    }
                }
                int pos = serializer.BeginBlock();
                if (newLine)
                {
                    serializer.WriteLineNoCommit();
                }
                style.Serialize(serializer);
                if (serializer.EndBlock(pos))
                {
                    newLine = true;
                }
                fSerialized[index] = true;
            }
        }

        internal void SetupStyles()
        {
            Style style = new Style("DefaultParagraphFont", null) {
                readOnly = true,
                styleType = { Value = 1 },
                buildIn = { Value = true }
            };
            this.Add(style);
            style = new Style("Normal", null) {
                styleType = { Value = 0 },
                buildIn = { Value = true },
                Font = { 
                    Name = "Verdana",
                    Size = 10,
                    Bold = false,
                    Italic = false,
                    Underline = Underline.None,
                    Color = Colors.Black,
                    Subscript = false,
                    Superscript = false
                },
                ParagraphFormat = { 
                    Alignment = ParagraphAlignment.Left,
                    FirstLineIndent = 0,
                    LeftIndent = 0,
                    RightIndent = 0,
                    KeepTogether = false,
                    KeepWithNext = false,
                    SpaceBefore = 0,
                    SpaceAfter = 0,
                    LineSpacing = 10,
                    LineSpacingRule = LineSpacingRule.Single,
                    OutlineLevel = OutlineLevel.BodyText,
                    PageBreakBefore = false,
                    WidowControl = true
                }
            };
            this.Add(style);
            style = new Style("Heading1", "Normal") {
                buildIn = { Value = true },
                ParagraphFormat = { OutlineLevel = OutlineLevel.Level1 }
            };
            this.Add(style);
            style = new Style("Heading2", "Heading1") {
                buildIn = { Value = true },
                ParagraphFormat = { OutlineLevel = OutlineLevel.Level2 }
            };
            this.Add(style);
            style = new Style("Heading3", "Heading2") {
                buildIn = { Value = true },
                ParagraphFormat = { OutlineLevel = OutlineLevel.Level3 }
            };
            this.Add(style);
            style = new Style("Heading4", "Heading3") {
                buildIn = { Value = true },
                ParagraphFormat = { OutlineLevel = OutlineLevel.Level4 }
            };
            this.Add(style);
            style = new Style("Heading5", "Heading4") {
                buildIn = { Value = true },
                ParagraphFormat = { OutlineLevel = OutlineLevel.Level5 }
            };
            this.Add(style);
            style = new Style("Heading6", "Heading5") {
                buildIn = { Value = true },
                ParagraphFormat = { OutlineLevel = OutlineLevel.Level6 }
            };
            this.Add(style);
            style = new Style("Heading7", "Heading6") {
                buildIn = { Value = true },
                ParagraphFormat = { OutlineLevel = OutlineLevel.Level7 }
            };
            this.Add(style);
            style = new Style("Heading8", "Heading7") {
                buildIn = { Value = true },
                ParagraphFormat = { OutlineLevel = OutlineLevel.Level8 }
            };
            this.Add(style);
            style = new Style("Heading9", "Heading8") {
                buildIn = { Value = true },
                ParagraphFormat = { OutlineLevel = OutlineLevel.Level9 }
            };
            this.Add(style);
            style = new Style("List", "Normal") {
                buildIn = { Value = true }
            };
            this.Add(style);
            style = new Style("Footnote", "Normal") {
                buildIn = { Value = true }
            };
            this.Add(style);
            style = new Style("Header", "Normal") {
                buildIn = { Value = true }
            };
            this.Add(style);
            style = new Style("Footer", "Normal") {
                buildIn = { Value = true }
            };
            this.Add(style);
            style = new Style("Hyperlink", "DefaultParagraphFont") {
                buildIn = { Value = true }
            };
            this.Add(style);
            style = new Style("InvalidStyleName", "Normal") {
                buildIn = { Value = true },
                Font = { 
                    Bold = true,
                    Underline = Underline.Dash,
                    Color = new Color(0xff00ff00)
                }
            };
            this.Add(style);
        }

        private void VisitStyle(Hashtable visitedStyles, Style style, DocumentObjectVisitor visitor, bool visitChildren)
        {
            if (!visitedStyles.Contains(style))
            {
                Style baseStyle = style.GetBaseStyle();
                if ((baseStyle != null) && !visitedStyles.Contains(baseStyle))
                {
                    this.VisitStyle(visitedStyles, baseStyle, visitor, visitChildren);
                }
                ((IVisitable) style).AcceptVisitor(visitor, visitChildren);
                visitedStyles.Add(style, null);
            }
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

        public Style this[string styleName]
        {
            get
            {
                int count = base.Count;
                for (int i = 1; i < count; i++)
                {
                    Style style = this[i];
                    if (string.Compare(style.Name, styleName, true) == 0)
                    {
                        return style;
                    }
                }
                return null;
            }
        }

        internal Style this[int index] =>
            ((Style) base[index]);

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Styles));
                }
                return meta;
            }
        }

        public Style Normal =>
            this["Normal"];
    }
}

