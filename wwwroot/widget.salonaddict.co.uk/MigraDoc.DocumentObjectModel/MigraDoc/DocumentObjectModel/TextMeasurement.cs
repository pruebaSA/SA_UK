namespace MigraDoc.DocumentObjectModel
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    public sealed class TextMeasurement
    {
        private MigraDoc.DocumentObjectModel.Font font;
        private System.Drawing.Font gdiFont;
        private Graphics graphics;

        public TextMeasurement(MigraDoc.DocumentObjectModel.Font font)
        {
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }
            this.font = font;
        }

        public SizeF MeasureString(string text) => 
            this.MeasureString(text, UnitType.Point);

        public SizeF MeasureString(string text, UnitType unitType)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (!Enum.IsDefined(typeof(UnitType), unitType))
            {
                throw new InvalidEnumArgumentException();
            }
            SizeF ef = this.Realize().MeasureString(text, this.gdiFont, new PointF(0f, 0f), StringFormat.GenericTypographic);
            switch (unitType)
            {
                case UnitType.Point:
                    return ef;

                case UnitType.Centimeter:
                    ef.Width = (float) ((ef.Width * 2.54) / 72.0);
                    ef.Height = (float) ((ef.Height * 2.54) / 72.0);
                    return ef;

                case UnitType.Inch:
                    ef.Width /= 72f;
                    ef.Height /= 72f;
                    return ef;

                case UnitType.Millimeter:
                    ef.Width = (float) ((ef.Width * 25.4) / 72.0);
                    ef.Height = (float) ((ef.Height * 25.4) / 72.0);
                    return ef;

                case UnitType.Pica:
                    ef.Width /= 12f;
                    ef.Height /= 12f;
                    return ef;
            }
            return ef;
        }

        private Graphics Realize()
        {
            if (this.graphics == null)
            {
                this.graphics = Graphics.FromHwnd(IntPtr.Zero);
            }
            this.graphics.PageUnit = GraphicsUnit.Point;
            if (this.gdiFont == null)
            {
                FontStyle regular = FontStyle.Regular;
                if (this.font.Bold)
                {
                    regular |= FontStyle.Bold;
                }
                if (this.font.Italic)
                {
                    regular |= FontStyle.Italic;
                }
                this.gdiFont = new System.Drawing.Font(this.font.Name, (float) this.font.Size, regular, GraphicsUnit.Point);
            }
            return this.graphics;
        }

        public MigraDoc.DocumentObjectModel.Font Font
        {
            get => 
                this.font;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (this.font != value)
                {
                    this.font = value;
                    this.gdiFont = null;
                }
            }
        }
    }
}

