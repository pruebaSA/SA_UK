namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class BordersRenderer : BorderRendererBase
    {
        private Borders borders;
        internal bool leaveAwayBottom;
        internal bool leaveAwayLeft;
        internal bool leaveAwayRight;
        internal bool leaveAwayTop;

        internal BordersRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.borders = domObj as Borders;
        }

        private BorderRenderer CreateBorderRenderer(Border border, BorderType borderType) => 
            new BorderRenderer(border, base.docRenderer) { 
                ParentCell = base.parentCell,
                ParentFormat = base.parentFormat,
                BorderType = borderType
            };

        internal override void Render()
        {
            base.useEffectiveValue = true;
            this.GetValueAsIntended("Visible");
            this.GetValueAsIntended("Style");
            this.GetValueAsIntended("Color");
            bool flag = base.parentCell != null;
            ParagraphFormat parentFormat = base.parentFormat;
            object valueAsIntended = null;
            if (this.leaveAwayTop)
            {
                base.rtfWriter.WriteControl(base.GetBorderControl(BorderType.Top));
            }
            else
            {
                valueAsIntended = this.GetValueAsIntended("Top");
                if (valueAsIntended != null)
                {
                    this.CreateBorderRenderer((Border) valueAsIntended, BorderType.Top).Render();
                }
                else
                {
                    base.RenderBorder(base.GetBorderControl(BorderType.Top));
                }
            }
            if (!flag)
            {
                base.Translate("DistanceFromTop", "brsp");
            }
            if (this.leaveAwayLeft)
            {
                base.rtfWriter.WriteControl(base.GetBorderControl(BorderType.Top));
            }
            else
            {
                valueAsIntended = this.GetValueAsIntended("Left");
                if (valueAsIntended != null)
                {
                    this.CreateBorderRenderer((Border) valueAsIntended, BorderType.Left).Render();
                }
                else
                {
                    base.RenderBorder(base.GetBorderControl(BorderType.Left));
                }
            }
            if (!flag)
            {
                base.Translate("DistanceFromLeft", "brsp");
            }
            if (this.leaveAwayRight)
            {
                base.rtfWriter.WriteControl(base.GetBorderControl(BorderType.Right));
            }
            else
            {
                valueAsIntended = this.GetValueAsIntended("Right");
                if (valueAsIntended != null)
                {
                    this.CreateBorderRenderer((Border) valueAsIntended, BorderType.Right).Render();
                }
                else
                {
                    base.RenderBorder(base.GetBorderControl(BorderType.Right));
                }
            }
            if (!flag)
            {
                base.Translate("DistanceFromRight", "brsp");
            }
            if (this.leaveAwayBottom)
            {
                base.rtfWriter.WriteControl(base.GetBorderControl(BorderType.Bottom));
            }
            else
            {
                valueAsIntended = this.GetValueAsIntended("Bottom");
                if (valueAsIntended != null)
                {
                    this.CreateBorderRenderer((Border) valueAsIntended, BorderType.Bottom).Render();
                }
                else
                {
                    base.RenderBorder(base.GetBorderControl(BorderType.Bottom));
                }
            }
            if (!flag)
            {
                base.Translate("DistanceFromBottom", "brsp");
            }
            if (flag)
            {
                valueAsIntended = this.GetValueAsIntended("DiagonalDown");
                if (valueAsIntended != null)
                {
                    this.CreateBorderRenderer((Border) valueAsIntended, BorderType.DiagonalDown).Render();
                }
                valueAsIntended = this.GetValueAsIntended("DiagonalUp");
                if (valueAsIntended != null)
                {
                    this.CreateBorderRenderer((Border) valueAsIntended, BorderType.DiagonalUp).Render();
                }
            }
        }
    }
}

