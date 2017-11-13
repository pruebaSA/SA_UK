namespace System.Web.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.Resources;
    using System.Web.Script.Serialization;

    [ConfigurationCollection(typeof(Converter)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ConvertersCollection : ConfigurationElementCollection
    {
        private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public void Add(Converter converter)
        {
            this.BaseAdd(converter);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        internal JavaScriptConverter[] CreateConverters()
        {
            List<JavaScriptConverter> list = new List<JavaScriptConverter>();
            foreach (Converter converter in this)
            {
                Type c = BuildManager.GetType(converter.Type, false);
                if (c == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ConvertersCollection_UnknownType, new object[] { converter.Type }));
                }
                if (!typeof(JavaScriptConverter).IsAssignableFrom(c))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ConvertersCollection_NotJavaScriptConverter, new object[] { c.Name }));
                }
                list.Add((JavaScriptConverter) Activator.CreateInstance(c));
            }
            return list.ToArray();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new Converter();

        protected override object GetElementKey(ConfigurationElement element) => 
            ((Converter) element).Name;

        public void Remove(Converter converter)
        {
            base.BaseRemove(this.GetElementKey(converter));
        }

        public Converter this[int index]
        {
            get => 
                ((Converter) base.BaseGet(index));
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

