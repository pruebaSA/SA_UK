﻿namespace System.Web.UI
{
    using System;
    using System.Web;

    internal sealed class ApplicationFileParser : TemplateParser
    {
        internal const string defaultDirectiveName = "application";

        internal ApplicationFileParser()
        {
        }

        internal override void CheckObjectTagScope(ref ObjectTagScope scope)
        {
            if (scope == ObjectTagScope.Default)
            {
                scope = ObjectTagScope.AppInstance;
            }
            if (scope == ObjectTagScope.Page)
            {
                throw new HttpException(System.Web.SR.GetString("Page_scope_in_global_asax"));
            }
        }

        internal override Type DefaultBaseType =>
            typeof(HttpApplication);

        internal override string DefaultDirectiveName =>
            "application";

        internal override bool FApplicationFile =>
            true;
    }
}

