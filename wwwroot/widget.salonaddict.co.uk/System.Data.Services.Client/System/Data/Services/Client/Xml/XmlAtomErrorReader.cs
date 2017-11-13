namespace System.Data.Services.Client.Xml
{
    using System;
    using System.Data.Services.Client;
    using System.Diagnostics;
    using System.Xml;

    [DebuggerDisplay("XmlAtomErrorReader {NodeType} {Name} {Value}")]
    internal class XmlAtomErrorReader : System.Data.Services.Client.Xml.XmlWrappingReader
    {
        internal XmlAtomErrorReader(XmlReader baseReader) : base(baseReader)
        {
            base.Reader = baseReader;
        }

        public override bool Read()
        {
            bool flag = base.Read();
            if ((this.NodeType == XmlNodeType.Element) && Util.AreSame(base.Reader, "error", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata"))
            {
                throw new DataServiceClientException(Strings.Deserialize_ServerException(ReadErrorMessage(base.Reader)));
            }
            return flag;
        }

        internal static string ReadElementString(XmlReader reader, bool checkNullAttribute)
        {
            // This item is obfuscated and can not be translated.
            int expressionStack_11_0;
            string str = null;
            if (checkNullAttribute)
            {
                expressionStack_11_0 = (int) !Util.DoesNullAttributeSayTrue(reader);
            }
            else
            {
                expressionStack_11_0 = 0;
            }
            bool flag = (bool) expressionStack_11_0;
            if (reader.IsEmptyElement)
            {
                if (!flag)
                {
                    return null;
                }
                return string.Empty;
            }
        Label_0091:
            if (!reader.Read())
            {
                throw Error.InvalidOperation(Strings.Deserialize_ExpectingSimpleValue);
            }
            switch (reader.NodeType)
            {
                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                case XmlNodeType.SignificantWhitespace:
                    if (str != null)
                    {
                        throw Error.InvalidOperation(Strings.Deserialize_MixedTextWithComment);
                    }
                    str = reader.Value;
                    goto Label_0091;

                case XmlNodeType.Comment:
                case XmlNodeType.Whitespace:
                    goto Label_0091;

                case XmlNodeType.EndElement:
                    string expressionStack_63_0;
                    if (str != null)
                    {
                        return str;
                    }
                    else
                    {
                        expressionStack_63_0 = str;
                    }
                    expressionStack_63_0 = string.Empty;
                    if (flag)
                    {
                        return string.Empty;
                    }
                    return null;
            }
            throw Error.InvalidOperation(Strings.Deserialize_ExpectingSimpleValue);
        }

        private static string ReadErrorMessage(XmlReader reader)
        {
            int num = 1;
            while ((num > 0) && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (!reader.IsEmptyElement)
                    {
                        num++;
                    }
                    if ((num == 2) && Util.AreSame(reader, "message", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata"))
                    {
                        return ReadElementString(reader, false);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    num--;
                }
            }
            return string.Empty;
        }
    }
}

