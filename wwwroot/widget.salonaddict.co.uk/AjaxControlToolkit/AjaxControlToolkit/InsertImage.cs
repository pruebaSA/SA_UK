namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class InsertImage : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            new Dictionary<string, string[]> { { 
                "src",
                new string[0]
            } };

        public override string CommandName =>
            "InsertImage";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { { 
                "img",
                new string[] { "src" }
            } };

        public override string Tooltip =>
            "Insert Image";
    }
}

