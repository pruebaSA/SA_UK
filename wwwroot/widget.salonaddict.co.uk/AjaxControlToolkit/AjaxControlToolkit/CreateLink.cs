namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;

    public class CreateLink : HtmlEditorExtenderButton
    {
        public override Dictionary<string, string[]> AttributeWhiteList
        {
            get
            {
                Dictionary<string, string[]> dictionary = new Dictionary<string, string[]> {
                    { 
                        "href",
                        new string[0]
                    }
                };
                return null;
            }
        }

        public override string CommandName =>
            "createLink";

        public override Dictionary<string, string[]> ElementWhiteList =>
            new Dictionary<string, string[]> { { 
                "a",
                new string[] { "href" }
            } };

        public override string Tooltip =>
            "Create Link";
    }
}

