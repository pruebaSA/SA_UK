namespace MigraDoc.DocumentObjectModel.Shapes
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.IO;

    public class Image : Shape
    {
        [DV]
        internal NBool lockAspectRatio;
        private static MigraDoc.DocumentObjectModel.Internals.Meta meta;
        [DV]
        internal NString name;
        [DV]
        internal MigraDoc.DocumentObjectModel.Shapes.PictureFormat pictureFormat;
        [DV]
        internal NDouble resolution;
        [DV]
        internal NDouble scaleHeight;
        [DV]
        internal NDouble scaleWidth;

        public Image()
        {
            this.name = NString.NullValue;
            this.scaleWidth = NDouble.NullValue;
            this.scaleHeight = NDouble.NullValue;
            this.lockAspectRatio = NBool.NullValue;
            this.resolution = NDouble.NullValue;
        }

        internal Image(DocumentObject parent) : base(parent)
        {
            this.name = NString.NullValue;
            this.scaleWidth = NDouble.NullValue;
            this.scaleHeight = NDouble.NullValue;
            this.lockAspectRatio = NBool.NullValue;
            this.resolution = NDouble.NullValue;
        }

        public Image(string name) : this()
        {
            this.Name = name;
        }

        public Image Clone() => 
            ((Image) this.DeepCopy());

        protected override object DeepCopy()
        {
            Image image = (Image) base.DeepCopy();
            if (image.pictureFormat != null)
            {
                image.pictureFormat = image.pictureFormat.Clone();
                image.pictureFormat.parent = image;
            }
            return image;
        }

        public string GetFilePath(string workingDir)
        {
            string root = "";
            try
            {
                if (!string.IsNullOrEmpty(workingDir))
                {
                    root = workingDir;
                }
                else
                {
                    root = Directory.GetCurrentDirectory() + @"\";
                }
                if (!base.Document.IsNull("ImagePath"))
                {
                    string str2 = ImageHelper.GetImageName(root, this.Name, base.Document.ImagePath);
                    if (str2 != null)
                    {
                        return str2;
                    }
                    return Path.Combine(root, this.Name);
                }
                root = Path.Combine(root, this.Name);
            }
            catch (Exception)
            {
                return null;
            }
            return root;
        }

        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine("\\image(\"" + this.name.Value.Replace(@"\", @"\\").Replace("\"", "\\\"") + "\")");
            int pos = serializer.BeginAttributes();
            base.Serialize(serializer);
            if (!this.scaleWidth.IsNull)
            {
                serializer.WriteSimpleAttribute("ScaleWidth", this.ScaleWidth);
            }
            if (!this.scaleHeight.IsNull)
            {
                serializer.WriteSimpleAttribute("ScaleHeight", this.ScaleHeight);
            }
            if (!this.lockAspectRatio.IsNull)
            {
                serializer.WriteSimpleAttribute("LockAspectRatio", this.LockAspectRatio);
            }
            if (!this.resolution.IsNull)
            {
                serializer.WriteSimpleAttribute("Resolution", this.Resolution);
            }
            if (!this.IsNull("PictureFormat"))
            {
                this.pictureFormat.Serialize(serializer);
            }
            serializer.EndAttributes(pos);
        }

        public bool LockAspectRatio
        {
            get => 
                this.lockAspectRatio.Value;
            set
            {
                this.lockAspectRatio.Value = value;
            }
        }

        internal override MigraDoc.DocumentObjectModel.Internals.Meta Meta
        {
            get
            {
                if (meta == null)
                {
                    meta = new MigraDoc.DocumentObjectModel.Internals.Meta(typeof(Image));
                }
                return meta;
            }
        }

        public string Name
        {
            get => 
                this.name.Value;
            set
            {
                this.name.Value = value;
            }
        }

        public MigraDoc.DocumentObjectModel.Shapes.PictureFormat PictureFormat
        {
            get
            {
                if (this.pictureFormat == null)
                {
                    this.pictureFormat = new MigraDoc.DocumentObjectModel.Shapes.PictureFormat(this);
                }
                return this.pictureFormat;
            }
            set
            {
                base.SetParent(value);
                this.pictureFormat = value;
            }
        }

        public double Resolution
        {
            get => 
                this.resolution.Value;
            set
            {
                this.resolution.Value = value;
            }
        }

        public double ScaleHeight
        {
            get => 
                this.scaleHeight.Value;
            set
            {
                this.scaleHeight.Value = value;
            }
        }

        public double ScaleWidth
        {
            get => 
                this.scaleWidth.Value;
            set
            {
                this.scaleWidth.Value = value;
            }
        }
    }
}

