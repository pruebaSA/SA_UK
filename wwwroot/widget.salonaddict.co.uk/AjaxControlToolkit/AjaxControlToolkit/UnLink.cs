namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class UnLink : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "UnLink";

        public override Dictionary<string, string[]> ElementWhiteList =>
            null;

        public override string Tooltip =>
            "UnLink";
    }
}

