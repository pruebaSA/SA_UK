namespace System.Web.UI
{
    using System;
    using System.Web;

    internal interface IControl : IClientUrlResolver
    {
        HttpContextBase Context { get; }

        bool DesignMode { get; }
    }
}

