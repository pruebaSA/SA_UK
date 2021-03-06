﻿namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Xml;

    internal class KeywordsTable
    {
        public string ApplyImports;
        public string ApplyTemplates;
        public string Assembly;
        public string Attribute;
        public string AttributeSet;
        public string CallTemplate;
        public string CaseOrder;
        public string CDataSectionElements;
        public string Choose;
        public string Comment;
        public string Copy;
        public string CopyOf;
        public string Count;
        public string DataType;
        public string DecimalFormat;
        public string DecimalSeparator;
        public string Digit;
        public string DisableOutputEscaping;
        public string DocTypePublic;
        public string DocTypeSystem;
        public string Element;
        public string Elements;
        public string Encoding;
        public string ExcludeResultPrefixes;
        public string ExtensionElementPrefixes;
        public string Fallback;
        public string ForEach;
        public string Format;
        public string From;
        public string GroupingSeparator;
        public string GroupingSize;
        public string Href;
        public string Id;
        public string If;
        public string ImplementsPrefix;
        public string Import;
        public string Include;
        public string Indent;
        public string Infinity;
        public string Key;
        public string Lang;
        public string Language;
        public string LetterValue;
        public string Level;
        public string Match;
        public string MediaType;
        public string Message;
        public string Method;
        public string MinusSign;
        public string Mode;
        public string Name;
        public string Namespace;
        public string NamespaceAlias;
        public string NaN;
        public string Number;
        public string OmitXmlDeclaration;
        public string Order;
        public string Otherwise;
        public string Output;
        public string Param;
        public string PatternSeparator;
        public string Percent;
        public string PerMille;
        public string PreserveSpace;
        public string Priority;
        public string ProcessingInstruction;
        public string ResultPrefix;
        public string Script;
        public string Select;
        public string Sort;
        public string Space;
        public string Standalone;
        public string StripSpace;
        public string Stylesheet;
        public string StylesheetPrefix;
        public string Template;
        public string Terminate;
        public string Test;
        public string Text;
        public string Transform;
        public string UriWdXsl;
        public string UriXml;
        public string UriXsl;
        public string UrnMsxsl;
        public string Use;
        public string UseAttributeSets;
        public string Using;
        public string Value;
        public string ValueOf;
        public string Variable;
        public string Version;
        public string When;
        public string WithParam;
        public string Xml;
        public string ZeroDigit;

        public KeywordsTable(XmlNameTable nt)
        {
            this.ApplyImports = nt.Add("apply-imports");
            this.ApplyTemplates = nt.Add("apply-templates");
            this.Assembly = nt.Add("assembly");
            this.Attribute = nt.Add("attribute");
            this.AttributeSet = nt.Add("attribute-set");
            this.CallTemplate = nt.Add("call-template");
            this.CaseOrder = nt.Add("case-order");
            this.CDataSectionElements = nt.Add("cdata-section-elements");
            this.Choose = nt.Add("choose");
            this.Comment = nt.Add("comment");
            this.Copy = nt.Add("copy");
            this.CopyOf = nt.Add("copy-of");
            this.Count = nt.Add("count");
            this.DataType = nt.Add("data-type");
            this.DecimalFormat = nt.Add("decimal-format");
            this.DecimalSeparator = nt.Add("decimal-separator");
            this.Digit = nt.Add("digit");
            this.DisableOutputEscaping = nt.Add("disable-output-escaping");
            this.DocTypePublic = nt.Add("doctype-public");
            this.DocTypeSystem = nt.Add("doctype-system");
            this.Element = nt.Add("element");
            this.Elements = nt.Add("elements");
            this.Encoding = nt.Add("encoding");
            this.ExcludeResultPrefixes = nt.Add("exclude-result-prefixes");
            this.ExtensionElementPrefixes = nt.Add("extension-element-prefixes");
            this.Fallback = nt.Add("fallback");
            this.ForEach = nt.Add("for-each");
            this.Format = nt.Add("format");
            this.From = nt.Add("from");
            this.GroupingSeparator = nt.Add("grouping-separator");
            this.GroupingSize = nt.Add("grouping-size");
            this.Href = nt.Add("href");
            this.Id = nt.Add("id");
            this.If = nt.Add("if");
            this.ImplementsPrefix = nt.Add("implements-prefix");
            this.Import = nt.Add("import");
            this.Include = nt.Add("include");
            this.Indent = nt.Add("indent");
            this.Infinity = nt.Add("infinity");
            this.Key = nt.Add("key");
            this.Lang = nt.Add("lang");
            this.Language = nt.Add("language");
            this.LetterValue = nt.Add("letter-value");
            this.Level = nt.Add("level");
            this.Match = nt.Add("match");
            this.MediaType = nt.Add("media-type");
            this.Message = nt.Add("message");
            this.Method = nt.Add("method");
            this.MinusSign = nt.Add("minus-sign");
            this.Mode = nt.Add("mode");
            this.Name = nt.Add("name");
            this.Namespace = nt.Add("namespace");
            this.NamespaceAlias = nt.Add("namespace-alias");
            this.NaN = nt.Add("NaN");
            this.Number = nt.Add("number");
            this.OmitXmlDeclaration = nt.Add("omit-xml-declaration");
            this.Otherwise = nt.Add("otherwise");
            this.Order = nt.Add("order");
            this.Output = nt.Add("output");
            this.Param = nt.Add("param");
            this.PatternSeparator = nt.Add("pattern-separator");
            this.Percent = nt.Add("percent");
            this.PerMille = nt.Add("per-mille");
            this.PreserveSpace = nt.Add("preserve-space");
            this.Priority = nt.Add("priority");
            this.ProcessingInstruction = nt.Add("processing-instruction");
            this.ResultPrefix = nt.Add("result-prefix");
            this.Script = nt.Add("script");
            this.Select = nt.Add("select");
            this.Sort = nt.Add("sort");
            this.Space = nt.Add("space");
            this.Standalone = nt.Add("standalone");
            this.StripSpace = nt.Add("strip-space");
            this.Stylesheet = nt.Add("stylesheet");
            this.StylesheetPrefix = nt.Add("stylesheet-prefix");
            this.Template = nt.Add("template");
            this.Terminate = nt.Add("terminate");
            this.Test = nt.Add("test");
            this.Text = nt.Add("text");
            this.Transform = nt.Add("transform");
            this.UrnMsxsl = nt.Add("urn:schemas-microsoft-com:xslt");
            this.UriXml = nt.Add("http://www.w3.org/XML/1998/namespace");
            this.UriXsl = nt.Add("http://www.w3.org/1999/XSL/Transform");
            this.UriWdXsl = nt.Add("http://www.w3.org/TR/WD-xsl");
            this.Use = nt.Add("use");
            this.UseAttributeSets = nt.Add("use-attribute-sets");
            this.Using = nt.Add("using");
            this.Value = nt.Add("value");
            this.ValueOf = nt.Add("value-of");
            this.Variable = nt.Add("variable");
            this.Version = nt.Add("version");
            this.When = nt.Add("when");
            this.WithParam = nt.Add("with-param");
            this.Xml = nt.Add("xml");
            this.ZeroDigit = nt.Add("zero-digit");
        }
    }
}

