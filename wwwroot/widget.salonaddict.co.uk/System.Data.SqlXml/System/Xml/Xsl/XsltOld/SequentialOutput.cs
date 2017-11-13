namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text;
    using System.Xml;
    using System.Xml.Xsl;

    internal abstract class SequentialOutput : RecordOutput
    {
        private byte[] byteBuffer;
        private Hashtable cdataElements;
        protected Encoding encoding;
        private bool firstLine = true;
        private bool indentOutput;
        private bool isHtmlOutput;
        private bool isXmlOutput;
        private bool omitXmlDeclCalled;
        private XsltOutput output;
        private ArrayList outputCache;
        private bool outputDoctype;
        private bool outputXmlDecl;
        private Processor processor;
        private const char s_Ampersand = '&';
        private const string s_CDataBegin = "<![CDATA[";
        private const string s_CDataEnd = "]]>";
        private const string s_CDataSplit = "]]]]><![CDATA[>";
        private const char s_Colon = ':';
        private const string s_CommentBegin = "<!--";
        private const string s_CommentEnd = "-->";
        private const string s_DocType = "<!DOCTYPE ";
        private const string s_EnAmpersand = "&amp;";
        private const string s_EncodingStart = " encoding=\"";
        private const string s_EndOfLine = "\r\n";
        private const string s_EnGreaterThan = "&gt;";
        private const string s_EnLessThan = "&lt;";
        private const string s_EnNewLine = "&#xA;";
        private const string s_EnQuote = "&quot;";
        private const string s_EnReturn = "&#xD;";
        private const string s_EqualQuote = "=\"";
        private const char s_GreaterThan = '>';
        private const string s_Html = "html";
        private const char s_LessThan = '<';
        private const string s_LessThanQuestion = "<?";
        private const string s_LessThanSlash = "</";
        private const char s_NewLine = '\n';
        private const string s_Public = "PUBLIC ";
        private const string s_QuestionGreaterThan = "?>";
        private const char s_Quote = '"';
        private const string s_QuoteSpace = "\" ";
        private const char s_Return = '\r';
        private const char s_Semicolon = ';';
        private const string s_SlashGreaterThan = " />";
        private const char s_Space = ' ';
        private const string s_Standalone = " standalone=\"";
        private const string s_System = "SYSTEM ";
        private static char[] s_TextValueFind = new char[] { '&', '>', '<' };
        private static string[] s_TextValueReplace = new string[] { "&amp;", "&gt;", "&lt;" };
        private const string s_VersionAll = " version=\"1.0\"";
        private static char[] s_XmlAttributeValueFind = new char[] { '&', '>', '<', '"', '\n', '\r' };
        private static string[] s_XmlAttributeValueReplace = new string[] { "&amp;", "&gt;", "&lt;", "&quot;", "&#xA;", "&#xD;" };
        private bool secondRoot;
        private Encoding utf8Encoding;
        private XmlCharType xmlCharType = XmlCharType.Instance;

        internal SequentialOutput(Processor processor)
        {
            this.processor = processor;
            this.CacheOuptutProps(processor.Output);
        }

        private void CacheOuptutProps(XsltOutput output)
        {
            this.output = output;
            this.isXmlOutput = this.output.Method == XsltOutput.OutputMethod.Xml;
            this.isHtmlOutput = this.output.Method == XsltOutput.OutputMethod.Html;
            this.cdataElements = this.output.CDataElements;
            this.indentOutput = this.output.Indent;
            this.outputDoctype = (this.output.DoctypeSystem != null) || (this.isHtmlOutput && (this.output.DoctypePublic != null));
            this.outputXmlDecl = (this.isXmlOutput && !this.output.OmitXmlDeclaration) && !this.omitXmlDeclCalled;
        }

        private void CacheRecord(RecordBuilder record)
        {
            if (this.outputCache == null)
            {
                this.outputCache = new ArrayList();
            }
            this.outputCache.Add(record.MainNode.Clone());
        }

        internal abstract void Close();
        private bool DecideDefaultOutput(BuilderInfo node)
        {
            XsltOutput.OutputMethod xml = XsltOutput.OutputMethod.Xml;
            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    if ((node.NamespaceURI.Length == 0) && (string.Compare("html", node.LocalName, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        xml = XsltOutput.OutputMethod.Html;
                    }
                    break;

                case XmlNodeType.Text:
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    if (this.xmlCharType.IsOnlyWhitespace(node.Value))
                    {
                        return false;
                    }
                    xml = XsltOutput.OutputMethod.Xml;
                    break;

                default:
                    return false;
            }
            if (this.processor.SetDefaultOutput(xml))
            {
                this.CacheOuptutProps(this.processor.Output);
            }
            return true;
        }

        private void Indent(int depth)
        {
            if (this.firstLine)
            {
                if (this.indentOutput)
                {
                    this.firstLine = false;
                }
            }
            else
            {
                this.Write("\r\n");
                for (int i = 2 * depth; 0 < i; i--)
                {
                    this.Write(" ");
                }
            }
        }

        private void Indent(RecordBuilder record)
        {
            if (!record.Manager.CurrentElementScope.Mixed)
            {
                this.Indent(record.MainNode.Depth);
            }
        }

        public void OmitXmlDecl()
        {
            this.omitXmlDeclCalled = true;
            this.outputXmlDecl = false;
        }

        private void OutputCachedRecords()
        {
            if (this.outputCache != null)
            {
                for (int i = 0; i < this.outputCache.Count; i++)
                {
                    BuilderInfo node = (BuilderInfo) this.outputCache[i];
                    this.OutputRecord(node);
                }
                this.outputCache = null;
            }
        }

        private void OutputRecord(BuilderInfo node)
        {
            if (this.outputXmlDecl)
            {
                this.WriteXmlDeclaration();
            }
            this.Indent(0);
            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                case XmlNodeType.Attribute:
                case XmlNodeType.CDATA:
                case XmlNodeType.Entity:
                case XmlNodeType.Document:
                case XmlNodeType.DocumentFragment:
                case XmlNodeType.Notation:
                case XmlNodeType.EndElement:
                    break;

                case XmlNodeType.Text:
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    this.WriteTextNode(node);
                    return;

                case XmlNodeType.EntityReference:
                    this.Write('&');
                    this.WriteName(node.Prefix, node.LocalName);
                    this.Write(';');
                    return;

                case XmlNodeType.ProcessingInstruction:
                    this.WriteProcessingInstruction(node);
                    return;

                case XmlNodeType.Comment:
                    this.Write("<!--");
                    this.Write(node.Value);
                    this.Write("-->");
                    return;

                case XmlNodeType.DocumentType:
                    this.Write(node.Value);
                    break;

                default:
                    return;
            }
        }

        private void OutputRecord(RecordBuilder record)
        {
            BuilderInfo mainNode = record.MainNode;
            if (this.outputXmlDecl)
            {
                this.WriteXmlDeclaration();
            }
            switch (mainNode.NodeType)
            {
                case XmlNodeType.Element:
                    this.WriteStartElement(record);
                    return;

                case XmlNodeType.Attribute:
                case XmlNodeType.CDATA:
                case XmlNodeType.Entity:
                case XmlNodeType.Document:
                case XmlNodeType.DocumentFragment:
                case XmlNodeType.Notation:
                    break;

                case XmlNodeType.Text:
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    this.WriteTextNode(record);
                    return;

                case XmlNodeType.EntityReference:
                    this.Write('&');
                    this.WriteName(mainNode.Prefix, mainNode.LocalName);
                    this.Write(';');
                    return;

                case XmlNodeType.ProcessingInstruction:
                    this.WriteProcessingInstruction(record);
                    return;

                case XmlNodeType.Comment:
                    this.Indent(record);
                    this.Write("<!--");
                    this.Write(mainNode.Value);
                    this.Write("-->");
                    return;

                case XmlNodeType.DocumentType:
                    this.Write(mainNode.Value);
                    return;

                case XmlNodeType.EndElement:
                    this.WriteEndElement(record);
                    break;

                default:
                    return;
            }
        }

        public Processor.OutputResult RecordDone(RecordBuilder record)
        {
            if (this.output.Method == XsltOutput.OutputMethod.Unknown)
            {
                if (!this.DecideDefaultOutput(record.MainNode))
                {
                    this.CacheRecord(record);
                }
                else
                {
                    this.OutputCachedRecords();
                    this.OutputRecord(record);
                }
            }
            else
            {
                this.OutputRecord(record);
            }
            record.Reset();
            return Processor.OutputResult.Continue;
        }

        public void TheEnd()
        {
            this.OutputCachedRecords();
            this.Close();
        }

        internal abstract void Write(char outputChar);
        internal abstract void Write(string outputText);
        private void WriteAttributes(ArrayList list, int count, HtmlElementProps htmlElementsProps)
        {
            for (int i = 0; i < count; i++)
            {
                BuilderInfo info = (BuilderInfo) list[i];
                string strB = info.Value;
                bool flag = false;
                bool flag2 = false;
                if ((htmlElementsProps != null) && (info.Prefix.Length == 0))
                {
                    HtmlAttributeProps htmlAttrProps = info.htmlAttrProps;
                    if ((htmlAttrProps == null) && info.search)
                    {
                        htmlAttrProps = HtmlAttributeProps.GetProps(info.LocalName);
                    }
                    if (htmlAttrProps != null)
                    {
                        flag = htmlElementsProps.AbrParent && htmlAttrProps.Abr;
                        flag2 = htmlElementsProps.UriParent && (htmlAttrProps.Uri || (htmlElementsProps.NameParent && htmlAttrProps.Name));
                    }
                }
                this.Write(' ');
                this.WriteName(info.Prefix, info.LocalName);
                if (!flag || (string.Compare(info.LocalName, strB, StringComparison.OrdinalIgnoreCase) != 0))
                {
                    this.Write("=\"");
                    if (flag2)
                    {
                        this.WriteHtmlUri(strB);
                    }
                    else if (this.isHtmlOutput)
                    {
                        this.WriteHtmlAttributeValue(strB);
                    }
                    else
                    {
                        this.WriteXmlAttributeValue(strB);
                    }
                    this.Write('"');
                }
            }
        }

        private void WriteCData(string value)
        {
            this.Write(value.Replace("]]>", "]]]]><![CDATA[>"));
        }

        private void WriteCDataSection(string value)
        {
            this.Write("<![CDATA[");
            this.WriteCData(value);
            this.Write("]]>");
        }

        private void WriteDoctype(BuilderInfo mainNode)
        {
            this.Indent(0);
            this.Write("<!DOCTYPE ");
            if (this.isXmlOutput)
            {
                this.WriteName(mainNode.Prefix, mainNode.LocalName);
            }
            else
            {
                this.WriteName(string.Empty, "html");
            }
            this.Write(' ');
            if (this.output.DoctypePublic != null)
            {
                this.Write("PUBLIC ");
                this.Write('"');
                this.Write(this.output.DoctypePublic);
                this.Write("\" ");
            }
            else
            {
                this.Write("SYSTEM ");
            }
            if (this.output.DoctypeSystem != null)
            {
                this.Write('"');
                this.Write(this.output.DoctypeSystem);
                this.Write('"');
            }
            this.Write('>');
        }

        private void WriteEndElement(RecordBuilder record)
        {
            BuilderInfo mainNode = record.MainNode;
            HtmlElementProps htmlElementProps = record.Manager.CurrentElementScope.HtmlElementProps;
            if ((htmlElementProps == null) || !htmlElementProps.Empty)
            {
                this.Indent(record);
                this.Write("</");
                this.WriteName(record.MainNode.Prefix, record.MainNode.LocalName);
                this.Write('>');
            }
        }

        private void WriteHtmlAttributeValue(string value)
        {
            int length = value.Length;
            int num2 = 0;
            while (num2 < length)
            {
                char outputChar = value[num2];
                num2++;
                switch (outputChar)
                {
                    case '"':
                    {
                        this.Write("&quot;");
                        continue;
                    }
                    case '&':
                    {
                        if ((num2 != length) && (value[num2] == '{'))
                        {
                            this.Write(outputChar);
                        }
                        else
                        {
                            this.Write("&amp;");
                        }
                        continue;
                    }
                }
                this.Write(outputChar);
            }
        }

        private void WriteHtmlUri(string value)
        {
            int length = value.Length;
            int num2 = 0;
            while (num2 < length)
            {
                char outputChar = value[num2];
                num2++;
                char ch2 = outputChar;
                if (ch2 <= '\r')
                {
                    switch (ch2)
                    {
                        case '\n':
                            goto Label_0078;

                        case '\r':
                            goto Label_0088;
                    }
                    goto Label_0098;
                }
                if (ch2 == '"')
                {
                    this.Write("&quot;");
                }
                else
                {
                    if (ch2 != '&')
                    {
                        goto Label_0098;
                    }
                    if ((num2 != length) && (value[num2] == '{'))
                    {
                        this.Write(outputChar);
                    }
                    else
                    {
                        this.Write("&amp;");
                    }
                }
                continue;
            Label_0078:
                this.Write("&#xA;");
                continue;
            Label_0088:
                this.Write("&#xD;");
                continue;
            Label_0098:
                if ('\x007f' < outputChar)
                {
                    if (this.utf8Encoding == null)
                    {
                        this.utf8Encoding = Encoding.UTF8;
                        this.byteBuffer = new byte[this.utf8Encoding.GetMaxByteCount(1)];
                    }
                    int num3 = this.utf8Encoding.GetBytes(value, num2 - 1, 1, this.byteBuffer, 0);
                    for (int i = 0; i < num3; i++)
                    {
                        this.Write("%");
                        this.Write(((uint) this.byteBuffer[i]).ToString("X2", CultureInfo.InvariantCulture));
                    }
                }
                else
                {
                    this.Write(outputChar);
                }
            }
        }

        private void WriteName(string prefix, string name)
        {
            if ((prefix != null) && (prefix.Length > 0))
            {
                this.Write(prefix);
                if ((name == null) || (name.Length <= 0))
                {
                    return;
                }
                this.Write(':');
            }
            this.Write(name);
        }

        private void WriteProcessingInstruction(BuilderInfo node)
        {
            this.Write("<?");
            this.WriteName(node.Prefix, node.LocalName);
            this.Write(' ');
            this.Write(node.Value);
            if (this.isHtmlOutput)
            {
                this.Write('>');
            }
            else
            {
                this.Write("?>");
            }
        }

        private void WriteProcessingInstruction(RecordBuilder record)
        {
            this.Indent(record);
            this.WriteProcessingInstruction(record.MainNode);
        }

        private void WriteStartElement(RecordBuilder record)
        {
            BuilderInfo mainNode = record.MainNode;
            HtmlElementProps htmlElementsProps = null;
            if (this.isHtmlOutput)
            {
                if (mainNode.Prefix.Length == 0)
                {
                    htmlElementsProps = mainNode.htmlProps;
                    if ((htmlElementsProps == null) && mainNode.search)
                    {
                        htmlElementsProps = HtmlElementProps.GetProps(mainNode.LocalName);
                    }
                    record.Manager.CurrentElementScope.HtmlElementProps = htmlElementsProps;
                    mainNode.IsEmptyTag = false;
                }
            }
            else if (this.isXmlOutput && (mainNode.Depth == 0))
            {
                if (this.secondRoot && ((this.output.DoctypeSystem != null) || this.output.Standalone))
                {
                    throw XsltException.Create("Xslt_MultipleRoots", new string[0]);
                }
                this.secondRoot = true;
            }
            if (this.outputDoctype)
            {
                this.WriteDoctype(mainNode);
                this.outputDoctype = false;
            }
            if (((this.cdataElements != null) && this.cdataElements.Contains(new XmlQualifiedName(mainNode.LocalName, mainNode.NamespaceURI))) && this.isXmlOutput)
            {
                record.Manager.CurrentElementScope.ToCData = true;
            }
            this.Indent(record);
            this.Write('<');
            this.WriteName(mainNode.Prefix, mainNode.LocalName);
            this.WriteAttributes(record.AttributeList, record.AttributeCount, htmlElementsProps);
            if (mainNode.IsEmptyTag)
            {
                this.Write(" />");
            }
            else
            {
                this.Write('>');
            }
            if ((htmlElementsProps != null) && htmlElementsProps.Head)
            {
                mainNode.Depth++;
                this.Indent(record);
                mainNode.Depth--;
                this.Write("<META http-equiv=\"Content-Type\" content=\"");
                this.Write(this.output.MediaType);
                this.Write("; charset=");
                this.Write(this.encoding.WebName);
                this.Write("\">");
            }
        }

        private void WriteTextNode(BuilderInfo node)
        {
            for (int i = 0; i < node.TextInfoCount; i++)
            {
                string str = node.TextInfo[i];
                if (str == null)
                {
                    i++;
                    this.Write(node.TextInfo[i]);
                }
                else
                {
                    this.WriteWithReplace(str, s_TextValueFind, s_TextValueReplace);
                }
            }
        }

        private void WriteTextNode(RecordBuilder record)
        {
            BuilderInfo mainNode = record.MainNode;
            OutputScope currentElementScope = record.Manager.CurrentElementScope;
            currentElementScope.Mixed = true;
            if ((currentElementScope.HtmlElementProps != null) && currentElementScope.HtmlElementProps.NoEntities)
            {
                this.Write(mainNode.Value);
            }
            else if (currentElementScope.ToCData)
            {
                this.WriteCDataSection(mainNode.Value);
            }
            else
            {
                this.WriteTextNode(mainNode);
            }
        }

        private void WriteWithReplace(string value, char[] find, string[] replace)
        {
            int length = value.Length;
            int startIndex = 0;
            while (startIndex < length)
            {
                int num3 = value.IndexOfAny(find, startIndex);
                if (num3 != -1)
                {
                    goto Label_002B;
                }
                break;
            Label_001A:
                this.Write(value[startIndex]);
                startIndex++;
            Label_002B:
                if (startIndex < num3)
                {
                    goto Label_001A;
                }
                char ch = value[startIndex];
                for (int i = find.Length - 1; 0 <= i; i--)
                {
                    if (find[i] == ch)
                    {
                        this.Write(replace[i]);
                        break;
                    }
                }
                startIndex++;
            }
            if (startIndex == 0)
            {
                this.Write(value);
            }
            else
            {
                while (startIndex < length)
                {
                    this.Write(value[startIndex]);
                    startIndex++;
                }
            }
        }

        private void WriteXmlAttributeValue(string value)
        {
            this.WriteWithReplace(value, s_XmlAttributeValueFind, s_XmlAttributeValueReplace);
        }

        private void WriteXmlDeclaration()
        {
            this.outputXmlDecl = false;
            this.Indent(0);
            this.Write("<?");
            this.WriteName(string.Empty, "xml");
            this.Write(" version=\"1.0\"");
            if (this.encoding != null)
            {
                this.Write(" encoding=\"");
                this.Write(this.encoding.WebName);
                this.Write('"');
            }
            if (this.output.HasStandalone)
            {
                this.Write(" standalone=\"");
                this.Write(this.output.Standalone ? "yes" : "no");
                this.Write('"');
            }
            this.Write("?>");
        }
    }
}

