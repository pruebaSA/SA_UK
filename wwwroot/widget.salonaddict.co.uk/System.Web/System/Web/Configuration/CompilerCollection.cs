namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [ConfigurationCollection(typeof(Compiler), AddItemName="compiler", CollectionType=ConfigurationElementCollectionType.BasicMap), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class CompilerCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public CompilerCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        protected override ConfigurationElement CreateNewElement() => 
            new Compiler();

        public Compiler Get(int index) => 
            ((Compiler) base.BaseGet(index));

        public Compiler Get(string language) => 
            ((Compiler) base.BaseGet(language));

        protected override object GetElementKey(ConfigurationElement element) => 
            ((Compiler) element).Language;

        public string GetKey(int index) => 
            ((string) base.BaseGetKey(index));

        public string[] AllKeys =>
            System.Web.Util.StringUtil.ObjectArrayToStringArray(base.BaseGetAllKeys());

        public override ConfigurationElementCollectionType CollectionType =>
            ConfigurationElementCollectionType.BasicMap;

        protected override string ElementName =>
            "compiler";

        public Compiler this[string language] =>
            ((Compiler) base.BaseGet(language));

        public Compiler this[int index] =>
            ((Compiler) base.BaseGet(index));

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

