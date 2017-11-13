namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Web.Resources;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class ResourceDefaultValueAttribute : DefaultValueAttribute
    {
        private bool _resourceLoaded;
        private Type _type;

        internal ResourceDefaultValueAttribute(string value) : base(value)
        {
        }

        internal ResourceDefaultValueAttribute(Type type, string value) : base(value)
        {
            this._type = type;
        }

        public override object TypeId =>
            typeof(DefaultValueAttribute);

        public override object Value
        {
            get
            {
                if (!this._resourceLoaded)
                {
                    this._resourceLoaded = true;
                    string str = (string) base.Value;
                    if (!string.IsNullOrEmpty(str))
                    {
                        object obj2 = AtlasWeb.ResourceManager.GetString(str, AtlasWeb.Culture);
                        if (this._type != null)
                        {
                            try
                            {
                                obj2 = TypeDescriptor.GetConverter(this._type).ConvertFromInvariantString((string) obj2);
                            }
                            catch (NotSupportedException)
                            {
                                obj2 = null;
                            }
                        }
                        base.SetValue(obj2);
                    }
                }
                return base.Value;
            }
        }
    }
}

