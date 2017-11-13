namespace System.Web.UI
{
    using System;

    internal class FileLevelMasterPageControlBuilder : FileLevelPageControlBuilder
    {
        internal override void AddContentTemplate(object obj, string templateName, ITemplate template)
        {
            ((MasterPage) obj).AddContentTemplate(templateName, template);
        }
    }
}

