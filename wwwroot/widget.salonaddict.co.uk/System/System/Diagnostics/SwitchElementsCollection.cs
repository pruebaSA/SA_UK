namespace System.Diagnostics
{
    using System;
    using System.Configuration;
    using System.Reflection;

    [ConfigurationCollection(typeof(SwitchElement))]
    internal class SwitchElementsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => 
            new SwitchElement();

        protected override object GetElementKey(ConfigurationElement element) => 
            ((SwitchElement) element).Name;

        public override ConfigurationElementCollectionType CollectionType =>
            ConfigurationElementCollectionType.AddRemoveClearMap;

        public SwitchElement this[string name] =>
            ((SwitchElement) base.BaseGet(name));
    }
}

