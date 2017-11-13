namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Runtime;

    internal class NumberAction : ContainerAction
    {
        private const int cchMaxFormat = 0x3f;
        private const int cchMaxFormatDecimal = 11;
        private int countKey = -1;
        private string countPattern;
        private static FormatInfo DefaultFormat = new FormatInfo(false, "0");
        private static FormatInfo DefaultSeparator = new FormatInfo(true, ".");
        private Avt formatAvt;
        private List<FormatInfo> formatTokens;
        private bool forwardCompatibility;
        private string from;
        private int fromKey = -1;
        private string groupingSep;
        private Avt groupingSepAvt;
        private string groupingSize;
        private Avt groupingSizeAvt;
        private string lang;
        private Avt langAvt;
        private string letter;
        private Avt letterAvt;
        private string level;
        private const long msofnfcAlwaysFormat = 2L;
        private const long msofnfcNil = 0L;
        private const long msofnfcTraditional = 1L;
        private const int OutputNumber = 2;
        private string value;
        private int valueKey = -1;

        private XPathNodeType BasicNodeType(XPathNodeType type)
        {
            if ((type != XPathNodeType.SignificantWhitespace) && (type != XPathNodeType.Whitespace))
            {
                return type;
            }
            return XPathNodeType.Text;
        }

        private bool checkFrom(Processor processor, XPathNavigator nav)
        {
            if (this.fromKey == -1)
            {
                return true;
            }
            do
            {
                if (processor.Matches(nav, this.fromKey))
                {
                    return true;
                }
            }
            while (nav.MoveToParent());
            return false;
        }

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            base.CheckEmpty(compiler);
            this.forwardCompatibility = compiler.ForwardCompatibility;
            this.formatTokens = ParseFormat(CompiledAction.PrecalculateAvt(ref this.formatAvt));
            this.letter = this.ParseLetter(CompiledAction.PrecalculateAvt(ref this.letterAvt));
            this.lang = CompiledAction.PrecalculateAvt(ref this.langAvt);
            this.groupingSep = CompiledAction.PrecalculateAvt(ref this.groupingSepAvt);
            if ((this.groupingSep != null) && (this.groupingSep.Length > 1))
            {
                throw XsltException.Create("Xslt_CharAttribute", new string[] { "grouping-separator" });
            }
            this.groupingSize = CompiledAction.PrecalculateAvt(ref this.groupingSizeAvt);
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string xpathQuery = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.Level))
            {
                if (((xpathQuery != "any") && (xpathQuery != "multiple")) && (xpathQuery != "single"))
                {
                    throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { "level", xpathQuery });
                }
                this.level = xpathQuery;
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Count))
            {
                this.countPattern = xpathQuery;
                this.countKey = compiler.AddQuery(xpathQuery, true, true, true);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.From))
            {
                this.from = xpathQuery;
                this.fromKey = compiler.AddQuery(xpathQuery, true, true, true);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Value))
            {
                this.value = xpathQuery;
                this.valueKey = compiler.AddQuery(xpathQuery);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Format))
            {
                this.formatAvt = Avt.CompileAvt(compiler, xpathQuery);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Lang))
            {
                this.langAvt = Avt.CompileAvt(compiler, xpathQuery);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.LetterValue))
            {
                this.letterAvt = Avt.CompileAvt(compiler, xpathQuery);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.GroupingSeparator))
            {
                this.groupingSepAvt = Avt.CompileAvt(compiler, xpathQuery);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.GroupingSize))
            {
                this.groupingSizeAvt = Avt.CompileAvt(compiler, xpathQuery);
            }
            else
            {
                return false;
            }
            return true;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            XPathNavigator navigator2;
            ArrayList numberList = processor.NumberList;
            switch (frame.State)
            {
                case 0:
                    numberList.Clear();
                    if (this.valueKey == -1)
                    {
                        if (this.level == "any")
                        {
                            int num = this.numberAny(processor, frame);
                            if (num != 0)
                            {
                                numberList.Add(num);
                            }
                            goto Label_0105;
                        }
                        bool flag = this.level == "multiple";
                        XPathNavigator node = frame.Node;
                        navigator2 = frame.Node.Clone();
                        if ((navigator2.NodeType == XPathNodeType.Attribute) || (navigator2.NodeType == XPathNodeType.Namespace))
                        {
                            navigator2.MoveToParent();
                        }
                        while (this.moveToCount(navigator2, processor, node))
                        {
                            numberList.Insert(0, this.numberCount(navigator2, processor, node));
                            if (!flag || !navigator2.MoveToParent())
                            {
                                break;
                            }
                        }
                        break;
                    }
                    numberList.Add(SimplifyValue(processor.Evaluate(frame, this.valueKey)));
                    goto Label_0105;

                case 1:
                    return;

                case 2:
                    goto Label_01AD;

                default:
                    return;
            }
            if (!this.checkFrom(processor, navigator2))
            {
                numberList.Clear();
            }
        Label_0105:
            frame.StoredOutput = Format(numberList, (this.formatAvt == null) ? this.formatTokens : ParseFormat(this.formatAvt.Evaluate(processor, frame)), (this.langAvt == null) ? this.lang : this.langAvt.Evaluate(processor, frame), (this.letterAvt == null) ? this.letter : this.ParseLetter(this.letterAvt.Evaluate(processor, frame)), (this.groupingSepAvt == null) ? this.groupingSep : this.groupingSepAvt.Evaluate(processor, frame), (this.groupingSizeAvt == null) ? this.groupingSize : this.groupingSizeAvt.Evaluate(processor, frame));
        Label_01AD:
            if (!processor.TextEvent(frame.StoredOutput))
            {
                frame.State = 2;
            }
            else
            {
                frame.Finished();
            }
        }

        private static string Format(ArrayList numberlist, List<FormatInfo> tokens, string lang, string letter, string groupingSep, string groupingSize)
        {
            StringBuilder builder = new StringBuilder();
            int count = 0;
            if (tokens != null)
            {
                count = tokens.Count;
            }
            NumberingFormat format = new NumberingFormat();
            if (groupingSize != null)
            {
                try
                {
                    format.setGroupingSize(Convert.ToInt32(groupingSize, CultureInfo.InvariantCulture));
                }
                catch (FormatException)
                {
                }
                catch (OverflowException)
                {
                }
            }
            if (groupingSep != null)
            {
                int length = groupingSep.Length;
                format.setGroupingSeparator(groupingSep);
            }
            if (0 < count)
            {
                FormatInfo info = tokens[0];
                FormatInfo info2 = null;
                if ((count % 2) == 1)
                {
                    info2 = tokens[count - 1];
                    count--;
                }
                FormatInfo info3 = (2 < count) ? tokens[count - 2] : DefaultSeparator;
                FormatInfo info4 = (0 < count) ? tokens[count - 1] : DefaultFormat;
                if (info != null)
                {
                    builder.Append(info.formatString);
                }
                int num2 = numberlist.Count;
                for (int i = 0; i < num2; i++)
                {
                    int num4 = i * 2;
                    bool flag = num4 < count;
                    if (0 < i)
                    {
                        FormatInfo info5 = flag ? tokens[num4] : info3;
                        builder.Append(info5.formatString);
                    }
                    FormatInfo info6 = flag ? tokens[num4 + 1] : info4;
                    format.setNumberingType(info6.numSequence);
                    format.setMinLen(info6.length);
                    builder.Append(format.FormatItem(numberlist[i]));
                }
                if (info2 != null)
                {
                    builder.Append(info2.formatString);
                }
            }
            else
            {
                format.setNumberingType(NumberingSequence.FirstDecimal);
                for (int j = 0; j < numberlist.Count; j++)
                {
                    if (j != 0)
                    {
                        builder.Append(".");
                    }
                    builder.Append(format.FormatItem(numberlist[j]));
                }
            }
            return builder.ToString();
        }

        private static void mapFormatToken(string wsToken, int startLen, int tokLen, out NumberingSequence seq, out int pminlen)
        {
            char ch = wsToken[startLen];
            bool flag = false;
            pminlen = 1;
            seq = NumberingSequence.Nil;
            switch (ch)
            {
                case '๐':
                case 0xc77b:
                case 0xff10:
                case '0':
                case '०':
                    do
                    {
                        pminlen++;
                    }
                    while ((--tokLen > 0) && (ch == wsToken[++startLen]));
                    if (wsToken[startLen] != ((char) (ch + '\x0001')))
                    {
                        flag = true;
                    }
                    break;
            }
            if (!flag)
            {
                switch (wsToken[startLen])
                {
                    case 'a':
                        seq = NumberingSequence.LCLetter;
                        goto Label_0307;

                    case 'i':
                        seq = NumberingSequence.LCRoman;
                        goto Label_0307;

                    case 'А':
                        seq = NumberingSequence.UCRus;
                        goto Label_0307;

                    case '1':
                        seq = NumberingSequence.FirstDecimal;
                        goto Label_0307;

                    case 'A':
                        seq = NumberingSequence.FirstAlpha;
                        goto Label_0307;

                    case 'I':
                        seq = NumberingSequence.FirstSpecial;
                        goto Label_0307;

                    case 'а':
                        seq = NumberingSequence.LCRus;
                        goto Label_0307;

                    case 'א':
                        seq = NumberingSequence.Hebrew;
                        goto Label_0307;

                    case 'أ':
                        seq = NumberingSequence.ArabicScript;
                        goto Label_0307;

                    case '१':
                        seq = NumberingSequence.Hindi3;
                        goto Label_0307;

                    case 'ก':
                        seq = NumberingSequence.Thai1;
                        goto Label_0307;

                    case 'अ':
                        seq = NumberingSequence.Hindi2;
                        goto Label_0307;

                    case 'क':
                        seq = NumberingSequence.Hindi1;
                        goto Label_0307;

                    case '一':
                        seq = NumberingSequence.FEDecimal;
                        goto Label_0307;

                    case '壱':
                        seq = NumberingSequence.DbNum3;
                        goto Label_0307;

                    case '壹':
                        seq = NumberingSequence.ChnCmplx;
                        goto Label_0307;

                    case 'ア':
                        seq = NumberingSequence.DAiueo;
                        goto Label_0307;

                    case 'イ':
                        seq = NumberingSequence.DIroha;
                        goto Label_0307;

                    case 'ㄱ':
                        seq = NumberingSequence.DChosung;
                        goto Label_0307;

                    case '๑':
                        seq = NumberingSequence.Thai2;
                        goto Label_0307;

                    case '子':
                        seq = NumberingSequence.Zodiac2;
                        goto Label_0307;

                    case '甲':
                        if ((tokLen > 1) && (wsToken[startLen + 1] == '子'))
                        {
                            seq = NumberingSequence.Zodiac3;
                            tokLen--;
                            startLen++;
                        }
                        else
                        {
                            seq = NumberingSequence.Zodiac1;
                        }
                        goto Label_0307;

                    case 0xac00:
                        seq = NumberingSequence.Ganada;
                        goto Label_0307;

                    case 0xff71:
                        seq = NumberingSequence.Aiueo;
                        goto Label_0307;

                    case 0xff72:
                        seq = NumberingSequence.Iroha;
                        goto Label_0307;

                    case 0xff11:
                        seq = NumberingSequence.DArabic;
                        goto Label_0307;

                    case 0xc77c:
                        seq = NumberingSequence.KorDbNum1;
                        goto Label_0307;

                    case 0xd558:
                        seq = NumberingSequence.KorDbNum3;
                        goto Label_0307;
                }
                seq = NumberingSequence.FirstDecimal;
            }
        Label_0307:
            if (flag)
            {
                seq = NumberingSequence.FirstDecimal;
                pminlen = 0;
            }
        }

        private bool MatchCountKey(Processor processor, XPathNavigator contextNode, XPathNavigator nav)
        {
            if (this.countKey != -1)
            {
                return processor.Matches(nav, this.countKey);
            }
            return ((contextNode.Name == nav.Name) && (this.BasicNodeType(contextNode.NodeType) == this.BasicNodeType(nav.NodeType)));
        }

        private bool moveToCount(XPathNavigator nav, Processor processor, XPathNavigator contextNode)
        {
            do
            {
                if ((this.fromKey != -1) && processor.Matches(nav, this.fromKey))
                {
                    return false;
                }
                if (this.MatchCountKey(processor, contextNode, nav))
                {
                    return true;
                }
            }
            while (nav.MoveToParent());
            return false;
        }

        private int numberAny(Processor processor, ActionFrame frame)
        {
            int num = 0;
            XPathNavigator node = frame.Node;
            if ((node.NodeType == XPathNodeType.Attribute) || (node.NodeType == XPathNodeType.Namespace))
            {
                node = node.Clone();
                node.MoveToParent();
            }
            XPathNavigator context = node.Clone();
            if (this.fromKey == -1)
            {
                context.MoveToRoot();
                XPathNodeIterator iterator2 = context.SelectDescendants(XPathNodeType.All, true);
                while (iterator2.MoveNext())
                {
                    if (this.MatchCountKey(processor, frame.Node, iterator2.Current))
                    {
                        num++;
                    }
                    if (iterator2.Current.IsSamePosition(node))
                    {
                        return num;
                    }
                }
                return num;
            }
            bool flag = false;
            do
            {
                if (processor.Matches(context, this.fromKey))
                {
                    flag = true;
                    break;
                }
            }
            while (context.MoveToParent());
            XPathNodeIterator iterator = context.SelectDescendants(XPathNodeType.All, true);
            while (iterator.MoveNext())
            {
                if (processor.Matches(iterator.Current, this.fromKey))
                {
                    flag = true;
                    num = 0;
                }
                else if (this.MatchCountKey(processor, frame.Node, iterator.Current))
                {
                    num++;
                }
                if (iterator.Current.IsSamePosition(node))
                {
                    break;
                }
            }
            if (!flag)
            {
                num = 0;
            }
            return num;
        }

        private int numberCount(XPathNavigator nav, Processor processor, XPathNavigator contextNode)
        {
            XPathNavigator navigator = nav.Clone();
            int num = 1;
            if (navigator.MoveToParent())
            {
                navigator.MoveToFirstChild();
                while (!navigator.IsSamePosition(nav))
                {
                    if (this.MatchCountKey(processor, contextNode, navigator))
                    {
                        num++;
                    }
                    if (!navigator.MoveToNext())
                    {
                        return num;
                    }
                }
            }
            return num;
        }

        private static List<FormatInfo> ParseFormat(string formatString)
        {
            if ((formatString == null) || (formatString.Length == 0))
            {
                return null;
            }
            int num = 0;
            bool flag = CharUtil.IsAlphaNumeric(formatString[num]);
            List<FormatInfo> list = new List<FormatInfo>();
            int startLen = 0;
            if (flag)
            {
                list.Add(null);
            }
            while (num <= formatString.Length)
            {
                bool flag2 = (num < formatString.Length) ? CharUtil.IsAlphaNumeric(formatString[num]) : !flag;
                if (flag != flag2)
                {
                    FormatInfo item = new FormatInfo();
                    if (flag)
                    {
                        mapFormatToken(formatString, startLen, num - startLen, out item.numSequence, out item.length);
                    }
                    else
                    {
                        item.isSeparator = true;
                        item.formatString = formatString.Substring(startLen, num - startLen);
                    }
                    startLen = num;
                    num++;
                    list.Add(item);
                    flag = flag2;
                }
                else
                {
                    num++;
                }
            }
            return list;
        }

        private string ParseLetter(string letter)
        {
            if (((letter == null) || (letter == "traditional")) || (letter == "alphabetic"))
            {
                return letter;
            }
            if (!this.forwardCompatibility)
            {
                throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { "letter-value", letter });
            }
            return null;
        }

        private static object SimplifyValue(object value)
        {
            if (Type.GetTypeCode(value.GetType()) == TypeCode.Object)
            {
                XPathNodeIterator iterator = value as XPathNodeIterator;
                if (iterator != null)
                {
                    if (iterator.MoveNext())
                    {
                        return iterator.Current.Value;
                    }
                    return string.Empty;
                }
                XPathNavigator navigator = value as XPathNavigator;
                if (navigator != null)
                {
                    return navigator.Value;
                }
            }
            return value;
        }

        internal class FormatInfo
        {
            public string formatString;
            public bool isSeparator;
            public int length;
            public NumberingSequence numSequence;

            public FormatInfo()
            {
            }

            public FormatInfo(bool isSeparator, string formatString)
            {
                this.isSeparator = isSeparator;
                this.formatString = formatString;
            }
        }

        private class NumberingFormat : NumberFormatterBase
        {
            private int cMinLen;
            private string separator;
            private NumberingSequence seq;
            private int sizeGroup;

            internal NumberingFormat()
            {
            }

            private static string ConvertToArabic(double val, int minLength, int groupSize, string groupSeparator)
            {
                string str;
                if ((groupSize != 0) && (groupSeparator != null))
                {
                    NumberFormatInfo provider = new NumberFormatInfo {
                        NumberGroupSizes = new int[] { groupSize },
                        NumberGroupSeparator = groupSeparator
                    };
                    if (Math.Floor(val) == val)
                    {
                        provider.NumberDecimalDigits = 0;
                    }
                    str = val.ToString("N", provider);
                }
                else
                {
                    str = Convert.ToString(val, CultureInfo.InvariantCulture);
                }
                if (str.Length >= minLength)
                {
                    return str;
                }
                StringBuilder builder = new StringBuilder(minLength);
                builder.Append('0', minLength - str.Length);
                builder.Append(str);
                return builder.ToString();
            }

            internal string FormatItem(object value)
            {
                double num;
                if (value is int)
                {
                    num = (int) value;
                }
                else
                {
                    num = XmlConvert.ToXPathDouble(value);
                    if ((0.5 <= num) && !double.IsPositiveInfinity(num))
                    {
                        num = XmlConvert.XPathRound(num);
                    }
                    else
                    {
                        return XmlConvert.ToXPathString(value);
                    }
                }
                switch (this.seq)
                {
                    case NumberingSequence.FirstAlpha:
                    case NumberingSequence.LCLetter:
                    {
                        if (num > 2147483647.0)
                        {
                            break;
                        }
                        StringBuilder sb = new StringBuilder();
                        NumberFormatterBase.ConvertToAlphabetic(sb, num, (this.seq == NumberingSequence.FirstAlpha) ? 'A' : 'a', 0x1a);
                        return sb.ToString();
                    }
                    case NumberingSequence.FirstSpecial:
                    case NumberingSequence.LCRoman:
                    {
                        if (num > 32767.0)
                        {
                            break;
                        }
                        StringBuilder builder2 = new StringBuilder();
                        NumberFormatterBase.ConvertToRoman(builder2, num, this.seq == NumberingSequence.FirstSpecial);
                        return builder2.ToString();
                    }
                }
                return ConvertToArabic(num, this.cMinLen, this.sizeGroup, this.separator);
            }

            internal void setGroupingSeparator(string separator)
            {
                this.separator = separator;
            }

            internal void setGroupingSize(int sizeGroup)
            {
                if ((0 <= sizeGroup) && (sizeGroup <= 9))
                {
                    this.sizeGroup = sizeGroup;
                }
            }

            internal void setMinLen(int cMinLen)
            {
                this.cMinLen = cMinLen;
            }

            internal void setNumberingType(NumberingSequence seq)
            {
                this.seq = seq;
            }
        }
    }
}

