namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class Undo : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "Undo";

        public override Dictionary<string, string[]> ElementWhiteList =>
            null;
    }
}

