namespace System.Web.UI.Design
{
    using System;

    public class ContentDefinition
    {
        private string _contentPlaceHolderID;
        private string _defaultContent;
        private string _defaultDesignTimeHTML;

        public ContentDefinition(string id, string content, string designTimeHtml)
        {
            this._contentPlaceHolderID = id;
            this._defaultContent = content;
            this._defaultDesignTimeHTML = designTimeHtml;
        }

        public string ContentPlaceHolderID =>
            this._contentPlaceHolderID;

        public string DefaultContent =>
            this._defaultContent;

        public string DefaultDesignTimeHtml =>
            this._defaultDesignTimeHTML;
    }
}

