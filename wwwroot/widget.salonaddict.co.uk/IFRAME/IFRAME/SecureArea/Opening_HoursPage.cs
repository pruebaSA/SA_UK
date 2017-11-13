namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Opening_HoursPage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected CheckBox cbFriday;
        protected CheckBox cbMonday;
        protected CheckBox cbSaturday;
        protected CheckBox cbSunday;
        protected CheckBox cbThursday;
        protected CheckBox cbTuesday;
        protected CheckBox cbWednesday;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected DropDownList ddlFridayEndHour;
        protected DropDownList ddlFridayEndMinute;
        protected DropDownList ddlFridayStartHour;
        protected DropDownList ddlFridayStartMinute;
        protected DropDownList ddlMondayEndHour;
        protected DropDownList ddlMondayEndMinute;
        protected DropDownList ddlMondayStartHour;
        protected DropDownList ddlMondayStartMinute;
        protected DropDownList ddlSaturdayEndHour;
        protected DropDownList ddlSaturdayEndMinute;
        protected DropDownList ddlSaturdayStartHour;
        protected DropDownList ddlSaturdayStartMinute;
        protected DropDownList ddlSundayEndHour;
        protected DropDownList ddlSundayEndMinute;
        protected DropDownList ddlSundayStartHour;
        protected DropDownList ddlSundayStartMinute;
        protected DropDownList ddlThursdayEndHour;
        protected DropDownList ddlThursdayEndMinute;
        protected DropDownList ddlThursdayStartHour;
        protected DropDownList ddlThursdayStartMinute;
        protected DropDownList ddlTuesdayEndHour;
        protected DropDownList ddlTuesdayEndMinute;
        protected DropDownList ddlTuesdayStartHour;
        protected DropDownList ddlTuesdayStartMinute;
        protected DropDownList ddlWednesdayEndHour;
        protected DropDownList ddlWednesdayEndMinute;
        protected DropDownList ddlWednesdayStartHour;
        protected DropDownList ddlWednesdayStartMinute;
        protected Panel pnl;

        private void BindOpeningHours()
        {
            SalonDB salon = IFRMContext.Current.Salon;
            OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salon.SalonId);
            if (!openingHoursBySalonId.MonClosed)
            {
                this.cbMonday.Checked = true;
                this.ddlMondayStartHour.SelectedValue = openingHoursBySalonId.MonStart1.Value.Hours.ToString("00");
                this.ddlMondayStartMinute.SelectedValue = openingHoursBySalonId.MonStart1.Value.Minutes.ToString("00");
                this.ddlMondayEndHour.SelectedValue = openingHoursBySalonId.MonEnd1.Value.Hours.ToString("00");
                this.ddlMondayEndMinute.SelectedValue = openingHoursBySalonId.MonEnd1.Value.Minutes.ToString("00");
            }
            else
            {
                this.cbMonday.Checked = false;
            }
            this.cbMonday_CheckedChanged(this, new EventArgs());
            if (!openingHoursBySalonId.TueClosed)
            {
                this.cbTuesday.Checked = true;
                this.ddlTuesdayStartHour.SelectedValue = openingHoursBySalonId.TueStart1.Value.Hours.ToString("00");
                this.ddlTuesdayStartMinute.SelectedValue = openingHoursBySalonId.TueStart1.Value.Minutes.ToString("00");
                this.ddlTuesdayEndHour.SelectedValue = openingHoursBySalonId.TueEnd1.Value.Hours.ToString("00");
                this.ddlTuesdayEndMinute.SelectedValue = openingHoursBySalonId.TueEnd1.Value.Minutes.ToString("00");
            }
            else
            {
                this.cbTuesday.Checked = false;
            }
            this.cbTuesday_CheckedChanged(this, new EventArgs());
            if (!openingHoursBySalonId.WedClosed)
            {
                this.cbWednesday.Checked = true;
                this.ddlWednesdayStartHour.SelectedValue = openingHoursBySalonId.WedStart1.Value.Hours.ToString("00");
                this.ddlWednesdayStartMinute.SelectedValue = openingHoursBySalonId.WedStart1.Value.Minutes.ToString("00");
                this.ddlWednesdayEndHour.SelectedValue = openingHoursBySalonId.WedEnd1.Value.Hours.ToString("00");
                this.ddlWednesdayEndMinute.SelectedValue = openingHoursBySalonId.WedEnd1.Value.Minutes.ToString("00");
            }
            else
            {
                this.cbWednesday.Checked = false;
            }
            this.cbWednesday_CheckedChanged(this, new EventArgs());
            if (!openingHoursBySalonId.ThuClosed)
            {
                this.cbThursday.Checked = true;
                this.ddlThursdayStartHour.SelectedValue = openingHoursBySalonId.ThuStart1.Value.Hours.ToString("00");
                this.ddlThursdayStartMinute.SelectedValue = openingHoursBySalonId.ThuStart1.Value.Minutes.ToString("00");
                this.ddlThursdayEndHour.SelectedValue = openingHoursBySalonId.ThuEnd1.Value.Hours.ToString("00");
                this.ddlThursdayEndMinute.SelectedValue = openingHoursBySalonId.ThuEnd1.Value.Minutes.ToString("00");
            }
            else
            {
                this.cbThursday.Checked = false;
            }
            this.cbThursday_CheckedChanged(this, new EventArgs());
            if (!openingHoursBySalonId.FriClosed)
            {
                this.cbFriday.Checked = true;
                this.ddlFridayStartHour.SelectedValue = openingHoursBySalonId.FriStart1.Value.Hours.ToString("00");
                this.ddlFridayStartMinute.SelectedValue = openingHoursBySalonId.FriStart1.Value.Minutes.ToString("00");
                this.ddlFridayEndHour.SelectedValue = openingHoursBySalonId.FriEnd1.Value.Hours.ToString("00");
                this.ddlFridayEndMinute.SelectedValue = openingHoursBySalonId.FriEnd1.Value.Minutes.ToString("00");
            }
            else
            {
                this.cbFriday.Checked = false;
            }
            this.cbFriday_CheckedChanged(this, new EventArgs());
            if (!openingHoursBySalonId.SatClosed)
            {
                this.cbSaturday.Checked = true;
                this.ddlSaturdayStartHour.SelectedValue = openingHoursBySalonId.SatStart1.Value.Hours.ToString("00");
                this.ddlSaturdayStartMinute.SelectedValue = openingHoursBySalonId.SatStart1.Value.Minutes.ToString("00");
                this.ddlSaturdayEndHour.SelectedValue = openingHoursBySalonId.SatEnd1.Value.Hours.ToString("00");
                this.ddlSaturdayEndMinute.SelectedValue = openingHoursBySalonId.SatEnd1.Value.Minutes.ToString("00");
            }
            else
            {
                this.cbSaturday.Checked = false;
            }
            this.cbSaturday_CheckedChanged(this, new EventArgs());
            if (!openingHoursBySalonId.SunClosed)
            {
                this.cbSunday.Checked = true;
                this.ddlSundayStartHour.SelectedValue = openingHoursBySalonId.SunStart1.Value.Hours.ToString("00");
                this.ddlSundayStartMinute.SelectedValue = openingHoursBySalonId.SunStart1.Value.Minutes.ToString("00");
                this.ddlSundayEndHour.SelectedValue = openingHoursBySalonId.SunEnd1.Value.Hours.ToString("00");
                this.ddlSundayEndMinute.SelectedValue = openingHoursBySalonId.SunEnd1.Value.Minutes.ToString("00");
            }
            else
            {
                this.cbSunday.Checked = false;
            }
            this.cbSunday_CheckedChanged(this, new EventArgs());
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.PostedReturnURL != null)
            {
                base.Response.Redirect(this.PostedReturnURL, true);
            }
            else
            {
                string uRL = IFRMHelper.GetURL("settings.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salon.SalonId);
                if (this.cbMonday.Checked)
                {
                    openingHoursBySalonId.MonClosed = false;
                    openingHoursBySalonId.MonStart1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlMondayStartHour.SelectedValue}:{this.ddlMondayStartMinute.SelectedValue}"));
                    openingHoursBySalonId.MonEnd1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlMondayEndHour.SelectedValue}:{this.ddlMondayEndMinute.SelectedValue}"));
                }
                else
                {
                    openingHoursBySalonId.MonClosed = true;
                }
                if (this.cbTuesday.Checked)
                {
                    openingHoursBySalonId.TueClosed = false;
                    openingHoursBySalonId.TueStart1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlTuesdayStartHour.SelectedValue}:{this.ddlTuesdayStartMinute.SelectedValue}"));
                    openingHoursBySalonId.TueEnd1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlTuesdayEndHour.SelectedValue}:{this.ddlTuesdayEndMinute.SelectedValue}"));
                }
                else
                {
                    openingHoursBySalonId.TueClosed = true;
                }
                if (this.cbWednesday.Checked)
                {
                    openingHoursBySalonId.WedClosed = false;
                    openingHoursBySalonId.WedStart1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlWednesdayStartHour.SelectedValue}:{this.ddlWednesdayStartMinute.SelectedValue}"));
                    openingHoursBySalonId.WedEnd1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlWednesdayEndHour.SelectedValue}:{this.ddlWednesdayEndMinute.SelectedValue}"));
                }
                else
                {
                    openingHoursBySalonId.WedClosed = true;
                }
                if (this.cbThursday.Checked)
                {
                    openingHoursBySalonId.ThuClosed = false;
                    openingHoursBySalonId.ThuStart1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlThursdayStartHour.SelectedValue}:{this.ddlThursdayStartMinute.SelectedValue}"));
                    openingHoursBySalonId.ThuEnd1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlThursdayEndHour.SelectedValue}:{this.ddlThursdayEndMinute.SelectedValue}"));
                }
                else
                {
                    openingHoursBySalonId.ThuClosed = true;
                }
                if (this.cbFriday.Checked)
                {
                    openingHoursBySalonId.FriClosed = false;
                    openingHoursBySalonId.FriStart1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlFridayStartHour.SelectedValue}:{this.ddlFridayStartMinute.SelectedValue}"));
                    openingHoursBySalonId.FriEnd1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlFridayEndHour.SelectedValue}:{this.ddlFridayEndMinute.SelectedValue}"));
                }
                else
                {
                    openingHoursBySalonId.FriClosed = true;
                }
                if (this.cbSaturday.Checked)
                {
                    openingHoursBySalonId.SatClosed = false;
                    openingHoursBySalonId.SatStart1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlSaturdayStartHour.SelectedValue}:{this.ddlSaturdayStartMinute.SelectedValue}"));
                    openingHoursBySalonId.SatEnd1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlSaturdayEndHour.SelectedValue}:{this.ddlSaturdayEndMinute.SelectedValue}"));
                }
                else
                {
                    openingHoursBySalonId.SatClosed = true;
                }
                if (this.cbSunday.Checked)
                {
                    openingHoursBySalonId.SunClosed = false;
                    openingHoursBySalonId.SunStart1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlSundayStartHour.SelectedValue}:{this.ddlSundayStartMinute.SelectedValue}"));
                    openingHoursBySalonId.SunEnd1 = new TimeSpan?(TimeSpan.Parse($"{this.ddlSundayEndHour.SelectedValue}:{this.ddlSundayEndMinute.SelectedValue}"));
                }
                else
                {
                    openingHoursBySalonId.SunClosed = true;
                }
                openingHoursBySalonId = IoC.Resolve<ISalonManager>().UpdateOpeningHours(openingHoursBySalonId);
            }
            if (this.PostedReturnURL != null)
            {
                base.Response.Redirect(this.PostedReturnURL, true);
            }
            else
            {
                string uRL = IFRMHelper.GetURL("settings.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
        }

        protected void cbFriday_CheckedChanged(object sender, EventArgs e)
        {
            bool flag = this.cbFriday.Checked;
            this.ddlFridayStartHour.Enabled = flag;
            this.ddlFridayStartMinute.Enabled = flag;
            this.ddlFridayEndHour.Enabled = flag;
            this.ddlFridayEndMinute.Enabled = flag;
        }

        protected void cbMonday_CheckedChanged(object sender, EventArgs e)
        {
            bool flag = this.cbMonday.Checked;
            this.ddlMondayStartHour.Enabled = flag;
            this.ddlMondayStartMinute.Enabled = flag;
            this.ddlMondayEndHour.Enabled = flag;
            this.ddlMondayEndMinute.Enabled = flag;
        }

        protected void cbSaturday_CheckedChanged(object sender, EventArgs e)
        {
            bool flag = this.cbSaturday.Checked;
            this.ddlSaturdayStartHour.Enabled = flag;
            this.ddlSaturdayStartMinute.Enabled = flag;
            this.ddlSaturdayEndHour.Enabled = flag;
            this.ddlSaturdayEndMinute.Enabled = flag;
        }

        protected void cbSunday_CheckedChanged(object sender, EventArgs e)
        {
            bool flag = this.cbSunday.Checked;
            this.ddlSundayStartHour.Enabled = flag;
            this.ddlSundayStartMinute.Enabled = flag;
            this.ddlSundayEndHour.Enabled = flag;
            this.ddlSundayEndMinute.Enabled = flag;
        }

        protected void cbThursday_CheckedChanged(object sender, EventArgs e)
        {
            bool flag = this.cbThursday.Checked;
            this.ddlThursdayStartHour.Enabled = flag;
            this.ddlThursdayStartMinute.Enabled = flag;
            this.ddlThursdayEndHour.Enabled = flag;
            this.ddlThursdayEndMinute.Enabled = flag;
        }

        protected void cbTuesday_CheckedChanged(object sender, EventArgs e)
        {
            bool flag = this.cbTuesday.Checked;
            this.ddlTuesdayStartHour.Enabled = flag;
            this.ddlTuesdayStartMinute.Enabled = flag;
            this.ddlTuesdayEndHour.Enabled = flag;
            this.ddlTuesdayEndMinute.Enabled = flag;
        }

        protected void cbWednesday_CheckedChanged(object sender, EventArgs e)
        {
            bool flag = this.cbWednesday.Checked;
            this.ddlWednesdayStartHour.Enabled = flag;
            this.ddlWednesdayStartMinute.Enabled = flag;
            this.ddlWednesdayEndHour.Enabled = flag;
            this.ddlWednesdayEndMinute.Enabled = flag;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.BindOpeningHours();
            }
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

