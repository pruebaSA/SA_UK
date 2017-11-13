namespace PdfSharp.Drawing.Layout
{
    using PdfSharp.Drawing;
    using System;
    using System.Collections.Generic;

    public class XTextFormatter
    {
        private XParagraphAlignment alignment = XParagraphAlignment.Left;
        private readonly List<Block> blocks = new List<Block>();
        private double cyAscent;
        private double cyDescent;
        private XFont font;
        private XGraphics gfx;
        private XRect layoutRectangle;
        private double lineSpace;
        private double spaceWidth;
        private string text;

        public XTextFormatter(XGraphics gfx)
        {
            if (gfx == null)
            {
                throw new ArgumentNullException("gfx");
            }
            this.gfx = gfx;
        }

        private void AlignLine(int firstIndex, int lastIndex, double layoutWidth)
        {
            XParagraphAlignment alignment = this.blocks[firstIndex].Alignment;
            if ((this.alignment != XParagraphAlignment.Left) && (alignment != XParagraphAlignment.Left))
            {
                int num = (lastIndex - firstIndex) + 1;
                if (num != 0)
                {
                    double num2 = -this.spaceWidth;
                    for (int i = firstIndex; i <= lastIndex; i++)
                    {
                        num2 += this.blocks[i].Width + this.spaceWidth;
                    }
                    double width = Math.Max((double) (layoutWidth - num2), (double) 0.0);
                    if (this.alignment != XParagraphAlignment.Justify)
                    {
                        if (this.alignment == XParagraphAlignment.Center)
                        {
                            width /= 2.0;
                        }
                        for (int j = firstIndex; j <= lastIndex; j++)
                        {
                            Block block = this.blocks[j];
                            block.Location += new XSize(width, 0.0);
                        }
                    }
                    else if (num > 1)
                    {
                        width /= (double) (num - 1);
                        int num6 = firstIndex + 1;
                        for (int k = 1; num6 <= lastIndex; k++)
                        {
                            Block block2 = this.blocks[num6];
                            block2.Location += new XSize(width * k, 0.0);
                            num6++;
                        }
                    }
                }
            }
        }

        private void CreateBlocks()
        {
            this.blocks.Clear();
            int length = this.text.Length;
            bool flag = false;
            int startIndex = 0;
            int num3 = 0;
            for (int i = 0; i < length; i++)
            {
                char c = this.text[i];
                switch (c)
                {
                    case '\r':
                        if ((i < (length - 1)) && (this.text[i + 1] == '\n'))
                        {
                            i++;
                        }
                        c = '\n';
                        break;

                    case '\n':
                    {
                        if (num3 != 0)
                        {
                            string text = this.text.Substring(startIndex, num3);
                            this.blocks.Add(new Block(text, BlockType.Text, this.gfx.MeasureString(text, this.font).Width));
                        }
                        startIndex = i + 1;
                        num3 = 0;
                        this.blocks.Add(new Block(BlockType.LineBreak));
                        continue;
                    }
                }
                if (char.IsWhiteSpace(c))
                {
                    if (flag)
                    {
                        string str2 = this.text.Substring(startIndex, num3);
                        this.blocks.Add(new Block(str2, BlockType.Text, this.gfx.MeasureString(str2, this.font).Width));
                        startIndex = i + 1;
                        num3 = 0;
                    }
                    else
                    {
                        num3++;
                    }
                }
                else
                {
                    flag = true;
                    num3++;
                }
            }
            if (num3 != 0)
            {
                string str3 = this.text.Substring(startIndex, num3);
                this.blocks.Add(new Block(str3, BlockType.Text, this.gfx.MeasureString(str3, this.font).Width));
            }
        }

        private void CreateLayout()
        {
            double width = this.layoutRectangle.width;
            double num2 = (this.layoutRectangle.height - this.cyAscent) - this.cyDescent;
            int firstIndex = 0;
            double x = 0.0;
            double y = 0.0;
            int count = this.blocks.Count;
            for (int i = 0; i < count; i++)
            {
                Block block = this.blocks[i];
                if (block.Type == BlockType.LineBreak)
                {
                    if (this.Alignment == XParagraphAlignment.Justify)
                    {
                        this.blocks[firstIndex].Alignment = XParagraphAlignment.Left;
                    }
                    this.AlignLine(firstIndex, i - 1, width);
                    firstIndex = i + 1;
                    x = 0.0;
                    y += this.lineSpace;
                }
                else
                {
                    double num8 = block.Width;
                    if ((((x + num8) <= width) || (x == 0.0)) && (block.Type != BlockType.LineBreak))
                    {
                        block.Location = new XPoint(x, y);
                        x += num8 + this.spaceWidth;
                    }
                    else
                    {
                        this.AlignLine(firstIndex, i - 1, width);
                        firstIndex = i;
                        y += this.lineSpace;
                        if (y > num2)
                        {
                            block.Stop = true;
                            break;
                        }
                        block.Location = new XPoint(0.0, y);
                        x = num8 + this.spaceWidth;
                    }
                }
            }
            if ((firstIndex < count) && (this.Alignment != XParagraphAlignment.Justify))
            {
                this.AlignLine(firstIndex, count - 1, width);
            }
        }

        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle)
        {
            this.DrawString(text, font, brush, layoutRectangle, XStringFormats.TopLeft);
        }

        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            if ((format.Alignment != XStringAlignment.Near) || (format.LineAlignment != XLineAlignment.Near))
            {
                throw new ArgumentException("Only TopLeft alignment is currently implemented.");
            }
            this.Text = text;
            this.Font = font;
            this.LayoutRectangle = layoutRectangle;
            if (text.Length != 0)
            {
                this.CreateBlocks();
                this.CreateLayout();
                double x = layoutRectangle.Location.x;
                double num2 = layoutRectangle.Location.y + this.cyAscent;
                int count = this.blocks.Count;
                for (int i = 0; i < count; i++)
                {
                    Block block = this.blocks[i];
                    if (block.Stop)
                    {
                        return;
                    }
                    if (block.Type != BlockType.LineBreak)
                    {
                        this.gfx.DrawString(block.Text, font, brush, (double) (x + block.Location.x), (double) (num2 + block.Location.y));
                    }
                }
            }
        }

        public XParagraphAlignment Alignment
        {
            get => 
                this.alignment;
            set
            {
                this.alignment = value;
            }
        }

        public XFont Font
        {
            get => 
                this.font;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("font");
                }
                this.font = value;
                this.lineSpace = this.font.GetHeight(this.gfx);
                this.cyAscent = (this.lineSpace * this.font.cellAscent) / ((double) this.font.cellSpace);
                this.cyDescent = (this.lineSpace * this.font.cellDescent) / ((double) this.font.cellSpace);
                this.spaceWidth = this.gfx.MeasureString("x\x00a0x", value).width;
                this.spaceWidth -= this.gfx.MeasureString("xx", value).width;
            }
        }

        public XRect LayoutRectangle
        {
            get => 
                this.layoutRectangle;
            set
            {
                this.layoutRectangle = value;
            }
        }

        public string Text
        {
            get => 
                this.text;
            set
            {
                this.text = value;
            }
        }

        private class Block
        {
            public XParagraphAlignment Alignment;
            public XPoint Location;
            public bool Stop;
            public string Text;
            public XTextFormatter.BlockType Type;
            public double Width;

            public Block(XTextFormatter.BlockType type)
            {
                this.Type = type;
            }

            public Block(string text, XTextFormatter.BlockType type, double width)
            {
                this.Text = text;
                this.Type = type;
                this.Width = width;
            }
        }

        private enum BlockType
        {
            Text,
            Space,
            Hyphen,
            LineBreak
        }
    }
}

