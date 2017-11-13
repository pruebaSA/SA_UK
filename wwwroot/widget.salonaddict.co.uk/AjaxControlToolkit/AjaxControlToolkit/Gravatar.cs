namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [Designer(typeof(GravatarDesigner)), ToolboxItem("System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ToolboxBitmap(typeof(Gravatar), "Gravatar.Gravatar.ico"), RequiredScript(typeof(ScriptControlBase), 1), ToolboxData("<{0}:Gravatar runat=\"server\"></{0}:Gravatar>"), RequiredScript(typeof(CommonToolkitScripts), 2)]
    public class Gravatar : WebControl
    {
        public Gravatar() : base(HtmlTextWriterTag.Img)
        {
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Src, this.GetUrl(this.Email, this.Size, this.DefaultImage, this.Rating));
        }

        private string GetHash(string Email)
        {
            Email = Email.ToLower();
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.ASCII.GetBytes(Email);
            bytes = provider.ComputeHash(bytes);
            string str = "";
            foreach (byte num in bytes)
            {
                str = str + num.ToString("x2");
            }
            return str;
        }

        private string GetUrl(string email, int? size, string defaultImage, GravatarRating rating)
        {
            StringBuilder builder = new StringBuilder("http://www.gravatar.com/avatar/");
            builder.Append(this.GetHash(this.Email));
            if (!size.HasValue)
            {
                size = 80;
            }
            builder.Append("?s=");
            builder.Append(size);
            if (!string.IsNullOrEmpty(defaultImage))
            {
                builder.Append("&d=");
                builder.Append(defaultImage);
            }
            else if (this.DefaultImageBehavior != GravatarDefaultImageBehavior.Default)
            {
                string str = this.DefaultImageBehavior.ToString().ToLower();
                if (this.DefaultImageBehavior == GravatarDefaultImageBehavior.MysteryMan)
                {
                    str = "mm";
                }
                builder.Append("&d=" + str);
            }
            if (rating != GravatarRating.Default)
            {
                builder.Append("&r=");
                builder.Append(rating.ToString().ToLowerInvariant());
            }
            return builder.ToString();
        }

        [Category("Behavior"), ClientPropertyName("defaultImage"), ExtenderControlProperty, Description("Image, that will be shown by default.")]
        public string DefaultImage { get; set; }

        [ClientPropertyName("defaultImage"), ExtenderControlProperty, Description("Behavior, that will be by default."), Category("Behavior")]
        public GravatarDefaultImageBehavior DefaultImageBehavior { get; set; }

        [ClientPropertyName("email"), Description("Account email."), Category("Behavior"), ExtenderControlProperty]
        public string Email { get; set; }

        [ExtenderControlProperty, Description("Image rating."), ClientPropertyName("rating"), Category("Behavior")]
        public GravatarRating Rating { get; set; }

        [ExtenderControlProperty, Description("Image size."), ClientPropertyName("size"), Category("Behavior")]
        public int? Size { get; set; }
    }
}

