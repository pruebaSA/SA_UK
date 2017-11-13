namespace System.Web.Compilation
{
    using System;
    using System.Web.Configuration;
    using System.Web.UI;
    using System.Web.Util;

    internal class BuildResultCompiledTemplateType : BuildResultCompiledType
    {
        public BuildResultCompiledTemplateType()
        {
        }

        public BuildResultCompiledTemplateType(Type t) : base(t)
        {
        }

        protected override void ComputeHashCode(HashCodeCombiner hashCodeCombiner)
        {
            base.ComputeHashCode(hashCodeCombiner);
            PagesSection pages = RuntimeConfig.GetConfig(base.VirtualPath).Pages;
            hashCodeCombiner.AddObject(Util.GetRecompilationHash(pages));
        }

        internal override BuildResultTypeCode GetCode() => 
            BuildResultTypeCode.BuildResultCompiledTemplateType;
    }
}

