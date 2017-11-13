namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Web;
    using System.Web.UI;

    [PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ParseChildren(true), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.ImageButton", "HTMLEditor.Toolbar_buttons.ImageButton.js")]
    public abstract class ImageButton : CommonButton
    {
        public ImageButton() : base(HtmlTextWriterTag.Img)
        {
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            writer.AddAttribute("src", this.NormalSrc);
            writer.AddAttribute("alt", "");
            base.AddAttributesToRender(writer);
        }

        private string getImagePath(Type type, string name, string ext, Toolbar toolbar)
        {
            string buttonImagesFolder = toolbar.ButtonImagesFolder;
            string webResourceUrl = base.Page.ClientScript.GetWebResourceUrl(type, "HTMLEditor.Images." + name + "." + ext);
            if (buttonImagesFolder.Length > 0)
            {
                string originalPath = buttonImagesFolder + name + "." + ext;
                string path = "";
                if (base.IsDesign && (base._designer != null))
                {
                    path = base._designer.MapPath(originalPath);
                }
                else
                {
                    path = HttpContext.Current.Server.MapPath(originalPath);
                }
                if ((path != null) && File.Exists(path))
                {
                    webResourceUrl = originalPath;
                }
            }
            return webResourceUrl;
        }

        internal void InternalRegisterButtonImages(string name)
        {
            this.RegisterButtonImages(name, "gif");
        }

        protected void RegisterButtonImages(string name)
        {
            this.RegisterButtonImages(name, "gif");
        }

        protected void RegisterButtonImages(string name, string ext)
        {
            Type type2;
            Type type = base.GetType();
            Toolbar toolbar = null;
            for (Control control = this.Parent; control != null; control = control.Parent)
            {
                toolbar = control as Toolbar;
                if (toolbar != null)
                {
                    break;
                }
            }
            if (toolbar == null)
            {
                throw new NotSupportedException("Toolbar's ImageButton can be inside Toolbar control only");
            }
            bool flag = false;
            for (type2 = type; type2 != typeof(CommonButton); type2 = type2.BaseType)
            {
                if (type2 == typeof(AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator))
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                this.NormalSrc = this.getImagePath(this.BaseImageButtonType, name, ext, toolbar);
            }
            else
            {
                this.NormalSrc = this.getImagePath(this.BaseImageButtonType, name + "_n", ext, toolbar);
                this.DownSrc = this.getImagePath(this.BaseImageButtonType, name + "_a", ext, toolbar);
            }
            bool flag2 = false;
            for (type2 = type.BaseType; type2 != typeof(CommonButton); type2 = type2.BaseType)
            {
                if ((type2 == typeof(EditorToggleButton)) || (type2 == typeof(ModeButton)))
                {
                    flag2 = true;
                    break;
                }
            }
            if (flag2)
            {
                this.ActiveSrc = this.DownSrc;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeActiveSrc() => 
            base.IsRenderingScript;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDownSrc() => 
            base.IsRenderingScript;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeHoverSrc() => 
            base.IsRenderingScript;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeNormalSrc() => 
            base.IsRenderingScript;

        [Category("Appearance"), ExtenderControlProperty, ClientPropertyName("activeSrc"), DefaultValue("")]
        public string ActiveSrc
        {
            get => 
                (this.ViewState["ActiveSrc"] ?? string.Empty);
            set
            {
                this.ViewState["ActiveSrc"] = value;
            }
        }

        protected virtual Type BaseImageButtonType =>
            typeof(ImageButton);

        [ExtenderControlProperty, DefaultValue(""), ClientPropertyName("downSrc"), Category("Appearance")]
        public string DownSrc
        {
            get => 
                (this.ViewState["DownSrc"] ?? string.Empty);
            set
            {
                this.ViewState["DownSrc"] = value;
            }
        }

        [ClientPropertyName("hoverSrc"), DefaultValue(""), Category("Appearance"), ExtenderControlProperty]
        public string HoverSrc
        {
            get => 
                (this.ViewState["HoverSrc"] ?? string.Empty);
            set
            {
                this.ViewState["HoverSrc"] = value;
            }
        }

        [ExtenderControlProperty, ClientPropertyName("normalSrc"), DefaultValue(""), Category("Appearance")]
        public string NormalSrc
        {
            get => 
                (this.ViewState["NormalSrc"] ?? string.Empty);
            set
            {
                this.ViewState["NormalSrc"] = value;
            }
        }
    }
}

