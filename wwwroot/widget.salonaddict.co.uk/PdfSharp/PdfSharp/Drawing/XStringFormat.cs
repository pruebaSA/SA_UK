namespace PdfSharp.Drawing
{
    using System;
    using System.Drawing;

    public class XStringFormat
    {
        private XStringAlignment alignment;
        private XStringFormatFlags formatFlags;
        private XLineAlignment lineAlignment;
        private StringFormat stringFormat = ((StringFormat) StringFormat.GenericTypographic.Clone());

        internal StringFormat RealizeGdiStringFormat()
        {
            if (this.stringFormat == null)
            {
                this.stringFormat = StringFormat.GenericTypographic;
                this.stringFormat.Alignment = (StringAlignment) this.alignment;
                this.stringFormat.LineAlignment = (StringAlignment) this.lineAlignment;
                this.stringFormat.FormatFlags = (StringFormatFlags) this.formatFlags;
            }
            return this.stringFormat;
        }

        public XStringAlignment Alignment
        {
            get => 
                this.alignment;
            set
            {
                this.alignment = value;
                this.stringFormat.Alignment = (StringAlignment) value;
            }
        }

        [Obsolete("Use XStringFormats.BottomCenter. (Note plural in class name.)")]
        public static XStringFormat BottomCenter =>
            new XStringFormat { 
                Alignment=XStringAlignment.Center,
                LineAlignment=XLineAlignment.Far
            };

        [Obsolete("Use XStringFormats.Center. (Note plural in class name.)")]
        public static XStringFormat Center =>
            new XStringFormat { 
                Alignment=XStringAlignment.Center,
                LineAlignment=XLineAlignment.Center
            };

        [Obsolete("Use XStringFormats.Default. (Note plural in class name.)")]
        public static XStringFormat Default =>
            new XStringFormat { LineAlignment=XLineAlignment.BaseLine };

        public XStringFormatFlags FormatFlags
        {
            get => 
                this.formatFlags;
            set
            {
                this.formatFlags = value;
                this.stringFormat.FormatFlags = (StringFormatFlags) value;
            }
        }

        public XLineAlignment LineAlignment
        {
            get => 
                this.lineAlignment;
            set
            {
                this.lineAlignment = value;
                if (value == XLineAlignment.BaseLine)
                {
                    this.stringFormat.LineAlignment = StringAlignment.Near;
                }
                else
                {
                    this.stringFormat.LineAlignment = (StringAlignment) value;
                }
            }
        }

        [Obsolete("Use XStringFormats.TopCenter. (Note plural in class name.)")]
        public static XStringFormat TopCenter =>
            new XStringFormat { 
                Alignment=XStringAlignment.Center,
                LineAlignment=XLineAlignment.Near
            };

        [Obsolete("Use XStringFormats.Default. (Note plural in class name.)")]
        public static XStringFormat TopLeft =>
            new XStringFormat { 
                Alignment=XStringAlignment.Near,
                LineAlignment=XLineAlignment.Near
            };
    }
}

