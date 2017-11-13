namespace System.Web.UI
{
    using System;

    internal interface IClientUrlResolver
    {
        string ResolveClientUrl(string relativeUrl);

        string AppRelativeTemplateSourceDirectory { get; }
    }
}

