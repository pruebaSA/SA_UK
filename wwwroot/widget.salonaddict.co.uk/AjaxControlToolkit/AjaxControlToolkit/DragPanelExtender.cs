namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxBitmap(typeof(DragPanelExtender), "DragPanel.DragPanel.ico"), Designer("AjaxControlToolkit.DragPanelDesigner, AjaxControlToolkit"), TargetControlType(typeof(WebControl)), ClientScriptResource("Sys.Extended.UI.FloatingBehavior", "DragPanel.FloatingBehavior.js"), RequiredScript(typeof(DragDropScripts))]
    public class DragPanelExtender : ExtenderControlBase
    {
        [ExtenderControlProperty, ClientPropertyName("handle"), IDReferenceProperty(typeof(WebControl)), ElementReference, RequiredProperty]
        public string DragHandleID
        {
            get
            {
                string propertyValue = base.GetPropertyValue<string>("DragHandleID", "");
                if (string.IsNullOrEmpty(propertyValue))
                {
                    propertyValue = base.TargetControlID;
                }
                return propertyValue;
            }
            set
            {
                base.SetPropertyValue<string>("DragHandleID", value);
            }
        }
    }
}

