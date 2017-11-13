namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class Underline : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            new Dictionary<string, string[]> { { 
                "style",
                new string[0]
            } };

        public override string CommandName =>
            "Underline";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { { 
                "u",
                new string[] { "style" }
            } };
    }
}

