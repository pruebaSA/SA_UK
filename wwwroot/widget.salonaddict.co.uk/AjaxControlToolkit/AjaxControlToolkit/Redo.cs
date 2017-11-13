namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class Redo : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "Redo";

        public override Dictionary<string, string[]> ElementWhiteList =>
            null;
    }
}

