namespace System.Web.Script.Services
{
    using System;
    using System.Globalization;
    using System.Security.Permissions;
    using System.ServiceModel;
    using System.Web;
    using System.Web.Resources;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public static class ProxyGenerator
    {
        public static string GetClientProxyScript(Type type, string path, bool debug)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            WebServiceData webServiceData = null;
            ClientProxyGenerator generator = null;
            if (IsWebServiceType(type))
            {
                generator = new WebServiceClientProxyGenerator(path, debug);
                webServiceData = new WebServiceData(type, false);
            }
            else if (IsPageType(type))
            {
                generator = new PageClientProxyGenerator(path, debug);
                webServiceData = new WebServiceData(type, true);
            }
            else
            {
                if (!IsWCFServiceType(type))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ProxyGenerator_UnsupportedType, new object[] { type.FullName }));
                }
                return WCFServiceClientProxyGenerator.GetClientProxyScript(type, path, debug);
            }
            return generator.GetClientProxyScript(webServiceData);
        }

        private static bool IsPageType(Type type) => 
            typeof(Page).IsAssignableFrom(type);

        private static bool IsWCFServiceType(Type type) => 
            (type.GetCustomAttributes(typeof(ServiceContractAttribute), true).Length != 0);

        private static bool IsWebServiceType(Type type) => 
            (type.GetCustomAttributes(typeof(ScriptServiceAttribute), true).Length != 0);
    }
}

