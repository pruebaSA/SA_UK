namespace IFRAME.Admin.Modules
{
    using IFRAME.Controllers;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.UI.WebControls;

    public class Menu : IFRMUserControl
    {
        protected Literal ltrBillingCount;
        protected Panel pnlBillingBadge;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public MenuItem SelectedItem { get; set; }

        public enum MenuItem
        {
            None,
            Overview,
            Salons,
            Billing,
            Profile,
            Reports,
            Audit
        }
    }
}

