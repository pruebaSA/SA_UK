namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class Indent : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            new Dictionary<string, string[]> { 
                { 
                    "style",
                    new string[] { 
                        "margin-right",
                        "margin",
                        "padding",
                        "border"
                    }
                },
                { 
                    "dir",
                    new string[] { 
                        "ltr",
                        "rtl",
                        "auto"
                    }
                }
            };

        public override string CommandName =>
            "Indent";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { { 
                "blockquote",
                new string[] { 
                    "style",
                    "dir"
                }
            } };

        public override string Tooltip =>
            "Indent";
    }
}

