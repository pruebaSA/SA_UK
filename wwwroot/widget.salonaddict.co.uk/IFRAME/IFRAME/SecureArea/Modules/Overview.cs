namespace IFRAME.SecureArea.Modules
{
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class Overview : IFRMUserControl
    {
        protected Literal ltrAppointmentCount;
        protected Panel pnlAppointentBadge;
        protected System.Web.UI.Timer Timer;
        protected UpdatePanel up;

        private void BindAppointmentNotification(SalonDB value)
        {
            int salonUnreadAppointments = IoC.Resolve<IReportManager>().GetSalonUnreadAppointments(value.SalonId);
            this.AlertAppointmentCount = (salonUnreadAppointments > 0x63) ? 0x63 : salonUnreadAppointments;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Timer.Interval = IFRAME.Controllers.Settings.IAPPOINTMENT_MANAGER_APPOINTMENT_REFRESH_RATE;
            if (!this.Page.IsPostBack)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                this.BindAppointmentNotification(salon);
            }
        }

        protected void Timer_Tick(object sender, EventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            this.BindAppointmentNotification(salon);
        }

        private int AlertAppointmentCount
        {
            set
            {
                if (value > 0)
                {
                    this.pnlAppointentBadge.Visible = true;
                    this.ltrAppointmentCount.Text = value.ToString();
                }
            }
        }
    }
}

