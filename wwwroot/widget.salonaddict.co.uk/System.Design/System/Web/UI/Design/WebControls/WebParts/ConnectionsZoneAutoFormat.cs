namespace System.Web.UI.Design.WebControls.WebParts
{
    using System;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.Design.WebControls;
    using System.Web.UI.WebControls.WebParts;

    internal sealed class ConnectionsZoneAutoFormat : BaseAutoFormat
    {
        internal const string PreviewControlID = "AutoFormatPreviewControl";

        public ConnectionsZoneAutoFormat(DataRow schemeData) : base(schemeData)
        {
            base.Style.Width = 0xe1;
        }

        public override Control GetPreviewControl(Control runtimeControl)
        {
            ConnectionsZone previewControl = (ConnectionsZone) base.GetPreviewControl(runtimeControl);
            previewControl.ID = "AutoFormatPreviewControl";
            return previewControl;
        }
    }
}

