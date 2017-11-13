namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel.Internals;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using MigraDoc.DocumentObjectModel.Tables;
    using MigraDoc.Rendering.ChartMapper;
    using PdfSharp.Charting;
    using PdfSharp.Drawing;
    using System;
    using System.Runtime.InteropServices;

    internal class ChartRenderer : ShapeRenderer
    {
        private Chart chart;

        internal ChartRenderer(XGraphics gfx, Chart chart, FieldInfos fieldInfos) : base(gfx, chart, fieldInfos)
        {
            this.chart = chart;
            ChartRenderInfo info = new ChartRenderInfo {
                shape = base.shape
            };
            base.renderInfo = info;
        }

        internal ChartRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos) : base(gfx, renderInfo, fieldInfos)
        {
            this.chart = (Chart) renderInfo.DocumentObject;
        }

        private XUnit AlignVertically(VerticalAlignment vAlign, XUnit top, XUnit bottom, XUnit height)
        {
            switch (vAlign)
            {
                case VerticalAlignment.Center:
                    return (((((double) top) + ((double) bottom)) - ((double) height)) / 2.0);

                case VerticalAlignment.Bottom:
                    return (((double) bottom) - ((double) height));
            }
            return top;
        }

        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            ChartFormatInfo formatInfo = (ChartFormatInfo) base.renderInfo.FormatInfo;
            TextArea area2 = (TextArea) this.chart.GetValue("HeaderArea", GV.ReadOnly);
            formatInfo.formattedHeader = this.GetFormattedTextArea(area2, this.chart.Width.Point);
            area2 = (TextArea) this.chart.GetValue("FooterArea", GV.ReadOnly);
            formatInfo.formattedFooter = this.GetFormattedTextArea(area2, this.chart.Width.Point);
            area2 = (TextArea) this.chart.GetValue("LeftArea", GV.ReadOnly);
            formatInfo.formattedLeft = this.GetFormattedTextArea(area2);
            area2 = (TextArea) this.chart.GetValue("RightArea", GV.ReadOnly);
            formatInfo.formattedRight = this.GetFormattedTextArea(area2);
            area2 = (TextArea) this.chart.GetValue("TopArea", GV.ReadOnly);
            formatInfo.formattedTop = this.GetFormattedTextArea(area2, this.GetTopBottomWidth());
            area2 = (TextArea) this.chart.GetValue("BottomArea", GV.ReadOnly);
            formatInfo.formattedBottom = this.GetFormattedTextArea(area2, this.GetTopBottomWidth());
            base.Format(area, previousFormatInfo);
            formatInfo.chartFrame = MigraDoc.Rendering.ChartMapper.ChartMapper.Map(this.chart);
        }

        private Rectangle GetBottomRect()
        {
            XUnit unit;
            XUnit unit2;
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo) base.renderInfo.FormatInfo;
            this.GetTopBottomHorizontalPosition(out unit, out unit2);
            XUnit y = (((double) contentArea.Y) + ((double) contentArea.Height)) - ((double) formatInfo.formattedBottom.InnerHeight);
            if (formatInfo.formattedFooter != null)
            {
                y = ((double) y) - ((double) formatInfo.formattedFooter.InnerHeight);
            }
            return new Rectangle(unit, y, ((double) unit2) - ((double) unit), formatInfo.formattedBottom.InnerHeight);
        }

        private Rectangle GetFooterRect()
        {
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo) base.renderInfo.FormatInfo;
            XUnit x = contentArea.X;
            XUnit y = (((double) contentArea.Y) + ((double) contentArea.Height)) - ((double) formatInfo.formattedFooter.InnerHeight);
            XUnit width = contentArea.Width;
            return new Rectangle(x, y, width, formatInfo.formattedFooter.InnerHeight);
        }

        private FormattedTextArea GetFormattedTextArea(TextArea area) => 
            this.GetFormattedTextArea(area, (double) 1.0 / (double) 0.0);

        private FormattedTextArea GetFormattedTextArea(TextArea area, XUnit width)
        {
            if (area == null)
            {
                return null;
            }
            FormattedTextArea area2 = new FormattedTextArea(base.documentRenderer, area, base.fieldInfos);
            if (!double.IsNaN((double) width))
            {
                area2.InnerWidth = width;
            }
            area2.Format(base.gfx);
            return area2;
        }

        private Rectangle GetHeaderRect()
        {
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo) base.renderInfo.FormatInfo;
            XUnit x = contentArea.X;
            XUnit y = contentArea.Y;
            XUnit width = contentArea.Width;
            return new Rectangle(x, y, width, formatInfo.formattedHeader.InnerHeight);
        }

        private Rectangle GetLeftRect()
        {
            XUnit unit;
            XUnit unit2;
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo) base.renderInfo.FormatInfo;
            this.GetLeftRightVerticalPosition(out unit, out unit2);
            XUnit x = contentArea.X;
            return new Rectangle(x, unit, formatInfo.formattedLeft.InnerWidth, ((double) unit2) - ((double) unit));
        }

        private void GetLeftRightVerticalPosition(out XUnit top, out XUnit bottom)
        {
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo) base.renderInfo.FormatInfo;
            top = contentArea.Y;
            if (formatInfo.formattedHeader != null)
            {
                top = ((double) top) + ((double) formatInfo.formattedHeader.InnerHeight);
            }
            bottom = ((double) contentArea.Y) + ((double) contentArea.Height);
            if (formatInfo.formattedFooter != null)
            {
                bottom = ((double) bottom) - ((double) formatInfo.formattedFooter.InnerHeight);
            }
        }

        private Rectangle GetPlotRect()
        {
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo) base.renderInfo.FormatInfo;
            XUnit y = contentArea.Y;
            if (formatInfo.formattedHeader != null)
            {
                y = ((double) y) + ((double) formatInfo.formattedHeader.InnerHeight);
            }
            if (formatInfo.formattedTop != null)
            {
                y = ((double) y) + ((double) formatInfo.formattedTop.InnerHeight);
            }
            XUnit unit2 = ((double) contentArea.Y) + ((double) contentArea.Height);
            if (formatInfo.formattedFooter != null)
            {
                unit2 = ((double) unit2) - ((double) formatInfo.formattedFooter.InnerHeight);
            }
            if (formatInfo.formattedBottom != null)
            {
                unit2 = ((double) unit2) - ((double) formatInfo.formattedBottom.InnerHeight);
            }
            XUnit x = contentArea.X;
            if (formatInfo.formattedLeft != null)
            {
                x = ((double) x) + ((double) formatInfo.formattedLeft.InnerWidth);
            }
            XUnit unit4 = ((double) contentArea.X) + ((double) contentArea.Width);
            if (formatInfo.formattedRight != null)
            {
                unit4 = ((double) unit4) - ((double) formatInfo.formattedRight.InnerWidth);
            }
            return new Rectangle(x, y, ((double) unit4) - ((double) x), ((double) unit2) - ((double) y));
        }

        private Rectangle GetRightRect()
        {
            XUnit unit;
            XUnit unit2;
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo) base.renderInfo.FormatInfo;
            this.GetLeftRightVerticalPosition(out unit, out unit2);
            XUnit x = (((double) contentArea.X) + ((double) contentArea.Width)) - ((double) formatInfo.formattedRight.InnerWidth);
            return new Rectangle(x, unit, formatInfo.formattedRight.InnerWidth, ((double) unit2) - ((double) unit));
        }

        private void GetTopBottomHorizontalPosition(out XUnit left, out XUnit right)
        {
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo) base.renderInfo.FormatInfo;
            left = contentArea.X;
            right = ((double) contentArea.X) + ((double) contentArea.Width);
            if (formatInfo.formattedRight != null)
            {
                right = ((double) right) - ((double) formatInfo.formattedRight.InnerWidth);
            }
            if (formatInfo.formattedLeft != null)
            {
                left = ((double) left) + ((double) formatInfo.formattedLeft.InnerWidth);
            }
        }

        private XUnit GetTopBottomWidth()
        {
            ChartFormatInfo formatInfo = (ChartFormatInfo) base.renderInfo.FormatInfo;
            XUnit point = this.chart.Width.Point;
            if (formatInfo.formattedRight != null)
            {
                point = ((double) point) - ((double) formatInfo.formattedRight.InnerWidth);
            }
            if (formatInfo.formattedLeft != null)
            {
                point = ((double) point) - ((double) formatInfo.formattedLeft.InnerWidth);
            }
            return point;
        }

        private Rectangle GetTopRect()
        {
            XUnit unit;
            XUnit unit2;
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo) base.renderInfo.FormatInfo;
            this.GetTopBottomHorizontalPosition(out unit, out unit2);
            XUnit y = contentArea.Y;
            if (formatInfo.formattedHeader != null)
            {
                y = ((double) y) + ((double) formatInfo.formattedHeader.InnerHeight);
            }
            return new Rectangle(unit, y, ((double) unit2) - ((double) unit), formatInfo.formattedTop.InnerHeight);
        }

        internal override void Render()
        {
            base.RenderFilling();
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            ChartFormatInfo formatInfo = (ChartFormatInfo) base.renderInfo.FormatInfo;
            if (formatInfo.formattedHeader != null)
            {
                this.RenderArea(formatInfo.formattedHeader, this.GetHeaderRect());
            }
            if (formatInfo.formattedFooter != null)
            {
                this.RenderArea(formatInfo.formattedFooter, this.GetFooterRect());
            }
            if (formatInfo.formattedTop != null)
            {
                this.RenderArea(formatInfo.formattedTop, this.GetTopRect());
            }
            if (formatInfo.formattedBottom != null)
            {
                this.RenderArea(formatInfo.formattedBottom, this.GetBottomRect());
            }
            if (formatInfo.formattedLeft != null)
            {
                this.RenderArea(formatInfo.formattedLeft, this.GetLeftRect());
            }
            if (formatInfo.formattedRight != null)
            {
                this.RenderArea(formatInfo.formattedRight, this.GetRightRect());
            }
            PlotArea area = (PlotArea) this.chart.GetValue("PlotArea", GV.ReadOnly);
            if (area != null)
            {
                this.RenderPlotArea(area, this.GetPlotRect());
            }
            base.RenderLine();
        }

        private void RenderArea(FormattedTextArea area, Rectangle rect)
        {
            if (area != null)
            {
                TextArea textArea = area.textArea;
                new FillFormatRenderer((FillFormat) textArea.GetValue("FillFormat", GV.ReadOnly), base.gfx).Render(rect.X, rect.Y, rect.Width, rect.Height);
                XUnit top = ((double) rect.Y) + ((double) textArea.TopPadding);
                XUnit bottom = ((double) rect.Y) + ((double) rect.Height);
                bottom = ((double) bottom) - ((double) textArea.BottomPadding);
                top = this.AlignVertically(textArea.VerticalAlignment, top, bottom, area.ContentHeight);
                XUnit xShift = ((double) rect.X) + ((double) textArea.LeftPadding);
                RenderInfo[] renderInfos = area.GetRenderInfos();
                base.RenderByInfos(xShift, top, renderInfos);
                new LineFormatRenderer((LineFormat) textArea.GetValue("LineFormat", GV.ReadOnly), base.gfx).Render(rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        private void RenderPlotArea(PlotArea area, Rectangle rect)
        {
            ChartFrame chartFrame = ((ChartFormatInfo) base.renderInfo.FormatInfo).chartFrame;
            XUnit unit = ((double) rect.Y) + ((double) area.TopPadding);
            XUnit unit2 = ((double) rect.Y) + ((double) rect.Height);
            unit2 = ((double) unit2) - ((double) area.BottomPadding);
            XUnit unit3 = ((double) rect.X) + ((double) area.LeftPadding);
            XUnit unit4 = ((double) rect.X) + ((double) rect.Width);
            unit4 = ((double) unit4) - ((double) area.RightPadding);
            chartFrame.set_Location(new XPoint((double) unit3, (double) unit));
            chartFrame.set_Size(new XSize(((double) unit4) - ((double) unit3), ((double) unit2) - ((double) unit)));
            chartFrame.DrawChart(base.gfx);
        }
    }
}

