namespace MS.Internal.IO.Packaging
{
    using MS.Internal;
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Xml;

    internal static class XmlSignatureProperties
    {
        private static readonly TimeFormatMapEntry[] _dateTimePatternMap = new TimeFormatMapEntry[] { new TimeFormatMapEntry("YYYY-MM-DDThh:mm:ss.sTZD", new string[] { "yyyy-MM-ddTHH:mm:ss.fzzz", "yyyy-MM-ddTHH:mm:ss.fZ" }), new TimeFormatMapEntry("YYYY-MM-DDThh:mm:ssTZD", new string[] { "yyyy-MM-ddTHH:mm:sszzz", "yyyy-MM-ddTHH:mm:ssZ" }), new TimeFormatMapEntry("YYYY-MM-DDThh:mmTZD", new string[] { "yyyy-MM-ddTHH:mmzzz", "yyyy-MM-ddTHH:mmZ" }), new TimeFormatMapEntry("YYYY-MM-DD", new string[] { "yyyy-MM-dd" }), new TimeFormatMapEntry("YYYY-MM", new string[] { "yyyy-MM" }), new TimeFormatMapEntry("YYYY", new string[] { "yyyy" }) };

        internal static XmlElement AssembleSignatureProperties(XmlDocument xDoc, DateTime dateTime, string xmlDateTimeFormat, string signatureId)
        {
            Invariant.Assert(xDoc != null);
            Invariant.Assert(signatureId != null);
            if (xmlDateTimeFormat == null)
            {
                xmlDateTimeFormat = DefaultDateTimeFormat;
            }
            string[] strArray = ConvertXmlFormatStringToDateTimeFormatString(xmlDateTimeFormat);
            XmlElement element = xDoc.CreateElement(XTable.Get(XTable.ID.SignaturePropertiesTagName), "http://www.w3.org/2000/09/xmldsig#");
            XmlElement newChild = xDoc.CreateElement(XTable.Get(XTable.ID.SignaturePropertyTagName), "http://www.w3.org/2000/09/xmldsig#");
            element.AppendChild(newChild);
            XmlAttribute node = xDoc.CreateAttribute(XTable.Get(XTable.ID.SignaturePropertyIdAttrName));
            node.Value = XTable.Get(XTable.ID.SignaturePropertyIdAttrValue);
            newChild.Attributes.Append(node);
            XmlAttribute attribute2 = xDoc.CreateAttribute(XTable.Get(XTable.ID.TargetAttrName));
            attribute2.Value = "#" + signatureId;
            newChild.Attributes.Append(attribute2);
            XmlElement element3 = xDoc.CreateElement(XTable.Get(XTable.ID.SignatureTimeTagName), XTable.Get(XTable.ID.OpcSignatureNamespace));
            XmlElement element4 = xDoc.CreateElement(XTable.Get(XTable.ID.SignatureTimeFormatTagName), XTable.Get(XTable.ID.OpcSignatureNamespace));
            XmlElement element5 = xDoc.CreateElement(XTable.Get(XTable.ID.SignatureTimeValueTagName), XTable.Get(XTable.ID.OpcSignatureNamespace));
            element4.AppendChild(xDoc.CreateTextNode(xmlDateTimeFormat));
            element5.AppendChild(xDoc.CreateTextNode(DateTimeToXmlFormattedTime(dateTime, strArray[0])));
            element3.AppendChild(element4);
            element3.AppendChild(element5);
            newChild.AppendChild(element3);
            return element;
        }

        private static string[] ConvertXmlFormatStringToDateTimeFormatString(string format) => 
            _dateTimePatternMap[GetIndex(format)].Patterns;

        private static string DateTimeToXmlFormattedTime(DateTime dt, string format)
        {
            DateTimeFormatInfo provider = new DateTimeFormatInfo {
                FullDateTimePattern = format
            };
            return dt.ToString(format, provider);
        }

