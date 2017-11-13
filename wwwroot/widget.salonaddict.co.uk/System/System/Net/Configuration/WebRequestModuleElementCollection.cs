namespace System.Net.Configuration
{
    using System;
    using System.Configuration;
    using System.Reflection;

    [ConfigurationCollection(typeof(WebRequestModuleElement))]
    public sealed class WebRequestModuleElementCollection : ConfigurationElementCollection
    {
        public void Add(WebRequestModuleElement element)
        {
            this.BaseAdd(element);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new WebRequestModuleElement();

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return ((WebRequestModuleElement) element).Key;
        }

        public int IndexOf(WebRequestModuleElement element) => 
            base.BaseIndexOf(element);

        public void Remove(WebRequestModuleElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            base.BaseRemove(element.Key);
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public WebRequestModuleElement this[int index]
        {
            get => 
                ((WebRequestModuleElement) base.BaseGet(index));
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public WebRequestModuleElement this[string name]
        {
            get => 
                ((WebRequestModuleElement) base.BaseGet(name));
            set
            {
                if (base.BaseGet(name) != null)
                {
                    base.BaseRemove(name);
                }
                this.BaseAdd(value);
            }
        }
    }
}

