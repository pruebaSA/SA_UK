namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class FontSizeSelector : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            new Dictionary<string, string[]> { { 
                "size",
                new string[0]
            } };

        public override string CommandName =>
            "FontSize";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { { 
                "font",
                new string[] { "size" }
            } };

        public override string Tooltip =>
            "Font Size";
    }
}

