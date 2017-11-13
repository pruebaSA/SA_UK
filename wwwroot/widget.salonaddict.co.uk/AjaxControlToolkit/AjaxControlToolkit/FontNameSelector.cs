namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class FontNameSelector : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            new Dictionary<string, string[]> { { 
                "face",
                new string[0]
            } };

        public override string CommandName =>
            "FontName";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { { 
                "font",
                new string[] { "face" }
            } };

        public override string Tooltip =>
            "Font Name";
    }
}

