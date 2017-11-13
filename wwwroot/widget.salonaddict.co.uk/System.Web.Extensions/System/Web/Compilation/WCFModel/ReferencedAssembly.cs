namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Xml.Serialization;

    internal class ReferencedAssembly
    {
        private string m_AssemblyName;

        public ReferencedAssembly()
        {
            this.m_AssemblyName = string.Empty;
        }

        public ReferencedAssembly(string assemblyName)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName");
            }
            this.m_AssemblyName = assemblyName;
        }

        [XmlAttribute]
        public string AssemblyName
        {
            get => 
                this.m_AssemblyName;
            set
            {
                this.m_AssemblyName = value;
            }
        }
    }
}

