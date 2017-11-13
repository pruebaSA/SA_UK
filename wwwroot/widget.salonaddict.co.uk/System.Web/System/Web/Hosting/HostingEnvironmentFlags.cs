namespace System.Web.Hosting
{
    using System;

    [Flags]
    internal enum HostingEnvironmentFlags
    {
        ClientBuildManager = 8,
        Default = 0,
        DontCallAppInitialize = 4,
        HideFromAppManager = 1,
        ThrowHostingInitErrors = 2
    }
}

