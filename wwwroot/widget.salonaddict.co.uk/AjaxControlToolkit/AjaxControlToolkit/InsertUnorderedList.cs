namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class InsertUnorderedList : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "insertUnorderedList";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { 
                { 
                    "ul",
                    new string[0]
                },
                { 
                    "li",
                    new string[0]
                }
            };

        public override string Tooltip =>
            "Insert Unordered List";
    }
}

