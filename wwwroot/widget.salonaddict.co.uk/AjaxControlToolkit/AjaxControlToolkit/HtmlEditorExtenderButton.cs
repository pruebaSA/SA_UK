namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public abstract class HtmlEditorExtenderButton
    {
        protected HtmlEditorExtenderButton()
        {
        }

        public abstract Dictionary<string, string[]> AttributeWhiteList { get; }

        public abstract string CommandName { get; }

        public abstract Dictionary<string, string[]> ElementWhiteList { get; }

        public virtual string Tooltip =>
            this.CommandName;
    }
}

