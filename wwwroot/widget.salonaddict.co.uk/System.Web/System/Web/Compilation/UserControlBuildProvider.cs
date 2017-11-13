namespace System.Web.Compilation
{
    using System.Web.UI;

    [BuildProviderAppliesTo(BuildProviderAppliesTo.Code | BuildProviderAppliesTo.Web)]
    internal class UserControlBuildProvider : TemplateControlBuildProvider
    {
        internal override BaseCodeDomTreeGenerator CreateCodeDomTreeGenerator(TemplateParser parser) => 
            new UserControlCodeDomTreeGenerator((UserControlParser) parser);

        internal override DependencyParser CreateDependencyParser() => 
            new UserControlDependencyParser();

        internal override BuildResultNoCompileTemplateControl CreateNoCompileBuildResult() => 
            new BuildResultNoCompileUserControl(base.Parser.BaseType, base.Parser);

        protected override TemplateParser CreateParser() => 
            new UserControlParser();
    }
}

