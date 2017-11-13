namespace System.Drawing.Printing
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;

    [Serializable, TypeConverter(typeof(MarginsConverter))]
    public class Margins : ICloneable
    {
        private int bottom;
        private int left;
        private int right;
        private int top;

        public Margins() : this(100, 100, 100, 100)
        {
        }

        public Margins(int left, int right, int top, int bottom)
        {
            this.CheckMargin(left, "left");
            this.CheckMargin(right, "right");
            this.CheckMargin(top, "top");
            this.CheckMargin(bottom, "bottom");
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        private void CheckMargin(int margin, string name)
        {
            if (margin < 0)
            {
                throw new ArgumentException(System.Drawing.SR.GetString("InvalidLowBoundArgumentEx", new object[] { name, margin, "0" }));
            }
        }

        public object Clone() => 
            base.MemberwiseClone();

        public override bool Equals(object obj)
        {
            Margins margins = obj as Margins;
            if (margins == this)
            {
                return true;
            }
            if (margins == null)
            {
                return false;
            }
            return ((((margins.left == this.left) && (margins.right == this.right)) && (margins.top == this.top)) && (margins.bottom == this.bottom));
        }

        public override int GetHashCode()
        {
            uint left = (uint) this.left;
            uint right = (uint) this.right;
            uint top = (uint) this.top;
            uint bottom = (uint) this.bottom;
            uint num5 = ((left ^ ((right << 13) | (right >> 0x13))) ^ ((top << 0x1a) | (top >> 6))) ^ ((bottom << 7) | (bottom >> 0x19));
            return (int) num5;
        }

        public static bool operator ==(Margins m1, Margins m2)
        {
            if (object.ReferenceEquals(m1, null) != object.ReferenceEquals(m2, null))
            {
                return false;
            }
            return (object.ReferenceEquals(m1, null) || ((((m1.Left == m2.Left) && (m1.Top == m2.Top)) && (m1.Right == m2.Right)) && (m1.Bottom == m2.Bottom)));
        }

        public static bool operator !=(Margins m1, Margins m2) => 
            !(m1 == m2);

        public override string ToString() => 
            ("[Margins Left=" + this.Left.ToString(CultureInfo.InvariantCulture) + " Right=" + this.Right.ToString(CultureInfo.InvariantCulture) + " Top=" + this.Top.ToString(CultureInfo.InvariantCulture) + " Bottom=" + this.Bottom.ToString(CultureInfo.InvariantCulture) + "]");

        public int Bottom
        {
            get => 
                this.bottom;
            set
            {
                this.CheckMargin(value, "Bottom");
                this.bottom = value;
            }
        }

        public int Left
        {
            get => 
                this.left;
            set
            {
                this.CheckMargin(value, "Left");
                this.left = value;
            }
        }

        public int Right
        {
            get => 
                this.right;
            set
            {
                this.CheckMargin(value, "Right");
                this.right = value;
            }
        }

        public int Top
        {
            get => 
                this.top;
            set
            {
                this.CheckMargin(value, "Top");
                this.top = value;
            }
        }
    }
}

