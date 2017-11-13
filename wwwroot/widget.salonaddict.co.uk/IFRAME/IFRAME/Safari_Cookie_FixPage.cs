namespace IFRAME
{
    using IFRAME.Controllers;
    using System;
    using System.Web;

    public class Safari_Cookie_FixPage : IFRMSecurePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.SetSafariCookie();
        }

        private void SetSafariCookie()
        {
            try
            {
                HttpCookie cookie = new HttpCookie("safari_cookie_fix") {
                    Expires = DateTime.Now.AddYears(1),
                    Value = "1"
                };
                base.Response.Cookies.Add(cookie);
            }
            catch
            {
            }
        }
    }
}

