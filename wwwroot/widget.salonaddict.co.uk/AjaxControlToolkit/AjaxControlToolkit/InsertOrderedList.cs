namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class InsertOrderedList : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "insertOrderedList";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { 
                { 
                    "ol",
                    new string[0]
                },
                { 
                    "li",
                    new string[0]
                }
            };

        public override string Tooltip =>
            "Insert Ordered List";
    }
}

