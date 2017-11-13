﻿namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;
    using System.Xml;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class XmlBuilder : ControlBuilder
    {
        public override void AppendLiteralString(string s)
        {
        }

        public override Type GetChildControlType(string tagName, IDictionary attribs) => 
            null;

        public override bool NeedsTagInnerText() => 
            true;

        public override void SetTagInnerText(string text)
        {
            if (!Util.IsWhiteSpaceString(text))
            {
                int startIndex = Util.FirstNonWhiteSpaceIndex(text);
                string s = text.Substring(startIndex);
                base.Line += Util.LineCount(text, 0, startIndex);
                XmlDocument document = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings {
                    LineNumberOffset = base.Line - 1,
                    ProhibitDtd = false,
                    CheckCharacters = false
                };
                XmlReader reader = XmlReader.Create(new StringReader(s), settings, string.Empty);
                try
                {
                    document.Load(reader);
                }
                catch (XmlException exception)
                {
                    if (exception.LineNumber >= 0)
                    {
                        base.Line = exception.LineNumber;
                    }
                    throw;
                }
                base.AppendLiteralString(text);
            }
        }
    }
}

