namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.Drawing;

    public sealed class Font : DocumentObject
    {
        [DV]
        internal NBool bold;
        [DV]
        internal MigraDoc.DocumentObjectModel.Color color;
        [DV]
        internal NBool italic;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString name;
        [DV]
        internal Unit size;
        [DV]
        internal NBool subscript;
        [DV]
        internal NBool superscript;
        [DV(Type=typeof(MigraDoc.DocumentObjectModel.Underline))]
        internal NEnum underline;

        public Font()
        {
            this.name = NString.NullValue;
            this.size = Unit.NullValue;
            this.bold = NBool.NullValue;
            this.italic = NBool.NullValue;
            this.underline = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Underline));
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
            this.superscript = NBool.NullValue;
            this.subscript = NBool.NullValue;
        }

        internal Font(DocumentObject parent) : base(parent)
        {
            this.name = NString.NullValue;
            this.size = Unit.NullValue;
            this.bold = NBool.NullValue;
            this.italic = NBool.NullValue;
            this.underline = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Underline));
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
            this.superscript = NBool.NullValue;
            this.subscript = NBool.NullValue;
        }

        public Font(string name)
        {
            this.name = NString.NullValue;
            this.size = Unit.NullValue;
            this.bold = NBool.NullValue;
            this.italic = NBool.NullValue;
            this.underline = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Underline));
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
            this.superscript = NBool.NullValue;
            this.subscript = NBool.NullValue;
            this.name.Value = name;
        }

        public Font(string name, Unit size)
        {
            this.name = NString.NullValue;
            this.size = Unit.NullValue;
            this.bold = NBool.NullValue;
            this.italic = NBool.NullValue;
            this.underline = NEnum.NullValue(typeof(MigraDoc.DocumentObjectModel.Underline));
            this.color = MigraDoc.DocumentObjectModel.Color.Empty;
            this.superscript = NBool.NullValue;
            this.subscript = NBool.NullValue;
            this.name.Value = name;
            this.size.Value = (double) size;
        }

        public void ApplyFont(MigraDoc.DocumentObjectModel.Font font)
        {
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }
            if (!font.name.IsNull && (font.name.Value != ""))
            {
                this.Name = font.Name;
            }
            if (!font.size.IsNull)
            {
                this.Size = font.Size;
            }
            if (!font.bold.IsNull)
            {
                this.Bold = font.Bold;
            }
            if (!font.italic.IsNull)
            {
                this.Italic = font.Italic;
            }
            if (!font.subscript.IsNull)
            {
                this.Subscript = font.Subscript;
            }
            else if (!font.superscript.IsNull)
            {
                this.Superscript = font.Superscript;
            }
            if (!font.underline.IsNull)
            {
                this.Underline = font.Underline;
            }
            if (!font.color.IsNull)
            {
                this.Color = font.Color;
            }
        }

        internal void ApplyFont(MigraDoc.DocumentObjectModel.Font font, MigraDoc.DocumentObjectModel.Font refFont)
        {
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }
            if ((!font.name.IsNull && (font.name.Value != "")) && ((refFont == null) || (font.Name != refFont.Name)))
            {
                this.Name = font.Name;
            }
            if (!font.size.IsNull && ((refFont == null) || (font.Size != refFont.Size)))
            {
                this.Size = font.Size;
            }
            if (!font.bold.IsNull && ((refFont == null) || (font.Bold != refFont.Bold)))
            {
                this.Bold = font.Bold;
            }
            if (!font.italic.IsNull && ((refFont == null) || (font.Italic != refFont.Italic)))
            {
                this.Italic = font.Italic;
            }
            if (!font.subscript.IsNull && ((refFont == null) || (font.Subscript != refFont.Subscript)))
            {
                this.Subscript = font.Subscript;
            }
            else if (!font.superscript.IsNull && ((refFont == null) || (font.Superscript != refFont.Superscript)))
            {
                this.Superscript = font.Superscript;
            }
            if (!font.underline.IsNull && ((refFont == null) || (font.Underline != refFont.Underline)))
            {
                this.Underline = font.Underline;
            }
            if (!font.color.IsNull && ((refFont == null) || (font.Color.Argb != refFont.Color.Argb)))
            {
                this.Color = font.Color;
            }
        }

        private FontProperties CheckWhatIsNotNull()
        {
            FontProperties none = FontProperties.None;
            if (!this.name.IsNull)
            {
                none |= FontProperties.Name;
            }
            if (!this.size.IsNull)
            {
                none |= FontProperties.Size;
            }
            if (!this.bold.IsNull)
            {
                none |= FontProperties.Bold;
            }
            if (!this.italic.IsNull)
            {
                none |= FontProperties.Italic;
            }
            if (!this.underline.IsNull)
            {
                none |= FontProperties.Underline;
            }
            if (!this.color.IsNull)
            {
                none |= FontProperties.Color;
            }
            if (!this.superscript.IsNull)
            {
                none |= FontProperties.Superscript;
            }
            if (!this.subscript.IsNull)
            {
                none |= FontProperties.Subscript;
            }
            return none;
        }

        public MigraDoc.DocumentObjectModel.Font Clone() => 
            ((MigraDoc.DocumentObjectModel.Font) this.DeepCopy());

        public static bool Exists(string fontName)
        {
            foreach (FontFamily family in FontFamily.Families)
            {
                if (string.Compare(family.Name, fontName, true) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        internal override void Serialize(Serializer serializer)
        {
            this.Serialize(serializer, null);
        }

        internal void Serialize(Serializer serializer, MigraDoc.DocumentObjectModel.Font font)
        {
            if (base.Parent is FormattedText)
            {
                string str = "";
                if (((FormattedText) base.Parent).style.IsNull)
                {
                    FontProperties properties = this.CheckWhatIsNotNull();
                    if (properties == FontProperties.Size)
                    {
                        serializer.Write(@"\fontsize(" + this.size.ToString() + ")");
                        return;
                    }
                    if ((properties == FontProperties.Bold) && this.bold.Value)
                    {
                        serializer.Write(@"\bold");
                        return;
                    }
                    if ((properties == FontProperties.Italic) && this.italic.Value)
                    {
                        serializer.Write(@"\italic");
                        return;
                    }
                    if (properties == FontProperties.Color)
                    {
                        serializer.Write(@"\fontcolor(" + this.color.ToString() + ")");
                        return;
                    }
                }
                else
                {
                    str = "(\"" + ((FormattedText) base.Parent).Style + "\")";
                }
                serializer.Write(@"\font" + str + "[");
                if (!this.name.IsNull && (this.name.Value != ""))
                {
                    serializer.WriteSimpleAttribute("Name", this.Name);
                }
                if (!this.size.IsNull)
                {
                    serializer.WriteSimpleAttribute("Size", this.Size);
                }
                if (!this.bold.IsNull)
                {
                    serializer.WriteSimpleAttribute("Bold", this.Bold);
                }
                if (!this.italic.IsNull)
                {
                    serializer.WriteSimpleAttribute("Italic", this.Italic);
                }
                if (!this.underline.IsNull)
                {
                    serializer.WriteSimpleAttribute("Underline", this.Underline);
                }
                if (!this.superscript.IsNull)
                {
                    serializer.WriteSimpleAttribute("Superscript", this.Superscript);
                }
                if (!this.subscript.IsNull)
                {
                    serializer.WriteSimpleAttribute("Subscript", this.Subscript);
                }
                if (!this.color.IsNull)
                {
                    serializer.WriteSimpleAttribute("Color", this.Color);
                }
                serializer.Write("]");
            }
            else
            {
                int pos = serializer.BeginContent("Font");
                if (((!this.name.IsNull && (this.Name != string.Empty)) && (font == null)) || (((font != null) && !this.name.IsNull) && ((this.Name != string.Empty) && (this.Name != font.Name))))
                {
                    serializer.WriteSimpleAttribute("Name", this.Name);
                }
                if ((!this.size.IsNull && (this.Size != 0)) && (this.Size.Point == 0.0))
                {
                    base.GetType();
                }
                if (!this.size.IsNull && ((font == null) || (this.Size != font.Size)))
                {
                    serializer.WriteSimpleAttribute("Size", this.Size);
                }
                if (!this.bold.IsNull && (((font == null) || (this.Bold != font.Bold)) || font.bold.IsNull))
                {
                    serializer.WriteSimpleAttribute("Bold", this.Bold);
                }
                if (!this.italic.IsNull && (((font == null) || (this.Italic != font.Italic)) || font.italic.IsNull))
                {
                    serializer.WriteSimpleAttribute("Italic", this.Italic);
                }
                if (!this.underline.IsNull && (((font == null) || (this.Underline != font.Underline)) || font.underline.IsNull))
                {
                    serializer.WriteSimpleAttribute("Underline", this.Underline);
                }
                if (!this.superscript.IsNull && (((font == null) || (this.Superscript != font.Superscript)) || font.superscript.IsNull))
                {
                    serializer.WriteSimpleAttribute("Superscript", this.Superscript);
                }
                if (!this.subscript.IsNull && (((font == null) || (this.Subscript != font.Subscript)) || font.subscript.IsNull))
                {
                    serializer.WriteSimpleAttribute("Subscript", this.Subscript);
                }
                if (!this.color.IsNull && ((font == null) || (this.Color.Argb != font.Color.Argb)))
                {
                    serializer.WriteSimpleAttribute("Color", this.Color);
                }
                serializer.EndContent(pos);
            }
        }

        public bool Bold
        {
            get => 
                this.bold.Value;
            set
            {
                this.bold.Value = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Color Color
        {
            get => 
                this.color;
            set
            {
                this.color = value;
            }
        }

        public bool Italic
        {
            get => 
                this.italic.Value;
            set
            {
                this.italic.Value = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(MigraDoc.DocumentObjectModel.Font));
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

        public Unit Size
        {
            get => 
                this.size;
            set
            {
                this.size = value;
            }
        }

        public bool Subscript
        {
            get => 
                this.subscript.Value;
            set
            {
                this.subscript.Value = value;
                this.superscript.SetNull();
            }
        }

        public bool Superscript
        {
            get => 
                this.superscript.Value;
            set
            {
                this.superscript.Value = value;
                this.subscript.SetNull();
            }
        }

        public MigraDoc.DocumentObjectModel.Underline Underline
        {
            get => 
                ((MigraDoc.DocumentObjectModel.Underline) this.underline.Value);
            set
            {
                this.underline.Value = (int) value;
            }
        }
    }
}

