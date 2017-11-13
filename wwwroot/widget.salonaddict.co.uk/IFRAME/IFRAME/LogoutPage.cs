namespace IFRAME
{
    using IFRAME.Controllers;
    using System;
    using System.Web;
    using System.Web.Security;

    public class LogoutPage : IFRMSecurePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Response.Cache.SetExpires(new DateTime(0x7cb, 5, 6, 12, 0, 0, DateTimeKind.Utc));
            base.Response.Cache.SetNoStore();
            base.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            base.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            base.Response.Cache.AppendCacheExtension("post-check=0,pre-check=0");
            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Abandon();
            }
            FormsAuthentication.SignOut();
            string uRL = IFRMHelper.GetURL("login.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }
    }
}

