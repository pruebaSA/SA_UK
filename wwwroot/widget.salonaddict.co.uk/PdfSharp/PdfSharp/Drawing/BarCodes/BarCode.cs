namespace PdfSharp.Drawing.BarCodes
{
    using PdfSharp.Drawing;
    using System;
    using System.ComponentModel;

    public abstract class BarCode : CodeBase
    {
        internal int dataLength;
        internal char endChar;
        internal char startChar;
        internal PdfSharp.Drawing.BarCodes.TextLocation textLocation;
        internal bool turboBit;

        public BarCode(string text, XSize size, CodeDirection direction) : base(text, size, direction)
        {
            base.text = text;
            base.size = size;
            base.direction = direction;
        }

        public static BarCode FromType(CodeType type) => 
            FromType(type, string.Empty, XSize.Empty, CodeDirection.LeftToRight);

        public static BarCode FromType(CodeType type, string text) => 
            FromType(type, text, XSize.Empty, CodeDirection.LeftToRight);

        public static BarCode FromType(CodeType type, string text, XSize size) => 
            FromType(type, text, size, CodeDirection.LeftToRight);

        public static BarCode FromType(CodeType type, string text, XSize size, CodeDirection direction)
        {
            switch (type)
            {
                case CodeType.Code2of5Interleaved:
                    return new Code2of5Interleaved(text, size, direction);

                case CodeType.Code3of9Standard:
                    return new Code3of9Standard(text, size, direction);
            }
            throw new InvalidEnumArgumentException("type", (int) type, typeof(CodeType));
        }

        internal virtual void InitRendering(BarCodeRenderInfo info)
        {
            if (base.text == null)
            {
                throw new InvalidOperationException(BcgSR.BarCodeNotSet);
            }
            if (base.Size.IsEmpty)
            {
                throw new InvalidOperationException(BcgSR.EmptyBarCodeSize);
            }
        }

        protected internal abstract void Render(XGraphics gfx, XBrush brush, XFont font, XPoint position);

        public int DataLength
        {
            get => 
                this.dataLength;
            set
            {
                this.dataLength = value;
            }
        }

        public char EndChar
        {
            get => 
                this.endChar;
            set
            {
                this.endChar = value;
            }
        }

        public char StartChar
        {
            get => 
                this.startChar;
            set
            {
                this.startChar = value;
            }
        }

        public PdfSharp.Drawing.BarCodes.TextLocation TextLocation
        {
            get => 
                this.textLocation;
            set
            {
                this.textLocation = value;
            }
        }

        public virtual bool TurboBit
        {
            get => 
                this.turboBit;
            set
            {
                this.turboBit = value;
            }
        }

        public virtual double WideNarrowRatio
        {
            get => 
                0.0;
            set
            {
            }
        }
    }
}

