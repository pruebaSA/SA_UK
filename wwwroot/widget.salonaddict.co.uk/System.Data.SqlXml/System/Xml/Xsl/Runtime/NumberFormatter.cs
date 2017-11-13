namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class NumberFormatter : NumberFormatterBase
    {
        private static readonly TokenInfo DefaultFormat = TokenInfo.CreateFormat("0", 0, 1);
        private static readonly TokenInfo DefaultSeparator = TokenInfo.CreateSeparator(".", 0, 1);
        public const char DefaultStartChar = '1';
        private string formatString;
        private string groupingSeparator;
        private int groupingSize;
        private int lang;
        private string letterValue;
        private List<TokenInfo> tokens;

        public NumberFormatter(string formatString, int lang, string letterValue, string groupingSeparator, int groupingSize)
        {
            this.formatString = formatString;
            this.lang = lang;
            this.letterValue = letterValue;
            this.groupingSeparator = groupingSeparator;
            this.groupingSize = (groupingSeparator.Length > 0) ? groupingSize : 0;
            if ((formatString != "1") && (formatString.Length != 0))
            {
                this.tokens = new List<TokenInfo>();
                int startIdx = 0;
                bool flag = CharUtil.IsAlphaNumeric(formatString[startIdx]);
                if (flag)
                {
                    this.tokens.Add(null);
                }
                for (int i = 0; i <= formatString.Length; i++)
                {
                    if ((i == formatString.Length) || (flag != CharUtil.IsAlphaNumeric(formatString[i])))
                    {
                        if (flag)
                        {
                            this.tokens.Add(TokenInfo.CreateFormat(formatString, startIdx, i - startIdx));
                        }
                        else
                        {
                            this.tokens.Add(TokenInfo.CreateSeparator(formatString, startIdx, i - startIdx));
                        }
                        startIdx = i;
                        flag = !flag;
                    }
                }
            }
        }

        private static unsafe string ConvertToDecimal(double val, int minLen, char zero, string groupSeparator, int groupSize)
        {
            string str = XPathConvert.DoubleToString(val);
            int num = zero - '0';
            int length = str.Length;
            int totalWidth = Math.Max(length, minLen);
            if (groupSize != 0)
            {
                totalWidth += (totalWidth - 1) / groupSize;
            }
            if ((totalWidth == length) && (num == 0))
            {
                return str;
            }
            if ((groupSize == 0) && (num == 0))
            {
                return str.PadLeft(totalWidth, zero);
            }
            char* chPtr = (char*) stackalloc byte[(2 * totalWidth)];
            char ch = (groupSeparator.Length > 0) ? groupSeparator[0] : ' ';
            fixed (char* str2 = ((char*) str))
            {
                char* chPtr2 = str2;
                char* chPtr3 = (chPtr2 + length) - 1;
                char* chPtr4 = (chPtr + totalWidth) - 1;
                int num4 = groupSize;
            Label_008B:
                chPtr4--;
                chPtr4[0] = (chPtr3 >= chPtr2) ? ((char) (*(--chPtr3) + num)) : zero;
                if (chPtr4 >= chPtr)
                {
                    if (--num4 == 0)
                    {
                        chPtr4--;
                        chPtr4[0] = ch;
                        num4 = groupSize;
                    }
                    goto Label_008B;
                }
            }
            return new string(chPtr, 0, totalWidth);
        }

        private void FormatItem(StringBuilder sb, XPathItem item, char startChar, int length)
        {
            double valueAsInt;
            if (item.ValueType == typeof(int))
            {
                valueAsInt = item.ValueAsInt;
            }
            else
            {
                valueAsInt = XsltFunctions.Round(item.ValueAsDouble);
            }
            char zero = '0';
            switch (startChar)
            {
                case 'I':
                case 'i':
                    if (valueAsInt > 32767.0)
                    {
                        break;
                    }
                    NumberFormatterBase.ConvertToRoman(sb, valueAsInt, startChar == 'I');
                    return;

                case 'a':
                case 'A':
                    if (valueAsInt > 2147483647.0)
                    {
                        break;
                    }
                    NumberFormatterBase.ConvertToAlphabetic(sb, valueAsInt, startChar, 0x1a);
                    return;

                case '1':
                    break;

                default:
                    zero = (char) (startChar - '\x0001');
                    break;
            }
            sb.Append(ConvertToDecimal(valueAsInt, length, zero, this.groupingSeparator, this.groupingSize));
        }

        public string FormatSequence(IList<XPathItem> val)
        {
            StringBuilder sb = new StringBuilder();
            if ((val.Count == 1) && (val[0].ValueType == typeof(double)))
            {
                double valueAsDouble = val[0].ValueAsDouble;
                if ((0.5 > valueAsDouble) || (valueAsDouble >= double.PositiveInfinity))
                {
                    return XPathConvert.DoubleToString(valueAsDouble);
                }
            }
            if (this.tokens == null)
            {
                for (int i = 0; i < val.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append('.');
                    }
                    this.FormatItem(sb, val[i], '1', 1);
                }
            }
            else
            {
                TokenInfo info2;
                int count = this.tokens.Count;
                TokenInfo info = this.tokens[0];
                if ((count % 2) == 0)
                {
                    info2 = null;
                }
                else
                {
                    info2 = this.tokens[--count];
                }
                TokenInfo info3 = (2 < count) ? this.tokens[count - 2] : DefaultSeparator;
                TokenInfo info4 = (0 < count) ? this.tokens[count - 1] : DefaultFormat;
                if (info != null)
                {
                    sb.Append(info.formatString, info.startIdx, info.length);
                }
                int num4 = val.Count;
                for (int j = 0; j < num4; j++)
                {
                    int num6 = j * 2;
                    bool flag = num6 < count;
                    if (j > 0)
                    {
                        TokenInfo info5 = flag ? this.tokens[num6] : info3;
                        sb.Append(info5.formatString, info5.startIdx, info5.length);
                    }
                    TokenInfo info6 = flag ? this.tokens[num6 + 1] : info4;
                    this.FormatItem(sb, val[j], info6.startChar, info6.length);
                }
                if (info2 != null)
                {
                    sb.Append(info2.formatString, info2.startIdx, info2.length);
                }
            }
            return sb.ToString();
        }
    }
}

