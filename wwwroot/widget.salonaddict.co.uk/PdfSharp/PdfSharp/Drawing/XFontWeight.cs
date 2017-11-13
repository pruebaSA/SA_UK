namespace PdfSharp.Drawing
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("'{Weight}'")]
    public class XFontWeight : IFormattable
    {
        private int weight;

        internal XFontWeight(int weight)
        {
            this.weight = weight;
        }

        public static int Compare(XFontWeight left, XFontWeight right) => 
            (left.weight - right.weight);

        private string ConvertToString(string format, IFormatProvider provider)
        {
            string str;
            if (!XFontWeights.FontWeightToString(this.Weight, out str))
            {
                return this.Weight.ToString(provider);
            }
            return str;
        }

        public bool Equals(XFontWeight obj) => 
            (this == obj);

        public override bool Equals(object obj) => 
            ((obj is XFontWeight) && (this == ((XFontWeight) obj)));

        public override int GetHashCode() => 
            this.Weight;

        public static bool operator ==(XFontWeight left, XFontWeight right) => 
            (Compare(left, right) == 0);

        public static bool operator >(XFontWeight left, XFontWeight right) => 
            (Compare(left, right) > 0);

        public static bool operator >=(XFontWeight left, XFontWeight right) => 
            (Compare(left, right) >= 0);

        public static bool operator !=(XFontWeight left, XFontWeight right) => 
            !(left == right);

        public static bool operator <(XFontWeight left, XFontWeight right) => 
            (Compare(left, right) < 0);

        public static bool operator <=(XFontWeight left, XFontWeight right) => 
            (Compare(left, right) <= 0);

        string IFormattable.ToString(string format, IFormatProvider provider) => 
            this.ConvertToString(format, provider);

        public override string ToString() => 
            this.ConvertToString(null, null);

        public int Weight =>
            this.weight;
    }
}

