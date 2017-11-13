namespace System.Web.UI.WebControls
{
    using System;
    using System.Web.UI;

    internal class ListViewContainer : Control, INamingContainer
    {
        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            if (e is ListViewCommandEventArgs)
            {
                base.RaiseBubbleEvent(source, e);
                return true;
            }
            if (e is CommandEventArgs)
            {
                ListViewCommandEventArgs args = new ListViewCommandEventArgs(null, source, (CommandEventArgs) e);
                base.RaiseBubbleEvent(this, args);
                return true;
            }
            return false;
        }
    }
}

