namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Compilation;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebServiceParser : SimpleWebHandlerParser
    {
        internal WebServiceParser(string virtualPath) : base(null, virtualPath, null)
        {
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public static Type GetCompiledType(string inputFile, HttpContext context)
        {
            BuildResultCompiledType vPathBuildResult = (BuildResultCompiledType) BuildManager.GetVPathBuildResult(context, VirtualPath.Create(inputFile));
            return vPathBuildResult.ResultType;
        }

        protected override string DefaultDirectiveName =>
            "webservice";
    }
}

