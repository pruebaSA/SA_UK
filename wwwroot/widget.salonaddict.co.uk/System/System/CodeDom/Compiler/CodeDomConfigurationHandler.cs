namespace System.CodeDom.Compiler
{
    using System;
    using System.Configuration;
    using System.Xml;

    internal class CodeDomConfigurationHandler : IConfigurationSectionHandler
    {
        internal CodeDomConfigurationHandler()
        {
        }

        public virtual object Create(object inheritedObject, object configContextObj, XmlNode node) => 
            CodeDomCompilationConfiguration.SectionHandler.CreateStatic(inheritedObject, node);
    }
}

