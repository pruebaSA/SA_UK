namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class Delete : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "Delete";

        public override Dictionary<string, string[]> ElementWhiteList =>
            null;

        public override string Tooltip =>
            "Delete";
    }
}

