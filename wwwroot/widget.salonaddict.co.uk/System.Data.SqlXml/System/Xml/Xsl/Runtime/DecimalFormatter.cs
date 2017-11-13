namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Xml.Xsl;

    internal class DecimalFormatter
    {
        private const string ClrSpecialChars = "0#.,%‰Ee\\'\";";
        private const char EscChar = '\a';
        private string negFormat;
        private NumberFormatInfo negFormatInfo;
        private string posFormat;
        private NumberFormatInfo posFormatInfo;
        private char zeroDigit;

        public DecimalFormatter(string formatPicture, DecimalFormat decimalFormat)
        {
            if (formatPicture.Length == 0)
            {
                throw XsltException.Create("Xslt_InvalidFormat", new string[0]);
            }
            this.zeroDigit = decimalFormat.zeroDigit;
            this.posFormatInfo = (NumberFormatInfo) decimalFormat.info.Clone();
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            char ch = this.posFormatInfo.NumberDecimalSeparator[0];
            char ch2 = this.posFormatInfo.NumberGroupSeparator[0];
            char ch3 = this.posFormatInfo.PercentSymbol[0];
            char ch4 = this.posFormatInfo.PerMilleSymbol[0];
            int commaIndex = 0;
            int num2 = 0;
            int decimalIndex = -1;
            int length = -1;
            for (int i = 0; i < formatPicture.Length; i++)
            {
                char ch5 = formatPicture[i];
                if (ch5 == decimalFormat.digit)
                {
                    if (flag3 && flag)
                    {
                        throw XsltException.Create("Xslt_InvalidFormat1", new string[] { formatPicture });
                    }
                    length = builder.Length;
                    flag4 = flag6 = true;
                    builder.Append('#');
                }
                else if (ch5 == decimalFormat.zeroDigit)
                {
                    if (flag4 && !flag)
                    {
                        throw XsltException.Create("Xslt_InvalidFormat2", new string[] { formatPicture });
                    }
                    length = builder.Length;
                    flag3 = flag6 = true;
                    builder.Append('0');
                }
                else if (ch5 == decimalFormat.patternSeparator)
                {
                    if (!flag6)
                    {
                        throw XsltException.Create("Xslt_InvalidFormat8", new string[0]);
                    }
                    if (flag2)
                    {
                        throw XsltException.Create("Xslt_InvalidFormat3", new string[] { formatPicture });
                    }
                    flag2 = true;
                    if (decimalIndex < 0)
                    {
                        decimalIndex = length + 1;
                    }
                    num2 = RemoveTrailingComma(builder, commaIndex, decimalIndex);
                    if (num2 > 9)
                    {
                        num2 = 0;
                    }
                    this.posFormatInfo.NumberGroupSizes = new int[] { num2 };
                    if (!flag5)
                    {
                        this.posFormatInfo.NumberDecimalDigits = 0;
                    }
                    this.posFormat = builder.ToString();
                    builder.Length = 0;
                    decimalIndex = -1;
                    length = -1;
                    commaIndex = 0;
                    flag4 = flag3 = flag6 = false;
                    flag5 = false;
                    flag = true;
                    this.negFormatInfo = (NumberFormatInfo) decimalFormat.info.Clone();
                    this.negFormatInfo.NegativeSign = string.Empty;
                }
                else if (ch5 == ch)
                {
                    if (flag5)
                    {
                        throw XsltException.Create("Xslt_InvalidFormat5", new string[] { formatPicture });
                    }
                    decimalIndex = builder.Length;
                    flag5 = true;
                    flag4 = flag3 = flag = false;
                    builder.Append('.');
                }
                else if (ch5 == ch2)
                {
                    commaIndex = builder.Length;
                    length = commaIndex;
                    builder.Append(',');
                }
                else if (ch5 == ch3)
                {
                    builder.Append('%');
                }
                else if (ch5 == ch4)
                {
                    builder.Append('‰');
                }
                else if (ch5 == '\'')
                {
                    int index = formatPicture.IndexOf('\'', i + 1);
                    if (index < 0)
                    {
                        index = formatPicture.Length - 1;
                    }
                    builder.Append(formatPicture, i, (index - i) + 1);
                    i = index;
                }
                else
                {
                    if (((('0' <= ch5) && (ch5 <= '9')) || (ch5 == '\a')) && (decimalFormat.zeroDigit != '0'))
                    {
                        builder.Append('\a');
                    }
                    if ("0#.,%‰Ee\\'\";".IndexOf(ch5) >= 0)
                    {
                        builder.Append('\\');
                    }
                    builder.Append(ch5);
                }
            }
            if (!flag6)
            {
                throw XsltException.Create("Xslt_InvalidFormat8", new string[0]);
            }
            NumberFormatInfo info = flag2 ? this.negFormatInfo : this.posFormatInfo;
            if (decimalIndex < 0)
            {
                decimalIndex = length + 1;
            }
            num2 = RemoveTrailingComma(builder, commaIndex, decimalIndex);
            if (num2 > 9)
            {
                num2 = 0;
            }
            info.NumberGroupSizes = new int[] { num2 };
            if (!flag5)
            {
                info.NumberDecimalDigits = 0;
            }
            if (flag2)
            {
                this.negFormat = builder.ToString();
            }
            else
            {
                this.posFormat = builder.ToString();
            }
        }

        public string Format(double value)
        {
            NumberFormatInfo negFormatInfo;
            string negFormat;
            if ((value < 0.0) && (this.negFormatInfo != null))
            {
                negFormatInfo = this.negFormatInfo;
                negFormat = this.negFormat;
            }
            else
            {
                negFormatInfo = this.posFormatInfo;
                negFormat = this.posFormat;
            }
            string str2 = value.ToString(negFormat, negFormatInfo);
            if (this.zeroDigit == '0')
            {
                return str2;
            }
            StringBuilder builder = new StringBuilder(str2.Length);
            int num = this.zeroDigit - '0';
            for (int i = 0; i < str2.Length; i++)
            {
                char ch = str2[i];
                if ((ch - '0') <= 9)
                {
                    ch = (char) (ch + ((char) num));
                }
                else if (ch == '\a')
                {
                    ch = str2[++i];
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static string Format(double value, string formatPicture, DecimalFormat decimalFormat) => 
            new DecimalFormatter(formatPicture, decimalFormat).Format(value);

        private static int RemoveTrailingComma(StringBuilder builder, int commaIndex, int decimalIndex)
        {
            if ((commaIndex > 0) && (commaIndex == (decimalIndex - 1)))
            {
                builder.Remove(decimalIndex - 1, 1);
            }
            else if (decimalIndex > commaIndex)
            {
                return ((decimalIndex - commaIndex) - 1);
            }
            return 0;
        }
    }
}

