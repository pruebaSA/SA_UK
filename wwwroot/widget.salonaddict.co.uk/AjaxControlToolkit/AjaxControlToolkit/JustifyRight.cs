namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class JustifyRight : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            new Dictionary<string, string[]> { 
                { 
                    "style",
                    new string[] { "text-align" }
                },
                { 
                    "align",
                    new string[] { "right" }
                }
            };

        public override string CommandName =>
            "JustifyRight";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { 
                { 
                    "p",
                    new string[] { "align" }
                },
                { 
                    "div",
                    new string[] { 
                        "style",
                        "align"
                    }
                }
            };

        public override string Tooltip =>
            "Justify Right";
    }
}

