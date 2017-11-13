namespace MigraDoc.RtfRendering
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.RtfRendering.Resources;
    using PdfSharp.Drawing;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    internal class ImageRenderer : ShapeRenderer
    {
        private string filePath;
        private Image image;
        private Stream imageFile;
        private Unit imageHeight;
        private Unit imageWidth;
        private bool isInline;
        private Unit originalHeight;
        private Unit originalWidth;
        private double scaleHeight;
        private double scaleWidth;

        internal ImageRenderer(DocumentObject domObj, RtfDocumentRenderer docRenderer) : base(domObj, docRenderer)
        {
            this.image = domObj as Image;
            this.filePath = this.image.GetFilePath(base.docRenderer.WorkingDirectory);
            this.isInline = DocumentRelations.HasParentOfType(this.image, typeof(Paragraph)) || this.RenderInParagraph();
            this.CalculateImageDimensions();
        }

        private void CalculateImageDimensions()
        {
            try
            {
                float valueAsIntended;
                float num2;
                this.imageFile = File.OpenRead(this.filePath);
                XImage image = XImage.FromFile(this.filePath);
                string str = Path.GetExtension(this.filePath).ToLower();
                float horizontalResolution = (float) image.HorizontalResolution;
                float verticalResolution = (float) image.VerticalResolution;
                this.originalHeight = ((float) (image.PixelHeight * 0x48)) / verticalResolution;
                this.originalWidth = ((float) (image.PixelWidth * 0x48)) / horizontalResolution;
                if (this.image.IsNull("Resolution"))
                {
                    valueAsIntended = (str == ".gif") ? 72f : ((float) image.HorizontalResolution);
                    num2 = (str == ".gif") ? 72f : ((float) image.VerticalResolution);
                }
                else
                {
                    valueAsIntended = (float) this.GetValueAsIntended("Resolution");
                    num2 = valueAsIntended;
                }
                Unit unit = (image.Size.Height * 72.0) / ((double) num2);
                Unit unit2 = (image.Size.Width * 72.0) / ((double) valueAsIntended);
                this.imageHeight = unit;
                this.imageWidth = unit2;
                bool flag = this.image.IsNull("ScaleWidth");
                bool flag2 = this.image.IsNull("ScaleHeight");
                float num5 = flag2 ? 1f : ((float) this.GetValueAsIntended("ScaleHeight"));
                this.scaleHeight = num5;
                float num6 = flag ? 1f : ((float) this.GetValueAsIntended("ScaleWidth"));
                this.scaleWidth = num6;
                if ((this.image.IsNull("LockAspectRatio") || this.image.LockAspectRatio) && (flag2 || flag))
                {
                    if (!this.image.IsNull("Width") && this.image.IsNull("Height"))
                    {
                        this.imageWidth = this.image.Width;
                        this.imageHeight = (((float) unit) * ((float) this.imageWidth)) / ((float) unit2);
                    }
                    else if (!this.image.IsNull("Height") && this.image.IsNull("Width"))
                    {
                        this.imageHeight = this.image.Height;
                        this.imageWidth = (((float) unit2) * ((float) this.imageHeight)) / ((float) unit);
                    }
                    else if (!this.image.IsNull("Height") && !this.image.IsNull("Width"))
                    {
                        this.imageWidth = this.image.Width;
                        this.imageHeight = this.image.Height;
                    }
                    if (flag && !flag2)
                    {
                        this.scaleWidth = this.scaleHeight;
                    }
                    else if (flag2 && !flag)
                    {
                        this.scaleHeight = this.scaleWidth;
                    }
                }
                else
                {
                    if (!this.image.IsNull("Width"))
                    {
                        this.imageWidth = this.image.Width;
                    }
                    if (!this.image.IsNull("Height"))
                    {
                        this.imageHeight = this.image.Height;
                    }
                }
                return;
            }
            catch (FileNotFoundException)
            {
                Trace.WriteLine(Messages.ImageNotFound(this.image.Name), "warning");
            }
            catch (Exception exception)
            {
                Trace.WriteLine(Messages.ImageNotReadable(this.image.Name, exception.Message), "warning");
            }
            this.imageFile = null;
            this.imageHeight = (Unit) base.GetValueOrDefault("Height", Unit.FromInch(1.0));
            this.imageWidth = (Unit) base.GetValueOrDefault("Width", Unit.FromInch(1.0));
            this.scaleHeight = (double) base.GetValueOrDefault("ScaleHeight", 1.0);
            this.scaleWidth = (double) base.GetValueOrDefault("ScaleWidth", 1.0);
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
            (((double) this.imageHeight) * this.scaleHeight);

        protected override Unit GetShapeWidth() => 
            (((double) this.imageWidth) * this.scaleWidth);

        internal override void Render()
        {
            bool flag = this.RenderInParagraph();
            DocumentElements parent = DocumentRelations.GetParent(this.image) as DocumentElements;
            if (((parent != null) && !flag) && (!(DocumentRelations.GetParent(parent) is Section) && !(DocumentRelations.GetParent(parent) is HeaderFooter)))
            {
                Trace.WriteLine(Messages.ImageFreelyPlacedInWrongContext(this.image.Name), "warning");
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
                this.RenderImage();
                if (!this.isInline)
                {
                    this.EndShapeArea();
                }
                if (flag)
                {
                    this.EndDummyParagraph();
                }
            }
        }

        private void RenderByteSeries()
        {
            if (this.imageFile != null)
            {
                int num;
                this.imageFile.Seek(0L, SeekOrigin.Begin);
                while ((num = this.imageFile.ReadByte()) != -1)
                {
                    string text = num.ToString("x");
                    if (text.Length == 1)
                    {
                        base.rtfWriter.WriteText("0");
                    }
                    base.rtfWriter.WriteText(text);
                }
                this.imageFile.Close();
            }
        }

        private void RenderCropping()
        {
            base.Translate("PictureFormat.CropLeft", "piccropl");
            base.Translate("PictureFormat.CropRight", "piccropr");
            base.Translate("PictureFormat.CropTop", "piccropt");
            base.Translate("PictureFormat.CropBottom", "piccropb");
        }

        private void RenderDimensionSettings()
        {
            float num = ((float) this.GetShapeWidth()) / ((float) this.originalWidth);
            float num2 = ((float) this.GetShapeHeight()) / ((float) this.originalHeight);
            base.rtfWriter.WriteControl("picscalex", (int) (num * 100f));
            base.rtfWriter.WriteControl("picscaley", (int) (num2 * 100f));
            base.RenderUnit("pichgoal", ((float) this.GetShapeHeight()) / num2);
            base.RenderUnit("picwgoal", ((float) this.GetShapeWidth()) / num);
            base.rtfWriter.WriteControl("pich", (int) (this.originalHeight.Millimeter * 100.0));
            base.rtfWriter.WriteControl("picw", (int) (this.originalWidth.Millimeter * 100.0));
        }

        private void RenderImage()
        {
            this.StartImageDescription();
            this.RenderImageAttributes();
            this.RenderByteSeries();
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
                base.RenderLineFormat();
                base.rtfWriter.EndContent();
            }
            this.RenderDimensionSettings();
            this.RenderCropping();
            this.RenderSourceType();
        }

        private void RenderSourceType()
        {
            string extension = Path.GetExtension(this.filePath);
            if (extension == null)
            {
                this.imageFile = null;
                Trace.WriteLine("No Image type given.", "warning");
            }
            else
            {
                switch (extension.ToLower())
                {
                    case ".jpeg":
                    case ".jpg":
                        base.rtfWriter.WriteControl("jpegblip");
                        return;

                    case ".png":
                        base.rtfWriter.WriteControl("pngblip");
                        return;

                    case ".gif":
                        base.rtfWriter.WriteControl("pngblip");
                        return;

                    case ".pdf":
                        this.imageFile = Assembly.GetExecutingAssembly().GetManifestResourceStream("MigraDoc.RtfRendering.Resources.PDF.png");
                        base.rtfWriter.WriteControl("pngblip");
                        return;
                }
                Trace.WriteLine(Messages.ImageTypeNotSupported(this.image.Name), "warning");
                this.imageFile = null;
            }
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
    }
}

