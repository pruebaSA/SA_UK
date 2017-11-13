namespace System.Web.Compilation
{
    using System.Web.UI;

    [BuildProviderAppliesTo(BuildProviderAppliesTo.Code | BuildProviderAppliesTo.Web)]
    internal class MasterPageBuildProvider : UserControlBuildProvider
    {
        internal override BaseCodeDomTreeGenerator CreateCodeDomTreeGenerator(TemplateParser parser) => 
            new MasterPageCodeDomTreeGenerator((MasterPageParser) parser);

        internal override DependencyParser CreateDependencyParser() => 
            new MasterPageDependencyParser();

        internal override BuildResultNoCompileTemplateControl CreateNoCompileBuildResult() => 
            new BuildResultNoCompileMasterPage(base.Parser.BaseType, base.Parser);

        protected override TemplateParser CreateParser() => 
            new MasterPageParser();
    }
}

