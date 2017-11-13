namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class RemoveFormat : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "RemoveFormat";

        public override Dictionary<string, string[]> ElementWhiteList =>
            null;

        public override string Tooltip =>
            "Remove Format";
    }
}

