namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes.Charts;
    using MigraDoc.Rendering;
    using MigraDoc.RtfRendering.Resources;
    using PdfSharp.Drawing;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    internal class ChartRenderer : MigraDoc.RtfRendering.ShapeRenderer
    {
        private Chart chart;
        private bool isInline;

        internal ChartRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.chart = (Chart) domObj;
            this.isInline = DocumentRelations.HasParentOfType(this.chart, typeof(Paragraph)) || this.RenderInParagraph();
        }

        private void EndImageDescription()
        {
            if (this.isInline)
            {
                base.rtfWriter.EndContent();
                base.rtfWriter.EndContent();
            }
            else
            {
                base.rtfWriter.EndContent();
                base.EndNameValuePair();
            }
        }

        protected override Unit GetShapeHeight() => 
            (((float) base.GetShapeHeight()) + ((float) base.GetLineWidth()));

        protected override Unit GetShapeWidth() => 
            (((float) base.GetShapeWidth()) + ((float) base.GetLineWidth()));

        internal override void Render()
        {
            string tempFileName = Path.GetTempFileName();
            if (this.StoreTempImage(tempFileName))
            {
                bool flag = this.RenderInParagraph();
                DocumentElements parent = DocumentRelations.GetParent(this.chart) as DocumentElements;
                if (((parent != null) && !flag) && (!(DocumentRelations.GetParent(parent) is Section) && !(DocumentRelations.GetParent(parent) is HeaderFooter)))
                {
                    Trace.WriteLine(Messages.ChartFreelyPlacedInWrongContext, "warning");
                }
                else
                {
                    if (flag)
                    {
                        this.StartDummyParagraph();
                    }
                    if (!this.isInline)
                    {
                        this.StartShapeArea();
                    }
                    this.RenderImage(tempFileName);
                    if (!this.isInline)
                    {
                        this.EndShapeArea();
                    }
                    if (flag)
                    {
                        this.EndDummyParagraph();
                    }
                    if (File.Exists(tempFileName))
                    {
                        File.Delete(tempFileName);
                    }
                }
            }
        }

        private void RenderByteSeries(string fileName)
        {
            FileStream stream = null;
            try
            {
                int num;
                stream = new FileStream(fileName, FileMode.Open);
                stream.Seek(0L, SeekOrigin.Begin);
                while ((num = stream.ReadByte()) != -1)
                {
                    string text = num.ToString("x");
                    if (text.Length == 1)
                    {
                        base.rtfWriter.WriteText("0");
                    }
                    base.rtfWriter.WriteText(text);
                }
            }
            catch
            {
                Trace.WriteLine("Chart image file not read", "warning");
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        private void RenderDimensionSettings()
        {
            base.rtfWriter.WriteControl("picscalex", 100);
            base.rtfWriter.WriteControl("picscaley", 100);
            base.RenderUnit("pichgoal", this.GetShapeHeight());
            base.RenderUnit("picwgoal", this.GetShapeWidth());
            base.rtfWriter.WriteControl("pich", (int) (this.GetShapeHeight().Millimeter * 100.0));
            base.rtfWriter.WriteControl("picw", (int) (this.GetShapeWidth().Millimeter * 100.0));
        }

        private void RenderImage(string fileName)
        {
            this.StartImageDescription();
            this.RenderImageAttributes();
            this.RenderByteSeries(fileName);
            this.EndImageDescription();
        }

        private void RenderImageAttributes()
        {
            if (this.isInline)
            {
                base.rtfWriter.StartContent();
                base.rtfWriter.WriteControlWithStar("picprop");
                base.RenderNameValuePair("shapeType", "75");
                base.RenderFillFormat();
                base.rtfWriter.EndContent();
            }
            this.RenderDimensionSettings();
            this.RenderSourceType();
        }

        private void RenderSourceType()
        {
            base.rtfWriter.WriteControl("pngblip");
        }

        private void StartImageDescription()
        {
            if (this.isInline)
            {
                base.rtfWriter.StartContent();
                base.rtfWriter.WriteControlWithStar("shppict");
                base.rtfWriter.StartContent();
                base.rtfWriter.WriteControl("pict");
            }
            else
            {
                base.RenderNameValuePair("shapeType", "75");
                base.StartNameValuePair("pib");
                base.rtfWriter.StartContent();
                base.rtfWriter.WriteControl("pict");
            }
        }

        private bool StoreTempImage(string fileName)
        {
            try
            {
                float xDpi = 96f;
                int width = (int) (this.GetShapeWidth().Inch * xDpi);
                int height = (int) (this.GetShapeHeight().Inch * xDpi);
                Bitmap bitmap = new Bitmap(width, height);
                XGraphics graphics = XGraphics.CreateMeasureContext(new XSize((double) width, (double) height), XGraphicsUnit.Point, XPageDirection.Downwards);
                graphics.ScaleTransform((double) (xDpi / 72f));
                new DocumentRenderer(this.chart.Document).RenderObject(graphics, 0, 0, this.GetShapeWidth().Point, this.chart);
                bitmap.SetResolution(xDpi, xDpi);
                bitmap.Save(fileName, ImageFormat.Png);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}

