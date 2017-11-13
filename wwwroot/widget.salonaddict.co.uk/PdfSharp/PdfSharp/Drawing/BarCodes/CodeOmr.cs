namespace PdfSharp.Drawing.BarCodes
{
    using PdfSharp.Drawing;
    using System;

    public class CodeOmr : BarCode
    {
        private double makerDistance;
        private double makerThickness;
        private bool synchronizeCode;

        public CodeOmr(string text, XSize size, CodeDirection direction) : base(text, size, direction)
        {
            this.makerDistance = 12.0;
            this.makerThickness = 1.0;
        }

        protected override void CheckCode(string text)
        {
        }

        protected internal override void Render(XGraphics gfx, XBrush brush, XFont font, XPoint position)
        {
            uint num;
            XGraphicsState state = gfx.Save();
            switch (base.direction)
            {
                case CodeDirection.BottomToTop:
                    gfx.RotateAtTransform(-90.0, position);
                    break;

                case CodeDirection.RightToLeft:
                    gfx.RotateAtTransform(180.0, position);
                    break;

                case CodeDirection.TopToBottom:
                    gfx.RotateAtTransform(90.0, position);
                    break;
            }
            XPoint point = position - CodeBase.CalcDistance(AnchorType.TopLeft, base.anchor, base.size);
            uint.TryParse(base.text, out num);
            num |= 1;
            this.synchronizeCode = true;
            if (this.synchronizeCode)
            {
                XRect rect = new XRect(point.x, point.y, this.makerThickness, this.size.height);
                gfx.DrawRectangle(brush, rect);
                point.x += 2.0 * this.makerDistance;
            }
            for (int i = 0; i < 0x20; i++)
            {
                if ((num & 1) == 1)
                {
                    XRect rect2 = new XRect(point.x + (i * this.makerDistance), point.y, this.makerThickness, this.size.height);
                    gfx.DrawRectangle(brush, rect2);
                }
                num = num >> 1;
            }
            gfx.Restore(state);
        }

        public double MakerDistance
        {
            get => 
                this.makerDistance;
            set
            {
                this.makerDistance = value;
            }
        }

        public double MakerThickness
        {
            get => 
                this.makerThickness;
            set
            {
                this.makerThickness = value;
            }
        }

        public bool SynchronizeCode
        {
            get => 
                this.synchronizeCode;
            set
            {
                this.synchronizeCode = value;
            }
        }
    }
}

