namespace PdfSharp.Drawing.BarCodes
{
    using PdfSharp.Drawing;
    using System;

    public class Code2of5Interleaved : ThickThinBarCode
    {
        private static bool[][] Lines;

        static Code2of5Interleaved()
        {
            bool[][] flagArray = new bool[10][];
            bool[] flagArray2 = new bool[5];
            flagArray2[2] = true;
            flagArray2[3] = true;
            flagArray[0] = flagArray2;
            bool[] flagArray3 = new bool[5];
            flagArray3[0] = true;
            flagArray3[4] = true;
            flagArray[1] = flagArray3;
            bool[] flagArray4 = new bool[5];
            flagArray4[1] = true;
            flagArray4[4] = true;
            flagArray[2] = flagArray4;
            bool[] flagArray5 = new bool[5];
            flagArray5[0] = true;
            flagArray5[1] = true;
            flagArray[3] = flagArray5;
            bool[] flagArray6 = new bool[5];
            flagArray6[2] = true;
            flagArray6[4] = true;
            flagArray[4] = flagArray6;
            bool[] flagArray7 = new bool[5];
            flagArray7[0] = true;
            flagArray7[2] = true;
            flagArray[5] = flagArray7;
            bool[] flagArray8 = new bool[5];
            flagArray8[1] = true;
            flagArray8[2] = true;
            flagArray[6] = flagArray8;
            bool[] flagArray9 = new bool[5];
            flagArray9[3] = true;
            flagArray9[4] = true;
            flagArray[7] = flagArray9;
            bool[] flagArray10 = new bool[5];
            flagArray10[0] = true;
            flagArray10[3] = true;
            flagArray[8] = flagArray10;
            bool[] flagArray11 = new bool[5];
            flagArray11[1] = true;
            flagArray11[3] = true;
            flagArray[9] = flagArray11;
            Lines = flagArray;
        }

        public Code2of5Interleaved() : base("", XSize.Empty, CodeDirection.LeftToRight)
        {
        }

        public Code2of5Interleaved(string code) : base(code, XSize.Empty, CodeDirection.LeftToRight)
        {
        }

        public Code2of5Interleaved(string code, XSize size) : base(code, size, CodeDirection.LeftToRight)
        {
        }

        public Code2of5Interleaved(string code, XSize size, CodeDirection direction) : base(code, size, direction)
        {
        }

        internal override void CalcThinBarWidth(BarCodeRenderInfo info)
        {
            double num = (6.0 + base.wideNarrowRatio) + (((2.0 * base.wideNarrowRatio) + 3.0) * base.text.Length);
            info.ThinBarWidth = base.Size.Width / num;
        }

        protected override void CheckCode(string text)
        {
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
                this.RenderNextPair(info);
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

        private void RenderNextPair(BarCodeRenderInfo info)
        {
            int index = int.Parse(base.text[info.CurrPosInString].ToString());
            int num2 = int.Parse(base.text[info.CurrPosInString + 1].ToString());
            bool[] flagArray = Lines[index];
            bool[] flagArray2 = Lines[num2];
            for (int i = 0; i < 5; i++)
            {
                base.RenderBar(info, flagArray[i]);
                base.RenderGap(info, flagArray2[i]);
            }
            info.CurrPosInString += 2;
        }

        private void RenderStart(BarCodeRenderInfo info)
        {
            base.RenderBar(info, false);
            base.RenderGap(info, false);
            base.RenderBar(info, false);
            base.RenderGap(info, false);
        }

        private void RenderStop(BarCodeRenderInfo info)
        {
            base.RenderBar(info, true);
            base.RenderGap(info, false);
            base.RenderBar(info, false);
        }

        private static bool[] ThickAndThinLines(int digit) => 
            Lines[digit];
    }
}

