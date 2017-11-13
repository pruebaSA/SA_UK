namespace IFRAME
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    public class LoginPage : IFRMSecurePage
    {
        protected Button btnCancel1;
        protected Button btnLogin;
        protected Label lblLoginError;
        protected MultiView mv;
        protected Panel pnl;
        protected TextBox txtPassword;
        protected TextBox txtUsername;
        protected System.Web.UI.WebControls.View v0;
        protected System.Web.UI.WebControls.View v1;
        protected RequiredFieldValidator valPassword;
        protected ValidatorCalloutExtender valPasswordEx;
        protected RequiredFieldValidator valUsername;
        protected ValidatorCalloutExtender valUsernameEx;

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("~/", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                string username = this.txtUsername.Text.Trim();
                string str2 = this.txtPassword.Text.Trim();
                if (username.Length < IFRAME.Controllers.Settings.IFRMMembership_MinimumUsernameLength)
                {
                    this.lblLoginError.Text = base.GetLocaleResourceString("lblLoginError.Text");
                }
                else if (username.Length > 30)
                {
                    this.lblLoginError.Text = base.GetLocaleResourceString("lblLoginError.Text");
                }
                else if (str2.Length < IFRAME.Controllers.Settings.IFRMMembership_MinimumPasswordLength)
                {
                    this.lblLoginError.Text = base.GetLocaleResourceString("lblLoginError.Text");
                }
                else if (str2.Length > 20)
                {
                    this.lblLoginError.Text = base.GetLocaleResourceString("lblLoginError.Text");
                }
                else
                {
                    SalonUserDB salonUserByUsername = IoC.Resolve<IUserManager>().GetSalonUserByUsername(username);
                    if (salonUserByUsername == null)
                    {
                        this.lblLoginError.Text = base.GetLocaleResourceString("lblLoginError.Text");
                    }
                    else if (IoC.Resolve<ISecurityManager>().DecryptUserPassword(salonUserByUsername.Password, IFRAME.Controllers.Settings.Security_Key_3DES) != str2)
                    {
                        this.lblLoginError.Text = base.GetLocaleResourceString("lblLoginError.Text");
                    }
                    else if (!salonUserByUsername.Active || salonUserByUsername.Deleted)
                    {
                        this.lblLoginError.Text = base.GetLocaleResourceString("lblLoginError.Text");
                    }
                    else if (salonUserByUsername.IsGuest)
                    {
                        this.lblLoginError.Text = base.GetLocaleResourceString("lblLoginError.Text");
                    }
                    else if (!salonUserByUsername.IsAdmin && (salonUserByUsername.SalonId != IFRMContext.Current.Salon.SalonId))
                    {
                        this.lblLoginError.Text = base.GetLocaleResourceString("lblLoginError.Text");
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(username, true);
                        if (salonUserByUsername.IsAdmin)
                        {
                            base.Response.Redirect(IFRMHelper.GetURL("admin/default.aspx", new string[0]), true);
                        }
                        else
                        {
                            base.Response.Redirect(IFRMHelper.GetURL("securearea/default.aspx", new string[0]), true);
                        }
                    }
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                base.Response.Redirect(IFRMHelper.GetURL("default.aspx", new string[0]), true);
            }
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Form.Attributes.Add("autocomplete", "off");
            HttpContext.Current.Response.AddHeader("p3p", "CP=\"CAO PSA OUR\"");
            if (!this.Page.IsPostBack)
            {
                string str = base.Request.UserAgent.ToLowerInvariant();
                if ((!str.Contains("chrome") && str.Contains("safari")) && (base.Request.Cookies["safari_cookie_fix"] == null))
                {
                    this.mv.ActiveViewIndex = 1;
                }
            }
        }
    }
}

