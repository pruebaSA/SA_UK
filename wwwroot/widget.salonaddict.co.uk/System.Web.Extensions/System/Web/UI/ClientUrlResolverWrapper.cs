namespace System.Web.UI
{
    using System;

    internal sealed class ClientUrlResolverWrapper : IClientUrlResolver
    {
        private readonly Control _control;

        public ClientUrlResolverWrapper(Control control)
        {
            this._control = control;
        }

        string IClientUrlResolver.ResolveClientUrl(string relativeUrl)
        {
            IClientUrlResolver resolver = this._control as IClientUrlResolver;
            if (resolver != null)
            {
                return resolver.ResolveClientUrl(relativeUrl);
            }
            return this._control.ResolveClientUrl(relativeUrl);
        }

        string IClientUrlResolver.AppRelativeTemplateSourceDirectory =>
            this._control.AppRelativeTemplateSourceDirectory;
    }
}

