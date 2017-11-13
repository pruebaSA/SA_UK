namespace System.Xml.Xsl.Xslt
{
    using System;

    internal class Number : XslNode
    {
        public readonly string Count;
        public readonly string Format;
        public readonly string From;
        public readonly string GroupingSeparator;
        public readonly string GroupingSize;
        public readonly string Lang;
        public readonly string LetterValue;
        public readonly NumberLevel Level;
        public readonly string Value;

        public Number(NumberLevel level, string count, string from, string value, string format, string lang, string letterValue, string groupingSeparator, string groupingSize, XslVersion xslVer) : base(XslNodeType.Number, null, null, xslVer)
        {
            this.Level = level;
            this.Count = count;
            this.From = from;
            this.Value = value;
            this.Format = format;
            this.Lang = lang;
            this.LetterValue = letterValue;
            this.GroupingSeparator = groupingSeparator;
            this.GroupingSize = groupingSize;
        }
    }
}

