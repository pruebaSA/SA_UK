namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class Superscript : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "Superscript";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { { 
                "sup",
                new string[0]
            } };

        public override string Tooltip =>
            "Super Script";
    }
}

