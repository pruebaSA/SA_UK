namespace System.Web
{
    using System;
    using System.Web.Configuration;

    internal sealed class UrlMappingsModule : IHttpModule
    {
        internal UrlMappingsModule()
        {
        }

        public void Dispose()
        {
        }

        public void Init(HttpApplication application)
        {
            UrlMappingsSection urlMappings = RuntimeConfig.GetConfig().UrlMappings;
            if (urlMappings.IsEnabled && (urlMappings.UrlMappings.Count > 0))
            {
                application.BeginRequest += new EventHandler(this.OnEnter);
            }
        }

        internal void OnEnter(object source, EventArgs eventArgs)
        {
            HttpApplication application = (HttpApplication) source;
            UrlMappingsSection urlMappings = RuntimeConfig.GetAppConfig().UrlMappings;
            string str = urlMappings.HttpResolveMapping(application.Request.RawUrl);
            if (str == null)
            {
                str = urlMappings.HttpResolveMapping(application.Request.Path);
            }
            if (!string.IsNullOrEmpty(str))
            {
                application.Context.RewritePath(str, false);
            }
        }
    }
}

