namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class XmlQueryContext
    {
        private XsltArgumentList argList;
        private Hashtable dataSourceCache;
        private XmlResolver dataSources;
        private XPathNavigator defaultDataSource;
        private XmlExtensionFunctionTable extFuncsLate;
        private QueryReaderSettings readerSettings;
        private XmlQueryRuntime runtime;
        private WhitespaceRuleLookup wsRules;

        internal XmlQueryContext(XmlQueryRuntime runtime, object defaultDataSource, XmlResolver dataSources, XsltArgumentList argList, WhitespaceRuleLookup wsRules)
        {
            this.runtime = runtime;
            this.dataSources = dataSources;
            this.dataSourceCache = new Hashtable();
            this.argList = argList;
            this.wsRules = wsRules;
            if (defaultDataSource is XmlReader)
            {
                this.readerSettings = new QueryReaderSettings((XmlReader) defaultDataSource);
            }
            else
            {
                this.readerSettings = new QueryReaderSettings(new NameTable());
            }
            if (defaultDataSource is string)
            {
                this.defaultDataSource = this.GetDataSource(defaultDataSource as string, null);
                if (this.defaultDataSource == null)
                {
                    throw new XslTransformException("XmlIl_UnknownDocument", new string[] { defaultDataSource as string });
                }
            }
            else if (defaultDataSource != null)
            {
                this.defaultDataSource = this.ConstructDocument(defaultDataSource, null, null);
            }
        }

        private XPathNavigator ConstructDocument(object dataSource, string uriRelative, Uri uriResolved)
        {
            Stream stream = dataSource as Stream;
            if (stream != null)
            {
                XmlReader baseReader = this.readerSettings.CreateReader(stream, uriResolved?.ToString());
                try
                {
                    return new XPathDocument(WhitespaceRuleReader.CreateReader(baseReader, this.wsRules), XmlSpace.Preserve).CreateNavigator();
                }
                finally
                {
                    baseReader.Close();
                }
            }
            if (dataSource is XmlReader)
            {
                return new XPathDocument(WhitespaceRuleReader.CreateReader(dataSource as XmlReader, this.wsRules), XmlSpace.Preserve).CreateNavigator();
            }
            if (!(dataSource is IXPathNavigable))
            {
                throw new XslTransformException("XmlIl_CantResolveEntity", new string[] { uriRelative, dataSource.GetType().ToString() });
            }
            if (this.wsRules != null)
            {
                throw new XslTransformException("XmlIl_CantStripNav", new string[] { string.Empty });
            }
            return (dataSource as IXPathNavigable).CreateNavigator();
        }

        public XPathNavigator GetDataSource(string uriRelative, string uriBase)
        {
            XPathNavigator navigator = null;
            try
            {
                Uri baseUri = (uriBase != null) ? this.dataSources.ResolveUri(null, uriBase) : null;
                Uri absoluteUri = this.dataSources.ResolveUri(baseUri, uriRelative);
                if (absoluteUri != null)
                {
                    navigator = this.dataSourceCache[absoluteUri] as XPathNavigator;
                }
                if (navigator == null)
                {
                    object dataSource = this.dataSources.GetEntity(absoluteUri, null, null);
                    if (dataSource != null)
                    {
                        navigator = this.ConstructDocument(dataSource, uriRelative, absoluteUri);
                        this.dataSourceCache.Add(absoluteUri, navigator);
                    }
                }
            }
            catch (XslTransformException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (!XmlException.IsCatchableException(exception))
                {
                    throw;
                }
                throw new XslTransformException(exception, "XmlIl_DocumentLoadError", new string[] { uriRelative });
            }
            return navigator;
        }

        public object GetLateBoundObject(string namespaceUri) => 
            this.argList?.GetExtensionObject(namespaceUri);

        public object GetParameter(string localName, string namespaceUri) => 
            this.argList?.GetParam(localName, namespaceUri);

        public IList<XPathItem> InvokeXsltLateBoundFunction(string name, string namespaceUri, IList<XPathItem>[] args)
        {
            object extensionObject = this.argList?.GetExtensionObject(namespaceUri);
            if (extensionObject == null)
            {
                throw new XslTransformException("Xslt_ScriptInvalidPrefix", new string[] { namespaceUri });
            }
            if (this.extFuncsLate == null)
            {
                this.extFuncsLate = new XmlExtensionFunctionTable();
            }
            XmlExtensionFunction function = this.extFuncsLate.Bind(name, namespaceUri, args.Length, extensionObject.GetType(), BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            object[] objArray = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                Type type2;
                XmlQueryType xmlArgumentType = function.GetXmlArgumentType(i);
                switch (xmlArgumentType.TypeCode)
                {
                    case XmlTypeCode.Item:
                        objArray[i] = args[i];
                        goto Label_011B;

                    case XmlTypeCode.Node:
                        if (!xmlArgumentType.IsSingleton)
                        {
                            break;
                        }
                        objArray[i] = XsltConvert.ToNode(args[i]);
                        goto Label_011B;

                    case XmlTypeCode.String:
                        objArray[i] = XsltConvert.ToString(args[i]);
                        goto Label_011B;

                    case XmlTypeCode.Boolean:
                        objArray[i] = XsltConvert.ToBoolean(args[i]);
                        goto Label_011B;

                    case XmlTypeCode.Double:
                        objArray[i] = XsltConvert.ToDouble(args[i]);
                        goto Label_011B;

                    default:
                        goto Label_011B;
                }
                objArray[i] = XsltConvert.ToNodeSet(args[i]);
            Label_011B:
                type2 = function.GetClrArgumentType(i);
                if ((xmlArgumentType.TypeCode == XmlTypeCode.Item) || !type2.IsAssignableFrom(objArray[i].GetType()))
                {
                    objArray[i] = this.runtime.ChangeTypeXsltArgument(xmlArgumentType, objArray[i], type2);
                }
            }
            object obj3 = function.Invoke(extensionObject, objArray);
            if ((obj3 == null) && (function.ClrReturnType == XsltConvert.VoidType))
            {
                return XmlQueryNodeSequence.Empty;
            }
            return (IList<XPathItem>) this.runtime.ChangeTypeXsltResult(XmlQueryTypeFactory.ItemS, obj3);
        }

        public bool LateBoundFunctionExists(string name, string namespaceUri)
        {
            if (this.argList == null)
            {
                return false;
            }
            object extensionObject = this.argList.GetExtensionObject(namespaceUri);
            if (extensionObject == null)
            {
                return false;
            }
            return new XmlExtensionFunction(name, namespaceUri, -1, extensionObject.GetType(), BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).CanBind();
        }

        public void OnXsltMessageEncountered(string message)
        {
            XsltMessageEncounteredEventHandler handler = (this.argList != null) ? this.argList.xsltMessageEncountered : null;
            if (handler != null)
            {
                handler(this, new XmlILQueryEventArgs(message));
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        public XPathNavigator DefaultDataSource
        {
            get
            {
                if (this.defaultDataSource == null)
                {
                    throw new XslTransformException("XmlIl_NoDefaultDocument", new string[] { string.Empty });
                }
                return this.defaultDataSource;
            }
        }

        public XmlNameTable DefaultNameTable =>
            this.defaultDataSource?.NameTable;

        public XmlNameTable QueryNameTable =>
            this.readerSettings.NameTable;
    }
}

