namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class Italic : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            new Dictionary<string, string[]> { { 
                "style",
                new string[0]
            } };

        public override string CommandName =>
            "Italic";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { 
                { 
                    "i",
                    new string[] { "style" }
                },
                { 
                    "em",
                    new string[] { "style" }
                }
            };
    }
}

