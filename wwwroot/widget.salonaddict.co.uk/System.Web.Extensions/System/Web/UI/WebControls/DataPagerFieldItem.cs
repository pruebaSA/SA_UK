namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataPagerFieldItem : Control, INonBindingContainer, INamingContainer
    {
        private DataPagerField _field;
        private DataPager _pager;

        public DataPagerFieldItem(DataPagerField field, DataPager pager)
        {
            this._field = field;
            this._pager = pager;
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            if (e is CommandEventArgs)
            {
                DataPagerFieldCommandEventArgs args = new DataPagerFieldCommandEventArgs(this, source, (CommandEventArgs) e);
                base.RaiseBubbleEvent(this, args);
                return true;
            }
            return false;
        }

        public DataPager Pager =>
            this._pager;

        public DataPagerField PagerField =>
            this._field;
    }
}

