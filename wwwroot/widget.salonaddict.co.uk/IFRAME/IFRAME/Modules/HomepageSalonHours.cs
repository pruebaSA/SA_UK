namespace IFRAME.Modules
{
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class HomepageSalonHours : IFRMUserControl
    {
        protected Literal ltrFriday;
        protected Literal ltrMonday;
        protected Literal ltrSaturday;
        protected Literal ltrSunday;
        protected Literal ltrThursday;
        protected Literal ltrTuesday;
        protected Literal ltrWednesday;

        protected void Page_Load(object sender, EventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            if (salon == null)
            {
                this.Visible = false;
            }
            else
            {
                OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salon.SalonId);
                if (!openingHoursBySalonId.ShowOnWidget)
                {
                    this.Visible = false;
                }
                else
                {
                    if (IFRMHelper.IsSalonClosed(DayOfWeek.Monday, openingHoursBySalonId))
                    {
                        this.ltrMonday.Text = base.GetLocaleResourceString("ltrClosed.Text");
                    }
                    else
                    {
                        this.ltrMonday.Text = $"{new DateTime(openingHoursBySalonId.MonStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.MonEnd1.Value.Ticks).ToString("HH:mm")}";
                    }
                    if (IFRMHelper.IsSalonClosed(DayOfWeek.Tuesday, openingHoursBySalonId))
                    {
                        this.ltrTuesday.Text = base.GetLocaleResourceString("ltrClosed.Text");
                    }
                    else
                    {
                        this.ltrTuesday.Text = $"{new DateTime(openingHoursBySalonId.TueStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.TueEnd1.Value.Ticks).ToString("HH:mm")}";
                    }
                    if (IFRMHelper.IsSalonClosed(DayOfWeek.Wednesday, openingHoursBySalonId))
                    {
                        this.ltrWednesday.Text = base.GetLocaleResourceString("ltrClosed.Text");
                    }
                    else
                    {
                        this.ltrWednesday.Text = $"{new DateTime(openingHoursBySalonId.WedStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.WedEnd1.Value.Ticks).ToString("HH:mm")}";
                    }
                    if (IFRMHelper.IsSalonClosed(DayOfWeek.Thursday, openingHoursBySalonId))
                    {
                        this.ltrThursday.Text = base.GetLocaleResourceString("ltrClosed.Text");
                    }
                    else
                    {
                        this.ltrThursday.Text = $"{new DateTime(openingHoursBySalonId.ThuStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.ThuEnd1.Value.Ticks).ToString("HH:mm")}";
                    }
                    if (IFRMHelper.IsSalonClosed(DayOfWeek.Friday, openingHoursBySalonId))
                    {
                        this.ltrFriday.Text = base.GetLocaleResourceString("ltrClosed.Text");
                    }
                    else
                    {
                        this.ltrFriday.Text = $"{new DateTime(openingHoursBySalonId.FriStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.FriEnd1.Value.Ticks).ToString("HH:mm")}";
                    }
                    if (IFRMHelper.IsSalonClosed(DayOfWeek.Saturday, openingHoursBySalonId))
                    {
                        this.ltrSaturday.Text = base.GetLocaleResourceString("ltrClosed.Text");
                    }
                    else
                    {
                        this.ltrSaturday.Text = $"{new DateTime(openingHoursBySalonId.SatStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.SatEnd1.Value.Ticks).ToString("HH:mm")}";
                    }
                    if (IFRMHelper.IsSalonClosed(DayOfWeek.Sunday, openingHoursBySalonId))
                    {
                        this.ltrSunday.Text = base.GetLocaleResourceString("ltrClosed.Text");
                    }
                    else
                    {
                        this.ltrSunday.Text = $"{new DateTime(openingHoursBySalonId.SunStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.SunEnd1.Value.Ticks).ToString("HH:mm")}";
                    }
                }
            }
        }
    }
}

