namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel.Shapes;
    using PdfSharp.Drawing;
    using System;

    internal class TextFrameRenderer : ShapeRenderer
    {
        private TextFrame textframe;

        internal TextFrameRenderer(XGraphics gfx, TextFrame textframe, FieldInfos fieldInfos) : base(gfx, textframe, fieldInfos)
        {
            this.textframe = textframe;
            TextFrameRenderInfo info = new TextFrameRenderInfo {
                shape = base.shape
            };
            base.renderInfo = info;
        }

        internal TextFrameRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos) : base(gfx, renderInfo, fieldInfos)
        {
            this.textframe = (TextFrame) renderInfo.DocumentObject;
        }

        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            FormattedTextFrame frame = new FormattedTextFrame(this.textframe, base.documentRenderer, base.fieldInfos);
            frame.Format(base.gfx);
            ((TextFrameFormatInfo) base.renderInfo.FormatInfo).formattedTextFrame = frame;
            base.Format(area, previousFormatInfo);
        }

        internal override void Render()
        {
            base.RenderFilling();
            this.RenderContent();
            base.RenderLine();
        }

        private void RenderContent()
        {
            RenderInfo[] renderInfos = ((TextFrameFormatInfo) base.renderInfo.FormatInfo).formattedTextFrame.GetRenderInfos();
            if (renderInfos != null)
            {
                XGraphicsState state = this.Transform();
                base.RenderByInfos(renderInfos);
                this.ResetTransform(state);
            }
        }

        private void ResetTransform(XGraphicsState state)
        {
            if (state != null)
            {
                base.gfx.Restore(state);
            }
        }

        private XGraphicsState Transform()
        {
            XUnit x;
            XUnit y;
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            XGraphicsState state = base.gfx.Save();
            switch (this.textframe.Orientation)
            {
                case TextOrientation.Upward:
                    state = base.gfx.Save();
                    x = contentArea.X;
                    y = ((double) contentArea.Y) + ((double) contentArea.Height);
                    base.gfx.TranslateTransform((double) x, (double) y);
                    base.gfx.RotateTransform(-90.0);
                    return state;

                case TextOrientation.Vertical:
                case TextOrientation.VerticalFarEast:
                case TextOrientation.Downward:
                    x = ((double) contentArea.X) + ((double) contentArea.Width);
                    y = contentArea.Y;
                    base.gfx.TranslateTransform((double) x, (double) y);
                    base.gfx.RotateTransform(90.0);
                    return state;
            }
            x = contentArea.X;
            y = contentArea.Y;
            base.gfx.TranslateTransform((double) x, (double) y);
            return state;
        }

        internal override LayoutInfo InitialLayoutInfo =>
            base.InitialLayoutInfo;
    }
}

