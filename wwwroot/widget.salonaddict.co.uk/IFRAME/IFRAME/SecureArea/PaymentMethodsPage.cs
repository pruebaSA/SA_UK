namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class PaymentMethodsPage : IFRMSecurePage
    {
        protected Button btnAdd;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
            this.gv.Columns[3].HeaderText = base.GetLocaleResourceString("gv.Columns[3].HeaderText");
            this.gv.Columns[4].HeaderText = base.GetLocaleResourceString("gv.Columns[4].HeaderText");
        }

        private void BindSalonPaymentMethods()
        {
            SalonDB salon = IFRMContext.Current.Salon;
            List<SalonPaymentMethodDB> salonPaymentMethodsBySalonId = IoC.Resolve<ISalonManager>().GetSalonPaymentMethodsBySalonId(salon.SalonId);
            List<object> list2 = new List<object>();
            foreach (SalonPaymentMethodDB ddb in salonPaymentMethodsBySalonId)
            {
                var item = new {
                    SalonPaymentMethodId = ddb.SalonPaymentMethodId,
                    Active = ddb.Active,
                    Alias = ddb.Alias,
                    CardExpirationMonth = IoC.Resolve<ISecurityManager>().DecryptUserPassword(ddb.CardExpirationMonth, IFRAME.Controllers.Settings.Security_Key_3DES),
                    CardExpirationYear = IoC.Resolve<ISecurityManager>().DecryptUserPassword(ddb.CardExpirationYear, IFRAME.Controllers.Settings.Security_Key_3DES),
                    IsPrimary = ddb.IsPrimary
                };
                list2.Add(item);
            }
            this.gv.DataSource = list2;
            this.gv.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("paymentmethod-create.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        private string ConvertToFriendlyCardName(string value)
        {
            switch (value)
            {
                case "VISA":
                    return "Visa";

                case "MASTERCARD":
                    return "MasterCard";

                case "LASER":
                    return "Laser";

                case "MAESTRO":
                    return "Maestro";
            }
            return value;
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.RowIndex].Value.ToString());
            string str = $"{"pmid"}={guid}";
            string uRL = IFRMHelper.GetURL("paymentmethod-delete.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.NewEditIndex].Value.ToString());
            string str = $"{"pmid"}={guid}";
            string uRL = IFRMHelper.GetURL("paymentmethod-edit.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Form.Attributes.Add("autocomplete", "off");
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindSalonPaymentMethods();
            }
        }

        protected void rb_CheckedChanged(object sender, EventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            List<SalonPaymentMethodDB> salonPaymentMethodsBySalonId = IoC.Resolve<ISalonManager>().GetSalonPaymentMethodsBySalonId(salon.SalonId);
            foreach (GridViewRow row in this.gv.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    RadioButton button = row.FindControl("rb") as RadioButton;
                    if (button == sender)
                    {
                        Guid guid = new Guid(this.gv.DataKeys[row.RowIndex].Value.ToString());
                        foreach (SalonPaymentMethodDB ddb in salonPaymentMethodsBySalonId)
                        {
                            if (ddb.SalonPaymentMethodId == guid)
                            {
                                ddb.IsPrimary = true;
                                IoC.Resolve<ISalonManager>().UpdateSalonPaymentMethod(ddb);
                            }
                            else
                            {
                                ddb.IsPrimary = false;
                                IoC.Resolve<ISalonManager>().UpdateSalonPaymentMethod(ddb);
                            }
                        }
                        break;
                    }
                }
            }
            this.BindSalonPaymentMethods();
        }
    }
}

