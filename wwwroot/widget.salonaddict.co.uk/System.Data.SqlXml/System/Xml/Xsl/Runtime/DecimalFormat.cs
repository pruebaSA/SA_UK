namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Globalization;

    internal class DecimalFormat
    {
        public char digit;
        public NumberFormatInfo info;
        public char patternSeparator;
        public char zeroDigit;

        internal DecimalFormat(NumberFormatInfo info, char digit, char zeroDigit, char patternSeparator)
        {
            this.info = info;
            this.digit = digit;
            this.zeroDigit = zeroDigit;
            this.patternSeparator = patternSeparator;
        }
    }
}

