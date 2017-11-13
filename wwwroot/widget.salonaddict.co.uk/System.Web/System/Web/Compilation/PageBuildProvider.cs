namespace System.Web.Compilation
{
    using System.Web.UI;

    [BuildProviderAppliesTo(BuildProviderAppliesTo.Web)]
    internal class PageBuildProvider : TemplateControlBuildProvider
    {
        internal override BaseCodeDomTreeGenerator CreateCodeDomTreeGenerator(TemplateParser parser) => 
            new PageCodeDomTreeGenerator((PageParser) parser);

        internal override DependencyParser CreateDependencyParser() => 
            new PageDependencyParser();

        internal override BuildResultNoCompileTemplateControl CreateNoCompileBuildResult() => 
            new BuildResultNoCompilePage(base.Parser.BaseType, base.Parser);

        protected override TemplateParser CreateParser() => 
            new PageParser();
    }
}

