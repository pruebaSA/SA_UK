namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.XPath;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class XmlILStorageConverter
    {
        public static XmlAtomicValue BooleanToAtomicValue(bool value, int index, XmlQueryRuntime runtime) => 
            new XmlAtomicValue(runtime.GetXmlType(index).SchemaType, value);

        public static XmlAtomicValue BytesToAtomicValue(byte[] value, int index, XmlQueryRuntime runtime) => 
            new XmlAtomicValue(runtime.GetXmlType(index).SchemaType, value);

        public static XmlAtomicValue DateTimeToAtomicValue(DateTime value, int index, XmlQueryRuntime runtime) => 
            new XmlAtomicValue(runtime.GetXmlType(index).SchemaType, value);

        public static XmlAtomicValue DecimalToAtomicValue(decimal value, int index, XmlQueryRuntime runtime) => 
            new XmlAtomicValue(runtime.GetXmlType(index).SchemaType, value);

        public static XmlAtomicValue DoubleToAtomicValue(double value, int index, XmlQueryRuntime runtime) => 
            new XmlAtomicValue(runtime.GetXmlType(index).SchemaType, value);

        public static XmlAtomicValue Int32ToAtomicValue(int value, int index, XmlQueryRuntime runtime) => 
            new XmlAtomicValue(runtime.GetXmlType(index).SchemaType, value);

        public static XmlAtomicValue Int64ToAtomicValue(long value, int index, XmlQueryRuntime runtime) => 
            new XmlAtomicValue(runtime.GetXmlType(index).SchemaType, value);

        public static IList<XPathNavigator> ItemsToNavigators(IList<XPathItem> listItems)
        {
            IList<XPathNavigator> list = listItems as IList<XPathNavigator>;
            if (list != null)
            {
                return list;
            }
            XmlQueryNodeSequence sequence = new XmlQueryNodeSequence(listItems.Count);
            for (int i = 0; i < listItems.Count; i++)
            {
                sequence.Add((XPathNavigator) listItems[i]);
            }
            return sequence;
        }

        public static IList<XPathItem> NavigatorsToItems(IList<XPathNavigator> listNavigators)
        {
            IList<XPathItem> list = listNavigators as IList<XPathItem>;
            if (list != null)
            {
                return list;
            }
            return new XmlQueryNodeSequence(listNavigators);
        }

        public static XmlAtomicValue SingleToAtomicValue(float value, int index, XmlQueryRuntime runtime) => 
            new XmlAtomicValue(runtime.GetXmlType(index).SchemaType, (double) value);

        public static XmlAtomicValue StringToAtomicValue(string value, int index, XmlQueryRuntime runtime) => 
            new XmlAtomicValue(runtime.GetXmlType(index).SchemaType, value);

        public static XmlAtomicValue TimeSpanToAtomicValue(TimeSpan value, int index, XmlQueryRuntime runtime) => 
            new XmlAtomicValue(runtime.GetXmlType(index).SchemaType, value);

        public static XmlAtomicValue XmlQualifiedNameToAtomicValue(XmlQualifiedName value, int index, XmlQueryRuntime runtime) => 
            new XmlAtomicValue(runtime.GetXmlType(index).SchemaType, value);
    }
}

