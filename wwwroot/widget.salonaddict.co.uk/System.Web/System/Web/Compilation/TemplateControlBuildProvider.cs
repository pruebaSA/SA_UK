namespace System.Web.Compilation
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Web.UI;

    internal abstract class TemplateControlBuildProvider : BaseTemplateBuildProvider
    {
        protected TemplateControlBuildProvider()
        {
        }

        internal override BuildResult CreateBuildResult(CompilerResults results)
        {
            if (base.Parser.RequiresCompilation)
            {
                return base.CreateBuildResult(results);
            }
            return this.CreateNoCompileBuildResult();
        }

        internal virtual DependencyParser CreateDependencyParser() => 
            null;

        internal abstract BuildResultNoCompileTemplateControl CreateNoCompileBuildResult();
        internal override ICollection GetBuildResultVirtualPathDependencies()
        {
            DependencyParser parser = this.CreateDependencyParser();
            if (parser == null)
            {
                return null;
            }
            parser.Init(base.VirtualPathObject);
            return parser.GetVirtualPathDependencies();
        }
    }
}

