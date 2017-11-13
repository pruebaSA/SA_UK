namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class StrikeThrough : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            new Dictionary<string, string[]> { { 
                "style",
                new string[0]
            } };

        public override string CommandName =>
            "StrikeThrough";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { { 
                "strike",
                new string[] { "style" }
            } };

        public override string Tooltip =>
            "Strike Through";
    }
}

