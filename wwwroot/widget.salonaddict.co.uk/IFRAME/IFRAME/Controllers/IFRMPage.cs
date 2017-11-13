namespace IFRAME.Controllers
{
    using System;
    using System.Web.UI;

    public class IFRMPage : Page
    {
        public string GetLocaleResourceString(string name) => 
            base.GetLocalResourceObject(name)?.ToString();

        protected override void OnPreInit(EventArgs e)
        {
            if (!base.Request.Url.IsLoopback)
            {
                bool isSecureConnection = base.Request.IsSecureConnection;
            }
            this.Theme = IFRMContext.Current.WorkingTheme;
            base.OnPreInit(e);
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            base.OnPreRenderComplete(e);
            IFRMHelper.RenderPageTitle(this.Page, this.GetLocaleResourceString("Page.Title"));
        }
    }
}

