namespace IFRAME.Modules
{
    using IFRAME.Controllers;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.UI.WebControls;

    public class ViewTypeMenu : IFRMUserControl
    {
        protected HyperLink hl3DayView;
        protected HyperLink hlDayView;
        protected HyperLink hlWeekView;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.SelectedView == ViewType.Day)
            {
                this.hlDayView.CssClass = "selected";
                this.hlDayView.NavigateUrl = "javascript:void(0)";
                this.hlDayView.Attributes.Add("style", "cursor:default");
            }
            else if (this.SelectedView == ViewType.ThreeDay)
            {
                this.hl3DayView.CssClass = "selected";
                this.hl3DayView.NavigateUrl = "javascript:void(0)";
                this.hl3DayView.Attributes.Add("style", "cursor:default");
            }
            else if (this.SelectedView == ViewType.Week)
            {
                this.hlWeekView.CssClass = "selected";
                this.hlWeekView.NavigateUrl = "javascript:void(0)";
                this.hlWeekView.Attributes.Add("style", "cursor:default");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.hlDayView.NavigateUrl = this.Page.ResolveUrl($"~/dayview.aspx{base.Request.Url.Query}");
            this.hl3DayView.NavigateUrl = this.Page.ResolveUrl($"~/threedayview.aspx{base.Request.Url.Query}");
            this.hlWeekView.NavigateUrl = this.Page.ResolveUrl($"~/weekview.aspx{base.Request.Url.Query}");
        }

        public ViewType SelectedView { get; set; }

        public enum ViewType
        {
            Day,
            ThreeDay,
            Week
        }
    }
}

