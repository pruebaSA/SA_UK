namespace IFRAME.Modules
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ValidationProperty("Text")]
    public class DateTextBox : IFRMUserControl
    {
        protected ImageButton ibValue;
        protected CalendarExtender ibValueEx;
        protected TextBox txtValue;
        protected CalendarExtender txtValueEX;

        public event EventHandler TextChanged
        {
            add
            {
                this.txtValue.TextChanged += value;
            }
            remove
            {
                this.txtValue.TextChanged -= value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DateTime minimumSearchDate = IFRMHelper.GetMinimumSearchDate();
            this.Date = minimumSearchDate;
            this.txtValueEX.StartDate = new DateTime?(minimumSearchDate);
            this.ibValueEx.StartDate = new DateTime?(minimumSearchDate);
        }

        public void Page_Load(object sender, EventArgs e)
        {
        }

        public bool AutoPostback
        {
            get => 
                this.txtValue.AutoPostBack;
            set
            {
                this.txtValue.AutoPostBack = value;
            }
        }

        public override string ClientID =>
            this.txtValue.ClientID;

        public DateTime Date
        {
            get
            {
                DateTime time;
                if (!DateTime.TryParse(this.Text.Trim(), out time))
                {
                    return DateTime.Today;
                }
                return time;
            }
            set
            {
                this.Text = value.ToShortDateString();
            }
        }

        [Localizable(true)]
        public string Format
        {
            get => 
                this.txtValueEX.Format;
            set
            {
                this.txtValueEX.Format = value;
                this.ibValueEx.Format = value;
            }
        }

        public DateTime StartDate
        {
            set
            {
                this.txtValueEX.StartDate = new DateTime?(value);
                this.ibValueEx.StartDate = new DateTime?(value);
            }
        }

        public string Text
        {
            get => 
                this.txtValue.Text;
            set
            {
                this.txtValue.Text = value;
            }
        }
    }
}

