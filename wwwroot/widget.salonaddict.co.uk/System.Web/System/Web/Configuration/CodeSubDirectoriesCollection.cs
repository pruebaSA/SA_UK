namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [ConfigurationCollection(typeof(CodeSubDirectory), CollectionType=ConfigurationElementCollectionType.BasicMap), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class CodeSubDirectoriesCollection : ConfigurationElementCollection
    {
        private bool _didRuntimeValidation;
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public CodeSubDirectoriesCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public void Add(CodeSubDirectory codeSubDirectory)
        {
            this.BaseAdd(codeSubDirectory);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new CodeSubDirectory();

        internal void EnsureRuntimeValidation()
        {
            if (!this._didRuntimeValidation)
            {
                foreach (CodeSubDirectory directory in this)
                {
                    directory.DoRuntimeValidation();
                }
                this._didRuntimeValidation = true;
            }
        }

        protected override object GetElementKey(ConfigurationElement element) => 
            ((CodeSubDirectory) element).DirectoryName;

        public void Remove(string directoryName)
        {
            base.BaseRemove(directoryName);
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public override ConfigurationElementCollectionType CollectionType =>
            ConfigurationElementCollectionType.BasicMap;

        protected override string ElementName =>
            "add";

        public CodeSubDirectory this[int index]
        {
            get => 
                ((CodeSubDirectory) base.BaseGet(index));
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

