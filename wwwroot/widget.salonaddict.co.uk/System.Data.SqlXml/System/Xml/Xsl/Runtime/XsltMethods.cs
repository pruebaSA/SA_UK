namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Reflection;

    internal static class XsltMethods
    {
        public static readonly MethodInfo BaseUri = GetMethod(typeof(XsltFunctions), "BaseUri");
        public static readonly MethodInfo Ceiling = GetMethod(typeof(Math), "Ceiling", new Type[] { typeof(double) });
        public static readonly MethodInfo CheckScriptNamespace = GetMethod(typeof(XsltLibrary), "CheckScriptNamespace");
        public static readonly MethodInfo Contains = GetMethod(typeof(XsltFunctions), "Contains");
        public static readonly MethodInfo ElementAvailable = GetMethod(typeof(XsltLibrary), "ElementAvailable");
        public static readonly MethodInfo EnsureNodeSet = GetMethod(typeof(XsltConvert), "EnsureNodeSet", new Type[] { typeof(IList<XPathItem>) });
        public static readonly MethodInfo EqualityOperator = GetMethod(typeof(XsltLibrary), "EqualityOperator");
        public static readonly MethodInfo EXslObjectType = GetMethod(typeof(XsltFunctions), "EXslObjectType");
        public static readonly MethodInfo Floor = GetMethod(typeof(Math), "Floor", new Type[] { typeof(double) });
        public static readonly MethodInfo FormatMessage = GetMethod(typeof(XsltLibrary), "FormatMessage");
        public static readonly MethodInfo FormatNumberDynamic = GetMethod(typeof(XsltLibrary), "FormatNumberDynamic");
        public static readonly MethodInfo FormatNumberStatic = GetMethod(typeof(XsltLibrary), "FormatNumberStatic");
        public static readonly MethodInfo FunctionAvailable = GetMethod(typeof(XsltLibrary), "FunctionAvailable");
        public static readonly MethodInfo IsSameNodeSort = GetMethod(typeof(XsltLibrary), "IsSameNodeSort");
        public static readonly MethodInfo Lang = GetMethod(typeof(XsltFunctions), "Lang");
        public static readonly MethodInfo LangToLcid = GetMethod(typeof(XsltLibrary), "LangToLcid");
        public static readonly MethodInfo MSFormatDateTime = GetMethod(typeof(XsltFunctions), "MSFormatDateTime");
        public static readonly MethodInfo MSLocalName = GetMethod(typeof(XsltFunctions), "MSLocalName");
        public static readonly MethodInfo MSNamespaceUri = GetMethod(typeof(XsltFunctions), "MSNamespaceUri");
        public static readonly MethodInfo MSNumber = GetMethod(typeof(XsltFunctions), "MSNumber");
        public static readonly MethodInfo MSStringCompare = GetMethod(typeof(XsltFunctions), "MSStringCompare");
        public static readonly MethodInfo MSUtc = GetMethod(typeof(XsltFunctions), "MSUtc");
        public static readonly MethodInfo NormalizeSpace = GetMethod(typeof(XsltFunctions), "NormalizeSpace");
        public static readonly MethodInfo NumberFormat = GetMethod(typeof(XsltLibrary), "NumberFormat");
        public static readonly MethodInfo OnCurrentNodeChanged = GetMethod(typeof(XmlQueryRuntime), "OnCurrentNodeChanged");
        public static readonly MethodInfo OuterXml = GetMethod(typeof(XsltFunctions), "OuterXml");
        public static readonly MethodInfo RegisterDecimalFormat = GetMethod(typeof(XsltLibrary), "RegisterDecimalFormat");
        public static readonly MethodInfo RegisterDecimalFormatter = GetMethod(typeof(XsltLibrary), "RegisterDecimalFormatter");
        public static readonly MethodInfo RelationalOperator = GetMethod(typeof(XsltLibrary), "RelationalOperator");
        public static readonly MethodInfo Round = GetMethod(typeof(XsltFunctions), "Round");
        public static readonly MethodInfo StartsWith = GetMethod(typeof(XsltFunctions), "StartsWith");
        public static readonly MethodInfo Substring2 = GetMethod(typeof(XsltFunctions), "Substring", new Type[] { typeof(string), typeof(double) });
        public static readonly MethodInfo Substring3 = GetMethod(typeof(XsltFunctions), "Substring", new Type[] { typeof(string), typeof(double), typeof(double) });
        public static readonly MethodInfo SubstringAfter = GetMethod(typeof(XsltFunctions), "SubstringAfter");
        public static readonly MethodInfo SubstringBefore = GetMethod(typeof(XsltFunctions), "SubstringBefore");
        public static readonly MethodInfo SystemProperty = GetMethod(typeof(XsltFunctions), "SystemProperty");
        public static readonly MethodInfo Translate = GetMethod(typeof(XsltFunctions), "Translate");

        public static MethodInfo GetMethod(Type className, string methName) => 
            className.GetMethod(methName);

        public static MethodInfo GetMethod(Type className, string methName, params Type[] args) => 
            className.GetMethod(methName, args);
    }
}

