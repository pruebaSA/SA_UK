namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class SelectAll : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "SelectAll";

        public override Dictionary<string, string[]> ElementWhiteList =>
            null;

        public override string Tooltip =>
            "Select All";
    }
}

