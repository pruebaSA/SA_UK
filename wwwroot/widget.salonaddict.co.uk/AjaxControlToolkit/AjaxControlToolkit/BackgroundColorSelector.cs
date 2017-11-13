namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class BackgroundColorSelector : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            new Dictionary<string, string[]> { { 
                "style",
                new string[] { "background-color" }
            } };

        public override string CommandName =>
            "BackColor";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { 
                { 
                    "font",
                    new string[] { "style" }
                },
                { 
                    "span",
                    new string[] { "style" }
                }
            };

        public override string Tooltip =>
            "Back Color";
    }
}

