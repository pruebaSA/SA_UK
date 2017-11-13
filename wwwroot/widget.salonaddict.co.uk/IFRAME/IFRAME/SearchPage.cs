namespace IFRAME
{
    using IFRAME.Controllers;
    using System;
    using System.Web.UI.WebControls;

    public class SearchPage : IFRMPage
    {
        protected Image imgLoader;
        protected Panel pnl;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            base.OnPreRenderComplete(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.imgLoader.ImageUrl = this.Page.ResolveUrl($"~/App_Themes/{base.Theme}/images/ajax-loader.gif");
        }
    }
}

