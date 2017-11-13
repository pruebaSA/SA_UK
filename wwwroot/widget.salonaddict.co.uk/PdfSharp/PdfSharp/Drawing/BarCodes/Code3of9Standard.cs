namespace PdfSharp.Drawing.BarCodes
{
    using PdfSharp.Drawing;
    using System;

    public class Code3of9Standard : ThickThinBarCode
    {
        private static bool[][] Lines = new bool[][] { 
            new bool[] { false, false, false, true, true, false, true, false, false }, new bool[] { true, false, false, true, false, false, false, false, true }, new bool[] { false, false, true, true, false, false, false, false, true }, new bool[] { true, false, true, true, false, false, false, false, false }, new bool[] { false, false, false, true, true, false, false, false, true }, new bool[] { true, false, false, true, true, false, false, false, false }, new bool[] { false, false, true, true, true, false, false, false, false }, new bool[] { false, false, false, true, false, false, true, false, true }, new bool[] { true, false, false, true, false, false, true, false, false }, new bool[] { false, false, true, true, false, false, true, false, false }, new bool[] { true, false, false, false, false, true, false, false, true }, new bool[] { false, false, true, false, false, true, false, false, true }, new bool[] { true, false, true, false, false, true, false, false, false }, new bool[] { false, false, false, false, true, true, false, false, true }, new bool[] { true, false, false, false, true, true, false, false, false }, new bool[] { false, false, true, false, true, true, false, false, false },
            new bool[] { false, false, false, false, false, true, true, false, true }, new bool[] { true, false, false, false, false, true, true, false, false }, new bool[] { false, false, true, false, false, true, true, false, false }, new bool[] { false, false, false, false, true, true, true, false, false }, new bool[] { true, false, false, false, false, false, false, true, true }, new bool[] { false, false, true, false, false, false, false, true, true }, new bool[] { true, false, true, false, false, false, false, true, false }, new bool[] { false, false, false, false, true, false, false, true, true }, new bool[] { true, false, false, false, true, false, false, true, false }, new bool[] { false, false, true, false, true, false, false, true, false }, new bool[] { false, false, false, false, false, false, true, true, true }, new bool[] { true, false, false, false, false, false, true, true, false }, new bool[] { false, false, true, false, false, false, true, true, false }, new bool[] { false, false, false, false, true, false, true, true, false }, new bool[] { true, true, false, false, false, false, false, false, true }, new bool[] { false, true, true, false, false, false, false, false, true },
            new bool[] { true, true, true, false, false, false, false, false, false }, new bool[] { false, true, false, false, true, false, false, false, true }, new bool[] { true, true, false, false, true, false, false, false, false }, new bool[] { false, true, true, false, true, false, false, false, false }, new bool[] { false, true, false, false, false, false, true, false, true }, new bool[] { true, true, false, false, false, false, true, false, false }, new bool[] { false, true, true, false, false, false, true, false, false }, new bool[] { false, true, false, true, false, true, false, false, false }, new bool[] { false, true, false, true, false, false, false, true, false }, new bool[] { false, true, false, false, false, true, false, true, false }, new bool[] { false, false, false, true, false, true, false, true, false }, new bool[] { false, true, false, false, true, false, true, false, false }
        };

        public Code3of9Standard() : base("", XSize.Empty, CodeDirection.LeftToRight)
        {
        }

        public Code3of9Standard(string code) : base(code, XSize.Empty, CodeDirection.LeftToRight)
        {
        }

        public Code3of9Standard(string code, XSize size) : base(code, size, CodeDirection.LeftToRight)
        {
        }

        public Code3of9Standard(string code, XSize size, CodeDirection direction) : base(code, size, direction)
        {
        }

        internal override void CalcThinBarWidth(BarCodeRenderInfo info)
        {
            double num = (13.0 + (6.0 * base.wideNarrowRatio)) + (((3.0 * base.wideNarrowRatio) + 7.0) * base.text.Length);
            info.ThinBarWidth = base.Size.Width / num;
        }

        protected override void CheckCode(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (text.Length == 0)
            {
                throw new ArgumentException(BcgSR.Invalid3Of9Code(text));
            }
            foreach (char ch in text)
            {
                if ("0123456789ABCDEFGHIJKLMNOP'QRSTUVWXYZ-. $/+%*".IndexOf(ch) < 0)
                {
                    throw new ArgumentException(BcgSR.Invalid3Of9Code(text));
                }
            }
        }

        protected internal override void Render(XGraphics gfx, XBrush brush, XFont font, XPoint position)
        {
            XGraphicsState state = gfx.Save();
            BarCodeRenderInfo info = new BarCodeRenderInfo(gfx, brush, font, position);
            this.InitRendering(info);
            info.CurrPosInString = 0;
            info.CurrPos = position - CodeBase.CalcDistance(AnchorType.TopLeft, base.anchor, base.size);
            if (this.TurboBit)
            {
                base.RenderTurboBit(info, true);
            }
            this.RenderStart(info);
            while (info.CurrPosInString < base.text.Length)
            {
                this.RenderNextChar(info);
                base.RenderGap(info, false);
            }
            this.RenderStop(info);
            if (this.TurboBit)
            {
                base.RenderTurboBit(info, false);
            }
            if (base.TextLocation != TextLocation.None)
            {
                base.RenderText(info);
            }
            gfx.Restore(state);
        }

        private void RenderChar(BarCodeRenderInfo info, char ch)
        {
            bool[] flagArray = ThickThinLines(ch);
            for (int i = 0; i < 9; i += 2)
            {
                base.RenderBar(info, flagArray[i]);
                if (i < 8)
                {
                    base.RenderGap(info, flagArray[i + 1]);
                }
            }
        }

        private void RenderNextChar(BarCodeRenderInfo info)
        {
            this.RenderChar(info, base.text[info.CurrPosInString]);
            info.CurrPosInString++;
        }

        private void RenderStart(BarCodeRenderInfo info)
        {
            this.RenderChar(info, '*');
            base.RenderGap(info, false);
        }

        private void RenderStop(BarCodeRenderInfo info)
        {
            this.RenderChar(info, '*');
        }

        private static bool[] ThickThinLines(char ch) => 
            Lines["0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*".IndexOf(ch)];
    }
}

