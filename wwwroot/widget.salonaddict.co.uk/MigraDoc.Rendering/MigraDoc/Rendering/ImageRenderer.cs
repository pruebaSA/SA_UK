namespace MigraDoc.Rendering
{
    using MigraDoc.DocumentObjectModel.Shapes;
    using MigraDoc.Rendering.Resources;
    using PdfSharp.Drawing;
    using System;
    using System.Diagnostics;

    internal class ImageRenderer : ShapeRenderer
    {
        private ImageFailure failure;
        private Image image;
        private string imageFilePath;

        internal ImageRenderer(XGraphics gfx, Image image, FieldInfos fieldInfos) : base(gfx, image, fieldInfos)
        {
            this.image = image;
            ImageRenderInfo info = new ImageRenderInfo {
                shape = base.shape
            };
            base.renderInfo = info;
        }

        internal ImageRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos) : base(gfx, renderInfo, fieldInfos)
        {
            this.image = (Image) renderInfo.DocumentObject;
        }

        private void CalculateImageDimensions()
        {
            ImageFormatInfo formatInfo = (ImageFormatInfo) base.renderInfo.FormatInfo;
            if (formatInfo.failure == ImageFailure.None)
            {
                XImage image = null;
                try
                {
                    image = XImage.FromFile(this.imageFilePath);
                }
                catch (InvalidOperationException exception)
                {
                    Trace.WriteLine(Messages.InvalidImageType(exception.Message));
                    formatInfo.failure = ImageFailure.InvalidType;
                }
                try
                {
                    XUnit point = this.image.Width.Point;
                    XUnit unit2 = this.image.Height.Point;
                    bool flag = !this.image.IsNull("Width");
                    bool flag2 = !this.image.IsNull("Height");
                    XUnit unit3 = point;
                    XUnit unit4 = unit2;
                    double pixelWidth = image.PixelWidth;
                    bool flag3 = !this.image.IsNull("Resolution");
                    double num2 = flag3 ? this.image.Resolution : image.HorizontalResolution;
                    XUnit unit5 = XUnit.FromInch(pixelWidth / num2);
                    double pixelHeight = image.PixelHeight;
                    double num4 = flag3 ? this.image.Resolution : image.VerticalResolution;
                    XUnit unit6 = XUnit.FromInch(pixelHeight / num4);
                    bool flag4 = this.image.IsNull("LockAspectRatio") || this.image.LockAspectRatio;
                    double scaleHeight = this.image.ScaleHeight;
                    double scaleWidth = this.image.ScaleWidth;
                    bool flag5 = !this.image.IsNull("ScaleHeight");
                    bool flag6 = !this.image.IsNull("ScaleWidth");
                    if (flag4 && (!flag5 || !flag6))
                    {
                        if (flag && !flag2)
                        {
                            unit4 = (((double) unit6) / ((double) unit5)) * ((double) point);
                        }
                        else if (flag2 && !flag)
                        {
                            unit3 = (((double) unit5) / ((double) unit6)) * ((double) unit2);
                        }
                        else if (!flag2 && !flag)
                        {
                            unit4 = unit6;
                            unit3 = unit5;
                        }
                        if (flag5)
                        {
                            unit4 = ((double) unit4) * scaleHeight;
                            unit3 = ((double) unit3) * scaleHeight;
                        }
                        if (flag6)
                        {
                            unit4 = ((double) unit4) * scaleWidth;
                            unit3 = ((double) unit3) * scaleWidth;
                        }
                    }
                    else
                    {
                        if (!flag2)
                        {
                            unit4 = unit6;
                        }
                        if (!flag)
                        {
                            unit3 = unit5;
                        }
                        if (flag5)
                        {
                            unit4 = ((double) unit4) * scaleHeight;
                        }
                        if (flag6)
                        {
                            unit3 = ((double) unit3) * scaleWidth;
                        }
                    }
                    formatInfo.CropWidth = (int) pixelWidth;
                    formatInfo.CropHeight = (int) pixelHeight;
                    if (!this.image.IsNull("PictureFormat"))
                    {
                        PictureFormat pictureFormat = this.image.PictureFormat;
                        XUnit unit7 = pictureFormat.CropLeft.Point;
                        XUnit unit8 = pictureFormat.CropRight.Point;
                        XUnit unit9 = pictureFormat.CropTop.Point;
                        XUnit unit10 = pictureFormat.CropBottom.Point;
                        formatInfo.CropX = (int) (num2 * unit7.Inch);
                        formatInfo.CropY = (int) (num4 * unit9.Inch);
                        XUnit unit17 = ((double) unit7) + ((double) unit8);
                        formatInfo.CropWidth -= (int) (num2 * unit17.Inch);
                        XUnit unit18 = ((double) unit9) + ((double) unit10);
                        formatInfo.CropHeight -= (int) (num4 * unit18.Inch);
                        double num7 = ((double) unit3) / ((double) unit5);
                        double num8 = ((double) unit4) / ((double) unit6);
                        unit7 = num7 * ((double) unit7);
                        unit8 = num7 * ((double) unit8);
                        unit9 = num8 * ((double) unit9);
                        unit10 = num8 * ((double) unit10);
                        unit4 = (((double) unit4) - ((double) unit9)) - ((double) unit10);
                        unit3 = (((double) unit3) - ((double) unit7)) - ((double) unit8);
                    }
                    if ((((double) unit4) <= 0.0) || (((double) unit3) <= 0.0))
                    {
                        formatInfo.Width = XUnit.FromCentimeter(2.5);
                        formatInfo.Height = XUnit.FromCentimeter(2.5);
                        Trace.WriteLine(Messages.EmptyImageSize);
                        this.failure = ImageFailure.EmptySize;
                    }
                    else
                    {
                        formatInfo.Width = unit3;
                        formatInfo.Height = unit4;
                    }
                }
                catch (Exception exception2)
                {
                    Trace.WriteLine(Messages.ImageNotReadable(this.image.Name, exception2.Message));
                    formatInfo.failure = ImageFailure.NotRead;
                }
                finally
                {
                    if (image != null)
                    {
                        image.Dispose();
                    }
                }
            }
            if (formatInfo.failure != ImageFailure.None)
            {
                if (!this.image.IsNull("Width"))
                {
                    formatInfo.Width = this.image.Width.Point;
                }
                else
                {
                    formatInfo.Width = XUnit.FromCentimeter(2.5);
                }
                if (!this.image.IsNull("Height"))
                {
                    formatInfo.Height = this.image.Height.Point;
                }
                else
                {
                    formatInfo.Height = XUnit.FromCentimeter(2.5);
                }
            }
        }

        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            this.imageFilePath = this.image.GetFilePath(base.documentRenderer.WorkingDirectory);
            if (!XImage.ExistsFile(this.imageFilePath))
            {
                this.failure = ImageFailure.FileNotFound;
                Trace.WriteLine(Messages.ImageNotFound(this.image.Name), "warning");
            }
            ImageFormatInfo formatInfo = (ImageFormatInfo) base.renderInfo.FormatInfo;
            formatInfo.failure = this.failure;
            formatInfo.ImagePath = this.imageFilePath;
            this.CalculateImageDimensions();
            base.Format(area, previousFormatInfo);
        }

        internal override void Render()
        {
            base.RenderFilling();
            ImageFormatInfo formatInfo = (ImageFormatInfo) base.renderInfo.FormatInfo;
            Area contentArea = base.renderInfo.LayoutInfo.ContentArea;
            XRect destRect = new XRect((double) contentArea.X, (double) contentArea.Y, (double) formatInfo.Width, (double) formatInfo.Height);
            if (formatInfo.failure == ImageFailure.None)
            {
                using (XImage image = null)
                {
                    try
                    {
                        XRect srcRect = new XRect((double) formatInfo.CropX, (double) formatInfo.CropY, (double) formatInfo.CropWidth, (double) formatInfo.CropHeight);
                        image = XImage.FromFile(formatInfo.ImagePath);
                        base.gfx.DrawImage(image, destRect, srcRect, XGraphicsUnit.Point);
                    }
                    catch (Exception)
                    {
                        this.RenderFailureImage(destRect);
                    }
                    goto Label_00C7;
                }
            }
            this.RenderFailureImage(destRect);
        Label_00C7:
            base.RenderLine();
        }

        private void RenderFailureImage(XRect destRect)
        {
            string displayImageFileNotFound;
            base.gfx.DrawRectangle(XBrushes.LightGray, destRect);
            ImageFormatInfo formatInfo = (ImageFormatInfo) base.RenderInfo.FormatInfo;
            switch (formatInfo.failure)
            {
                case ImageFailure.FileNotFound:
                    displayImageFileNotFound = Messages.DisplayImageFileNotFound;
                    break;

                case ImageFailure.InvalidType:
                    displayImageFileNotFound = Messages.DisplayInvalidImageType;
                    break;

                case ImageFailure.EmptySize:
                    displayImageFileNotFound = Messages.DisplayEmptyImageSize;
                    break;

                default:
                    displayImageFileNotFound = Messages.DisplayImageNotRead;
                    break;
            }
            XFont font = new XFont("Courier New", 8.0);
            base.gfx.DrawString(displayImageFileNotFound, font, XBrushes.Red, destRect, XStringFormats.Center);
        }

        protected override XUnit ShapeHeight
        {
            get
            {
                ImageFormatInfo formatInfo = (ImageFormatInfo) base.renderInfo.FormatInfo;
                return (((double) formatInfo.Height) + ((double) base.lineFormatRenderer.GetWidth()));
            }
        }

        protected override XUnit ShapeWidth
        {
            get
            {
                ImageFormatInfo formatInfo = (ImageFormatInfo) base.renderInfo.FormatInfo;
                return (((double) formatInfo.Width) + ((double) base.lineFormatRenderer.GetWidth()));
            }
        }
    }
}

