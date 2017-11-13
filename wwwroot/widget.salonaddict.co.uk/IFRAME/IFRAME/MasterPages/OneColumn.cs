namespace IFRAME.MasterPages
{
    using IFRAME.Controllers;
    using IFRAME.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class OneColumn : MasterPage
    {
        protected Footer cntlFooter;
        protected NoScript cntlNoScript;
        protected LoginView lv;
        protected ContentPlaceHolder ph1c;

        protected void Page_Load(object sender, EventArgs e)
        {
            SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
            if (workingUser != null)
            {
                HyperLink link = this.lv.FindControl("hlUsername") as HyperLink;
                if (link != null)
                {
                    if (workingUser.IsAdmin)
                    {
                        link.NavigateUrl = IFRMHelper.GetURL(this.Page.ResolveUrl("~/admin/"), new string[0]);
                    }
                    else
                    {
                        link.NavigateUrl = IFRMHelper.GetURL(this.Page.ResolveUrl("~/securearea/"), new string[0]);
                    }
                }
                Literal literal = this.lv.FindControl("ltrUsername") as Literal;
                if (literal != null)
                {
                    string firstName = workingUser.FirstName;
                    if (string.IsNullOrEmpty(firstName))
                    {
                        firstName = workingUser.Username;
                    }
                    literal.Text = firstName;
                }
            }
        }
    }
}