        private static int GetIndex(string format)
        {
            for (int i = 0; i < _dateTimePatternMap.GetLength(0); i++)
            {
                if (string.CompareOrdinal(_dateTimePatternMap[i].Format, format) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        internal static bool LegalFormat(string candidateFormat)
        {
            if (candidateFormat == null)
            {
                throw new ArgumentNullException("candidateFormat");
            }
            return (GetIndex(candidateFormat) != -1);
        }

        private static DateTime ParseSignatureTimeTag(XmlReader reader, out string timeFormat)
        {
            int num = 0;
            string strB = XTable.Get(XTable.ID.OpcSignatureNamespace);
            string strA = XTable.Get(XTable.ID.SignaturePropertyTagName);
            string str3 = XTable.Get(XTable.ID.SignatureTimeTagName);
            string str4 = XTable.Get(XTable.ID.SignatureTimeValueTagName);
            string str5 = XTable.Get(XTable.ID.SignatureTimeFormatTagName);
            timeFormat = null;
            string s = null;
            if (((!reader.Read() || (reader.MoveToContent() != XmlNodeType.Element)) || ((string.CompareOrdinal(reader.NamespaceURI, strB) != 0) || (string.CompareOrdinal(reader.LocalName, str3) != 0))) || ((reader.Depth != 3) || (PackagingUtilities.GetNonXmlnsAttributeCount(reader) != num)))
            {
                throw new XmlException(System.Windows.SR.Get("RequiredTagNotFound", new object[] { str3 }));
            }
            while (reader.Read())
            {
                if (((string.CompareOrdinal(reader.NamespaceURI, strB) == 0) && (reader.MoveToContent() == XmlNodeType.Element)) && (reader.Depth == 4))
                {
                    if ((string.CompareOrdinal(reader.LocalName, str4) != 0) || (PackagingUtilities.GetNonXmlnsAttributeCount(reader) != num))
                    {
                        if ((string.CompareOrdinal(reader.LocalName, str5) != 0) || (PackagingUtilities.GetNonXmlnsAttributeCount(reader) != num))
                        {
                            throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
                        }
                        if (((timeFormat != null) || !reader.Read()) || ((reader.MoveToContent() != XmlNodeType.Text) || (reader.Depth != 5)))
                        {
                            throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
                        }
                        timeFormat = reader.ReadContentAsString();
                    }
                    else
                    {
                        if (((s != null) || !reader.Read()) || ((reader.MoveToContent() != XmlNodeType.Text) || (reader.Depth != 5)))
                        {
                            throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
                        }
                        s = reader.ReadContentAsString();
                    }
                }
                else
                {
                    if ((string.CompareOrdinal(str3, reader.LocalName) != 0) || (reader.NodeType != XmlNodeType.EndElement))
                    {
                        throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
                    }
                    if ((!reader.Read() || (reader.MoveToContent() != XmlNodeType.EndElement)) || (string.CompareOrdinal(strA, reader.LocalName) != 0))
                    {
                        throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
                    }
                    break;
                }
            }
            if ((s == null) || (timeFormat == null))
            {
                throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
            }
            return XmlFormattedTimeToDateTime(s, timeFormat);
        }

        internal static DateTime ParseSigningTime(XmlReader reader, string signatureId, out string timeFormat)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            bool flag = false;
            bool flag2 = false;
            string strB = "http://www.w3.org/2000/09/xmldsig#";
            string str2 = XTable.Get(XTable.ID.SignaturePropertyTagName);
            string strA = XTable.Get(XTable.ID.SignaturePropertiesTagName);
            DateTime now = DateTime.Now;
            timeFormat = null;
            while (reader.Read())
            {
                if (((reader.MoveToContent() == XmlNodeType.Element) && (string.CompareOrdinal(reader.NamespaceURI, strB) == 0)) && ((string.CompareOrdinal(reader.LocalName, str2) == 0) && (reader.Depth == 2)))
                {
                    if (VerifyIdAttribute(reader))
                    {
                        if (flag2)
                        {
                            throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
                        }
                        flag2 = true;
                        if (VerifyTargetAttribute(reader, signatureId))
                        {
                            now = ParseSignatureTimeTag(reader, out timeFormat);
                            flag = true;
                        }
                    }
                }
                else if (((string.CompareOrdinal(str2, reader.LocalName) != 0) || (reader.NodeType != XmlNodeType.EndElement)) && (reader.Depth <= 2))
                {
                    if ((string.CompareOrdinal(strA, reader.LocalName) != 0) || (reader.NodeType != XmlNodeType.EndElement))
                    {
                        throw new XmlException(System.Windows.SR.Get("RequiredTagNotFound", new object[] { str2 }));
                    }
                    break;
                }
            }
            if (!flag)
            {
                throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
            }
            return now;
        }

        private static bool VerifyIdAttribute(XmlReader reader)
        {
            string attribute = reader.GetAttribute(XTable.Get(XTable.ID.SignaturePropertyIdAttrName));
            return ((attribute != null) && (string.CompareOrdinal(attribute, XTable.Get(XTable.ID.SignaturePropertyIdAttrValue)) == 0));
        }

        private static bool VerifyTargetAttribute(XmlReader reader, string signatureId)
        {
            string attribute = reader.GetAttribute(XTable.Get(XTable.ID.TargetAttrName));
            if (attribute == null)
            {
                return false;
            }
            return ((string.CompareOrdinal(attribute, string.Empty) == 0) || ((signatureId != null) && (string.CompareOrdinal(attribute, "#" + signatureId) == 0)));
        }

        private static DateTime XmlFormattedTimeToDateTime(string s, string format)
        {
            string[] formats = ConvertXmlFormatStringToDateTimeFormatString(format);
            DateTimeFormatInfo provider = new DateTimeFormatInfo {
                FullDateTimePattern = format
            };
            return DateTime.ParseExact(s, formats, provider, DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowLeadingWhite);
        }

        internal static string DefaultDateTimeFormat =>
            _dateTimePatternMap[0].Format;

        [StructLayout(LayoutKind.Sequential)]
        private struct TimeFormatMapEntry
        {
            private string _xmlFormatString;
            private string[] _dateTimePatterns;
            public TimeFormatMapEntry(string xmlFormatString, string[] dateTimePatterns)
            {
                this._xmlFormatString = xmlFormatString;
                this._dateTimePatterns = dateTimePatterns;
            }

            public string Format =>
                this._xmlFormatString;
            public string[] Patterns =>
                this._dateTimePatterns;
        }
    }
}

