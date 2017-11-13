namespace System.Web.UI.Design.WebControls.WebParts
{
    using System;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.Design.WebControls;
    using System.Web.UI.WebControls.WebParts;

    internal sealed class EditorZoneAutoFormat : BaseAutoFormat
    {
        internal const string PreviewControlID = "AutoFormatPreviewControl";

        public EditorZoneAutoFormat(DataRow schemeData) : base(schemeData)
        {
            base.Style.Height = 0x113;
            base.Style.Width = 300;
        }

        public override Control GetPreviewControl(Control runtimeControl)
        {
            EditorZone previewControl = (EditorZone) base.GetPreviewControl(runtimeControl);
            if ((previewControl != null) && (previewControl.EditorParts.Count == 0))
            {
                previewControl.ZoneTemplate = new AutoFormatTemplate();
            }
            previewControl.ID = "AutoFormatPreviewControl";
            return previewControl;
        }

        private sealed class AutoFormatTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                LayoutEditorPart child = new LayoutEditorPart {
                    ID = "LayoutEditorPart"
                };
                container.Controls.Add(child);
            }
        }
    }
}

