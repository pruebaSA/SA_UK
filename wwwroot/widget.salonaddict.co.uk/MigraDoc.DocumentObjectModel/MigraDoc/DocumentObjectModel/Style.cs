namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Visitors;
    using System;

    public sealed class Style : DocumentObject, IVisitable
    {
        [DV]
        internal NString baseStyle;
        [DV]
        internal NBool buildIn;
        [DV]
        internal NString comment;
        public const string DefaultParagraphFontName = "DefaultParagraphFont";
        public const string DefaultParagraphName = "Normal";
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString name;
        [DV]
        internal MigraDoc.DocumentObjectModel.ParagraphFormat paragraphFormat;
        internal bool readOnly;
        [DV(Type=typeof(StyleType))]
        internal NEnum styleType;

        internal Style()
        {
            this.name = NString.NullValue;
            this.baseStyle = NString.NullValue;
            this.styleType = NEnum.NullValue(typeof(StyleType));
            this.buildIn = NBool.NullValue;
            this.comment = NString.NullValue;
        }

        internal Style(DocumentObject parent) : base(parent)
        {
            this.name = NString.NullValue;
            this.baseStyle = NString.NullValue;
            this.styleType = NEnum.NullValue(typeof(StyleType));
            this.buildIn = NBool.NullValue;
            this.comment = NString.NullValue;
        }

        public Style(string name, string baseStyleName) : this()
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name == "")
            {
                throw new ArgumentException("name");
            }
            this.name.Value = name;
            this.baseStyle.Value = baseStyleName;
        }

        public Style Clone() => 
            ((Style) this.DeepCopy());

        protected override object DeepCopy()
        {
            Style style = (Style) base.DeepCopy();
            if (style.paragraphFormat != null)
            {
                style.paragraphFormat = style.paragraphFormat.Clone();
                style.paragraphFormat.parent = style;
            }
            return style;
        }

        public Style GetBaseStyle()
        {
            if (this.IsRootStyle)
            {
                return null;
            }
            Styles parent = base.Parent as Styles;
            if (parent == null)
            {
                throw new InvalidOperationException("This instance of 'style' is currently not owner of a parent; access failed");
            }
            if (this.baseStyle.Value == "")
            {
                throw new ArgumentException("User defined Style defined without a BaseStyle");
            }
            if (this.baseStyle.Value == "DefaultParagraphFont")
            {
                return parent[0];
            }
            return parent[this.baseStyle.Value];
        }

        public override object GetValue(string name, GV flags)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name == "")
            {
                throw new ArgumentException("name");
            }
            if (name.ToLower().StartsWith("font"))
            {
                return this.ParagraphFormat.GetValue(name);
            }
            return base.GetValue(name, flags);
        }

        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitStyle(this);
        }

        private void Optimize()
        {
        }

        internal override void Serialize(Serializer serializer)
        {
            Styles buildInStyles = Styles.BuildInStyles;
            Style style = null;
            MigraDoc.DocumentObjectModel.ParagraphFormat refFormat = null;
            serializer.WriteComment(this.comment.Value);
            if (this.buildIn.Value)
            {
                if (this.BaseStyle == "")
                {
                    if (string.Compare(this.name.Value, "Normal", true) != 0)
                    {
                        throw new ArgumentException("Internal Error: BaseStyle not set.");
                    }
                    style = buildInStyles[buildInStyles.GetIndex(this.Name)];
                    refFormat = style.ParagraphFormat;
                    MigraDoc.DocumentObjectModel.Font font = refFormat.Font;
                    string str = DdlEncoder.QuoteIfNameContainsBlanks(this.Name);
                    serializer.WriteLineNoCommit(str);
                }
                else
                {
                    style = buildInStyles[buildInStyles.GetIndex(this.Name)];
                    MigraDoc.DocumentObjectModel.Font font2 = style.ParagraphFormat.Font;
                    if (string.Compare(this.BaseStyle, style.BaseStyle, true) == 0)
                    {
                        string str2 = DdlEncoder.QuoteIfNameContainsBlanks(this.Name);
                        serializer.WriteLineNoCommit(str2);
                        style = base.Document.Styles[base.Document.Styles.GetIndex(this.baseStyle.Value)];
                        refFormat = style.ParagraphFormat;
                        MigraDoc.DocumentObjectModel.Font font3 = refFormat.Font;
                    }
                    else
                    {
                        serializer.WriteLine(DdlEncoder.QuoteIfNameContainsBlanks(this.Name) + " : " + DdlEncoder.QuoteIfNameContainsBlanks(this.BaseStyle));
                        style = base.Document.Styles[base.Document.Styles.GetIndex(this.baseStyle.Value)];
                        refFormat = style.ParagraphFormat;
                        MigraDoc.DocumentObjectModel.Font font4 = refFormat.Font;
                    }
                }
            }
            else
            {
                serializer.WriteLine(DdlEncoder.QuoteIfNameContainsBlanks(this.Name) + " : " + DdlEncoder.QuoteIfNameContainsBlanks(this.BaseStyle));
                Style style1 = base.Document.Styles[base.Document.Styles.GetIndex(this.baseStyle.Value)];
                style = base.Document.Styles[this.baseStyle.Value];
                refFormat = style?.ParagraphFormat;
                MigraDoc.DocumentObjectModel.Font font5 = style.Font;
            }
            serializer.BeginContent();
            if (!this.IsNull("ParagraphFormat"))
            {
                if (!this.ParagraphFormat.IsNull("Font"))
                {
                    this.Font.Serialize(serializer, refFormat?.Font);
                }
                if (this.Type == StyleType.Paragraph)
                {
                    this.ParagraphFormat.Serialize(serializer, "ParagraphFormat", refFormat);
                }
            }
            serializer.EndContent();
        }

        public string BaseStyle
        {
            get => 
                this.baseStyle.Value;
            set
            {
                if ((value == null) || ((value == "") && (this.baseStyle.Value != "")))
                {
                    throw new ArgumentException(DomSR.EmptyBaseStyle);
                }
                if (string.Compare(this.baseStyle.Value, value, true) == 0)
                {
                    this.baseStyle.Value = value;
                }
                else
                {
                    if ((string.Compare(this.name.Value, "Normal", true) == 0) || (string.Compare(this.name.Value, "DefaultParagraphFont", true) == 0))
                    {
                        throw new ArgumentException($"Style '{this.name}' has no base style and that cannot be altered.");
                    }
                    Styles parent = (Styles) base.parent;
                    int index = parent.GetIndex(value);
                    if (index == -1)
                    {
                        throw new ArgumentException($"Base style '{value}' does not exist.");
                    }
                    if (index > 1)
                    {
                        for (Style style = parent[index]; style != null; style = parent[style.BaseStyle])
                        {
                            if (style == this)
                            {
                                throw new ArgumentException($"Base style '{value}' leads to a circular dependency.");
                            }
                        }
                    }
                    this.baseStyle.Value = value;
                }
            }
        }

        public bool BuildIn =>
            this.buildIn.Value;

        public string Comment
        {
            get => 
                this.comment.Value;
            set
            {
                this.comment.Value = value;
            }
        }

        [DV]
        public MigraDoc.DocumentObjectModel.Font Font
        {
            get => 
                this.ParagraphFormat.Font;
            set
            {
                this.ParagraphFormat.Font = value;
            }
        }

        public bool IsReadOnly =>
            this.readOnly;

        internal bool IsRootStyle
        {
            get
            {
                if (string.Compare(this.Name, "DefaultParagraphFont", true) != 0)
                {
                    return (string.Compare(this.Name, "Normal", true) == 0);
                }
                return true;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Style));
                }
                return meta;
            }
        }

        public string Name =>
            this.name.Value;

        public MigraDoc.DocumentObjectModel.ParagraphFormat ParagraphFormat
        {
            get
            {
                if (this.paragraphFormat == null)
                {
                    this.paragraphFormat = new MigraDoc.DocumentObjectModel.ParagraphFormat(this);
                }
                if (this.readOnly)
                {
                    return this.paragraphFormat.Clone();
                }
                return this.paragraphFormat;
            }
            set
            {
                base.SetParent(value);
                this.paragraphFormat = value;
            }
        }

        public StyleType Type
        {
            get
            {
                if (this.styleType.IsNull)
                {
                    if (string.Compare(this.baseStyle.Value, "DefaultParagraphFont", true) == 0)
                    {
                        this.styleType.Value = 1;
                    }
                    else
                    {
                        Style baseStyle = this.GetBaseStyle();
                        if (baseStyle == null)
                        {
                            throw new InvalidOperationException("User defined style has no valid base Style.");
                        }
                        this.styleType.Value = (int) baseStyle.Type;
                    }
                }
                return (StyleType) this.styleType.Value;
            }
        }
    }
}

