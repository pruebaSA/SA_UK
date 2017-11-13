namespace System.Web.Compilation
{
    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Data.Design;
    using System.IO;
    using System.Reflection;
    using System.Web.UI;
    using System.Xml;

    [BuildProviderAppliesTo(BuildProviderAppliesTo.Code)]
    internal class XsdBuildProvider : BuildProvider
    {
        public override void GenerateCode(AssemblyBuilder assemblyBuilder)
        {
            string namespaceFromVirtualPath = Util.GetNamespaceFromVirtualPath(base.VirtualPathObject);
            XmlDocument document = new XmlDocument();
            using (Stream stream = base.OpenStream())
            {
                document.Load(stream);
            }
            string outerXml = document.OuterXml;
            CodeCompileUnit compileUnit = new CodeCompileUnit();
            CodeNamespace namespace2 = new CodeNamespace(namespaceFromVirtualPath);
            compileUnit.Namespaces.Add(namespace2);
            if (CompilationUtil.IsCompilerVersion35(assemblyBuilder.CodeDomProvider.GetType()))
            {
                TypedDataSetGenerator.GenerateOption none = TypedDataSetGenerator.GenerateOption.None;
                none |= TypedDataSetGenerator.GenerateOption.HierarchicalUpdate;
                none |= TypedDataSetGenerator.GenerateOption.LinqOverTypedDatasets;
                Hashtable customDBProviders = null;
                TypedDataSetGenerator.Generate(outerXml, compileUnit, namespace2, assemblyBuilder.CodeDomProvider, customDBProviders, none);
            }
            else
            {
                TypedDataSetGenerator.Generate(outerXml, compileUnit, namespace2, assemblyBuilder.CodeDomProvider);
            }
            if (TypedDataSetGenerator.ReferencedAssemblies != null)
            {
                foreach (Assembly assembly in TypedDataSetGenerator.ReferencedAssemblies)
                {
                    assemblyBuilder.AddAssemblyReference(assembly);
                }
            }
            assemblyBuilder.AddCodeCompileUnit(this, compileUnit);
        }
    }
}

