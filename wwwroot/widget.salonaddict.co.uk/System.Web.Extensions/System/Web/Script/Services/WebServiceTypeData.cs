namespace System.Web.Script.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Schema;

    internal class WebServiceTypeData
    {
        private System.Type _actualType;
        private static Dictionary<XmlQualifiedName, System.Type> _nameToType = new Dictionary<XmlQualifiedName, System.Type>();
        private string _stringRepresentation;
        private string _typeName;
        private string _typeNamespace;
        private static XmlQualifiedName actualTypeAnnotationName;
        private const string ActualTypeLocalName = "ActualType";
        private const string ActualTypeNameAttribute = "Name";
        private const string ActualTypeNamespaceAttribute = "Namespace";
        private static XmlQualifiedName enumerationValueAnnotationName;
        private const string EnumerationValueLocalName = "EnumerationValue";
        private const string OccursUnbounded = "unbounded";
        private const string SchemaNamespace = "http://www.w3.org/2001/XMLSchema";
        private const string SerializationNamespace = "http://schemas.microsoft.com/2003/10/Serialization/";
        private const string StringLocalName = "string";

        static WebServiceTypeData()
        {
            Add(typeof(sbyte), "byte");
            Add(typeof(byte), "unsignedByte");
            Add(typeof(short), "short");
            Add(typeof(ushort), "unsignedShort");
            Add(typeof(int), "int");
            Add(typeof(uint), "unsignedInt");
            Add(typeof(long), "long");
            Add(typeof(ulong), "unsignedLong");
        }

        internal WebServiceTypeData(string name, string ns) : this(name, ns, null)
        {
        }

        internal WebServiceTypeData(string name, string ns, System.Type type)
        {
            if (string.IsNullOrEmpty(ns))
            {
                this._typeName = name;
                if (type == null)
                {
                    this._stringRepresentation = name;
                }
            }
            else
            {
                this._typeName = ns + "." + name;
                if (type == null)
                {
                    this._stringRepresentation = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[] { name, ns });
                }
            }
            this._typeNamespace = ns;
            this._actualType = type;
        }

        private static void Add(System.Type type, string localName)
        {
            XmlQualifiedName key = new XmlQualifiedName(localName, "http://www.w3.org/2001/XMLSchema");
            _nameToType.Add(key, type);
        }

        private static bool CheckIfCollection(XmlSchemaComplexType type)
        {
            if (type == null)
            {
                return false;
            }
            bool flag = false;
            if (type.ContentModel == null)
            {
                flag = CheckIfCollectionSequence(type.Particle as XmlSchemaSequence);
            }
            return flag;
        }

        private static bool CheckIfCollectionSequence(XmlSchemaSequence rootSequence)
        {
            if ((rootSequence.Items == null) || (rootSequence.Items.Count == 0))
            {
                return false;
            }
            if (rootSequence.Items.Count != 1)
            {
                return false;
            }
            XmlSchemaObject obj2 = rootSequence.Items[0];
            if (!(obj2 is XmlSchemaElement))
            {
                return false;
            }
            XmlSchemaElement element = (XmlSchemaElement) obj2;
            if (element.MaxOccursString != "unbounded")
            {
                return (element.MaxOccurs > 1M);
            }
            return true;
        }

        private static bool CheckIfEnum(XmlSchemaSimpleType simpleType, out XmlSchemaSimpleTypeRestriction simpleTypeRestriction)
        {
            simpleTypeRestriction = null;
            if (simpleType != null)
            {
                XmlSchemaSimpleTypeRestriction content = simpleType.Content as XmlSchemaSimpleTypeRestriction;
                if (content != null)
                {
                    simpleTypeRestriction = content;
                    return CheckIfEnumRestriction(content);
                }
                XmlSchemaSimpleTypeList list = simpleType.Content as XmlSchemaSimpleTypeList;
                XmlSchemaSimpleType itemType = list.ItemType;
                if (itemType != null)
                {
                    content = itemType.Content as XmlSchemaSimpleTypeRestriction;
                    if (content != null)
                    {
                        simpleTypeRestriction = content;
                        return CheckIfEnumRestriction(content);
                    }
                }
            }
            return false;
        }

        private static bool CheckIfEnumRestriction(XmlSchemaSimpleTypeRestriction restriction)
        {
            foreach (XmlSchemaFacet facet in restriction.Facets)
            {
                if (!(facet is XmlSchemaEnumerationFacet))
                {
                    return false;
                }
            }
            if (!(restriction.BaseTypeName != XmlQualifiedName.Empty))
            {
                return false;
            }
            return (((restriction.BaseTypeName.Name == "string") && (restriction.BaseTypeName.Namespace == "http://www.w3.org/2001/XMLSchema")) && (restriction.Facets.Count > 0));
        }

        private static string GetInnerText(XmlQualifiedName typeName, XmlElement xmlElement)
        {
            if (xmlElement == null)
            {
                return null;
            }
            for (System.Xml.XmlNode node = xmlElement.FirstChild; node != null; node = node.NextSibling)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    throw new InvalidOperationException();
                }
            }
            return xmlElement.InnerText;
        }

        internal static List<WebServiceTypeData> GetKnownTypes(System.Type type, WebServiceTypeData typeData)
        {
            List<WebServiceTypeData> list = new List<WebServiceTypeData>();
            XsdDataContractExporter exporter = new XsdDataContractExporter();
            exporter.Export(type);
            foreach (XmlSchema schema in exporter.Schemas.Schemas())
            {
                if (schema.TargetNamespace != "http://schemas.microsoft.com/2003/10/Serialization/")
                {
                    foreach (XmlSchemaType type2 in schema.Items)
                    {
                        string typeNamespace = XmlConvert.DecodeName(schema.TargetNamespace);
                        if (((type2 != null) && ((type2.Name != typeData.TypeName) || (typeNamespace != typeData.TypeNamespace))) && !string.IsNullOrEmpty(type2.Name))
                        {
                            XmlSchemaSimpleTypeRestriction restriction;
                            WebServiceTypeData item = null;
                            if (CheckIfEnum(type2 as XmlSchemaSimpleType, out restriction))
                            {
                                item = ImportEnum(XmlConvert.DecodeName(type2.Name), typeNamespace, type2.QualifiedName, restriction, type2.Annotation);
                            }
                            else
                            {
                                if (CheckIfCollection(type2 as XmlSchemaComplexType))
                                {
                                    continue;
                                }
                                if (!(type2 is XmlSchemaSimpleType))
                                {
                                    item = new WebServiceTypeData(XmlConvert.DecodeName(type2.Name), typeNamespace);
                                }
                            }
                            if (item != null)
                            {
                                list.Add(item);
                            }
                        }
                    }
                }
            }
            return list;
        }

        internal static WebServiceTypeData GetWebServiceTypeData(System.Type type)
        {
            WebServiceTypeData data = null;
            XmlQualifiedName schemaTypeName = new XsdDataContractExporter().GetSchemaTypeName(type);
            if (schemaTypeName.IsEmpty)
            {
                return data;
            }
            if (type.IsEnum)
            {
                return new WebServiceEnumData(XmlConvert.DecodeName(schemaTypeName.Name), XmlConvert.DecodeName(schemaTypeName.Namespace), Enum.GetNames(type), Enum.GetValues(type), Enum.GetUnderlyingType(type) == typeof(ulong));
            }
            return new WebServiceTypeData(XmlConvert.DecodeName(schemaTypeName.Name), XmlConvert.DecodeName(schemaTypeName.Namespace));
        }

        internal static XmlQualifiedName ImportActualType(XmlSchemaAnnotation annotation, XmlQualifiedName defaultTypeName, XmlQualifiedName typeName)
        {
            XmlElement element = ImportAnnotation(annotation, ActualTypeAnnotationName);
            if (element == null)
            {
                return defaultTypeName;
            }
            string name = element.Attributes.GetNamedItem("Name").Value;
            return new XmlQualifiedName(name, element.Attributes.GetNamedItem("Namespace").Value);
        }

        private static XmlElement ImportAnnotation(XmlSchemaAnnotation annotation, XmlQualifiedName annotationQualifiedName)
        {
            if (((annotation != null) && (annotation.Items != null)) && ((annotation.Items.Count > 0) && (annotation.Items[0] is XmlSchemaAppInfo)))
            {
                XmlSchemaAppInfo info = (XmlSchemaAppInfo) annotation.Items[0];
                System.Xml.XmlNode[] markup = info.Markup;
                if (markup != null)
                {
                    for (int i = 0; i < markup.Length; i++)
                    {
                        XmlElement element = markup[i] as XmlElement;
                        if (((element != null) && (element.LocalName == annotationQualifiedName.Name)) && (element.NamespaceURI == annotationQualifiedName.Namespace))
                        {
                            return element;
                        }
                    }
                }
            }
            return null;
        }

        private static WebServiceEnumData ImportEnum(string typeName, string typeNamespace, XmlQualifiedName typeQualifiedName, XmlSchemaSimpleTypeRestriction restriction, XmlSchemaAnnotation annotation)
        {
            XmlQualifiedName name = ImportActualType(annotation, new XmlQualifiedName("int", "http://www.w3.org/2001/XMLSchema"), typeQualifiedName);
            System.Type type = _nameToType[name];
            bool isULong = type == typeof(ulong);
            List<string> list = new List<string>();
            List<long> list2 = new List<long>();
            foreach (XmlSchemaFacet facet in restriction.Facets)
            {
                long count;
                XmlSchemaEnumerationFacet facet2 = facet as XmlSchemaEnumerationFacet;
                string innerText = GetInnerText(typeQualifiedName, ImportAnnotation(facet2.Annotation, EnumerationValueAnnotationName));
                if (innerText == null)
                {
                    count = list.Count;
                }
                else if (isULong)
                {
                    count = (long) ulong.Parse(innerText, NumberFormatInfo.InvariantInfo);
                }
                else
                {
                    count = long.Parse(innerText, NumberFormatInfo.InvariantInfo);
                }
                list.Add(facet2.Value);
                list2.Add(count);
            }
            return new WebServiceEnumData(typeName, typeNamespace, list.ToArray(), list2.ToArray(), isULong);
        }

        private static XmlQualifiedName ActualTypeAnnotationName
        {
            get
            {
                if (actualTypeAnnotationName == null)
                {
                    actualTypeAnnotationName = new XmlQualifiedName("ActualType", "http://schemas.microsoft.com/2003/10/Serialization/");
                }
                return actualTypeAnnotationName;
            }
        }

        private static XmlQualifiedName EnumerationValueAnnotationName
        {
            get
            {
                if (enumerationValueAnnotationName == null)
                {
                    enumerationValueAnnotationName = new XmlQualifiedName("EnumerationValue", "http://schemas.microsoft.com/2003/10/Serialization/");
                }
                return enumerationValueAnnotationName;
            }
        }

        internal string StringRepresentation =>
            this._stringRepresentation;

        internal System.Type Type =>
            this._actualType;

        internal string TypeName =>
            this._typeName;

        internal string TypeNamespace =>
            this._typeNamespace;
    }
}

