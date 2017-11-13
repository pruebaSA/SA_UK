namespace System.Xml.Serialization
{
    using System;
    using System.Security.Policy;

    public class XmlSerializerFactory
    {
        private static TempAssemblyCache cache = new TempAssemblyCache();

        public XmlSerializer CreateSerializer(Type type) => 
            this.CreateSerializer(type, (string) null);

        public XmlSerializer CreateSerializer(XmlTypeMapping xmlTypeMapping) => 
            ((XmlSerializer) XmlSerializer.GenerateTempAssembly(xmlTypeMapping).Contract.TypedSerializers[xmlTypeMapping.Key]);

        public XmlSerializer CreateSerializer(Type type, XmlRootAttribute root) => 
            this.CreateSerializer(type, null, new Type[0], root, null, null, null);

        public XmlSerializer CreateSerializer(Type type, Type[] extraTypes) => 
            this.CreateSerializer(type, null, extraTypes, null, null, null, null);

        public XmlSerializer CreateSerializer(Type type, string defaultNamespace)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            TempAssembly assembly = cache[defaultNamespace, type];
            XmlTypeMapping xmlMapping = null;
            if (assembly == null)
            {
                lock (cache)
                {
                    assembly = cache[defaultNamespace, type];
                    if (assembly == null)
                    {
                        XmlSerializerImplementation implementation;
                        if (TempAssembly.LoadGeneratedAssembly(type, defaultNamespace, out implementation) == null)
                        {
                            xmlMapping = new XmlReflectionImporter(defaultNamespace).ImportTypeMapping(type, null, defaultNamespace);
                            assembly = XmlSerializer.GenerateTempAssembly(xmlMapping, type, defaultNamespace);
                        }
                        else
                        {
                            assembly = new TempAssembly(implementation);
                        }
                        cache.Add(defaultNamespace, type, assembly);
                    }
                }
            }
            if (xmlMapping == null)
            {
                xmlMapping = XmlReflectionImporter.GetTopLevelMapping(type, defaultNamespace);
            }
            return assembly.Contract.GetSerializer(type);
        }

        public XmlSerializer CreateSerializer(Type type, XmlAttributeOverrides overrides) => 
            this.CreateSerializer(type, overrides, new Type[0], null, null, null, null);

        public XmlSerializer CreateSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace) => 
            this.CreateSerializer(type, overrides, extraTypes, root, defaultNamespace, null, null);

        public XmlSerializer CreateSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location, Evidence evidence)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            XmlReflectionImporter importer = new XmlReflectionImporter(overrides, defaultNamespace);
            for (int i = 0; i < extraTypes.Length; i++)
            {
                importer.IncludeType(extraTypes[i]);
            }
            XmlTypeMapping xmlMapping = importer.ImportTypeMapping(type, root, defaultNamespace);
            return (XmlSerializer) XmlSerializer.GenerateTempAssembly(xmlMapping, type, defaultNamespace, location, evidence).Contract.TypedSerializers[xmlMapping.Key];
        }
    }
}

