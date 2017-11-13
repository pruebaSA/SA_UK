namespace PdfSharp.Drawing.BarCodes
{
    using PdfSharp.Drawing;
    using System;

    public abstract class ThickThinBarCode : BarCode
    {
        internal double wideNarrowRatio;

        public ThickThinBarCode(string code, XSize size, CodeDirection direction) : base(code, size, direction)
        {
            this.wideNarrowRatio = 2.6;
        }

        internal abstract void CalcThinBarWidth(BarCodeRenderInfo info);
        internal double GetBarWidth(BarCodeRenderInfo info, bool isThick)
        {
            if (isThick)
            {
                return (info.ThinBarWidth * this.wideNarrowRatio);
            }
            return info.ThinBarWidth;
        }

        internal override void InitRendering(BarCodeRenderInfo info)
        {
            base.InitRendering(info);
            this.CalcThinBarWidth(info);
            info.BarHeight = base.Size.Height;
            if (base.textLocation != TextLocation.None)
            {
                info.BarHeight *= 0.8;
            }
            switch (base.direction)
            {
                case CodeDirection.BottomToTop:
                    info.Gfx.RotateAtTransform(-90.0, info.Position);
                    return;

                case CodeDirection.RightToLeft:
                    info.Gfx.RotateAtTransform(180.0, info.Position);
                    return;

                case CodeDirection.TopToBottom:
                    info.Gfx.RotateAtTransform(90.0, info.Position);
                    return;
            }
        }

        internal void RenderBar(BarCodeRenderInfo info, bool isThick)
        {
            double barWidth = this.GetBarWidth(info, isThick);
            double height = base.Size.Height;
            if (base.TextLocation != TextLocation.None)
            {
                height *= 0.8;
            }
            XRect rect = new XRect(info.CurrPos.X, info.CurrPos.Y, barWidth, height);
            info.Gfx.DrawRectangle(info.Brush, rect);
            info.CurrPos.X += barWidth;
        }

        internal void RenderGap(BarCodeRenderInfo info, bool isThick)
        {
            info.CurrPos.X += this.GetBarWidth(info, isThick);
        }

        internal void RenderText(BarCodeRenderInfo info)
        {
            if (info.Font == null)
            {
                info.Font = new XFont("Courier New", base.Size.Height / 6.0);
            }
            XPoint location = info.Position + CodeBase.CalcDistance(base.anchor, AnchorType.TopLeft, base.size);
            info.Gfx.DrawString(base.text, info.Font, info.Brush, new XRect(location, base.Size), XStringFormats.BottomCenter);
        }

        internal void RenderTurboBit(BarCodeRenderInfo info, bool startBit)
        {
            if (startBit)
            {
                info.CurrPos.X -= 0.5 + this.GetBarWidth(info, true);
            }
            else
            {
                info.CurrPos.X += 0.5;
            }
            this.RenderBar(info, true);
            if (startBit)
            {
                info.CurrPos.X += 0.5;
            }
        }

        public override double WideNarrowRatio
        {
            get => 
                this.wideNarrowRatio;
            set
            {
                if ((value > 3.0) || (value < 2.0))
                {
                    throw new ArgumentOutOfRangeException("value", BcgSR.Invalid2of5Relation);
                }
                this.wideNarrowRatio = value;
            }
        }
    }
}

