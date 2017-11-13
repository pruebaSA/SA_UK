namespace PdfSharp.Pdf
{
    using PdfSharp;
    using PdfSharp.Drawing;
    using PdfSharp.Pdf.Internal;
    using PdfSharp.Pdf.IO;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class PdfOutline : PdfDictionary
    {
        private int count;
        private PdfPage destinationPage;
        internal int openCount;
        private bool opened2;
        private PdfOutlineCollection outlines;
        private PdfOutline parent;
        private XColor textColor;

        public PdfOutline()
        {
        }

        public PdfOutline(PdfDictionary dict) : base(dict)
        {
        }

        internal PdfOutline(PdfDocument document) : base(document)
        {
        }

        public PdfOutline(string title, PdfPage destinationPage)
        {
            this.Title = title;
            this.DestinationPage = destinationPage;
        }

        public PdfOutline(string title, PdfPage destinationPage, bool opened)
        {
            this.Title = title;
            this.DestinationPage = destinationPage;
            this.Opened = opened;
        }

        public PdfOutline(string title, PdfPage destinationPage, bool opened, PdfOutlineStyle style)
        {
            this.Title = title;
            this.DestinationPage = destinationPage;
            this.Opened = opened;
            this.Style = style;
        }

        public PdfOutline(string title, PdfPage destinationPage, bool opened, PdfOutlineStyle style, XColor textColor)
        {
            this.Title = title;
            this.DestinationPage = destinationPage;
            this.Opened = opened;
            this.Style = style;
            this.TextColor = textColor;
        }

        internal override void PrepareForSave()
        {
            bool flag = (this.outlines != null) && (this.outlines.Count > 0);
            if ((this.parent != null) || flag)
            {
                if (this.parent == null)
                {
                    base.Elements["/First"] = this.outlines[0].Reference;
                    base.Elements["/Last"] = this.outlines[this.outlines.Count - 1].Reference;
                    if (this.openCount > 0)
                    {
                        base.Elements["/Count"] = new PdfInteger(this.openCount);
                    }
                }
                else
                {
                    base.Elements["/Parent"] = this.parent.Reference;
                    int count = this.parent.outlines.Count;
                    int index = this.parent.outlines.IndexOf(this);
                    if (this.DestinationPage != null)
                    {
                        base.Elements["/Dest"] = new PdfArray(this.Owner, new PdfItem[] { this.DestinationPage.Reference, new PdfLiteral("/XYZ null null 0") });
                    }
                    if (index > 0)
                    {
                        base.Elements["/Prev"] = this.parent.outlines[index - 1].Reference;
                    }
                    if (index < (count - 1))
                    {
                        base.Elements["/Next"] = this.parent.outlines[index + 1].Reference;
                    }
                    if (flag)
                    {
                        base.Elements["/First"] = this.outlines[0].Reference;
                        base.Elements["/Last"] = this.outlines[this.outlines.Count - 1].Reference;
                    }
                    if (this.openCount > 0)
                    {
                        base.Elements["/Count"] = new PdfInteger((this.opened2 ? 1 : -1) * this.openCount);
                    }
                    if ((this.textColor != XColor.Empty) && this.Owner.HasVersion("1.4"))
                    {
                        base.Elements["/C"] = new PdfLiteral("[{0}]", new object[] { PdfEncoders.ToString(this.textColor, PdfColorMode.Rgb) });
                    }
                }
                if (flag)
                {
                    foreach (PdfOutline outline in this.outlines)
                    {
                        outline.PrepareForSave();
                    }
                }
            }
        }

        internal override void WriteObject(PdfWriter writer)
        {
            bool flag = (this.outlines != null) && (this.outlines.Count > 0);
            if ((this.parent != null) || flag)
            {
                PdfOutline parent = this.parent;
                base.WriteObject(writer);
            }
        }

        internal int Count
        {
            get => 
                this.count;
            set
            {
                this.count = value;
            }
        }

        public PdfPage DestinationPage
        {
            get => 
                this.destinationPage;
            set
            {
                this.destinationPage = value;
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public bool Opened
        {
            get => 
                this.opened2;
            set
            {
                this.opened2 = value;
            }
        }

        public PdfOutlineCollection Outlines
        {
            get
            {
                if (this.outlines == null)
                {
                    this.outlines = new PdfOutlineCollection(this.Owner, this);
                }
                return this.outlines;
            }
        }

        internal PdfOutline Parent
        {
            get => 
                this.parent;
            set
            {
                this.parent = value;
            }
        }

        public PdfOutlineStyle Style
        {
            get => 
                ((PdfOutlineStyle) base.Elements.GetInteger("/F"));
            set
            {
                base.Elements.SetInteger("/F", (int) value);
            }
        }

        public XColor TextColor
        {
            get => 
                this.textColor;
            set
            {
                this.textColor = value;
            }
        }

        public string Title
        {
            get => 
                base.Elements.GetString("/Title");
            set
            {
                PdfString str = new PdfString(value, PdfStringEncoding.PDFDocEncoding);
                base.Elements.SetValue("/Title", str);
            }
        }

        internal sealed class Keys : KeysBase
        {
            [KeyInfo(KeyType.Optional | KeyType.Dictionary)]
            public const string A = "/A";
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string C = "/C";
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string Count = "/Count";
            [KeyInfo(KeyType.Optional | KeyType.ArrayOrNameOrString)]
            public const string Dest = "/Dest";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string F = "/F";
            [KeyInfo(KeyType.Required | KeyType.Dictionary)]
            public const string First = "/First";
            [KeyInfo(KeyType.Required | KeyType.Dictionary)]
            public const string Last = "/Last";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Required | KeyType.Dictionary)]
            public const string Next = "/Next";
            [KeyInfo(KeyType.Required | KeyType.Dictionary)]
            public const string Parent = "/Parent";
            [KeyInfo(KeyType.Required | KeyType.Dictionary)]
            public const string Prev = "/Prev";
            [KeyInfo(KeyType.Optional | KeyType.Dictionary)]
            public const string SE = "/SE";
            [KeyInfo(KeyType.Required | KeyType.String)]
            public const string Title = "/Title";
            [KeyInfo(KeyType.Optional | KeyType.Name, FixedValue="Outlines")]
            public const string Type = "/Type";

            public static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfOutline.Keys));
                    }
                    return meta;
                }
            }
        }

        public class PdfOutlineCollection : PdfObject, IEnumerable
        {
            private List<PdfOutline> outlines;
            private PdfOutline parent;

            internal PdfOutlineCollection(PdfDocument document, PdfOutline parent) : base(document)
            {
                this.outlines = new List<PdfOutline>();
                this.parent = parent;
            }

            public void Add(PdfOutline outline)
            {
                if (outline == null)
                {
                    throw new ArgumentNullException("outline");
                }
                if (!object.ReferenceEquals(this.Owner, outline.DestinationPage.Owner))
                {
                    throw new ArgumentException("Destination page must belong to this document.");
                }
                outline.Document = this.Owner;
                outline.parent = this.parent;
                this.outlines.Add(outline);
                this.Owner.irefTable.Add(outline);
                if (outline.Opened)
                {
                    outline = this.parent;
                    while (outline != null)
                    {
                        outline.openCount++;
                        outline = outline.parent;
                    }
                }
            }

            public PdfOutline Add(string title, PdfPage destinationPage)
            {
                PdfOutline outline = new PdfOutline(title, destinationPage);
                this.Add(outline);
                return outline;
            }

            public PdfOutline Add(string title, PdfPage destinationPage, bool opened)
            {
                PdfOutline outline = new PdfOutline(title, destinationPage, opened);
                this.Add(outline);
                return outline;
            }

            public PdfOutline Add(string title, PdfPage destinationPage, bool opened, PdfOutlineStyle style)
            {
                PdfOutline outline = new PdfOutline(title, destinationPage, opened, style);
                this.Add(outline);
                return outline;
            }

            public PdfOutline Add(string title, PdfPage destinationPage, bool opened, PdfOutlineStyle style, XColor textColor)
            {
                PdfOutline outline = new PdfOutline(title, destinationPage, opened, style, textColor);
                this.Add(outline);
                return outline;
            }

            public IEnumerator GetEnumerator() => 
                this.outlines.GetEnumerator();

            public int IndexOf(PdfOutline item) => 
                this.outlines.IndexOf(item);

            public int Count =>
                this.outlines.Count;

            public bool HasOutline =>
                ((this.outlines != null) && (this.outlines.Count > 0));

            public PdfOutline this[int index]
            {
                get
                {
                    if ((index < 0) || (index >= this.outlines.Count))
                    {
                        throw new ArgumentOutOfRangeException("index", index, PSSR.OutlineIndexOutOfRange);
                    }
                    return this.outlines[index];
                }
            }
        }
    }
}

