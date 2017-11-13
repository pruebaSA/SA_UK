namespace PdfSharp.Drawing.BarCodes
{
    using PdfSharp.Drawing;
    using System;
    using System.Runtime.InteropServices;

    public abstract class CodeBase
    {
        internal AnchorType anchor;
        private static Delta[,] deltas;
        internal CodeDirection direction;
        internal XSize size;
        internal string text;

        static CodeBase()
        {
            Delta[,] deltaArray = new Delta[9, 9];
            *(deltaArray[0, 0]) = new Delta(0, 0);
            *(deltaArray[0, 1]) = new Delta(1, 0);
            *(deltaArray[0, 2]) = new Delta(2, 0);
            *(deltaArray[0, 3]) = new Delta(0, 1);
            *(deltaArray[0, 4]) = new Delta(1, 1);
            *(deltaArray[0, 5]) = new Delta(2, 1);
            *(deltaArray[0, 6]) = new Delta(0, 2);
            *(deltaArray[0, 7]) = new Delta(1, 2);
            *(deltaArray[0, 8]) = new Delta(2, 2);
            *(deltaArray[1, 0]) = new Delta(-1, 0);
            *(deltaArray[1, 1]) = new Delta(0, 0);
            *(deltaArray[1, 2]) = new Delta(1, 0);
            *(deltaArray[1, 3]) = new Delta(-1, 1);
            *(deltaArray[1, 4]) = new Delta(0, 1);
            *(deltaArray[1, 5]) = new Delta(1, 1);
            *(deltaArray[1, 6]) = new Delta(-1, 2);
            *(deltaArray[1, 7]) = new Delta(0, 2);
            *(deltaArray[1, 8]) = new Delta(1, 2);
            *(deltaArray[2, 0]) = new Delta(-2, 0);
            *(deltaArray[2, 1]) = new Delta(-1, 0);
            *(deltaArray[2, 2]) = new Delta(0, 0);
            *(deltaArray[2, 3]) = new Delta(-2, 1);
            *(deltaArray[2, 4]) = new Delta(-1, 1);
            *(deltaArray[2, 5]) = new Delta(0, 1);
            *(deltaArray[2, 6]) = new Delta(-2, 2);
            *(deltaArray[2, 7]) = new Delta(-1, 2);
            *(deltaArray[2, 8]) = new Delta(0, 2);
            *(deltaArray[3, 0]) = new Delta(0, -1);
            *(deltaArray[3, 1]) = new Delta(1, -1);
            *(deltaArray[3, 2]) = new Delta(2, -1);
            *(deltaArray[3, 3]) = new Delta(0, 0);
            *(deltaArray[3, 4]) = new Delta(1, 0);
            *(deltaArray[3, 5]) = new Delta(2, 0);
            *(deltaArray[3, 6]) = new Delta(0, 1);
            *(deltaArray[3, 7]) = new Delta(1, 1);
            *(deltaArray[3, 8]) = new Delta(2, 1);
            *(deltaArray[4, 0]) = new Delta(-1, -1);
            *(deltaArray[4, 1]) = new Delta(0, -1);
            *(deltaArray[4, 2]) = new Delta(1, -1);
            *(deltaArray[4, 3]) = new Delta(-1, 0);
            *(deltaArray[4, 4]) = new Delta(0, 0);
            *(deltaArray[4, 5]) = new Delta(1, 0);
            *(deltaArray[4, 6]) = new Delta(-1, 1);
            *(deltaArray[4, 7]) = new Delta(0, 1);
            *(deltaArray[4, 8]) = new Delta(1, 1);
            *(deltaArray[5, 0]) = new Delta(-2, -1);
            *(deltaArray[5, 1]) = new Delta(-1, -1);
            *(deltaArray[5, 2]) = new Delta(0, -1);
            *(deltaArray[5, 3]) = new Delta(-2, 0);
            *(deltaArray[5, 4]) = new Delta(-1, 0);
            *(deltaArray[5, 5]) = new Delta(0, 0);
            *(deltaArray[5, 6]) = new Delta(-2, 1);
            *(deltaArray[5, 7]) = new Delta(-1, 1);
            *(deltaArray[5, 8]) = new Delta(0, 1);
            *(deltaArray[6, 0]) = new Delta(0, -2);
            *(deltaArray[6, 1]) = new Delta(1, -2);
            *(deltaArray[6, 2]) = new Delta(2, -2);
            *(deltaArray[6, 3]) = new Delta(0, -1);
            *(deltaArray[6, 4]) = new Delta(1, -1);
            *(deltaArray[6, 5]) = new Delta(2, -1);
            *(deltaArray[6, 6]) = new Delta(0, 0);
            *(deltaArray[6, 7]) = new Delta(1, 0);
            *(deltaArray[6, 8]) = new Delta(2, 0);
            *(deltaArray[7, 0]) = new Delta(-1, -2);
            *(deltaArray[7, 1]) = new Delta(0, -2);
            *(deltaArray[7, 2]) = new Delta(1, -2);
            *(deltaArray[7, 3]) = new Delta(-1, -1);
            *(deltaArray[7, 4]) = new Delta(0, -1);
            *(deltaArray[7, 5]) = new Delta(1, -1);
            *(deltaArray[7, 6]) = new Delta(-1, 0);
            *(deltaArray[7, 7]) = new Delta(0, 0);
            *(deltaArray[7, 8]) = new Delta(1, 0);
            *(deltaArray[8, 0]) = new Delta(-2, -2);
            *(deltaArray[8, 1]) = new Delta(-1, -2);
            *(deltaArray[8, 2]) = new Delta(0, -2);
            *(deltaArray[8, 3]) = new Delta(-2, -1);
            *(deltaArray[8, 4]) = new Delta(-1, -1);
            *(deltaArray[8, 5]) = new Delta(0, -1);
            *(deltaArray[8, 6]) = new Delta(-2, 0);
            *(deltaArray[8, 7]) = new Delta(-1, 0);
            *(deltaArray[8, 8]) = new Delta(0, 0);
            deltas = deltaArray;
        }

        public CodeBase(string text, XSize size, CodeDirection direction)
        {
            this.text = text;
            this.size = size;
            this.direction = direction;
        }

        public static XVector CalcDistance(AnchorType oldType, AnchorType newType, XSize size)
        {
            if (oldType == newType)
            {
                return new XVector();
            }
            Delta delta = *(deltas[(int) oldType, (int) newType]);
            return new XVector((size.width / 2.0) * delta.x, (size.height / 2.0) * delta.y);
        }

        protected abstract void CheckCode(string text);

        public AnchorType Anchor
        {
            get => 
                this.anchor;
            set
            {
                this.anchor = value;
            }
        }

        public CodeDirection Direction
        {
            get => 
                this.direction;
            set
            {
                this.direction = value;
            }
        }

        public XSize Size
        {
            get => 
                this.size;
            set
            {
                this.size = value;
            }
        }

        public string Text
        {
            get => 
                this.text;
            set
            {
                this.CheckCode(value);
                this.text = value;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Delta
        {
            public int x;
            public int y;
            public Delta(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }
}

