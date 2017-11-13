namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class Subscript : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "Subscript";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { { 
                "sub",
                new string[0]
            } };

        public override string Tooltip =>
            "Sub Script";
    }
}

