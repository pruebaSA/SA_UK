namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class InsertHorizontalRule : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            new Dictionary<string, string[]> { 
                { 
                    "size",
                    new string[0]
                },
                { 
                    "width",
                    new string[0]
                }
            };

        public override string CommandName =>
            "InsertHorizontalRule";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { { 
                "hr",
                new string[] { 
                    "size",
                    "width"
                }
            } };

        public override string Tooltip =>
            "Insert Horizontal Rule";
    }
}

