namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Tables;
    using System;
    using System.Globalization;

    internal abstract class BorderRendererBase : RendererBase
    {
        protected Cell parentCell;
        protected ParagraphFormat parentFormat;

        internal BorderRendererBase(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
        }

        protected string GetBorderControl(BorderType type)
        {
            string str;
            if (this.parentCell != null)
            {
                str = "clbrdr";
            }
            else
            {
                str = "brdr";
            }
            switch (type)
            {
                case BorderType.Top:
                    return (str + "t");

                case BorderType.Left:
                    return (str + "l");

                case BorderType.Bottom:
                    return (str + "b");

                case BorderType.Right:
                    return (str + "r");

                case BorderType.Horizontal:
                case BorderType.Vertical:
                    return str;

                case BorderType.DiagonalDown:
                    return "cldglu";

                case BorderType.DiagonalUp:
                    return "cldgll";
            }
            return str;
        }

        private Color GetDefaultColor()
        {
            if (this.parentFormat != null)
            {
                object obj2 = this.parentFormat.GetValue("Font.Color");
                if (obj2 != null)
                {
                    return (Color) obj2;
                }
            }
            return Colors.Black;
        }

        protected void RenderBorder(string borderControl)
        {
            object valueAsIntended = this.GetValueAsIntended("Visible");
            object obj3 = this.GetValueAsIntended("Style");
            object obj4 = this.GetValueAsIntended("Color");
            object obj5 = this.GetValueAsIntended("Width");
            base.rtfWriter.WriteControl(borderControl);
            if (((((valueAsIntended != null) || (obj3 != null)) || ((obj4 != null) || (obj5 != null))) && ((obj5 == null) || (((Unit) obj5) != 0))) && (((valueAsIntended == null) || ((bool) valueAsIntended)) && ((obj3 == null) || (((BorderStyle) obj3) != BorderStyle.None))))
            {
                base.Translate("Style", "brdr", RtfUnit.Undefined, "s", false);
                base.Translate("Width", "brdrw", RtfUnit.Twips, "10", false);
                base.Translate("Color", "brdrcf", RtfUnit.Undefined, base.docRenderer.GetColorIndex(this.GetDefaultColor()).ToString(CultureInfo.InvariantCulture), false);
            }
        }

        internal Cell ParentCell
        {
            set
            {
                this.parentCell = value;
                if (value != null)
                {
                    this.parentFormat = null;
                }
            }
        }

        internal ParagraphFormat ParentFormat
        {
            set
            {
                this.parentFormat = value;
                if (value != null)
                {
                    this.parentCell = null;
                }
            }
        }
    }
}

