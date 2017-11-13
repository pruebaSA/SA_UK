namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class ForeColorSelector : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            new Dictionary<string, string[]> { { 
                "color",
                new string[0]
            } };

        public override string CommandName =>
            "ForeColor";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { { 
                "font",
                new string[] { "color" }
            } };

        public override string Tooltip =>
            "Fore Color";
    }
}

