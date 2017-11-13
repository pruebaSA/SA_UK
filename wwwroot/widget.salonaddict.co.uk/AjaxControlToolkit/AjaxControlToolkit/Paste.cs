namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class Paste : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "Paste";

        public override Dictionary<string, string[]> ElementWhiteList =>
            null;

        public override string Tooltip =>
            "Paste";
    }
}

