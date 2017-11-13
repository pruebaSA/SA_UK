namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Collections.Generic;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class XmlILTypeHelper
    {
        private static readonly Type[] TypeCodeToCachedStorage = new Type[] { 
            typeof(IList<XPathItem>), typeof(IList<XPathItem>), typeof(IList<XPathNavigator>), typeof(IList<XPathNavigator>), typeof(IList<XPathNavigator>), typeof(IList<XPathNavigator>), typeof(IList<XPathNavigator>), typeof(IList<XPathNavigator>), typeof(IList<XPathNavigator>), typeof(IList<XPathNavigator>), typeof(IList<XPathItem>), typeof(IList<string>), typeof(IList<string>), typeof(IList<bool>), typeof(IList<decimal>), typeof(IList<float>),
            typeof(IList<double>), typeof(IList<string>), typeof(IList<DateTime>), typeof(IList<DateTime>), typeof(IList<DateTime>), typeof(IList<DateTime>), typeof(IList<DateTime>), typeof(IList<DateTime>), typeof(IList<DateTime>), typeof(IList<DateTime>), typeof(IList<byte[]>), typeof(IList<byte[]>), typeof(IList<string>), typeof(IList<XmlQualifiedName>), typeof(IList<XmlQualifiedName>), typeof(IList<string>),
            typeof(IList<string>), typeof(IList<string>), typeof(IList<string>), typeof(IList<string>), typeof(IList<string>), typeof(IList<string>), typeof(IList<string>), typeof(IList<string>), typeof(IList<long>), typeof(IList<decimal>), typeof(IList<decimal>), typeof(IList<long>), typeof(IList<int>), typeof(IList<int>), typeof(IList<int>), typeof(IList<decimal>),
            typeof(IList<decimal>), typeof(IList<long>), typeof(IList<int>), typeof(IList<int>), typeof(IList<decimal>), typeof(IList<TimeSpan>), typeof(IList<TimeSpan>)
        };
        private static readonly Type[] TypeCodeToStorage = new Type[] { 
            typeof(XPathItem), typeof(XPathItem), typeof(XPathNavigator), typeof(XPathNavigator), typeof(XPathNavigator), typeof(XPathNavigator), typeof(XPathNavigator), typeof(XPathNavigator), typeof(XPathNavigator), typeof(XPathNavigator), typeof(XPathItem), typeof(string), typeof(string), typeof(bool), typeof(decimal), typeof(float),
            typeof(double), typeof(string), typeof(DateTime), typeof(DateTime), typeof(DateTime), typeof(DateTime), typeof(DateTime), typeof(DateTime), typeof(DateTime), typeof(DateTime), typeof(byte[]), typeof(byte[]), typeof(string), typeof(XmlQualifiedName), typeof(XmlQualifiedName), typeof(string),
            typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(long), typeof(decimal), typeof(decimal), typeof(long), typeof(int), typeof(int), typeof(int), typeof(decimal),
            typeof(decimal), typeof(long), typeof(int), typeof(int), typeof(decimal), typeof(TimeSpan), typeof(TimeSpan)
        };

        private XmlILTypeHelper()
        {
        }

        public static Type GetStorageType(XmlQueryType qyTyp)
        {
            Type type;
            if (qyTyp.IsSingleton)
            {
                type = TypeCodeToStorage[(int) qyTyp.TypeCode];
                if (!qyTyp.IsStrict && (type != typeof(XPathNavigator)))
                {
                    return typeof(XPathItem);
                }
                return type;
            }
            type = TypeCodeToCachedStorage[(int) qyTyp.TypeCode];
            if (!qyTyp.IsStrict && (type != typeof(IList<XPathNavigator>)))
            {
                return typeof(IList<XPathItem>);
            }
            return type;
        }
    }
}

