namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.XPath;
    using System.Xml.Xsl.Runtime;

    internal class XmlILStorageMethods
    {
        public MethodInfo AggAvg;
        public MethodInfo AggAvgResult;
        public MethodInfo AggCreate;
        public MethodInfo AggIsEmpty;
        public MethodInfo AggMax;
        public MethodInfo AggMaxResult;
        public MethodInfo AggMin;
        public MethodInfo AggMinResult;
        public MethodInfo AggSum;
        public MethodInfo AggSumResult;
        public MethodInfo IListCount;
        public MethodInfo IListItem;
        public Type IListType;
        public MethodInfo SeqAdd;
        public FieldInfo SeqEmpty;
        public MethodInfo SeqReuse;
        public MethodInfo SeqReuseSgl;
        public MethodInfo SeqSortByKeys;
        public Type SeqType;
        public MethodInfo ToAtomicValue;
        public MethodInfo ValueAs;

        public XmlILStorageMethods(Type storageType)
        {
            if (((storageType == typeof(int)) || (storageType == typeof(long))) || ((storageType == typeof(decimal)) || (storageType == typeof(double))))
            {
                Type className = Type.GetType("System.Xml.Xsl.Runtime." + storageType.Name + "Aggregator");
                this.AggAvg = XmlILMethods.GetMethod(className, "Average");
                this.AggAvgResult = XmlILMethods.GetMethod(className, "get_AverageResult");
                this.AggCreate = XmlILMethods.GetMethod(className, "Create");
                this.AggIsEmpty = XmlILMethods.GetMethod(className, "get_IsEmpty");
                this.AggMax = XmlILMethods.GetMethod(className, "Maximum");
                this.AggMaxResult = XmlILMethods.GetMethod(className, "get_MaximumResult");
                this.AggMin = XmlILMethods.GetMethod(className, "Minimum");
                this.AggMinResult = XmlILMethods.GetMethod(className, "get_MinimumResult");
                this.AggSum = XmlILMethods.GetMethod(className, "Sum");
                this.AggSumResult = XmlILMethods.GetMethod(className, "get_SumResult");
            }
            if (storageType == typeof(XPathNavigator))
            {
                this.SeqType = typeof(XmlQueryNodeSequence);
                this.SeqAdd = XmlILMethods.GetMethod(this.SeqType, "AddClone");
            }
            else if (storageType == typeof(XPathItem))
            {
                this.SeqType = typeof(XmlQueryItemSequence);
                this.SeqAdd = XmlILMethods.GetMethod(this.SeqType, "AddClone");
            }
            else
            {
                this.SeqType = typeof(XmlQuerySequence<>).MakeGenericType(new Type[] { storageType });
                this.SeqAdd = XmlILMethods.GetMethod(this.SeqType, "Add");
            }
            this.SeqEmpty = this.SeqType.GetField("Empty");
            this.SeqReuse = XmlILMethods.GetMethod(this.SeqType, "CreateOrReuse", new Type[] { this.SeqType });
            this.SeqReuseSgl = XmlILMethods.GetMethod(this.SeqType, "CreateOrReuse", new Type[] { this.SeqType, storageType });
            this.SeqSortByKeys = XmlILMethods.GetMethod(this.SeqType, "SortByKeys");
            this.IListType = typeof(IList<>).MakeGenericType(new Type[] { storageType });
            this.IListItem = XmlILMethods.GetMethod(this.IListType, "get_Item");
            this.IListCount = XmlILMethods.GetMethod(typeof(ICollection<>).MakeGenericType(new Type[] { storageType }), "get_Count");
            if (storageType == typeof(string))
            {
                this.ValueAs = XmlILMethods.GetMethod(typeof(XPathItem), "get_Value");
            }
            else if (storageType == typeof(int))
            {
                this.ValueAs = XmlILMethods.GetMethod(typeof(XPathItem), "get_ValueAsInt");
            }
            else if (storageType == typeof(long))
            {
                this.ValueAs = XmlILMethods.GetMethod(typeof(XPathItem), "get_ValueAsLong");
            }
            else if (storageType == typeof(DateTime))
            {
                this.ValueAs = XmlILMethods.GetMethod(typeof(XPathItem), "get_ValueAsDateTime");
            }
            else if (storageType == typeof(double))
            {
                this.ValueAs = XmlILMethods.GetMethod(typeof(XPathItem), "get_ValueAsDouble");
            }
            else if (storageType == typeof(bool))
            {
                this.ValueAs = XmlILMethods.GetMethod(typeof(XPathItem), "get_ValueAsBoolean");
            }
            if (storageType == typeof(byte[]))
            {
                this.ToAtomicValue = XmlILMethods.GetMethod(typeof(XmlILStorageConverter), "BytesToAtomicValue");
            }
            else if ((storageType != typeof(XPathItem)) && (storageType != typeof(XPathNavigator)))
            {
                this.ToAtomicValue = XmlILMethods.GetMethod(typeof(XmlILStorageConverter), storageType.Name + "ToAtomicValue");
            }
        }
    }
}

