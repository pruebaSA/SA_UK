namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Web.Caching;

    internal class UserControlParser : TemplateControlParser
    {
        private bool _fSharedPartialCaching;
        internal const string defaultDirectiveName = "control";

        internal virtual void ApplyBaseType()
        {
            if ((base.PagesConfig != null) && (base.PagesConfig.UserControlBaseTypeInternal != null))
            {
                base.BaseType = base.PagesConfig.UserControlBaseTypeInternal;
            }
        }

        internal override RootBuilder CreateDefaultFileLevelBuilder() => 
            new FileLevelUserControlBuilder();

        internal override void ProcessConfigSettings()
        {
            base.ProcessConfigSettings();
            this.ApplyBaseType();
        }

        internal override void ProcessOutputCacheDirective(string directiveName, IDictionary directive)
        {
            Util.GetAndRemoveBooleanAttribute(directive, "shared", ref this._fSharedPartialCaching);
            string andRemoveNonEmptyAttribute = Util.GetAndRemoveNonEmptyAttribute(directive, "sqldependency");
            if (andRemoveNonEmptyAttribute != null)
            {
                SqlCacheDependency.ValidateOutputCacheDependencyString(andRemoveNonEmptyAttribute, false);
                base.OutputCacheParameters.SqlDependency = andRemoveNonEmptyAttribute;
            }
            base.ProcessOutputCacheDirective(directiveName, directive);
        }

        internal override Type DefaultBaseType =>
            typeof(UserControl);

        internal override string DefaultDirectiveName =>
            "control";

        internal override Type DefaultFileLevelBuilderType =>
            typeof(FileLevelUserControlBuilder);

        internal bool FSharedPartialCaching =>
            this._fSharedPartialCaching;

        internal override bool FVaryByParamsRequiredOnOutputCache =>
            (base.OutputCacheParameters.VaryByControl == null);

        internal override string UnknownOutputCacheAttributeError =>
            "Attr_not_supported_in_ucdirective";
    }
}

