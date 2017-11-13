namespace PdfSharp.Pdf
{
    using PdfSharp.Drawing;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("(Left={left.Millimeter}mm, Right={right.Millimeter}mm, Top={top.Millimeter}mm, Bottom={bottom.Millimeter}mm)")]
    public sealed class TrimMargins
    {
        private XUnit bottom;
        private XUnit left;
        private XUnit right;
        private XUnit top;

        public XUnit All
        {
            set
            {
                this.left = value;
                this.right = value;
                this.top = value;
                this.bottom = value;
            }
        }

        public bool AreSet
        {
            get
            {
                if (((this.left.Value == 0.0) && (this.right.Value == 0.0)) && (this.top.Value == 0.0))
                {
                    return (this.bottom.Value != 0.0);
                }
                return true;
            }
        }

        public XUnit Bottom
        {
            get => 
                this.bottom;
            set
            {
                this.bottom = value;
            }
        }

        public XUnit Left
        {
            get => 
                this.left;
            set
            {
                this.left = value;
            }
        }

        public XUnit Right
        {
            get => 
                this.right;
            set
            {
                this.right = value;
            }
        }

        public XUnit Top
        {
            get => 
                this.top;
            set
            {
                this.top = value;
            }
        }
    }
}

