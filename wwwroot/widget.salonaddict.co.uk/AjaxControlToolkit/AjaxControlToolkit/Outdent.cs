namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class Outdent : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "Outdent";

        public override Dictionary<string, string[]> ElementWhiteList =>
            null;

        public override string Tooltip =>
            "Outdent";
    }
}

