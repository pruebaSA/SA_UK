﻿namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class Copy : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList =>
            null;

        public override string CommandName =>
            "Copy";

        public override Dictionary<string, string[]> ElementWhiteList =>
            null;

        public override string Tooltip =>
            "Copy";
    }
}

