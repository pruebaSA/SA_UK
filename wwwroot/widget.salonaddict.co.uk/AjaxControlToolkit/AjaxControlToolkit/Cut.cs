namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class Cut : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "Cut";

        public override Dictionary<string, string[]> ElementWhiteList =>
            null;

        public override string Tooltip =>
            "Cut";
    }
}

