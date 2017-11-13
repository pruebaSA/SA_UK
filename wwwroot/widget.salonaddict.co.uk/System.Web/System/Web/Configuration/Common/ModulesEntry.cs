namespace System.Web.Configuration.Common
{
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.Configuration;

    internal class ModulesEntry
    {
        private string _name;
        private Type _type;

        internal ModulesEntry(string name, string typeName, string propertyName, ConfigurationElement configElement)
        {
            this._name = (name != null) ? name : string.Empty;
            this._type = ConfigUtil.GetType(typeName, propertyName, configElement, false);
            if (!typeof(IHttpModule).IsAssignableFrom(this._type))
            {
                if (configElement == null)
                {
                    throw new ConfigurationErrorsException(System.Web.SR.GetString("Type_not_module", new object[] { typeName }));
                }
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Type_not_module", new object[] { typeName }), configElement.ElementInformation.Properties["type"].Source, configElement.ElementInformation.Properties["type"].LineNumber);
            }
        }

        internal IHttpModule Create() => 
            ((IHttpModule) HttpRuntime.CreateNonPublicInstance(this._type));

        internal static bool IsTypeMatch(Type type, string typeName)
        {
            if (!type.Name.Equals(typeName))
            {
                return type.FullName.Equals(typeName);
            }
            return true;
        }

        internal string ModuleName =>
            this._name;
    }
}

