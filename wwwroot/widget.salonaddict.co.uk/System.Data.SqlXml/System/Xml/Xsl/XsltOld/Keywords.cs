namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Diagnostics;
    using System.Xml;

    internal class Keywords
    {
        private string _AtomApplyImports;
        private string _AtomApplyTemplates;
        private string _AtomAttribute;
        private string _AtomAttributeSet;
        private string _AtomCallTemplate;
        private string _AtomCaseOrder;
        private string _AtomCdataSectionElements;
        private string _AtomChoose;
        private string _AtomComment;
        private string _AtomCopy;
        private string _AtomCopyOf;
        private string _AtomCount;
        private string _AtomDataType;
        private string _AtomDecimalFormat;
        private string _AtomDecimalSeparator;
        private string _AtomDigit;
        private string _AtomDisableOutputEscaping;
        private string _AtomDoctypePublic;
        private string _AtomDoctypeSystem;
        private string _AtomElement;
        private string _AtomElements;
        private string _AtomEmpty;
        private string _AtomEncoding;
        private string _AtomExcludeResultPrefixes;
        private string _AtomExtensionElementPrefixes;
        private string _AtomFallback;
        private string _AtomForEach;
        private string _AtomFormat;
        private string _AtomFrom;
        private string _AtomGroupingSeparator;
        private string _AtomGroupingSize;
        private string _AtomHashDefault;
        private string _AtomHref;
        private string _AtomId;
        private string _AtomIf;
        private string _AtomImplementsPrefix;
        private string _AtomImport;
        private string _AtomInclude;
        private string _AtomIndent;
        private string _AtomInfinity;
        private string _AtomKey;
        private string _AtomLang;
        private string _AtomLanguage;
        private string _AtomLetterValue;
        private string _AtomLevel;
        private string _AtomMatch;
        private string _AtomMediaType;
        private string _AtomMessage;
        private string _AtomMethod;
        private string _AtomMinusSign;
        private string _AtomMode;
        private string _AtomMsXsltNamespace;
        private string _AtomName;
        private string _AtomNamespace;
        private string _AtomNamespaceAlias;
        private string _AtomNaN;
        private string _AtomNo;
        private string _AtomNumber;
        private string _AtomOmitXmlDeclaration;
        private string _AtomOrder;
        private string _AtomOtherwise;
        private string _AtomOutput;
        private string _AtomParam;
        private string _AtomPatternSeparator;
        private string _AtomPercent;
        private string _AtomPerMille;
        private string _AtomPreserveSpace;
        private string _AtomPriority;
        private string _AtomProcessingInstruction;
        private string _AtomResultPrefix;
        private string _AtomScript;
        private string _AtomSelect;
        private string _AtomSort;
        private string _AtomStandalone;
        private string _AtomStripSpace;
        private string _AtomStylesheet;
        private string _AtomStylesheetPrefix;
        private string _AtomTemplate;
        private string _AtomTerminate;
        private string _AtomTest;
        private string _AtomText;
        private string _AtomTransform;
        private string _AtomUse;
        private string _AtomUseAttributeSets;
        private string _AtomValue;
        private string _AtomValueOf;
        private string _AtomVariable;
        private string _AtomVersion;
        private string _AtomWhen;
        private string _AtomWithParam;
        private string _AtomXsltNamespace;
        private string _AtomYes;
        private string _AtomZeroDigit;
        private XmlNameTable _NameTable;
        internal const string s_Alphabetic = "alphabetic";
        internal const string s_Any = "any";
        internal const string s_ApplyImports = "apply-imports";
        internal const string s_ApplyTemplates = "apply-templates";
        internal const string s_Ascending = "ascending";
        internal const string s_Attribute = "attribute";
        internal const string s_AttributeSet = "attribute-set";
        internal const string s_CallTemplate = "call-template";
        internal const string s_CaseOrder = "case-order";
        internal const string s_CdataSectionElements = "cdata-section-elements";
        internal const string s_Choose = "choose";
        internal const string s_Comment = "comment";
        internal const string s_Copy = "copy";
        internal const string s_CopyOf = "copy-of";
        internal const string s_Count = "count";
        internal const string s_DataType = "data-type";
        internal const string s_DecimalFormat = "decimal-format";
        internal const string s_DecimalSeparator = "decimal-separator";
        internal const string s_Descending = "descending";
        internal const string s_Digit = "digit";
        internal const string s_DisableOutputEscaping = "disable-output-escaping";
        internal const string s_DoctypePublic = "doctype-public";
        internal const string s_DoctypeSystem = "doctype-system";
        internal const string s_Element = "element";
        internal const string s_Elements = "elements";
        internal const string s_Encoding = "encoding";
        internal const string s_ExcludeResultPrefixes = "exclude-result-prefixes";
        internal const string s_ExtensionElementPrefixes = "extension-element-prefixes";
        internal const string s_Fallback = "fallback";
        internal const string s_ForEach = "for-each";
        internal const string s_Format = "format";
        internal const string s_From = "from";
        internal const string s_GroupingSeparator = "grouping-separator";
        internal const string s_GroupingSize = "grouping-size";
        internal const string s_HashDefault = "#default";
        internal const string s_Href = "href";
        internal const string s_Html = "html";
        internal const string s_Id = "id";
        internal const string s_If = "if";
        internal const string s_ImplementsPrefix = "implements-prefix";
        internal const string s_Import = "import";
        internal const string s_Include = "include";
        internal const string s_Indent = "indent";
        internal const string s_Infinity = "infinity";
        internal const string s_Key = "key";
        internal const string s_Lang = "lang";
        internal const string s_Language = "language";
        internal const string s_LetterValue = "letter-value";
        internal const string s_Level = "level";
        internal const string s_LowerFirst = "lower-first";
        internal const string s_Match = "match";
        internal const string s_MediaType = "media-type";
        internal const string s_Message = "message";
        internal const string s_Method = "method";
        internal const string s_MinusSign = "minus-sign";
        internal const string s_Mode = "mode";
        internal const string s_MsXsltNamespace = "urn:schemas-microsoft-com:xslt";
        internal const string s_Multiple = "multiple";
        internal const string s_Name = "name";
        internal const string s_Namespace = "namespace";
        internal const string s_NamespaceAlias = "namespace-alias";
        internal const string s_NaN = "NaN";
        internal const string s_No = "no";
        internal const string s_Number = "number";
        internal const string s_OmitXmlDeclaration = "omit-xml-declaration";
        internal const string s_Order = "order";
        internal const string s_Otherwise = "otherwise";
        internal const string s_Output = "output";
        internal const string s_Param = "param";
        internal const string s_PatternSeparator = "pattern-separator";
        internal const string s_Percent = "percent";
        internal const string s_PerMille = "per-mille";
        internal const string s_PreserveSpace = "preserve-space";
        internal const string s_Priority = "priority";
        internal const string s_ProcessingInstruction = "processing-instruction";
        internal const string s_ResultPrefix = "result-prefix";
        internal const string s_Script = "script";
        internal const string s_Select = "select";
        internal const string s_Single = "single";
        internal const string s_Sort = "sort";
        internal const string s_Space = "space";
        internal const string s_Standalone = "standalone";
        internal const string s_StripSpace = "strip-space";
        internal const string s_Stylesheet = "stylesheet";
        internal const string s_StylesheetPrefix = "stylesheet-prefix";
        internal const string s_Template = "template";
        internal const string s_Terminate = "terminate";
        internal const string s_Test = "test";
        internal const string s_Text = "text";
        internal const string s_Traditional = "traditional";
        internal const string s_Transform = "transform";
        internal const string s_UpperFirst = "upper-first";
        internal const string s_Use = "use";
        internal const string s_UseAttributeSets = "use-attribute-sets";
        internal const string s_Value = "value";
        internal const string s_ValueOf = "value-of";
        internal const string s_Variable = "variable";
        internal const string s_Vendor = "vendor";
        internal const string s_VendorUrl = "vendor-url";
        internal const string s_Version = "version";
        internal const string s_Version10 = "1.0";
        internal const string s_WdXslNamespace = "http://www.w3.org/TR/WD-xsl";
        internal const string s_When = "when";
        internal const string s_WithParam = "with-param";
        internal const string s_Xml = "xml";
        internal const string s_XmlNamespace = "http://www.w3.org/XML/1998/namespace";
        internal const string s_Xmlns = "xmlns";
        internal const string s_XmlnsNamespace = "http://www.w3.org/2000/xmlns/";
        internal const string s_XsltNamespace = "http://www.w3.org/1999/XSL/Transform";
        internal const string s_Yes = "yes";
        internal const string s_ZeroDigit = "zero-digit";

        internal Keywords(XmlNameTable nameTable)
        {
            this._NameTable = nameTable;
        }

        [Conditional("DEBUG")]
        private void CheckKeyword(string keyword)
        {
        }

        internal static bool Compare(string strA, string strB) => 
            string.Equals(strA, strB);

        internal static bool Equals(string strA, string strB) => 
            (strA == strB);

        internal void LookupKeywords()
        {
            this._AtomEmpty = this._NameTable.Add(string.Empty);
            this._AtomXsltNamespace = this._NameTable.Add("http://www.w3.org/1999/XSL/Transform");
            this._AtomApplyTemplates = this._NameTable.Add("apply-templates");
            this._AtomChoose = this._NameTable.Add("choose");
            this._AtomForEach = this._NameTable.Add("for-each");
            this._AtomIf = this._NameTable.Add("if");
            this._AtomOtherwise = this._NameTable.Add("otherwise");
            this._AtomStylesheet = this._NameTable.Add("stylesheet");
            this._AtomTemplate = this._NameTable.Add("template");
            this._AtomTransform = this._NameTable.Add("transform");
            this._AtomValueOf = this._NameTable.Add("value-of");
            this._AtomWhen = this._NameTable.Add("when");
            this._AtomMatch = this._NameTable.Add("match");
            this._AtomName = this._NameTable.Add("name");
            this._AtomSelect = this._NameTable.Add("select");
            this._AtomTest = this._NameTable.Add("test");
            this._AtomMsXsltNamespace = this._NameTable.Add("urn:schemas-microsoft-com:xslt");
            this._AtomScript = this._NameTable.Add("script");
        }

        internal string ApplyImports
        {
            get
            {
                if (this._AtomApplyImports == null)
                {
                    this._AtomApplyImports = this._NameTable.Add("apply-imports");
                }
                return this._AtomApplyImports;
            }
        }

        internal string ApplyTemplates =>
            this._AtomApplyTemplates;

        internal string Attribute
        {
            get
            {
                if (this._AtomAttribute == null)
                {
                    this._AtomAttribute = this._NameTable.Add("attribute");
                }
                return this._AtomAttribute;
            }
        }

        internal string AttributeSet
        {
            get
            {
                if (this._AtomAttributeSet == null)
                {
                    this._AtomAttributeSet = this._NameTable.Add("attribute-set");
                }
                return this._AtomAttributeSet;
            }
        }

        internal string CallTemplate
        {
            get
            {
                if (this._AtomCallTemplate == null)
                {
                    this._AtomCallTemplate = this._NameTable.Add("call-template");
                }
                return this._AtomCallTemplate;
            }
        }

        internal string CaseOrder
        {
            get
            {
                if (this._AtomCaseOrder == null)
                {
                    this._AtomCaseOrder = this._NameTable.Add("case-order");
                }
                return this._AtomCaseOrder;
            }
        }

        internal string CdataSectionElements
        {
            get
            {
                if (this._AtomCdataSectionElements == null)
                {
                    this._AtomCdataSectionElements = this._NameTable.Add("cdata-section-elements");
                }
                return this._AtomCdataSectionElements;
            }
        }

        internal string Choose =>
            this._AtomChoose;

        internal string Comment
        {
            get
            {
                if (this._AtomComment == null)
                {
                    this._AtomComment = this._NameTable.Add("comment");
                }
                return this._AtomComment;
            }
        }

        internal string Copy
        {
            get
            {
                if (this._AtomCopy == null)
                {
                    this._AtomCopy = this._NameTable.Add("copy");
                }
                return this._AtomCopy;
            }
        }

        internal string CopyOf
        {
            get
            {
                if (this._AtomCopyOf == null)
                {
                    this._AtomCopyOf = this._NameTable.Add("copy-of");
                }
                return this._AtomCopyOf;
            }
        }

        internal string Count
        {
            get
            {
                if (this._AtomCount == null)
                {
                    this._AtomCount = this._NameTable.Add("count");
                }
                return this._AtomCount;
            }
        }

        internal string DataType
        {
            get
            {
                if (this._AtomDataType == null)
                {
                    this._AtomDataType = this._NameTable.Add("data-type");
                }
                return this._AtomDataType;
            }
        }

        internal string DecimalFormat
        {
            get
            {
                if (this._AtomDecimalFormat == null)
                {
                    this._AtomDecimalFormat = this._NameTable.Add("decimal-format");
                }
                return this._AtomDecimalFormat;
            }
        }

        internal string DecimalSeparator
        {
            get
            {
                if (this._AtomDecimalSeparator == null)
                {
                    this._AtomDecimalSeparator = this._NameTable.Add("decimal-separator");
                }
                return this._AtomDecimalSeparator;
            }
        }

        internal string Digit
        {
            get
            {
                if (this._AtomDigit == null)
                {
                    this._AtomDigit = this._NameTable.Add("digit");
                }
                return this._AtomDigit;
            }
        }

        internal string DisableOutputEscaping
        {
            get
            {
                if (this._AtomDisableOutputEscaping == null)
                {
                    this._AtomDisableOutputEscaping = this._NameTable.Add("disable-output-escaping");
                }
                return this._AtomDisableOutputEscaping;
            }
        }

        internal string DoctypePublic
        {
            get
            {
                if (this._AtomDoctypePublic == null)
                {
                    this._AtomDoctypePublic = this._NameTable.Add("doctype-public");
                }
                return this._AtomDoctypePublic;
            }
        }

        internal string DoctypeSystem
        {
            get
            {
                if (this._AtomDoctypeSystem == null)
                {
                    this._AtomDoctypeSystem = this._NameTable.Add("doctype-system");
                }
                return this._AtomDoctypeSystem;
            }
        }

        internal string Element
        {
            get
            {
                if (this._AtomElement == null)
                {
                    this._AtomElement = this._NameTable.Add("element");
                }
                return this._AtomElement;
            }
        }

        internal string Elements
        {
            get
            {
                if (this._AtomElements == null)
                {
                    this._AtomElements = this._NameTable.Add("elements");
                }
                return this._AtomElements;
            }
        }

        internal string Empty =>
            this._AtomEmpty;

        internal string Encoding
        {
            get
            {
                if (this._AtomEncoding == null)
                {
                    this._AtomEncoding = this._NameTable.Add("encoding");
                }
                return this._AtomEncoding;
            }
        }

        internal string ExcludeResultPrefixes
        {
            get
            {
                if (this._AtomExcludeResultPrefixes == null)
                {
                    this._AtomExcludeResultPrefixes = this._NameTable.Add("exclude-result-prefixes");
                }
                return this._AtomExcludeResultPrefixes;
            }
        }

        internal string ExtensionElementPrefixes
        {
            get
            {
                if (this._AtomExtensionElementPrefixes == null)
                {
                    this._AtomExtensionElementPrefixes = this._NameTable.Add("extension-element-prefixes");
                }
                return this._AtomExtensionElementPrefixes;
            }
        }

        internal string Fallback
        {
            get
            {
                if (this._AtomFallback == null)
                {
                    this._AtomFallback = this._NameTable.Add("fallback");
                }
                return this._AtomFallback;
            }
        }

        internal string ForEach =>
            this._AtomForEach;

        internal string Format
        {
            get
            {
                if (this._AtomFormat == null)
                {
                    this._AtomFormat = this._NameTable.Add("format");
                }
                return this._AtomFormat;
            }
        }

        internal string From
        {
            get
            {
                if (this._AtomFrom == null)
                {
                    this._AtomFrom = this._NameTable.Add("from");
                }
                return this._AtomFrom;
            }
        }

        internal string GroupingSeparator
        {
            get
            {
                if (this._AtomGroupingSeparator == null)
                {
                    this._AtomGroupingSeparator = this._NameTable.Add("grouping-separator");
                }
                return this._AtomGroupingSeparator;
            }
        }

        internal string GroupingSize
        {
            get
            {
                if (this._AtomGroupingSize == null)
                {
                    this._AtomGroupingSize = this._NameTable.Add("grouping-size");
                }
                return this._AtomGroupingSize;
            }
        }

        internal string HashDefault
        {
            get
            {
                if (this._AtomHashDefault == null)
                {
                    this._AtomHashDefault = this._NameTable.Add("#default");
                }
                return this._AtomHashDefault;
            }
        }

        internal string Href
        {
            get
            {
                if (this._AtomHref == null)
                {
                    this._AtomHref = this._NameTable.Add("href");
                }
                return this._AtomHref;
            }
        }

        internal string Id
        {
            get
            {
                if (this._AtomId == null)
                {
                    this._AtomId = this._NameTable.Add("id");
                }
                return this._AtomId;
            }
        }

        internal string If =>
            this._AtomIf;

        internal string ImplementsPrefix
        {
            get
            {
                if (this._AtomImplementsPrefix == null)
                {
                    this._AtomImplementsPrefix = this._NameTable.Add("implements-prefix");
                }
                return this._AtomImplementsPrefix;
            }
        }

        internal string Import
        {
            get
            {
                if (this._AtomImport == null)
                {
                    this._AtomImport = this._NameTable.Add("import");
                }
                return this._AtomImport;
            }
        }

        internal string Include
        {
            get
            {
                if (this._AtomInclude == null)
                {
                    this._AtomInclude = this._NameTable.Add("include");
                }
                return this._AtomInclude;
            }
        }

        internal string Indent
        {
            get
            {
                if (this._AtomIndent == null)
                {
                    this._AtomIndent = this._NameTable.Add("indent");
                }
                return this._AtomIndent;
            }
        }

        internal string Infinity
        {
            get
            {
                if (this._AtomInfinity == null)
                {
                    this._AtomInfinity = this._NameTable.Add("infinity");
                }
                return this._AtomInfinity;
            }
        }

        internal string Key
        {
            get
            {
                if (this._AtomKey == null)
                {
                    this._AtomKey = this._NameTable.Add("key");
                }
                return this._AtomKey;
            }
        }

        internal string Lang
        {
            get
            {
                if (this._AtomLang == null)
                {
                    this._AtomLang = this._NameTable.Add("lang");
                }
                return this._AtomLang;
            }
        }

        internal string Language
        {
            get
            {
                if (this._AtomLanguage == null)
                {
                    this._AtomLanguage = this._NameTable.Add("language");
                }
                return this._AtomLanguage;
            }
        }

        internal string LetterValue
        {
            get
            {
                if (this._AtomLetterValue == null)
                {
                    this._AtomLetterValue = this._NameTable.Add("letter-value");
                }
                return this._AtomLetterValue;
            }
        }

        internal string Level
        {
            get
            {
                if (this._AtomLevel == null)
                {
                    this._AtomLevel = this._NameTable.Add("level");
                }
                return this._AtomLevel;
            }
        }

        internal string Match =>
            this._AtomMatch;

        internal string MediaType
        {
            get
            {
                if (this._AtomMediaType == null)
                {
                    this._AtomMediaType = this._NameTable.Add("media-type");
                }
                return this._AtomMediaType;
            }
        }

        internal string Message
        {
            get
            {
                if (this._AtomMessage == null)
                {
                    this._AtomMessage = this._NameTable.Add("message");
                }
                return this._AtomMessage;
            }
        }

        internal string Method
        {
            get
            {
                if (this._AtomMethod == null)
                {
                    this._AtomMethod = this._NameTable.Add("method");
                }
                return this._AtomMethod;
            }
        }

        internal string MinusSign
        {
            get
            {
                if (this._AtomMinusSign == null)
                {
                    this._AtomMinusSign = this._NameTable.Add("minus-sign");
                }
                return this._AtomMinusSign;
            }
        }

        internal string Mode
        {
            get
            {
                if (this._AtomMode == null)
                {
                    this._AtomMode = this._NameTable.Add("mode");
                }
                return this._AtomMode;
            }
        }

        internal string MsXsltNamespace =>
            this._AtomMsXsltNamespace;

        internal string Name =>
            this._AtomName;

        internal string Namespace
        {
            get
            {
                if (this._AtomNamespace == null)
                {
                    this._AtomNamespace = this._NameTable.Add("namespace");
                }
                return this._AtomNamespace;
            }
        }

        internal string NamespaceAlias
        {
            get
            {
                if (this._AtomNamespaceAlias == null)
                {
                    this._AtomNamespaceAlias = this._NameTable.Add("namespace-alias");
                }
                return this._AtomNamespaceAlias;
            }
        }

        internal string NaN
        {
            get
            {
                if (this._AtomNaN == null)
                {
                    this._AtomNaN = this._NameTable.Add("NaN");
                }
                return this._AtomNaN;
            }
        }

        internal string No
        {
            get
            {
                if (this._AtomNo == null)
                {
                    this._AtomNo = this._NameTable.Add("no");
                }
                return this._AtomNo;
            }
        }

        internal string Number
        {
            get
            {
                if (this._AtomNumber == null)
                {
                    this._AtomNumber = this._NameTable.Add("number");
                }
                return this._AtomNumber;
            }
        }

        internal string OmitXmlDeclaration
        {
            get
            {
                if (this._AtomOmitXmlDeclaration == null)
                {
                    this._AtomOmitXmlDeclaration = this._NameTable.Add("omit-xml-declaration");
                }
                return this._AtomOmitXmlDeclaration;
            }
        }

        internal string Order
        {
            get
            {
                if (this._AtomOrder == null)
                {
                    this._AtomOrder = this._NameTable.Add("order");
                }
                return this._AtomOrder;
            }
        }

        internal string Otherwise =>
            this._AtomOtherwise;

        internal string Output
        {
            get
            {
                if (this._AtomOutput == null)
                {
                    this._AtomOutput = this._NameTable.Add("output");
                }
                return this._AtomOutput;
            }
        }

        internal string Param
        {
            get
            {
                if (this._AtomParam == null)
                {
                    this._AtomParam = this._NameTable.Add("param");
                }
                return this._AtomParam;
            }
        }

        internal string PatternSeparator
        {
            get
            {
                if (this._AtomPatternSeparator == null)
                {
                    this._AtomPatternSeparator = this._NameTable.Add("pattern-separator");
                }
                return this._AtomPatternSeparator;
            }
        }

        internal string Percent
        {
            get
            {
                if (this._AtomPercent == null)
                {
                    this._AtomPercent = this._NameTable.Add("percent");
                }
                return this._AtomPercent;
            }
        }

        internal string PerMille
        {
            get
            {
                if (this._AtomPerMille == null)
                {
                    this._AtomPerMille = this._NameTable.Add("per-mille");
                }
                return this._AtomPerMille;
            }
        }

        internal string PreserveSpace
        {
            get
            {
                if (this._AtomPreserveSpace == null)
                {
                    this._AtomPreserveSpace = this._NameTable.Add("preserve-space");
                }
                return this._AtomPreserveSpace;
            }
        }

        internal string Priority
        {
            get
            {
                if (this._AtomPriority == null)
                {
                    this._AtomPriority = this._NameTable.Add("priority");
                }
                return this._AtomPriority;
            }
        }

        internal string ProcessingInstruction
        {
            get
            {
                if (this._AtomProcessingInstruction == null)
                {
                    this._AtomProcessingInstruction = this._NameTable.Add("processing-instruction");
                }
                return this._AtomProcessingInstruction;
            }
        }

        internal string ResultPrefix
        {
            get
            {
                if (this._AtomResultPrefix == null)
                {
                    this._AtomResultPrefix = this._NameTable.Add("result-prefix");
                }
                return this._AtomResultPrefix;
            }
        }

        internal string Script =>
            this._AtomScript;

        internal string Select =>
            this._AtomSelect;

        internal string Sort
        {
            get
            {
                if (this._AtomSort == null)
                {
                    this._AtomSort = this._NameTable.Add("sort");
                }
                return this._AtomSort;
            }
        }

        internal string Standalone
        {
            get
            {
                if (this._AtomStandalone == null)
                {
                    this._AtomStandalone = this._NameTable.Add("standalone");
                }
                return this._AtomStandalone;
            }
        }

        internal string StripSpace
        {
            get
            {
                if (this._AtomStripSpace == null)
                {
                    this._AtomStripSpace = this._NameTable.Add("strip-space");
                }
                return this._AtomStripSpace;
            }
        }

        internal string Stylesheet =>
            this._AtomStylesheet;

        internal string StylesheetPrefix
        {
            get
            {
                if (this._AtomStylesheetPrefix == null)
                {
                    this._AtomStylesheetPrefix = this._NameTable.Add("stylesheet-prefix");
                }
                return this._AtomStylesheetPrefix;
            }
        }

        internal string Template =>
            this._AtomTemplate;

        internal string Terminate
        {
            get
            {
                if (this._AtomTerminate == null)
                {
                    this._AtomTerminate = this._NameTable.Add("terminate");
                }
                return this._AtomTerminate;
            }
        }

        internal string Test =>
            this._AtomTest;

        internal string Text
        {
            get
            {
                if (this._AtomText == null)
                {
                    this._AtomText = this._NameTable.Add("text");
                }
                return this._AtomText;
            }
        }

        internal string Transform =>
            this._AtomTransform;

        internal string Use
        {
            get
            {
                if (this._AtomUse == null)
                {
                    this._AtomUse = this._NameTable.Add("use");
                }
                return this._AtomUse;
            }
        }

        internal string UseAttributeSets
        {
            get
            {
                if (this._AtomUseAttributeSets == null)
                {
                    this._AtomUseAttributeSets = this._NameTable.Add("use-attribute-sets");
                }
                return this._AtomUseAttributeSets;
            }
        }

        internal string Value
        {
            get
            {
                if (this._AtomValue == null)
                {
                    this._AtomValue = this._NameTable.Add("value");
                }
                return this._AtomValue;
            }
        }

        internal string ValueOf =>
            this._AtomValueOf;

        internal string Variable
        {
            get
            {
                if (this._AtomVariable == null)
                {
                    this._AtomVariable = this._NameTable.Add("variable");
                }
                return this._AtomVariable;
            }
        }

        internal string Version
        {
            get
            {
                if (this._AtomVersion == null)
                {
                    this._AtomVersion = this._NameTable.Add("version");
                }
                return this._AtomVersion;
            }
        }

        internal string When =>
            this._AtomWhen;

        internal string WithParam
        {
            get
            {
                if (this._AtomWithParam == null)
                {
                    this._AtomWithParam = this._NameTable.Add("with-param");
                }
                return this._AtomWithParam;
            }
        }

        internal string XsltNamespace =>
            this._AtomXsltNamespace;

        internal string Yes
        {
            get
            {
                if (this._AtomYes == null)
                {
                    this._AtomYes = this._NameTable.Add("yes");
                }
                return this._AtomYes;
            }
        }

        internal string ZeroDigit
        {
            get
            {
                if (this._AtomZeroDigit == null)
                {
                    this._AtomZeroDigit = this._NameTable.Add("zero-digit");
                }
                return this._AtomZeroDigit;
            }
        }
    }
}

