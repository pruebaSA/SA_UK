namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Xslt;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class XsltLibrary
    {
        private DecimalFormats decimalFormats;
        private List<DecimalFormatter> decimalFormatters;
        private HybridDictionary functionsAvail;
        internal const int InvariantCultureLcid = 0x7f;
        private XmlQueryRuntime runtime;

        internal XsltLibrary(XmlQueryRuntime runtime)
        {
            this.runtime = runtime;
        }

        public int CheckScriptNamespace(string nsUri)
        {
            if (this.runtime.ExternalContext.GetLateBoundObject(nsUri) != null)
            {
                throw new XslTransformException("Xslt_ScriptAndExtensionClash", new string[] { nsUri });
            }
            return 0;
        }

        [Conditional("DEBUG")]
        internal static void CheckXsltValue(IList<XPathItem> val)
        {
            if (val.Count == 1)
            {
                XsltFunctions.EXslObjectType(val);
            }
            else
            {
                int count = val.Count;
                for (int i = 0; i < count; i++)
                {
                    if (!val[i].IsNode)
                    {
                        return;
                    }
                    if (i == 1)
                    {
                        i += Math.Max(count - 4, 0);
                    }
                }
            }
        }

        [Conditional("DEBUG")]
        internal static void CheckXsltValue(XPathItem item)
        {
        }

        private static bool CompareNodeSetAndNodeSet(ComparisonOperator op, IList<XPathNavigator> left, IList<XPathNavigator> right, TypeCode compType)
        {
            int count = left.Count;
            int num2 = right.Count;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    if (CompareValues(op, left[i], right[j], compType))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool CompareNodeSetAndValue(ComparisonOperator op, IList<XPathNavigator> nodeset, XPathItem val, TypeCode compType)
        {
            if (compType == TypeCode.Boolean)
            {
                return CompareNumbers(op, (nodeset.Count != 0) ? ((double) 1) : ((double) 0), XsltConvert.ToBoolean(val) ? ((double) 1) : ((double) 0));
            }
            int count = nodeset.Count;
            for (int i = 0; i < count; i++)
            {
                if (CompareValues(op, nodeset[i], val, compType))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CompareNumbers(ComparisonOperator op, double left, double right)
        {
            switch (op)
            {
                case ComparisonOperator.Eq:
                    return (left == right);

                case ComparisonOperator.Ne:
                    return (left != right);

                case ComparisonOperator.Lt:
                    return (left < right);

                case ComparisonOperator.Le:
                    return (left <= right);

                case ComparisonOperator.Gt:
                    return (left > right);
            }
            return (left >= right);
        }

        private static bool CompareValues(ComparisonOperator op, XPathItem left, XPathItem right, TypeCode compType)
        {
            if (compType == TypeCode.Double)
            {
                return CompareNumbers(op, XsltConvert.ToDouble(left), XsltConvert.ToDouble(right));
            }
            if (compType == TypeCode.String)
            {
                return ((XsltConvert.ToString(left) == XsltConvert.ToString(right)) == (op == ComparisonOperator.Eq));
            }
            return ((XsltConvert.ToBoolean(left) == XsltConvert.ToBoolean(right)) == (op == ComparisonOperator.Eq));
        }

        private DecimalFormatter CreateDecimalFormatter(string formatPicture, string infinitySymbol, string nanSymbol, string characters)
        {
            NumberFormatInfo info = new NumberFormatInfo {
                NumberDecimalSeparator = char.ToString(characters[0]),
                NumberGroupSeparator = char.ToString(characters[1]),
                PositiveInfinitySymbol = infinitySymbol,
                NegativeSign = char.ToString(characters[7]),
                NaNSymbol = nanSymbol,
                PercentSymbol = char.ToString(characters[2]),
                PerMilleSymbol = char.ToString(characters[3])
            };
            info.NegativeInfinitySymbol = info.NegativeSign + info.PositiveInfinitySymbol;
            return new DecimalFormatter(formatPicture, new DecimalFormat(info, characters[5], characters[4], characters[6]));
        }

        public bool ElementAvailable(XmlQualifiedName name) => 
            QilGenerator.IsElementAvailable(name);

        public bool EqualityOperator(double opCode, IList<XPathItem> left, IList<XPathItem> right)
        {
            ComparisonOperator op = (ComparisonOperator) ((int) opCode);
            if (IsNodeSetOrRtf(left))
            {
                if (IsNodeSetOrRtf(right))
                {
                    return CompareNodeSetAndNodeSet(op, ToNodeSetOrRtf(left), ToNodeSetOrRtf(right), TypeCode.String);
                }
                XPathItem val = right[0];
                return CompareNodeSetAndValue(op, ToNodeSetOrRtf(left), val, GetTypeCode(val));
            }
            if (IsNodeSetOrRtf(right))
            {
                XPathItem item2 = left[0];
                return CompareNodeSetAndValue(op, ToNodeSetOrRtf(right), item2, GetTypeCode(item2));
            }
            XPathItem item3 = left[0];
            XPathItem item4 = right[0];
            return CompareValues(op, item3, item4, WeakestTypeCode(GetTypeCode(item3), GetTypeCode(item4)));
        }

        public string FormatMessage(string res, IList<string> args)
        {
            string[] strArray = new string[args.Count];
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = args[i];
            }
            return XslTransformException.CreateMessage(res, strArray);
        }

        public string FormatNumberDynamic(double value, string formatPicture, XmlQualifiedName decimalFormatName, string errorMessageName)
        {
            DecimalFormatDecl decl;
            if ((this.decimalFormats != null) && this.decimalFormats.Contains(decimalFormatName))
            {
                decl = this.decimalFormats[decimalFormatName];
            }
            else
            {
                if (decimalFormatName != DecimalFormatDecl.Default.Name)
                {
                    throw new XslTransformException("Xslt_NoDecimalFormat", new string[] { errorMessageName });
                }
                decl = DecimalFormatDecl.Default;
            }
            return this.CreateDecimalFormatter(formatPicture, decl.InfinitySymbol, decl.NanSymbol, new string(decl.Characters)).Format(value);
        }

        public string FormatNumberStatic(double value, double decimalFormatterIndex)
        {
            int num = (int) decimalFormatterIndex;
            return this.decimalFormatters[num].Format(value);
        }

        public bool FunctionAvailable(XmlQualifiedName name)
        {
            if (this.functionsAvail == null)
            {
                this.functionsAvail = new HybridDictionary();
            }
            else
            {
                object obj2 = this.functionsAvail[name];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
            }
            bool flag = this.FunctionAvailableHelper(name);
            this.functionsAvail[name] = flag;
            return flag;
        }

        private bool FunctionAvailableHelper(XmlQualifiedName name)
        {
            if (QilGenerator.IsFunctionAvailable(name.Name, name.Namespace))
            {
                return true;
            }
            if ((name.Namespace.Length == 0) || (name.Namespace == "http://www.w3.org/1999/XSL/Transform"))
            {
                return false;
            }
            return (this.runtime.ExternalContext.LateBoundFunctionExists(name.Name, name.Namespace) || this.runtime.EarlyBoundFunctionExists(name.Name, name.Namespace));
        }

        private static TypeCode GetTypeCode(XPathItem item)
        {
            Type valueType = item.ValueType;
            if (valueType == XsltConvert.StringType)
            {
                return TypeCode.String;
            }
            if (valueType == XsltConvert.DoubleType)
            {
                return TypeCode.Double;
            }
            return TypeCode.Boolean;
        }

        private static ComparisonOperator InvertOperator(ComparisonOperator op)
        {
            switch (op)
            {
                case ComparisonOperator.Lt:
                    return ComparisonOperator.Gt;

                case ComparisonOperator.Le:
                    return ComparisonOperator.Ge;

                case ComparisonOperator.Gt:
                    return ComparisonOperator.Lt;

                case ComparisonOperator.Ge:
                    return ComparisonOperator.Le;
            }
            return op;
        }

        private static bool IsNodeSetOrRtf(IList<XPathItem> val)
        {
            if (val.Count == 1)
            {
                return val[0].IsNode;
            }
            return true;
        }

        public bool IsSameNodeSort(XPathNavigator nav1, XPathNavigator nav2)
        {
            XPathNodeType nodeType = nav1.NodeType;
            XPathNodeType type2 = nav2.NodeType;
            if ((XPathNodeType.Text <= nodeType) && (nodeType <= XPathNodeType.Whitespace))
            {
                return ((XPathNodeType.Text <= type2) && (type2 <= XPathNodeType.Whitespace));
            }
            return (((nodeType == type2) && Ref.Equal(nav1.LocalName, nav2.LocalName)) && Ref.Equal(nav1.NamespaceURI, nav2.NamespaceURI));
        }

        public int LangToLcid(string lang, bool forwardCompatibility) => 
            LangToLcidInternal(lang, forwardCompatibility, null);

        internal static int LangToLcidInternal(string lang, bool forwardCompatibility, IErrorHelper errorHelper)
        {
            int lCID = 0x7f;
            if (lang != null)
            {
                if (lang.Length == 0)
                {
                    if (!forwardCompatibility)
                    {
                        if (errorHelper == null)
                        {
                            throw new XslTransformException("Xslt_InvalidAttrValue", new string[] { "lang", lang });
                        }
                        errorHelper.ReportError("Xslt_InvalidAttrValue", new string[] { "lang", lang });
                    }
                    return lCID;
                }
                try
                {
                    lCID = new CultureInfo(lang).LCID;
                }
                catch (ArgumentException)
                {
                    if (!forwardCompatibility)
                    {
                        if (errorHelper == null)
                        {
                            throw new XslTransformException("Xslt_InvalidLanguage", new string[] { lang });
                        }
                        errorHelper.ReportError("Xslt_InvalidLanguage", new string[] { lang });
                    }
                    return lCID;
                }
            }
            return lCID;
        }

        public string NumberFormat(IList<XPathItem> value, string formatString, double lang, string letterValue, string groupingSeparator, double groupingSize)
        {
            NumberFormatter formatter = new NumberFormatter(formatString, (int) lang, letterValue, groupingSeparator, (int) groupingSize);
            return formatter.FormatSequence(value);
        }

        public int RegisterDecimalFormat(XmlQualifiedName name, string infinitySymbol, string nanSymbol, string characters)
        {
            if (this.decimalFormats == null)
            {
                this.decimalFormats = new DecimalFormats();
            }
            DecimalFormatDecl item = new DecimalFormatDecl(name, infinitySymbol, nanSymbol, characters);
            this.decimalFormats.Add(item);
            return 0;
        }

        public double RegisterDecimalFormatter(string formatPicture, string infinitySymbol, string nanSymbol, string characters)
        {
            if (this.decimalFormatters == null)
            {
                this.decimalFormatters = new List<DecimalFormatter>();
            }
            this.decimalFormatters.Add(this.CreateDecimalFormatter(formatPicture, infinitySymbol, nanSymbol, characters));
            return (double) (this.decimalFormatters.Count - 1);
        }

        public bool RelationalOperator(double opCode, IList<XPathItem> left, IList<XPathItem> right)
        {
            ComparisonOperator op = (ComparisonOperator) ((int) opCode);
            if (IsNodeSetOrRtf(left))
            {
                if (IsNodeSetOrRtf(right))
                {
                    return CompareNodeSetAndNodeSet(op, ToNodeSetOrRtf(left), ToNodeSetOrRtf(right), TypeCode.Double);
                }
                XPathItem val = right[0];
                return CompareNodeSetAndValue(op, ToNodeSetOrRtf(left), val, WeakestTypeCode(GetTypeCode(val), TypeCode.Double));
            }
            if (IsNodeSetOrRtf(right))
            {
                XPathItem item2 = left[0];
                return CompareNodeSetAndValue(InvertOperator(op), ToNodeSetOrRtf(right), item2, WeakestTypeCode(GetTypeCode(item2), TypeCode.Double));
            }
            XPathItem item3 = left[0];
            XPathItem item4 = right[0];
            return CompareValues(op, item3, item4, TypeCode.Double);
        }

        private static IList<XPathNavigator> ToNodeSetOrRtf(IList<XPathItem> val) => 
            XmlILStorageConverter.ItemsToNavigators(val);

        private static TypeCode WeakestTypeCode(TypeCode typeCode1, TypeCode typeCode2)
        {
            if (typeCode1 >= typeCode2)
            {
                return typeCode2;
            }
            return typeCode1;
        }

        internal enum ComparisonOperator
        {
            Eq,
            Ne,
            Lt,
            Le,
            Gt,
            Ge
        }
    }
}

