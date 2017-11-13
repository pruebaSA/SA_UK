namespace System.Web.Script.Services
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Web;

    internal class WebServiceClientProxyGenerator : ClientProxyGenerator
    {
        private string _path;

        internal WebServiceClientProxyGenerator(string path, bool debug)
        {
            this._path = path;
            base._debugMode = debug;
        }

        private static DateTime GetAssemblyModifiedTime(Assembly assembly)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(new Uri(assembly.GetName().CodeBase).LocalPath);
            return new DateTime(lastWriteTime.Year, lastWriteTime.Month, lastWriteTime.Day, lastWriteTime.Hour, lastWriteTime.Minute, lastWriteTime.Second);
        }

        internal static string GetClientProxyScript(HttpContext context)
        {
            DateTime time2;
            WebServiceData webServiceData = WebServiceData.GetWebServiceData(context, context.Request.FilePath);
            DateTime assemblyModifiedTime = GetAssemblyModifiedTime(webServiceData.TypeData.Type.Assembly);
            string s = context.Request.Headers["If-Modified-Since"];
            if (((s != null) && DateTime.TryParse(s, out time2)) && (time2 >= assemblyModifiedTime))
            {
                context.Response.StatusCode = 0x130;
                return null;
            }
            bool debug = RestHandlerFactory.IsClientProxyDebugRequest(context.Request.PathInfo);
            if (!debug && (assemblyModifiedTime.ToUniversalTime() < DateTime.UtcNow))
            {
                HttpCachePolicy cache = context.Response.Cache;
                cache.SetCacheability(HttpCacheability.Public);
                cache.SetLastModified(assemblyModifiedTime);
                cache.SetExpires(assemblyModifiedTime.AddYears(-1));
            }
            WebServiceClientProxyGenerator generator = new WebServiceClientProxyGenerator(context.Request.FilePath, debug);
            return generator.GetClientProxyScript(webServiceData);
        }

        internal static string GetInlineClientProxyScript(string path, HttpContext context, bool debug)
        {
            WebServiceData webServiceData = WebServiceData.GetWebServiceData(context, path, true, false, true);
            WebServiceClientProxyGenerator generator = new WebServiceClientProxyGenerator(path, debug);
            return generator.GetClientProxyScript(webServiceData);
        }

        protected override string GetProxyPath() => 
            this._path;
    }
}

