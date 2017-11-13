namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class XmlExtensionFunctionTable
    {
        private XmlExtensionFunction funcCached;
        private Dictionary<XmlExtensionFunction, XmlExtensionFunction> table = new Dictionary<XmlExtensionFunction, XmlExtensionFunction>();

        public XmlExtensionFunction Bind(string name, string namespaceUri, int numArgs, Type objectType, BindingFlags flags)
        {
            XmlExtensionFunction funcCached;
            if (this.funcCached == null)
            {
                this.funcCached = new XmlExtensionFunction();
            }
            this.funcCached.Init(name, namespaceUri, numArgs, objectType, flags);
            if (!this.table.TryGetValue(this.funcCached, out funcCached))
            {
                funcCached = this.funcCached;
                this.funcCached = null;
                funcCached.Bind();
                this.table.Add(funcCached, funcCached);
            }
            return funcCached;
        }
    }
}

