namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Web.UI.Design;

    public class GravatarDesigner : ControlDesigner
    {
        private Gravatar _gravatar;

        public override string GetDesignTimeHtml()
        {
            string str = (this._gravatar.Rating == GravatarRating.Default) ? "G" : this._gravatar.Rating.ToString();
            string webResourceUrl = base.ViewControl.Page.ClientScript.GetWebResourceUrl(base.GetType(), "Gravatar.Images.gravatar-" + str + ".jpg");
            return $"<div style='width:80px; height:80px;'><img src='{webResourceUrl}'/></div>";
        }

        public override void Initialize(IComponent component)
        {
            this._gravatar = component as Gravatar;
            if (this._gravatar == null)
            {
                throw new ArgumentException("Component must be a gravatar control", "component");
            }
            base.Initialize(component);
        }

        public override bool AllowResize =>
            false;

        protected override bool Visible =>
            true;
    }
}

