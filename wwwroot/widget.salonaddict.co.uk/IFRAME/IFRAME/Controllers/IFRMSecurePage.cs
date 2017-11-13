namespace IFRAME.Controllers
{
    using System;
    using System.Web.UI;

    public class IFRMSecurePage : Page
    {
        public string GetLocaleResourceString(string name) => 
            base.GetLocalResourceObject(name)?.ToString();

        protected override void OnPreInit(EventArgs e)
        {
            if (!base.Request.Url.IsLoopback && !base.Request.IsSecureConnection)
            {
                base.Response.Redirect(base.Request.Url.ToString().Replace("http://", "https://"), true);
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

