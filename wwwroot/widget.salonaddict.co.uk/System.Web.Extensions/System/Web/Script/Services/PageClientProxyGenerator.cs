namespace System.Web.Script.Services
{
    using System;
    using System.Web;
    using System.Web.UI;

    internal class PageClientProxyGenerator : ClientProxyGenerator
    {
        private string _path;

        internal PageClientProxyGenerator(string path, bool debug)
        {
            this._path = path;
            base._debugMode = debug;
        }

        internal PageClientProxyGenerator(IPage page, bool debug) : this(page.Request.FilePath, debug)
        {
        }

        protected override void GenerateTypeDeclaration(WebServiceData webServiceData, bool genClass)
        {
            if (genClass)
            {
                base._builder.Append("PageMethods.prototype = ");
            }
            else
            {
                base._builder.Append("var PageMethods = ");
            }
        }

        internal static string GetClientProxyScript(HttpContext context, IPage page, bool debug)
        {
            if ((context == null) || (page == null))
            {
                return null;
            }
            WebServiceData webServiceData = WebServiceData.GetWebServiceData(context, page.AppRelativeVirtualPath, false, true);
            if (webServiceData == null)
            {
                return null;
            }
            PageClientProxyGenerator generator = new PageClientProxyGenerator(page, debug);
            return generator.GetClientProxyScript(webServiceData);
        }

        protected override string GetProxyPath() => 
            this._path;

        protected override string GetProxyTypeName(WebServiceData data) => 
            "PageMethods";
    }
}

