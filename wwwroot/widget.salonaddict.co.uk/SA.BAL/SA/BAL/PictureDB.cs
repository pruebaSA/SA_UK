namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class PictureDB : BaseEntity
    {
        public bool Deleted { get; set; }

        public int Height { get; set; }

        public string MimeType { get; set; }

        public string Name { get; set; }

        public byte[] PictureBinary { get; set; }

        public Guid PictureId { get; set; }

        public string SEName { get; set; }

        public int Width { get; set; }
    }
}

