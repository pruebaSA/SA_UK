namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class SalonCreatePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected TabPanel p1;
        protected Panel pnl;
        protected Panel pnl2;
        protected TabContainer tc;
        protected TextBox txtAddressLine1;
        protected TextBox txtAddressLine2;
        protected TextBox txtCity;
        protected TextBox txtLatitude;
        protected TextBox txtLongitude;
        protected TextBox txtName;
        protected TextBox txtPhone;
        protected TextBox txtPostalCode;
        protected TextBox txtSalonId;
        protected RequiredFieldValidator valAddressLine1;
        protected ValidatorCalloutExtender valAddressLine1Ex;
        protected RequiredFieldValidator valAddressLine3;
        protected ValidatorCalloutExtender valAddressLine3Ex;
        protected RequiredFieldValidator valAddressLine5;
        protected ValidatorCalloutExtender valAddressLine5Ex;
        protected RequiredFieldValidator valLatitude;
        protected ValidatorCalloutExtender valLatitudeEx;
        protected RequiredFieldValidator valLongitude;
        protected ValidatorCalloutExtender valLongitudeEx;
        protected RequiredFieldValidator valName;
        protected ValidatorCalloutExtender valNameEx;
        protected RequiredFieldValidator valPhone;
        protected ValidatorCalloutExtender valPhoneEx;
        protected RequiredFieldValidator valSalonId;
        protected ValidatorCalloutExtender valSalonIdEx;

        private void ApplyLocalization()
        {
            this.tc.Tabs[0].HeaderText = base.GetLocaleResourceString("Tabs[0].HeaderText");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.PostedReturnURL != null)
            {
                base.Response.Redirect(this.PostedReturnURL, true);
            }
            string uRL = IFRMHelper.GetURL("salons.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonDB salon = new SalonDB {
                    Abbreviation = this.txtSalonId.Text.Trim(),
                    Active = true,
                    BookOnWeb = true,
                    BookOnWidget = true,
                    BookOnMobile = true,
                    CreatedOnUtc = DateTime.UtcNow,
                    CurrencyCode = "GBP",
                    Deleted = false,
                    Published = true,
                    ShowOnWeb = true,
                    ShowOnWidget = true,
                    ShowOnMobile = true,
                    Name = this.txtName.Text.Trim(),
                    SEName = Guid.NewGuid().ToString().ToLowerInvariant(),
                    PhoneNumber = this.txtPhone.Text.Trim(),
                    ShortDescription = string.Empty,
                    CreatedOnUtc = DateTime.UtcNow,
                    AddressLine1 = this.txtAddressLine1.Text.Trim(),
                    AddressLine2 = this.txtAddressLine2.Text.Trim(),
                    AddressLine3 = this.txtCity.Text.Trim(),
                    AddressLine4 = string.Empty,
                    AddressLine5 = this.txtPostalCode.Text.Trim(),
                    County = string.Empty,
                    CityTown = this.txtCity.Text.Trim(),
                    ZipPostalCode = this.txtPostalCode.Text.Trim(),
                    Directions = string.Empty,
                    Longitude = this.txtLongitude.Text.Trim(),
                    Latitude = this.txtLatitude.Text.Trim()
                };
                salon = IoC.Resolve<ISalonManager>().InsertSalon(salon);
                OpeningHoursDB hours = new OpeningHoursDB {
                    SalonId = salon.SalonId,
                    MonClosed = false,
                    TueClosed = false,
                    WedClosed = false,
                    ThuClosed = false,
                    FriClosed = false,
                    SatClosed = false,
                    SunClosed = false,
                    MonStart1 = new TimeSpan?(TimeSpan.Parse("06:00")),
                    MonEnd1 = new TimeSpan?(TimeSpan.Parse("22:00")),
                    TueStart1 = new TimeSpan?(TimeSpan.Parse("06:00")),
                    TueEnd1 = new TimeSpan?(TimeSpan.Parse("22:00")),
                    WedStart1 = new TimeSpan?(TimeSpan.Parse("06:00")),
                    WedEnd1 = new TimeSpan?(TimeSpan.Parse("22:00")),
                    ThuStart1 = new TimeSpan?(TimeSpan.Parse("06:00")),
                    ThuEnd1 = new TimeSpan?(TimeSpan.Parse("22:00")),
                    FriStart1 = new TimeSpan?(TimeSpan.Parse("06:00")),
                    FriEnd1 = new TimeSpan?(TimeSpan.Parse("22:00")),
                    SatStart1 = new TimeSpan?(TimeSpan.Parse("06:00")),
                    SatEnd1 = new TimeSpan?(TimeSpan.Parse("22:00")),
                    SunStart1 = new TimeSpan?(TimeSpan.Parse("06:00")),
                    SunEnd1 = new TimeSpan?(TimeSpan.Parse("22:00")),
                    ShowOnWeb = true,
                    ShowOnMobile = true,
                    ShowOnWidget = true
                };
                hours = IoC.Resolve<ISalonManager>().InsertOpeningHours(hours);
                List<ClosingDayDB> closingDays = new List<ClosingDayDB>();
                ClosingDayDB item = null;
                item = new ClosingDayDB {
                    Active = true,
                    Category = "Holiday",
                    CycleLength = 1,
                    CyclePeriodType = 40,
                    Description = "New Year's Day",
                    SalonId = salon.SalonId,
                    StartDateUtc = new DateTime(DateTime.Now.Year, 1, 1)
                };
                closingDays.Add(item);
                item = new ClosingDayDB {
                    Active = true,
                    Category = "Holiday",
                    CycleLength = 1,
                    CyclePeriodType = 40,
                    Description = "Christmas Day",
                    SalonId = salon.SalonId,
                    StartDateUtc = new DateTime(DateTime.Now.Year, 12, 0x19)
                };
                closingDays.Add(item);
                item = new ClosingDayDB {
                    Active = true,
                    Category = "Holiday",
                    CycleLength = 1,
                    CyclePeriodType = 40,
                    Description = "St. Stephen's Day",
                    SalonId = salon.SalonId,
                    StartDateUtc = new DateTime(DateTime.Now.Year, 12, 0x1a)
                };
                closingDays.Add(item);
                IoC.Resolve<ISalonManager>().InsertClosingDaysXML(closingDays);
                DateTime today = DateTime.Today;
                if (today.Day > 0x1c)
                {
                    today.AddDays((double) -(today.Day % 0x1c));
                }
                DateTime time2 = today.AddMonths(2).AddDays(-1.0);
                DateTime time3 = today;
                DateTime time4 = time3.AddMonths(1).AddDays(-1.0);
                WSPDB wspdb = new WSPDB {
                    Active = true,
                    PlanStartDate = IFRMHelper.ToUrlFriendlyDate(today),
                    PlanEndDate = IFRMHelper.ToUrlFriendlyDate(time2),
                    BillStartDate = IFRMHelper.ToUrlFriendlyDate(time3),
                    BillEndDate = IFRMHelper.ToUrlFriendlyDate(time4),
                    CancelDate = string.Empty,
                    CreatedBy = IFRMContext.Current.WorkingUser.Username,
                    CreatedOn = DateTime.Now,
                    Description = "Trial Plan",
                    ExcessFeeWT = 0,
                    ExcessLimitWT = 0,
                    ParentId = null,
                    PlanPrice = 0,
                    PlanType = "10",
                    Prorate = false,
                    SalonId = salon.SalonId,
                    UpdatedBy = IFRMContext.Current.WorkingUser.Username,
                    UpdatedOn = DateTime.Now
                };
                wspdb = IoC.Resolve<IBillingManager>().InsertWSP(wspdb);
                string str = $"{"s"}={salon.Name.Substring(0, 1).ToUpperInvariant()}";
                string uRL = IFRMHelper.GetURL("salons.aspx", new string[] { str });
                base.Response.Redirect(uRL, true);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
        }

        public string PostedReturnURL
        {
            get
            {
                string str = base.Request.QueryString["url"];
                if (string.IsNullOrEmpty(str))
                {
                    return null;
                }
                if (!IFRMHelper.IsUrlLocalToHost(str))
                {
                    return null;
                }
                return str;
            }
        }
    }
}

