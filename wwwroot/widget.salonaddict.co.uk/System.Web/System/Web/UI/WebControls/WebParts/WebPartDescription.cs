namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebPartDescription
    {
        private string _description;
        private string _id;
        private string _imageUrl;
        private System.Web.UI.WebControls.WebParts.WebPart _part;
        private string _title;

        private WebPartDescription()
        {
        }

        public WebPartDescription(System.Web.UI.WebControls.WebParts.WebPart part)
        {
            string iD = part.ID;
            if (string.IsNullOrEmpty(iD))
            {
                throw new ArgumentException(System.Web.SR.GetString("WebPartManager_NoWebPartID"), "part");
            }
            this._id = iD;
            string displayTitle = part.DisplayTitle;
            this._title = (displayTitle != null) ? displayTitle : string.Empty;
            string description = part.Description;
            this._description = (description != null) ? description : string.Empty;
            string catalogIconImageUrl = part.CatalogIconImageUrl;
            this._imageUrl = (catalogIconImageUrl != null) ? catalogIconImageUrl : string.Empty;
            this._part = part;
        }

        public WebPartDescription(string id, string title, string description, string imageUrl)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException("title");
            }
            this._id = id;
            this._title = title;
            this._description = (description != null) ? description : string.Empty;
            this._imageUrl = (imageUrl != null) ? imageUrl : string.Empty;
        }

        public string CatalogIconImageUrl =>
            this._imageUrl;

        public string Description =>
            this._description;

        public string ID =>
            this._id;

        public string Title =>
            this._title;

        internal System.Web.UI.WebControls.WebParts.WebPart WebPart =>
            this._part;
    }
}

