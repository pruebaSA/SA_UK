namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class Bold : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            new Dictionary<string, string[]> { { 
                "style",
                new string[0]
            } };

        public override string CommandName =>
            "Bold";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { 
                { 
                    "b",
                    new string[] { "style" }
                },
                { 
                    "strong",
                    new string[] { "style" }
                }
            };
    }
}

