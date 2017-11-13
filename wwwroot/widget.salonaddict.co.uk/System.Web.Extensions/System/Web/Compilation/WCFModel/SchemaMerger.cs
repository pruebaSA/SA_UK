namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Web.Resources;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    internal class SchemaMerger
    {
        private static readonly object[] emptyCollection = new object[0];
        private static readonly System.Xml.XmlAttribute[] emptyXmlAttributeCollection = new System.Xml.XmlAttribute[0];
        private static Type[] ignorablePropertyTypes = new Type[] { typeof(System.Xml.XmlAttribute[]), typeof(XmlElement[]), typeof(System.Xml.XmlNode[]), typeof(XmlSchemaAnnotation) };
        private static SchemaTopLevelItemType[] schemaTopLevelItemTypes = new SchemaTopLevelItemType[] { new SchemaTopLevelItemType(typeof(XmlSchemaType), "type"), new SchemaTopLevelItemType(typeof(XmlSchemaElement), "element"), new SchemaTopLevelItemType(typeof(XmlSchemaAttribute), "attribute"), new SchemaTopLevelItemType(typeof(XmlSchemaGroup), "group"), new SchemaTopLevelItemType(typeof(XmlSchemaAttributeGroup), "attributeGroup") };
        private static Type[] xmlSerializationAttributes = new Type[] { typeof(XmlElementAttribute), typeof(XmlAttributeAttribute), typeof(XmlAnyAttributeAttribute), typeof(XmlAnyElementAttribute), typeof(XmlTextAttribute) };

        private static bool AreSchemaObjectsEquivalent(XmlSchemaObject originalItem, XmlSchemaObject item, out string differentLocation)
        {
            differentLocation = string.Empty;
            Type type = originalItem.GetType();
            if (type != item.GetType())
            {
                return false;
            }
            string str = string.Empty;
            foreach (PropertyInfo info in type.GetProperties())
            {
                if (IsPersistedProperty(info))
                {
                    bool flag = ShouldIgnoreSchemaProperty(info);
                    object originalValue = info.GetValue(originalItem, new object[0]);
                    object newValue = info.GetValue(item, new object[0]);
                    if (!CompareSchemaPropertyValues(info, originalValue, newValue, out differentLocation) && !flag)
                    {
                        return false;
                    }
                    if (string.IsNullOrEmpty(str))
                    {
                        str = differentLocation;
                    }
                }
            }
            differentLocation = str;
            return true;
        }

        private static string CombinePath(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1))
            {
                return path2;
            }
            if (string.IsNullOrEmpty(path2))
            {
                return path1;
            }
            return (path1 + "/" + path2);
        }

        private static string CombineTwoNames(string name1, string name2)
        {
            string str = string.Empty;
            if (name1.Length > 0)
            {
                if (name2.Length <= 0)
                {
                    return name1;
                }
                if (string.Equals(name1, name2, StringComparison.Ordinal))
                {
                    return name1;
                }
                return (name1 + "|" + name2);
            }
            if (name2.Length > 0)
            {
                str = name2;
            }
            return str;
        }

        private static bool CompareSchemaCollections(IEnumerable originalCollection, IEnumerable newCollection, out object differentItem1, out object differentItem2, out string differentLocation)
        {
            differentLocation = string.Empty;
            IEnumerator enumerator = originalCollection.GetEnumerator();
            IEnumerator enumerator2 = newCollection.GetEnumerator();
            string str = string.Empty;
            object obj2 = null;
            object obj3 = null;
            do
            {
                differentItem1 = enumerator.MoveNext() ? enumerator.Current : null;
                differentItem2 = enumerator2.MoveNext() ? enumerator2.Current : null;
                if (!CompareSchemaValues(differentItem1, differentItem2, out differentLocation))
                {
                    return false;
                }
                if (string.IsNullOrEmpty(str))
                {
                    obj2 = differentItem1;
                    obj3 = differentItem2;
                    str = differentLocation;
                }
            }
            while ((differentItem1 != null) && (differentItem2 != null));
            differentLocation = str;
            differentItem1 = obj2;
            differentItem2 = obj3;
            return true;
        }

        private static bool CompareSchemaPropertyValues(PropertyInfo propertyInfo, object originalValue, object newValue, out string differentLocation)
        {
            differentLocation = string.Empty;
            if ((originalValue != null) || (newValue != null))
            {
                if (typeof(System.Xml.XmlAttribute[]) == propertyInfo.PropertyType)
                {
                    System.Xml.XmlAttribute attribute;
                    System.Xml.XmlAttribute attribute2;
                    if (originalValue == null)
                    {
                        originalValue = emptyXmlAttributeCollection;
                    }
                    if (newValue == null)
                    {
                        newValue = emptyXmlAttributeCollection;
                    }
                    if (!CompareXmlAttributeCollections((System.Xml.XmlAttribute[]) originalValue, (System.Xml.XmlAttribute[]) newValue, out attribute, out attribute2))
                    {
                        differentLocation = GetSchemaPropertyNameInXml(propertyInfo, attribute, attribute2);
                        return false;
                    }
                    return true;
                }
                if (typeof(ICollection).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    object obj2;
                    object obj3;
                    if (originalValue == null)
                    {
                        originalValue = emptyCollection;
                    }
                    if (newValue == null)
                    {
                        newValue = emptyCollection;
                    }
                    if (!CompareSchemaCollections((ICollection) originalValue, (ICollection) newValue, out obj2, out obj3, out differentLocation))
                    {
                        differentLocation = CombinePath(GetSchemaPropertyNameInXml(propertyInfo, obj2, obj3), differentLocation);
                        return false;
                    }
                    if (!string.IsNullOrEmpty(differentLocation))
                    {
                        differentLocation = CombinePath(GetSchemaPropertyNameInXml(propertyInfo, obj2, obj3), differentLocation);
                    }
                    return true;
                }
                if ((originalValue == null) || (newValue == null))
                {
                    differentLocation = CombinePath(GetSchemaPropertyNameInXml(propertyInfo, originalValue, newValue), differentLocation);
                    return false;
                }
                if (originalValue.GetType() != newValue.GetType())
                {
                    differentLocation = CombinePath(GetSchemaPropertyNameInXml(propertyInfo, originalValue, newValue), differentLocation);
                    return false;
                }
                if (!CompareSchemaValues(originalValue, newValue, out differentLocation))
                {
                    differentLocation = CombinePath(GetSchemaPropertyNameInXml(propertyInfo, originalValue, newValue), differentLocation);
                    return false;
                }
                if (!string.IsNullOrEmpty(differentLocation))
                {
                    differentLocation = CombinePath(GetSchemaPropertyNameInXml(propertyInfo, originalValue, newValue), differentLocation);
                }
            }
            return true;
        }

        private static bool CompareSchemaValues(object originalValue, object newValue, out string differentLocation)
        {
            differentLocation = string.Empty;
            if ((originalValue == null) || (newValue == null))
            {
                return ((originalValue == null) && (newValue == null));
            }
            if (originalValue.GetType() != newValue.GetType())
            {
                return false;
            }
            if (originalValue is XmlSchemaObject)
            {
                return AreSchemaObjectsEquivalent((XmlSchemaObject) originalValue, (XmlSchemaObject) newValue, out differentLocation);
            }
            if (originalValue is System.Xml.XmlAttribute)
            {
                return CompareXmlAttributes((System.Xml.XmlAttribute) originalValue, (System.Xml.XmlAttribute) newValue);
            }
            if (originalValue is XmlElement)
            {
                return CompareXmlElements((XmlElement) originalValue, (XmlElement) newValue, out differentLocation);
            }
            if (originalValue is XmlText)
            {
                return CompareXmlTexts((XmlText) originalValue, (XmlText) newValue);
            }
            return originalValue.Equals(newValue);
        }

        private static bool CompareXmlAttributeCollections(ICollection attributeCollection1, ICollection attributeCollection2, out System.Xml.XmlAttribute differentAttribute1, out System.Xml.XmlAttribute differentAttribute2)
        {
            object obj2;
            object obj3;
            string str;
            differentAttribute1 = null;
            differentAttribute2 = null;
            System.Xml.XmlAttribute[] sortedAttributeArray = GetSortedAttributeArray(attributeCollection1);
            System.Xml.XmlAttribute[] newCollection = GetSortedAttributeArray(attributeCollection2);
            if (!CompareSchemaCollections(sortedAttributeArray, newCollection, out obj2, out obj3, out str))
            {
                differentAttribute1 = (System.Xml.XmlAttribute) obj2;
                differentAttribute2 = (System.Xml.XmlAttribute) obj3;
                return false;
            }
            return true;
        }

        private static bool CompareXmlAttributes(System.Xml.XmlAttribute attribute1, System.Xml.XmlAttribute attribute2) => 
            ((string.Equals(attribute1.LocalName, attribute2.LocalName, StringComparison.Ordinal) && string.Equals(attribute1.NamespaceURI, attribute2.NamespaceURI, StringComparison.Ordinal)) && string.Equals(attribute1.Value, attribute2.Value, StringComparison.Ordinal));

        private static bool CompareXmlElements(XmlElement element1, XmlElement element2, out string differentLocation)
        {
            System.Xml.XmlAttribute attribute;
            System.Xml.XmlAttribute attribute2;
            object obj2;
            object obj3;
            differentLocation = string.Empty;
            if (!string.Equals(element1.LocalName, element2.LocalName, StringComparison.Ordinal) || !string.Equals(element1.NamespaceURI, element2.NamespaceURI, StringComparison.Ordinal))
            {
                return false;
            }
            if (!CompareXmlAttributeCollections(element1.Attributes, element2.Attributes, out attribute, out attribute2))
            {
                string str = (attribute != null) ? ("@" + attribute.LocalName) : string.Empty;
                string str2 = (attribute2 != null) ? ("@" + attribute2.LocalName) : string.Empty;
                differentLocation = CombineTwoNames(str, str2);
                return false;
            }
            if (!CompareSchemaCollections(element1.ChildNodes, element2.ChildNodes, out obj2, out obj3, out differentLocation))
            {
                string str3 = (obj2 != null) ? ((System.Xml.XmlNode) obj2).LocalName : string.Empty;
                string str4 = (obj3 != null) ? ((System.Xml.XmlNode) obj3).LocalName : string.Empty;
                differentLocation = CombinePath(CombineTwoNames(str3, str4), differentLocation);
                return false;
            }
            return true;
        }

        private static bool CompareXmlTexts(XmlText text1, XmlText text2) => 
            string.Equals(text1.Value, text2.Value, StringComparison.Ordinal);

        private static void FindDuplicatedItems(System.Xml.Schema.XmlSchema schema, Type itemType, string itemTypeName, Dictionary<XmlQualifiedName, XmlSchemaObject> knownItemTable, List<XmlSchemaObject> duplicatedItems, IList<ProxyGenerationError> importErrors)
        {
            string targetNamespace = schema.TargetNamespace;
            if (string.IsNullOrEmpty(targetNamespace))
            {
                targetNamespace = string.Empty;
            }
            foreach (XmlSchemaObject obj2 in schema.Items)
            {
                if (itemType.IsInstanceOfType(obj2))
                {
                    XmlQualifiedName key = new XmlQualifiedName(GetSchemaItemName(obj2), targetNamespace);
                    XmlSchemaObject obj3 = null;
                    if (knownItemTable.TryGetValue(key, out obj3))
                    {
                        string str2;
                        if (!AreSchemaObjectsEquivalent(obj3, obj2, out str2))
                        {
                            str2 = CombinePath(".", str2);
                            importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.MergeMetadata, string.Empty, new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_DuplicatedSchemaItems, new object[] { itemTypeName, key.ToString(), schema.SourceUri, obj3.SourceUri, str2 }))));
                        }
                        else if (!string.IsNullOrEmpty(str2))
                        {
                            str2 = CombinePath(".", str2);
                            importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.MergeMetadata, string.Empty, new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_DuplicatedSchemaItemsIgnored, new object[] { itemTypeName, key.ToString(), schema.SourceUri, obj3.SourceUri, str2 })), true));
                        }
                        duplicatedItems.Add(obj2);
                    }
                    else
                    {
                        obj2.SourceUri = schema.SourceUri;
                        knownItemTable.Add(key, obj2);
                    }
                }
            }
        }

        private static string GetSchemaItemName(XmlSchemaObject item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            PropertyInfo property = item.GetType().GetProperty("Name");
            if (property != null)
            {
                object obj2 = property.GetValue(item, new object[0]);
                if (obj2 is string)
                {
                    return (string) obj2;
                }
            }
            return string.Empty;
        }

        private static string GetSchemaPropertyNameInXml(PropertyInfo property, object value1, object value2)
        {
            object[] customAttributes = property.GetCustomAttributes(true);
            string name = string.Empty;
            if (customAttributes != null)
            {
                string schemaPropertyNameInXmlHelper = GetSchemaPropertyNameInXmlHelper(customAttributes, value1);
                string str3 = GetSchemaPropertyNameInXmlHelper(customAttributes, value2);
                name = CombineTwoNames(schemaPropertyNameInXmlHelper, str3);
            }
            if (string.IsNullOrEmpty(name))
            {
                name = property.Name;
            }
            return name;
        }

        private static string GetSchemaPropertyNameInXmlHelper(object[] propertyAttributes, object value)
        {
            if (value != null)
            {
                foreach (object obj2 in propertyAttributes)
                {
                    if (obj2 is XmlAttributeAttribute)
                    {
                        return ("@" + ((XmlAttributeAttribute) obj2).AttributeName);
                    }
                    if (obj2 is XmlElementAttribute)
                    {
                        XmlElementAttribute attribute = (XmlElementAttribute) obj2;
                        Type type = attribute.Type;
                        if ((type == null) || type.IsInstanceOfType(value))
                        {
                            if (value is XmlSchemaObject)
                            {
                                string schemaItemName = GetSchemaItemName((XmlSchemaObject) value);
                                if (schemaItemName.Length > 0)
                                {
                                    return string.Format(CultureInfo.InvariantCulture, "{0}[@name='{1}']", new object[] { attribute.ElementName, schemaItemName });
                                }
                            }
                            return attribute.ElementName;
                        }
                    }
                    if ((obj2 is XmlAnyAttributeAttribute) && (value is System.Xml.XmlAttribute))
                    {
                        return ("@" + ((System.Xml.XmlAttribute) value).LocalName);
                    }
                    if ((obj2 is XmlAnyElementAttribute) && (value is XmlElement))
                    {
                        return ((XmlElement) value).LocalName;
                    }
                    if ((obj2 is XmlTextAttribute) && (value is XmlText))
                    {
                        return ((XmlText) value).Name;
                    }
                }
            }
            return string.Empty;
        }

        private static System.Xml.XmlAttribute[] GetSortedAttributeArray(ICollection attributeCollection)
        {
            System.Xml.XmlAttribute[] array = new System.Xml.XmlAttribute[attributeCollection.Count];
            int num = 0;
            foreach (System.Xml.XmlAttribute attribute in attributeCollection)
            {
                array[num++] = attribute;
            }
            Array.Sort<System.Xml.XmlAttribute>(array, new AttributeComparer());
            return array;
        }

        private static bool IsPersistedProperty(PropertyInfo property)
        {
            object[] customAttributes = property.GetCustomAttributes(true);
            if (customAttributes != null)
            {
                foreach (object obj2 in customAttributes)
                {
                    foreach (Type type in xmlSerializationAttributes)
                    {
                        if (type.IsInstanceOfType(obj2))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal static void MergeSchemas(IEnumerable<System.Xml.Schema.XmlSchema> schemaList, IList<ProxyGenerationError> importErrors, out IEnumerable<System.Xml.Schema.XmlSchema> duplicatedSchemas)
        {
            if (schemaList == null)
            {
                throw new ArgumentNullException("schemaList");
            }
            if (importErrors == null)
            {
                throw new ArgumentNullException("importErrors");
            }
            List<System.Xml.Schema.XmlSchema> list = new List<System.Xml.Schema.XmlSchema>();
            duplicatedSchemas = list;
            Dictionary<XmlQualifiedName, XmlSchemaObject>[] dictionaryArray = new Dictionary<XmlQualifiedName, XmlSchemaObject>[schemaTopLevelItemTypes.Length];
            for (int i = 0; i < schemaTopLevelItemTypes.Length; i++)
            {
                dictionaryArray[i] = new Dictionary<XmlQualifiedName, XmlSchemaObject>();
            }
            foreach (System.Xml.Schema.XmlSchema schema in schemaList)
            {
                bool flag = false;
                List<XmlSchemaObject> duplicatedItems = new List<XmlSchemaObject>();
                for (int j = 0; j < schemaTopLevelItemTypes.Length; j++)
                {
                    Dictionary<XmlQualifiedName, XmlSchemaObject> knownItemTable = dictionaryArray[j];
                    int count = knownItemTable.Count;
                    FindDuplicatedItems(schema, schemaTopLevelItemTypes[j].ItemType, schemaTopLevelItemTypes[j].Name, knownItemTable, duplicatedItems, importErrors);
                    if (knownItemTable.Count > count)
                    {
                        flag = true;
                    }
                }
                if (duplicatedItems.Count > 0)
                {
                    if (!flag)
                    {
                        list.Add(schema);
                    }
                    else
                    {
                        foreach (XmlSchemaObject obj2 in duplicatedItems)
                        {
                            schema.Items.Remove(obj2);
                        }
                    }
                }
            }
        }

        private static bool ShouldIgnoreSchemaProperty(PropertyInfo property)
        {
            Type propertyType = property.PropertyType;
            foreach (Type type2 in ignorablePropertyTypes)
            {
                if ((propertyType == type2) || propertyType.IsSubclassOf(type2))
                {
                    return true;
                }
            }
            return string.Equals(property.Name, "Constraints", StringComparison.Ordinal);
        }

        private class AttributeComparer : IComparer<System.Xml.XmlAttribute>
        {
            public int Compare(System.Xml.XmlAttribute x, System.Xml.XmlAttribute y)
            {
                int num = string.Compare(x.NamespaceURI, y.NamespaceURI, StringComparison.Ordinal);
                if (num != 0)
                {
                    return num;
                }
                return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SchemaTopLevelItemType
        {
            public Type ItemType;
            public string Name;
            public SchemaTopLevelItemType(Type itemType, string name)
            {
                this.ItemType = itemType;
                this.Name = name;
            }
        }
    }
}

