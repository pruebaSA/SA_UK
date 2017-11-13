namespace System.Web.Compilation
{
    using System.Web.UI;

    internal class WebServiceBuildProvider : SimpleHandlerBuildProvider
    {
        protected override SimpleWebHandlerParser CreateParser() => 
            new WebServiceParser(base.VirtualPath);
    }
}

