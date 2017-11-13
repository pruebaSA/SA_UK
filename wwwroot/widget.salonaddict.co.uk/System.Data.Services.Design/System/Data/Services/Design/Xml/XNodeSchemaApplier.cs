namespace System.Data.Services.Design.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;

    internal class XNodeSchemaApplier
    {
        private readonly XmlNamespaceManager namespaceManager;
        private readonly XmlSchemaSet schemas;
        private XmlSchemaValidator validator;
        private readonly XName xsiNilName;
        private readonly XName xsiTypeName;

        private XNodeSchemaApplier(XmlSchemaSet schemas)
        {
            this.schemas = schemas;
            XNamespace namespace2 = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
            this.xsiTypeName = namespace2.GetName("type");
            this.xsiNilName = namespace2.GetName("nil");
            this.namespaceManager = new XmlNamespaceManager(schemas.NameTable);
        }

        internal static void AppendWithCreation<T>(ref List<T> list, T element)
        {
            if (list == null)
            {
                list = new List<T>();
            }
            list.Add(element);
        }

        internal static void Apply(XmlSchemaSet schemas, XElement element)
        {
            new XNodeSchemaApplier(schemas).Validate(element);
        }

        private static string GetTargetNamespace(XmlSchemaObject schemaObject)
        {
            string targetNamespace = null;
            do
            {
                XmlSchema schema = schemaObject as XmlSchema;
                if (schema != null)
                {
                    targetNamespace = schema.TargetNamespace;
                }
                else
                {
                    schemaObject = schemaObject.Parent;
                }
            }
            while (targetNamespace == null);
            return targetNamespace;
        }

        private static bool IsAttributeExpected(XAttribute attribute, XmlSchemaAnyAttribute anyAttribute, XmlSchemaAttribute[] expectedAttributes)
        {
            XmlQualifiedName name = ToQualifiedName(attribute.Name);
            if (name.Namespace.Length == 0)
            {
                foreach (XmlSchemaAttribute attribute2 in expectedAttributes)
                {
                    if (attribute2.Name == name.Name)
                    {
                        return true;
                    }
                }
            }
            if (anyAttribute != null)
            {
                if (anyAttribute.Namespace == "##any")
                {
                    return true;
                }
                string namespaceName = attribute.Name.NamespaceName;
                if ((namespaceName.Length > 0) && (namespaceName != GetTargetNamespace(anyAttribute)))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsElementExpected(XElement element, XmlSchemaParticle[] expectedParticles)
        {
            XmlQualifiedName elementName = ToQualifiedName(element.Name);
            foreach (XmlSchemaParticle particle in expectedParticles)
            {
                if (IsElementExpected(element, elementName, particle))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsElementExpected(XElement element, XmlQualifiedName elementName, XmlSchemaParticle expected)
        {
            XmlSchemaAny any = expected as XmlSchemaAny;
            XmlSchemaElement element2 = expected as XmlSchemaElement;
            if (any != null)
            {
                if (any.Namespace == "##any")
                {
                    return true;
                }
                if ((any.Namespace == "##other") && (element.Name.NamespaceName != GetTargetNamespace(expected)))
                {
                    return true;
                }
            }
            return ((element2 != null) && (element2.QualifiedName == elementName));
        }

        private void PushAncestorsAndSelf(XElement element)
        {
            while (element != null)
            {
                foreach (XAttribute attribute in element.Attributes())
                {
                    if (attribute.IsNamespaceDeclaration)
                    {
                        string localName = attribute.Name.LocalName;
                        if (localName == "xmlns")
                        {
                            localName = string.Empty;
                        }
                        if (!this.namespaceManager.HasNamespace(localName))
                        {
                            this.namespaceManager.AddNamespace(localName, attribute.Value);
                        }
                    }
                }
                element = element.Parent;
            }
        }

        private void PushElement(XElement element, ref string xsiType, ref string xsiNil)
        {
            this.namespaceManager.PushScope();
            foreach (XAttribute attribute in element.Attributes())
            {
                if (attribute.IsNamespaceDeclaration)
                {
                    string localName = attribute.Name.LocalName;
                    if (localName == "xmlns")
                    {
                        localName = string.Empty;
                    }
                    this.namespaceManager.AddNamespace(localName, attribute.Value);
                }
                else
                {
                    XName name = attribute.Name;
                    if (name == this.xsiTypeName)
                    {
                        xsiType = attribute.Value;
                    }
                    else if (name == this.xsiNilName)
                    {
                        xsiNil = attribute.Value;
                    }
                }
            }
        }

        private static XmlQualifiedName ToQualifiedName(XName name) => 
            new XmlQualifiedName(name.LocalName, name.NamespaceName);

        private void TrimAndValidateNodes(XElement parent)
        {
            List<XNode> list = null;
            XmlSchemaParticle[] expectedParticles = null;
            foreach (XNode node in parent.Nodes())
            {
                if (expectedParticles == null)
                {
                    expectedParticles = this.validator.GetExpectedParticles();
                }
                XElement element = node as XElement;
                if (element != null)
                {
                    if (!IsElementExpected(element, expectedParticles))
                    {
                        AppendWithCreation<XNode>(ref list, element);
                    }
                    else
                    {
                        this.ValidateElement(element);
                        expectedParticles = null;
                    }
                }
                else
                {
                    XText text = node as XText;
                    if (text != null)
                    {
                        string elementValue = text.Value;
                        if (elementValue.Length > 0)
                        {
                            this.validator.ValidateText(elementValue);
                            expectedParticles = null;
                        }
                    }
                }
            }
            if (list != null)
            {
                foreach (XNode node2 in list)
                {
                    node2.Remove();
                }
            }
        }

        private void TrimAttributes(XElement element, XmlSchemaAnyAttribute anyAttribute)
        {
            List<XAttribute> list = null;
            XmlSchemaAttribute[] expectedAttributes = this.validator.GetExpectedAttributes();
            foreach (XAttribute attribute in element.Attributes())
            {
                if (!attribute.IsNamespaceDeclaration && !IsAttributeExpected(attribute, anyAttribute, expectedAttributes))
                {
                    AppendWithCreation<XAttribute>(ref list, attribute);
                }
            }
            if (list != null)
            {
                foreach (XAttribute attribute2 in list)
                {
                    attribute2.Remove();
                }
            }
        }

        private void Validate(XElement element)
        {
            XmlSchemaValidationFlags allowXmlAttributes = XmlSchemaValidationFlags.AllowXmlAttributes;
            this.PushAncestorsAndSelf(element.Parent);
            this.validator = new XmlSchemaValidator(this.schemas.NameTable, this.schemas, this.namespaceManager, allowXmlAttributes);
            this.validator.XmlResolver = null;
            this.validator.Initialize();
            this.ValidateElement(element);
            this.validator.EndValidation();
        }

        private void ValidateAttributes(XElement element)
        {
            foreach (XAttribute attribute in element.Attributes())
            {
                if (!attribute.IsNamespaceDeclaration)
                {
                    this.validator.ValidateAttribute(attribute.Name.LocalName, attribute.Name.NamespaceName, attribute.Value, null);
                }
            }
        }

        private void ValidateElement(XElement e)
        {
            XmlSchemaInfo schemaInfo = new XmlSchemaInfo();
            string xsiType = null;
            string xsiNil = null;
            this.PushElement(e, ref xsiType, ref xsiNil);
            this.validator.ValidateElement(e.Name.LocalName, e.Name.NamespaceName, schemaInfo, xsiType, xsiNil, null, null);
            if (schemaInfo.SchemaElement != null)
            {
                XmlSchemaComplexType elementSchemaType = schemaInfo.SchemaElement.ElementSchemaType as XmlSchemaComplexType;
                this.TrimAttributes(e, elementSchemaType?.AttributeWildcard);
                this.ValidateAttributes(e);
                this.validator.ValidateEndOfAttributes(null);
                this.TrimAndValidateNodes(e);
            }
            this.validator.ValidateEndElement(null);
            this.namespaceManager.PopScope();
        }
    }
}

