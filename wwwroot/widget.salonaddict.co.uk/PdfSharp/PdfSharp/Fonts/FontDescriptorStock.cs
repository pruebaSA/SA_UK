namespace PdfSharp.Fonts
{
    using PdfSharp.Drawing;
    using PdfSharp.Fonts.OpenType;
    using System;
    using System.Collections.Generic;

    internal class FontDescriptorStock
    {
        private static FontDescriptorStock global;
        private Dictionary<FontSelector, FontDescriptor> table = new Dictionary<FontSelector, FontDescriptor>();

        private FontDescriptorStock()
        {
        }

        public FontDescriptor CreateDescriptor(XFont font)
        {
            FontDescriptor descriptor;
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }
            FontSelector key = new FontSelector(font);
            if (!this.table.TryGetValue(key, out descriptor))
            {
                lock (typeof(FontDescriptorStock))
                {
                    if (!this.table.TryGetValue(key, out descriptor))
                    {
                        descriptor = new OpenTypeDescriptor(font);
                        this.table.Add(key, descriptor);
                    }
                }
            }
            return descriptor;
        }

        public FontDescriptor CreateDescriptor(XFontFamily family, XFontStyle style)
        {
            FontDescriptor descriptor;
            if (family == null)
            {
                throw new ArgumentNullException("family");
            }
            FontSelector key = new FontSelector(family, style);
            if (!this.table.TryGetValue(key, out descriptor))
            {
                lock (typeof(FontDescriptorStock))
                {
                    if (this.table.TryGetValue(key, out descriptor))
                    {
                        return descriptor;
                    }
                    XFont font = new XFont(family.Name, 10.0, style);
                    descriptor = new OpenTypeDescriptor(font);
                    if (this.table.ContainsKey(key))
                    {
                        base.GetType();
                        return descriptor;
                    }
                    this.table.Add(key, descriptor);
                }
            }
            return descriptor;
        }

        public FontDescriptor CreateDescriptor(string idName, byte[] fontData)
        {
            FontDescriptor descriptor;
            FontSelector key = new FontSelector(idName);
            if (!this.table.TryGetValue(key, out descriptor))
            {
                lock (typeof(FontDescriptorStock))
                {
                    if (!this.table.TryGetValue(key, out descriptor))
                    {
                        descriptor = new OpenTypeDescriptor(idName, fontData);
                        this.table.Add(key, descriptor);
                    }
                }
            }
            return descriptor;
        }

        public FontDescriptor FindDescriptor(FontSelector selector)
        {
            if (selector == null)
            {
                return null;
            }
            return this.table[selector];
        }

        public static FontDescriptorStock Global
        {
            get
            {
                if (global == null)
                {
                    lock (typeof(FontDescriptorStock))
                    {
                        if (global == null)
                        {
                            global = new FontDescriptorStock();
                        }
                    }
                }
                return global;
            }
        }

        internal class FontSelector
        {
            private string name;
            private XFontStyle style;

            public FontSelector(XFont font)
            {
                this.name = font.Name;
                this.style = font.Style;
            }

            public FontSelector(string idName)
            {
                this.name = idName;
                this.style = XFontStyle.Regular;
            }

            public FontSelector(XFontFamily family, XFontStyle style)
            {
                this.name = family.Name;
                this.style = style;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }
                FontDescriptorStock.FontSelector objA = obj as FontDescriptorStock.FontSelector;
                if (object.Equals(objA, null))
                {
                    return false;
                }
                return ((this.name == objA.name) && (this.style == objA.style));
            }

            public override int GetHashCode() => 
                (this.name.GetHashCode() ^ this.style.GetHashCode());

            public static bool operator ==(FontDescriptorStock.FontSelector selector1, FontDescriptorStock.FontSelector selector2)
            {
                if (!object.Equals(selector1, null))
                {
                    selector1.Equals(selector2);
                }
                return object.Equals(selector2, null);
            }

            public static bool operator !=(FontDescriptorStock.FontSelector selector1, FontDescriptorStock.FontSelector selector2) => 
                !(selector1 == selector2);

            public override string ToString()
            {
                string str = "";
                switch (this.style)
                {
                    case XFontStyle.Regular:
                        str = "(Regular)";
                        break;

                    case XFontStyle.Bold:
                        str = "(Bold)";
                        break;

                    case XFontStyle.Italic:
                        str = "(Italic)";
                        break;

                    case XFontStyle.BoldItalic:
                        str = "(BoldItalic)";
                        break;
                }
                return (this.name + str);
            }

            public string Name =>
                this.name;

            public XFontStyle Style =>
                this.style;
        }
    }
}

