namespace IFRAME.SecureArea
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using SA.Payments.Realex.RealAuth;
    using SA.Payments.Realex.RealVault;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.UI.WebControls;

    public class PaymentMethod_CreatePage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected DropDownList ddlCardType;
        protected DropDownList ddlExpiryMonth;
        protected DropDownList ddlExpiryYear;
        protected Label lblError;
        protected Panel pnl;
        protected TextBox txtAlias;
        protected TextBox txtCardName;
        protected TextBox txtCardNumber;
        protected RequiredFieldValidator valAlias;
        protected ValidatorCalloutExtender valAliasEx;
        protected RequiredFieldValidator valCardName;
        protected ValidatorCalloutExtender valCardNameEx;
        protected RegularExpressionValidator valCardNameRegex1;
        protected ValidatorCalloutExtender valCardNameRegex1Ex;
        protected RequiredFieldValidator valCardNumber;
        protected ValidatorCalloutExtender valCardNumberEx;
        protected RegularExpressionValidator valCardNumberRegex1;
        protected ValidatorCalloutExtender valCardNumberRegex1Ex;

        private void BindExpiryDropDownLists()
        {
            this.BindExpiryMonthDropDownList();
            this.BindExpiryYearDropDownList();
        }

        private void BindExpiryMonthDropDownList()
        {
            ListItemCollection items = new ListItemCollection();
            for (int i = 1; i < 13; i++)
            {
                string text = i.ToString("00");
                ListItem item = new ListItem(text, i.ToString("00"));
                items.Add(item);
            }
            this.ddlExpiryMonth.DataSource = items;
            this.ddlExpiryMonth.DataTextField = "Text";
            this.ddlExpiryMonth.DataValueField = "Value";
            this.ddlExpiryMonth.DataBind();
        }

        private void BindExpiryYearDropDownList()
        {
            DateTime time = DateTime.Now.AddYears(-1);
            ListItemCollection items = new ListItemCollection();
            for (int i = 0; i < 0x15; i++)
            {
                time = time.AddYears(1);
                string text = time.Year.ToString("0000");
                ListItem item = new ListItem(text, time.Year.ToString("0000"));
                items.Add(item);
            }
            this.ddlExpiryYear.DataSource = items;
            this.ddlExpiryYear.DataTextField = "Text";
            this.ddlExpiryYear.DataValueField = "Value";
            this.ddlExpiryYear.DataBind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("paymentmethods.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                string str = this.txtAlias.Text.Trim();
                string str2 = this.ddlCardType.SelectedValue.ToUpper().Trim();
                string cardName = this.txtCardName.Text.Trim();
                string cardNumber = this.txtCardNumber.Text.Replace("-", string.Empty).Replace(" ", string.Empty).Trim();
                string clearText = cardNumber.Substring(cardNumber.Length - 4);
                string str6 = string.Empty.PadLeft(cardNumber.Length - 4, '*') + clearText;
                string str7 = this.ddlExpiryMonth.SelectedValue.Trim();
                string str8 = this.ddlExpiryYear.SelectedValue.Trim();
                DateTime time = DateTime.ParseExact($"{str7}-{str8}", "MM-yyyy", CultureInfo.InvariantCulture);
                if (time.CompareTo(DateTime.Today) < 0)
                {
                    this.lblError.Text = base.GetLocaleResourceString("Error.Expiry");
                }
                else
                {
                    SalonDB salon = IFRMContext.Current.Salon;
                    SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
                    List<SalonPaymentMethodDB> salonPaymentMethodsBySalonId = IoC.Resolve<ISalonManager>().GetSalonPaymentMethodsBySalonId(salon.SalonId);
                    if (salonPaymentMethodsBySalonId.Count > 1)
                    {
                        this.lblError.Text = base.GetLocaleResourceString("Error.PaymentMethodLimit");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(salon.RealexPayerRef))
                        {
                            string payerRef = "SLN" + Guid.NewGuid().ToString().ToUpperInvariant().Replace("-", string.Empty);
                            SA.Payments.Realex.RealVault.SettingsProviderConfig config = new SA.Payments.Realex.RealVault.SettingsProviderConfig();
                            PayerCreateResponse response = new PayerCreateRequest("SALON", payerRef, workingUser.FirstName, workingUser.LastName, config).GetResponse<PayerCreateResponse>();
                            if (response.HasErrors() || !response.IsValid(config))
                            {
                                throw new Exception(response.Message);
                            }
                            salon.RealexPayerRef = payerRef;
                            salon = IoC.Resolve<ISalonManager>().UpdateSalon(salon);
                        }
                        SA.Payments.Realex.RealAuth.CardType realAuthCardType = this.GetRealAuthCardType(str2);
                        string orderID = "AUTH" + Guid.NewGuid().ToString().ToUpperInvariant().Replace("-", string.Empty);
                        SA.Payments.Realex.RealAuth.SettingsProviderConfig settings = new SA.Payments.Realex.RealAuth.SettingsProviderConfig();
                        AuthResponse response2 = new AuthRequest(orderID, "100", "GBP", realAuthCardType, cardName, cardNumber, time.ToString("MMyy"), false, settings).GetResponse<AuthResponse>();
                        if (response2.HasErrors())
                        {
                            this.lblError.Text = base.GetLocaleResourceString("Error.AuthFailed");
                            LogDB log = new LogDB {
                                CreatedOn = DateTime.Now,
                                Exception = "Authorization Failed.",
                                LogType = "REALEX",
                                Message = response2.Message,
                                PageURL = base.Request.RawUrl,
                                UserHostAddress = base.Request.UserHostAddress,
                                UserId = new Guid?(workingUser.UserId)
                            };
                            log = IoC.Resolve<ILogManager>().InsertError(log);
                        }
                        else
                        {
                            string cardRef = string.Empty;
                            SA.Payments.Realex.RealVault.CardType realVaultCardType = this.GetRealVaultCardType(str2);
                            cardRef = this.GenerateCardRef(realVaultCardType, salonPaymentMethodsBySalonId);
                            SA.Payments.Realex.RealVault.SettingsProviderConfig config3 = new SA.Payments.Realex.RealVault.SettingsProviderConfig();
                            CardCreateResponse response3 = new CardCreateRequest(salon.RealexPayerRef, cardRef, realVaultCardType, cardName, cardNumber, time.ToString("MMyy"), config3).GetResponse<CardCreateResponse>();
                            if (response3.HasErrors())
                            {
                                throw new Exception(response3.Message);
                            }
                            SalonPaymentMethodDB paymentMethod = new SalonPaymentMethodDB {
                                Active = true,
                                Alias = str,
                                CardCvv2 = string.Empty,
                                CardExpirationMonth = IoC.Resolve<ISecurityManager>().EncryptUserPassword(str7, IFRAME.Controllers.Settings.Security_Key_3DES),
                                CardExpirationYear = IoC.Resolve<ISecurityManager>().EncryptUserPassword(str8, IFRAME.Controllers.Settings.Security_Key_3DES),
                                CardName = IoC.Resolve<ISecurityManager>().EncryptUserPassword(cardName, IFRAME.Controllers.Settings.Security_Key_3DES),
                                CardNumber = IoC.Resolve<ISecurityManager>().EncryptUserPassword(clearText, IFRAME.Controllers.Settings.Security_Key_3DES),
                                CardType = IoC.Resolve<ISecurityManager>().EncryptUserPassword(str2, IFRAME.Controllers.Settings.Security_Key_3DES),
                                CreatedBy = workingUser.Username,
                                CreatedOn = DateTime.Now,
                                IsPrimary = salonPaymentMethodsBySalonId.Count == 0,
                                MaskedCardNumber = IoC.Resolve<ISecurityManager>().EncryptUserPassword(str6, IFRAME.Controllers.Settings.Security_Key_3DES),
                                RealexPayerRef = salon.RealexPayerRef,
                                RealexCardRef = cardRef,
                                SalonId = salon.SalonId,
                                UpdatedBy = string.Empty,
                                UpdatedOn = DateTime.Now
                            };
                            paymentMethod = IoC.Resolve<ISalonManager>().InsertSalonPaymentMethod(paymentMethod);
                            string uRL = IFRMHelper.GetURL("paymentmethods.aspx", new string[0]);
                            base.Response.Redirect(uRL, true);
                        }
                    }
                }
            }
        }

        private string GenerateCardRef(SA.Payments.Realex.RealVault.CardType cardType, List<SalonPaymentMethodDB> methods)
        {
            Predicate<SalonPaymentMethodDB> match = null;
            string cardRef = string.Empty;
            switch (cardType)
            {
                case SA.Payments.Realex.RealVault.CardType.Visa:
                    cardRef = "VISA";
                    break;

                case SA.Payments.Realex.RealVault.CardType.MasterCard:
                    cardRef = "MC";
                    break;

                case SA.Payments.Realex.RealVault.CardType.Switch:
                    cardRef = "MAESTRO";
                    break;

                case SA.Payments.Realex.RealVault.CardType.Amex:
                    cardRef = "AMEX";
                    break;

                case SA.Payments.Realex.RealVault.CardType.Laser:
                    cardRef = "LASER";
                    break;
            }
            for (int i = 1; i < 20; i++)
            {
                cardRef = $"{cardType}{i.ToString("00")}";
                if (match == null)
                {
                    match = item => item.RealexCardRef == cardRef;
                }
                if (!methods.Exists(match))
                {
                    break;
                }
            }
            return cardRef;
        }

        private SA.Payments.Realex.RealAuth.CardType GetRealAuthCardType(string value)
        {
            value = value.ToUpperInvariant();
            switch (value)
            {
                case "VISA":
                    return SA.Payments.Realex.RealAuth.CardType.Visa;

                case "MASTERCARD":
                    return SA.Payments.Realex.RealAuth.CardType.MasterCard;

                case "LASER":
                    return SA.Payments.Realex.RealAuth.CardType.Laser;

                case "AMEX":
                    return SA.Payments.Realex.RealAuth.CardType.Amex;

                case "MAESTRO":
                    return SA.Payments.Realex.RealAuth.CardType.Switch;
            }
            throw new Exception("Invalid card type");
        }

        private SA.Payments.Realex.RealVault.CardType GetRealVaultCardType(string value)
        {
            value = value.ToUpperInvariant();
            switch (value)
            {
                case "VISA":
                    return SA.Payments.Realex.RealVault.CardType.Visa;

                case "MASTERCARD":
                    return SA.Payments.Realex.RealVault.CardType.MasterCard;

                case "LASER":
                    return SA.Payments.Realex.RealVault.CardType.Laser;

                case "AMEX":
                    return SA.Payments.Realex.RealVault.CardType.Amex;

                case "MAESTRO":
                    return SA.Payments.Realex.RealVault.CardType.Switch;
            }
            throw new Exception("Invalid card type");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Form.Attributes.Add("autocomplete", "off");
            if (!this.Page.IsPostBack)
            {
                this.BindExpiryDropDownLists();
            }
        }
    }
}

