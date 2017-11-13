namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class AssemblyInfo : ConfigurationElement
    {
        private System.Reflection.Assembly[] _assembly;
        private CompilationSection _compilationSection;
        private static readonly ConfigurationProperty _propAssembly = new ConfigurationProperty("assembly", typeof(string), null, null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        static AssemblyInfo()
        {
            _properties.Add(_propAssembly);
        }

        internal AssemblyInfo()
        {
        }

        public AssemblyInfo(string assemblyName)
        {
            this.Assembly = assemblyName;
        }

        internal void SetCompilationReference(CompilationSection compSection)
        {
            this._compilationSection = compSection;
        }

        [ConfigurationProperty("assembly", IsRequired=true, IsKey=true, DefaultValue=""), StringValidator(MinLength=1)]
        public string Assembly
        {
            get => 
                ((string) base[_propAssembly]);
            set
            {
                base[_propAssembly] = value;
            }
        }

        internal System.Reflection.Assembly[] AssemblyInternal
        {
            get
            {
                if (this._assembly == null)
                {
                    this._assembly = this._compilationSection.LoadAssembly(this);
                }
                return this._assembly;
            }
            set
            {
                this._assembly = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

