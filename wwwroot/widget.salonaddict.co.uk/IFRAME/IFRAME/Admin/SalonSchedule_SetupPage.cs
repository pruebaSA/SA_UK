namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class SalonSchedule_SetupPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected Literal ltrFridayHours;
        protected Literal ltrHeader;
        protected Literal ltrMondayHours;
        protected Literal ltrSaturdayHours;
        protected Literal ltrSundayHours;
        protected Literal ltrThursdayHours;
        protected Literal ltrTuesdayHours;
        protected Literal ltrWednesdayHours;
        protected Panel pnl;
        protected TextBox txtFriday;
        protected TextBox txtMonday;
        protected TextBox txtSaturday;
        protected TextBox txtSunday;
        protected TextBox txtThursday;
        protected TextBox txtTuesday;
        protected TextBox txtWednesday;
        protected UpdatePanel up;
        protected RegularExpressionValidator valFridayRegex;
        protected ValidatorCalloutExtender valFridayRegexEx;
        protected RegularExpressionValidator valMondayRegex;
        protected ValidatorCalloutExtender valMondayRegexEx;
        protected RegularExpressionValidator valSaturdayRegex;
        protected ValidatorCalloutExtender valSaturdayRegexEx;
        protected RegularExpressionValidator valSundayRegex;
        protected ValidatorCalloutExtender valSundayRegexEx;
        protected RegularExpressionValidator valThursdayRegex;
        protected ValidatorCalloutExtender valThursdayRegexEx;
        protected RegularExpressionValidator valTuesdayRegex;
        protected ValidatorCalloutExtender valTuesdayRegexEx;
        protected RegularExpressionValidator valWednesdayRegex;
        protected ValidatorCalloutExtender valWednesdayRegexEx;

        private void BindHours(SalonDB value)
        {
            OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(value.SalonId);
            if (IFRMHelper.IsSalonClosed(DayOfWeek.Monday, openingHoursBySalonId))
            {
                this.txtMonday.Visible = false;
                this.valMondayRegex.Visible = false;
                this.ltrMondayHours.Text = base.GetLocaleResourceString("ltrClosed.Text");
            }
            else
            {
                this.ltrMondayHours.Text = $"{new DateTime(openingHoursBySalonId.MonStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.MonEnd1.Value.Ticks).ToString("HH:mm")}";
            }
            if (IFRMHelper.IsSalonClosed(DayOfWeek.Tuesday, openingHoursBySalonId))
            {
                this.txtTuesday.Visible = false;
                this.valTuesdayRegex.Visible = false;
                this.ltrTuesdayHours.Text = base.GetLocaleResourceString("ltrClosed.Text");
            }
            else
            {
                this.ltrTuesdayHours.Text = $"{new DateTime(openingHoursBySalonId.TueStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.TueEnd1.Value.Ticks).ToString("HH:mm")}";
            }
            if (IFRMHelper.IsSalonClosed(DayOfWeek.Wednesday, openingHoursBySalonId))
            {
                this.txtWednesday.Visible = false;
                this.valWednesdayRegex.Visible = false;
                this.ltrWednesdayHours.Text = base.GetLocaleResourceString("ltrClosed.Text");
            }
            else
            {
                this.ltrWednesdayHours.Text = $"{new DateTime(openingHoursBySalonId.WedStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.WedEnd1.Value.Ticks).ToString("HH:mm")}";
            }
            if (IFRMHelper.IsSalonClosed(DayOfWeek.Thursday, openingHoursBySalonId))
            {
                this.txtThursday.Visible = false;
                this.valThursdayRegex.Visible = false;
                this.ltrThursdayHours.Text = base.GetLocaleResourceString("ltrClosed.Text");
            }
            else
            {
                this.ltrThursdayHours.Text = $"{new DateTime(openingHoursBySalonId.ThuStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.ThuEnd1.Value.Ticks).ToString("HH:mm")}";
            }
            if (IFRMHelper.IsSalonClosed(DayOfWeek.Friday, openingHoursBySalonId))
            {
                this.txtFriday.Visible = false;
                this.valFridayRegex.Visible = false;
                this.ltrFridayHours.Text = base.GetLocaleResourceString("ltrClosed.Text");
            }
            else
            {
                this.ltrFridayHours.Text = $"{new DateTime(openingHoursBySalonId.FriStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.FriEnd1.Value.Ticks).ToString("HH:mm")}";
            }
            if (IFRMHelper.IsSalonClosed(DayOfWeek.Saturday, openingHoursBySalonId))
            {
                this.txtSaturday.Visible = false;
                this.valSaturdayRegex.Visible = false;
                this.ltrSaturdayHours.Text = base.GetLocaleResourceString("ltrClosed.Text");
            }
            else
            {
                this.ltrSaturdayHours.Text = $"{new DateTime(openingHoursBySalonId.SatStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.SatEnd1.Value.Ticks).ToString("HH:mm")}";
            }
            if (IFRMHelper.IsSalonClosed(DayOfWeek.Sunday, openingHoursBySalonId))
            {
                this.txtSunday.Visible = false;
                this.valSundayRegex.Visible = false;
                this.ltrSundayHours.Text = base.GetLocaleResourceString("ltrClosed.Text");
            }
            else
            {
                this.ltrSundayHours.Text = $"{new DateTime(openingHoursBySalonId.SunStart1.Value.Ticks).ToString("HH:mm")} - {new DateTime(openingHoursBySalonId.SunEnd1.Value.Ticks).ToString("HH:mm")}";
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonsetup.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salonById.SalonId);
                List<ScheduleDB> schedulingBySalonId = IoC.Resolve<ISchedulingManager>().GetSchedulingBySalonId(salonById.SalonId);
                List<ScheduleDB> source = (from item in schedulingBySalonId
                    where ((DayOfWeek) item.WeekDayEnum) == DayOfWeek.Monday
                    select item).ToList<ScheduleDB>();
                List<ScheduleDB> list3 = (from item in schedulingBySalonId
                    where ((DayOfWeek) item.WeekDayEnum) == DayOfWeek.Tuesday
                    select item).ToList<ScheduleDB>();
                List<ScheduleDB> list4 = (from item in schedulingBySalonId
                    where ((DayOfWeek) item.WeekDayEnum) == DayOfWeek.Wednesday
                    select item).ToList<ScheduleDB>();
                List<ScheduleDB> list5 = (from item in schedulingBySalonId
                    where ((DayOfWeek) item.WeekDayEnum) == DayOfWeek.Thursday
                    select item).ToList<ScheduleDB>();
                List<ScheduleDB> list6 = (from item in schedulingBySalonId
                    where ((DayOfWeek) item.WeekDayEnum) == DayOfWeek.Friday
                    select item).ToList<ScheduleDB>();
                List<ScheduleDB> list7 = (from item in schedulingBySalonId
                    where ((DayOfWeek) item.WeekDayEnum) == DayOfWeek.Saturday
                    select item).ToList<ScheduleDB>();
                List<ScheduleDB> list8 = (from item in schedulingBySalonId
                    where ((DayOfWeek) item.WeekDayEnum) == DayOfWeek.Sunday
                    select item).ToList<ScheduleDB>();
                string[] strArray = this.txtMonday.Text.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length > 0)
                {
                    foreach (string str in strArray)
                    {
                        TimeSpan time = TimeSpan.Parse(str.Substring(0, 5));
                        int num = int.Parse(str.Substring(6, 1));
                        if (source.FirstOrDefault<ScheduleDB>(item => (item.Time == time)) == null)
                        {
                            ScheduleDB schedule = new ScheduleDB {
                                SalonId = salonById.SalonId,
                                ScheduleType = 1,
                                Time = new TimeSpan?(time),
                                Slots = new int?(num),
                                WeekDay = 2
                            };
                            schedule = IoC.Resolve<ISchedulingManager>().InsertSchedule(schedule);
                        }
                    }
                }
                strArray = this.txtTuesday.Text.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length > 0)
                {
                    foreach (string str2 in strArray)
                    {
                        TimeSpan time = TimeSpan.Parse(str2.Substring(0, 5));
                        int num2 = int.Parse(str2.Substring(6, 1));
                        if (list3.FirstOrDefault<ScheduleDB>(item => (item.Time == time)) == null)
                        {
                            ScheduleDB edb2 = new ScheduleDB {
                                SalonId = salonById.SalonId,
                                ScheduleType = 1,
                                Time = new TimeSpan?(time),
                                Slots = new int?(num2),
                                WeekDay = 3
                            };
                            edb2 = IoC.Resolve<ISchedulingManager>().InsertSchedule(edb2);
                        }
                    }
                }
                strArray = this.txtWednesday.Text.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length > 0)
                {
                    foreach (string str3 in strArray)
                    {
                        TimeSpan time = TimeSpan.Parse(str3.Substring(0, 5));
                        int num3 = int.Parse(str3.Substring(6, 1));
                        if (list4.FirstOrDefault<ScheduleDB>(item => (item.Time == time)) == null)
                        {
                            ScheduleDB edb3 = new ScheduleDB {
                                SalonId = salonById.SalonId,
                                ScheduleType = 1,
                                Time = new TimeSpan?(time),
                                Slots = new int?(num3),
                                WeekDay = 4
                            };
                            edb3 = IoC.Resolve<ISchedulingManager>().InsertSchedule(edb3);
                        }
                    }
                }
                strArray = this.txtThursday.Text.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length > 0)
                {
                    foreach (string str4 in strArray)
                    {
                        TimeSpan time = TimeSpan.Parse(str4.Substring(0, 5));
                        int num4 = int.Parse(str4.Substring(6, 1));
                        if (list5.FirstOrDefault<ScheduleDB>(item => (item.Time == time)) == null)
                        {
                            ScheduleDB edb4 = new ScheduleDB {
                                SalonId = salonById.SalonId,
                                ScheduleType = 1,
                                Time = new TimeSpan?(time),
                                Slots = new int?(num4),
                                WeekDay = 5
                            };
                            edb4 = IoC.Resolve<ISchedulingManager>().InsertSchedule(edb4);
                        }
                    }
                }
                strArray = this.txtFriday.Text.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length > 0)
                {
                    foreach (string str5 in strArray)
                    {
                        TimeSpan time = TimeSpan.Parse(str5.Substring(0, 5));
                        int num5 = int.Parse(str5.Substring(6, 1));
                        if (list6.FirstOrDefault<ScheduleDB>(item => (item.Time == time)) == null)
                        {
                            ScheduleDB edb5 = new ScheduleDB {
                                SalonId = salonById.SalonId,
                                ScheduleType = 1,
                                Time = new TimeSpan?(time),
                                Slots = new int?(num5),
                                WeekDay = 6
                            };
                            edb5 = IoC.Resolve<ISchedulingManager>().InsertSchedule(edb5);
                        }
                    }
                }
                strArray = this.txtSaturday.Text.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length > 0)
                {
                    foreach (string str6 in strArray)
                    {
                        TimeSpan time = TimeSpan.Parse(str6.Substring(0, 5));
                        int num6 = int.Parse(str6.Substring(6, 1));
                        if (list7.FirstOrDefault<ScheduleDB>(item => (item.Time == time)) == null)
                        {
                            ScheduleDB edb6 = new ScheduleDB {
                                SalonId = salonById.SalonId,
                                ScheduleType = 1,
                                Time = new TimeSpan?(time),
                                Slots = new int?(num6),
                                WeekDay = 7
                            };
                            edb6 = IoC.Resolve<ISchedulingManager>().InsertSchedule(edb6);
                        }
                    }
                }
                strArray = this.txtSunday.Text.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length > 0)
                {
                    foreach (string str7 in strArray)
                    {
                        TimeSpan time = TimeSpan.Parse(str7.Substring(0, 5));
                        int num7 = int.Parse(str7.Substring(6, 1));
                        if (list8.FirstOrDefault<ScheduleDB>(item => (item.Time == time)) == null)
                        {
                            ScheduleDB edb7 = new ScheduleDB {
                                SalonId = salonById.SalonId,
                                ScheduleType = 1,
                                Time = new TimeSpan?(time),
                                Slots = new int?(num7),
                                WeekDay = 1
                            };
                            edb7 = IoC.Resolve<ISchedulingManager>().InsertSchedule(edb7);
                        }
                    }
                }
            }
            string str8 = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonsetup.aspx", new string[] { str8 });
            base.Response.Redirect(uRL, true);
        }

        protected void cb_CheckedChanged(object sender, EventArgs e)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedSalonId == Guid.Empty)
            {
                string str = $"{"sid"}={this.PostedSalonId}";
                string uRL = IFRMHelper.GetURL("salonsetup.aspx", new string[] { str });
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string str3 = $"{"sid"}={this.PostedSalonId}";
                    string url = IFRMHelper.GetURL("salonsetup.aspx", new string[] { str3 });
                    base.Response.Redirect(url, true);
                }
                this.ltrHeader.Text = string.Format(base.GetLocaleResourceString("ltrHeader.Text"), salonById.Name);
                this.BindHours(salonById);
            }
        }

        public Guid PostedSalonId
        {
            get
            {
                string str = base.Request.QueryString["sid"];
                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        return new Guid(str);
                    }
                    catch
                    {
                    }
                }
                return Guid.Empty;
            }
        }
    }
}

