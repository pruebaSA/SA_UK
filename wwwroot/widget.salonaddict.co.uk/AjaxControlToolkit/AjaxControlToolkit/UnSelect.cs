namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class UnSelect : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "UnSelect";

        public override Dictionary<string, string[]> ElementWhiteList =>
            null;

        public override string Tooltip =>
            "UnSelect";
    }
}

