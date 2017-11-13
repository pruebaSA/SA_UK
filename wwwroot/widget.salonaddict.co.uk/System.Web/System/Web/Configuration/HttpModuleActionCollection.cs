namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [ConfigurationCollection(typeof(HttpModuleAction)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HttpModuleActionCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public HttpModuleActionCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public void Add(HttpModuleAction httpModule)
        {
            this.BaseAdd(httpModule);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new HttpModuleAction();

        protected override object GetElementKey(ConfigurationElement element) => 
            ((HttpModuleAction) element).Key;

        public int IndexOf(HttpModuleAction action) => 
            base.BaseIndexOf(action);

        protected override bool IsElementRemovable(ConfigurationElement element)
        {
            HttpModuleAction action = (HttpModuleAction) element;
            if (base.BaseIndexOf(action) != -1)
            {
                return true;
            }
            if (HttpModuleAction.IsSpecialModuleName(action.Name))
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Special_module_cannot_be_removed_manually", new object[] { action.Name }), action.FileName, action.LineNumber);
            }
            throw new ConfigurationErrorsException(System.Web.SR.GetString("Module_not_in_app", new object[] { action.Name }), action.FileName, action.LineNumber);
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void Remove(HttpModuleAction action)
        {
            base.BaseRemove(action.Key);
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public HttpModuleAction this[int index]
        {
            get => 
                ((HttpModuleAction) base.BaseGet(index));
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

