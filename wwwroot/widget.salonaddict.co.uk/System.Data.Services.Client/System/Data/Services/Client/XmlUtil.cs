namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    internal static class XmlUtil
    {
        private static NameTable CreateAtomNameTable()
        {
            NameTable table = new NameTable();
            table.Add("http://www.w3.org/2005/Atom");
            table.Add("http://schemas.microsoft.com/ado/2007/08/dataservices");
            table.Add("http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            table.Add("content");
            table.Add("src");
            table.Add("entry");
            table.Add("etag");
            table.Add("feed");
            table.Add("id");
            table.Add("inline");
            table.Add("link");
            table.Add("rel");
            table.Add("null");
            table.Add("properties");
            table.Add("title");
            table.Add("type");
            table.Add("code");
            table.Add("error");
            table.Add("innererror");
            table.Add("message");
            table.Add("type");
            return table;
        }

        internal static XmlReader CreateXmlReader(Stream stream, Encoding encoding)
        {
            XmlReaderSettings settings = new XmlReaderSettings {
                CheckCharacters = false,
                CloseInput = true,
                IgnoreWhitespace = true,
                NameTable = CreateAtomNameTable()
            };
            if (encoding == null)
            {
                return XmlReader.Create(stream, settings);
            }
            return XmlReader.Create(new StreamReader(stream, encoding), settings);
        }

        internal static XmlWriter CreateXmlWriterAndWriteProcessingInstruction(Stream stream, Encoding encoding)
        {
            XmlWriterSettings settings = CreateXmlWriterSettings(encoding);
            XmlWriter writer = XmlWriter.Create(stream, settings);
            writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"" + encoding.WebName + "\" standalone=\"yes\"");
            return writer;
        }

        internal static XmlWriterSettings CreateXmlWriterSettings(Encoding encoding) => 
            new XmlWriterSettings { 
                CheckCharacters = false,
                ConformanceLevel = ConformanceLevel.Fragment,
                Encoding = encoding,
                Indent = true,
                NewLineHandling = NewLineHandling.Entitize
            };

        internal static string GetAttributeEx(this XmlReader reader, string attributeName, string namespaceUri) => 
            (reader.GetAttribute(attributeName, namespaceUri) ?? reader.GetAttribute(attributeName));

        internal static void RemoveDuplicateNamespaceAttributes(XElement element)
        {
            HashSet<string> set = new HashSet<string>(EqualityComparer<string>.Default);
            foreach (XElement element2 in element.DescendantsAndSelf())
            {
                bool flag = false;
                foreach (XAttribute attribute in element2.Attributes())
                {
                    if (!flag)
                    {
                        flag = true;
                        set.Clear();
                    }
                    if (attribute.IsNamespaceDeclaration)
                    {
                        string localName = attribute.Name.LocalName;
                        if (!set.Add(localName))
                        {
                            attribute.Remove();
                        }
                    }
                }
            }
        }
    }
}

