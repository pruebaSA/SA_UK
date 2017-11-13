namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    internal sealed class PdfImageTable : PdfResourceTable
    {
        private readonly Dictionary<ImageSelector, PdfImage> images;

        public PdfImageTable(PdfDocument document) : base(document)
        {
            this.images = new Dictionary<ImageSelector, PdfImage>();
        }

        public PdfImage GetImage(XImage image)
        {
            PdfImage image2;
            ImageSelector key = image.selector;
            if (key == null)
            {
                key = new ImageSelector(image);
                image.selector = key;
            }
            if (!this.images.TryGetValue(key, out image2))
            {
                image2 = new PdfImage(base.owner, image);
                this.images[key] = image2;
            }
            return image2;
        }

        public class ImageSelector
        {
            private string path;

            public ImageSelector(XImage image)
            {
                if (image.path == null)
                {
                    image.path = Guid.NewGuid().ToString();
                }
                this.path = image.path.ToLower(CultureInfo.InvariantCulture);
            }

            public override bool Equals(object obj)
            {
                PdfImageTable.ImageSelector selector = obj as PdfImageTable.ImageSelector;
                if (obj == null)
                {
                    return false;
                }
                return (this.path == selector.path);
            }

            public override int GetHashCode() => 
                this.path.GetHashCode();

            public string Path
            {
                get => 
                    this.path;
                set
                {
                    this.path = value;
                }
            }
        }
    }
}

