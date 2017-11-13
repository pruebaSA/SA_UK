namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class XsltConvert
    {
        internal static readonly Type BooleanType = typeof(bool);
        internal static readonly Type ByteArrayType = typeof(byte[]);
        internal static readonly Type ByteType = typeof(byte);
        internal static readonly Type DateTimeType = typeof(DateTime);
        internal static readonly Type DecimalType = typeof(decimal);
        internal static readonly Type DoubleType = typeof(double);
        internal static readonly Type ICollectionType = typeof(ICollection);
        internal static readonly Type IEnumerableType = typeof(IEnumerable);
        internal static readonly Type IListType = typeof(IList);
        internal static readonly Type Int16Type = typeof(short);
        internal static readonly Type Int32Type = typeof(int);
        internal static readonly Type Int64Type = typeof(long);
        internal static readonly Type IXPathNavigableType = typeof(IXPathNavigable);
        internal static readonly Type ObjectType = typeof(object);
        internal static readonly Type SByteType = typeof(sbyte);
        internal static readonly Type SingleType = typeof(float);
        internal static readonly Type StringType = typeof(string);
        internal static readonly Type TimeSpanType = typeof(TimeSpan);
        internal static readonly Type UInt16Type = typeof(ushort);
        internal static readonly Type UInt32Type = typeof(uint);
        internal static readonly Type UInt64Type = typeof(ulong);
        internal static readonly Type UriType = typeof(Uri);
        internal static readonly Type VoidType = typeof(void);
        internal static readonly Type XmlAtomicValueType = typeof(XmlAtomicValue);
        internal static readonly Type XmlQualifiedNameType = typeof(XmlQualifiedName);
        internal static readonly Type XPathItemType = typeof(XPathItem);
        internal static readonly Type XPathNavigatorArrayType = typeof(XPathNavigator[]);
        internal static readonly Type XPathNavigatorType = typeof(XPathNavigator);
        internal static readonly Type XPathNodeIteratorType = typeof(XPathNodeIterator);

        internal static XmlAtomicValue ConvertToType(XmlAtomicValue value, XmlQueryType destinationType)
        {
            switch (destinationType.TypeCode)
            {
                case XmlTypeCode.String:
                    switch (value.XmlType.TypeCode)
                    {
                        case XmlTypeCode.String:
                        case XmlTypeCode.Boolean:
                        case XmlTypeCode.Double:
                            return new XmlAtomicValue(destinationType.SchemaType, ToString(value));

                        case XmlTypeCode.Decimal:
                        case XmlTypeCode.Float:
                        case XmlTypeCode.Duration:
                            return value;

                        case XmlTypeCode.DateTime:
                            return new XmlAtomicValue(destinationType.SchemaType, ToString(value.ValueAsDateTime));
                    }
                    return value;

                case XmlTypeCode.Boolean:
                    switch (value.XmlType.TypeCode)
                    {
                        case XmlTypeCode.String:
                        case XmlTypeCode.Boolean:
                        case XmlTypeCode.Double:
                            return new XmlAtomicValue(destinationType.SchemaType, ToBoolean(value));

                        case XmlTypeCode.Decimal:
                        case XmlTypeCode.Float:
                            return value;
                    }
                    return value;

                case XmlTypeCode.Decimal:
                    if (value.XmlType.TypeCode != XmlTypeCode.Double)
                    {
                        return value;
                    }
                    return new XmlAtomicValue(destinationType.SchemaType, ToDecimal(value.ValueAsDouble));

                case XmlTypeCode.Float:
                case XmlTypeCode.Duration:
                    return value;

                case XmlTypeCode.Double:
                    switch (value.XmlType.TypeCode)
                    {
                        case XmlTypeCode.String:
                        case XmlTypeCode.Boolean:
                        case XmlTypeCode.Double:
                            return new XmlAtomicValue(destinationType.SchemaType, ToDouble(value));

                        case XmlTypeCode.Decimal:
                            return new XmlAtomicValue(destinationType.SchemaType, ToDouble((decimal) value.ValueAs(DecimalType, null)));

                        case XmlTypeCode.Float:
                            return value;

                        case XmlTypeCode.Long:
                        case XmlTypeCode.Int:
                            return new XmlAtomicValue(destinationType.SchemaType, ToDouble(value.ValueAsLong));
                    }
                    return value;

                case XmlTypeCode.DateTime:
                    if (value.XmlType.TypeCode != XmlTypeCode.String)
                    {
                        return value;
                    }
                    return new XmlAtomicValue(destinationType.SchemaType, ToDateTime(value.Value));

                case XmlTypeCode.Long:
                case XmlTypeCode.Int:
                    if (value.XmlType.TypeCode != XmlTypeCode.Double)
                    {
                        return value;
                    }
                    return new XmlAtomicValue(destinationType.SchemaType, ToLong(value.ValueAsDouble));
            }
            return value;
        }

        public static IList<XPathNavigator> EnsureNodeSet(IList<XPathItem> listItems)
        {
            if (listItems.Count == 1)
            {
                XPathItem item = listItems[0];
                if (!item.IsNode)
                {
                    throw new XslTransformException("XPath_NodeSetExpected", new string[] { string.Empty });
                }
                if (item is RtfNavigator)
                {
                    throw new XslTransformException("XPath_RtfInPathExpr", new string[] { string.Empty });
                }
            }
            return XmlILStorageConverter.ItemsToNavigators(listItems);
        }

        internal static XmlQueryType InferXsltType(Type clrType)
        {
            if (clrType == BooleanType)
            {
                return XmlQueryTypeFactory.BooleanX;
            }
            if (clrType == ByteType)
            {
                return XmlQueryTypeFactory.DoubleX;
            }
            if (clrType == DecimalType)
            {
                return XmlQueryTypeFactory.DoubleX;
            }
            if (clrType == DateTimeType)
            {
                return XmlQueryTypeFactory.StringX;
            }
            if (clrType == DoubleType)
            {
                return XmlQueryTypeFactory.DoubleX;
            }
            if (clrType == Int16Type)
            {
                return XmlQueryTypeFactory.DoubleX;
            }
            if (clrType == Int32Type)
            {
                return XmlQueryTypeFactory.DoubleX;
            }
            if (clrType == Int64Type)
            {
                return XmlQueryTypeFactory.DoubleX;
            }
            if (clrType == IXPathNavigableType)
            {
                return XmlQueryTypeFactory.NodeNotRtf;
            }
            if (clrType == SByteType)
            {
                return XmlQueryTypeFactory.DoubleX;
            }
            if (clrType == SingleType)
            {
                return XmlQueryTypeFactory.DoubleX;
            }
            if (clrType == StringType)
            {
                return XmlQueryTypeFactory.StringX;
            }
            if (clrType == UInt16Type)
            {
                return XmlQueryTypeFactory.DoubleX;
            }
            if (clrType == UInt32Type)
            {
                return XmlQueryTypeFactory.DoubleX;
            }
            if (clrType == UInt64Type)
            {
                return XmlQueryTypeFactory.DoubleX;
            }
            if (clrType == XPathNavigatorArrayType)
            {
                return XmlQueryTypeFactory.NodeDodS;
            }
            if (clrType == XPathNavigatorType)
            {
                return XmlQueryTypeFactory.NodeNotRtf;
            }
            if (clrType == XPathNodeIteratorType)
            {
                return XmlQueryTypeFactory.NodeDodS;
            }
            if (clrType.IsEnum)
            {
                return XmlQueryTypeFactory.DoubleX;
            }
            if (clrType == VoidType)
            {
                return XmlQueryTypeFactory.Empty;
            }
            return XmlQueryTypeFactory.ItemS;
        }

        public static bool ToBoolean(IList<XPathItem> listItems)
        {
            if (listItems.Count == 0)
            {
                return false;
            }
            return ToBoolean(listItems[0]);
        }

        public static bool ToBoolean(XPathItem item)
        {
            if (!item.IsNode)
            {
                Type valueType = item.ValueType;
                if (valueType == StringType)
                {
                    return (item.Value.Length != 0);
                }
                if (valueType != DoubleType)
                {
                    return item.ValueAsBoolean;
                }
                double valueAsDouble = item.ValueAsDouble;
                if (valueAsDouble >= 0.0)
                {
                    return (0.0 < valueAsDouble);
                }
            }
            return true;
        }

        public static DateTime ToDateTime(string value) => 
            ((DateTime) new XsdDateTime(value, XsdDateTimeFlags.AllXsd));

        public static decimal ToDecimal(double value) => 
            ((decimal) value);

        public static double ToDouble(IList<XPathItem> listItems)
        {
            if (listItems.Count == 0)
            {
                return double.NaN;
            }
            return ToDouble(listItems[0]);
        }

        public static double ToDouble(decimal value) => 
            ((double) value);

        public static double ToDouble(int value) => 
            ((double) value);

        public static double ToDouble(long value) => 
            ((double) value);

        public static double ToDouble(string value) => 
            XPathConvert.StringToDouble(value);

        public static double ToDouble(XPathItem item)
        {
            if (item.IsNode)
            {
                return XPathConvert.StringToDouble(item.Value);
            }
            Type valueType = item.ValueType;
            if (valueType == StringType)
            {
                return XPathConvert.StringToDouble(item.Value);
            }
            if (valueType == DoubleType)
            {
                return item.ValueAsDouble;
            }
            if (!item.ValueAsBoolean)
            {
                return 0.0;
            }
            return 1.0;
        }

        public static int ToInt(double value) => 
            ((int) value);

        public static long ToLong(double value) => 
            ((long) value);

        public static XPathNavigator ToNode(IList<XPathItem> listItems)
        {
            if (listItems.Count != 1)
            {
                throw new XslTransformException("Xslt_NodeSetNotNode", new string[] { string.Empty });
            }
            return ToNode(listItems[0]);
        }

        public static XPathNavigator ToNode(XPathItem item)
        {
            if (!item.IsNode)
            {
                XPathDocument document = new XPathDocument();
                XmlRawWriter writer = document.LoadFromWriter(XPathDocument.LoadFlags.AtomizeNames, string.Empty);
                writer.WriteString(ToString(item));
                writer.Close();
                return document.CreateNavigator();
            }
            RtfNavigator navigator = item as RtfNavigator;
            if (navigator != null)
            {
                return navigator.ToNavigator();
            }
            return (XPathNavigator) item;
        }

        public static IList<XPathNavigator> ToNodeSet(IList<XPathItem> listItems)
        {
            if (listItems.Count == 1)
            {
                return new XmlQueryNodeSequence(ToNode(listItems[0]));
            }
            return XmlILStorageConverter.ItemsToNavigators(listItems);
        }

        public static IList<XPathNavigator> ToNodeSet(XPathItem item) => 
            new XmlQueryNodeSequence(ToNode(item));

        public static string ToString(IList<XPathItem> listItems)
        {
            if (listItems.Count == 0)
            {
                return string.Empty;
            }
            return ToString(listItems[0]);
        }

        public static string ToString(DateTime value)
        {
            XsdDateTime time = new XsdDateTime(value, XsdDateTimeFlags.DateTime);
            return time.ToString();
        }

        public static string ToString(double value) => 
            XPathConvert.DoubleToString(value);

        public static string ToString(XPathItem item)
        {
            if (!item.IsNode && (item.ValueType == DoubleType))
            {
                return XPathConvert.DoubleToString(item.ValueAsDouble);
            }
            return item.Value;
        }
    }
}

