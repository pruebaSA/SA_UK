namespace System.Xml.Utils
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Threading;

    internal sealed class Res
    {
        internal const string Coll_BadOptFormat = "Coll_BadOptFormat";
        internal const string Coll_Unsupported = "Coll_Unsupported";
        internal const string Coll_UnsupportedLanguage = "Coll_UnsupportedLanguage";
        internal const string Coll_UnsupportedOpt = "Coll_UnsupportedOpt";
        internal const string Coll_UnsupportedOptVal = "Coll_UnsupportedOptVal";
        internal const string Coll_UnsupportedSortOpt = "Coll_UnsupportedSortOpt";
        private static Res loader;
        internal const string Qil_Validation = "Qil_Validation";
        private ResourceManager resources;
        private static object s_InternalSyncObject;
        internal const string Xml_EndOfInnerExceptionStack = "Xml_EndOfInnerExceptionStack";
        internal const string Xml_ErrorFilePosition = "Xml_ErrorFilePosition";
        internal const string Xml_InvalidOperation = "Xml_InvalidOperation";
        internal const string Xml_UserException = "Xml_UserException";
        internal const string XmlIl_AmbiguousExtensionMethod = "XmlIl_AmbiguousExtensionMethod";
        internal const string XmlIl_BadXmlState = "XmlIl_BadXmlState";
        internal const string XmlIl_BadXmlStateAttr = "XmlIl_BadXmlStateAttr";
        internal const string XmlIl_ByRefType = "XmlIl_ByRefType";
        internal const string XmlIl_CantResolveEntity = "XmlIl_CantResolveEntity";
        internal const string XmlIl_CantStripNav = "XmlIl_CantStripNav";
        internal const string XmlIl_DocumentLoadError = "XmlIl_DocumentLoadError";
        internal const string XmlIl_ExtensionError = "XmlIl_ExtensionError";
        internal const string XmlIl_GenericExtensionMethod = "XmlIl_GenericExtensionMethod";
        internal const string XmlIl_NmspAfterAttr = "XmlIl_NmspAfterAttr";
        internal const string XmlIl_NmspConflict = "XmlIl_NmspConflict";
        internal const string XmlIl_NoDefaultDocument = "XmlIl_NoDefaultDocument";
        internal const string XmlIl_NoExtensionMethod = "XmlIl_NoExtensionMethod";
        internal const string XmlIl_NonPublicExtensionMethod = "XmlIl_NonPublicExtensionMethod";
        internal const string XmlIl_TooManyParameters = "XmlIl_TooManyParameters";
        internal const string XmlIl_TopLevelAttrNmsp = "XmlIl_TopLevelAttrNmsp";
        internal const string XmlIl_UnknownDocument = "XmlIl_UnknownDocument";
        internal const string XmlIl_UnknownExtObj = "XmlIl_UnknownExtObj";
        internal const string XmlIl_UnknownParam = "XmlIl_UnknownParam";
        internal const string XPath_AtLeastNArgsExpected = "XPath_AtLeastNArgsExpected";
        internal const string XPath_AtMostMArgsExpected = "XPath_AtMostMArgsExpected";
        internal const string XPath_EofExpected = "XPath_EofExpected";
        internal const string XPath_InvalidAxisInPattern = "XPath_InvalidAxisInPattern";
        internal const string XPath_NArgsExpected = "XPath_NArgsExpected";
        internal const string XPath_NodeSetArgumentExpected = "XPath_NodeSetArgumentExpected";
        internal const string XPath_NodeSetExpected = "XPath_NodeSetExpected";
        internal const string XPath_NodeTestExpected = "XPath_NodeTestExpected";
        internal const string XPath_NOrMArgsExpected = "XPath_NOrMArgsExpected";
        internal const string XPath_PredicateAfterDot = "XPath_PredicateAfterDot";
        internal const string XPath_PredicateAfterDotDot = "XPath_PredicateAfterDotDot";
        internal const string XPath_RtfInPathExpr = "XPath_RtfInPathExpr";
        internal const string XPath_ScientificNotation = "XPath_ScientificNotation";
        internal const string XPath_TokenExpected = "XPath_TokenExpected";
        internal const string XPath_UnclosedString = "XPath_UnclosedString";
        internal const string XPath_UnexpectedToken = "XPath_UnexpectedToken";
        internal const string XPath_UnknownAxis = "XPath_UnknownAxis";
        internal const string Xslt_ApplyImports = "Xslt_ApplyImports";
        internal const string Xslt_AssemblyBothNameHrefAbsent = "Xslt_AssemblyBothNameHrefAbsent";
        internal const string Xslt_AssemblyBothNameHrefPresent = "Xslt_AssemblyBothNameHrefPresent";
        internal const string Xslt_AttributeRedefinition = "Xslt_AttributeRedefinition";
        internal const string Xslt_BistateAttribute = "Xslt_BistateAttribute";
        internal const string Xslt_BothMatchNameAbsent = "Xslt_BothMatchNameAbsent";
        internal const string Xslt_CannotLoadStylesheet = "Xslt_CannotLoadStylesheet";
        internal const string Xslt_CantResolve = "Xslt_CantResolve";
        internal const string Xslt_CharAttribute = "Xslt_CharAttribute";
        internal const string Xslt_CircularAttributeSet = "Xslt_CircularAttributeSet";
        internal const string Xslt_CircularInclude = "Xslt_CircularInclude";
        internal const string Xslt_CircularReference = "Xslt_CircularReference";
        internal const string Xslt_CompileError = "Xslt_CompileError";
        internal const string Xslt_CompileError2 = "Xslt_CompileError2";
        internal const string Xslt_CurrentNotAllowed = "Xslt_CurrentNotAllowed";
        internal const string Xslt_DecimalFormatRedefined = "Xslt_DecimalFormatRedefined";
        internal const string Xslt_DecimalFormatSignsNotDistinct = "Xslt_DecimalFormatSignsNotDistinct";
        internal const string Xslt_DocumentFuncProhibited = "Xslt_DocumentFuncProhibited";
        internal const string Xslt_DupDecimalFormat = "Xslt_DupDecimalFormat";
        internal const string Xslt_DupGlobalVariable = "Xslt_DupGlobalVariable";
        internal const string Xslt_DuplicateParametr = "Xslt_DuplicateParametr";
        internal const string Xslt_DuplicateWithParam = "Xslt_DuplicateWithParam";
        internal const string Xslt_DupLocalVariable = "Xslt_DupLocalVariable";
        internal const string Xslt_DupNsAlias = "Xslt_DupNsAlias";
        internal const string Xslt_DupOtherwise = "Xslt_DupOtherwise";
        internal const string Xslt_DupTemplateName = "Xslt_DupTemplateName";
        internal const string Xslt_DupVarName = "Xslt_DupVarName";
        internal const string Xslt_EmptyAttrValue = "Xslt_EmptyAttrValue";
        internal const string Xslt_EmptyAvtExpr = "Xslt_EmptyAvtExpr";
        internal const string Xslt_EmptyNsAlias = "Xslt_EmptyNsAlias";
        internal const string Xslt_EmptyTagRequired = "Xslt_EmptyTagRequired";
        internal const string Xslt_FunctionFailed = "Xslt_FunctionFailed";
        internal const string Xslt_InvalidApplyImports = "Xslt_InvalidApplyImports";
        internal const string Xslt_InvalidAttribute = "Xslt_InvalidAttribute";
        internal const string Xslt_InvalidAttrValue = "Xslt_InvalidAttrValue";
        internal const string Xslt_InvalidCallTemplate = "Xslt_InvalidCallTemplate";
        internal const string Xslt_InvalidCompareOption = "Xslt_InvalidCompareOption";
        internal const string Xslt_InvalidContents = "Xslt_InvalidContents";
        internal const string Xslt_InvalidEncoding = "Xslt_InvalidEncoding";
        internal const string Xslt_InvalidExtensionNamespace = "Xslt_InvalidExtensionNamespace";
        internal const string Xslt_InvalidExtensionPermitions = "Xslt_InvalidExtensionPermitions";
        internal const string Xslt_InvalidFormat = "Xslt_InvalidFormat";
        internal const string Xslt_InvalidFormat1 = "Xslt_InvalidFormat1";
        internal const string Xslt_InvalidFormat2 = "Xslt_InvalidFormat2";
        internal const string Xslt_InvalidFormat3 = "Xslt_InvalidFormat3";
        internal const string Xslt_InvalidFormat4 = "Xslt_InvalidFormat4";
        internal const string Xslt_InvalidFormat5 = "Xslt_InvalidFormat5";
        internal const string Xslt_InvalidFormat6 = "Xslt_InvalidFormat6";
        internal const string Xslt_InvalidFormat7 = "Xslt_InvalidFormat7";
        internal const string Xslt_InvalidFormat8 = "Xslt_InvalidFormat8";
        internal const string Xslt_InvalidLanguage = "Xslt_InvalidLanguage";
        internal const string Xslt_InvalidLanguageTag = "Xslt_InvalidLanguageTag";
        internal const string Xslt_InvalidMethod = "Xslt_InvalidMethod";
        internal const string Xslt_InvalidModeAttribute = "Xslt_InvalidModeAttribute";
        internal const string Xslt_InvalidParamNamespace = "Xslt_InvalidParamNamespace";
        internal const string Xslt_InvalidPattern = "Xslt_InvalidPattern";
        internal const string Xslt_InvalidPrefix = "Xslt_InvalidPrefix";
        internal const string Xslt_InvalidQName = "Xslt_InvalidQName";
        internal const string Xslt_InvalidVariable = "Xslt_InvalidVariable";
        internal const string Xslt_InvalidXPath = "Xslt_InvalidXPath";
        internal const string Xslt_ItemNull = "Xslt_ItemNull";
        internal const string Xslt_KeyNotAllowed = "Xslt_KeyNotAllowed";
        internal const string Xslt_MissingAttribute = "Xslt_MissingAttribute";
        internal const string Xslt_ModeWithoutMatch = "Xslt_ModeWithoutMatch";
        internal const string Xslt_MultipleRoots = "Xslt_MultipleRoots";
        internal const string Xslt_NestedAvt = "Xslt_NestedAvt";
        internal const string Xslt_NoAttributeSet = "Xslt_NoAttributeSet";
        internal const string Xslt_NoDecimalFormat = "Xslt_NoDecimalFormat";
        internal const string Xslt_NodeSetNotNode = "Xslt_NodeSetNotNode";
        internal const string Xslt_NoNavigatorConversion = "Xslt_NoNavigatorConversion";
        internal const string Xslt_NoNodeSetConversion = "Xslt_NoNodeSetConversion";
        internal const string Xslt_NoStylesheetLoaded = "Xslt_NoStylesheetLoaded";
        internal const string Xslt_NotAtTop = "Xslt_NotAtTop";
        internal const string Xslt_NotEmptyContents = "Xslt_NotEmptyContents";
        internal const string Xslt_NotFirstImport = "Xslt_NotFirstImport";
        internal const string Xslt_NoWhen = "Xslt_NoWhen";
        internal const string Xslt_NullNsAtTopLevel = "Xslt_NullNsAtTopLevel";
        internal const string Xslt_OpenBracesAvt = "Xslt_OpenBracesAvt";
        internal const string Xslt_OpenLiteralAvt = "Xslt_OpenLiteralAvt";
        internal const string Xslt_PriorityWithoutMatch = "Xslt_PriorityWithoutMatch";
        internal const string Xslt_ReservedNS = "Xslt_ReservedNS";
        internal const string Xslt_ScriptAndExtensionClash = "Xslt_ScriptAndExtensionClash";
        internal const string Xslt_ScriptCompileErrors = "Xslt_ScriptCompileErrors";
        internal const string Xslt_ScriptCompileException = "Xslt_ScriptCompileException";
        internal const string Xslt_ScriptDub = "Xslt_ScriptDub";
        internal const string Xslt_ScriptEmpty = "Xslt_ScriptEmpty";
        internal const string Xslt_ScriptInvalidLang = "Xslt_ScriptInvalidLang";
        internal const string Xslt_ScriptInvalidLanguage = "Xslt_ScriptInvalidLanguage";
        internal const string Xslt_ScriptInvalidPrefix = "Xslt_ScriptInvalidPrefix";
        internal const string Xslt_ScriptMixedLanguages = "Xslt_ScriptMixedLanguages";
        internal const string Xslt_ScriptMixLang = "Xslt_ScriptMixLang";
        internal const string Xslt_ScriptNotAtTop = "Xslt_ScriptNotAtTop";
        internal const string Xslt_ScriptsProhibited = "Xslt_ScriptsProhibited";
        internal const string Xslt_ScriptXsltNamespace = "Xslt_ScriptXsltNamespace";
        internal const string Xslt_SingleRightAvt = "Xslt_SingleRightAvt";
        internal const string Xslt_SingleRightBraceInAvt = "Xslt_SingleRightBraceInAvt";
        internal const string Xslt_TemplateNoAttrib = "Xslt_TemplateNoAttrib";
        internal const string Xslt_Terminate = "Xslt_Terminate";
        internal const string Xslt_TextNodesNotAllowed = "Xslt_TextNodesNotAllowed";
        internal const string Xslt_UndefinedKey = "Xslt_UndefinedKey";
        internal const string Xslt_UnexpectedElement = "Xslt_UnexpectedElement";
        internal const string Xslt_UnexpectedElementQ = "Xslt_UnexpectedElementQ";
        internal const string Xslt_UnexpectedKeyword = "Xslt_UnexpectedKeyword";
        internal const string Xslt_UnknownExtensionElement = "Xslt_UnknownExtensionElement";
        internal const string Xslt_UnknownXsltFunction = "Xslt_UnknownXsltFunction";
        internal const string Xslt_UnsuppFunction = "Xslt_UnsuppFunction";
        internal const string Xslt_UnsupportedClrType = "Xslt_UnsupportedClrType";
        internal const string Xslt_UnsupportedXsltFunction = "Xslt_UnsupportedXsltFunction";
        internal const string Xslt_VariableCntSel = "Xslt_VariableCntSel";
        internal const string Xslt_VariableCntSel2 = "Xslt_VariableCntSel2";
        internal const string Xslt_VariablesNotAllowed = "Xslt_VariablesNotAllowed";
        internal const string Xslt_WarningAsError = "Xslt_WarningAsError";
        internal const string Xslt_WdXslNamespace = "Xslt_WdXslNamespace";
        internal const string Xslt_WhenAfterOtherwise = "Xslt_WhenAfterOtherwise";
        internal const string Xslt_WrongNamespace = "Xslt_WrongNamespace";
        internal const string Xslt_WrongNumberArgs = "Xslt_WrongNumberArgs";
        internal const string Xslt_WrongStylesheetElement = "Xslt_WrongStylesheetElement";
        internal const string Xslt_XmlnsAttr = "Xslt_XmlnsAttr";

        internal Res()
        {
            this.resources = new ResourceManager("System.Xml.Utils", base.GetType().Assembly);
        }

        private static Res GetLoader()
        {
            if (loader == null)
            {
                lock (InternalSyncObject)
                {
                    if (loader == null)
                    {
                        loader = new Res();
                    }
                }
            }
            return loader;
        }

        public static object GetObject(string name)
        {
            Res loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetObject(name, Culture);
        }

        public static string GetString(string name)
        {
            Res loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetString(name, Culture);
        }

        public static string GetString(string name, params object[] args)
        {
            Res loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            string format = loader.resources.GetString(name, Culture);
            if ((args == null) || (args.Length <= 0))
            {
                return format;
            }
            for (int i = 0; i < args.Length; i++)
            {
                string str2 = args[i] as string;
                if ((str2 != null) && (str2.Length > 0x400))
                {
                    args[i] = str2.Substring(0, 0x3fd) + "...";
                }
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        private static CultureInfo Culture =>
            null;

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }

        public static ResourceManager Resources =>
            GetLoader().resources;
    }
}

