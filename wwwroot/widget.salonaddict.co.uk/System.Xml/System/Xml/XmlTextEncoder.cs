namespace System.Xml
{
    using System;
    using System.Globalization;
    using System.IO;

    internal class XmlTextEncoder
    {
        private BufferBuilder attrValue;
        private bool cacheAttrValue;
        private bool inAttribute;
        private char quoteChar;
        private const int SurHighEnd = 0xdbff;
        private const int SurHighStart = 0xd800;
        private const int SurLowEnd = 0xdfff;
        private const int SurLowStart = 0xdc00;
        private TextWriter textWriter;
        private XmlCharType xmlCharType;

        internal XmlTextEncoder(TextWriter textWriter)
        {
            this.textWriter = textWriter;
            this.quoteChar = '"';
            this.xmlCharType = XmlCharType.Instance;
        }

        internal void EndAttribute()
        {
            if (this.cacheAttrValue)
            {
                this.attrValue.Clear();
            }
            this.inAttribute = false;
            this.cacheAttrValue = false;
        }

        internal void Flush()
        {
        }

        internal void StartAttribute(bool cacheAttrValue)
        {
            this.inAttribute = true;
            this.cacheAttrValue = cacheAttrValue;
            if (cacheAttrValue)
            {
                if (this.attrValue == null)
                {
                    this.attrValue = new BufferBuilder();
                }
                else
                {
                    this.attrValue.Clear();
                }
            }
        }

        internal unsafe void Write(char ch)
        {
            if (this.cacheAttrValue)
            {
                this.attrValue.Append(ch);
            }
            if ((this.xmlCharType.charProperties[ch] & 0x80) != 0)
            {
                this.textWriter.Write(ch);
            }
            else
            {
                switch (ch)
                {
                    case '\t':
                        this.textWriter.Write(ch);
                        return;

                    case '\n':
                    case '\r':
                        if (!this.inAttribute)
                        {
                            this.textWriter.Write(ch);
                            return;
                        }
                        this.WriteCharEntityImpl(ch);
                        return;

                    case '"':
                        if (this.inAttribute && (this.quoteChar == ch))
                        {
                            this.WriteEntityRefImpl("quot");
                            return;
                        }
                        this.textWriter.Write('"');
                        return;

                    case '&':
                        this.WriteEntityRefImpl("amp");
                        return;

                    case '\'':
                        if (!this.inAttribute || (this.quoteChar != ch))
                        {
                            this.textWriter.Write('\'');
                            return;
                        }
                        this.WriteEntityRefImpl("apos");
                        return;

                    case '<':
                        this.WriteEntityRefImpl("lt");
                        return;

                    case '>':
                        this.WriteEntityRefImpl("gt");
                        return;
                }
                if ((ch >= 0xd800) && (ch <= 0xdbff))
                {
                    throw new ArgumentException(Res.GetString("Xml_InvalidSurrogateMissingLowChar"));
                }
                if ((ch >= 0xdc00) && (ch <= 0xdfff))
                {
                    throw XmlConvert.CreateInvalidHighSurrogateCharException(ch);
                }
                this.WriteCharEntityImpl(ch);
            }
        }

        internal unsafe void Write(string text)
        {
            char[] chArray;
            if (text == null)
            {
                return;
            }
            if (this.cacheAttrValue)
            {
                this.attrValue.Append(text);
            }
            int length = text.Length;
            int num2 = 0;
            int offset = 0;
            char ch = '\0';
        Label_002B:
            while ((num2 < length) && ((this.xmlCharType.charProperties[ch = text[num2]] & 0x80) != 0))
            {
                num2++;
            }
            if (num2 == length)
            {
                this.textWriter.Write(text);
                return;
            }
            if (this.inAttribute)
            {
                if (ch != '\t')
                {
                    goto Label_0090;
                }
                num2++;
                goto Label_002B;
            }
            if (((ch == '\t') || (ch == '\n')) || (((ch == '\r') || (ch == '"')) || (ch == '\'')))
            {
                num2++;
                goto Label_002B;
            }
        Label_0090:
            chArray = new char[0x100];
            while (true)
            {
                if (offset < num2)
                {
                    this.WriteStringFragment(text, offset, num2 - offset, chArray);
                }
                if (num2 == length)
                {
                    return;
                }
                switch (ch)
                {
                    case '\t':
                        this.textWriter.Write(ch);
                        break;

                    case '\n':
                    case '\r':
                        if (!this.inAttribute)
                        {
                            this.textWriter.Write(ch);
                        }
                        else
                        {
                            this.WriteCharEntityImpl(ch);
                        }
                        break;

                    case '"':
                        if (this.inAttribute && (this.quoteChar == ch))
                        {
                            this.WriteEntityRefImpl("quot");
                        }
                        else
                        {
                            this.textWriter.Write('"');
                        }
                        break;

                    case '&':
                        this.WriteEntityRefImpl("amp");
                        break;

                    case '\'':
                        if (!this.inAttribute || (this.quoteChar != ch))
                        {
                            this.textWriter.Write('\'');
                        }
                        else
                        {
                            this.WriteEntityRefImpl("apos");
                        }
                        break;

                    case '<':
                        this.WriteEntityRefImpl("lt");
                        break;

                    case '>':
                        this.WriteEntityRefImpl("gt");
                        break;

                    default:
                        if ((ch >= 0xd800) && (ch <= 0xdbff))
                        {
                            if ((num2 + 1) >= length)
                            {
                                throw XmlConvert.CreateInvalidSurrogatePairException(text[num2], ch);
                            }
                            this.WriteSurrogateChar(text[++num2], ch);
                        }
                        else
                        {
                            if ((ch >= 0xdc00) && (ch <= 0xdfff))
                            {
                                throw XmlConvert.CreateInvalidHighSurrogateCharException(ch);
                            }
                            this.WriteCharEntityImpl(ch);
                        }
                        break;
                }
                num2++;
                offset = num2;
                while ((num2 < length) && ((this.xmlCharType.charProperties[ch = text[num2]] & 0x80) != 0))
                {
                    num2++;
                }
            }
        }

        internal unsafe void Write(char[] array, int offset, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (0 > offset)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (0 > count)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (count > (array.Length - offset))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (this.cacheAttrValue)
            {
                this.attrValue.Append(array, offset, count);
            }
            int num = offset + count;
            int index = offset;
            char ch = '\0';
            while (true)
            {
                int num3 = index;
                while ((index < num) && ((this.xmlCharType.charProperties[ch = array[index]] & 0x80) != 0))
                {
                    index++;
                }
                if (num3 < index)
                {
                    this.textWriter.Write(array, num3, index - num3);
                }
                if (index == num)
                {
                    return;
                }
                switch (ch)
                {
                    case '\t':
                        this.textWriter.Write(ch);
                        break;

                    case '\n':
                    case '\r':
                        if (!this.inAttribute)
                        {
                            this.textWriter.Write(ch);
                        }
                        else
                        {
                            this.WriteCharEntityImpl(ch);
                        }
                        break;

                    case '"':
                        if (this.inAttribute && (this.quoteChar == ch))
                        {
                            this.WriteEntityRefImpl("quot");
                        }
                        else
                        {
                            this.textWriter.Write('"');
                        }
                        break;

                    case '&':
                        this.WriteEntityRefImpl("amp");
                        break;

                    case '\'':
                        if (!this.inAttribute || (this.quoteChar != ch))
                        {
                            this.textWriter.Write('\'');
                        }
                        else
                        {
                            this.WriteEntityRefImpl("apos");
                        }
                        break;

                    case '<':
                        this.WriteEntityRefImpl("lt");
                        break;

                    case '>':
                        this.WriteEntityRefImpl("gt");
                        break;

                    default:
                        if ((ch >= 0xd800) && (ch <= 0xdbff))
                        {
                            if ((index + 1) >= num)
                            {
                                throw new ArgumentException(Res.GetString("Xml_SurrogatePairSplit"));
                            }
                            this.WriteSurrogateChar(array[++index], ch);
                        }
                        else
                        {
                            if ((ch >= 0xdc00) && (ch <= 0xdfff))
                            {
                                throw XmlConvert.CreateInvalidHighSurrogateCharException(ch);
                            }
                            this.WriteCharEntityImpl(ch);
                        }
                        break;
                }
                index++;
            }
        }

        internal void WriteCharEntity(char ch)
        {
            if ((ch >= 0xd800) && (ch <= 0xdfff))
            {
                throw new ArgumentException(Res.GetString("Xml_InvalidSurrogateMissingLowChar"));
            }
            string str = ((int) ch).ToString("X", NumberFormatInfo.InvariantInfo);
            if (this.cacheAttrValue)
            {
                this.attrValue.Append("&#x");
                this.attrValue.Append(str);
                this.attrValue.Append(';');
            }
            this.WriteCharEntityImpl(str);
        }

        private void WriteCharEntityImpl(char ch)
        {
            this.WriteCharEntityImpl(((int) ch).ToString("X", NumberFormatInfo.InvariantInfo));
        }

        private void WriteCharEntityImpl(string strVal)
        {
            this.textWriter.Write("&#x");
            this.textWriter.Write(strVal);
            this.textWriter.Write(';');
        }

        internal void WriteEntityRef(string name)
        {
            if (this.cacheAttrValue)
            {
                this.attrValue.Append('&');
                this.attrValue.Append(name);
                this.attrValue.Append(';');
            }
            this.WriteEntityRefImpl(name);
        }

        private void WriteEntityRefImpl(string name)
        {
            this.textWriter.Write('&');
            this.textWriter.Write(name);
            this.textWriter.Write(';');
        }

        internal void WriteRaw(string value)
        {
            if (this.cacheAttrValue)
            {
                this.attrValue.Append(value);
            }
            this.textWriter.Write(value);
        }

        internal void WriteRaw(char[] array, int offset, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (0 > count)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (0 > offset)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (count > (array.Length - offset))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (this.cacheAttrValue)
            {
                this.attrValue.Append(array, offset, count);
            }
            this.textWriter.Write(array, offset, count);
        }

        internal unsafe void WriteRawWithSurrogateChecking(string text)
        {
            if (text == null)
            {
                return;
            }
            if (this.cacheAttrValue)
            {
                this.attrValue.Append(text);
            }
            int length = text.Length;
            int num2 = 0;
            char hi = '\0';
        Label_0029:
            while ((num2 < length) && (((this.xmlCharType.charProperties[hi = text[num2]] & 0x10) != 0) || (hi < ' ')))
            {
                num2++;
            }
            if (num2 != length)
            {
                if ((hi >= 0xd800) && (hi <= 0xdbff))
                {
                    if ((num2 + 1) >= length)
                    {
                        throw new ArgumentException(Res.GetString("Xml_InvalidSurrogateMissingLowChar"));
                    }
                    char low = text[num2 + 1];
                    if ((low < 0xdc00) || (low > 0xdfff))
                    {
                        throw XmlConvert.CreateInvalidSurrogatePairException(low, hi);
                    }
                    num2 += 2;
                }
                else
                {
                    if ((hi >= 0xdc00) && (hi <= 0xdfff))
                    {
                        throw XmlConvert.CreateInvalidHighSurrogateCharException(hi);
                    }
                    num2++;
                }
                goto Label_0029;
            }
            this.textWriter.Write(text);
        }

        private void WriteStringFragment(string str, int offset, int count, char[] helperBuffer)
        {
            int length = helperBuffer.Length;
            while (count > 0)
            {
                int num2 = count;
                if (num2 > length)
                {
                    num2 = length;
                }
                str.CopyTo(offset, helperBuffer, 0, num2);
                this.textWriter.Write(helperBuffer, 0, num2);
                offset += num2;
                count -= num2;
            }
        }

        internal void WriteSurrogateChar(char lowChar, char highChar)
        {
            if (((lowChar < 0xdc00) || (lowChar > 0xdfff)) || ((highChar < 0xd800) || (highChar > 0xdbff)))
            {
                throw XmlConvert.CreateInvalidSurrogatePairException(lowChar, highChar);
            }
            this.textWriter.Write(highChar);
            this.textWriter.Write(lowChar);
        }

        internal void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            if (((lowChar < 0xdc00) || (lowChar > 0xdfff)) || ((highChar < 0xd800) || (highChar > 0xdbff)))
            {
                throw XmlConvert.CreateInvalidSurrogatePairException(lowChar, highChar);
            }
            int num = (lowChar - 0xdc00) | (((highChar - 0xd800) << 10) + 0x10000);
            if (this.cacheAttrValue)
            {
                this.attrValue.Append(highChar);
                this.attrValue.Append(lowChar);
            }
            this.textWriter.Write("&#x");
            this.textWriter.Write(num.ToString("X", NumberFormatInfo.InvariantInfo));
            this.textWriter.Write(';');
        }

        internal string AttributeValue
        {
            get
            {
                if (this.cacheAttrValue)
                {
                    return this.attrValue.ToString();
                }
                return string.Empty;
            }
        }

        internal char QuoteChar
        {
            set
            {
                this.quoteChar = value;
            }
        }
    }
}

